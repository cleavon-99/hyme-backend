using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hyme.Infrastructure.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(ProjectId id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Project?> GetByOwnerIdAsync(UserId ownerId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == ownerId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Project>> GetListAsync(PaginationFilter filter)
        {
            return await _context.Projects
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();
        }
    }
}
