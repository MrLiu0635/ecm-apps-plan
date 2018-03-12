using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Extensions
{
    //
    // 摘要:
    //     Extension methods for Microsoft.Extensions.Configuration.IConfiguration.
    public static class ConfigurationExtensions
    {
        public static T GetCinfig<T>(this IConfiguration configuration, string section)
        {
            if (configuration == null)
            {
                return default(T);
            }
            IConfigurationSection sectionCfg = configuration.GetSection(section);
            if (sectionCfg == null)
            {
                return default(T);
            }
            return sectionCfg.Get<T>();
        }

        public static T GetCinfig<T>(this IConfiguration configuration, string section, string name)
        {
            if (configuration == null)
            {
                return default(T);
            }
            IConfigurationSection sectionCfg = configuration.GetSection(section);
            if (sectionCfg == null)
            {
                return default(T);
            }
            return sectionCfg.GetSection(name).Get<T>();
        }

        public static string GetStringCinfig(this IConfiguration configuration, string section, string name)
        {
            if (configuration == null)
            {
                return string.Empty;
            }
            IConfigurationSection sectionCfg = configuration.GetSection(section);
            if (sectionCfg == null)
            {
                return string.Empty;
            }
            return sectionCfg.GetSection(name).Get<string>();
        }

    }


}
