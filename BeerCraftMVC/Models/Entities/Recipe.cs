using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models.Entities
{
    /// <summary>
    /// основен модел на рецепта за бира
    /// </summary>
    public class Recipe
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("CreatedByUserId")]
        public int CreatedByUserId { get; set; } // 1:M Външен ключ към потребителя, който е създал рецептата
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>(); // M:N
        public ICollection<LikedRecipe> LikedByUsers { get; set; } = new List<LikedRecipe>(); // M:N






    }
}
