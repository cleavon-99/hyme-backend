using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Domain.Primitives
{
    public class PaginationFilter
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int Skip { get; private set; }

        private PaginationFilter(int pageNumber, int pageSize, int skip)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Skip = skip;
        }

        public static PaginationFilter Create(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            return new PaginationFilter(pageNumber, pageSize, skip);
        }

    }
}
