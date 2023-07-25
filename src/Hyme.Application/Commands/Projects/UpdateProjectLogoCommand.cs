using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record UpdateProjectLogoCommand(Guid ProjectId, byte[] Logo, string FileName) : IRequest<Result>; 
}
