using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Inspur.ECP.Rtf.Extensions
{
    public static class HttpContextProvider
    {
        internal static IHttpContextAccessor HttpContextAccessor { get; set; }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return HttpContextAccessor.HttpContext;
            }
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// 支持在任意代码中访问 HttpContext 对象
        /// </summary>
        /// <param name="app">IApplicationBuilder </param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
        {
            HttpContextProvider.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            return app;
        }

    }
}
