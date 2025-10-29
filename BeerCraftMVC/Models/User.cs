using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerCraftMVC.Models
{
    /// <summary>
    /// DTO for User Profile
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [PasswordPropertyText]
        public string HashedPassword { get; set; }

        public ICollection<Inventory> Inventory { get; set; } = new List<Inventory>();// 1:М
        public ICollection<LikedRecipe> LikedRecipes { get; set; } = new List<LikedRecipe>(); //1:М
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>(); // 1:M 

    }
}

