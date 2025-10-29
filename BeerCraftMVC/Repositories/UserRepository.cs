using BeerCraftMVC.Data;
using BeerCraftMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerCraftMVC.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly BeerCraftDbContext _context;

        public UserRepository(BeerCraftDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
         .Include(u => u.Inventory) 
             .ThenInclude(inv => inv.Ingredient)
         .AsNoTracking() 
         .FirstOrDefaultAsync(u => u.Id == id); 
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
