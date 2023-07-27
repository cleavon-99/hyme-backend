using FluentResults;

namespace Hyme.Application.Errors
{
    public class NFTNotFoundError : Error
    {
        public NFTNotFoundError(Guid id) : base($"NFT with id {id} not found")
        {
            
        }
    }
}
