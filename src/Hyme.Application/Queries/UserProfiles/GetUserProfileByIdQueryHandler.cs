using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;
using System.Security.Claims;

namespace Hyme.Application.Queries.UserProfiles
{
    public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfleByIdQuery, UserResponse?>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public GetUserProfileByIdQueryHandler(
            IUserProfileRepository userProfileRepository,
            IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public async Task<UserResponse?> Handle(GetUserProfleByIdQuery request, CancellationToken cancellationToken)
        {
            User? userProfile = await _userProfileRepository.GetByIdAsync(new UserId(request.Id));
            
            if (userProfile is null)
                return null;

            return _mapper.Map<UserResponse>(userProfile);
        }
    }
}
