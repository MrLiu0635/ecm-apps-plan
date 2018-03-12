using Inspur.GSP.Gsf.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// SqlDatabase 的摘要说明。
    /// </summary>
    public class SqlDatabase : Database
    {
        private SqlConnection sqlconnection = null;

        #region 构造函数。

        /// <summary>
        /// 构造一个database实例对象
        /// </summary>
        /// <param name="dbConfiguration">连接信息</param>
        public SqlDatabase(GSPDbConfigData dbConfiguration)
            : base(dbConfiguration)
        {
        }

        #endregion

        #region 属性。

        /// <summary>
        /// 数据库类型。只读。
        /// </summary>
        public override GSPDbType DbType
        {
            get { return GSPDbType.SQLServer; }
        }

        /// <summary>
        /// 数据库版本。
        /// </summary>
        protected override string OnGetDatabaseVersion()
        {
            if (this.dbConn == null)
                return GSPDbType.SQLServer.ToString();
            else
            {
                return (this.dbConn as DbConnection).ServerVersion;
            }
        }

        /// <summary>
        /// 连接操作符。用于连接字符串。
        /// </summary>
        /// <remarks>不同数据在连接字符串时所用的具体操作符不同，在此引入，以向上屏蔽细节。</remarks>
        public override string ConcatenationOperator
        {
            get
            {
                return "+";
            }
        }


        #endregion

        #region 生成参数。

        /// <summary>
        /// 生成参数。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="direction">参数方向。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">长度。</param>
        /// <param name="paramValue">参数值。</param>
        /// <returns>构造好的参数对象。</returns>
        /// <remarks>
        /// 注意：重写的目的是为参数类型赋值。
        /// 若数据类型为Default，不处理参数类型字段。
        /// </remarks>
        public override IDbDataParameter MakeParam(string paramName, ParameterDirection direction, GSPDbDataType dataType, int size, object paramValue)
        {
            SqlParameter param = (SqlParameter)base.MakeParam(paramName, direction, dataType, size, paramValue);
            //兼容nvarchar(max)类型字段，绑定变量4001-8000范围时报The incoming tabular data stream (TDS) remote procedure call (RPC) protocol stream is incorrect" exception when using NVarchar parameters with Sqlclient
            //https://support.microsoft.com/en-us/kb/970519
            //Status: Microsoft has confirmed that this is a bug in the Microsoft products that are listed in the "Applies to" section. This is scheduled to be addressed in the next major release of .NET Framework
            //Resolution：To work around this issue, use one of the following options: 
            //· Set Sqlparamter.size property to -1 to ensure that you are getting the entire data from the backend without truncation. 
            //· When working with String DbTypes whose sizes are greater than 4000, explicitly map them to another SqlDBType like NText instead of using NVarchar(which also is the default SqlDBType for strings). 
            //· Use a value that is not between 4001 and 8000 for Sqlparameter.size.
            if (size > 0)
                param.Size = size;
            else
                param.Size = -1;

            if (dataType != GSPDbDataType.Default)
                param.SqlDbType = (SqlDbType)DBTypeManager.GetDBType(this.DbType, dataType);
            return param;
        }

        #endregion

        #region 其他数据库无关接口的创建接口。
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns>数据库连接接口</returns>
        protected override IDbConnection CreateDBConnection()
        {
            sqlconnection = new SqlConnection();
            return sqlconnection;
        }

        /// <summary>
        /// 建立数据库命令
        /// </summary>
        /// <returns>数据库命令接口</returns>
        protected override IDbCommand CreateDBCommand()
        {
            return new SqlCommand();
        }

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <returns>数据库适配器接口</returns>
        protected override IDbDataAdapter CreateDbDataAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <param name="selectCommandText">查询Sql文本。</param>
        /// <returns>数据库适配器接口</returns>
        protected override IDbDataAdapter CreateDbDataAdapter(string selectCommandText)
        {
            return new SqlDataAdapter(selectCommandText, (SqlConnection)this.dbConn);
        }

        /// <summary>
        /// 建立数据库参数
        /// </summary>
        /// <returns>数据库参数接口</returns>
        protected override IDbDataParameter CreateDBDataParameter()
        {
            return new SqlParameter();
        }

        #region 处理参数

        ///// <summary>
        ///// 在执行前对参数的处理。
        ///// </summary>
        ///// <param name="dataParams">参数数组。</param>
        ///// <param name="isSqlStatement">是否是Sql语句。</param>
        //protected override void HandlingParameters(IDbDataParameter[] dataParams, bool isSqlStatement)
        //{
        //    if (dataParams == null)
        //        return;

        //    for (int index = 0; index < dataParams.Length; index++)
        //        dataParams[index] = this.HandlingParameter(dataParams[index], isSqlStatement);
        //}

        //private IDbDataParameter HandlingParameter(IDbDataParameter parameter, bool isSqlStatement)
        //{
        //    this.HandlingParameterName(parameter);
        //    return this.ExamineParameterDataType(parameter);
        //}

        /// <summary>
        /// 处理Sql Server的参数名。
        /// </summary>
        /// <remarks>为上层调用屏蔽存储过程调用的数据库差异。</remarks>
        /// <param name="parameter">参数。</param>
        private void HandlingParameterName(IDbDataParameter parameter)
        {
            if (parameter != null && !string.IsNullOrEmpty(parameter.ParameterName) && !parameter.ParameterName.StartsWith("@"))
                parameter.ParameterName = "@" + parameter.ParameterName;
        }

        #endregion

        /// <summary>
        /// 处理sql参数。
        /// </summary>
        /// <param name="sqlStatement">Sql语句。</param>
        /// <param name="dataParams">参数数组。</param>
        /// <returns>处理后的sql语句。</returns>
        protected override string HandleSqlStatement(string sqlStatement, IDbDataParameter[] dataParams)
        {
            if (dataParams != null)
            {
                string[] parameterNames = new string[dataParams.Length];
                sqlStatement = sqlStatement.Replace("=:", "=@");
                for (int index = 0; index < dataParams.Length; index++)
                {
                    if (dataParams[index] != null)
                        parameterNames[index] = dataParams[index].ParameterName;
                }
                return String.Format(sqlStatement, parameterNames);
            }
            else
                return sqlStatement;
        }

        /// <summary>
        /// 设置IDbCommand对象。
        /// </summary>
        /// <param name="cmdText">Sql语句或者存储过程名。</param>
        /// <param name="dataParams">参数列表。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        /// <returns>IDbCommand对象。</returns>
        protected override IDbCommand SetCommand(string cmdText, IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            IDbCommand dbCommand = this.CreateDBCommand();
            dbCommand.Connection = this.dbConn;
            dbCommand.CommandText = cmdText;
 

            // 如果执行sql,为Command对象建立SQL模式,否则为执行存储过程模式
            if (isSqlStatement)
            {
                dbCommand.CommandType = CommandType.Text;
            }
            else
            {
                dbCommand.CommandType = CommandType.StoredProcedure;
            }

            if (null != this.trans)
            {
                dbCommand.Transaction = (SqlTransaction)this.trans;
            }
            else if (null != context.InternalTransaction)
                dbCommand.Transaction = context.InternalTransaction;


            dbCommand.CommandTimeout = this.dbConfiguration.CommandTimeout;

            // 添加参数。
            if (dataParams == null)
                return dbCommand;

            dbCommand.Parameters.Clear();
            foreach (IDbDataParameter parameter in dataParams)
            {
                dbCommand.Parameters.Add(parameter);
            }

            return dbCommand;
        }

        /// <summary>
        /// 执行数据的批量导入功能
        /// </summary>
        /// <param name="srcdt">数据源</param>
        /// <param name="destTableName">要导入的目的数据表</param>
        /// <returns>导入的数据行数</returns>
        protected override int DataBatchImportPart(DataTable srcdt, string destTableName)
        {
            int rowCount = 0;
            //SqlBulkCopy sqlbulk = new SqlBulkCopy(this._dbConfiguration.ConnectionString, SqlBulkCopyOptions.UseInternalTransaction);
            SqlBulkCopy sqlbulk;
            bool inTrans = System.Transactions.Transaction.Current != null || this.IsInTransaction;
            if(inTrans)
                sqlbulk = new SqlBulkCopy(sqlconnection, SqlBulkCopyOptions.Default, this.trans as SqlTransaction);
            else
                sqlbulk = new SqlBulkCopy(sqlconnection, SqlBulkCopyOptions.UseInternalTransaction, null);
            sqlbulk.BulkCopyTimeout = this.dbConfiguration.CommandTimeout;
            sqlbulk.DestinationTableName = destTableName;  
            try
            {
                ////字段映射
                //var mappings = sqlbulk.ColumnMappings;
                //foreach (DataColumn column in srcdt.Columns)
                //{
                //    //源列名->目标列名，SqlServer列映射严格区分大小写
                //    mappings.Add(column.ColumnName, column.ColumnName);
                //}
                sqlbulk.WriteToServer(srcdt);
                rowCount = srcdt.Rows.Count;
            }
            catch 
            {
                throw;
            }
            finally
            {
               sqlbulk.Close();
            }
            return rowCount;
        }
        /// <summary>
        /// 转换为日期比较中使用的sql。
        /// </summary>
        /// <param name="dateString">日期字符串。</param>
        /// <returns>转换后的字符串。</returns>
        /// <remarks>数据库中日期是日期类型的时候使用。</remarks>
        public override string ToDate(string dateString)
        {
            return "cast('" + dateString + "' as DateTime) ";
        }

		/// <summary>
		/// 获取SQL中字符串的连接符。
		/// </summary>
		public override string JoinSymbol
		{
			get { return "+"; }
		}

        /// <summary>
        /// 获取SQL中创建GUID的函数。
        /// </summary>
        public override string NewIdFunc
        {
            get { return "CAST(NEWID() AS VARCHAR(36))"; }
        }

        /// <summary>
        /// 获取SQL中获取数据库当前时间的函数。
        /// </summary>
        public override string DBDataTimeFunc
        {
            get { return "GETDATE()"; }
        }

        /// <summary>
        /// 获取SQL中取字符串子串的函数。
        /// </summary>
        public override string SubStrFunc
        {
            get { return "SUBSTRING"; }
        }

        /// <summary>
        /// 获取数据库当前时间。
        /// </summary>
        public override DateTime CurrentDateTime
        {
            get
            {
                string sql = "SELECT GETDATE() AS SYSDATE";
                DataSet ds = this.ExecuteDataSet(sql);

                return Convert.ToDateTime(ds.Tables[0].Rows[0]["SYSDATE"]);
            }
        }

        /// <summary>
        /// 获取SQL中判断Null值的函数。
        /// </summary>
        public override string IsNullFunc
        {
            get { return "isnull"; }
        }

        /// <summary>
        /// 获取SQL中取字符串长度的函数。
        /// </summary>
        public override string StrLenFunc
        {
            get { return "len"; }
        }

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            sqlconnection.EnlistTransaction(transaction);
            return;
        }
        #endregion

        #region 简化存储过程调用
        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the SQL Server database.</para>
        /// </summary>
        /// <value>
        /// <para>The '@' symbol.</para>
        /// </value>
        protected char ParameterToken
        {
            get { return '@'; }
        }

        /// <summary>
        /// Returns the starting index for parameters in a command.
        /// </summary>
        /// <returns>The starting index for parameters in a command.</returns>
        protected override int UserParametersStartIndex()
        {
            return 1;
        }
        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        protected override string BuildParameterName(string name)
        {

            if (name[0] != ParameterToken)
            {
                return name.Insert(0, new string(ParameterToken, 1));
            }
            return name;
        }
        /// <summary>
        /// Determines if the number of parameters in the command matches the array of parameter values.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> containing the parameters.</param>
        /// <param name="values">The array of parameter values.</param>
        /// <returns><see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.</returns>
        protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            int returnParameterCount = 1;
            int numberOfParametersToStoredProcedure = command.Parameters.Count - returnParameterCount;
            int numberOfValuesProvidedForStoredProcedure = values.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }
        

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object. 
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        /// <remarks>The <see cref="DbCommand"/> must be a <see cref="SqlCommand"/> instance.</remarks>
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {
            //SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
        }
        #endregion
    }
}
