using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.Cloud.Platform.Common
{
    public class PushMessage
    {
        private static object obj = new object();
        private static string token;
        private static string Token
        {
            get
            {
                if (string.IsNullOrEmpty(token) || Time.AddHours(4) < DateTime.Now)
                {
                    lock (obj)
                    {
                        if (string.IsNullOrEmpty(token) || Time.AddHours(4) < DateTime.Now)
                        {
                            GetToken("inspur_etc@app.iec.io", "DgINCgkHAwYNDgoHBAEKAg");
                        }
                    }
                }

                return token;
            }
        }
        private static DateTime Time = DateTime.MinValue;
        public static void Push(string[] users, string message, string url = "")
        {
            Task task = new Task(() =>
            {
                StringBuilder sb = new StringBuilder(360);
                List<string> inspurids = GetInspurID(users);
                PushByInspurIDAsync(inspurids, message, url);

            });

            task.Start();
        }


        public static async Task PushByInspurIDAsync(List<string> inspurids, string message, string url = "")
        {
            StringBuilder sb = new StringBuilder(360);

            foreach (var item in inspurids)
            {
                sb.AppendFormat("contact={0}&", item);
            }

            string contacts = sb.ToString().TrimEnd('&');
            string pushUrl = "https://ecm.inspur.com/bot/ecc_community/api/v0/bot/enterprise/10000/message?" + contacts;

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("type", "txt_rich");
            data.Add("creationDate", Convert.ToString(DateTime.Now.Ticks));
            data.Add("source", message);

            string jsonStr = JsonConvert.SerializeObject(data);

            HttpContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, pushUrl);
            msg.Content = content;

            msg.Headers.Add("Authorization", Token);
            msg.Headers.Add("User-Agent", "Inspur travel cloud 1.0.0");

            var httpClient = new HttpClient();
            var rm = await httpClient.SendAsync(msg);
            string json = await rm.Content.ReadAsStringAsync();
            if (rm.IsSuccessStatusCode == false)
            {
                NLogger.Error(json);
            }
        }


        private const string loginUrl = "https://id.inspur.com/oauth2.0/token";
        private static void GetToken(string ucode, string psd)
        {
            Dictionary<string, string> postDataDic = new Dictionary<string, string>();
            postDataDic.Add("grant_type", "password");
            postDataDic.Add("username", ucode);
            postDataDic.Add("password", psd);
            postDataDic.Add("client_id", "com.inspur.ecm.client.android");
            postDataDic.Add("client_secret", "6b3c48dc-2e56-440c-84fb-f35be37480e8");


            HttpResponseMessage resMsg = HttpHelper.PostFormData(loginUrl, postDataDic).Result;
            string str = resMsg.Content.ReadAsStringAsync().Result;

            if (resMsg.IsSuccessStatusCode)
            {
                Dictionary<string, string> responseDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                token = "Bearer " + responseDic["access_token"];
            }
            else
            {
                NLogger.Warn("消息推送账号登录失败！");
            }

        }

        private static List<string> GetInspurID(string[] users)
        {
            string qu = string.Empty;
            List<string> list = new List<string>();

            for (int i = 0; i < users.Length; i++)
            {
                qu += string.Format("'{0}',", users[i]);
            }

            qu = qu.TrimEnd(',');

            string sql = "select inspur_id from users where inspur_id<>'' and  id in (" + qu + ")";

            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql))
            {
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));

                }
            }

            return list;
        }
    }
}
