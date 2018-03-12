using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common.Message
{
    public class MailMessage : IMessage
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }


        /// <summary>
        /// 发送给
        /// </summary>
        public List<object> To { get; set; }

        /// <summary>
        /// 是否html
        /// </summary>
        public bool IsHtml { get; set; }


        /// <summary>
        /// 收件人邮箱，
        /// 最终 收件人是 To 和  Recipients  的并集。
        /// </summary>
        List<string> Recipients { get; set; }
    }
}
