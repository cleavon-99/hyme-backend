using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class UserprofileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public UserprofileRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(User userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
        }

        public async Task<User?> GetByIdAsync(UserId id)
        {
            return await _context.UserProfiles.FindAsync(id);
        }

        public async Task<User?> GetByWalletAddress(WalletAddress walletAddress)
        {
            return await _context.UserProfiles
                .Where(u => u.WalletAddress == walletAddress)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();
        }

        public Task<List<User>> GetListAsync(PaginationFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
