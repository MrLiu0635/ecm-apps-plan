using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class BasicManager
    {
        EcpUserService userService = new EcpUserService();
        public List<User> GetUsersByUserName(string userName)
        {

            List<SysUser> sysUsers = userService.QueryByUserName(userName, PlanState.TenantId);
            List<User> users = new List<User>();
            if (sysUsers != null && sysUsers.Count > 0)
            {
                sysUsers.ForEach(user =>
                {
                    users.Add(new User() { ID = user.ID, Name = user.Name,OrgName = user.org.Name });
                });
            }
            return users;
        }
    }
}
