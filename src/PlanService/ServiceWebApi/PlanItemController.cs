using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.EcmCloud.Apps.Plan.Service.Service;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    [ApiVersion("1.0")]
    [Route("app/plan/api/v{version:apiVersion}/[controller]")]
    public class PlanItemController : StateController
    {
        /// <summary>
        /// 获取上级计划
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("superior/{periodID}")]
        public ListResponse<PlanInfo> GetSuperiorPlan(string periodID)
        {
            ListResponse<PlanInfo> response = new ListResponse<PlanInfo>();
            response.Data = PlanItemService.Current.GetSuperiorPlan(periodID);
            return response;
        }
        /// <summary>
        /// 获取父计划
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("parent/{periodID}")]
        public ListResponse<PlanInfo> GetParentPlan(string periodID)
        {
            ListResponse<PlanInfo> response = new ListResponse<PlanInfo>();
            response.Data = PlanItemService.Current.GetParentPlan(periodID);
            return response;
        }
        /// <summary>
        /// 获取计划项
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("{planID}")]
        public ListResponse<PlanItem> GetPlanItems(string planID)
        {
            ListResponse<PlanItem> response = new ListResponse<PlanItem>();
            response.Data = PlanItemService.Current.GetPlanItems(planID);
            return response;
        }

        /// <summary>
        /// 保存计划项
        /// </summary>
        /// <returns></returns>
        [HttpPut,Route("{planDefineID}/{periodID}")]
        public InfoResponse<string> Put(string planDefineID,string periodID,[FromBody]PlanItem planItem)
        {
            InfoResponse<string> response = new InfoResponse<string>();
            response.Data = PlanItemService.Current.SavePlanItem(planDefineID, periodID, planItem);
            return response;
        }
    }
}
