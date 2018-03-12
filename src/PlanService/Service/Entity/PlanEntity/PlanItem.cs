using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanItem
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Order { get; set; }

        public string ParentPlanItemID { get; set; }

        public string SourcePlanItemID { get; set; }

        public List<PlanItemColContent> PlanItemContent { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public string SelfAssessment { get; set; }

        public string AssessmentOfSuperior { get; set; }

        public string SelfAssessmentScore { get; set; }

        public string AssessmentScoreOfSuperior { get; set; }

        public List<PlanItemColContent> SummaryContent { get; set; }

        public int Weight { get; set; }
    }
}
