using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class OtherService
    {
        private static OtherService instance = null;
        private static EcpOrgService orgService = new EcpOrgService();
        Manager manager = new Manager();
        private OtherService() { }

        public static OtherService Current => instance ?? (instance = new OtherService());

        public List<Role> GetRoles()
        {
            return manager.GetRoles();
        }

        public List<Organization> GetOrgs(string parentOrgID)
        {
            return orgService.GetOrgsByParentOrgID(parentOrgID, Utils.GetTenantId());
        }
        public List<SysUser> GetUsersByUserName(string userName)
        {
            return manager.GetUsersByUserName(userName);
        }
    }
}
