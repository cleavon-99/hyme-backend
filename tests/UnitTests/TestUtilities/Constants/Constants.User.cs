using Hyme.Domain.ValueObjects;

namespace TestUtilities.Constants
{

    public static partial class Constants
    {
        public static class User
        {
            public static readonly UserId UserId = new(Guid.NewGuid());
            public const string Name = "Arjay Reyes Maligaya";
            public static readonly WalletAddress WalletAddress = new(Authentication.WalletAddress);
        }
    }
    
}
