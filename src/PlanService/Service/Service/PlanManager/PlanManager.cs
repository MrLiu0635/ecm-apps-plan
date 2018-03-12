using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;
using Inspur.EcmCloud.Apps.Plan.Service.Service;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanManager
    {
        EcpUserService userService = new EcpUserService();
        internal List<PlanInfo> Get(PlanFilter filter)
        {
            PlanDac dac = new PlanDac();
            if (filter.MyStatus == 1)
            {
                if (filter.Senders == null)
                    filter.Senders = new List<string>() { Utils.GetUserId() };
                else
                    filter.Senders.Add(Utils.GetUserId());
            }
            else if (filter.MyStatus == 2)
            {
                if (filter.FirRecips == null)
                    filter.FirRecips = new List<string>() { Utils.GetUserId() };
                else
                    filter.FirRecips.Add(Utils.GetUserId());
            }
            else if (filter.MyStatus == 3)
            {
                if (filter.SecRecips == null)
                    filter.SecRecips = new List<string>() { Utils.GetUserId() };
                else
                    filter.SecRecips.Add(Utils.GetUserId());
            }
            List<PlanInfo> plans = dac.Get(filter);
            return plans;
        }
        public List<PlanInfo> GetTeamCompletedPlan()
        {
            PlanFilter filter = new PlanFilter();
            filter.MyStatus = 2;
            filter.Stage = PlanStage.CompleteEvaluate;
            return Get(filter);
        }
        public List<PlanInfo> GetTeamCurrentPlan()
        {
            List<PlanInfo> result = new List<PlanInfo>();
            PlanFilter filter = new PlanFilter();
            filter.MyStatus = 2;
            List<PlanInfo> palnList= Get(filter);
            if (palnList!=null && palnList.Count>0)
            {
                foreach(PlanInfo plan in palnList)
                {
                    if ((plan.State == PlanState.Initilize && plan.Stage == PlanStage.PlanInitialize) || plan.Stage == PlanStage.CompleteEvaluate)
                        continue;
                    PlanDynamicManager dynamicManager = new PlanDynamicManager();
                    PlanDynamicState stage = dynamicManager.GetPlanDynamicState(plan.PlanDefineID, plan.Period.ID);
                    if (stage == PlanDynamicState.Unknown)
                        continue;
                    else
                        result.Add(plan);
                }
            }
            return result;
        }
        internal PlanInfo Get(string planInfoID)
        {
            PlanDac dac = new PlanDac();
            PlanItemModelDac modelDac = new PlanItemModelDac();
            PlanInfo planInfo = dac.GetPlanBaseInfo(planInfoID);
            planInfo.PlanItems = dac.GetPlanItems(planInfoID);
            List<string> carbonCopyRecipientsID = dac.GetRecipientsID(planInfoID);
            if (carbonCopyRecipientsID != null && carbonCopyRecipientsID.Count > 0)
            {
                planInfo.CarbonCopyRecipient = GetUserInfos(carbonCopyRecipientsID);
            }
            // 创建人
            SysUser user = userService.GetUserByID(planInfo.Creator.ID);
            planInfo.Creator.Name = user == null ? "张三" : user.Name;
            // 审批人
            user = userService.GetUserByID(planInfo.MainRecipient.ID);
            planInfo.MainRecipient.Name = user == null ? "李四" : user.Name;
            return planInfo;
        }

        private List<SysUser> GetUserInfos(List<string> users)
        {
            List<SysUser> userInfos = new List<SysUser>();
            users.ForEach(userID =>
            {
                SysUser user = userService.GetUserByID(userID);
                userInfos.Add(user ?? new SysUser() { ID = "9999", Name = "某某某" });
            });
            return userInfos;
        }
        internal void PutSuperiorAssessment(List<PlanItem> planItemList)
        {
            PlanDac dac = new PlanDac();
            dac.PutSuperiorAssessment(planItemList);
        }
        public void PutSelfAssessment(List<PlanItem> planItemList)
        {
            PlanDac dac = new PlanDac();
            dac.PutSelfAssessment(planItemList);
        }
        public void UpdatePlanState(string planID, int approvalState)
        {
            PlanDac dac = new PlanDac();
            dac.UpdatePlanState(planID, approvalState);
        }
        public void UpdatePlanStage(string planID, int approvalState)
        {
            PlanDac dac = new PlanDac();
            dac.UpdatePlanStage(planID, approvalState);
        }
        public void DeletePlan(string planID)
        {
            var db = Utils.GetDb();
            PlanDac planDac = new PlanDac(db);
            db.BeginTransaction();
            try
            {
                planDac.DeletePlanInfo(planID);
                planDac.DeletePlanItem(planID);
                planDac.DeletePlanCarbonCopyRecipientInfo(planID);
                db.Commit();
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        public string SavePlanInfo(PlanInfo plan)
        {
            var db = Utils.GetDb();
            PlanDac planDac = new PlanDac(db);
            db.BeginTransaction();
            try
            {
                if (planDac.IsExistPlanInfo(plan))
                {
                    planDac.UpdatePlanInfo(plan);
                }
                else
                {
                    plan.ID = Guid.NewGuid().ToString();
                    planDac.AddPlanInfo(plan);
                }
                planDac.SavePlanItems(plan);
                planDac.SaveCarbonCopyRecipients(plan);

                db.Commit();
                return plan.ID;
            }
            catch(Exception e)
            {
                db.Rollback();
                throw;
            }
        }

        public bool IsExistPlanDefineRef(string planDefineID)
        {
            PlanDac dac = new PlanDac();
            return dac.IsExistPlanDefineRef(planDefineID);
        }
    }
}
