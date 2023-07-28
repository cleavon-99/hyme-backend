using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Users
{
    public record UpdateUserCommand(Guid UserId, string Name) : IRequest<Result>;

}
