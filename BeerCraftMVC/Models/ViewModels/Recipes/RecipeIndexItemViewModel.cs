namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeIndexItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLiked { get; set; }

    }
}
