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
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryIndexViewModel model)
        {
            var userId = GetUser();
            var addForm = model.AddForm;
            if(ModelState.IsValid)
            {
                var newItem = new Inventory
                {
                    UserId = userId,
                    IngredientId = addForm.IngredientId,
                    Quantity = addForm.Quantity,
                    BoughtAt = DateTime.UtcNow
                };
                await _inventoryRepository.AddInventoryItemAsync(newItem);
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
                AddForm = addForm
            };
            viewModel.AddForm.AvailableIngredients = await GetIngredientsListAsync();
            return View("Index", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Delete (int ingredientId)
        {
            var userId = GetUser();
            if(userId==0) return Unauthorized();
            await _inventoryRepository.DeleteAync(userId, ingredientId);
            return RedirectToAction(nameof(Index));
        }
    }
}
