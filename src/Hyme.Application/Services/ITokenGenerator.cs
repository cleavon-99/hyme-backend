using Hyme.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(User userProfile);
    }
}
