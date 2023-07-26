using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(ProjectId id);
        Task<Project?> GetByOwnerIdAsync(UserId ownerId);
        Task<List<Project>> GetListAsync(PaginationFilter filter);
    }
}
