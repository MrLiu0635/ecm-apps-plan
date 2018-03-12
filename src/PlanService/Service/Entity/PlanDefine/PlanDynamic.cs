using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanDynamic
    {
        public string ID { get; set; }

        public PlanDefine PlanDefine { get; set; }

        public Period Period { get; set; }

        public PlanDynamicState State { get; set; }
    }
}
