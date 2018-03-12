using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;
using Inspur.ECP.Rtf.Common.Message;
using Inspur.GSP.Gsf.DataAccess;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class Utils
    {
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public static IGSPDatabase GetDb()
        {
            return GSPDbFactory.GetEnvDatabase();
        }

        /// <summary>
        /// 获取当前登录用户的标识
        /// </summary>
        /// <returns>登录用户标识</returns>
        public static string GetUserId()
        {
            return PlanState.UserId;
            //return "9999";
        }

        /// <summary>
        /// 获取租户ID
        /// </summary>
        /// <returns>返回租户ID</returns>
        public static string GetTenantId()
        {
            return PlanState.TenantId;
            //return "TenantId";
        }

        public static string GetUserName()
        {
            return PlanState.UserName;

        }

        public static string GetTelphone()
        {
            return PlanState.Telphone;
        }

        // 发送消息
        public static void SendMessage(string subject, string content, List<string> userInspurIdList)
        {
            var message = new SmsMessage()
            {
                Subject = subject,
                Content = content
            };
            MessageHelper.SendMessageAsync(message, userInspurIdList, SendMethods.PushMessage);
        }
    }
}
