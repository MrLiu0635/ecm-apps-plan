using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    [ApiVersion("1.0")]
    [Route("app/plan/api/v{version:apiVersion}/[controller]")]
    public class PlanItemModelController : StateController
    {
        [HttpGet, Route("all")]
        public ListResponse<PlanItemModel> Get()
        {
            ListResponse<PlanItemModel> response = new ListResponse<PlanItemModel>();
            response.Data = PlanItemModelService.Current.GetPlanItemModel();
            return response;
        }
        

        [HttpGet]
        public InfoResponse<PlanItemModel> GetPlanItemModel([FromQuery] string modelID, [FromQuery] string planDefineID)
        {
            InfoResponse<PlanItemModel> response = new InfoResponse<PlanItemModel>();
            response.Data = PlanItemModelService.Current.GetPlanItemModelByID(modelID,planDefineID);
            return response;
        }
    }
}
