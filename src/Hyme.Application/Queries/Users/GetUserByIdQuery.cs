using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.Users
{
    public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;

}
