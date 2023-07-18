using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class MockUserProfileRepository : IUserProfileRepository
    {
        private readonly List<User> _userProfiles = new();
        
        public MockUserProfileRepository()
        {
            
        }


        public Task AddAsync(User userProfile)
        {
            _userProfiles.Add(userProfile);
            return Task.CompletedTask;
        }

        public Task<User?> GetByIdAsync(UserId id)
        {
            return Task.FromResult(_userProfiles.Where(u => u.Id == id).FirstOrDefault());
        }

        public Task<User?> GetByWalletAddress(WalletAddress walletAddress)
        {
            return Task.FromResult(_userProfiles.Where(u => u.WalletAddress == walletAddress).FirstOrDefault());
        }

        public Task<List<User>> GetListAsync(PaginationFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
