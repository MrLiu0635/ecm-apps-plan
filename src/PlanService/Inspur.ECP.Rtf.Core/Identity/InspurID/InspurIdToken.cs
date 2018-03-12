using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Core.Identity
{
    class InspurIdToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int keep_alive { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string openid { get; set; }

    }
}
