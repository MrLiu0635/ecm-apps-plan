using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 数据访问接口
    /// </summary>
    public interface IGSPDatabase : IDisposable
    {
        #region 属性。

        /// <summary>
        /// 数据库类型。只读。
        /// </summary>
        GSPDbType DbType
        {
            get;
        }

        /// <summary>
        /// 数据库版本。
        /// </summary>
        string DatabaseVersion
        {
            get;
        }

        /// <summary>
        /// 连接操作符。用于连接字符串。
        /// </summary>
        /// <remarks>不同数据在连接字符串时所用的具体操作符不同，在此引入，以向上屏蔽细节。</remarks>
        string ConcatenationOperator
        {
            get;
        }

        /// <summary>
        /// 当前登录数据库的用户。
        /// </summary>
        string User
        {
            get;
        }

        #endregion

        #region 关闭/打开连接。

        /// <summary>
        /// 关闭数据访问连接。
        /// </summary>
        void Close();

        /// <summary>
        /// 打开数据访问连接。
        /// </summary>
        void Open();

        #endregion

        #region 生成参数。
        /// <summary>
        /// 生成输入参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeInParam(string paramName, object paramValue);

        /// <summary>
        /// 生成输入参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeInParam(string paramName, GSPDbDataType dataType, int size, object paramValue);

        /// <summary>
        /// 生成输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeOutParam(string paramName, GSPDbDataType dataType, int size);

        /// <summary>
        /// 生成输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeOutParam(string paramName, GSPDbDataType dataType);

        /// <summary>
        /// 生成输入、输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeInOutParam(string paramName, GSPDbDataType dataType, int size, object paramValue);

        /// <summary>
        /// 生成输入、输出参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeInOutParam(string paramName, object paramValue);

        /// <summary>
        /// 生成参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="direction">参数方向。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeParam(string paramName, ParameterDirection direction, object paramValue);

        /// <summary>
        /// 生成参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="direction">参数方向。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        IDbDataParameter MakeParam(string paramName, ParameterDirection direction, GSPDbDataType dataType, int size, object paramValue);

        #endregion

        #region 执行存储过程。
        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <remarks>无参数。无返回值。</remarks>
        /// <param name="procName">存储过程名称。</param>
        void RunProcWithNoQuery(string procName);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <remarks>无返回值。</remarks>
        void RunProcWithNoQuery(string procName, IDbDataParameter[] dataParams);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        IDataReader RunProcGetDataReader(string procName, params int[] resultNum);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        IDataReader RunProcGetDataReader(string procName, IDbDataParameter[] dataParams, params int[] resultNum);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        DataSet RunProcGetDataSet(string procName, params int[] resultNum);

        /// <summary>
        ///  执行存储过程，返回DataSet,2011-05-16
        ///  存储过程名对应数据库中名称，可以直接传入参数值
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        DataSet RunProcGetDataSet(string procName, params object[] parameterValues);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数值列表。</param>
        /// <remarks>无返回值。</remarks>
        void RunProcWithNoQuery(string procName, params object[] parameterValues);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <returns>指定的结果集。用IDataReader。</returns>
        IDataReader RunProcGetDataReader(string procName, params object[] parameterValues);

        /// <summary>
        /// 执行存储过程,返回DataSet. lizhonghua add 20070914。
        /// 直接可以传入参数名称和值，在内部构造IDbDataParameter
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="paramNames">参数名字</param>
        /// <param name="paramValues">参数值</param>
        /// <param name="resultNum">返回的数据集的个数</param>
        /// <returns>指定的结果集，用DataSet</returns>
        DataSet RunProcGetDataSet(string procName, string[] paramNames, object[] paramValues, params int[] resultNum);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        DataSet RunProcGetDataSet(string procName, IDbDataParameter[] dataParams, params int[] resultNum);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="dataSetTableName">返回的<see cref="DataSet"/>中<see cref="DataTable"/>的<code>TableName</code>。</param>
        /// <param name="procName">存储过程名称。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="resultNum">返回的数据集的个数。</param>
        /// <returns>指定的结果集。用DataSet。</returns>
        DataSet RunProcGetDataSet(string dataSetTableName, string procName, IDbDataParameter[] dataParams, params int[] resultNum);

        #endregion

        #region 执行SQL。
        /// <summary>
        /// 执行无返回数据集的SQL。
        /// 返回类型改为 int ，返回SQL执行影响的行数。李中华 20070920
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        int ExecSqlStatement(string sqlStatement);

        /// <summary>
        /// 根据输入的SQL集合以及SQL集合相应的参数集合，执行多个SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// </remarks>
        void ExecSqlStatement(string[] sqlStatements);

        /// <summary>
        /// 根据输入的SQL集合以及SQL集合相应的参数集合，执行多个SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="isRunTran">是否启动内部事务</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// </remarks>
        void ExecSqlStatement(string[] sqlStatements, bool isRunTran);

        /// <summary>
        /// 根据输入的SQL集合以及SQL集合相应的参数集合，执行多个SQL。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="sqlsDataParams">多个sql的相应参数数组。</param>
        /// <remarks>
        /// 在一个事务中，执行多个SQL。
        /// 若当前的Database对象已经启动事务，则使用当前事务；否则启动一个事务，该事务在SQL执行完毕后自动提交。
        /// 参数数组为二维数组，第一维的下标和SQL集合下标对应。
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        void ExecSqlStatement(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams);

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
        void ExecSqlStatement(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams, bool isRunTran);

        /// <summary>
        /// 根据输入的参数列表，执行无返回数据集的SQL。
        /// 返回类型改为 int ，返回SQL执行影响的行数。李中华 20070920
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <remarks>
        /// 在SQL中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        int ExecSqlStatement(string sqlStatement, params IDbDataParameter[] dataParams);

        /// <summary>
        /// 根据输入的参数值列表，执行无返回数据集的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        void ExecSqlStatement(string sqlStatement, params object[] paramDataList);

        /// <summary>
        /// 执行返回单个结果的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <returns>结果值</returns>
        object ExecuteScalar(string sqlStatement);

        /// <summary>
        /// 执行返回单个结果的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <param name="sqlDataParams">参数列表</param>
        /// <returns>结果值</returns>
        /// <remarks>
        /// 在SQL中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        object ExecuteScalar(string sqlStatement, IDbDataParameter[] sqlDataParams);

        /// <summary>
        /// 根据输入的sql语句以及参数值列表，执行返回单个结果。
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <param name="paramDataList">参数值列表</param>
        /// <returns>结果值</returns>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        object ExecuteScalar(string sqlStatement, params object[] paramDataList);

        /// <summary>
        /// 执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        IDataReader ExecuteReader(string sqlStatement);

        /// <summary>
        /// 执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        /// <remarks>
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        IDataReader ExecuteReader(string sqlStatement, IDbDataParameter[] dataParams);

        /// <summary>
        /// 根据输入的参数值列表，执行获取数据集（IDataReader）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <returns>返回的结果集。用IDataReader。</returns>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        IDataReader ExecuteReader(string sqlStatement, params object[] paramDataList);

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        DataSet ExecuteDataSet(string sqlStatement);

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL集合。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>多条SQL查询结果填充到同一个结果集中。</remarks>
        DataSet ExecuteDataSet(string[] sqlStatements);

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
        DataSet ExecuteDataSet(string[] sqlStatements, IDbDataParameter[][] sqlsDataParams);

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL集合，指定数据集中每个Table的名称。
        /// </summary>
        /// <param name="sqlStatements">SQL语句。</param>
        /// <param name="tableNames">返回数据集的表名称集合。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>多条查询填充到同一个结果集中。</remarks>
        DataSet ExecuteDataSet(string[] sqlStatements, string[] tableNames);

        /// <summary>
        /// 执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>
        /// 每个SQL语句中对应参数的位置使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        DataSet ExecuteDataSet(string sqlStatement, IDbDataParameter[] dataParams);

        /// <summary>
        /// 根据输入的参数值列表，执行获取数据集（DataSet）的SQL。
        /// </summary>
        /// <param name="sqlStatement">SQL语句。</param>
        /// <param name="paramDataList">参数值列表。</param>
        /// <returns>返回的结果集。用DataSet。</returns>
        /// <remarks>
        /// 使用参数值集合，主要目标是简化调用代码，内部会自动生成参数名称，构造参数数组。
        /// 在SQL中对应参数的地方使用“{序号}”的方式，序号从“0”开始。
        /// </remarks>
        DataSet ExecuteDataSet(string sqlStatement, params object[] paramDataList);

        DataSet ExecuteDataSet(string sqlStatement, object param1, object param2, object param3);


        /// <summary>
        /// 执行数据的批量导入功能
        /// </summary>
        /// <param name="srcdt">数据源DataTable</param>
        /// <param name="destTableName">要导入的目标表名</param>
        /// <returns>批量导入的数据行数</returns>
        int DataBatchImport(DataTable srcdt, string destTableName);

        /// <summary>
        /// 返回当前连接的数据源的架构信息
        /// </summary>
        /// <returns>包含架构信息的 DataTable</returns>
        DataTable GetSchema();

        /// <summary>
        /// 使用表示架构名称的指定字符串返回当前连接的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称</param>
        /// <returns>包含架构信息的 DataTable</returns>
        DataTable GetSchema(string collectionName);

        /// <summary>
        /// 使用表示架构名称的指定字符串以及表示限制值的指定字符串数组返回当前连接的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称</param>
        /// <param name="restrictionValues">为请求的架构指定一组限制值</param>
        /// <returns>包含架构信息的 DataTable</returns>
        DataTable GetSchema(string collectionName, string[] restrictionValues);

        #endregion

        #region 事务操作。
        /// <summary>
        /// 开始事务。
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 根据指定的隔离级别开始事务。
        /// </summary>
        /// <param name="isolationLevel">隔离级别。</param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务。
        /// </summary>
        void Rollback();

        void EnlistTransaction(System.Transactions.Transaction transaction);

        /// <summary> 是否当前数据库访问对象启动的事务正在进行中。</summary>
        bool IsInTransaction
        {
            get;
        }

        IsolationLevel IsolationLevel
        {
            get;
        }

        #endregion

        #region 公共操作。
        /// <summary>
        /// 转换为日期比较中使用的sql。
        /// </summary>
        /// <param name="dateString">日期字符串。</param>
        /// <returns>转换后的字符串。</returns>
        /// <remarks>数据库中日期是日期类型的时候使用。</remarks>
        string ToDate(string dateString);

        /// <summary>
        /// 获取SQL中字符串的连接符。
        /// </summary>
        string JoinSymbol
        {
            get;
        }

        /// <summary>
        /// 获取SQL中创建GUID的函数。
        /// </summary>
        string NewIdFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中获取数据库当前时间的函数。
        /// </summary>
        string DBDataTimeFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中取字符串子串的函数。
        /// </summary>
        string SubStrFunc
        {
            get;
        }

        /// <summary>
        /// 获取数据库当前时间。
        /// </summary>
        DateTime CurrentDateTime
        {
            get;
        }

        /// <summary>
        /// 获取SQL中判断Null值的函数。
        /// </summary>
        string IsNullFunc
        {
            get;
        }

        /// <summary>
        /// 获取SQL中取字符串长度的函数。
        /// </summary>
        string StrLenFunc
        {
            get;
        }

        /// <summary>
        /// 是否在每次执行完SQL后自动关闭连接。
        /// </summary>
        bool AutoClose
        {
            get;
            set;
        }

        #endregion
    }
}
