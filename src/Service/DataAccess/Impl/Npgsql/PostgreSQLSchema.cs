//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;


//namespace Inspur.GSP.Gsf.DataAccess
//{
//    /// <summary>
//    /// PostgreSQL 的数据库架构类
//    /// </summary>
//    internal class PostgreSQLSchema : GSPDatabaseSchema
//    {

//        //要把Owner从串中取出来
//        private string owner = "";

//        /// <summary>
//        /// 根据输入的数据库访问对象构造。
//        /// </summary>
//        /// <param name="db">数据库访问对象</param>
//        public PostgreSQLSchema(IGSPDatabase db)
//            : base(db)
//        {
//            owner = this._db.User;
//        }

//        #region 对表的操作
//        /// <summary>
//        /// 获取当前数据库中的用户表
//        /// </summary>
//        /// <returns>包含表信息的DataTable</returns>
//        public override DataTable GetTables()
//        {
//            //string selectSql = "select B.TABLE_NAME,B.TABLE_TYPE from PG_TABLES A,information_schema.TABLES B WHERE UPPER(A.TABLEOWNER)={0} AND A.TABLENAME = B.TABLE_NAME ORDER BY B.TABLE_NAME ASC";
//            //DataSet ds = this._db.ExecuteDataSet(selectSql, owner.ToUpper());

//            //string selectSql = "SELECT B.TABLE_NAME,B.TABLE_TYPE from PG_STAT_USER_TABLES A,information_schema.TABLES B WHERE A.RELNAME = B.TABLE_NAME ORDER BY B.TABLE_NAME ASC";
//            //DataSet ds = this._db.ExecuteDataSet(selectSql);
//            //return  ds.Tables[0];

//            DataTable table = null;
//            string[] restrictionValues = { null, null, null, "BASE TABLE" };//new string[] { "table_catalog", "table_schema", "table_name", "table_type" }))
//            table = this._db.GetSchema("Tables", restrictionValues);
//            table.Columns.RemoveAt(0);
//            table.Columns.RemoveAt(0);
//            return table;
//        }

//        /// <summary>
//        /// 获取指定数据库表的唯一性约束与主键约束
//        /// </summary>
//        /// <param name="tableName">表名</param>
//        /// <returns>包含有约束的字符串</returns>
//        public override string GetTableUniqueAndPK(string tableName)
//        {
//            StringBuilder keysAndUnique = new StringBuilder();

//            string[] pkRestrictionValues = { null, null, tableName.ToLower() };//new string[] { "current_database()", "pgtn.nspname", "pgt.relname", "pgc.conname" }))
//            string[] uqRestrictionValues = { null, null, tableName.ToLower() };//new string[] { "current_database()", "pgtn.nspname", "pgt.relname", "pgc.conname" }))
//            string[] indexRestrictionValues = { null, null, tableName.ToLower(), tableName.ToLower() };//new string[] { "current_database()", "n.nspname", "t.relname", "i.relname", "a.attname" }))


//            DataTable pkDt = new DataTable();
//            DataTable uqDt = new DataTable();
//            DataTable indexDt = new DataTable();

//            pkDt = this._db.GetSchema("PrimaryKey", pkRestrictionValues);//获取主键约束名字
//            uqDt = this._db.GetSchema("UniqueKeys", uqRestrictionValues);//获取唯一性约束名字
//            indexDt = this._db.GetSchema("IndexColumns", indexRestrictionValues);//索引所属列

//            if (pkDt.Rows.Count > 0)//找到包含主键的列
//            {
//                foreach (DataRow pkRow in pkDt.Rows)
//                {
//                    foreach (DataRow indexRow in indexDt.Rows)
//                    {
//                        if (indexRow["index_name"].ToString() == pkRow["CONSTRAINT_NAME"].ToString())
//                        {
//                            keysAndUnique.Append(indexRow["column_name"].ToString() + ",");
//                        }
//                    }
//                }
//            }

//            keysAndUnique.Append(";");

//            if (uqDt.Rows.Count > 0)
//            {
//                foreach (DataRow uqRow in uqDt.Rows)
//                {
//                    foreach (DataRow indexRow in indexDt.Rows)
//                    {
//                        if (indexRow["index_name"].ToString() == uqRow["CONSTRAINT_NAME"].ToString())
//                        {
//                            keysAndUnique.Append(indexRow["column_name"].ToString() + ",");
//                        }
//                    }
//                }
//            }

//            return keysAndUnique.ToString();
//        }
//        #endregion

//        #region 对表和视图的共用操作

//        /// <summary>
//        /// 获取指定表(或视图)的所有列信息
//        /// </summary>
//        /// <param name="tableName">表(或视图)名称</param>
//        /// <returns>包含所有列信息的DataTable
//        ///         [0]列名
//        ///         [1]默认值
//        ///         [2]是否为空
//        ///         [3]数据类型
//        ///         [4]字符类的最大长度
//        ///         [5]数字类的精度
//        ///         [6]数字类的尾数
//        /// </returns>
//        public override DataSet GetTableColumns(string tableName)
//        {
//            //采用从系统表里读的办法，GetSchema("Columns")方法取到的字段太多
//            string selectSql = "SELECT column_name, column_default, is_nullable, udt_name AS data_type, character_maximum_length, numeric_precision, numeric_scale FROM information_schema.columns where table_name={0} order by ordinal_position asc";
//            return this._db.ExecuteDataSet(selectSql, tableName.ToLower());
//        }
//        #endregion

//        #region 对视图的操作
//        /// <summary>
//        /// 获取当前数据库中的视图
//        /// </summary>
//        /// <returns>包含视图信息的DataTable</returns>
//        public override DataTable GetViews()
//        {
//            string selectSql = "SELECT table_name as TABLE_NAME, 'VIEW' as TABLE_TYPE FROM information_schema.views";
//            DataSet ds = this._db.ExecuteDataSet(selectSql, owner.ToUpper());
//            DataTable table = ds.Tables[0].Copy();
//            return table;
//        }
//        #endregion
//    }
//}

