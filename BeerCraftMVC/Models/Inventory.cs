using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models
{
    public class Inventory
    {
        [Key]
        [Column(Order = 0)]
        
        public int UserId { get; set; }
        public User User { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; }

        [Required]
        public double Quantity { get; set; }
        public DateTime BoughtAt { get; set; }
    }
}
