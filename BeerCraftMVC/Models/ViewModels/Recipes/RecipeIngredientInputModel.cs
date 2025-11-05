using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeIngredientInputModel
    {
        [Required(ErrorMessage = "Please select an ingredient")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an ingredient")]
        [Display(Name = "Ingredient")]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be positive")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Please select a unit")]
        [MaxLength(20)]
        public string Unit { get; set; } = "g";
    }
}
