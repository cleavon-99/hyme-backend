using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class WhiteListRepository : IWhitelistRepository
    {
        private readonly ApplicationDbContext _context;

        public WhiteListRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Whitelist?> FindAsync(WalletAddress walletAddress)
        {
            return await _context.Whitelists.FindAsync(walletAddress);
        }
    }
}
