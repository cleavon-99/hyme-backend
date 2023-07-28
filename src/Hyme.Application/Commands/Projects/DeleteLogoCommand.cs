using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record DeleteLogoCommand(Guid ProjectId) : IRequest<Result>;
    
}
