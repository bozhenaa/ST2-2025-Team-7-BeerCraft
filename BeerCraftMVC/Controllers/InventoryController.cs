using BeerCraftMVC.Data;
using BeerCraftMVC.Models.Entities;
using BeerCraftMVC.Models.ViewModels.Inventory;
using BeerCraftMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeerCraftMVC.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly BeerCraftDbContext _context;
        public InventoryController(IInventoryRepository inventoryRepo, BeerCraftDbContext context)
        {
            _inventoryRepository = inventoryRepo;
            _context = context;
        }

        private int GetUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdString, out int userId);
            return userId;
        }

        private async Task<SelectList> GetIngredientTypesSelctListAsync()
        {
            var types = await _context.IngredientTypes
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .AsNoTracking()
                .ToListAsync();
            return new SelectList(types, "Id", "Name");
        }

        public async Task<SelectList> GetIngredientsListAsync()
        {
            var ingredients = await _context.Ingredients
                .OrderBy(i => i.Name)
                .Select(i => new { i.Id, i.Name })
                .AsNoTracking()
                .ToListAsync();
            return new SelectList(ingredients, "Id", "Name");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUser();
            var inventoryItems = await _inventoryRepository.GetUserInventoryItemsAsync(userId);

            var viewModel = new InventoryIndexViewModel
            {
                InventoryItems = inventoryItems.Select(i => new InventoryItemViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.Ingredient.Name,
                    Quantity = i.Quantity
                }).ToList(),

                AddForm = new AddItemViewModel
                {
                    AvailableIngredients = await GetIngredientsListAsync()
                },
                AvailableIngredientTypes = await GetIngredientTypesSelctListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryIndexViewModel model)
        {
            var userId = GetUser();
            var addForm = model.AddForm;
            if (addForm.IngredientId == -1)
            {
                if (string.IsNullOrWhiteSpace(addForm.NewIngredientName))
                {
                    ModelState.AddModelError("AddForm.NewIngredientName", "New ingredient name is required.");
                }
                if (!addForm.NewIngredientTypeId.HasValue || addForm.NewIngredientTypeId.Value <= 0)
                {
                    ModelState.AddModelError("AddForm.NewIngredientTypeId", "Ingredient type is required.");
                }
            }
            if (ModelState.IsValid)
            {
                int finalIngredientId;
                if (addForm.IngredientId == -1)
                {
                    var existingIngredient = await _context.Ingredients
                        .FirstOrDefaultAsync(i => i.Name.ToLower() == addForm.NewIngredientName.Trim().ToLower());
                    if (existingIngredient != null)
                    {
                        finalIngredientId = existingIngredient.Id;
                    }

                    else
                    {
                        var newIngredient = new Ingredient
                        {
                            Name = addForm.NewIngredientName.Trim(),
                            IngredientTypeId = addForm.NewIngredientTypeId.Value
                        };
                        _context.Ingredients.Add(newIngredient);
                        await _context.SaveChangesAsync();
                        finalIngredientId = newIngredient.Id;
                    }
                }
                else
                {
                    finalIngredientId = addForm.IngredientId;
                }
                var inventoryItem = new Inventory
                {
                    UserId = userId,
                    IngredientId = finalIngredientId,
                    Quantity = addForm.Quantity,
                    BoughtAt = DateTime.UtcNow
                };
                await _inventoryRepository.AddInventoryItemAsync(inventoryItem);
                return RedirectToAction(nameof(Index));
            }
            var items = await _inventoryRepository.GetUserInventoryItemsAsync(userId);
            var viewModel = new InventoryIndexViewModel
            {
                InventoryItems = items.Select(i => new InventoryItemViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.Ingredient.Name,
                    Quantity = i.Quantity
                }).ToList(),
                AddForm = addForm,
                AvailableIngredientTypes = await GetIngredientTypesSelctListAsync()
            };
            viewModel.AddForm.AvailableIngredients = await GetIngredientsListAsync();
            return View("Index", viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ingredientId)
        {
            var userId = GetUser();
            if (userId == 0) return Unauthorized();
            await _inventoryRepository.DeleteAync(userId, ingredientId);
            return RedirectToAction(nameof(Index));
        }
    }
}
