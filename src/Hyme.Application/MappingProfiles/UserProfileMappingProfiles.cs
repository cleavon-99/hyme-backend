using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;

namespace Hyme.Application.MappingProfiles
{
    public class UserProfileMappingProfiles : Profile
    {
        public UserProfileMappingProfiles()
        {
            CreateMap<User, UserProfileResponse>()
                .ForMember(u => u.Id, options => options.MapFrom(s => s.Id.Value))
                .ForMember(u => u.WalletAddress, options => options.MapFrom(s => s.WalletAddress.Value));
        }
    }
}
