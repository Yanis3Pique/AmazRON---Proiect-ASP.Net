using System.Diagnostics;
using AmazRON.Models;
using Microsoft.AspNetCore.Mvc;

namespace AmazRON.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Products");
        }

    }
}