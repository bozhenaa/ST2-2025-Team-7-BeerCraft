using BeerCraftMVC.Models.ViewModels.AI;
using BeerCraftMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeerCraftMVC.Controllers
{
    /// <summary>
    /// контролер за управлението на асистента чатбот
    /// </summary>
    public class AIController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            //зареждане View-то за AI, вижда се чат интерфейса
            return View();
        }

       
        /// <summary>
        /// POST Web API - асинхронно извикване от JavaScript, не презарежда цялата страница, а връща данни
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody]ChatPromptViewModel model)
        {
            //валидация дали model е нул и промпта в него е празен
            if(model == null || string.IsNullOrWhiteSpace(model.Prompt))
            {
                //връща съответните грешки
                return BadRequest("Prompt cannot be empty.");
            }
            //викаме услугата и се взема промпта от ViewModel-a
            string response = await LLMService.Instance.GetSuggestionAsync(model.Prompt);

            //връщаме резултат, създаваме анонимен обект, който ще се превърне в JSON, а JavaScript получава response
            return Ok(new {response=response});
        }
    }
}
