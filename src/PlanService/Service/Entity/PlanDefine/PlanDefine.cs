using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class PlanDefine
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public PlanDefineState State { get; set; }

        private PlanItemModel planModel;
        public PlanItemModel PlanModel
        {
            get
            {
                if (planModel == null)
                    planModel=new PlanItemModel();
                return planModel;
            }
            set { planModel = value; }
        }
        private PeriodSet periodSet;
        public PeriodSet PeriodSet
        {
            get
            {
                if (periodSet == null)
                    periodSet=new PeriodSet();
                return periodSet;
            }
            set { periodSet = value; }
        }

        private PeriodType periodType { get; set; }
        public PeriodType PeriodType
        {
            get
            {
                if (periodType == null)
                    periodType=new PeriodType();
                return periodType;
            }
            set { periodType = value; }
        }
        private CustomizedModel planItemCustomization;
        public CustomizedModel PlanItemCustomization
        {
            get
            {
                if (planItemCustomization == null)
                    planItemCustomization = new CustomizedModel();
                return planItemCustomization;
            }
            set { planItemCustomization = value; }
        }

        public List<Organization> OrgList { get; set; }

        public List<Role> RoleList { get; set; }
    }
}
