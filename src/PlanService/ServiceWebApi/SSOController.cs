using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    [Route("app/plan/api/[controller]")]
    public class SSOController : StateController
    {
        [HttpGet]
        public IActionResult Get([FromQuery]string url)
        {
            return Redirect(url);
        }
    }
}