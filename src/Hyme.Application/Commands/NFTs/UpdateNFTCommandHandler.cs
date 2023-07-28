using FluentResults;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public class UpdateNFTCommandHandler : IRequestHandler<UpdateNFTCommand, Result>
    {
        private readonly INFTRepository _nftRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNFTCommandHandler(INFTRepository nftRepository, IUnitOfWork unitOfWork)
        {
            _nftRepository = nftRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateNFTCommand request, CancellationToken cancellationToken)
        {
            NFT? nft = await _nftRepository.GetByIdAsync(new NFTId(request.NFTId));
            if (nft is null)
                return Result.Fail(new NFTNotFoundError(request.NFTId));
            nft.Update(request.Title, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
