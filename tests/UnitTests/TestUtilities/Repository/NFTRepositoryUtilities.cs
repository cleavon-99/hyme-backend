using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;

namespace TestUtilities.Repository
{
    public static class NFTRepositoryUtilities
    {
        public static NFT CreateNFT() 
            => NFT.Create(new NFTId(Guid.NewGuid()), new ProjectId(Guid.NewGuid()), "Title", "Description", "Image");
    }
}
