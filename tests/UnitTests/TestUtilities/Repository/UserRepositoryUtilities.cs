using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;

namespace TestUtilities.Repository
{
    public static class UserRepositoryUtilities
    {
        public static User GetUser() => User.Create(Constants.Constants.User.UserId, Constants.Constants.User.WalletAddress);
        public static List<User> GetUsers() => new() 
        {
            User.Create(new UserId(Guid.NewGuid()), new WalletAddress(Constants.Constants.Authentication.WalletAddress)),
            User.Create(new UserId(Guid.NewGuid()), new WalletAddress(Constants.Constants.Authentication.WalletAddress)),
            User.Create(new UserId(Guid.NewGuid()), new WalletAddress(Constants.Constants.Authentication.WalletAddress)),
        };
    }
}
