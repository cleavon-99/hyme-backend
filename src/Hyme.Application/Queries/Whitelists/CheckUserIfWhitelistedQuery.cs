using MediatR;

namespace Hyme.Application.Queries.Whitelists
{
    public record CheckUserIfWhitelistedQuery(Guid Id) : IRequest<bool>;

}
