using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.GSP.Caf.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDynamicService
    {
        private PlanDynamicManager manager = new PlanDynamicManager();
        private static PlanDynamicService instance = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private PlanDynamicService()
        {
            // 测试时
            // PlanState.IsTest = true;
        }

        public static PlanDynamicService Current => instance ?? (instance = new PlanDynamicService());

        public List<PlanDynamic> GetOnUsedPlanDynamicList(string roleID)
        {
            return manager.GetOnUsedPlanDynamicList(roleID);
        }

        public List<PlanDynamic> GetPlanDynamicList()
        {
            return manager.GetPlanDynamicList();
        }
        public PlanDynamicState GetPlanDynamicState(string planDefineID, string periodID)
        {
            return manager.GetPlanDynamicState(planDefineID, periodID);
        }

        public void DeletePlanDynamic(List<string> planDynamicIDs)
        {
            DataValidator.CheckForNullReference(planDynamicIDs, "planDynamicIDs");
            manager.DeletePlanDynamic(planDynamicIDs);
        }

        public void AddPlanDynamic(PlanDynamic entity)
        {
            DataValidator.CheckForNullReference(entity, "plandynamic");
            DataValidator.CheckForNullReference(entity.PlanDefine, "plandynamic.plandefine");
            DataValidator.CheckForNullReference(entity.Period, "plandynamic.period");
            manager.AddPlanDynamic(entity);
        }

        public void UpdatePlanDynamic(PlanDynamic entity)
        {
            DataValidator.CheckForNullReference(entity, "plandynamic");
            DataValidator.CheckForNullReference(entity.PlanDefine, "plandynamic.plandefine");
            DataValidator.CheckForNullReference(entity.Period, "plandynamic.period");
            manager.UpdatePlanDynamic(entity);
        }

        public bool IsExistPlanDefineRef(string planDefineID)
        {
            return manager.IsExistPlanDefineRef(planDefineID);
        }
    }
}
