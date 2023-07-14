using Hyme.Application.Services;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Infrastructure.Services
{
    public class WalletValidationService : IWalletValidationService
    {
        private readonly EthereumMessageSigner _signer;
        public WalletValidationService()
        {
            _signer = new();
        }

        public bool Validate(string message, string signature, string walletAddress)
        {
            string result = _signer.EncodeUTF8AndEcRecover(message, signature);
            if (result == walletAddress)
                return true;
            return false;
        }
    }
}
