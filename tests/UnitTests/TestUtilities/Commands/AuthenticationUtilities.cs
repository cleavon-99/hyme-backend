using Hyme.Application.Commands.Authentication;
using Hyme.Domain.Entities;

namespace TestUtilities.Commands
{
    public static class AuthenticationUtilities
    {
        public static ConnectToWalletCommand ConnectToWalletCommand() => new(
            Constants.Constants.Authentication.Message,
            Constants.Constants.Authentication.Signature,
            Constants.Constants.Authentication.WalletAddress);


    }
}
