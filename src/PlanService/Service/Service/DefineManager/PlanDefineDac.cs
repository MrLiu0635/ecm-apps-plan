using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Core;
using Inspur.GSP.Caf.Common;
using Inspur.GSP.Caf.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDefineDac
    {
        IGSPDatabase db;
        public PlanDefineDac()
        {
            db = Utils.GetDb();
        }
        public PlanDefineDac(IGSPDatabase db)
        {
            this.db = db;
        }


        private string planDefineBaseInfo = @"SELECT a.id, a.NAME, a.state, a.modelid,a.setid, a.typeid, b.NAME AS setname, c.CODE AS typecode, c.NAME AS typename, a.createdtime, a.lastmodifiedtime FROM plandefine AS a INNER JOIN periodset AS b ON a.setid = b.id INNER JOIN periodtype AS c ON a.typeid = c.id ";
        private string planDefineDetailInfo = @"select a.id,a.name,a.state,a.modelid,a.setid,a.typeid,b.name as modelname,b.modelcontent,c.name as setname,d.code as typecode, d.name as typename,e.id as customizeid,e.modeldesc,a.createdtime, a.lastmodifiedtime from plandefine as a inner join planitemmodel as b on a.modelid=b.id inner join periodset as c on a.setid=c.id inner join periodtype as d on a.typeid=d.id left join planitemcustomization as e on a.id=e.plandefineid ";

        internal void DeletePlanDefineScope(string planDefineID)
        {
            string deleteStr = $@"Delete from plandefineallocation where plandefineid = '{planDefineID}' and tenantid = '{Utils.GetTenantId()}'";
            db.ExecSqlStatement(deleteStr);
        }

        internal List<string> GetPlanDefineIDsByModelID(string modelID)
        {
            List<string> planDefineList = new List<string>();
            var queryStr = $@"SELECT a.id FROM plandefine AS a where (a.tenantid='0' or a.tenantid='{Utils.GetTenantId()}') and a.modelid = '{modelID}' order by a.lastmodifiedtime";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    planDefineList.Add(Convert.ToString(row["id"]));
                }

            }
            return planDefineList;
        }

        internal void UpdatePlanDefineState(string planDefineID, PlanDefineState state)
        {
            string updateSql = $@" update plandefine set state='{Convert.ToInt32(state)}' where id='{planDefineID}'";
            db.ExecSqlStatement(updateSql);
        }

        internal void DeletePlanItemCustomization(string planDefineID)
        {
            string deleteStr = $@"Delete from planitemcustomization where plandefineid = '{planDefineID}' and tenantid = '{Utils.GetTenantId()}'";
            db.ExecSqlStatement(deleteStr);
        }

        internal void DeletePlanDefineDynamic(string planDefineID)
        {
            string deleteStr = $@"Delete from plandefinedynamic where plandefineid = '{planDefineID}' and tenantid = '{Utils.GetTenantId()}'";
            db.ExecSqlStatement(deleteStr);
        }


        public List<PlanDefine> GetPlanDefineList()
        {
            List<PlanDefine> planDefineList = new List<PlanDefine>();
            var queryStr = planDefineBaseInfo + $@"where a.tenantid='0' or a.tenantid='{Utils.GetTenantId()}' order by a.lastmodifiedtime desc";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    planDefineList.Add(AssamblyPlanDefineBaseInfo(row));
                }

            }
            return planDefineList;
        }

        internal void SavePlanDefineScope(PlanDefine planDefine)
        {
            List<Organization> orgList = planDefine.OrgList;
            List<Role> roleList = planDefine.RoleList;
            if (orgList != null && orgList.Count > 0)
            {
                if (roleList != null && roleList.Count > 0)
                {
                    orgList.ForEach(org =>
                    {
                        roleList.ForEach(role =>
                        {
                            string insertSql = $@"insert into plandefineallocation(id, roleid, plandefineid, tenantid, orgid) values('{Guid.NewGuid().ToString()}','{role.ID}','{planDefine.ID}','{Utils.GetTenantId()}','{org.ID}')";
                            db.ExecSqlStatement(insertSql);
                        });
                    });
                }
                else
                {
                    throw new Exception("角色未指定。");
                }
            }
            else
            {
                throw new Exception("组织未指定。");
            }
        }

        internal void AssemblyAllocation(string planDefineID, List<string> orgList, List<string> roleList)
        {
            List<PlanDefine> planDefineList = new List<PlanDefine>();
            var querStr = $@"SELECT id, roleid, plandefineid, tenantid, orgid FROM plandefineallocation where tenantid='{Utils.GetTenantId()}' and plandefineid = '{planDefineID}'";
            var ds = db.ExecuteDataSet(querStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string orgID = row["orgid"] as string;
                    string roleID = row["roleid"] as string;
                    if (!orgList.Contains(orgID)) orgList.Add(orgID);
                    if (!roleList.Contains(roleID)) roleList.Add(roleID);
                }
            }
        }

        internal PlanDefine GetPlanDefineInfo(string planDefineID)
        {
            PlanDefine planDefine = new PlanDefine();
            var queryStr = planDefineDetailInfo + $@" where (a.tenantid='0' or a.tenantid='{Utils.GetTenantId()}') and a.id = '{planDefineID}' ";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                planDefine = AssamblyPlanDefineDetailInfo(ds.Tables[0].Rows[0]);
            }
            return planDefine;
        }

        public void DeletePlanDefine(string planDefineID)
        {
            string deletePlanItemSql = @"delete from plandefine where id={0}";
            db.ExecSqlStatement(deletePlanItemSql, planDefineID);
        }

        public string SavePlanDefine(PlanDefine planDefine)
        {
            planDefine.ID = Guid.NewGuid().ToString();
            string insertSql = $@"insert into plandefine(id,name,state,modelid,setid,typeid,tenantid, createdtime, lastmodifiedtime) values('{ planDefine.ID }','{planDefine.Name}','{Convert.ToInt32(planDefine.State)}','{planDefine.PlanModel.ID}','{planDefine.PeriodSet.ID}','{planDefine.PeriodType.ID}','{Utils.GetTenantId()}','{DateTime.Now}','{DateTime.Now}')";
            db.ExecSqlStatement(insertSql);
            return planDefine.ID;
        }

        public string UpdatePlanDefine(PlanDefine planDefine)
        {
            string updateSql = $@" update plandefine set name='{planDefine.Name}',state='{Convert.ToInt32(planDefine.State)}',modelid='{planDefine.PlanModel.ID}',setid='{planDefine.PeriodSet.ID}',typeid='{planDefine.PeriodType.ID}',lastmodifiedtime='{DateTime.Now}' where id='{planDefine.ID}'";
            db.ExecSqlStatement(updateSql);
            return planDefine.ID;
        }
        public void SaveCustomizedModel(CustomizedModel customizedModel, string defineID)
        {
            string insertStr = @"insert into planitemcustomization(id,tenantid,modeldesc,plandefineid) values({0},{1},{2},{3})";

            string colDesc = Serializer.JsonSerialize(customizedModel.CustomizedModelContent);
            db.ExecSqlStatement(insertStr, Guid.NewGuid().ToString(), Utils.GetTenantId(), colDesc, defineID);
        }
        public bool IsExistCustomizedModel(string planDefineID)
        {
            string sql = $@"select count(1) from planitemcustomization where plandefineid='{planDefineID}' and tenantid='{Utils.GetTenantId()}'";
            return Convert.ToInt32(db.ExecuteScalar(sql)) > 0;
        }
        public void UpdateCustomizedModel(CustomizedModel customizedModel, string  defineID)
        {
            string colDesc = Serializer.JsonSerialize(customizedModel.CustomizedModelContent);
            string updateStr = $@"Update planitemcustomization set modeldesc='{colDesc}' where plandefineid='{defineID}' and tenantid='{Utils.GetTenantId()}'";
            
            db.ExecSqlStatement(updateStr);
        }

        private PlanDefine AssamblyPlanDefineBaseInfo(DataRow row)
        {
            PlanDefine define = new PlanDefine();
            define.ID = Convert.ToString(row["id"]);
            define.Name = Convert.ToString(row["name"]);
            define.PeriodSet.ID = Convert.ToString(row["setid"]);
            define.PeriodSet.Name = Convert.ToString(row["setname"]);
            define.PeriodType.ID = Convert.ToString(row["typeid"]);
            define.PeriodType.Code = Convert.ToString(row["typecode"]);
            define.PeriodType.Name = Convert.ToString(row["typename"]);
            define.PlanModel.ID = Convert.ToString(row["modelid"]);
            define.State = (PlanDefineState)Convert.ToInt32(row["state"]);
            return define;
        }

        private PlanDefine AssamblyPlanDefineDetailInfo(DataRow row)
        {
            PlanDefine define = AssamblyPlanDefineBaseInfo(row);
            define.PlanModel.Name= Convert.ToString(row["modelname"]);
            string modelContent = Convert.ToString(row["modelcontent"]);
            if (!string.IsNullOrEmpty(modelContent))
                define.PlanModel.PlanItemModelContent = Serializer.JsonDeserialize<List<PlanItemModelField>>(modelContent);
            define.PlanItemCustomization.ID = Convert.ToString(row["customizeid"]);
            string coleDesc = Convert.ToString(row["modeldesc"]);
            if (!string.IsNullOrEmpty(coleDesc))
            define.PlanItemCustomization.CustomizedModelContent = Serializer.JsonDeserialize<List<CustomizedModelField>>(coleDesc);
            return define;
        }

    }
}
