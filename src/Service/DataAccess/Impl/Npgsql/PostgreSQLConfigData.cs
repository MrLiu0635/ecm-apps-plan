using System;
using System.Configuration;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// PostgreSQL数据库的配置。。
    /// </summary>
    //[ConfigurationElementType(typeof(PostgreSQLConfigData))]
    public class PostgreSQLConfigData : GSPDbConfigData
    {
        private const string DefaultPort = "5432";
        private const string ConnFormat = "Server={0};Port={1};Database={2};User Id={3};Password={4};Enlist=true";
        private const string ConnFormatWithPoolSize = "Server={0};Port={1};Database={2};User Id={3};Password={4};MaxPoolSize={5};Enlist=true";
        /// <summary>
        /// 构造函数。
        /// </summary>
        public PostgreSQLConfigData()
            : base()
        {
            this.DbType = GSPDbType.PostgreSQL;
        }

        /// <summary>
        /// 数据库连接的配置字符串。
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                string result = base.ConnectionString;
                if (string.IsNullOrEmpty(result)&& !string.IsNullOrEmpty(this.Source))
                {
                    string[] sourceArr = this.Source.Split(',', ':');
                    if (sourceArr.Length != 2)
                        throw new ArgumentException("无效的数据库源:{0}", this.Source);

                    string host = sourceArr[0];
                    string port = sourceArr[1];

                    if (this.MaxPoolSize == 0)
                        result = string.Format(ConnFormat, host, port, this.Catalog, this.UserId, this.Password);
                    else
                        result = string.Format(ConnFormatWithPoolSize, host, port, this.Catalog, this.UserId, this.Password, this.MaxPoolSize);
                }
                return result;
            }
        }

    }
}
