using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.Users
{
    public record GetUsersQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<UserResponse>>;
   
}
