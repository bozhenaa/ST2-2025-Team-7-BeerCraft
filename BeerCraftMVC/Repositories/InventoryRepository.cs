using BeerCraftMVC.Data;
using BeerCraftMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerCraftMVC.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly BeerCraftDbContext _context;
        public InventoryRepository(BeerCraftDbContext context)
        {
            _context = context;
        }
        public async Task AddInventoryItemAsync(Inventory item)
        {
           var existingItem = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserId == item.UserId && i.IngredientId == item.IngredientId);
            if(existingItem != null)
            {
                existingItem.Quantity = item.Quantity;
                existingItem.BoughtAt = item.BoughtAt;
                _context.Inventories.Update(existingItem);
            }
            else
            {
                await  _context.Inventories.AddAsync(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAync(int userId, int itemId)
        {
            var item = _context.Inventories.FindAsync(userId, itemId);
            if (item != null)
            {
                _context.Inventories.Remove(item.Result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Inventory> GetItemAsync(int userId, int itemId)
        {
           return await _context.Inventories
                .FindAsync(userId, itemId);
        }

        public async Task<IEnumerable<Inventory>> GetUserInventoryItemsAsync(int userId)
        {
            return await _context.Inventories
                .Include(i=> i.Ingredient)
                .Where(i => i.UserId == userId)
                .OrderBy(i=> i.Ingredient.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
