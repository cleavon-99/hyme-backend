using FluentResults;
using MediatR;

namespace Hyme.Application.Commands.NFTs
{
    public record UpdateNFTCommand(Guid NFTId, string Title, string Description) : IRequest<Result>;
}
