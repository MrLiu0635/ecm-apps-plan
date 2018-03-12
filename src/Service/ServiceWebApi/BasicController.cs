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
    public class BasicController : StateController
    {
        // GET api/Plan
        [HttpGet("{userName}")]
        public List<User> GetUsersByUserName(string userName)
        {
            return BasicService.Current.GetUsersByUserName(userName);
        }
    }
}
