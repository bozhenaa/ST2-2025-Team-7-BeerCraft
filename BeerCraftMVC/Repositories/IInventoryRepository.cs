using BeerCraftMVC.Models.Entities;

namespace BeerCraftMVC.Repositories
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetUserInventoryItemsAsync(int userId);
        Task AddInventoryItemAsync(Inventory item);
        Task <Inventory> GetItemAsync(int userId, int itemId);
        Task DeleteAync (int userId, int itemId);
    }
}
