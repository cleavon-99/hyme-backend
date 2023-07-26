using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record UpdateProjectTrailerCommand(Guid ProjectId, byte[]Trailer, string FileName) : IRequest<Result>;
  
}
