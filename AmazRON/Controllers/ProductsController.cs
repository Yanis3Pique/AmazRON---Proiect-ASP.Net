using AmazRON.Data;
using AmazRON.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmazRON.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        //[Authorize(Roles = "Administrator,Colaborator,UserInregistrat")]
        public IActionResult Index()
        {
            var products = db.Products.Include("Category")
                                      .Include("User")
                                      .Where(pr => pr.Accepted || 
                                      (pr.Pending && User.IsInRole("Administrator")) ||
                                      (pr.Pending && User.IsInRole("Colaborator") && 
                                      pr.UserId == _userManager.GetUserId(User)));

            var sortProperty = Convert.ToString(HttpContext.Request.Query["sort"]);
            var sortOrder = Convert.ToString(HttpContext.Request.Query["order"]);

            if(sortProperty == null || sortProperty == "")
            {
                sortProperty = "Price";
            }

            if (sortOrder == null || sortOrder == "")
            {
                sortOrder = "asc";
            }

            ViewBag.sortProp = sortProperty;
            ViewBag.orderProp = sortOrder;

            //AFISEAZA ERORI
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // SEARCH
            var search = Convert.ToString(HttpContext.Request.Query["search"]);
            
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                // Cautare in produs (Title, Content si CategoryName)
                List<int> productIds = db.Products.Where(at => at.Title.Contains(search) ||
                                                         at.Description.Contains(search) ||
                                                         at.Category.CategoryName.Contains(search))
                                                  .Select(a => a.Id).ToList();

                // Cautare in reviews (Content)

                List<int> productIdsOfReviewsWithSearchString = db.Reviews.Where(c => c.Content.Contains(search))
                                                                          .Select(c => (int)c.ProductId).ToList();
                List<int> mergedIds = productIds.Union(productIdsOfReviewsWithSearchString).ToList();

                products = db.Products.Where(product =>
                                mergedIds.Contains(product.Id))
                                .Include("Category")
                                .Include("User")
                                .Where(pr => pr.Accepted ||
                                             User.IsInRole("Administrator") ||
                                             (User.IsInRole("Colaborator")
                                                && pr.UserId == _userManager.GetUserId(User)))
                                .OrderBy(a => a.Date);
            }

            ViewBag.SearchString = search;
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            }

            //Sortare
            switch (sortProperty)
            {
                case "Price":
                    if (sortOrder == "desc")
                    {
                        products = products.OrderByDescending(a => a.Price);
                    }
                    else
                    {
                        products = products.OrderBy(a => a.Price);
                    }
                    break;
                case "Rating":
                    if (sortOrder == "desc")
                    {
                        products = products.OrderByDescending(a => a.Rating);
                    }
                    else
                    {
                        products = products.OrderBy(a => a.Rating);
                    }
                    break;
                default:
                    products = products.OrderBy(a => a.Date);
                    break;
            }

            //PAGINATIE
            int _perPage = 4;
            int totalItems = products.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);


            ViewBag.Products = paginatedProducts;

            var listaOrdonare = new List<SelectListItem>();

            listaOrdonare.Add(new SelectListItem
            {
                Value = "asc",
                Text = "Crescător"
            });
            listaOrdonare.Add(new SelectListItem
            {
                Value = "desc",
                Text = "Descrescător"
            });

            var listaSortare = new List<SelectListItem>();

            listaSortare.Add(new SelectListItem
            {
                Value = "Price",
                Text = "Pret"
            });
            listaSortare.Add(new SelectListItem
            {
                Value = "Rating",
                Text = "Rating"
            });

            ViewBag.listaOrdonare = listaOrdonare;
            ViewBag.listaSortare = listaSortare;


            return View();
        }

        //[Authorize(Roles = "Administrator,Colaborator,UserInregistrat")]
        public IActionResult Show(int id)
        {
            Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Include("Reviews")
                                         .Include("Reviews.User")
                                         .Where(art => art.Id == id)
                                         .First();
            if (!product.Accepted &&
                !(User.IsInRole("Administrator")
                || (User.IsInRole("Colaborator")
                    && product.UserId == _userManager.GetUserId(User))))
            {
                
                TempData["message"] = "Produsul nu este aprobat!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(product);
        }


        [HttpPost]
        [Authorize(Roles = "Administrator,Colaborator,UserInregistrat,UserNeinregistrat")]
        public IActionResult Show([FromForm] Review review)
        {
            var prod = db.Products.Find(review.ProductId);
            if (!prod.Accepted)
            {
                TempData["message"] = "Produsul nu este aprobat, nu poti adauga un review!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            review.Date = DateTime.Now;
            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();

                var reviews = db.Reviews.Where(r => r.ProductId == review.ProductId).ToList();
                double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                prod.Rating = averageRating;
                db.SaveChanges();

                return Redirect("/Products/Show/" + review.ProductId);
            }

            else
            {
                Product art = db.Products.Include("Category")
                                         .Include("User")
                                         .Include("Reviews")
                                         .Include("Reviews.User")
                                         .Where(art => art.Id == review.ProductId)
                                         .First();

                SetAccessRights();

                return View(art);
            }
        }

        [Authorize(Roles = "Colaborator")]
        public IActionResult New()
        {
            Product product = new Product();

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator")]
        [HttpPost]
        public IActionResult New(Product product)
        {
            var sanitizer = new HtmlSanitizer();

            product.Date = DateTime.Now;

            product.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                product.Description = sanitizer.Sanitize(product.Description);

                product.Description = (product.Description);
                product.Accepted = false;
                product.Pending = true;

                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        [Authorize(Roles = "Administrator,Colaborator")]
        public IActionResult Edit(int id)
        {

            Product product = db.Products.Include("Category")
                                        .Where(art => art.Id == id)
                                        .First();

            product.Categ = GetAllCategories();

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                return View(product);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Colaborator")]
        public IActionResult Edit(int id, Product requestedProduct)
        {
            var sanitizer = new HtmlSanitizer();

            Product product = db.Products.Find(id);

            if (ModelState.IsValid)
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
                {
                    product.Title = requestedProduct.Title;

                    requestedProduct.Description = sanitizer.Sanitize(requestedProduct.Description);

                    product.Price = requestedProduct.Price;
                    product.Stoc = requestedProduct.Stoc;
                    product.Photo = requestedProduct.Photo;
                    product.Description = requestedProduct.Description;
                    product.CategoryId = requestedProduct.CategoryId;
                    TempData["message"] = "Produsul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestedProduct.Categ = GetAllCategories();
                return View(requestedProduct);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Colaborator")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Include("Reviews")
                                         .Where(art => art.Id == id)
                                         .First();

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Accept(int id)
        {
            var prod = db.Products.Find(id);
            if (prod != null)
            {
                prod.Accepted = true;
                prod.Pending = false;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Refuz(int id)
        {
            var prod = db.Products.Find(id);
            if (prod != null)
            {
                prod.Accepted = false;
                prod.Pending = false;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Colaborator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Administrator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            return selectList;
        }
    }
}
