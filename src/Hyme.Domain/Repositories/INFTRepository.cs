using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Repositories
{
    public interface INFTRepository
    {
        Task<List<NFT>> GetNFTsAsync(ProjectId projectId, PaginationFilter filter);
        Task<NFT?> GetByIdAsync(NFTId id);
    }
}
