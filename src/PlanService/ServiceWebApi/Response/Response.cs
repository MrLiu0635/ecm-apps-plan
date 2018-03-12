using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    public class Response
    {
        public Response()
        {
            Code = 200;
        }

        public int Code { get; set; }

        public string Msg { get; set; }
    }
}
