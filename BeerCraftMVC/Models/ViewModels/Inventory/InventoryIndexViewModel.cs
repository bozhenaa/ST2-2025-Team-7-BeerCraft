using Microsoft.AspNetCore.Mvc;

namespace BeerCraftMVC.Models.ViewModels.Inventory
{
    public class InventoryIndexViewModel
    {
        public List<InventoryItemViewModel> InventoryItems { get; set; } = new List<InventoryItemViewModel>();
        [BindProperty]
        public AddItemViewModel AddForm { get; set; }
    }
}
