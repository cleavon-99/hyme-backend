using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record ApproveProjectCommand(Guid ProjectId) : IRequest<Result>;
    
}
