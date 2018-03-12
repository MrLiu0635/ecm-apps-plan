using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.GSP.Caf.DataAccess;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class Dac
    {
        IGSPDatabase db;
        public Dac()
        {
            db = Utils.GetDb();
        }

        public List<Role> GetRoles()
        {
            List<Role> roleList = new List<Role>();
            var queryStr = $@"select id,name from role where tenantid='0' or tenantid='{Utils.GetTenantId()}'";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    roleList.Add(AssamblyRoleInfo(row));
                }
            }
            return roleList;
        }

        internal Role GetRoleByID(string id)
        {
            Role role = new Role();
            var queryStr = $@"select id, name from role where (tenantid = '0' or tenantid = '{Utils.GetTenantId()}') and id = '{id}'";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                //将数据库信息封装到实体类
                role = AssamblyRoleInfo(ds.Tables[0].Rows[0]);
            }
            return role;
        }

        public Role AssamblyRoleInfo(DataRow row)
        {
            Role role = new Role();
            role.ID = Convert.ToString(row["id"]);
            role.Name = Convert.ToString(row["name"]);
            return role;
        }
    }
}
