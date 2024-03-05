using AmazRON.Data;
using AmazRON.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmazRON.Controllers
{
    [Authorize]
    public class UserProductsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UserProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            var cosUser = db.UserProducts
                    .Include("Product")
                    .Include("User")
                    .Where(up => up.UserId == _userManager.GetUserId(User));
            

            ViewBag.Cos = cosUser;

            return View();
        }

        public ActionResult Edit(int id)
        {

            var userProdus = db.UserProducts
                    .Include("Product")
                    .Include("User")
                    .Where(up =>
                        up.UserId == _userManager.GetUserId(User)
                        && up.Id == id
                    ).First();


            if(userProdus == null)
            {
                TempData["message"] = "Produsul nu exista in cosul tau";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }


           

            return View(userProdus);
        }


        [HttpPost]
        public ActionResult Edit(int id, UserProduct requestUserProduct)
        {
            var userProduct = db.UserProducts
                .Include("Product")
                .Where(up =>
                    up.Id == id &&
                    up.UserId == _userManager.GetUserId(User))
                .First();

            if (ModelState.IsValid)
            {
                if (requestUserProduct.Bucati < 0)
                    requestUserProduct.Bucati = 0;
                var ramas = userProduct.Bucati - requestUserProduct.Bucati;
                userProduct.Bucati -= ramas;
                userProduct.Product.Stoc += ramas;

                
                if(userProduct.Product.Stoc >= 0)
                {
                    if (userProduct.Bucati <= 0)
                    {
                        db.UserProducts.Remove(userProduct);
                    }
                    db.SaveChanges();
                    TempData["message"] = "Bucatile au fost modificatate!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu sunt atatea bucati pe stoc!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
                
            }
            else
            {
                return View(requestUserProduct);
            }
        }

        [HttpPost]
        public ActionResult New(UserProduct userProdusNew)
        {
            var produs = db.Products.Find(userProdusNew.ProductId);
            if(produs == null)
            {
                TempData["message"] = "Nu exista produsul!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
            if (produs.Stoc < 1)
            {
                TempData["message"] = "Produsul nu mai este pe stoc";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            var userProdus = db.UserProducts
                .Include("Product")
                .Where(up => up.ProductId == userProdusNew.ProductId && up.UserId == _userManager.GetUserId(User))
                .FirstOrDefault();
            if (userProdus == null)
            {
                userProdusNew.Bucati = 1;
                userProdusNew.UserId = _userManager.GetUserId(User);
                db.UserProducts.Add(userProdusNew);
            }
            else
            {
                userProdus.Bucati += 1;
            }
            produs.Stoc--;
            db.SaveChanges();

            TempData["message"] = "Produs adaugat in cos!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var userProduct = db.UserProducts
                .Include("Product")
                .Where(up =>
                    up.Id == id &&
                    up.UserId == _userManager.GetUserId(User))
                .First();
            var produs = db.Products.Find(userProduct.ProductId);
            if (produs == null)
            {
                TempData["message"] = "Nu exista produsul!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
            if (userProduct == null)
            {
                TempData["message"] = "Nu exista produsul in cosul tau!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            produs.Stoc += userProduct.Bucati;
            db.UserProducts.Remove(userProduct);
            db.SaveChanges();


            return RedirectToAction("Index");
        }



        }
}
