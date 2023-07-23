using Hyme.Application.Commands.Authentication;
using Hyme.Application.Tests.TestUtilities.Constants;

namespace Hyme.Application.Tests.Commands.Authentication
{
    public static class AuthenticationUtilities
    {
        public static ConnectToWalletCommand ConnectToWalletCommand() => new(
            Constants.Authentication.Message, 
            Constants.Authentication.Signature, 
            Constants.Authentication.WalletAddress);
    }
}
