using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDefineManager
    {
        PlanDefineDac dac = new PlanDefineDac();
        public List<PlanDefine> GetPlanDefineList()
        {
            return dac.GetPlanDefineList();
        }
        public List<string> GetPlanDefineIDsByModelID(string modelID)
        {
            return dac.GetPlanDefineIDsByModelID(modelID);
        }

        public void DeletePlanDefine(string planDefineID)
        {
            var db = Utils.GetDb();
            PlanDefineDac dac = new PlanDefineDac(db);
            db.BeginTransaction();
            try
            {
                dac.DeletePlanDefine(planDefineID);
                dac.DeletePlanItemCustomization(planDefineID);
                dac.DeletePlanDefineScope(planDefineID);
                dac.DeletePlanDefineDynamic(planDefineID);
                db.Commit();
            }
            catch
            {
                db.Rollback();
                throw;
            }
        } 

        public string SavePlanDefine(PlanDefine planDefine)
        {
            var db = Utils.GetDb();
            PlanDefineDac dac = new PlanDefineDac(db);
            db.BeginTransaction();
            try
            {
                string id = dac.SavePlanDefine(planDefine);
                if (planDefine.PlanItemCustomization != null)
                {
                    dac.SaveCustomizedModel(planDefine.PlanItemCustomization, id);
                }
                dac.SavePlanDefineScope(planDefine);
                db.Commit();
                return id;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }

        internal void UpdatePlanDefineState(string planDefineID, PlanDefineState state)
        {
            dac.UpdatePlanDefineState(planDefineID, state);
        }

        internal PlanDefine GetPlanDefine(string planDefineID)
        {
            PlanDefine planDefine = dac.GetPlanDefineInfo(planDefineID);
            List<string> orgStrList = new List<string>();
            List<string> roleStrList = new List<string>();
            dac.AssemblyAllocation(planDefineID, orgStrList, roleStrList);
            List<Organization> orgList = new List<Organization>();
            List<Role> roleList = new List<Role>();
            EcpOrgService orgService = new EcpOrgService();
            if (orgStrList != null && orgStrList.Count > 0)
            {
                orgStrList.ForEach(orgID => {
                    Organization org = orgService.GetByID(orgID, Utils.GetTenantId());
                    if (org != null) orgList.Add(org);
                });
                planDefine.OrgList = orgList;
            }

            if (roleStrList != null && roleStrList.Count > 0)
            {
                Manager manager = new Manager();
                roleStrList.ForEach(roleID => {
                    Role role = manager.GetRoleByID(roleID);
                    if (role != null) roleList.Add(role);
                });
                planDefine.RoleList = roleList;
            }
            if (planDefine.PlanModel != null && planDefine.PlanModel.PlanItemModelContent != null && planDefine.PlanModel.PlanItemModelContent.Count > 0)
            {
                Dictionary<string, bool> customizedModelField = new Dictionary<string, bool>();
                if (planDefine.PlanItemCustomization != null && planDefine.PlanItemCustomization.CustomizedModelContent != null && planDefine.PlanItemCustomization.CustomizedModelContent.Count > 0)
                {
                    planDefine.PlanItemCustomization.CustomizedModelContent.ForEach(field =>
                    {
                        if (!customizedModelField.ContainsKey(field.ID))
                            customizedModelField.Add(field.ID, field.IsEnable);
                    });
                }
                List<PlanItemModelField> content = planDefine.PlanModel.PlanItemModelContent;
                for (int i = 0; i < content.Count; i++)
                {
                    content[i].IsEnable = true;
                    if (customizedModelField.ContainsKey(content[i].ID))
                        content[i].IsEnable = customizedModelField[content[i].ID];
                }
            }
            return planDefine;
        }

        public string UpdatePlanDefine(PlanDefine planDefine)
        {
            var db = Utils.GetDb();
            PlanDefineDac dac = new PlanDefineDac(db);
            db.BeginTransaction();
            try
            {
                dac.UpdatePlanDefine(planDefine);
                if (planDefine.PlanItemCustomization != null)
                {
                    if (dac.IsExistCustomizedModel(planDefine.ID))
                        dac.UpdateCustomizedModel(planDefine.PlanItemCustomization, planDefine.ID);
                    else
                        dac.SaveCustomizedModel(planDefine.PlanItemCustomization, planDefine.ID);
                }
                if (planDefine.OrgList != null && planDefine.OrgList.Count > 0 && planDefine.RoleList != null && planDefine.RoleList.Count > 0)
                {
                    dac.DeletePlanDefineScope(planDefine.ID);
                    dac.SavePlanDefineScope(planDefine);
                }
                db.Commit();
                return planDefine.ID;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
    }
}
