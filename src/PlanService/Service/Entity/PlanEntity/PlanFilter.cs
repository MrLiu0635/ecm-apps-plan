using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanFilter
    {
        public PlanState State { get; set; }

        public PlanStage Stage { get; set; }

        // 0：无 1:发送人 2：接收人 3：抄送人
        public int MyStatus { get; set; }

        public List<string> FirRecips { get; set; }

        public List<string> SecRecips { get; set; }

        public List<string> Senders { get; set; }

        public List<string> Periods { get; set; }

        public string PeriodTypeID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<string> PlanDefines { get; set; }

        public string PlanItemModelID { get; set; }
    }
}
