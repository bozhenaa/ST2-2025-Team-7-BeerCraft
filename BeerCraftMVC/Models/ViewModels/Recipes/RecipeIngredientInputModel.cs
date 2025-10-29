using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeerCraftMVC.Models.ViewModels.Recipes
{
    public class RecipeIngredientInputModel
    {
        [Required]
        [Display(Name ="Ingredient")]
        public int IngredientId { get; set; }

        [Display(Name = "New Ingredient Name")]
        [MaxLength(100)] 
        public string? NewIngredientName { get; set; } 

        [Display(Name = "Ingredient Type")]
        public int? NewIngredientTypeId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be positive")]
        public double Quantity { get; set; }
        [Required]
        [MaxLength(20)]
        public string Unit { get; set; }


    }
}
