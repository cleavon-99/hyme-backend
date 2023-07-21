using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Users
{
    public record UpdateUserCommand(Guid UserProfileId, string Name) : IRequest<Result>;

}
