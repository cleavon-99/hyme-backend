using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record DeleteBannerCommand(Guid ProjectId) : IRequest<Result>;
    
    
}
