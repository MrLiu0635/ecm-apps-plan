using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public static class Utility
    {
        public static IConfigurationRoot GetJsonConfig(string path)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(path);
            return builder.Build();
        }
    }
}
