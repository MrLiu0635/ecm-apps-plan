using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Common
{
    public class HttpHelper
    {
        public static async Task<HttpResponseMessage> PostJsonData(string url, string data)
        {
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            return await httpClient.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PostFormData(string url, Dictionary<string, string> data)
        {
            StringBuilder str = new StringBuilder();

            foreach (var item in data)
            {
                str.AppendFormat("{0}={1}&", item.Key, UrlEncoder.Default.Encode(item.Value));
            }

            HttpContent content = new StringContent(str.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            var httpClient = new HttpClient();
            return await httpClient.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PostFormData(string url, Dictionary<string, string> data, Dictionary<string, string> headers)
        {
            StringBuilder str = new StringBuilder();

            if (data != null)
            {
                foreach (var item in data)
                {
                    str.AppendFormat("{0}={1}&", item.Key, UrlEncoder.Default.Encode(item.Value));
                }
            }
            HttpContent content = new StringContent(str.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    content.Headers.Add(item.Key, item.Value);
                }
            }
            var httpClient = new HttpClient();
            return await httpClient.PostAsync(url, content);
        }

        public static async Task<T> PostFormData<T>(string url, Dictionary<string, string> data, Dictionary<string, string> headers)
        {
            StringBuilder str = new StringBuilder();

            if (data != null)
            {
                foreach (var item in data)
                {
                    str.AppendFormat("{0}={1}&", item.Key, UrlEncoder.Default.Encode(item.Value));
                }
            }
            HttpContent content = new StringContent(str.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Content = content;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    msg.Headers.Remove(item.Key);
                    msg.Headers.Add(item.Key, item.Value);
                }
            }

            var httpClient = new HttpClient();

            HttpResponseMessage rm = await httpClient.SendAsync(msg); ;
            string json = await rm.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);

        }

        public static async Task<T> SendJsonData<T>(HttpMethod method, string url, object data, Dictionary<string, string> headers)
        {
            string jsonStr = string.Empty;
            if (data != null)
            {
                jsonStr = JsonConvert.SerializeObject(data);
            }
            HttpContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(method, url);
            msg.Content = content;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    msg.Headers.Remove(item.Key);
                    msg.Headers.Add(item.Key, item.Value);
                }
            }

            var httpClient = new HttpClient();

            HttpResponseMessage rm = await httpClient.SendAsync(msg);
            if (rm.IsSuccessStatusCode)
            {
                string json = await rm.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                throw new Exception(await rm.Content.ReadAsStringAsync());
            }


        }


    }
}
