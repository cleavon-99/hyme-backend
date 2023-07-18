using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.UserProfiles
{
    public record GetUsersQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<UserResponse>>;
   
}
