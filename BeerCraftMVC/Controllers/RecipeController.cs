using BeerCraftMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using BeerCraftMVC.Models.ViewModels.Recipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeerCraftMVC.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        public RecipeController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        [HttpGet]

        public async Task<IActionResult> Index()
        {

            var recipesFromDb = await _recipeRepository.GetAllAsync();
            var recipeItems = recipesFromDb.Select(r => new RecipeIndexItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            }).ToList();
            var model = new RecipeIndexViewModel
            {
                Recipes = recipeItems
            };
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new AddRecipeViewModel();
            await PopulateAvailableIngredients(viewModel);
            return View(viewModel);
        }
        private async Task PopulateAvailableIngredients(AddRecipeViewModel viewModel)
        {
            var ingredientsData = await _recipeRepository.GetAllIngredientsSimpleAsync();
            viewModel.AvailableIngredients = new SelectList(ingredientsData, "Id", "Name");
        }
    }
}
