using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Common
{
    public class MysqlDatabase
    {
        private string ConnectionString { get; set; }

        private MysqlDatabase()
        { }

        public static MysqlDatabase GetDatabase(string code)
        {
            MysqlDatabase db = new MysqlDatabase();

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(@"configs/dbconfig.json");
            var connectionStringConfig = builder.Build();
            db.ConnectionString = connectionStringConfig.GetConnectionString(code);
            return db;
        }

        public int ExcuteNonQuery(string sql, params object[] objParams)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                for (int i = 0; i < objParams.Length; i++)
                {
                    string pa = string.Format("p{0}", i);

                    var objPar = objParams[i];
                    if (objPar == null)
                    {
                        objPar = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue(pa, objPar);
                    sql = sql.Replace("{" + i + "}", "@" + pa);
                }

                cmd.CommandText = sql;
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        public async Task<int> ExcuteNonQueryAsync(string sql, params object[] objParams)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                for (int i = 0; i < objParams.Length; i++)
                {
                    string pa = string.Format("p{0}", i);

                    var objPar = objParams[i];
                    if (objPar == null)
                    {
                        objPar = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue(pa, objPar);
                    sql = sql.Replace("{" + i + "}", "@" + pa);
                }
                cmd.CommandText = sql;
                await conn.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
            }
            return result;
        }

        public async void InsertDataAsync(string tableName, Dictionary<string, string> data)
        {
            StringBuilder columns = new StringBuilder(100);
            StringBuilder values = new StringBuilder(100);

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                foreach (var item in data)
                {
                    columns.AppendFormat("{0},", item.Key);
                    values.AppendFormat("@{0},", item.Key);

                    cmd.Parameters.AddWithValue(string.Format("@{0}", item.Key), item.Value);
                }
                string sql = string.Format("insert into {0} ({1}) values ({2})", tableName, columns.ToString().TrimEnd(','), values.ToString().TrimEnd(','));
                cmd.CommandText = sql;
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<object> ExecuteScalar(string cmdText, params object[] objParams)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                for (int i = 0; i < objParams.Length; i++)
                {
                    string pa = string.Format("p{0}", i);

                    var objPar = objParams[i];
                    if (objPar == null)
                    {
                        objPar = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue(pa, objPar);
                    cmdText = cmdText.Replace("{" + i + "}", "@" + pa);
                }
                cmd.CommandText = cmdText;
                await conn.OpenAsync();
                return await cmd.ExecuteScalarAsync();
            }
        }

    }
}
