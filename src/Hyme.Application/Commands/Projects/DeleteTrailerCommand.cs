using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record DeleteTrailerCommand(Guid ProjectId) : IRequest<Result>;
   
}
