using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models
{
    /// <summary>
    /// M:N връзка между Recipe и Ingredient
    /// </summary>
    public class RecipeIngredient
    {
        [Key]
        [Column(Order = 0)]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        [Required]
        public double Quantity { get; set; }  

        [MaxLength(20)]
        public string Unit { get; set; }  //  "g", "ml", "kg", "oz" и т.н.
    }
}
