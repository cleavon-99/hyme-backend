using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Queries.Users
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User? userProfile = await _userRepository.GetByIdAsync(new UserId(request.Id));

            if (userProfile is null)
                return null;

            return _mapper.Map<UserResponse>(userProfile);
        }
    }
}
