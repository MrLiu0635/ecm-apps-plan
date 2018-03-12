using Inspur.EcmCloud.Apps.Plan.Service;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    [ApiVersion("1.0")]
    [Route("app/plan/api/v{version:apiVersion}/[controller]")]
    public class PlanDefineController
    {
       
        [HttpGet]
        public ListResponse<PlanDefine> Get()
        {
            ListResponse<PlanDefine> response = new ListResponse<PlanDefine>();
            response.Data = PlanDefineService.Current.GetPlanDefineList();
            return response;
        }

        [HttpGet, Route("{planDefineID}")]
        public InfoResponse<PlanDefine> Get(string planDefineID)
        {
            InfoResponse<PlanDefine> response = new InfoResponse<PlanDefine>();
            response.Data = PlanDefineService.Current.GetPlanDefine(planDefineID);
            return response;
        }

        [HttpDelete, Route("{planDefineID}")]
        public Response Delete(string planDefineID)
        {
            Response resp = new Response();
            if (PlanService.Current.IsExistPlanDefineRef(planDefineID)) {
                resp.Code = 201;
                resp.Msg = "计划定义已经应用于计划实例，不允许删除。";
                return resp;
            }
            if (PlanDynamicService.Current.IsExistPlanDefineRef(planDefineID))
            {
                resp.Code = 201;
                resp.Msg = "计划定义已经分配到计划管理实例，不允许删除。";
                return resp;
            }
            PlanDefineService.Current.DeletePlanDefine(planDefineID);
            return new Response();
        }

        [HttpPost]
        public InfoResponse<string> Post([FromBody] PlanDefine planDefine)
        {
            InfoResponse<string> response = new InfoResponse<string>();
            response.Data = PlanDefineService.Current.SavePlanDefine(planDefine);
            return response;
        }

        [HttpPut]
        public InfoResponse<string> Put([FromBody] PlanDefine planDefine)
        {
            InfoResponse<string> response = new InfoResponse<string>();
            response.Data = PlanDefineService.Current.UpdatePlanDefine(planDefine);
            return response;
        }

        [HttpPut, Route("state")]
        public Response Put([FromQuery] string planDefineID, [FromQuery] PlanDefineState state)
        {
            Response response = new Response();
            PlanDefineService.Current.UpdatePlanDefineState(planDefineID, state);
            return response;
        }
    }
}
