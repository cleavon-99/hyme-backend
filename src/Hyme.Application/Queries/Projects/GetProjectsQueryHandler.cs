﻿using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Queries.Projects
{
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PagedResponse<ProjectResponse>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectsQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ProjectResponse>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            PaginationFilter filter = PaginationFilter.Create(request.PageNumber, request.PageSize);
            List<Project> projects = await _projectRepository.GetListAsync(filter);
            List<ProjectResponse> projectsReponse = _mapper.Map<List<ProjectResponse>>(projects);
            return PagedResponse<ProjectResponse>.Create(projectsReponse, request.PageNumber, request.PageSize);
        }
    }
}
