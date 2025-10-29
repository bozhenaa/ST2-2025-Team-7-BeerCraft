namespace BeerCraftMVC.Models.ViewModels.Inventory
{
    public class InventoryItemViewModel
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public double Quantity { get; set; }
        public DateTime BoughtAt { get; set; }
        public string Unit { get; set; }
    }
}
