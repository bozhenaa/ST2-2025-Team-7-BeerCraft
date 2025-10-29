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
    }
}
