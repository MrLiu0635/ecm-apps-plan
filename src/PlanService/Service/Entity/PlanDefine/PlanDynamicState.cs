using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public enum PlanDynamicState
    {
        Unknown = 0,
        ToSet = 1,
        InExecution=2,
        ToSummarize = 3
    }
}
