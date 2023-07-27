using AutoMapper;
using Hyme.Application.Common;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Extensions;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Queries.NFTs
{
    public class GetProjectNFTsQueryHandler : IRequestHandler<GetProjectNFTsQuery, PagedResponse<NFTResponse>>
    {
        private readonly INFTRepository _nftRepository;
        private readonly IMapper _mapper;

        public GetProjectNFTsQueryHandler(INFTRepository nftRepository, IMapper mapper)
        {
            _nftRepository = nftRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<NFTResponse>> Handle(GetProjectNFTsQuery request, CancellationToken cancellationToken)
        {
            List<NFT> nfts = await _nftRepository.GetNFTsAsync(new ProjectId(request.ProjectId), PaginationFilter.Create(request.PageNumber, request.PageSize));

            List<NFTResponse> nftResponses = _mapper.Map<List<NFTResponse>>(nfts);
            nftResponses.ForEach(nft => nft.Image = nft.Image.ToLink(LinkType.Image));
            return PagedResponse<NFTResponse>.Create(nftResponses, request.PageNumber, request.PageSize);   
        }
    }
}
