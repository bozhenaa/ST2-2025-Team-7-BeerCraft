using BeerCraftMVC.Data;
using BeerCraftMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerCraftMVC.Repositories
{
    public class RecipeRepository :IRecipeRepository
    {
        private readonly BeerCraftDbContext _context;

        public RecipeRepository(BeerCraftDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe> GetByIdAsync(int id)
        {
            return await _context.Recipes.FindAsync(id);
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var recipe = await GetByIdAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await _context.Recipes.ToListAsync();
        }
      
        public async Task UpdateAsync(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task <IEnumerable<Recipe>> SearchAsync(string searchTerm)
        {
            var query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string lowerSearchTerm = searchTerm.ToLower(); 
                query = query.Where(r =>
                    r.Name.ToLower().Contains(lowerSearchTerm) ||
                    (r.Description != null && r.Description.ToLower().Contains(lowerSearchTerm)));
            }

            return await query
                        .OrderByDescending(r => r.CreatedAt)
                        .AsNoTracking() 
                        .ToListAsync();
            
        }
        public async Task<IEnumerable<Ingredient>> GetAllIngredientsSimpleAsync()
        {
            return await _context.Ingredients
                .OrderBy(i => i.Name)
                .Select(i => new Ingredient { Id = i.Id, Name = i.Name })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
