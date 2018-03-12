using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.ServiceWebApi
{
    internal class MethodResponse
    {
        private MethodResponse()
        {

        }
        public static MethodResponse Instance
        {
            get
            {
                return new MethodResponse();
            }

        }

        public Response DoWork(Action work)
        {
            try
            {
                work();
                return null;
            }
            catch (Exception e)
            {
                Response result = new Response
                {
                    //wrap exception
                    Msg = e.Message
                };
                return result;
            }
        }
    }
}
