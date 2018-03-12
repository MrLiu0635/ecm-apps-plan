using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanItemModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public List<PlanItemModelField> PlanItemModelContent { get; set; }
    }
}
