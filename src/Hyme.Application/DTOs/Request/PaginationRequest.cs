using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.DTOs.Request
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; }
        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > 100 ? 100 : value; }
        }

        public PaginationRequest()
        {
            PageNumber = 1;
            PageSize = 20;
        }
    }
}
