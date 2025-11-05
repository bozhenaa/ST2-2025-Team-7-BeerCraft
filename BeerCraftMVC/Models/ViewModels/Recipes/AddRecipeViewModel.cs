using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        [ValidateNever]
        public SelectList AvailableIngredients { get; set; }

        [ValidateNever]
        public List<string> AvailableUnits { get; set; } =
            new List<string> { "g", "kg", "ml", "l", "oz", "lb", "packet", "tsp", "tbsp" };

        public AddRecipeViewModel()
        {
          
        }
    }
}
