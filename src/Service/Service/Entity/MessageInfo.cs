using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class MessageInfo
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public List<string> UserList { get; set; }
    }
}
