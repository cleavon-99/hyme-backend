using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.Projects
{
    public record GetProjectsQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<ProjectResponse>>;
    
}
