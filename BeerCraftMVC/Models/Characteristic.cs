using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models
{
    /// <summary>
    /// характеристика за всяка съставка (цвят, горчивина, наситеност и т.н.) 
    /// </summary>
    public class Characteristic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<IngredientCharacteristic> IngredientCharacteristics { get; set; } = new List<IngredientCharacteristic>(); //
    }
}
