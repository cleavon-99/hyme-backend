using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetListAsync(PaginationFilter filter);
        Task<User?> GetByIdAsync(UserId id);
        Task<User?> GetByWalletAddress(WalletAddress walletAddress);
        Task AddAsync(User userProfile);
    }
}
