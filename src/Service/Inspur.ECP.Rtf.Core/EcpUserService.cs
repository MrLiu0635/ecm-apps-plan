using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;

namespace Inspur.ECP.Rtf.Core
{
    public class EcpUserService
    {
        public SysUser GetUserByInspurID(string inspurId)
        {
            string sql = @"select a.id, a.inspur_id, a.code, a.full_name, a.name_global, a.mobile, a.email,
                        c.id org_id, c.name org_name, c.full_name org_full_name, c.full_path,c.parent_id,a.serial_number 
                            from users a inner join r_org_user b on a.id = b.user_id  inner JOIN  organizations c on b.org_id = c.id  where a.inspur_id ={0}";
            SysUser user = default(SysUser);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, inspurId))
            {
                if (reader.Read())
                {
                    user = new SysUser();
                    user.ID = reader.GetString(0);
                    user.InspurID = reader.GetString(1);
                    user.Code = reader.GetString(2);
                    user.Name = reader.GetString(3);
                    user.GlobalName = reader.GetString(4);
                    user.Phone = reader.GetString(5);
                    user.Mail = reader.GetString(6);
                    user.SerialNumber = reader.GetString(12);

                    Organization org = new Organization()
                    {
                        ID = reader.GetString(7),
                        Name = reader.GetString(8),
                        FullName = reader.GetString(9),
                        FullPath = reader.GetString(10),
                        ParentID = reader.GetString(11)
                    };
                }
            }

            return user;
        }

        public SysUser GetUserByID(string id)
        {
            string sql = @"select a.id, a.inspur_id, a.code, a.full_name, a.name_global, a.mobile, a.email,
                        c.id org_id, c.name org_name, c.full_name org_full_name, c.full_path,c.parent_id,a.serial_number
                            from users a inner join r_org_user b on a.id = b.user_id  inner JOIN  organizations c on b.org_id = c.id  where a.id ={0}";
            SysUser user = default(SysUser);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, id))
            {
                if (reader.Read())
                {
                    user = new SysUser();
                    user.ID = reader.GetString(0);
                    user.InspurID = reader.GetString(1);
                    user.Code = reader.GetString(2);
                    user.Name = reader.GetString(3);
                    user.GlobalName = reader.GetString(4);
                    user.Phone = reader.GetString(5);
                    user.Mail = reader.GetString(6);
                    user.SerialNumber = reader.GetString(12);

                    Organization org = new Organization()
                    {
                        ID = reader.GetString(7),
                        Name = reader.GetString(8),
                        FullName = reader.GetString(9),
                        FullPath = reader.GetString(10),
                        ParentID = reader.GetString(11)
                    };

                }
            }

            return user;
        }


        /// <summary>
        /// 获取用户身份证
        /// </summary>
        ///  <param name="userId">用户ID</param>
        /// <returns></returns>
        //public string GetIdCardNumber(string userId)
        //{
        //    UserExtension ext = UserDac.GetUserExtension(userId, UserExtensionTypes.IDCard);
        //    string nmb = ext == null ? string.Empty : ext.Value;
        //    return nmb;
        //}

        /// <summary>
        /// 获取用户护照信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Passport GetPassport(string userId)
        {
            Passport ppt = PassportDac.GetPassport(userId);
            return ppt;
        }

        /// <summary>
        /// 获取用户护照编号
        /// </summary>
        ///  <param name="userId">用户ID</param>
        /// <returns></returns>
        public string GetPassportNmb(string userId)
        {
            return PassportDac.GetPassportNmb(userId);
        }

        /// <summary>
        /// 获取用户护照照片
        /// </summary>
        ///  <param name="userId">用户ID</param>
        /// <returns></returns>
        public Stream GetPassportPic(string userId)
        {
            return PassportDac.GetPassportPic(userId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<SysUser> QueryByUserName(string userName, string tenant_id)
        {
            List<SysUser> list = new List<SysUser>();
            if (userName.Length < 2) //至少要输入两个字符。
            {
                return list;
            }

            string sql = @"select a.id, a.inspur_id, a.code, a.full_name, a.name_global, a.mobile, a.email,
                        c.id org_id, c.name org_name, c.full_name org_full_name, c.full_path,c.parent_id,a.serial_number
                            from users a inner join r_org_user b on a.id = b.user_id  inner JOIN  organizations c on b.org_id = c.id  where a.full_name like {0}  and  b.tenant_id={1}";
            SysUser user = default(SysUser);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, string.Format("%{0}%", userName), tenant_id))
            {
                while (reader.Read())
                {
                    user = new SysUser();
                    user.ID = reader.GetString(0);
                    user.InspurID = reader.GetString(1);
                    user.Code = reader.GetString(2);
                    user.Name = reader.GetString(3);
                    user.GlobalName = reader.GetString(4);
                    user.Phone = reader.GetString(5);
                    user.Mail = reader.GetString(6);
                    user.SerialNumber = reader.GetString(12);

                    Organization org = new Organization()
                    {
                        ID = reader.GetString(7),
                        Name = reader.GetString(8),
                        FullName = reader.GetString(9),
                        FullPath = reader.GetString(10),
                        ParentID = reader.GetString(11)
                    };

                    user.org = org;
                    list.Add(user);
                }
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<SysUser> QueryByUserName(string userName, string tenant_id, int page, int total)
        {
            List<SysUser> list = new List<SysUser>();
            if (string.IsNullOrEmpty(userName)) //至少要输入两个字符。
            {
                throw new Exception("名称不能为空。");
            }

            string sql = @"select a.id, a.inspur_id, a.code, a.full_name, a.name_global, a.mobile, a.email,
                        c.id org_id, c.name org_name, c.full_name org_full_name, c.full_path,c.parent_id,a.serial_number
                            from users a inner join r_org_user b on a.id = b.user_id  inner JOIN  organizations c on b.org_id = c.id  where a.full_name like {0}  and  b.tenant_id={1}";
            SysUser user = default(SysUser);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, string.Format("%{0}%", userName), tenant_id))
            {
                while (reader.Read())
                {
                    user = new SysUser();
                    user.ID = reader.GetString(0);
                    user.InspurID = reader.GetString(1);
                    user.Code = reader.GetString(2);
                    user.Name = reader.GetString(3);
                    user.GlobalName = reader.GetString(4);
                    user.Phone = reader.GetString(5);
                    user.Mail = reader.GetString(6);
                    user.SerialNumber = reader.GetString(12);

                    Organization org = new Organization()
                    {
                        ID = reader.GetString(7),
                        Name = reader.GetString(8),
                        FullName = reader.GetString(9),
                        FullPath = reader.GetString(10),
                        ParentID = reader.GetString(11)
                    };

                    list.Add(user);
                }
            }

            return list;
        }

        public List<string> GetInspurIdByUserId(List<string> users, string tenant_id)
        {
            if (users == null || users.Count < 1)
            {
                throw new Exception("用户标识列表为空");
            }

            string id = string.Join("','", users.ToArray());
            string sql = $@"select a.inspur_id from users a inner join r_org_user b on a.id = b.user_id where b.tenant_id='{tenant_id}' and a.id in('{id}')";
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            List<string> list = new List<string>();
            using (IDataReader reader = db.ExcuteDataReader(sql))
            {
                while (reader.Read())
                {
                    string inspurID = reader.GetString(0);
                    list.Add(inspurID);
                }
            }

            return list;
        }
    }
}
