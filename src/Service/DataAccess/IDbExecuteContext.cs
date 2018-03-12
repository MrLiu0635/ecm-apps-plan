using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    #region 数据访问方法执行上下文接口。
    /// <summary>
    /// 数据访问方法执行上下文接口。
    /// </summary>
    public interface IDbExecuteContext
    {
        /// <summary>
        /// 释放资源。
        /// </summary>
        void Release();

        /// <summary>
        /// 完成。
        /// </summary>
        void Complete();

        /// <summary>
        /// 异常退出。
        /// </summary>
        void Abort();

        /// <summary>
        /// 开始内部事务。
        /// </summary>
        /// <param name="conn">数据库连接对象。</param>
        void BeginInternalTransaction(IDbConnection conn);

        /// <summary>
        /// 内部事务对象。
        /// </summary>
        IDbTransaction InternalTransaction
        {
            get;
        }

        /// <summary>
        /// 返回的数据集个数。
        /// </summary>
        int ResultCount
        {
            get;
            set;
        }

    }

    #endregion
}
