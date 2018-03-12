using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public static class AppEnvSettings
    {
        private static IConfiguration appConfig;
        public static IConfiguration AppConfig
        {
            get
            {
                return appConfig;
            }
            internal set
            {
                appConfig = value;
            }
        }

    }
}
