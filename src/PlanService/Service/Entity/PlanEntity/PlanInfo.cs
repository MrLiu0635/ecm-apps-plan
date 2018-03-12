using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public Period Period { get; set; }

        public SysUser MainRecipient { get; set; }

        public List<SysUser> CarbonCopyRecipient { get; set; }

        public PlanState State { get; set; }

        public string ApprovalInstance { get; set; }

        public PlanStage Stage { get; set; }

        public List<PlanItem> PlanItems { get; set; }

        public SysUser Creator { get; set; }

        public DateTime CreatedTime { get; set; }

        public string PlanDefineID { get; set; }
    }
}
