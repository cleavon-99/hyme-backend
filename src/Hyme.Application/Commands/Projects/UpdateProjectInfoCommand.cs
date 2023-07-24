using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record UpdateProjectInfoCommand(
        Guid ProjectId,
        string Title,
        string ShortDescription,
        string ProjectDescription) : IRequest<Result>;
    
}
