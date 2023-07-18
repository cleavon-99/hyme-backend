using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.DTOs.Response
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextpage { get; }
        public bool HasNextPage { get; private set; }

        private PagedResponse(List<T> data, int pageNumber, int pageSize, bool hasPreviousPage, bool hasNextpage)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasPreviousPage = hasPreviousPage;
            HasNextpage = hasNextpage;
        }

        public static PagedResponse<T> Create(List<T> data, int pageNumber, int pageSize)
        {
            bool hasNextPage = data.Count == pageSize;
            bool hasPreviousPage = pageNumber - 1 >= 1;
            return new PagedResponse<T>(data, pageNumber, pageSize, hasPreviousPage, hasNextPage);
        }
    }
}
