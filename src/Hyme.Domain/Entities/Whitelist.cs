using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Whitelist
    {
        public WalletAddress WalletAddress { get; private set; }

        public Whitelist(WalletAddress walletAddress)
        {
            WalletAddress = walletAddress;
        }
    }
}
