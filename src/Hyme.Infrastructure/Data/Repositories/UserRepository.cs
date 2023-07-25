using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(User userProfile)
        {
            await _context.Users.AddAsync(userProfile);
        }

        public async Task<User?> GetByIdAsync(UserId id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByIdWithProjectAsync(UserId id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Project)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByWalletAddress(WalletAddress walletAddress)
        {
            return await _context.Users
                .Where(u => u.WalletAddress == walletAddress)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetListAsync(PaginationFilter filter)
        {
            return await _context.Users
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();
        }
    }
}
