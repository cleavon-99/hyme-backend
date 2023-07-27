using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class NFTRepository : INFTRepository
    {
        private readonly ApplicationDbContext _context;

        public NFTRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NFT>> GetNFTsAsync(ProjectId id, PaginationFilter filter)
        {
            return await _context.NFTs
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();
        }
    }
}
