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
    public class CommonController
    {
        [HttpGet, Route("role")]
        public ListResponse<Role> GetRoles()
        {
            ListResponse<Role> response = new ListResponse<Role>();
            response.Data = OtherService.Current.GetRoles();
            return response;
        }

        [HttpGet, Route("org")]
        public ListResponse<Organization> GetOrgs(string parentOrgID)
        {
            ListResponse<Organization> response = new ListResponse<Organization>();
            response.Data = OtherService.Current.GetOrgs(parentOrgID);
            return response;
        }

        [HttpGet("{userName}")]
        public ListResponse<SysUser> GetUsers(string userName)
        {
            ListResponse<SysUser> response = new ListResponse<SysUser>();
            response.Data = OtherService.Current.GetUsersByUserName(userName);
            return response;
        }
    }
}
