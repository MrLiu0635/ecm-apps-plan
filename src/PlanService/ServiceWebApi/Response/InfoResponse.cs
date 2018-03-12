using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    public class InfoResponse<T> : Response
    {
        public InfoResponse()
        {
        }

        public InfoResponse(string msg)
        {
            this.Msg = msg;
        }

        public T Data { get; set; }
    }
}
