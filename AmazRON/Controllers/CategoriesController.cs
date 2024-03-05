using AmazRON.Data;
using AmazRON.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmazRON.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(
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
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            ViewBag.Categories = categories;

            return View();
        }

        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);

            return View(category);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category cat)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                return RedirectToAction("Index");
            }

            else
            {
                return View(cat);
            }
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

            if (ModelState.IsValid)
            {

                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();
                TempData["message"] = "Categoria a fost modificata!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Include("Products")
                                             .Include("Products.Reviews")
                                             .Where(c => c.Id == id)
                                             .First();
            db.Categories.Remove(category);

            TempData["message"] = "Categoria a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
