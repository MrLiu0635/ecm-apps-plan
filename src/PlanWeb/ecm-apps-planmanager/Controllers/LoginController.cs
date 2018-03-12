using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspur.ECP.Rtf.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspur.EcmCloud.Apps.Plan.Main.Controllers
{
    [Route("api/[controller]")]
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
