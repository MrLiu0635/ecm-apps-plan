using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
//using Inspur.EcmCloud.Apps.PlanManager.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{

    [ApiVersion("1.0")]
    public class PlanController : StateController
    {
        // GET api/Plan
        [HttpGet, Route("app/plan/api/v{version:apiVersion}/[controller]")]
        public List<WorkReport> Get([FromQuery]WRQueryFilter filterCondition)
        {
            return PlanService.Current.GetWorkReportList(filterCondition);
        }

        //GET workreport
        [HttpGet, Route("app/plan/api/v{version:apiVersion}/[controller]/{workReportID}")]
        public WorkReport GetWorkReportByID(string workReportID)
        {
            if (string.IsNullOrEmpty(workReportID)) return null;
            return PlanService.Current.GetWorkReportByID(workReportID);
        }
        //GET ComponentModelList
        [HttpGet, Route("app/plan/api/v{version:apiVersion}/[controller]/wrtype/{wrTypeCode}")]
        public WorkReportModel GetComponentModelList(string wrTypeCode)
        {
            if (string.IsNullOrEmpty(wrTypeCode)) return null;
            return PlanService.Current.GetComponentModelList(wrTypeCode);
        }

        [HttpDelete, Route("app/plan/api/v{version:apiVersion}/[controller]")]
        public void Delete([FromBody] List<string> workReportIDs)
        {
            PlanService.Current.DeleteWorkReports(workReportIDs);
        }

        [HttpPost, Route("app/plan/api/v{version:apiVersion}/[controller]")]
        public List<string> Post([FromBody] List<WorkReport> workReports)
        {
            return PlanService.Current.AddWorkReports(workReports);
        }

        [HttpPut, Route("app/plan/api/v{version:apiVersion}/[controller]")]
        public void Put([FromBody] WorkReport workReport, bool isReNotice)
        {
            PlanService.Current.UpdateWorkReport(workReport, isReNotice);
        }
    }
}
