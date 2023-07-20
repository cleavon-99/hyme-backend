using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(ProjectId id);
        Task<List<Project>> GetListAsync(PaginationFilter filter);
    }
}
