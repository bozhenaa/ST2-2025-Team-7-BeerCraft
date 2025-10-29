namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeIngredientViewModel
    {
            public int IngredientId { get; set; }
            public string IngredientName { get; set; }
            public double Quantity { get; set; }
            public string Unit { get; set; }
            public string IngredientTypeName { get; set; } // Напр. "Малц", "Хмел"
    }
}
