using Hyme.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Infrastructure.Data.Repositories.Mock
{
    public class MockUnitOfWork : IUnitOfWork
    {
        public Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
