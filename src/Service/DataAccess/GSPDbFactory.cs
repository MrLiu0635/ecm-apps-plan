using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// Database创建工厂
    /// </summary>
    public class GSPDbFactory
    {
        private static GSPDbConfigData ebvDBConfigData;


        private static GSPDbSettings dbConfigurations;
        private static object lockObject = new object();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static GSPDbFactory()
        {

        }

        #region 静态参数化工厂方法。

        /// <summary>
        /// 获取根据配置默认的数据访问对象。由配置文件中的DefaultCode决定。
        /// </summary>
        /// <returns>数据访问对象接口。</returns>
        public static IGSPDatabase GetEnvDatabase()
        {
            GSPDbConfigData defaultDbConfiguration = GetEnvDBConfig();
            return GetDatabase(defaultDbConfiguration);
        }

        /// <summary>
        /// 获取根据配置默认的数据访问对象。由配置文件中的DefaultCode决定。
        /// </summary>
        /// <returns>数据访问对象接口。</returns>
        public static IGSPDatabase GetDatabase()
        {
            GSPDbConfigData defaultDbConfiguration = GetEnvDBConfig();

            if (defaultDbConfiguration != null)
            {
                return GetDatabase(defaultDbConfiguration);
            }

            defaultDbConfiguration = GetDefaultConfiguration();
            return GetDatabase(defaultDbConfiguration);
        }

        /// <summary>
        /// 获取根据数据源配置对应的数据访问对象。
        /// </summary>
        /// <param name="dataSrcCode"></param>
        /// <returns>数据访问对象接口。</returns>
        public static IGSPDatabase GetDatabase(string dataSrcCode)
        {
            GSPDbConfigData dbConfiguration = GetConfiguration(dataSrcCode);
            if (dbConfiguration == null)
                throw new ArgumentOutOfRangeException("dataSrcCode", dataSrcCode, string.Format("无法找到编号为[{0}]的数据库配置项，请检查配置是否正确", dataSrcCode));
            return GetDatabase(dbConfiguration);
        }

        /// <summary>
        /// 获取根据数据库连接信息对应的数据访问对象。
        /// </summary>
        /// <param name="dbConfiguration">连接信息对象。</param>
        /// <returns>数据访问对象接口。</returns>
        public static IGSPDatabase GetDatabase(GSPDbConfigData dbConfiguration)
        {
            //DataValidator.CheckForNullReference(dbConfiguration, "数据库配置对象(dbConfiguration)");

            if (null == dbConfiguration)
            {
                throw new ArgumentNullException("数据库配置对象(dbConfiguration)");
            }

            switch (dbConfiguration.DbType)
            {
                case GSPDbType.PostgreSQL:
                    return new PostgreSQLDatabase(dbConfiguration);

                //case GSPDbType.MySQL:
                //    return new MySqlDatabase(dbConfiguration);

                case GSPDbType.SQLServer:
                    return new SqlDatabase(dbConfiguration);

                //case GSPDbType.Oracle:
                //    //return new OracleDatabase(dbConfiguration);
                //    return new OracleClientDatabase(dbConfiguration);

                default: return null;
            }
        }
        #endregion

        #region 数据库连接配置维护。

        /// <summary>
        /// 获取环境变量中的数据连接配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="">如果环境变量没有配置数据库连接信息，返回null</exception>
        private static GSPDbConfigData GetEnvDBConfig()
        {
            if (ebvDBConfigData != null)
            {
                return ebvDBConfigData;
            }

            IConfiguration config = new ConfigurationBuilder()
            .AddEnvironmentVariables("DBConn_")
            .Build();

            EvnDBConfig dbConfig = new ServiceCollection()
            .AddOptions()
            .Configure<EvnDBConfig>(config)
            .BuildServiceProvider()
            .GetService<IOptions<EvnDBConfig>>()
            .Value;

            //如果没有配置相关环境变量，dbConfig会有默认值
            if (dbConfig.ConnectionString == null)
            {
                return null;
            }

            switch (dbConfig.DbType)
            {
                case GSPDbType.PostgreSQL:
                    ebvDBConfigData = new PostgreSQLConfigData()
                    {
                        DbType = dbConfig.DbType,
                        ConnectionString = dbConfig.ConnectionString
                    };

                    break;
                case GSPDbType.MySQL:
                    break;
                case GSPDbType.SQLServer:
                    break;
                case GSPDbType.Oracle:
                    break;
                case GSPDbType.Unknown:
                    break;
                default:
                    ebvDBConfigData = new PostgreSQLConfigData()
                    {
                        DbType = dbConfig.DbType,
                        ConnectionString = dbConfig.ConnectionString
                    };
                    break;
            }

            ebvDBConfigData = DBConnectionStringBuilder.GetConfigData(ebvDBConfigData);
            return ebvDBConfigData;
        }

        /// <summary>
        /// 获取所有已注册的数据库连接配置。
        /// </summary>
        /// <returns>数据库连接配置对象数组。</returns>
        public static GSPDbConfigData[] GetAllConfigurations()
        {
            GSPDbConfigData[] result = new GSPDbConfigData[DbConfigurations.Connections.Count];
            DbConfigurations.Connections.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// 获取指定的数据库连接配置。
        /// </summary>
        /// <param name="dataSrcCode">数据库连接配置Code。</param>
        /// <returns>指定的数据库连接配置。</returns>
        public static GSPDbConfigData GetConfiguration(string dataSrcCode)
        {
            //DataValidator.CheckForEmptyString(dataSrcCode, "dataSrcCode");

            //if (DbConfigurations.ConnectionConfigurations.Contains(dataSrcCode))
            //    return DbConfigurations.ConnectionConfigurations.Get(dataSrcCode);
            //else
            //{
            //    //根据dataSrcCode找不到，尝试改变大小写再取
            //    foreach (var item in DbConfigurations.ConnectionConfigurations)
            //    {
            //        if (item.Code.Equals(dataSrcCode, StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            return item;
            //        }
            //    }
            //} 

            if (string.IsNullOrWhiteSpace(dataSrcCode))
            {
                throw new ArgumentException("数据库编号dataSrcCode");
            }

            return DbConfigurations.GetConfigurationByCode(dataSrcCode); ;
        }

        /// <summary>
        /// 获取默认的数据库连接配置。
        /// </summary>
        /// <returns>指定的数据库连接配置。</returns>
        public static GSPDbConfigData GetDefaultConfiguration()
        {
            return DbConfigurations.GetDefaultConfiguration();
        }


        /// <summary>
        /// 数据库连接配置
        /// </summary>
        private static GSPDbSettings DbConfigurations
        {
            get
            {
                if (dbConfigurations == null)
                {
                    lock (lockObject)
                    {
                        if (dbConfigurations == null)
                        {
                            dbConfigurations = GSPDbSettings.GetGSPDbSettings();
                            //GSPConfigurationSource.Current.AddSectionChangeHandler(GSPDbSettings.SectionName, ConfigurationChanged);
                        }
                    }
                }
                return dbConfigurations;
            }
        }

        #endregion 数据库连接配置维护。
    }
}
