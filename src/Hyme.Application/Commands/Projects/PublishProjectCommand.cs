using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record PublishProjectCommand(Guid UserId) : IRequest<Result>;   
}
