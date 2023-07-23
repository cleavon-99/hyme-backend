
using Hyme.Domain.ValueObjects;

namespace Hyme.Application.Tests.TestUtilities.Constants
{
    public static partial class Constants
    {
        public static class User
        {
            public static readonly UserId Id = new(Guid.NewGuid());
            public const string Name = "Arjay Reyes Maligaya";
            public static readonly WalletAddress WalletAddress = new(Authentication.WalletAddress);
        }
    }
}
