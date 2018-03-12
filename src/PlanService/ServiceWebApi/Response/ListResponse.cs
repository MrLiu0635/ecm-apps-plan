using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    public class ListResponse<T> : Response
    {
        public List<T> Data { get; set; }
        public Pagination PaginationInfo { get; set; }
    }
}
