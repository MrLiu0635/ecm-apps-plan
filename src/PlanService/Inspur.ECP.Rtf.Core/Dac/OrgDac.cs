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

        internal static List<Organization> GetOrgsByParentOrgID(string parentOrgId, string tenant_id)
        {
            string sql = @"select a.id ,a.name , a.full_name , a.full_path,a.parent_id from organizations a where a.parent_id={0} and a.tenant_id={1}";
            List<Organization> orgs = GetOrgs(sql, parentOrgId, tenant_id);
            return orgs;
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
                        ID = Convert.ToString(reader["id"]),
                        Name = Convert.ToString(reader["name"]),
                        FullName = Convert.ToString(reader["full_name"]),
                        FullPath = Convert.ToString(reader["full_path"]),
                        ParentID = Convert.ToString(reader["parent_id"])
                    };
                }
            }
            return org;
        }

        private static List<Organization> GetOrgs(string sql, params object[] objParams)
        {
            List<Organization> orgs = new List<Organization>();
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (var reader = db.ExcuteDataReader(sql, objParams))
            {
                while (reader.Read())
                {
                    orgs.Add(new Organization()
                    {
                        ID = Convert.ToString(reader["id"]),
                        Name = Convert.ToString(reader["name"]),
                        FullName = Convert.ToString(reader["full_name"]),
                        FullPath = Convert.ToString(reader["full_path"]),
                        ParentID = Convert.ToString(reader["parent_id"])
                    });
                }
            }
            return orgs;
        }

    }
}
