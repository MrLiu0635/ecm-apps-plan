using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanItemModelManager
    {
        PlanItemModelDac dac = new PlanItemModelDac();
        public List<PlanItemModel> GetPlanItemModel()
        {
            return dac.GetPlanItemModel();
        }

        internal PlanItemModel GetPlanItemModelByID(string modelID,string planDefineID)
        {
            return dac.GetPlanItemModelByID(modelID,planDefineID);
        }
    }
}
