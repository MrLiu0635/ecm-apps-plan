using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Service
{
    public class PlanItemService
    {
        private static PlanItemService instance = null;

        private PlanItemService()
        {

        }
        public static PlanItemService Current => instance ?? (instance = new PlanItemService());
        #region old
        /// <summary>
        /// 根据当前时间周期获取上级同一时间周期的计划项列表
        /// </summary>
        /// <param name="period">时间周期</param>
        /// <returns></returns>
        public List<Entity.PlanItem> GetSuperiorPlanItem(string periodID)
        {
            PlanDac dac = new PlanDac();
            List<Entity.PlanItem> planItemList = new List<Entity.PlanItem>();
#warning 缺少获取上级ID
            string superiorID = "test"; //获取当前用户的直接上级
            PlanFilter planFilter = new PlanFilter();
            planFilter.Periods = new List<string>() { periodID };
            planFilter.Senders = new List<string>() { superiorID };
            //获取当前用户直接上级当前时间段内的计划
            List<PlanInfo> planInfoList = PlanService.Current.Get(planFilter);
            if (planInfoList == null || planInfoList.Count <= 0)
                return null;
            planInfoList.ForEach(item =>
            {
                planItemList.AddRange(dac.GetPlanItems(item.ID));
            });

            return planItemList;
        }
        /// <summary>
        /// 根据当前时间周期获取父周期的计划项列表
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<Entity.PlanItem> GetParentPlanItem(string periodID)
        {
            PeriodManager pm = new PeriodManager();
            var parentPeriod = pm.GetParentPeriod(periodID);
            if (parentPeriod == null)
                return null;
            PlanDac dac = new PlanDac();
#warning 缺少判断父周期是否启用
            List<Entity.PlanItem> planItemList = new List<Entity.PlanItem>();
            PlanFilter planFilter = new PlanFilter();
            planFilter.Periods = new List<string>() { parentPeriod.ID };
            planFilter.Senders = new List<string>() { Utils.GetUserId() };
            //获取当前用户父时间段内的计划信息
            List<PlanInfo> planInfoList = PlanService.Current.Get(planFilter);
            if (planInfoList == null || planInfoList.Count <= 0)
                return null;
            planInfoList.ForEach(item =>
            {
                planItemList.AddRange(dac.GetPlanItems(item.ID));
            });

            return planItemList;
        }
        #endregion
        /// <summary>
        /// 获取上级计划列表
        /// </summary>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public List<PlanInfo> GetSuperiorPlan(string periodID)
        {
#warning 缺少获取上级ID
            string superiorID = "test"; //获取当前用户的直接上级
            PlanFilter planFilter = new PlanFilter();
            planFilter.Periods = new List<string>() { periodID };
            planFilter.Senders = new List<string>() { superiorID };
            //获取当前用户直接上级当前时间段内的计划
            List<PlanInfo> planInfoList = PlanService.Current.Get(planFilter);

            return planInfoList;
        }
        /// <summary>
        /// 获取父计划列表
        /// </summary>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public List<PlanInfo> GetParentPlan(string periodID)
        {
            PeriodManager pm = new PeriodManager();
            var parentPeriod = pm.GetParentPeriod(periodID);
            if (parentPeriod == null)
                return null;
#warning 缺少判断父周期是否启用
            PlanFilter planFilter = new PlanFilter();
            planFilter.Periods = new List<string>() { parentPeriod.ID };
            planFilter.Senders = new List<string>() { Utils.GetUserId() };
            //获取当前用户父时间段内的计划信息
            List<PlanInfo> planInfoList = PlanService.Current.Get(planFilter);

            return planInfoList;
        }
        /// <summary>
        /// 根据计划获取计划项
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public List<PlanItem> GetPlanItems(string planID)
        {
            PlanDac dac = new PlanDac();
            return dac.GetPlanItems(planID);
        }


        public string SavePlanItem(string planDefineID, string periodID, PlanItem planItem)
        {
            PlanDac dac = new PlanDac();
            PlanFilter filter = new PlanFilter();
            filter.MyStatus = 1;
            filter.PlanDefines = new List<string>() { planDefineID };
            filter.Periods = new List<string>() { periodID };
            List<PlanInfo> planList= PlanService.Current.Get(filter);
            PlanInfo plan = new PlanInfo();
            if (planList==null|| planList.Count<=0)
            {
               
                plan.PlanDefineID = planDefineID;
                plan.Period.ID = periodID;
                plan.PlanItems = new List<PlanItem>() { planItem };
                PlanService.Current.SavePlanInfo(plan);
            }
            else
            {
                plan = planList[0];
                PlanItem planItemInfo = plan.PlanItems.Find(item => item.ID == planItem.ID);
                if (planItemInfo == null)
                    dac.SavePlanItem(plan.ID, planItem);
                else
                {
                    dac.UpdatePlanItem(planItem);
                }
            }
            return plan.ID;
        }
    }
}
