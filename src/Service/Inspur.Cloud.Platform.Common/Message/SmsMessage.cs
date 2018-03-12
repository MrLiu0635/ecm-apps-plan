using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common.Message
{
    public class SmsMessage : IMessage
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
        /// 是否html
        /// </summary>
        public bool IsHtml { get; set; }

    }
}
