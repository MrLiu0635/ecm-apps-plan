using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Core;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDynamicManager
    {
        PlanDynamicDac dac = new PlanDynamicDac();
        public List<PlanDynamic> GetOnUsedPlanDynamicList(string roleID)
        {
            Organization org = new EcpOrgService().GetUserOrg(Utils.GetUserId(), Utils.GetTenantId());
            string orgID = org.ID;
            return dac.GetOnUsedPlanDynamicList(roleID, orgID);
        }
        public PlanDynamicState GetPlanDynamicState(string planDefineID,string periodID)
        {
                return dac.GetPlanDynamicState(planDefineID, periodID);
        }
        public List<PlanDynamic> GetPlanDynamicList()
        {
            return dac.GetPlanDynamicList();
        }

        public void DeletePlanDynamic(List<string> planDynamicIDs)
        {
            //do delete
            dac.DeletePlanDynamic(planDynamicIDs);
        }

        public void AddPlanDynamic(PlanDynamic entity)
        {
            //do Add
            dac.AddPlanDynamic(entity);
        }

        public void UpdatePlanDynamic(PlanDynamic entity)
        {
            //do update
            dac.UpdatePlanDynamic(entity);
        }

        public bool IsExistPlanDefineRef(string planDefineID)
        {
            return dac.IsExistPlanDefineRef(planDefineID);
        }
    }
}
