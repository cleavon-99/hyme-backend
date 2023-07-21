using FluentResults;
using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record AddProjectCommand(Guid UserId, string Title, string ShortDescription, string ProjectDescription) : IRequest<Result<ProjectResponse>>;
    
}
