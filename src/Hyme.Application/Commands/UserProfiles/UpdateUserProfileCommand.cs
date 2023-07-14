using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Commands.UserProfiles
{
    public record UpdateUserProfileCommand(Guid UserProfileId, string Name) : IRequest<Result>;
    
}
