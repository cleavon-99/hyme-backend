using FluentResults;
using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.Projects
{
    public record GetProjectByOwnerIdQuery(Guid Id) : IRequest<ProjectResponse?>;
   
}
