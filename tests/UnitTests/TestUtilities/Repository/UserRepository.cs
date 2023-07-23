using Hyme.Domain.Entities;

namespace TestUtilities.Repository
{
    public static class UserRepository
    {
        public static User GetUser() => User.Create(Constants.Constants.User.UserId, Constants.Constants.User.WalletAddress);
    }
}
