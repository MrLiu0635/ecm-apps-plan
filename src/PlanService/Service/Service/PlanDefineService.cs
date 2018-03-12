using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.EcmCloud.Apps.Plan.Service.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Inspur.GSP.Caf.Common;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDefineService
    {
        private readonly PlanDefineManager manager = new PlanDefineManager();
        private static PlanDefineService instance = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private PlanDefineService()
        {
            // 测试时
            // PlanState.IsTest = true;
        }

        public static PlanDefineService Current => instance ?? (instance = new PlanDefineService());

        public List<PlanDefine> GetPlanDefineList()
        {
            return manager.GetPlanDefineList();
        }
        
        public void DeletePlanDefine(string planDefineID)
        {
            manager.DeletePlanDefine(planDefineID);
        }

        public PlanDefine GetPlanDefine(string planDefineID)
        {
            return manager.GetPlanDefine(planDefineID);
        }

        public string SavePlanDefine( PlanDefine planDefine)
        {
           return manager.SavePlanDefine(planDefine);
        }
        public string UpdatePlanDefine(PlanDefine planDefine)
        {
            return manager.UpdatePlanDefine(planDefine);
        }

        public void UpdatePlanDefineState(string planDefineID, PlanDefineState state)
        {
            DataValidator.CheckForEmptyString(planDefineID, "planDefineID");
            if (state.Equals(PlanDefineState.Unknown))
                throw new Exception();
            manager.UpdatePlanDefineState(planDefineID, state);
        }
    }
}
