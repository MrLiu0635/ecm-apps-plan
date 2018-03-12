using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    internal class DBConnectionStringBuilder
    {
        /// <summary>
        /// 根据数据源信息获取IGSPDatabase接口
        /// </summary>
        /// <param name="dataSource">数据源信息</param>
        /// <returns>IGSPDatabase接口 data/Source为null返回Null</returns>
        public static GSPDbConfigData GetConfigData(GSPDbConfigData dataSource)
        {
            if (dataSource == null)
                return null;
            GSPDbConfigData configdata = null;

            switch (dataSource.DbType)
            {
                case GSPDbType.PostgreSQL:
                    configdata = new PostgreSQLConfigData();
                    NpgsqlConnectionStringBuilder npgBiulder = new NpgsqlConnectionStringBuilder(dataSource.ConnectionString);

                    //npgBiulder.Password = ProtectePassword.DecryptPasswd(npgBiulder.Password);

                    configdata.ConnectionString = npgBiulder.ConnectionString;
                    configdata.UserId = npgBiulder.Username;
                    configdata.Password = npgBiulder.Password;
                    configdata.Source = npgBiulder.Host;
                    configdata.Catalog = npgBiulder.Database;

                    break;
                case GSPDbType.MySQL:
                    break;
                case GSPDbType.SQLServer:
                    //configdata = new Genersoft.Platform.Core.DataAccess.Configuration.SqlDbConfigData();
                    //SqlConnectionStringBuilder sqlbuilder = new SqlConnectionStringBuilder(dataSource.ConnectionString);
                    //sqlbuilder.Password = ProtectePassword.DecryptPasswd(sqlbuilder.Password);
                    //dataSource.ConnectionString = sqlbuilder.ConnectionString;
                    //configdata.UserId = sqlbuilder.UserID;
                    //configdata.Password = sqlbuilder.Password;
                    //configdata.Source = sqlbuilder.DataSource;
                    //configdata.Catalog = sqlbuilder.InitialCatalog;
                    break;
                case GSPDbType.Oracle:
                    //configdata = new Genersoft.Platform.Core.DataAccess.Configuration.OracleDbConfigData();
                    //configdata.DbType = GSPDbType.Oracle;
                    //OracleConnectionStringBuilder orabuilder = new OracleConnectionStringBuilder(dataSource.ConnectionString);
                    //orabuilder.Password = ProtectePassword.DecryptPasswd(orabuilder.Password);
                    //dataSource.ConnectionString = orabuilder.ConnectionString;
                    //configdata.Source = orabuilder.DataSource;
                    //configdata.UserId = orabuilder.UserID;
                    //configdata.Password = orabuilder.Password;
                    break;
                case GSPDbType.Unknown:
                    break;
                default:
                    break;
            }

            if (configdata != null)
            {
                configdata.ConnectionString = dataSource.ConnectionString;
                configdata.CommandTimeout = 600;
            }
            return configdata;
        }
    }
}
