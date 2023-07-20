using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;

namespace Hyme.Application.MappingProfiles
{
    public class ProjectMappingProfiles : Profile
    {
        public ProjectMappingProfiles()
        {
            CreateMap<Project, ProjectResponse>()
                .ForMember(p => p.Id, options => options.MapFrom(p => p.Id.Value));
        }
    }
}
