using Inspur.ECP.Rtf.Common;
using Inspur.ECP.Rtf.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Inspur.ECP.Rtf.Api
{
    public class StateController : Controller
    {
        protected StateController()
            : base()
        {

            try
            {
                if (EcpState.IsLogin() != true)
                {
                    IIdentityServer server = NServiceProvider.GetService<IIdentityServer>();
                    string auth = HttpContextProvider.Current.Request.Headers["Authorization"];
                    EcpState state = server.InspurIdSSO(auth).Result;
                    EcpState.SetCurrent(state);
                }
            }
            catch (Exception ex)
            {
                throw new ForbidException(ex.Message,ex);
            }
        }
    }
}
