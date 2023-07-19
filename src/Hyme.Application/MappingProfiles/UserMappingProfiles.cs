using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;

namespace Hyme.Application.MappingProfiles
{
    public class UserMappingProfiles : Profile
    {
        public UserMappingProfiles()
        {
            CreateMap<User, UserResponse>()
                .ForMember(u => u.Id, options => options.MapFrom(s => s.Id.Value))
                .ForMember(u => u.WalletAddress, options => options.MapFrom(s => s.WalletAddress.Value));
        }
    }
}
