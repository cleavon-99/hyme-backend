using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Services
{
    public interface IWalletValidationService
    {
        bool Validate(string message, string signature, string walletAddress);
    }
}
