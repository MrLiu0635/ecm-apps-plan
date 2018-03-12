using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class CustomizedModel
    {
        public string ID { get; set; }

        public string PlanDefineID { get; set; }

        public List<CustomizedModelField> CustomizedModelContent { get; set; }
    }
}
