using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Repositories
{
    public interface IWhitelistRepository
    {
        Task<Whitelist?> FindAsync(WalletAddress walletAddress);
    }
}
