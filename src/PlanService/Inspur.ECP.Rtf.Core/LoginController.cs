using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;
using Inspur.ECP.Rtf.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Core
{
    public class LoginController : Controller
    {
        private IIdentityServer _identityServer;
        public LoginController(IIdentityServer svr)
        {
            _identityServer = svr;
        }
        // GET: /<controller>/
        [HttpPost]
        public async Task<IActionResult> Index([FromForm]string uname, [FromForm]string psd)
        {
            EcpState state = await _identityServer.SimpleAuthen(uname, psd);
            return new JsonResult(state);
        }

        [HttpGet]
        public async Task<IActionResult> SSO(string returnUrl)
        {
            if (EcpState.IsLogin() == false)
            {
                string auth = HttpContext.Request.Headers["Authorization"];
                EcpState state = await _identityServer.InspurIdSSO(auth);
                EcpState.SetCurrent(state);
            }
            return new RedirectResult(returnUrl);
        }
    }
}
