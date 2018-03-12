using Inspur.ECP.Rtf.Api;
using System;
using System.Data;
using System.Collections.Generic;

namespace Inspur.ECP.Rtf.Core
{
    public class EcpOrgService
    {
        public static Organization GetOrganization(string orgId)
        {
#warning 兼容处理，强制 浪潮集团的租户ID，应该为传递参数或者从上下文中获取
            string tenantid = "10000";
            Organization org = OrgDac.GetByID(orgId, tenantid);

            return org;
        }

        public Organization GetByID(string orgId, string tenant_id)
        {
            return OrgDac.GetByID(orgId, tenant_id);
        }

        public List<Organization> GetOrgsByParentOrgID(string parentOrgId, string tenant_id)
        {
            return OrgDac.GetOrgsByParentOrgID(parentOrgId, tenant_id);
        }

        public Organization GetUserFirstOrg(string user_id, string tenant_id)
        {
            throw new NotImplementedException();
        }

        public Organization GetUserOrg(string user_id, string tenant_id)
        {
            return OrgDac.GetUserOrg(user_id, tenant_id);
        }
    }
}
