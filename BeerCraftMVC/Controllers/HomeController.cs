using BeerCraftMVC.Models;
using BeerCraftMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BeerCraftMVC.Controllers
{
    public class HomeController : Controller
    {  
        public HomeController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
