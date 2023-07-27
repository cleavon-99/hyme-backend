using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public record DeleteNFTCommand(Guid NFTId) : IRequest<Result>;
}
