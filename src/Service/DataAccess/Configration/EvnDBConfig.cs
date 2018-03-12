using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    internal class EvnDBConfig
    {
        /// <summary>
        /// 数据库类型描述。
        /// </summary>
        public GSPDbType DbType
        {
            get;
            set;
        }


        /// <summary>
        /// 数据库连接的配置字符串。
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }
    }
}
