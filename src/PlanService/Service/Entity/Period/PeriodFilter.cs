using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PeriodFilter
    {
        public string PeriodID { get; set; }

        public string PeriodSetID { get; set; }

        public string PeriodTypeID { get; set; }

        public string PeriodTypeCode { get; set; }

        public DateTime Time { get; set; }
    }
}
