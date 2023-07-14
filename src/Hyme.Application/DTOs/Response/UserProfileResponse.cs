using Hyme.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.DTOs.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string WalletAddress { get; set; } = null!;
    }
}
