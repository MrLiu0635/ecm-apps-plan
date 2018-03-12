using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Inspur.ECP.Rtf.Common.Message
{
    public class MessageHelper
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="receiverList">如果类型是用户，则传入的是inspurId，如果是邮箱，则是邮箱列表</param>
        /// <param name="method">发送方式，支持邮件、app消息等多种</param>
        public static async void SendMessageAsync(IMessage msg, List<string> receiverList, SendMethods method)
        {
            try
            {
                if ((method & SendMethods.Mail) != 0)
                {
                    await MailHelper.SendBySmtpAsync(receiverList, msg.Subject, msg.Content, msg.IsHtml);
                }

                if ((method & SendMethods.PushMessage) != 0)
                {
                    await PushMessage.PushByInspurIDAsync(receiverList, msg.Content);
                }
            }
            catch(Exception e)
            {
                //todo：后期加日志，先暂时吞掉
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
            
        }
    }
}
