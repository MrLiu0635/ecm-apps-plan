using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Runtime.CompilerServices;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// PostgreSQLDatabase 的摘要说明。
    /// </summary>
    public class PostgreSQLDatabase : Database
    {
        private const string dateFormat = "YYYY-MM-DD HH24:MI:SS";
        private NpgsqlConnection connection = null;
        #region 构造函数。

        /// <summary>
        /// 构造一个database实例对象
        /// </summary>
        /// <param name="dbConfiguration">连接信息</param>
        public PostgreSQLDatabase(GSPDbConfigData dbConfiguration)
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
            get { return GSPDbType.PostgreSQL; }
        }

        /// <summary>
        /// 数据库版本。
        /// </summary>
        protected override string OnGetDatabaseVersion()
        {
            if (this.dbConn == null)
                return GSPDbType.PostgreSQL.ToString();
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
                return "||";
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
            //NpgsqlParameter param = (NpgsqlParameter)base.MakeParam(paramName, direction, dataType, size, paramValue);

            NpgsqlParameter param = this.CreateDBDataParameter() as NpgsqlParameter;
            param.ParameterName = paramName;

            if (size > 0)
                param.Size = size;
            if (dataType != GSPDbDataType.Default)
            {
                param.NpgsqlDbType = (NpgsqlDbType)DBTypeManager.GetDBType(this.DbType, dataType);
            }
            else
            {
                //如果都用bit存储boolean值，则放开；如果用boolean类型存储boolean,则放开
                //if (paramValue != null && paramValue != DBNull.Value && paramValue is bool)
                //{
                //    //反射微软驱动:Boolean是对应byte
                //    //new MetaType(DbType.Boolean, OracleType.Byte, OCI.DATATYPE.UNSIGNEDINT, "UNSIGNED INTEGER", typeof(byte), typeof(byte), 1, 1, false)
                //    param.NpgsqlDbType = NpgsqlDbType.Bit;
                //}
                //加这一句是对枚举类型的报错
                if (paramValue != null && paramValue != DBNull.Value && paramValue.GetType().IsEnum)
                {
                    param.NpgsqlDbType = (NpgsqlDbType)DBTypeManager.GetDBType(this.DbType, GSPDbDataType.Int);
                }
            }
            //参数方向
            param.Direction = direction;

            //设置参数值，对于DateTime类型，特殊处理下
            if (direction != ParameterDirection.Output || (paramValue != null && paramValue != DBNull.Value))
            {
                //需要特殊处理的               
                //日期类型的
                if (paramValue != null && paramValue != DBNull.Value && dataType == GSPDbDataType.DateTime)
                {
                    DateTime date = Convert.ToDateTime(param.Value);
                    param.Value = date;
                }
                else
                    param.Value = paramValue;
            }
            return param;
        }

        #endregion

        #region 其他数据库无关接口的创建接口。

        /// <summary>
        /// 打开数据访问连接。
        /// </summary>
        public override void Open()
        {           
            if (dbConn == null)
            {
                dbConn = this.CreateDBConnection();
            }
            if (dbConn.State != ConnectionState.Open)
            {
                dbConn.ConnectionString = this.dbConfiguration.ConnectionString;
                //SetSQLConnectionContext(_dbConn, _sessionInfo);
                dbConn.Open();
            }
        }        

        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns>数据库连接接口</returns>
        protected override IDbConnection CreateDBConnection()
        {
            this.connection = new NpgsqlConnection();
            return this.connection;
        }

        /// <summary>
        /// 建立数据库命令
        /// </summary>
        /// <returns>数据库命令接口</returns>
        protected override IDbCommand CreateDBCommand()
        {
            return new NpgsqlCommand();
        }

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <returns>数据库适配器接口</returns>
        protected override IDbDataAdapter CreateDbDataAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        /// <summary>
        /// 建立数据库适配器
        /// </summary>
        /// <param name="selectCommandText">查询Sql文本。</param>
        /// <returns>数据库适配器接口</returns>
        protected override IDbDataAdapter CreateDbDataAdapter(string selectCommandText)
        {
            return new NpgsqlDataAdapter(selectCommandText, (NpgsqlConnection)this.dbConn);
        }

        /// <summary>
        /// 建立数据库参数
        /// </summary>
        /// <returns>数据库参数接口</returns>
        protected override IDbDataParameter CreateDBDataParameter()
        {
            return new NpgsqlParameter();
        }

        /// <summary>
        /// 建立数据库CommandBuilder
        /// </summary>
        /// <param name="dataAdapter">数据库适配器接口</param>
        /// <returns>命令生成器</returns>
        protected DbCommandBuilder CreateDbCommandBuilder(IDbDataAdapter dataAdapter)
        {
            return new NpgsqlCommandBuilder(dataAdapter as NpgsqlDataAdapter) as DbCommandBuilder;
        }

        #region 处理参数

        /// <summary>
        /// 在执行前对参数的处理。
        /// </summary>
        /// <param name="dataParams">参数数组。</param>
        /// <param name="isSqlStatement">是否是Sql语句。</param>
        protected override void HandlingParameters(IDbDataParameter[] dataParams, bool isSqlStatement)
        {
            if (dataParams == null)
                return;

            for (int index = 0; index < dataParams.Length; index++)
                this.HandlingParameter(dataParams[index], isSqlStatement);
        }

        private void HandlingParameter(IDbDataParameter parameter, bool isSqlStatement)
        {
            if (parameter != null && !string.IsNullOrEmpty(parameter.ParameterName) && !parameter.ParameterName.StartsWith(":"))
                parameter.ParameterName = ":" + parameter.ParameterName;
        }
        #endregion

        #region 覆写
        private bool CheckOutParam(IDbDataParameter[] dataParams)
        {
            foreach (IDbDataParameter item in dataParams)
            {
                if (item.Direction == ParameterDirection.InputOutput || item.Direction == ParameterDirection.Output)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="dataSetTableName"></param>
        /// <param name="procName"></param>
        /// <param name="dataParams"></param>
        /// <param name="resultNum"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override DataSet RunProcGetDataSet(string dataSetTableName, string procName, IDbDataParameter[] dataParams, params int[] resultNum)
        {
            DataSet ds = base.RunProcGetDataSet(dataSetTableName, procName, dataParams, resultNum);

            if (CheckOutParam(dataParams))
            {
                if (ds.Tables.Count > 0)
                {
                    ds.Tables.RemoveAt(0);
                }
            }
            return ds;
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override DataSet RunProcGetDataSet(string storedProcedureName, params object[] parameterValues)
        {
            DataSet ds = null;
            try
            {
                this.Open();
                DbCommand command = GetStoredProcCommand(storedProcedureName, parameterValues);

                NpgsqlParameter[] dataParams = new NpgsqlParameter[parameterValues.Length];
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    dataParams[i] = (NpgsqlParameter)(command.Parameters[i]);//parameterValues为其他非NpgsqlParameter类型
                }
                command.Parameters.Clear(); //解决dataParams被command占用的问题：DbCommandHandling中无法add进dbCommand.Parameters
                if (this.context.ResultCount > 0)
                {
                    ds = this.RunProcGetDataSet(storedProcedureName, dataParams, this.context.ResultCount);
                }
                else
                {
                    ds = this.RunProcGetDataSet(storedProcedureName, dataParams);
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
        /// 获取IDataReader
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="dataParams"></param>
        /// <param name="resultNum"></param>F:\TFSMapping\GSP6\Dev\GFI\GSF\src\DataAccess\DatabaseImpl\PostgreSQL\PostgreSQLConfigData.cs
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IDataReader RunProcGetDataReader(string procName, IDbDataParameter[] dataParams, params int[] resultNum)
        {
            IDataReader dr = base.RunProcGetDataReader(procName, dataParams, resultNum);

            if (CheckOutParam(dataParams))
            {
                dr.NextResult();
            }
            return dr;
        }

        /// <summary>
        /// 获取IDataReader
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IDataReader RunProcGetDataReader(string procName, params object[] parameterValues)
        {
            IDataReader dataReader = null;
            DbCommand command = null;
            try
            {
                this.Open();
                command = GetStoredProcCommand(procName, parameterValues);
                command.Connection = this.dbConn as DbConnection;
                command.Transaction = this.trans as DbTransaction;

                NpgsqlParameter[] dataParams = new NpgsqlParameter[parameterValues.Length];
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    dataParams[i] = (NpgsqlParameter)(command.Parameters[i]);//parameterValues为其他非NpgsqlParameter类型
                }
                command.Parameters.Clear(); //解决dataParams被command占用的问题：DbCommandHandling中无法add进dbCommand.Parameters
                if (this.context.ResultCount > 0)
                {
                    dataReader = this.RunProcGetDataReader(procName, dataParams, this.context.ResultCount);
                }
                else
                {
                    dataReader = this.RunProcGetDataReader(procName, dataParams);
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

        protected override bool SameNumberOfParametersAndValues(DbCommand command,
                                                               object[] values)
        {
            //函数存在返回结果集值时，需要定义recursor，一般程序调用时vlaues列表中不要求带着，所以不能强制再要求参数个数
            int numberOfParametersToStoredProcedure = command.Parameters.Count;
            int numberOfValuesProvidedForStoredProcedure = values.Length;
            this.context.ResultCount = numberOfParametersToStoredProcedure - numberOfValuesProvidedForStoredProcedure;
            return numberOfParametersToStoredProcedure >= numberOfValuesProvidedForStoredProcedure;
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
                dbCommand.Transaction = (NpgsqlTransaction)this.trans;
            }
            else if (null != context.InternalTransaction)
                dbCommand.Transaction = context.InternalTransaction;

            dbCommand.CommandTimeout = this.dbConfiguration.CommandTimeout;

            // 没使用游标的情况  DbExecuteContext.RESULTCOUNT:-1
            if (this.context.ResultCount == -1)
            {
                // 添加参数
                if (dataParams == null)
                    return dbCommand;
                int i = 0;
                dbCommand.Parameters.Clear();
                foreach (IDbDataParameter parameter in dataParams)
                {
                    NpgsqlParameter np = parameter as NpgsqlParameter;
                    if (np.NpgsqlDbType == NpgsqlDbType.Refcursor)
                    {
                        np.Value = string.Format(@"cursor{0}", i++);
                    }
                    dbCommand.Parameters.Add(parameter);
                }
            }
            // 若有多个结果集返回，PostgreSQL需要加上游标参数来返回结果集。以下为处理
            else if (this.context.ResultCount > 0)
            {
                DbCommandHandling(cmdText, dbCommand, dataParams);
            }

            return dbCommand;
        }

        /// <summary>
        /// 拼写sql和参数处理
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="dbCommand"></param>
        /// <param name="parameters"></param>
        private void DbCommandHandling(string cmdText, IDbCommand dbCommand, IDbDataParameter[] parameters)
        {
            dbCommand.CommandType = CommandType.Text; //构造新的sql，使用游标取出结果表
            dbCommand.Parameters.Clear();

            int i = 0;
            foreach (var parameter in parameters)
            {
                dbCommand.Parameters.Add(parameter);
            }

            for (i = 0; i < this.context.ResultCount; i++)
            {
                string cur = string.Format(@"cursor{0}", i);
                NpgsqlParameter np = new NpgsqlParameter(cur, NpgsqlDbType.Refcursor) { Value = cur };
                dbCommand.Parameters.Add(np);
            }


            //动态拼sql
            //select * from GetMultiResault('9999','cursor1','cursor2'); fetch all in cursor1;fetch all in cursor2;
            string selectsql = @" select * from {0}(";
            string vcharsql = @" '{0}', ";
            string fetchsql = @" fetch all in cursor{0}; ";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(selectsql, cmdText); //cmdText:存储过程名

            foreach (NpgsqlParameter parameter in dbCommand.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Output)
                {
                    continue;
                }

                sb.AppendFormat(vcharsql, parameter.Value);
            }

            selectsql = sb.ToString();
            selectsql = selectsql.TrimEnd(new char[] { ' ', ',' });
            sb.Clear();
            sb.Append(selectsql);
            sb.Append(" ); ");

            for (i = 0; i < this.context.ResultCount; i++)
            {
                sb.AppendFormat(fetchsql, i);
            }

            dbCommand.CommandText = sb.ToString();
        }

        /// <summary>
        /// 执行数据的批量导入功能
        /// </summary>
        /// <param name="srcdt">数据源</param>
        /// <param name="destTableName">要导入的目的数据表</param>
        /// <returns>导入的数据行数</returns>
        protected override int DataBatchImportPart(DataTable srcdt, string destTableName)
        {
            this.Open();
            DataSet myDS = new DataSet();
            IDbCommand command = SetCommand(string.Format("select * from {0} where 1=0", destTableName), null, true);
            IDbDataAdapter sqlDA = CreateDbDataAdapter();
            sqlDA.SelectCommand = command;
            sqlDA.TableMappings.Add(destTableName, destTableName);
            sqlDA.Fill(myDS);

            //DbCommandBuilder 自动生成用于协调 DataSet 的更改与关联数据库的单表命令
            //此对象用于填充SqlDataAdapter 的InsertCommand、DeleteCommand、UpdateCommand对象
            DbCommandBuilder sqlcommandbuilder = CreateDbCommandBuilder(sqlDA);
            for (int j = 0; j < srcdt.Rows.Count; j++)
            {
                myDS.Tables[0].Rows.Add(srcdt.Rows[j].ItemArray);
            }

            sqlDA.Update(myDS);
            return srcdt.Rows.Count; 
        }
        /// <summary>
        /// 转换为日期比较中使用的sql。
        /// </summary>
        /// <param name="dateString">日期字符串。</param>
        /// <returns>转换后的字符串。</returns>
        /// <remarks>数据库中日期是日期类型的时候使用。</remarks>
        public override string ToDate(string dateString)
        {
            DateTime date = Convert.ToDateTime(dateString);
            return "TO_DATE('" + date.ToString(fullDateFormat) + "', '" + dateFormat + "')";
        }

        /// <summary>
        /// 获取SQL中字符串的连接符。
        /// </summary>
        public override string JoinSymbol
        {
            get { return "||"; }
        }

        /// <summary>
        /// 获取SQL中创建GUID的函数。
        /// </summary>
        public override string NewIdFunc
        {
            //默认PostgreSQL是木有UUID函数可使用，而不像MySQL提供uuid()函数，
            //不过在contrib里有，只需要导入一下uuid-ossp.sql即可。（PS：注意权限问题，要Pg可读改文件。）
            //主要就是uuid_generate_v1和uuid_generate_v4，当然还有uuid_generate_v3和uuid_generate_v5。其他使用可以参见PostgreSQL官方文档uuid-ossp。
            //create extension "uuid-ossp"
            get
            {
                return "uuid_generate_v1";
            }
        }

        /// <summary>
        /// 获取SQL中获取数据库当前时间的函数。
        /// </summary>
        public override string DBDataTimeFunc
        {
            get { return "current_timestamp"; }
        }

        /// <summary>
        /// 获取SQL中取字符串子串的函数。
        /// </summary>
        public override string SubStrFunc
        {
            get { return "substring"; }
        }

        /// <summary>
        /// 获取数据库当前时间。
        /// </summary>
        public override DateTime CurrentDateTime
        {
            get
            {
                string sql = "SELECT LOCALTIMESTAMP AS SYSDATE";
                //string sql = "SELECT CURRENT_TIMESTAMP AS SYSDATE";
                object obj = this.ExecuteScalar(sql);

                return Convert.ToDateTime(obj);
            }
        }

        ///// <summary>
        ///// 获取当前数据库的UTC时间。
        ///// </summary>
        //public override DateTime CurrentUTCDateTime
        //{
        //    get
        //    {
        //        string sql = "SELECT CURRENT_TIMESTAMP AS SYSDATE";
        //        object obj = this.ExecuteScalar(sql);

        //        return Convert.ToDateTime(obj);
        //    }
        //}

        /// <summary>
        /// 获取SQL中判断Null值的函数。
        /// </summary>
        public override string IsNullFunc
        {
            //nvl(parent_id, '-1')
            get { return "nvl"; }
        }

        /// <summary>
        /// 获取SQL中取字符串长度的函数。
        /// </summary>
        public override string StrLenFunc
        {
            get { return "length"; }
        }

        /// <summary>
        /// 在事务中进行登记
        /// </summary>
        /// <param name="transaction">分布式事务</param>
        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            if (this.connection != null)
            {
                this.connection.EnlistTransaction(transaction);
            }
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
            get { return ':'; }
        }
        
        /// <summary>
        /// Does this <see cref='Database'/> object support parameter discovery?
        /// </summary>
        /// <value>true.</value>
        public override bool SupportsParemeterDiscovery
        {
            get { return true; }
        }

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        protected override string BuildParameterName(string name)
        {
            //DataValidator.CheckForNullReference(name, "name");

            if (name[0] != ParameterToken)
            {
                return name.Insert(0, new string(ParameterToken, 1));
            }
            return name;
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object. 
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        /// <remarks>The <see cref="DbCommand"/> must be a <see cref="SqlCommand"/> instance.</remarks>
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {
            NpgsqlCommandBuilder.DeriveParameters((NpgsqlCommand)discoveryCommand);
        }
        #endregion
    }
}