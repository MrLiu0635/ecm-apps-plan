using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Api
{
    public interface IIdentityServer
    {
        Task<EcpState> SimpleAuthen(string account, string password);

        Task<EcpState> InspurIdSSO(string auth);

        Task<EcpState> Auth4InspurID(string client_id, string client_secret, string grant_type, string authToken);
    }
}
