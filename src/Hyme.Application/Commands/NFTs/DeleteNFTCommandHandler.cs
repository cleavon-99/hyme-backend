using FluentResults;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public class DeleteNFTCommandHandler : IRequestHandler<DeleteNFTCommand, Result>
    {
        private readonly INFTRepository _nftRepositry;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNFTCommandHandler(INFTRepository nftRepositry, IUnitOfWork unitOfWork)
        {
            _nftRepositry = nftRepositry;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<Result> Handle(DeleteNFTCommand request, CancellationToken cancellationToken)
        {
            NFT? nft = await _nftRepositry.GetByIdAsync(new NFTId(request.NFTId));
            if (nft is null)
                return Result.Fail(new NFTNotFoundError(request.NFTId));

            nft.Delete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
