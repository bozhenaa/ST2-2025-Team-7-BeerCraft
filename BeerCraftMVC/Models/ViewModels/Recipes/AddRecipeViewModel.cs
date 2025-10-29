using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class AddRecipeViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public List<RecipeIngredientInputModel> Ingredients { get; set; } = new List<RecipeIngredientInputModel>();
        public SelectList AvailableIngredients { get; set; }

        public SelectList AvailableIngredientTypes { get; set; }
        public SelectList AvailableUnits { get; set; } = new SelectList(new List<string> { "g", "kg", "ml", "l", "oz", "lb" });

        public AddRecipeViewModel()
        {
            Ingredients.Add(new RecipeIngredientInputModel());
        }


    }
}
