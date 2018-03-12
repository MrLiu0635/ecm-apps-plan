using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    #region 数据访问方法执行上下文类。
    /// <summary>
    /// 数据访问方法执行上下文。
    /// </summary>
    internal class DbExecuteContext : IDbExecuteContext
    {
        private const int RESULTCOUNT = -1;

        private int resultCount;

        private IDbTransaction internalTrans;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public DbExecuteContext()
        {
            this.resultCount = RESULTCOUNT;
            this.internalTrans = null;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public virtual void Release()
        {
            if (this.internalTrans != null)
                this.internalTrans = null;

            this.resultCount = RESULTCOUNT; //释放后，要把_resultCount 重新设置为-1. lizhonghua 20080121
        }

        /// <summary>
        /// 完成。
        /// </summary>
        public virtual void Complete()
        {
            if (this.internalTrans != null)
                this.internalTrans.Commit();
        }

        /// <summary>
        /// 异常退出。
        /// </summary>
        public virtual void Abort()
        {
            if (this.internalTrans != null)
                this.internalTrans.Rollback();
        }

        /// <summary>
        /// 开始内部事务。
        /// </summary>
        /// <param name="conn">数据库连接对象。</param>
        public void BeginInternalTransaction(IDbConnection conn)
        {
            internalTrans = conn.BeginTransaction();
        }

        /// <summary>
        /// 内部事务对象。
        /// </summary>
        public IDbTransaction InternalTransaction
        {
            get { return this.internalTrans; }
        }

        /// <summary>
        /// 返回的数据集个数。
        /// </summary>
        public int ResultCount
        {
            get { return this.resultCount; }
            set { this.resultCount = value; }
        }

    }

    #endregion
}
