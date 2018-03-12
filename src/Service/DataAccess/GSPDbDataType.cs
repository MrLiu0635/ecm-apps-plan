using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 公共的数据类型。标准类型，与数据库无关。
    /// </summary>
    public enum GSPDbDataType : int
    {
        /// <summary> 默认类型 </summary>
        Default = -1,

        /// <summary> 字符类型 </summary>
        Char = 0,

        /// <summary> Unicode编码的字符类型 </summary>
        NChar = 1,

        /// <summary> 可变字符类型 </summary>
        VarChar = 2,
        
        /// <summary> Unicode编码的可变字符类型 </summary>
        NVarChar = 3,

        /// <summary> 整数类型 </summary>
        Int = 4,

        /// <summary> 浮点类型 </summary>
        Decimal = 5,

        /// <summary> 日期类型 </summary>
        DateTime = 6,

        /// <summary> 二进制类型 </summary>
        Blob = 7,

        ///<summary>长文本</summary>
        Clob = 8,

        /// <summary> Unicode编码的长文本</summary>
        NClob = 9,

        /// <summary> 游标类型 </summary>
        Cursor = 10,

        /// <summary> 未识别类型 </summary>
        UnKnown = 255,
    }
}
