using Inspur.ECP.Rtf.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Core
{
    internal class UserDac
    {
       
        /// <summary>
        /// 获取身份证号
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetUserIdCardNmb(string userID)
        {
            string sql = @"select id_nmb from  users where id={0}";
            string nmb = string.Empty;
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, userID))
            {
                if (reader.Read())
                {
                    nmb = reader.GetString(0);
                }
            }
            return nmb;
        }
    }
}
