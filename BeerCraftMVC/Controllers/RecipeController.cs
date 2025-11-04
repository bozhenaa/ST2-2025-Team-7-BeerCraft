using BeerCraftMVC.Data;
using BeerCraftMVC.Models.Entities;
using BeerCraftMVC.Models.ViewModels.Recipes;
using BeerCraftMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeerCraftMVC.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly BeerCraftDbContext _context;

        public RecipeController(IRecipeRepository recipeRepository, BeerCraftDbContext context)
        {
            _recipeRepository = recipeRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var recipesFromDb = await _recipeRepository.SearchAsync(searchTerm);

            var authorIds = recipesFromDb
                                .Select(r => r.CreatedByUserId)
                                .Distinct()
                                .ToList();

            var authors = await _context.Users
                                        .Where(u => authorIds.Contains(u.Id))
                                        .ToDictionaryAsync(u => u.Id, u => u.Username);

            var recipeItems = recipesFromDb.Select(r => new RecipeIndexItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                AuthorUsername = authors.ContainsKey(r.CreatedByUserId)
                                    ? authors[r.CreatedByUserId]
                                    : "Unknown",
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
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddRecipeViewModel viewModel)
        {
            viewModel.Ingredients.RemoveAll(i => i.IngredientId == 0 || i.Quantity <= 0 || string.IsNullOrEmpty(i.Unit));

            if (!viewModel.Ingredients.Any())
            {
                ModelState.AddModelError("Ingredients", "Add at least one ingredient to create recipe");
            }

            if (ModelState.IsValid)
            {
                var userId = GetUser();
                if (userId == 0) return Unauthorized();

                var recipe = new Recipe
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    RecipeIngredients = viewModel.Ingredients.Select(i => new RecipeIngredient
                    {
                        IngredientId = i.IngredientId,
                        Quantity = i.Quantity,
                        Unit = i.Unit
                    }).ToList()
                };

                await _recipeRepository.AddAsync(recipe);
                return RedirectToAction("Details", new { id = recipe.Id });
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var author = await _context.Users.FindAsync(recipe.CreatedByUserId);
            var authorUsername = author?.Username ?? "Unknown";

            var viewModel = new RecipeDetailViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                AuthorUsername = authorUsername,
                CreatedAt = recipe.CreatedAt,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientViewModel
                {
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit,
                    IngredientTypeName = ri.Ingredient.IngredientType?.Name ?? "N/A"
                }).ToList()
            };

            return View(viewModel);
        }

        private async Task PopulateDropdowns(AddRecipeViewModel viewModel)
        {
            var ingredientsData = await _recipeRepository.GetAllIngredientsSimpleAsync();
            viewModel.AvailableIngredients = new SelectList(ingredientsData, "Id", "Name");

            viewModel.AvailableUnits = new SelectList(new List<string> { "g", "kg", "ml", "l", "oz", "lb", "packet", "tsp", "tbsp" });
        }

        private int GetUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdString, out int userId);
            return userId;
        }
    
    public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUser();
            if (userId == 0) return Unauthorized();

            var recipe = await _recipeRepository.GetByIdAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            if (recipe.CreatedByUserId != userId)
            {
                return Forbid();
            }

            await _recipeRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}