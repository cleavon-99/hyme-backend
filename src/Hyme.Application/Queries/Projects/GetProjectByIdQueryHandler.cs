using AutoMapper;
using FluentResults;
using Hyme.Application.Common;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Extensions;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Hyme.Application.Queries.Projects
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectResponse?>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectByIdQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<ProjectResponse?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.Id));
            if (project is null)
                return null;

            ProjectResponse response = _mapper.Map<ProjectResponse>(project);
            response.Logo = response.Logo.ToLink(LinkType.Image);
            response.Banner = response.Banner.ToLink(LinkType.Image);
            response.Trailer = response.Trailer.ToLink(LinkType.Video);
            return response;
        }
    }
}
