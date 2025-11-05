using Microsoft.AspNetCore.Mvc;

namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeIndexViewModel
    {
        public List<RecipeIndexItemViewModel> Recipes { get; set; } = new List<RecipeIndexItemViewModel>();

    }
}
