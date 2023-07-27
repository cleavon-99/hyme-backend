using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Queries.NFTs
{
    public record GetProjectNFTsQuery(Guid ProjectId, int PageNumber, int PageSize) : IRequest<PagedResponse<NFTResponse>>;
}
