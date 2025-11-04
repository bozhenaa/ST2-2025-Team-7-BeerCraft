using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeerCraftMVC.Models.ViewModels.Calculator
{
    public class SrmCalculatorViewModel
    {
        [Required]
        [Display(Name = "Batch Volume (Liters)")]
        [Range(1, 200)]
        public double BatchVolume { get; set; } = 20;

        public List<SrmGrainInputModel> Grains { get; set; }

        public SrmCalculatorViewModel()
        {
            Grains = new List<SrmGrainInputModel>();
            Grains.Add(new SrmGrainInputModel()); 
        }
    }
}