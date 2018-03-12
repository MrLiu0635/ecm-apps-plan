using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class Manager
    {
        Dac dac = new Dac();
        public List<Role> GetRoles()
        {
            return dac.GetRoles();
        }

        public Role GetRoleByID(string id)
        {
            return dac.GetRoleByID(id);
        }
        public List<SysUser> GetUsersByUserName(string userName)
        {
            EcpUserService userService = new EcpUserService();
            List<SysUser> sysUsers = userService.QueryByUserName(userName, Utils.GetTenantId());
            return sysUsers;
        }
    }
}
