using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models.Entities
{
    /// <summary>
    /// харесани рецепти
    /// </summary>
    public class LikedRecipe
    {
        [Key]
        [Column(Order = 0)]
        public int UserId { get; set; }
        public User User { get; set; }

        [Key]
        [Column(Order = 1)]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
