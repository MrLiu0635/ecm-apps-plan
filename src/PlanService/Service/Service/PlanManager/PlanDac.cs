using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.GSP.Caf.DataAccess;
using Newtonsoft.Json;
using Inspur.GSP.Caf.Common;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDac
    {
        IGSPDatabase db;
        public PlanDac()
        {
            db = Utils.GetDb();
        }

        public PlanDac(IGSPDatabase db)
        {
            this.db = db;
        }

        internal List<PlanInfo> Get(PlanFilter filter)
        {
            List<PlanInfo> list = new List<PlanInfo>();
            StringBuilder stringBuilder = new StringBuilder($@"SELECT DISTINCT a.id, a.NAME, c.NAME AS periodname,c.id as periodid, c.alias as periodalias, c.parentid as periodparentid, d.name as typename, d.code AS typecode, a.mainrecipient, a.state, a.stage, a.tenantid, a.userid, a.createdtime, a.lastmodifiedtime,a.approvalinstance, a.plandefineid FROM plan a LEFT JOIN carboncopyplanrecipients b ON b.tenantid = '{Utils.GetTenantId()}' AND a.id = b.planid LEFT JOIN period c ON c.id = a.periodid Left join periodType d on d.id = c.typeid WHERE a.tenantid = '{Utils.GetTenantId()}'");
            // 状态
            if (!filter.State.Equals(PlanState.Unknown))
                stringBuilder.Append($@" and state = '{(int)filter.State}'");
            // 阶段
            if (!filter.Stage.Equals(PlanStage.Unknown))
                stringBuilder.Append($@" and stage = '{(int)filter.Stage}'");
            // 发送人
            if (filter.Senders != null && filter.Senders.Count > 0)
            {
                stringBuilder.Append(@" AND a.userid in ('");
                stringBuilder.Append(string.Join("','", filter.Senders.ToArray()));
                stringBuilder.Append(@"') ");
            }
            // 主送人
            if (filter.FirRecips != null && filter.FirRecips.Count > 0)
            {
                stringBuilder.Append(@" AND a.mainrecipient in ('");
                stringBuilder.Append(string.Join("','", filter.FirRecips.ToArray()));
                stringBuilder.Append(@"') ");
            }
            // 抄送人
            if (filter.SecRecips != null && filter.SecRecips.Count > 0)
            {
                stringBuilder.Append(@" AND b.recipientid in ('");
                stringBuilder.Append(string.Join("','", filter.SecRecips.ToArray()));
                stringBuilder.Append(@"') ");
            }
            // 相关计划定义
            if (filter.PlanDefines != null && filter.PlanDefines.Count > 0)
            {
                stringBuilder.Append(@" AND a.plandefineid in ('");
                stringBuilder.Append(string.Join("','", filter.PlanDefines.ToArray()));
                stringBuilder.Append(@"') ");
            }
            // 相关周期
            if (filter.Periods != null && filter.Periods.Count > 0)
            {
                stringBuilder.Append(@" AND a.periodid in ('");
                stringBuilder.Append(string.Join("','", filter.Periods.ToArray()));
                stringBuilder.Append(@"') ");
            }
            DateTime defaultDateTime = new DateTime();
            if (filter.StartTime != null && !filter.StartTime.Equals(defaultDateTime))
                stringBuilder.Append($@" AND a.createdtime >= '{filter.StartTime}' ");
            if (filter.EndTime != null && !filter.EndTime.Equals(defaultDateTime))
                stringBuilder.Append($@" AND a.createdtime <= '{filter.EndTime}' ");
            if (!string.IsNullOrEmpty(filter.PeriodTypeID))
                stringBuilder.Append($@" and c.typeid = '{filter.PeriodTypeID}'");
            stringBuilder.Append(@" order by a.createdtime desc");
            var ds = db.ExecuteDataSet(stringBuilder.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPlanInfo(row));
                }
            }
            return list;
        }
        
        internal List<string> GetRecipientsID(string planInfoID)
        {
            List<string> list = new List<string>();
            var queryStr = $@"select recipientid from carboncopyplanrecipients where planid = '{planInfoID}'";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(Convert.ToString(row["recipientid"]));
                }
            }
            return list;
        }

        internal List<PlanItem> GetPlanItems(string planInfoID)
        {
            List<PlanItem> list = new List<PlanItem>();
            var queryStr = $@"select a.id,a.name,a.planid,a.starttime,a.endtime,a.itemorder,a.parentplanitemid,a.sourceplanitemid,a.planitemcontent,a.createdtime,a.lastmodifiedtime,a.selfassessment,a.assessmentofsuperior,a.selfassessmentscore,a.assessmentofsuperiorscore,a.summarycontent,a.tenantid,a.weight from planitem a join plan b on b.tenantid = '{Utils.GetTenantId()}' and b.id = a.planid where a.tenantid = '{Utils.GetTenantId()}' and b.id = '{planInfoID}'";
            queryStr += " order by a.itemorder";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPlanItem(row));
                }
            }
            return list;
        }

        internal bool IsExistPlanDefineRef(string planDefineID)
        {
            if (string.IsNullOrEmpty(planDefineID))
                return false;
            var result = db.ExecuteScalar(@"select count(1) from plan where plandefineid={0}", planDefineID);
            return Convert.ToInt32(result) > 0;
        }

        private PlanItem AssamblyPlanItem(DataRow row)
        {
            PlanItem item = new PlanItem();
            item.ID = Convert.ToString(row["id"]);
            item.Name = Convert.ToString(row["name"]);
            item.StartTime = Convert.ToDateTime(row["starttime"]);
            item.EndTime = Convert.ToDateTime(row["endtime"]);
            item.ParentPlanItemID = Convert.ToString(row["parentplanitemid"]);
            item.SourcePlanItemID = Convert.ToString(row["sourceplanitemid"]);
            item.Order = Convert.ToInt32(row["itemorder"]);
            item.CreatedTime = Convert.ToDateTime(row["createdtime"]);
            item.LastModifiedTime = Convert.ToDateTime(row["lastmodifiedtime"]);
            item.SelfAssessment = Convert.ToString(row["selfassessment"]);
            item.AssessmentOfSuperior = Convert.ToString(row["assessmentofsuperior"]);
            item.SelfAssessmentScore = Convert.ToString(row["selfassessmentscore"]);
            item.AssessmentScoreOfSuperior = Convert.ToString(row["assessmentofsuperiorscore"]);
            string content = Convert.ToString(row["planitemcontent"]);
            item.PlanItemContent = JsonConvert.DeserializeObject<List<PlanItemColContent>>(content);
            item.Weight= Convert.ToInt32(row["weight"]);
            return item;
        }

        internal PlanInfo GetPlanBaseInfo(string planInfoID)
        {
            PlanInfo plan = new PlanInfo();
            StringBuilder stringBuilder = new StringBuilder($@"select a.id, a.NAME, b.NAME AS periodname,b.id as periodid, b.alias as periodalias, b.parentid as periodparentid, c.name as typename, c.code AS typecode, a.mainrecipient, a.state, a.stage, a.tenantid, a.userid, a.createdtime, a.lastmodifiedtime,a.approvalinstance, a.plandefineid 
from plan a left join period b on b.id = a.periodid left join periodtype c on c.id = b.typeid where a.tenantid = '{Utils.GetTenantId()}' and a.id = '{planInfoID}'");
            var ds = db.ExecuteDataSet(stringBuilder.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                plan = AssamblyPlanInfo(ds.Tables[0].Rows[0]);
            }
            return plan;
        }

        private PlanInfo AssamblyPlanInfo(DataRow row)
        {
            PlanInfo plan = new PlanInfo();
            plan.ID = Convert.ToString(row["id"]);
            plan.MainRecipient = new SysUser() { ID = Convert.ToString(row["mainrecipient"] )};
            plan.Name = Convert.ToString(row["name"]);
            plan.State = (PlanState)Convert.ToInt32(row["state"]);
            plan.Stage = (PlanStage)Convert.ToInt32(row["stage"]);
            if (plan.Stage == PlanStage.SelfEvaluate)
            {
                PlanDynamicManager dynamicManager = new PlanDynamicManager();
                PlanDynamicState stage = dynamicManager.GetPlanDynamicState(Convert.ToString(row["plandefineid"]), Convert.ToString(row["periodid"]));
                if (stage == PlanDynamicState.InExecution|| stage== PlanDynamicState.ToSet)
                    plan.Stage = PlanStage.PlanExecution;
            }
            if (!row.IsNull("periodid"))
            {
                plan.Period = new Period
                {
                    ID = Convert.ToString(row["periodid"]),
                    Name = Convert.ToString(row["periodname"]),
                    ParentID = Convert.ToString(row["periodparentid"]),
                    Type = new PeriodType() { Code = Convert.ToString(row["typecode"]), Name = Convert.ToString(row["typename"]) }
                };
            }
            plan.Creator = new EcpUserService().GetUserByID(Convert.ToString(row["userid"]));
            plan.CreatedTime = Convert.ToDateTime(row["createdtime"]);
            plan.PlanDefineID = Convert.ToString(row["plandefineid"]);
            return plan;
        }
        internal void PutSuperiorAssessment(List<PlanItem> planItemList)
        {
            string updateSql = @"update planitem set assessmentofsuperior={0},assessmentofsuperiorscore={1} where id={2}";
            foreach (PlanItem planItem in planItemList)
            {
                db.ExecSqlStatement(updateSql, planItem.AssessmentOfSuperior, planItem.AssessmentScoreOfSuperior, planItem.ID);
            }
        }
        public void PutSelfAssessment(List<PlanItem> planItemList)
        {
            string updateSql = @"update planitem set selfassessment={0},selfassessmentscore={1},summarycontent={2} where id={3}";
            foreach (PlanItem planItem in planItemList)
            {
                string summaryContent = Serializer.JsonSerialize(planItem.SummaryContent);
                db.ExecSqlStatement(updateSql, planItem.SelfAssessment, planItem.SelfAssessmentScore, summaryContent, planItem.ID);
            }
        }
        public void UpdatePlanState(string planID, int approvalState)
        {
            string updateSql = @"update plan set state={0} where id={1}";
            db.ExecSqlStatement(updateSql,Convert.ToInt32(approvalState), planID);
        }

        public void UpdatePlanStage(string planID, int approvalState)
        {
            string updateSql = @"update plan set stage={0} where id={1}";
            db.ExecSqlStatement(updateSql, Convert.ToInt32(approvalState), planID);
        }
        public void DeletePlanInfo(string planID)
        {
            string deletePlanSql = @"delete from plan  where id={0}";
            db.ExecSqlStatement(deletePlanSql, planID);
        }

        internal void DeletePlanCarbonCopyRecipientInfo(string planID)
        {
            string deletePlanCarbonCoypyRecipient = @"delete from carboncopyplanrecipients where planid={0}";
            db.ExecSqlStatement(deletePlanCarbonCoypyRecipient, planID);
        }

        internal void DeletePlanItem(string planID)
        {
            string deletePlanItemSql = @"delete from planItem where planid={0}";
            db.ExecSqlStatement(deletePlanItemSql, planID);
        }
        internal void UpdatePlanItem(PlanItem planItem)
        {
            string updatePlanItemSql =$@"update planItem set parentplanitemid='{planItem.ParentPlanItemID}',sourceplanitemid='{planItem.SourcePlanItemID}',planitemcontent='{planItem.PlanItemContent}',lastmodifiedtime='{DateTime.Now}',weight='{planItem.Weight}' where planid='{planItem.ID}'";
            db.ExecSqlStatement(updatePlanItemSql);
        }
        internal void UpdatePlanInfo(PlanInfo plan)
        {
            string updateSql = $@"update plan set name='{plan.Name}',mainrecipient='{plan.MainRecipient.ID}',lastmodifiedtime='{DateTime.Now}' where id='{plan.ID}'";
            db.ExecSqlStatement(updateSql);
        }

        internal void AddPlanInfo(PlanInfo plan)
        {
            string insertSql = $@"insert into plan(id,name,periodid,mainrecipient,state,stage,tenantid,userid,createdtime,lastmodifiedtime,plandefineid) values('{plan.ID}','{plan.Name}','{plan.Period.ID}','{plan.MainRecipient.ID}','{Convert.ToInt32(plan.State)}','{Convert.ToInt32(plan.Stage)}','{Utils.GetTenantId()}','{Utils.GetUserId()}','{DateTime.Now}','{DateTime.Now}','{plan.PlanDefineID}')";
            db.ExecSqlStatement(insertSql);
        }

        internal void SavePlanItems(PlanInfo plan)
        {
            string deletePlanItem = "delete from planitem where planid={0}";
            db.ExecSqlStatement(deletePlanItem, plan.ID);
            foreach (PlanItem item in plan.PlanItems)
            {
                SavePlanItem(plan.ID, item);
            }
        }

        internal void SaveCarbonCopyRecipients(PlanInfo plan)
        {
            DeletePlanCarbonCopyRecipientInfo(plan.ID);
            foreach (SysUser user in plan.CarbonCopyRecipient)
            {
                SaveCarbonCopyRecipient(plan.ID, user.ID);
            }
        }

        internal void SavePlanItem(string planID, PlanItem item)
        {
            string content = Serializer.JsonSerialize(item.PlanItemContent);
            string insertSql = $@"insert into planitem(id,name,planid,starttime,endtime,itemorder,parentplanitemid,sourceplanitemid,planitemcontent,tenantid,createdtime,lastmodifiedtime,weight) values('{Guid.NewGuid().ToString()}','{item.Name}','{planID}','{item.StartTime}','{item.EndTime}','{item.Order}','{item.ParentPlanItemID}','{item.SourcePlanItemID}','{content}','{Utils.GetTenantId()}','{DateTime.Now}','{DateTime.Now}','{item.Weight}')";
            db.ExecSqlStatement(insertSql);
        }
        internal bool IsExistPlanInfo(PlanInfo plan)
        {
            if (string.IsNullOrEmpty(plan.ID))
                return false;
            var result = db.ExecuteScalar(@"select count(1) from plan where id={0}", plan.ID);
            return Convert.ToInt32(result) > 0;
        }
        internal void SaveCarbonCopyRecipient(string planID, string userID)
        {
            string insertSql = "insert into carboncopyplanrecipients(id,recipientid,planid,tenantid) values({0},{1},{2},{3})";
            db.ExecSqlStatement(insertSql, Guid.NewGuid().ToString(), userID, planID, Utils.GetTenantId());
        }
    }
}
