using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models.ViewModels.Inventory
{
    public class AddItemViewModel
    {
        [Required]
        [Display(Name = "Ingredient")]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public double Quantity { get; set; }
        public SelectList? AvailableIngredients { get; set; }
    }
}
