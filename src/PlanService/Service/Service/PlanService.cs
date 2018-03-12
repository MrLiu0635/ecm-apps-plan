using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanService
    {
        private readonly PlanManager manager = new PlanManager();
        private readonly PlanItemModelManager modelManager = new PlanItemModelManager();
        private readonly PlanDefineManager defineManager = new PlanDefineManager();
        private static PlanService instance = null;

        private PlanService() { }

        public static PlanService Current => instance ?? (instance = new PlanService());

        public List<PlanInfo> Get(PlanFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.PlanItemModelID) && (filter.PlanDefines == null || filter.PlanDefines.Count < 1))
            {
                List<string> planDefineIDs = defineManager.GetPlanDefineIDsByModelID(filter.PlanItemModelID);
                if (planDefineIDs != null && planDefineIDs.Count > 0)
                {
                    filter.PlanDefines = planDefineIDs;
                }
            }
            return manager.Get(filter);
        }
        public List<PlanInfo> GetTeamCompletedPlan()
        {
            return manager.GetTeamCompletedPlan();
        }
        
        public List<PlanInfo> GetTeamCurrentPlan()
        {
            return manager.GetTeamCurrentPlan();
        }
        public PlanInfo Get(string planInfoID)
        {
            PlanInfo plan = manager.Get(planInfoID);
            return plan;
        }
        public void PutSuperiorAssessment( List<PlanItem> planItemList)
        {
            manager.PutSuperiorAssessment(planItemList);
        }
        public void PutSelfAssessment(List<PlanItem> planItemList)
        {
            manager.PutSelfAssessment(planItemList);
        }
        public void UpdatePlanState(string planID, int approvalState)
        {
            manager.UpdatePlanState(planID, approvalState);
        }
        public void UpdatePlanStage(string planID, int approvalState)
        {
            manager.UpdatePlanStage(planID, approvalState);
        }
        public void DeletePlan(string planID)
        {
            manager.DeletePlan(planID);
        }
        public string SavePlanInfo( PlanInfo plan)
        {
            return manager.SavePlanInfo(plan);
        }

        public bool IsExistPlanDefineRef(string planDefineID)
        {
            return manager.IsExistPlanDefineRef(planDefineID);
        }
    }
}
