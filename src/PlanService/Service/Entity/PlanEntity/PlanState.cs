using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public enum PlanState
    {
        Unknown = 0,
        Initilize = 1,
        Submitted = 2,
        PassSubmitted = 3,
        UnPassSubmitted = 4
    }
}
