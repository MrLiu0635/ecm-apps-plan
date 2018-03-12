using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 数据访问封装的抽象类。
    /// </summary>
    public abstract class Database : IGSPDatabase
    {
        /// <summary> 日期格式。 </summary>
        protected const string fullDateFormat = "yyyy-MM-dd HH:mm:ss";

        #region 字段和属性。

        /// <summary>
        /// 数据库连接。
        /// </summary>
        protected IDbConnection dbConn;
        /// <summary>
        /// 数据库事务。
        /// </summary>
        protected IDbTransaction trans;

        /// <summary>
        /// 数据库连接信息。
        /// </summary>
        protected GSPDbConfigData dbConfiguration;

        /// <summary>
        /// 数据访问方法执行上下文对象。
        /// </summary>
        protected IDbExecuteContext context;

        /// <summary>
        /// 是否在每次执行完SQL后自动关闭连接。
        /// </summary>
        private bool autoClose;

        #endregion 字段和属性。

        #region 构造函数。

        /// <summary>
        /// 构造一个database实例对象
        /// </summary>
        /// <param name="dbConfiguration">连接信息</param>
        protected Database(GSPDbConfigData dbConfiguration)
        {
            Initialize();
            this.dbConfiguration = dbConfiguration;
        }

        /// <summary>
        /// 初始。
        /// </summary>
        private void Initialize()
        {
            context = new DbExecuteContext();
            this.autoClose = true;
            this.OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        #endregion

        #region IGSPDatabase 成员。

        #region 属性。
        /// <summary>
        /// 数据库类型。只读。
        /// </summary>
        public abstract GSPDbType DbType
        {
            get;
        }

        /// <summary>
        /// 数据库版本。
        /// </summary>
        public string DatabaseVersion
        {
            get
            {
                string version = string.Empty;
                try
                {
                    this.Open();
                    version = this.OnGetDatabaseVersion();
                }
                catch
                {
                    this.Abort();
                    throw;
                }
                finally
                {
                    this.Release();
                }
                return version;
            }
        }

        protected virtual string OnGetDatabaseVersion()
        {
            return this.DbType.ToString();
        }

        /// <summary>
        /// 连接操作符。用于连接字符串。
        /// </summary>
        /// <remarks>不同数据在连接字符串时所用的具体操作符不同，在此引入，以向上屏蔽细节。</remarks>
        public abstract string ConcatenationOperator
        {
            get;
        }

        /// <summary>
        /// 当前登录数据库的用户。
        /// </summary>
        public string User
        {
            get
            {
                if (this.dbConfiguration != null)
                    return this.dbConfiguration.UserId;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 数据库连接信息
        /// </summary>
        public GSPDbConfigData Configuration
        {
            get
            {
                return this.dbConfiguration;
            }
        }

        public IsolationLevel IsolationLevel
        {
            get
            {
                if (this.trans != null)
                    return trans.IsolationLevel;
                return System.Data.IsolationLevel.Unspecified;
            }
        }

        #endregion 属性。

        #region 关闭/打开连接。

        /// <summary>
        /// 打开数据访问连接。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Open()
        {
            ////休眠一段时间
            //Random rd = new Random();
            //int i = rd.Next(1, 50); //1到50之间的数，可任意更改
            //System.Threading.Thread.Sleep(i);

            if (dbConn == null)
            {
                dbConn = this.CreateDBConnection();
            }
            if (dbConn.State != ConnectionState.Open)
            {
                dbConn.ConnectionString = this.dbConfiguration.ConnectionString;
                dbConn.Open();
            }
        }

        /// <summary>
        /// 关闭数据访问连接。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Close()
        {
            if (IsInTransaction == false)
                this.Dispose();
        }

        #endregion

        #region 生成参数。

        /// <summary>
        /// 生成输入参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeInParam(string paramName, object paramValue)
        {
            IDbDataParameter parameter;
            if (paramValue is DateTime)
            {
                parameter = this.MakeInParam(paramName, GSPDbDataType.DateTime, -1, paramValue);
            }
            else if (paramValue is byte[])
            {
                parameter = this.MakeInParam(paramName, GSPDbDataType.Blob, (paramValue as byte[]).Length, paramValue);
            }
            else if (paramValue is string)
            {
                int length = 300;
                //不同数据库varchar支持的最大长度不一致，默认4000以内的指定长度，
                //超过4000的不指定，按原有逻辑处理（sql server max处理）
                if (length > 4000)
                    length = -1;
                parameter = this.MakeInParam(paramName, GSPDbDataType.Default, length, paramValue);
            }
            else
            {
                parameter = this.MakeParam(paramName, ParameterDirection.Input, paramValue);
            }
            return parameter;
        }

        /// <summary>
        /// 生成输入参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeInParam(string paramName, GSPDbDataType dataType, int size, object paramValue)
        {
            return MakeParam(paramName, ParameterDirection.Input, dataType, size, paramValue);
        }

        /// <summary>
        /// 生成输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeOutParam(string paramName, GSPDbDataType dataType, int size)
        {
            return MakeParam(paramName, ParameterDirection.Output, dataType, size, null);
        }

        /// <summary>
        /// 生成输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeOutParam(string paramName, GSPDbDataType dataType)
        {
            return MakeParam(paramName, ParameterDirection.Output, dataType, -1, null);
        }

        /// <summary>
        /// 生成输入、输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeInOutParam(string paramName, GSPDbDataType dataType, int size, object paramValue)
        {
            return MakeParam(paramName, ParameterDirection.InputOutput, dataType, size, paramValue);
        }

        /// <summary>
        /// 生成输入、输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeInOutParam(string paramName, object paramValue)
        {
            return MakeParam(paramName, ParameterDirection.InputOutput, paramValue);
        }

        /// <summary>
        /// 生成参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="direction">参数方向。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        /// <remarks>数据类型不指定，则设置为Default，相当于没有指定数据类型。</remarks>
        public virtual IDbDataParameter MakeParam(string paramName, ParameterDirection direction, object paramValue)
        {
            return this.MakeParam(paramName, direction, GSPDbDataType.Default, -1, paramValue);
        }

        /// <summary>
        /// 生成参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="direction">参数方向。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <remarks>注意：该方法中的数据类型没有在此处理</remarks>
        /// <returns>构造好的参数对象。</returns>
        public virtual IDbDataParameter MakeParam(string paramName, ParameterDirection direction, GSPDbDataType dataType, int size, object paramValue)
        {
            IDbDataParameter parameter = this.CreateDBDataParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;

            if (size > 0)
                parameter.Size = size;

            parameter.Direction = direction;
            if (!(direction == ParameterDirection.Output && paramValue == null))
                parameter.Value = paramValue;

            return parameter;
        }


        #endregion

        #region 执行存储过程和SQL。

        #region 执行存储过程。
        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <remarks>无参数。无返回值。</remarks>
        /// <param name="procName">存储过程名称。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunProcWithNoQuery(string procName)
        {
            RunProcWithNoQuery(procName, null);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <remarks>无返回值。</remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunProcWithNoQuery(string procName, IDbDataParameter[] dataParams)
        {
            try
            {
                ExecuteWithNoQuery(procName, dataParams, false);
                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDataReader RunProcGetDataReader(string procName, params int[] resultNum)
        {
            return RunProcGetDataReader(procName, null, resultNum);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual IDataReader RunProcGetDataReader(string procName, IDbDataParameter[] dataParams, params int[] resultNum)
        {
            IDataReader dataReader = null;
            try
            {
                if (resultNum.Length > 0)
                {
                    this.context.ResultCount = resultNum[0];
                }

                dataReader = GetDataReader(procName, dataParams, false);

                this.Complete();

            }
            catch
            {
                this.Abort();
                this.Release();
                throw;
            }
            finally
            {
                this.Release(false);
            }
            return dataReader;
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet RunProcGetDataSet(string procName, params int[] resultNum)
        {
            return RunProcGetDataSet(procName, null, resultNum);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet RunProcGetDataSet(string procName, IDbDataParameter[] dataParams, params int[] resultNum)
        {
            return RunProcGetDataSet(null, procName, dataParams, resultNum);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="dataSetTableName">返回的<see cref="DataSet"/>中<see cref="DataTable"/>的<code>TableName</code>。</param>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DataSet RunProcGetDataSet(string dataSetTableName, string procName, IDbDataParameter[] dataParams, params int[] resultNum)
        {
            DataSet ds = null;
            try
            {
                if (resultNum.Length > 0)
                {
                    this.context.ResultCount = resultNum[0];
                }
                ds = GetDataSet(dataSetTableName, procName, dataParams, false);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }

            return ds;
        }

        /// <summary>
        /// 执行数据的批量导入功能
        /// </summary>
        /// <param name="srcdt">数据源DataTable</param>
        /// <param name="destTableName">要导入的目标表名</param>
        /// <returns>批量导入的数据行数</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int DataBatchImport(DataTable srcdt, string destTableName)
        {
            int rowCount = 0;
            if (srcdt.Rows.Count == 0)
            {
                return 0;
            }
            try
            {
                this.Open();
                rowCount = DataBatchImportPart(srcdt, destTableName);
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return rowCount;
        }
        /// <summary>
        /// 执行存储过程,返回DataSet. lizhonghua add 20070914。
        /// 直接可以传入参数名称和值，在内部构造IDbDataParameter
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="paramNames">参数名字</param>
        /// <param name="paramValues">参数值</param>
        /// <param name="resultNum">返回的数据集的个数</param>
        /// <returns>指定的结果集，用DataSet</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet RunProcGetDataSet(string procName, string[] paramNames, object[] paramValues, params int[] resultNum)
        {
            IDbDataParameter[] paramArray = this.GetDbDataParameterArray(paramNames, paramValues);
            return RunProcGetDataSet(procName, paramArray, resultNum);
        }

        #endregion 执行存储过程。

        #region 执行SQL。

        /// <summary>
        /// 执行无返回数据集的SQL。
        /// </summary>
        /// <remarks>无参数。无返回值。</remarks>
        /// <param name="sqlStatement">SQL语句。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ExecSqlStatement(string sqlStatement)
        {
            return ExecSqlStatement(sqlStatement, null);
        }

        /// <summary>
        /// 执行无返回数据集的SQL集合。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ExecSqlStatement(string[] sqlStatements)
        {
            try
            {
                this.Open();

                //if (this.IsInTransaction == false)
                //    this.context.BeginInternalTransaction(this._dbConn);

                ExecuteWithNoQuery(sqlStatements, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
        }

        /// <summary>
        /// 执行无返回数据集的SQL集合。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ExecSqlStatement(string[] sqlStatements, bool isRunTran)
        {
            try
            {
                this.Open();

                if (this.IsInTransaction == false && isRunTran)
                    this.context.BeginInternalTransaction(this.dbConn);

                ExecuteWithNoQuery(sqlStatements, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
        }

        /// <summary>
        /// 根据输入的SQL集合以及SQL集合相应的参数集合，执行多个SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="sqlsDataParams">多个sql的参数数组。</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// 参数数组为二维数组，第一维的下标和SQL集合下标对应。
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ExecSqlStatement(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams)
        {
            try
            {
                this.Open();

                //if (this.IsInTransaction == false)
                //    this.context.BeginInternalTransaction(this._dbConn);

                ExecuteWithNoQuery(sqlStatements, sqlsDataParams, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
        }

        /// <summary>
        /// 根据输入的SQL集合以及SQL集合相应的参数集合，执行多个SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="sqlsDataParams">多个sql的参数数组。</param>
        /// <param name="isRunTran">是否启动内部事务</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// 参数数组为二维数组，第一维的下标和SQL集合下标对应。
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ExecSqlStatement(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams, bool isRunTran)
        {
            try
            {
                this.Open();

                if (this.IsInTransaction == false && isRunTran)
                    this.context.BeginInternalTransaction(this.dbConn);

                ExecuteWithNoQuery(sqlStatements, sqlsDataParams, true);


                this.Complete();

            }
            catch
            {
                if (isRunTran)
                {
                    this.Abort();
                }
                throw;
            }
            finally
            {
                this.Release();
            }
        }

        /// <summary>
        /// 根据输入的参数集合，执行无返回数据集的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <remarks>
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ExecSqlStatement(string sqlStatement, IDbDataParameter[] dataParams)
        {
            int resultNum = 0;

            try
            {
                resultNum = ExecuteWithNoQuery(sqlStatement, dataParams, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return resultNum;
        }

        /// <summary>
        /// 根据输入的参数值集合，执行无返回数据集的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ExecSqlStatement(string sqlStatement, params object[] paramDataList)
        {
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            this.ExecSqlStatement(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行返回单个结果的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <returns>结果值</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object ExecuteScalar(string sqlStatement)
        {
            return this.ExecuteScalar(sqlStatement, null);
        }

        /// <summary>
        /// 根据输入然的参数集合，执行返回单个结果的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <param name="dataParams">参数列表</param>
        /// <returns>结果值</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual object ExecuteScalar(string sqlStatement, IDbDataParameter[] dataParams)
        {
            object obj = null;
            try
            {
                this.Open();
                this.HandlingParameters(dataParams, true);
                sqlStatement = this.HandleSqlStatement(sqlStatement, dataParams);
                IDbCommand command = SetCommand(sqlStatement, dataParams, true);
                try
                {
                    obj = command.ExecuteScalar();
                    command.Parameters.Clear();
                    this.Complete();
                }
                finally
                {

                }
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return obj;
        }


        /// <summary>
        /// 根据输入的sql语句以及参数值列表，执行SQL，返回单个结果。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <returns>结果值。</returns>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object ExecuteScalar(string sqlStatement, params object[] paramDataList)
        {
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteScalar(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDataReader ExecuteReader(string sqlStatement)
        {
            return ExecuteReader(sqlStatement, null);
        }

        /// <summary>
        /// 执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        /// <remarks>
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDataReader ExecuteReader(string sqlStatement, IDbDataParameter[] dataParams)
        {
            IDataReader dataReader = null;
            try
            {
                dataReader = GetDataReader(sqlStatement, dataParams, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                this.Release();
                throw;
            }
            finally
            {
                this.Release(false);
            }
            return dataReader;
        }

        /// <summary>
        /// 执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDataReader ExecuteReader(string sqlStatement, params object[] paramDataList)
        {
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteReader(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string sqlStatement)
        {
            return this.ExecuteDataSet(sqlStatement, null);
        }

        /// <summary>
        /// 执行一组获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>多条查询填充到同一个结果集中。</remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string[] sqlStatements)
        {
            DataSet ds = null;
            try
            {
                ds = GetDataSet(sqlStatements, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return ds;
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL集合。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="sqlsDataParams">多个sql的相应参数数组。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>
        /// 多条SQL查询结果填充到同一个结果集中。
        /// 参数数组为二维数组，第一维的下标和SQL集合下标对应。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams)
        {
            DataSet ds = null;
            try
            {
                ds = GetDataSet(sqlStatements, sqlsDataParams, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return ds;
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL集合，指定数据集中每个Table的名称。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="tableNames">返回数据集的表名称集合。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>多条查询填充到同一个结果集中。</remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string[] sqlStatements, string[] tableNames)
        {
            DataSet ds = null;
            try
            {
                ds = GetDataSet(sqlStatements, tableNames, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return ds;
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string sqlStatement, IDbDataParameter[] dataParams)
        {
            DataSet ds = null;
            try
            {
                ds = GetDataSet(sqlStatement, dataParams, true);

                this.Complete();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return ds;
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数值列表。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string sqlStatement, params object[] paramDataList)
        {
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteDataSet(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <returns>返回结果DataSet</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataSet ExecuteDataSet(string sqlStatement, object param1, object param2, object param3)
        {
            object[] paramDataList = { param1, param2, param3 };
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteDataSet(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL</param>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sqlStatement, object param1, object param2)
        {
            object[] paramDataList = { param1, param2 };
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteDataSet(sqlStatement, dataParams);
        }

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">的SQL</param>
        /// <param name="param1">第一个参数</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sqlStatement, object param1)
        {
            object[] paramDataList = { param1 };
            IDbDataParameter[] dataParams = this.GetDbDataParameterArray(paramDataList);
            return this.ExecuteDataSet(sqlStatement, dataParams);
        }

        /// <summary>
        /// 根据参数列表自动构造绑定参数
        /// </summary>
        private IDbDataParameter[] GetDbDataParameterArray(string[] paramNames, object[] paramValues)
        {
            IDbDataParameter[] paramsList = new IDbDataParameter[paramValues.Length];

            string paramName = string.Empty;
            object paramValue = null;
            for (int index = 0; index < paramValues.Length; index++)
            {
                paramName = paramNames[index];
                paramValue = paramValues[index];

                if (paramValue == null)
                    paramValue = DBNull.Value;

                if (paramValue is IDbDataParameter)
                {
                    IDbDataParameter dataParameter = (IDbDataParameter)paramValue;
                    dataParameter.ParameterName = paramName;
                    paramsList[index] = dataParameter;
                }
                else
                {
                    paramsList[index] = this.MakeInParam(paramName, paramValue);
                }
            }

            return paramsList;
        }
        private IDbDataParameter[] GetDbDataParameterArray(object[] dataParams)
        {
            if (dataParams == null)
                return null;

            string[] paramNames = new string[dataParams.Length];
            for (int i = 0; i < paramNames.Length; i++)
            {
                paramNames[i] = string.Format("param{0}", i);
            }
            return GetDbDataParameterArray(paramNames, dataParams);
        }

        /// <summary>
        /// 返回当前连接的数据源的架构信息。
        /// </summary>
        /// <returns>包含架构信息的<see cref="System.Data.DataTable"/></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DataTable GetSchema()
        {
            return this.GetSchema(null);
        }

        /// <summary>
        /// 使用表示架构名称的指定字符串返回当前连接的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称</param>
        /// <returns>包含架构信息的 DataTable</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DataTable GetSchema(string collectionName)
        {
            DataTable table = null;
            try
            {
                this.Open();
                if (dbConn is DbConnection)
                {
                    if (string.IsNullOrWhiteSpace(collectionName) == false)
                        table = (this.dbConn as DbConnection).GetSchema(collectionName);
                    else
                        table = (this.dbConn as DbConnection).GetSchema();
                }
                this.Commit();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }

            return table;
        }

        /// <summary>
        /// 使用表示架构名称的指定字符串以及表示限制值的指定字符串数组返回当前连接的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称</param>
        /// <param name="restrictionValues">为请求的架构指定一组限制值</param>
        /// <returns>包含架构信息的 DataTable</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            DataTable table = null;
            try
            {
                this.Open();
                if (dbConn is DbConnection)
                {
                    table = (dbConn as DbConnection).GetSchema(collectionName, restrictionValues);
                }
                this.Commit();
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }

            return table;
        }

        #endregion 执行SQL。

        #region 具体数据库访问类可覆写的用于执行存储过程、SQL的方法。

        /// <summary>
        /// 执行完毕。
        /// </summary>
        protected virtual void Complete()
        {
            if (IsInTransaction == false)
                this.context.Complete();
        }

        /// <summary>
        /// 执行发生错误中断。
        /// </summary>
        protected virtual void Abort()
        {
            this.context.Abort();
        }

        /// <summary>
        /// 执行完毕释放资源。
        /// </summary>
        protected virtual void Release()
        {
            this.Release(true);
        }

        /// <summary>
        /// 执行完毕释放资源。
        /// </summary>
        protected virtual void Release(bool closeConn)
        {
            this.context.Release();

            if (this.IsInTransaction == false && closeConn == true && this.autoClose == true)
                this.Close();
        }

        /// <summary>
        /// 执行没有返回结果集的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdText">执行的存储过程或者Sql语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        protected virtual int ExecuteWithNoQuery(string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            this.Open();

            this.HandlingParameters(dataParams, isSqlStatement);

            if (isSqlStatement == true)
                cmdText = this.HandleSqlStatement(cmdText, dataParams);

            IDbCommand command = SetCommand(cmdText, dataParams, isSqlStatement);
            int result;
            try
            {
                result = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// 执行没有返回结果集的存储过程或者Sql语句集合。
        /// </summary>
        /// <param name="cmdTextArray">执行的存储过程或者Sql语句集合。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        protected virtual void ExecuteWithNoQuery(string[] cmdTextArray, bool isSqlStatement)
        {
            this.Open();
            for (int index = 0; index < cmdTextArray.Length; index++)
            {
                string cmdText = cmdTextArray[index];
                IDbCommand command = SetCommand(cmdText, null, isSqlStatement);
                try
                {
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                finally
                {

                }
            }
        }

        /// <summary>
        /// 执行没有返回结果集的存储过程或者Sql语句集合。
        /// </summary>
        /// <param name="cmdTextArray">执行的存储过程或者Sql语句集合。</param>
        /// <param name="sqlsDataParams">多个sql的参数数组。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        protected virtual void ExecuteWithNoQuery(string[] cmdTextArray, IDbDataParameter[][] sqlsDataParams, bool isSqlStatement)
        {
            this.Open();
            for (int index = 0; index < cmdTextArray.Length; index++)
            {
                if (sqlsDataParams != null)
                {
                    ExecuteWithNoQuery(cmdTextArray[index], sqlsDataParams[index], isSqlStatement);
                }
                else
                {
                    ExecuteWithNoQuery(cmdTextArray[index], null, isSqlStatement);
                }
            }
        }

        /// <summary>
        /// 执行返回IDataReader的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdText">执行的存储过程或者Sql语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>IDataReader。</returns>
        protected virtual IDataReader GetDataReader(string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            IDbCommand command = null;
            IDataReader dataReader = null;
            this.Open();

            this.HandlingParameters(dataParams, isSqlStatement);

            if (isSqlStatement == true)
                cmdText = this.HandleSqlStatement(cmdText, dataParams);

            command = SetCommand(cmdText, dataParams, isSqlStatement);

            CommandBehavior cmdBehavior;

            if (!this.IsInTransaction)
            {
                cmdBehavior = CommandBehavior.CloseConnection;
            }
            else
            {
                cmdBehavior = CommandBehavior.Default;
            }

            try
            {
                dataReader = command.ExecuteReader(cmdBehavior);
                //command.Parameters.Clear();
            }
            finally
            {

            }
            return dataReader;
        }

        /// <summary>
        /// 执行返回DataSet的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdText">执行的存储过程或者Sql语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>DataSet。</returns>
        protected virtual DataSet GetDataSet(string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            return GetDataSet("", cmdText, dataParams, isSqlStatement);
        }

        /// <summary>
        /// 执行返回DataSet的存储过程或者Sql。
        /// <param name="dataSetTableName">返回的<see cref="DataSet"/>中<see cref="DataTable"/>的<code>TableName</code>。</param>
        /// <param name="cmdText">执行的存储过程或者Sql语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>DataSet。</returns>
        /// </summary>
        protected virtual DataSet GetDataSet(string dataSetTableName, string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            if (dataSetTableName == null || dataSetTableName == "")
                dataSetTableName = "Table";
            this.Open();
            DataSet ds = new DataSet();

            this.HandlingParameters(dataParams, isSqlStatement);

            if (isSqlStatement == true)
                cmdText = this.HandleSqlStatement(cmdText, dataParams);

            IDbDataAdapter dataAdapter = this.CreateDbDataAdapter();
            IDbCommand command = SetCommand(cmdText, dataParams, isSqlStatement);

            try
            {
                dataAdapter.SelectCommand = command;
                dataAdapter.TableMappings.Add("Table", dataSetTableName);

                dataAdapter.Fill(ds);
            }
            finally
            {

            }
            return ds;
        }

        /// <summary>
        /// 执行返回DataSet的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdTextArray">执行的存储过程或者Sql语句数组。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>DataSet。</returns>
        /// <remarks>本方法不支持参数。</remarks>
        protected virtual DataSet GetDataSet(string[] cmdTextArray, bool isSqlStatement)
        {
            string[] tableNames = new string[cmdTextArray.Length];
            for (int index = 0; index < tableNames.Length; index++)
            {
                tableNames[index] = "Table" + index.ToString();
            }

            return GetDataSet(cmdTextArray, tableNames, isSqlStatement);
        }

        /// <summary>
        /// 执行返回DataSet的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdTextArray">执行的存储过程或者Sql语句数组。</param>
        /// <param name="tableNames">返回数据集的<see cref="DataTable"/>的名称集合。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>DataSet。</returns>
        /// <remarks>本方法不支持参数。</remarks>
        protected virtual DataSet GetDataSet(string[] cmdTextArray, string[] tableNames, bool isSqlStatement)
        {
            this.Open();
            DataSet ds = new DataSet();

            for (int index = 0; index < cmdTextArray.Length; index++)
            {
                string cmdText = cmdTextArray[index];
                IDbDataAdapter dataAdapter = this.CreateDbDataAdapter();
                IDbCommand command = SetCommand(cmdText, null, isSqlStatement);

                try
                {
                    dataAdapter.SelectCommand = command;

                    dataAdapter.TableMappings.Add("Table", tableNames[index]);
                    dataAdapter.Fill(ds);
                }
                finally
                {

                }
            }
            return ds;
        }

        /// <summary>
        /// 执行返回DataSet的存储过程或者Sql。
        /// </summary>
        /// <param name="cmdTextArray">执行的存储过程或者Sql语句数组。</param>
        /// <param name="tableNames">返回数据集的<see cref="DataTable"/>的名称集合。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>DataSet。</returns>
        /// <remarks>本方法不支持参数。</remarks>
        protected virtual DataSet GetDataSet(string[] cmdTextArray, IDbDataParameter[][] sqlsDataParams, bool isSqlStatement)
        {

            this.Open();
            DataSet ds = new DataSet();

            for (int index = 0; index < cmdTextArray.Length; index++)
            {
                if (string.IsNullOrEmpty(cmdTextArray[index]))
                    continue;

                string cmdText = cmdTextArray[index];

                IDbDataParameter[] paramsList = null;
                if (sqlsDataParams != null)
                    paramsList = sqlsDataParams[index];

                this.HandlingParameters(paramsList, isSqlStatement);
                if (isSqlStatement == true)
                    cmdText = this.HandleSqlStatement(cmdText, paramsList);
                IDbDataAdapter dataAdapter = this.CreateDbDataAdapter();
                IDbCommand command = SetCommand(cmdText, paramsList, isSqlStatement);
                try
                {
                    dataAdapter.SelectCommand = command;
                    dataAdapter.TableMappings.Add("Table", string.Format("Table{0}", index));
                    dataAdapter.Fill(ds);
                }
                finally
                {

                }
            }
            return ds;
        }

        #endregion 具体数据库访问类可覆写的用于执行SQL、存储过程的方法。

        #endregion 执行存储过程和SQL。

        #region 事务操作。
        /// <summary>
        /// 开始事务。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void BeginTransaction()
        {
            try
            {
                this.Open();
                if (this.trans == null)	//sql不支持并行事务
                    this.trans = this.dbConn.BeginTransaction();

                context.Complete();
            }
            catch
            {
                context.Abort();
                throw;
            }
            finally
            {
                context.Release();
            }
        }

        /// <summary>
        /// 根据指定的隔离级别开始事务。
        /// </summary>
        /// <param name="isolationLevel">隔离级别。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this.BeginTransaction();
        }

        /// <summary>
        /// 提交事务。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Commit()
        {
            try
            {
                if (null != this.trans)
                {
                    this.trans.Commit();
                    this.dbConn.Close();
                    this.trans = null;
                }

                context.Complete();
            }
            catch
            {
                context.Abort();
                throw;
            }
            finally
            {
                context.Release();
            }
        }

        /// <summary>
        /// 回滚事务。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Rollback()
        {
            try
            {
                if (null != this.trans)
                {
                    this.trans.Rollback();
                    this.dbConn.Close();
                    this.trans = null;
                }

                context.Complete();
            }
            catch
            {
                context.Abort();
                throw;
            }
            finally
            {
                context.Release();
            }
        }

        public virtual void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            return;
        }


        /// <summary>
        /// 是否有事务正在进行中。
        /// </summary>
        /// <returns>是返回<code>true</code>，否则返回<code>false</code>。</returns>
        public virtual bool IsInTransaction
        {
            get
            {
                if (this.trans != null)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region 公共操作。

        /// <summary>
        /// 转换为日期比较中使用的sql。
        /// </summary>
        /// <param name="dateString">日期字符串。</param>
        /// <returns>转换后的字符串。</returns>
        /// <remarks>数据库中日期是日期类型的时候使用。</remarks>
        public abstract string ToDate(string dateString);

        /// <summary>
        /// 获取SQL中字符串的连接符。
        /// </summary>
        public abstract string JoinSymbol
        {
            get;
        }

        /// <summary>
        /// 获取SQL中创建GUID的函数。
        /// </summary>
        public abstract string NewIdFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中获取数据库当前时间的函数。
        /// </summary>
        public abstract string DBDataTimeFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中取字符串子串的函数。
        /// </summary>
        public abstract string SubStrFunc
        {
            get;
        }

        /// <summary>
        /// 获取数据库当前时间。
        /// </summary>
        public abstract DateTime CurrentDateTime
        {
            get;
        }

        /// <summary>
        /// 获取SQL中判断Null值的函数。
        /// </summary>
        public abstract string IsNullFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中取字符串长度的函数。
        /// </summary>
        public abstract string StrLenFunc
        {
            get;
        }

        /// <summary>
        /// 是否在每次执行完SQL后自动关闭连接。
        /// </summary>
        public bool AutoClose
        {
            get { return this.autoClose; }
            set { this.autoClose = value; }
        }

        #endregion 公共操作。

        #region 数据库架构。


        #endregion 数据库架构。

        #region IDisposable 成员。

        /// <summary>
        /// 释放资源。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Dispose()
        {
            if (this.dbConn != null && this.dbConn.State != ConnectionState.Closed)
            {
                this.dbConn.Close();
            }
        }

        #endregion IDisposable 成员。

        #endregion IGSPDatabase 成员。

        #region 其他数据库无关接口的创建接口。
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns>数据库连接接口</returns>
        protected abstract IDbConnection CreateDBConnection();

        /// <summary>
        /// 建立数据库命令
        /// </summary>
        /// <returns>数据库命令接口</returns>
        protected abstract IDbCommand CreateDBCommand();

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <returns>数据库适配器接口</returns>
        protected abstract IDbDataAdapter CreateDbDataAdapter();

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <returns>数据库适配器接口</returns>
        protected abstract IDbDataAdapter CreateDbDataAdapter(string selectCommandText);

        /// <summary>
        /// 建立数据库参数
        /// </summary>
        /// <returns>数据库参数接口</returns>
        protected abstract IDbDataParameter CreateDBDataParameter();

        /// <summary>
        /// 处理sql参数。
        /// </summary>
        /// <param name="sqlStatement">Sql语句。</param>
        /// <param name="dataParams">参数数组。</param>
        /// <returns>处理后的sql语句。</returns>
        protected abstract string HandleSqlStatement(string sqlStatement, IDbDataParameter[] dataParams);

        /// <summary>
        /// 在执行前对参数的处理。
        /// </summary>
        /// <param name="dataParams">参数数组。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        protected virtual void HandlingParameters(IDbDataParameter[] dataParams, bool isSqlStatement)
        {
        }

        /// <summary>
        /// 设置IDbCommand对象。
        /// </summary>
        /// <param name="cmdText">Sql语句或者存储过程名。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>IDbCommand对象。</returns>
        protected abstract IDbCommand SetCommand(string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement);

        /// <summary>
        /// 实现数据批量导入
        /// </summary>
        /// <param name="srcdt">数据源</param>
        /// <param name="destTableName">要导入的目的数据表</param>
        /// <returns></returns>
        protected abstract int DataBatchImportPart(DataTable srcdt, string destTableName);

        #endregion 其他数据库无关接口的创建接口。

        #region 简化存储过程调用
        /// <summary>
        /// Does this <see cref='Database'/> object support parameter discovery?
        /// </summary>
        /// <value>Base class always returns false.</value>
        public virtual bool SupportsParemeterDiscovery
        {
            get { return false; }
        }

        /// <summary>
        ///  执行存储过程，返回DataSet,2011-05-16
        ///  存储过程名对应数据库中名称，可以直接传入参数值,自动获取参数名构造IDbParameter
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DataSet RunProcGetDataSet(string storedProcedureName, params object[] parameterValues)
        {
            DataSet ds = null;
            try
            {
                this.Open();
                DbCommand command = GetStoredProcCommand(storedProcedureName, parameterValues);
                command.Connection = this.dbConn as DbConnection;
                command.Transaction = this.trans as DbTransaction;
                ds = new DataSet();
                IDbDataAdapter dataAdapter = this.CreateDbDataAdapter();

                try
                {
                    dataAdapter.SelectCommand = command;

                    dataAdapter.TableMappings.Add("Table", "Table");
                    dataAdapter.Fill(ds);
                    this.Complete();
                }
                finally
                {

                }
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
            return ds;

        }
        /// <summary>
        ///  执行存储过程，返回DataSet,2011-05-17
        ///  存储过程名对应数据库中名称，可以直接传入参数值,自动获取参数名构造IDbParameter
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunProcWithNoQuery(string procName, params object[] parameterValues)
        {
            try
            {
                this.Open();
                DbCommand command = GetStoredProcCommand(procName, parameterValues);
                command.Connection = this.dbConn as DbConnection;
                command.Transaction = this.trans as DbTransaction;

                int result;
                try
                {
                    result = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    this.Complete();
                }
                finally
                {

                }
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                this.Release();
            }
        }
        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="parameterValues">参数列表。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual IDataReader RunProcGetDataReader(string procName, params object[] parameterValues)
        {
            IDataReader dataReader = null;
            try
            {
                this.Open();
                DbCommand command = GetStoredProcCommand(procName, parameterValues);
                command.Connection = this.dbConn as DbConnection;
                command.Transaction = this.trans as DbTransaction;
                CommandBehavior cmdBehavior;

                if (!this.IsInTransaction)
                {
                    cmdBehavior = CommandBehavior.CloseConnection;
                }
                else
                {
                    cmdBehavior = CommandBehavior.Default;
                }

                try
                {
                    dataReader = command.ExecuteReader(cmdBehavior);
                    command.Parameters.Clear();
                    //this.Complete();
                }
                finally
                {

                }
            }
            catch
            {
                this.Abort();
                throw;
            }
            finally
            {
                //this.Release();
            }
            return dataReader;
        }
        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        protected virtual DbCommand GetStoredProcCommand(string storedProcedureName,
                                                      params object[] parameterValues)
        {
            //获取IDbCommand
            DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
            //为IDbCommand设置参数
            AssignParameters(command, parameterValues);

            return command;
        }
        protected DbCommand CreateCommandByCommandType(CommandType commandType,
                                             string commandText)
        {
            DbCommand command = this.CreateDBCommand() as DbCommand;
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }

        /// <summary>
        /// <para>Discovers parameters on the <paramref name="command"/> and assigns the values from <paramref name="parameterValues"/> to the <paramref name="command"/>s Parameters list.</para>
        /// </summary>
        /// <param name="command">The command the parameeter values will be assigned to</param>
        /// <param name="parameterValues">The parameter values that will be assigned to the command.</param>
        public virtual void AssignParameters(DbCommand command, object[] parameterValues)
        {

            if (SameNumberOfParametersAndValues(command, parameterValues) == false)
            {
                throw new InvalidOperationException("ExceptionMessageParameterMatchFailure");
            }

            AssignParameterValues(command, parameterValues);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="values"></param>
        protected void AssignParameterValues(DbCommand command,
                                   object[] values)
        {
            int parameterIndexShift = UserParametersStartIndex(); // DONE magic number, depends on the database
            for (int i = 0; i < values.Length; i++)
            {
                IDataParameter parameter = command.Parameters[i + parameterIndexShift];

                // There used to be code here that checked to see if the parameter was input or input/output
                // before assigning the value to it. We took it out because of an operational bug with
                // deriving parameters for a stored procedure. It turns out that output parameters are set
                // to input/output after discovery, so any direction checking was unneeded. Should it ever
                // be needed, it should go here, and check that a parameter is input or input/output before
                // assigning a value to it.
                SetParameterValue(command, parameter.ParameterName, values[i]);
            }
        }
        /// <summary>
        /// Returns the starting index for parameters in a command.
        /// </summary>
        /// <returns>The starting index for parameters in a command.</returns>
        protected virtual int UserParametersStartIndex()
        {
            return 0;
        }
        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="command">The command with the parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public virtual void SetParameterValue(DbCommand command,
                                              string parameterName,
                                              object value)
        {
            if (command == null) throw new ArgumentNullException("command");

            command.Parameters[BuildParameterName(parameterName)].Value = value ?? DBNull.Value;
        }
        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        protected virtual string BuildParameterName(string name)
        {
            return name;
        }
        /// <summary>
        /// Determines if the number of parameters in the command matches the array of parameter values.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> containing the parameters.</param>
        /// <param name="values">The array of parameter values.</param>
        /// <returns><see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.</returns>
        protected virtual bool SameNumberOfParametersAndValues(DbCommand command,
                                                               object[] values)
        {
            int numberOfParametersToStoredProcedure = command.Parameters.Count;
            int numberOfValuesProvidedForStoredProcedure = values.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object. 
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        protected abstract void DeriveParameters(DbCommand discoveryCommand);
        #endregion


    }
}
