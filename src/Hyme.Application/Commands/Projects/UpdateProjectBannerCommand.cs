using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public record UpdateProjectBannerCommand(Guid ProjectId, byte[]Banner, string FileName) : IRequest<Result>;
   
}
