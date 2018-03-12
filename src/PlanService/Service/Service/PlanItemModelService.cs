using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanItemModelService
    {
        private readonly PlanItemModelManager manager = new PlanItemModelManager();
        private static PlanItemModelService instance = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private PlanItemModelService()
        {
            // 测试时
            // PlanState.IsTest = true;
        }

        public static PlanItemModelService Current => instance ?? (instance = new PlanItemModelService());

        public List<PlanItemModel> GetPlanItemModel()
        {
            return manager.GetPlanItemModel();
        }

        public PlanItemModel GetPlanItemModelByID(string modelID,string planDefineID)
        {
            return manager.GetPlanItemModelByID(modelID, planDefineID);
        }
    }
}
