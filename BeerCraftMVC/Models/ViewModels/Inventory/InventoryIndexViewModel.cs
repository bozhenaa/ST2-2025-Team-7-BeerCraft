using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeerCraftMVC.Models.ViewModels.Inventory
{
    public class InventoryIndexViewModel
    {
        public List<InventoryItemViewModel> InventoryItems { get; set; } = new List<InventoryItemViewModel>();
        [BindProperty]
        public AddItemViewModel AddForm { get; set; }

        public SelectList? AvailableIngredientTypes { get; set; }
    }
}
