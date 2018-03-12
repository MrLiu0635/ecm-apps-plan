using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.GSP.Caf.DataAccess;
using System.Data;
using Inspur.GSP.Caf.Common;

namespace Inspur.EcmCloud.Apps.Plan.Service
{ 
    public class PlanItemModelDac
    {
        IGSPDatabase db;
        public PlanItemModelDac()
        {
            db = Utils.GetDb();
        }
        public List<PlanItemModel> GetPlanItemModel()
        {
            List<PlanItemModel> planModelList = new List<PlanItemModel>();
            var queryStr = $@"select id,name from planitemmodel where tenantid='0' or tenantid='{Utils.GetTenantId()}'";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    planModelList.Add(AssamblyPlanItemModelInfo(row));
                }

            }
            return planModelList;
        }

        internal PlanItemModel GetPlanItemModelByID(string modelID,string planDefineID)
        {
            PlanItemModel planModel = new PlanItemModel();
            var queryStr = $@"select id,name,modelcontent from planitemmodel where id ='{modelID}' ";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                planModel = AssamblyPlanItemModel(row);
                if (!string.IsNullOrEmpty(planDefineID))
                {
                    var queryDescStr = $@"select id,modeldesc from planitemcustomization where plandefineid ='{planDefineID}'";
                    var dt = db.ExecuteDataSet(queryDescStr);
                    if (dt != null && dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
                    {
                        string modeldesc = dt.Tables[0].Rows[0]["modeldesc"] as string;
                        if (!string.IsNullOrEmpty(modeldesc))
                        {
                            //将数据库信息封装到实体类
                            var modelDesc = Serializer.JsonDeserialize<List<CustomizedModelField>>(modeldesc);
                            Dictionary<string, bool> modelDescDic = new Dictionary<string, bool>();
                            modelDesc.ForEach(desc =>
                            {
                                modelDescDic.Add(desc.ID, desc.IsEnable);
                            });
                            for (int i = planModel.PlanItemModelContent.Count - 1; i >= 0; i--)
                            {
                                string id = planModel.PlanItemModelContent[i].ID;
                                if (modelDescDic.ContainsKey(id) && !modelDescDic[id])
                                {
                                    planModel.PlanItemModelContent[i].IsEnable = false;
                                }
                            }
                        }
                    }
                }
            }
            return planModel;
        }

        public PlanItemModel AssamblyPlanItemModelInfo(DataRow row)
        {
            PlanItemModel planModel = new PlanItemModel();
            planModel.ID= Convert.ToString(row["id"]);
            planModel.Name = Convert.ToString(row["name"]);
            return planModel;
        }

        public PlanItemModel AssamblyPlanItemModel(DataRow row)
        {
            PlanItemModel planModel = new PlanItemModel();
            planModel.ID = Convert.ToString(row["id"]);
            planModel.Name = Convert.ToString(row["name"]);
            planModel.PlanItemModelContent = Serializer.JsonDeserialize<List<PlanItemModelField>>(Convert.ToString(row["modelcontent"]));
            foreach (PlanItemModelField colDesc in planModel.PlanItemModelContent)
                colDesc.IsEnable = true;
            return planModel;
        }
    }
}
