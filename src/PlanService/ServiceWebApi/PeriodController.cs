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
    public class PeriodController : StateController
    {

        [HttpGet, Route("sets/all")]
        public ListResponse<PeriodSet> GetAllPeriodSets()
        {
            List<PeriodSet> sets = PeriodService.Current.GetAllPeriodSets();
            ListResponse<PeriodSet> response = new ListResponse<PeriodSet>();
            if (sets != null && sets.Count > 0)
            {
                response.Data = sets;
            }
            return response;
        }

        [HttpGet, Route("sets")]
        public ListResponse<PeriodSet> GetPeriodSets()
        {
            List<PeriodSet> sets = PeriodService.Current.GetMyPeriodSets();
            ListResponse<PeriodSet> response = new ListResponse<PeriodSet>();
            if (sets != null && sets.Count > 0)
            {
                response.Data = sets;
            }
            return response;
        }

        [HttpGet]
        public ListResponse<Period> GetPeriodByFilter([FromQuery] PeriodFilter periodFilter)
        {
            if (periodFilter == null) return new ListResponse<Period>();
            List<Period> cycles = PeriodService.Current.GetPeriodByFilter(periodFilter);
            ListResponse<Period> response = new ListResponse<Period>();
            if (cycles != null && cycles.Count > 0)
            {
                response.Data = cycles;
            }
            return response;
        }

        [HttpGet, Route("types")]
        public ListResponse<PeriodType> GetPeriodType()
        {
            List<PeriodType> types = PeriodService.Current.GetPeriodTypes();
            ListResponse<PeriodType> response = new ListResponse<PeriodType>();
            if (types != null && types.Count > 0)
            {
                response.Data = types;
            }
            return response;
        }

        [HttpPost, Route("sets")]
        public Response PostMyPeriodSets([FromBody]List<string> PeriodSetIDList)
        {
            PeriodService.Current.UpdateMyPeriodSets(PeriodSetIDList);
            Response response = new Response();
            return response;
        }
    }
}
