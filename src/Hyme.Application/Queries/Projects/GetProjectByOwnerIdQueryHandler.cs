using AutoMapper;
using Hyme.Application.Common;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Extensions;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Queries.Projects
{
    public class GetProjectByOwnerIdQueryHandler : IRequestHandler<GetProjectByOwnerIdQuery, ProjectResponse?>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectByOwnerIdQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }


        public async Task<ProjectResponse?> Handle(GetProjectByOwnerIdQuery request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByOwnerIdAsync(new UserId(request.Id));
            if (project is null)
                return null;

            ProjectResponse projectResponse = _mapper.Map<ProjectResponse>(project);
            projectResponse.Logo = projectResponse.Logo.ToLink(LinkType.Image);
            projectResponse.Banner = projectResponse.Banner.ToLink(LinkType.Image);
            projectResponse.Trailer = projectResponse.Trailer.ToLink(LinkType.Video);
            return projectResponse;
        }
    }
}
