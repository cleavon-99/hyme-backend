using AutoMapper;
using FluentResults;
using Hyme.Application.Common;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Extensions;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public class AddNFTCommandHandler : IRequestHandler<AddNFTCommand, Result<NFTResponse>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;
        private readonly IMapper _mapper;

        public AddNFTCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IBlobService blobService, 
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            _mapper = mapper;
        }

        public async Task<Result<NFTResponse>> Handle(AddNFTCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.ProjectId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.ProjectId));

            NFT nft = project.AddNFT(request.Title, request.Description, request.FileName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _blobService.UploadImageAsync(request.Image, request.FileName);
            NFTResponse nftResponse = _mapper.Map<NFTResponse>(nft);
            nftResponse.Image = nftResponse.Image.ToLink(LinkType.Image);
            return nftResponse;
        }
    }
}
