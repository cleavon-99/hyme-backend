using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;

namespace Hyme.Application.MappingProfiles
{
    public class NFTMappingProfiles : Profile
    {
        public NFTMappingProfiles()
        {
            CreateMap<NFT, NFTResponse>()
                .ForMember(m => m.Id, options => options.MapFrom(n => n.Id.Value));
        }
    }
}
