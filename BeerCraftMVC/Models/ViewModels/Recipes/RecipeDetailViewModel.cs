namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorUsername { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<RecipeIngredientViewModel> Ingredients { get; set; } = new List<RecipeIngredientViewModel>();
    }
}
