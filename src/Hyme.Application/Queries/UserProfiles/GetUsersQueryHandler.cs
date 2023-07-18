using AutoMapper;
using Hyme.Application.DTOs.Response;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using MediatR;

namespace Hyme.Application.Queries.UserProfiles
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserResponse>>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserProfileRepository userProfileRepository, IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            PaginationFilter filter = PaginationFilter.Create(request.PageNumber, request.PageSize);
            List<User> users = await _userProfileRepository.GetListAsync(filter);
            List<UserResponse> userResponse = _mapper.Map<List<UserResponse>>(users);
            return PagedResponse<UserResponse>.Create(userResponse, request.PageNumber, request.PageSize);
        }
    }
}
