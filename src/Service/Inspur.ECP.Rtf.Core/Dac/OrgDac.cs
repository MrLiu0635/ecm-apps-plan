using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inspur.ECP.Rtf.Core
{
    class OrgDac
    {
        public static Organization GetByID(string id, string tenant_id)
        {
            string sql = @"select id ,name , full_name , full_path,parent_id  from organizations a where id ={0} and tenant_id={1}";
            Organization org = GetOrg(sql, id, tenant_id);
            return org;
        }

        public static Organization GetUserFirstOrg(string user_id, string tenant_id)
        {
            string sql = @"select a.id ,a.name , a.full_name , a.full_path,a.parent_id from r_org_user b  inner JOIN organizations a on  b.first_org = a.id and  b.user_id ={0} and b.tenant_id={1}";
            Organization org = GetOrg(sql, user_id, tenant_id);
            return org;
        }

        public static Organization GetUserOrg(string user_id, string tenant_id)
        {
            string sql = @"select a.id ,a.name , a.full_name , a.full_path,a.parent_id from r_org_user b  inner JOIN organizations a on  b.org_id = a.id and  b.user_id ={0} and b.tenant_id={1}";

            Organization org = GetOrg(sql, user_id, tenant_id);
            return org;
        }

        private static Organization GetOrg(string sql, params object[] objParams)
        {
            Organization org = default(Organization);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, objParams))
            {
                if (reader.Read())
                {
                    org = new Organization()
                    {
                        ID = reader.GetString(0),
                        Name = reader.GetString(1),
                        FullName = reader.GetString(2),
                        FullPath = reader.GetString(3),
                        ParentID = reader.GetString(4)
                    };
                }
            }
            return org;
        }

    }
}
