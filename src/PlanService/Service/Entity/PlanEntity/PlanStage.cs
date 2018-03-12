using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public enum PlanStage
    {
        Unknown=0,
        PlanInitialize=1,
        PlanExecution = 2,
        SelfEvaluate =3,
        SupirorEvaluate=4,
        CompleteEvaluate=5
    }
}
