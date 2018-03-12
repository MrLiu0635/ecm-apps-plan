using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Inspur.ECP.Rtf.Common
{
    public static class NServiceProvider
    {
        public static IServiceProvider Current { get; set; }

        public static Microsoft.AspNetCore.Http.HttpContext HttpCurrent
        {
            get
            {
                object factory = Current.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                Microsoft.AspNetCore.Http.HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        public static T GetService<T>()
        {
            return (T)Current.GetService(typeof(T));
        }


        public static void AddEcpServices(this IServiceCollection services)
        {
            Type _interface = GetType("Inspur.ECP.Rtf.Api.dll", "Inspur.ECP.Rtf.Api.IIdentityServer");
            Type _server = GetType("Inspur.ECP.Rtf.Core.dll", "Inspur.ECP.Rtf.Core.Identity.InspurIdServer");
            services.AddSingleton(_interface, _server);
        }

        private static Type GetType(string asmName, string type)
        {
            Assembly assembly = Assembly.LoadFrom(asmName);
            Type t = assembly.GetType(type);
            return t;
        }
    }
}
