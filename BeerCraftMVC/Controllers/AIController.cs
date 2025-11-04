using BeerCraftMVC.Models.ViewModels.AI;
using BeerCraftMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeerCraftMVC.Controllers
{
    public class AIController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetSuggestion(string prompt)
        {
            if(string.IsNullOrWhiteSpace(prompt))
            {
                TempData["Error"] = "Prompt cannot be empty.";
                return RedirectToAction(nameof(Index));
            }
            string response = await LLMService.Instance.GetSuggestionAsync(prompt);
            TempData["Prompt"] = prompt;
            TempData["Response"] = response;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody]ChatPromptViewModel model)
        {
            if(model == null || string.IsNullOrWhiteSpace(model.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }
            string response = await LLMService.Instance.GetSuggestionAsync(model.Prompt);
            return Ok(new {response=response});
        }
    }
}
