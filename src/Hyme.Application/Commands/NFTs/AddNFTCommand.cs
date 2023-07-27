using FluentResults;
using Hyme.Application.DTOs.Response;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public record class AddNFTCommand(Guid ProjectId, string Title, string Description, string FileName, byte[] Image) : IRequest<Result<NFTResponse>>;

}
