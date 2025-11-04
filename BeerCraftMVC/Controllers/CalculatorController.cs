using BeerCraftMVC.Models.ViewModels.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace BeerCraftMVC.Controllers
{
    public class CalculatorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new SrmCalculatorViewModel();
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Srm()
        {
            var viewModel = new SrmCalculatorViewModel();
            return View(viewModel);
        }
    }
}
