using AmazRON.Data;
using AmazRON.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmazRON.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ReviewsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Colaborator,UserInregistrat")]
        public IActionResult Delete(int id)
        {
            Review review = db.Reviews.First(r => r.Id == id);

            if(review.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                var prod = db.Products.Find(review.ProductId);

                db.Reviews.Remove(review);
                db.SaveChanges();

                var reviews = db.Reviews.Where(r => r.ProductId == review.ProductId).ToList();
                double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                prod.Rating = averageRating;
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti review-ul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }

        [Authorize(Roles = "Administrator,Colaborator,UserInregistrat")]
        public IActionResult Edit(int id)
        {
            Review review = db.Reviews.First(r => r.Id == id);

            if (review.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                return View(review);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati review-ul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Colaborator,UserInregistrat")]
        public IActionResult Edit(int id, Review requestReview)
        {
            Review review = db.Reviews.First(r => r.Id == id);

            if (review.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    review.Content = requestReview.Content;

                    review.Rating = requestReview.Rating;

                    db.SaveChanges();


                    var prod = db.Products.Find(review.ProductId);

                    var reviews = db.Reviews.Where(r => r.ProductId == review.ProductId).ToList();
                    double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                    prod.Rating = averageRating;
                    db.SaveChanges();

                    return Redirect("/Products/Show/" + review.ProductId);
                }
                else
                {
                    return View(requestReview);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }
    }
}
