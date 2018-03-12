using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.EcmCloud.Apps.Plan.Service;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    [ApiVersion("1.0")]
    [Route("app/plan/api/v{version:apiVersion}/[controller]")]
    public class PlanDynamicController : StateController
    {
        [HttpGet, Route("{roleID}")]
        public ListResponse<PlanDynamic> GetOnUsedPlanDefineList(string roleID)
        {
            ListResponse<PlanDynamic> response = new ListResponse<PlanDynamic>();
            response.Data = PlanDynamicService.Current.GetOnUsedPlanDynamicList(roleID);
            return response;
        }

        [HttpGet, Route("all")]
        public ListResponse<PlanDynamic> GetPlanDefineList()
        {
            ListResponse<PlanDynamic> response = new ListResponse<PlanDynamic>();
            response.Data = PlanDynamicService.Current.GetPlanDynamicList();
            return response;
        }
        [HttpGet, Route("{planDefineID}/{periodID}")]
        public InfoResponse<int> GetPlanDynamicState(string planDefineID, string periodID)
        {
            InfoResponse<int> response = new InfoResponse<int>();
             response.Data =Convert.ToInt32(PlanDynamicService.Current.GetPlanDynamicState(planDefineID, periodID));
            return response;
        }
        [HttpDelete]
        public Response Delete([FromBody]List<string> planDynamicIDs)
        {
            PlanDynamicService.Current.DeletePlanDynamic(planDynamicIDs);
            Response response = new Response();
            return response;
            //return MethodResponse.Instance.DoWork(() =>
            //{
            //    PlanDynamicService.Current.DeletePlanDynamic(planDynamicIDs);
            //}
            //);
        }

        [HttpPost] 
        public Response Post([FromBody] PlanDynamic entity)
        {
            PlanDynamicService.Current.AddPlanDynamic(entity);
            Response response = new Response();
            return response;
            //return MethodResponse.Instance.DoWork(() =>
            //{
            //    PlanDynamicService.Current.AddPlanDynamic(entity);
            //}
            //);
        }

        [HttpPut]
        public Response Put([FromBody] PlanDynamic entity)
        {
            PlanDynamicService.Current.UpdatePlanDynamic(entity);
            Response response = new Response();
            return response;
            //return MethodResponse.Instance.DoWork(() =>
            //{
            //    PlanDynamicService.Current.UpdatePlanDynamic(entity);
            //}
            //);
        }
    }

    
}
