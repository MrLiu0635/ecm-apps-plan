using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public interface IMessage
    {
        /// <summary>
        /// 主题
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }


        /// <summary>
        /// 是否是 html
        /// </summary>
        bool IsHtml { get; set; }
    }
}
