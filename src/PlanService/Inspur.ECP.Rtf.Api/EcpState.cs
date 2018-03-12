using Inspur.ECP.Rtf.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Api
{
    public class EcpState
    {
        private const string getProfileUrl = "https://id.inspur.com/oauth2.0/profile";
        public static readonly string StateKey = $"ecp_state_private";

        public static EcpState Current
        {
            get
            {
                EcpState state = HttpContextProvider.Current.Session.Get<EcpState>(StateKey);
                if (state == null)
                {
                    throw new RtfException("会话丢失！");
                }
                return state;
            }

        }

        internal EcpState()
        { }

        public static void SetCurrent(EcpState state)
        {
            HttpContextProvider.Current.Session.Set(StateKey, state);
        }

        public static bool IsLogin()
        {
            EcpState state = HttpContextProvider.Current.Session.Get<EcpState>(StateKey);
            return state != null;
        }

        public SysUser User { get; set; }

        public string TenantID { get; set; }
        public string TenantName { get; set; }
    }
}
