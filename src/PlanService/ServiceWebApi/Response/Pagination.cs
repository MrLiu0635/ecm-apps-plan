using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    public class Pagination
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int RecordCount { get; set; }
    }
}
