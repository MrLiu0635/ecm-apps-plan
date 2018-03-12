using Inspur.Cloud.Platform.Api;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.Cloud.Platform.Common
{
    public class EcpState
    {
        private const string getProfileUrl = "https://id.inspur.com/oauth2.0/profile";
        public static readonly string StateKey = "ecp_state";

        public static EcpState Current
        {
            get
            {
                EcpState state = HttpContextProvider.Current.Session.GetObject<EcpState>(StateKey);
                if (state == null)
                {
                    throw new Exception("会话丢失！");
                }
                return state;
            }
        }

        public EcpState()
        { }

        public SysUser User { get; set; }

        public string TenantID { get; set; }
        public string TenantName { get; set; }
    }
}
