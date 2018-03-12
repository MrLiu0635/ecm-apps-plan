using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 数据库连接配置。
    /// </summary>
    public class GSPDbConfigData
    {
        private int connectTimeout = 30;
        private int commandTimeout = 30;
        private int maxPoolSize = 100;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public GSPDbConfigData()
        {

        }
        
        /// <summary>
        /// 数据库配置编码。
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库配置名称。
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库类型描述。
        /// </summary>
        public GSPDbType DbType
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 口令。
        /// </summary>
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 数据源。
        /// </summary>
        public string Source
        {
            get;
            set;
        }

        /// <summary>
        /// 服务提供者。
        /// </summary>
        public string Provider
        {
            get;
            set;
        }

        /// <summary>
        /// 指代数据库名。
        /// </summary>
        public string Catalog
        {
            get;
            set;
        }

        ///<summary>
        ///指定数据库连接TimeOut
        ///</summary>
        public int ConnectTimeout
        {
            get { return this.connectTimeout; }
            set { this.connectTimeout = value; }
        }

        ///<summary>
        ///指定sql执行TimeOut
        ///</summary>
        public int CommandTimeout
        {
            get { return this.commandTimeout; }
            set { this.commandTimeout = value; }
        }

        ///<summary>
        ///指定连接池最大连接数
        ///默认值为0，即外部不指定，此时池大小依赖于ADO.NET的默认值
        ///</summary>
        public int MaxPoolSize
        {
            get { return this.maxPoolSize; }
            set { this.maxPoolSize = value; }
        }

        /// <summary>
        /// 返回描述当前数据库连接配置类的字符串。
        /// </summary>
        /// <returns>一个描述当前数据库连接配置类的字符串。</returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// 数据库连接的配置字符串。
        /// </summary>
        public virtual string ConnectionString
        {
            get;
            set;
        }
    }
}
