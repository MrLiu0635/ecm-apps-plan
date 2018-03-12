using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    [Flags]
    public enum SendMethods
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        Mail = 0x1,

        /// <summary>
        /// 推送通知
        /// </summary>
        PushMessage = 0x2,

        /// <summary>
        /// 短信
        /// </summary>
        Sms = 0x4
        //QQ = 0x8,
        //Phone = 0x10,
        //Wechat = 0x20,
        //Other = 0x40
    }
}
