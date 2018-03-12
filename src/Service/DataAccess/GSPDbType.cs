using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum GSPDbType : int
    {
        /// <summary>PostgreSQL类型</summary>
        PostgreSQL = 0,

        /// <summary>MySQL类型</summary>
        MySQL =1,

        /// <summary>SqlServer类型</summary>
        SQLServer =2,

        /// <summary>Oracle类型</summary>
        Oracle =3 ,

        /// <summary>未知类型</summary>
        Unknown = 255
    };
}
