using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Inspur.GSP.Gsf.DataAccess
{

    [DisplayName("数据库连接配置")]
    public class GSPDbSettings : ICloneable
    {
        /// <summary>数据库连接的配置小节名称。</summary>
        public const string SectionName = "GSPDbConfiguration";

        /// <summary>静态Root变量</summary>
        private static IConfigurationRoot configurationRoot;

        /// <summary>连接字符串列表</summary>
        private List<GSPDbConfigData> connectionConfigurations;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static GSPDbSettings()
        {
            string path = @"Config/database.json";
            configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path)
                .Build();
        }


        /// <summary>
        /// 构造函数。
        /// </summary>
        public GSPDbSettings()
        {

        }

        /// <summary>
        ///  返回系统中数据库连接的配置。
        /// </summary>
        /// <returns>数据库连接的配置，配置文件中不存在配置节时，实例化一个新的<see cref="GSPDbSettings"/>并返回。</returns>
        /// <remarks>适用于读取所有数据库连接配置列表的场景。</remarks>
        public static GSPDbSettings GetGSPDbSettings()
        {
            GSPDbSettings settings = configurationRoot.GetSection(GSPDbSettings.SectionName).Get<GSPDbSettings>();
            if (settings != null)
            {
                settings = settings.Clone() as GSPDbSettings;
            }
            else
            {
                settings = new GSPDbSettings();
            }
            return settings;
        }


        /// <summary>
		/// 连接配置项信息的集合（对应json配置，外部请访问ConnectionConfigurations）
		/// </summary>
        public List<GSPDbConfigData> Connections
        {
            get;
            set;
        }

        /// <summary>
        /// 默认的数据库配置。
        /// </summary>
        public string DefaultCode
        {
            get;
            set;
        }


        /// <summary>
        /// 获取默认的配置连接信息。
        /// </summary>
        /// <returns>指定的数据库配置连接。</returns>
        public GSPDbConfigData GetDefaultConfiguration()
        {
            return this.GetConfigurationByCode(this.DefaultCode);
        }

        /// <summary>
        /// 连接配置项信息的集合
        /// </summary>
        public List<GSPDbConfigData> ConnectionConfigurations
        {
            get
            {
                if (this.Connections != null && this.Connections.Count > 0 && this.connectionConfigurations == null)
                {
                    this.connectionConfigurations = new List<GSPDbConfigData>();
                    for (int i = 0; i < this.Connections.Count; i++)
                    {
                        GSPDbConfigData data = this.Connections[i],
                            item = null;
                        string name = string.Format("GSPDbConfiguration:Connections:{0}", i);
                        switch (data.DbType)
                        {
                            case GSPDbType.PostgreSQL:
                                item = configurationRoot.GetSection(name).Get<PostgreSQLConfigData>();
                                break;
                            case GSPDbType.MySQL:
                                break;
                            case GSPDbType.SQLServer:
                                //item = configurationRoot.GetSection(name).Get<>();
                                break;
                            case GSPDbType.Oracle:
                                break;
                            case GSPDbType.Unknown:
                                break;
                            default:
                                break;
                        }
                        if (item != null)
                            this.connectionConfigurations.Add(item);
                    }
                }

                return connectionConfigurations;
            }
        }

        /// <summary>
        /// 获取默认的配置连接信息。
        /// </summary>
        /// <returns>指定的数据库配置连接。</returns>
        public GSPDbConfigData GetConfigurationByCode(string dbCode)
        {
            if (this.ConnectionConfigurations != null && this.ConnectionConfigurations.Count > 0)
            {
                return this.ConnectionConfigurations.Find(s => s.Code == dbCode);
            }
            return null;
        }

        #region ICloneable Members
        /// <summary>
        /// 复制一个新的数据访问对象设置类。
        /// </summary>
        /// <returns>复制的数据访问对象设置类。</returns>
        public object Clone()
        {
            GSPDbSettings copy = new GSPDbSettings();
            copy.DefaultCode = this.DefaultCode;
            copy.Connections = this.Connections;
            return copy;
        }
        #endregion
    }
}
