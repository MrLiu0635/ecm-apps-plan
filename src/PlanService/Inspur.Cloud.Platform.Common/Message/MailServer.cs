using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public class MailServer
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public string Host { get; set; }


        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }


        /// <summary>
        /// 是否启用ssl
        /// </summary>
        public bool UserSSL { get; set; }


        /// <summary>
        /// 发件箱账号
        /// </summary>
        public string Account { get; set; }


        /// <summary>
        /// 发件箱账号密码
        /// </summary>
        public string Password { get; set; }
    }
}
