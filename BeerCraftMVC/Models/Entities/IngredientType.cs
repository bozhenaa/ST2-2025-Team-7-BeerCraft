using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models.Entities
{

    /// <summary>
    /// вида на съставка (малц, хмел, дрожди, добавки)
    /// </summary>
    public class IngredientType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } //1:М (1 тип има много разновидности на съставки) 
    }
}
