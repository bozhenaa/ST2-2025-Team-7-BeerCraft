using BeerCraftMVC.Models.Entities;
namespace BeerCraftMVC.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync (string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
