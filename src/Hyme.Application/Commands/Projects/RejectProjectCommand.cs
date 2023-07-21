using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record RejectProjectCommand(Guid ProjectId) : IRequest<Result>;
   
}
