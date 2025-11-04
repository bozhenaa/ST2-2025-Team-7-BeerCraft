using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeerCraftMVC.Models.ViewModels.Calculator
{
    public class SrmGrainInputModel
    {
        [Required]
        [Display(Name = "Grain Weight (kg)")]
        [Range(0.01, 100)]
        public double Weight { get; set; } = 1;

        [Required]
        [Display(Name = "Grain Color (°L)")]
        [Range(1, 500)]
        public double Lovibond { get; set; } = 10;
    }
}