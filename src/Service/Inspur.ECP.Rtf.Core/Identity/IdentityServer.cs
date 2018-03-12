using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;
using Inspur.ECP.Rtf.Core.Identity;
using Inspur.ECP.Rtf.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Core
{
    public class IdentityServer : IIdentityServer
    {
        private const string loginUrl = "https://id.inspur.com/oauth2.0/token";
        private const string getProfileUrl = "https://id.inspur.com/oauth2.0/profile";

        public async Task<EcpState> SimpleAuthen(string account, string password)
        {
            Dictionary<string, string> postDataDic = new Dictionary<string, string>();
            postDataDic.Add("grant_type", "password");
            postDataDic.Add("username", account);
            postDataDic.Add("password", password);
            postDataDic.Add("client_id", "com.inspur.ecm.client.android");
            postDataDic.Add("client_secret", "6b3c48dc-2e56-440c-84fb-f35be37480e8");


            HttpResponseMessage resMsg = await HttpHelper.PostFormData(loginUrl, postDataDic);
            string str = await resMsg.Content.ReadAsStringAsync();

            if (resMsg.IsSuccessStatusCode)
            {
                Dictionary<string, string> responseDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);

                string access_token = responseDic["access_token"];
                string token_type = responseDic["token_type"];
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", string.Format("{0} {1}", token_type, access_token));

                InspurIdAuthResult ar = await HttpHelper.SendJsonData<InspurIdAuthResult>(HttpMethod.Get, getProfileUrl, null, headers);

                EcpUserService userSvr = new EcpUserService();
                SysUser user = userSvr.GetUserByInspurID(ar.ID);
                EcpState state = new EcpState() { User = user, TenantID = ar.Enterprise.ID, TenantName = ar.Enterprise.Name };
                HttpContextProvider.Current.Session.Clear();
                EcpState.SetCurrent(state);
                return state;
            }
            else
            {
                throw new RtfException(str);
            }
        }

        public async Task<EcpState> InspurIdSSO(string auth)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", auth);
            InspurIdAuthResult ar = await HttpHelper.SendJsonData<InspurIdAuthResult>(HttpMethod.Get, getProfileUrl, null, headers);

            EcpUserService userSvr = new EcpUserService();
            SysUser user = userSvr.GetUserByInspurID(ar.ID);
            EcpState state = new EcpState() { User = user, TenantID = ar.Enterprise.ID, TenantName = ar.Enterprise.Name };
            return state;
        }

        public async Task<EcpState> Auth4InspurID(string client_id, string client_secret, string grant_type, string authToken)
        {
            var authorization = "Basic " + Base64Encode(client_id + ":" + client_secret);
            string token = GetToken(authorization, grant_type, authToken, loginUrl);
            return await InspurIdSSO(token);
        }

        private string GetToken(string authorization, string grant_type, string code, string redirect_uri)
        {
            var access_token = string.Empty;
            string tokenStr = string.Empty;
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("grant_type", grant_type));
            nvc.Add(new KeyValuePair<string, string>("code", code));
            nvc.Add(new KeyValuePair<string, string>("redirect_uri", redirect_uri));
            FormUrlEncodedContent content = new FormUrlEncodedContent(nvc);

            using (HttpClient _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("Authorization", authorization);
                using (Task<HttpResponseMessage> resp = _client.PostAsync(loginUrl, content))
                {
                    using (HttpContent responseContent = resp.Result.Content)
                    {
                        tokenStr = responseContent.ReadAsStringAsync().Result;
                        InspurIdToken info = JsonConvert.DeserializeObject<InspurIdToken>(tokenStr);
                        access_token = info.access_token;
                    }
                }
            }

            return access_token;
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
