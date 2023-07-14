using Hyme.Application.DTOs.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Queries.UserProfiles
{
    public record GetUserProfleByIdQuery(Guid Id) : IRequest<UserProfileResponse?>;
    
}
