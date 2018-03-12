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
    public class PlanController : StateController
    {
        [HttpGet]
        public ListResponse<PlanInfo> Get([FromQuery] PlanFilter filter)
        {
            List<PlanInfo> list = PlanService.Current.Get(filter);
            ListResponse<PlanInfo> response = new ListResponse<PlanInfo>();
            if (list != null && list.Count > 0)
            {
                response.Data = list;
            }
            return response;
        }
        [HttpGet,Route("teamhistory")]
        public ListResponse<PlanInfo> GetTeamCompletedPlan()
        {
            List<PlanInfo> list = PlanService.Current.GetTeamCompletedPlan();
            ListResponse<PlanInfo> response = new ListResponse<PlanInfo>();
            if (list != null && list.Count > 0)
            {
                response.Data = list;
            }
            return response;
        }
        [HttpGet, Route("teamcurrent")]
        public ListResponse<PlanInfo> GetTeamCurrentPlan()
        {
            List<PlanInfo> list = PlanService.Current.GetTeamCurrentPlan();
            ListResponse<PlanInfo> response = new ListResponse<PlanInfo>();
            if (list != null && list.Count > 0)
            {
                response.Data = list;
            }
            return response;
        }
        [HttpGet, Route("{planID}")]
        public InfoResponse<PlanInfo> Get(string planID)
        {
            PlanInfo planInfo = PlanService.Current.Get(planID);
            InfoResponse<PlanInfo> response = new InfoResponse<PlanInfo>();
            if (planInfo != null && !string.IsNullOrEmpty(planInfo.ID))
            {
                response.Data = planInfo;
            }
            else
            {
                response.Code = 204;
                response.Msg = "The Plan does not exist";
            }
            return response;
        }

        [HttpPost]
        public InfoResponse<string> Post([FromBody] PlanInfo plan)
        {
            InfoResponse<string> response = new InfoResponse<string>();
            response.Data= PlanService.Current.SavePlanInfo(plan);
            return response;
        }

        [HttpDelete, Route("{planID}")]
        public Response Delete(string planID)
        {
            PlanService.Current.DeletePlan(planID);
            return new Response();
        }

        [HttpPut, Route("approval/{planID}/{approvalState}")]
        public Response Put(string planID, int approvalState)
        {
            PlanService.Current.UpdatePlanState(planID, approvalState);
            return new Response();
        }
        [HttpPut, Route("stage/{planID}/{approvalState}")]
        public Response PutPlanStage(string planID, int approvalState)
        {
            PlanService.Current.UpdatePlanStage(planID, approvalState);
            return new Response();
        }
        [HttpPut, Route("selfassessment")]
        public Response PutSelfAssessment([FromBody] List<PlanItem> planItemList)
        {
            PlanService.Current.PutSelfAssessment(planItemList);
            return new Response();
        }

        [HttpPut, Route("superiorassessment")]
        public Response PutSuperiorAssessment([FromBody] List<PlanItem> planItemList)
        {
            PlanService.Current.PutSuperiorAssessment(planItemList);
            return new Response();
        }
    }
}
