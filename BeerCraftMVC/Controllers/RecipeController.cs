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
            var userId = GetUser();
            var likedIds = new HashSet<int>();
            if (userId > 0)
            {
                likedIds = await _context.LikedRecipes
                    .Where(lr => lr.UserId == userId)
                    .Select(lr => lr.RecipeId)
                    .ToHashSetAsync();
            }

            var recipeItems = recipesFromDb.Select(r => new RecipeIndexItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                AuthorUsername = authors.ContainsKey(r.CreatedByUserId)
                                    ? authors[r.CreatedByUserId]
                                    : "Unknown",
                CreatedAt = r.CreatedAt,
                IsLiked = likedIds.Contains(r.Id)
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
            for (int i = 0; i < viewModel.Ingredients.Count; i++)
{
            var ing = viewModel.Ingredients[i];
            if (ing.IngredientId == 0 || ing.Quantity <= 0 || string.IsNullOrEmpty(ing.Unit))
                ModelState.AddModelError($"Ingredients[{i}]", "Invalid ingredient data");
}

            if (!viewModel.Ingredients.Any())
            {
                ModelState.AddModelError("Ingredients", "Add at least one ingredient to create recipe");
            }
            if (viewModel.Ingredients != null)
            {
                var duplicated = viewModel.Ingredients
                    .GroupBy(i => i.IngredientId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if(duplicated.Any())
                {
                    ModelState.AddModelError("", "You have added the same ingredient multiple times. Combine or remove the duplicates");
                   
                }
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

            var userId = GetUser();
            bool isLiked = false;
            if (userId > 0)
            {
                isLiked = await _context.LikedRecipes.AnyAsync(lr => lr.RecipeId == id && lr.UserId == userId);
            }

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
                }).ToList(),
                IsLikedByCurrentUser = isLiked
            };

            return View(viewModel);
        }

        private async Task PopulateDropdowns(AddRecipeViewModel viewModel)
        {
            var ingredientsData = await _recipeRepository.GetAllIngredientsSimpleAsync();
            viewModel.AvailableIngredients = new SelectList(ingredientsData, "Id", "Name");

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


        [HttpGet]
        public async Task<IActionResult> SearchSuggestion(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.Recipes
                .Where(r => r.Name.ToLower().Contains(term.ToLower()))
                .OrderBy(r => r.Name)
                .Take(5)
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name
                })
                .ToListAsync();

            return Json(suggestions);
        }



        [HttpPost]
        public async Task<IActionResult> LikeRecipe(int id)
        {
            var userId = GetUser();
            if (userId == 0)
            {
                return Unauthorized();
            }
            var recipeId = id;
            var isLiked = await _context.LikedRecipes
                .FirstOrDefaultAsync(lr => lr.RecipeId == recipeId && lr.UserId == userId);
            if(isLiked==null)
            {
                var newLike = new LikedRecipe
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    LikedAt = DateTime.Now,
                };
                _context.LikedRecipes.Add(newLike);
            }
            else
            {
                _context.LikedRecipes.Remove(isLiked);  
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = recipeId });
        }

    }
}
