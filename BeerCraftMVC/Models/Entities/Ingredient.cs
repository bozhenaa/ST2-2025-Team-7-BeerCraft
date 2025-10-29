using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models.Entities
{
    /// <summary>
    /// съставка от видовете (Pale Malt, Crystal Malt, Cascade Hops, Ale Yeast и т.н.)
    /// </summary>
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int IngredientTypeId { get; set; }

        [ForeignKey("IngredientTypeId")]
        public IngredientType IngredientType { get; set; } //външен ключ към типа съставката

        public ICollection <Inventory> UserInventories { get; set; }  = new List<Inventory>();//1:М
        public ICollection<IngredientCharacteristic> IngredientCharacteristics { get; set; } = new List<IngredientCharacteristic>();//M:M
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>(); //M:N

    }
}
