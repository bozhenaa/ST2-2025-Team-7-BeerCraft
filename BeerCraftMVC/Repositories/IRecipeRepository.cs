using BeerCraftMVC.Models.Entities;

namespace BeerCraftMVC.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe>GetByIdAsync (int id);
        Task<IEnumerable<Recipe>> GetAllAsync();
        Task AddAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);
        Task<IEnumerable<Recipe>> SearchAsync(string searchTerm);
        Task<IEnumerable<Ingredient>> GetAllIngredientsSimpleAsync();

    }
}
