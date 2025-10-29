using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models.Entities
{
    /// <summary>
    /// свързваща таблица за съставка и характеристиките ѝ 
    /// </summary>
    public class IngredientCharacteristic
    {
        //FK към Ingredient
        [Key]
        [Column (Order= 0)]
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        //FK към Characteristic
        [Key]
        [Column(Order = 1)]
        public int CharacteristicId { get; set; }
        public Characteristic Characteristics { get; set; }
    }
}
