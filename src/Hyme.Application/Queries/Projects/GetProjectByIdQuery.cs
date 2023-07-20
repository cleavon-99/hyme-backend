using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.Projects
{
    public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectResponse?>;
    
}
