using Inspur.GSP.Caf.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    internal class PlanDynamicDac
    {
        IGSPDatabase db;
        public PlanDynamicDac()
        {
            db = Utils.GetDb();
        }
        /// <summary>
        /// 获取用户当前启用状态的计划定义
        /// 需要根据用户的角色和组织信息获取，后续添加
        /// </summary>
        /// <returns></returns>
        public List<PlanDynamic> GetOnUsedPlanDynamicList(string roleID, string orgID)
        {
            List<PlanDynamic> planDefineList = new List<PlanDynamic>();
            var queryStr = new StringBuilder($@"
select a.id,a.name,a.state,a.modelid,a.setid,a.typeid,a.tenantid,c.id as dynid,c.state as stage,d.id as periodid,d.name as periodname,d.parentid as periodparentid
from plandefine as a 
inner join plandefineallocation as b on a.id=b.plandefineid 
inner join plandefinedynamic as c on a.id=c.plandefineid 
inner join period as d on c.periodid=d.id  
where a.tenantid='0' or a.tenantid='{Utils.GetTenantId()}' and a.state='1' and b.roleid='{roleID}' and b.orgid='{orgID}'");
            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    planDefineList.Add(AssamblyPlanDynamic(row));
                }

            }
            return planDefineList;
        }

        internal bool IsExistPlanDefineRef(string planDefineID)
        {
            if (string.IsNullOrEmpty(planDefineID))
                return false;
            var result = db.ExecuteScalar(@"select count(1) from plandefinedynamic where plandefineid={0}", planDefineID);
            return Convert.ToInt32(result) > 0;
        }

        public PlanDynamicState GetPlanDynamicState(string planDefineID, string periodID)
        {
            var queryStr = new StringBuilder($@"
select b.state
from plandefine as a 
inner join plandefinedynamic as b on a.id=b.plandefineid 
inner join period as c on b.periodid=c.id  
where a.tenantid='0' or a.tenantid='{Utils.GetTenantId()}' and a.state='1' and a.id='{planDefineID}' and c.id='{periodID}'");
            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                return (PlanDynamicState)ds.Tables[0].Rows[0]["state"];
            }
            return PlanDynamicState.Unknown;
        }
        public List<PlanDynamic> GetPlanDynamicList()
        {
            List<PlanDynamic> planDefineList = new List<PlanDynamic>();
            string sql = "select a.id,a.name,a.state,a.modelid,a.setid,a.typeid,a.tenantid,c.id as dynid, c.state as stage,d.id as periodid,d.name as periodname,d.parentid as periodparentid from plandefine as a inner join plandefinedynamic as c on a.id = c.plandefineid inner join period as d on c.periodid = d.id where c.tenantid = {0} ";
            var ds = db.ExecuteDataSet(sql, Utils.GetTenantId());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    planDefineList.Add(AssamblyPlanDynamic(row));
                }

            }
            return planDefineList;
        }

        public void DeletePlanDynamic(List<string> planDynamicIDs)
        {
            StringBuilder sqlBuilder = new StringBuilder(100);
            sqlBuilder.Append("delete from plandefinedynamic where id in (");
            int count = planDynamicIDs.Count;
            for (int i = 0; i < count; i++)
            {
                sqlBuilder.Append("{" + i + "},");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(")");
            var sql = sqlBuilder.ToString();
            db.ExecSqlStatement(sql, planDynamicIDs.ToArray());

        }

        public void AddPlanDynamic(PlanDynamic entity)
        {
            string sql ="insert into plandefinedynamic(id, plandefineid, periodid, state, tenantid) values({0}, {1}, {2}, {3}, {4})";
            db.ExecSqlStatement(sql, Guid.NewGuid().ToString(), entity.PlanDefine.ID, entity.Period.ID, entity.State, Utils.GetTenantId());
        }

        public void UpdatePlanDynamic(PlanDynamic entity)
        {
            string sql = "update plandefinedynamic set plandefineid = {0}, periodid = {1}, state = {2}, tenantid={3} where id = {4}";
            db.ExecSqlStatement(sql, entity.PlanDefine.ID, entity.Period.ID, entity.State, Utils.GetTenantId(), entity.ID);
        }

        private PlanDynamic AssamblyPlanDynamic(DataRow row)
        {
            PlanDynamic planDynamic = new PlanDynamic {
                PlanDefine = new PlanDefine {
                    PlanModel = new PlanItemModel(),
                    PeriodSet = new PeriodSet(),
                    PeriodType = new PeriodType()
                },
                Period = new Period()
            };
            planDynamic.ID = Convert.ToString(row["dynid"]);
            planDynamic.PlanDefine.ID = Convert.ToString(row["id"]);
            planDynamic.PlanDefine.Name = Convert.ToString(row["name"]);
            planDynamic.PlanDefine.PlanModel.ID = Convert.ToString(row["modelid"]);
            planDynamic.PlanDefine.PeriodSet.ID = Convert.ToString(row["setid"]);
            planDynamic.PlanDefine.PeriodType.ID = Convert.ToString(row["typeid"]);
            planDynamic.PlanDefine.State = (PlanDefineState)Convert.ToInt32(row["state"]);
            planDynamic.State = (PlanDynamicState)Convert.ToInt32(row["stage"]);
            planDynamic.Period.ID = Convert.ToString(row["periodid"]);
            planDynamic.Period.Name = Convert.ToString(row["periodname"]);
            planDynamic.Period.ParentID = Convert.ToString(row["periodparentid"]);
            return planDynamic;
        }
    }
}
