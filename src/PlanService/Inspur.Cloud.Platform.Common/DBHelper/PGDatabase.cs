using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public class PGDatabase
    {
        private static IConfigurationRoot DbConfig = Utility.GetJsonConfig("configs/dbconfig.json");
        private string ConnectionString { get; set; }
        public static PGDatabase GetDatabase(string code)
        {
            PGDatabase db = new PGDatabase();
            db.ConnectionString = DbConfig.GetConnectionString(code);
            return db;
        }

        public int ExcuteNonQuery(string cmdText)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdText;
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int ExcuteNonQuery(string cmdText, List<NpgsqlParameter> objParams)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Parameters.AddRange(objParams.ToArray());
                    cmd.CommandText = cmdText;
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int ExcuteNonQuery(string cmdText, params object[] objParams)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
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
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int ExcuteNonQuery(string cmdText, params NpgsqlParameter[] objParams)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Parameters.AddRange(objParams);
                    cmd.CommandText = cmdText;
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int InsertData(string tableName, Dictionary<string, object> dic)
        {
            string sql = "insert into {0} ({1}) values ({2}) ";
            string fiels = "";
            string vals = "";
            int i = 0;
            foreach (var item in dic)
            {
                fiels += item.Key + ",";
                vals += string.Format("{{{0}}},", i++);
            }

            fiels = fiels.TrimEnd(',');
            vals = vals.TrimEnd(',');
            sql = string.Format(sql, tableName, fiels, vals);

            object[] pas = new object[dic.Count];
            dic.Values.CopyTo(pas, 0);
            return ExcuteNonQuery(sql, pas);
        }

        public int UpdateData(string tableName, string key, string keyValue, Dictionary<string, object> dic)
        {
            return 0;
            string sql = $"update {tableName} set ({1})  where {key}='{keyValue}'";
            string fiels = "";
            string vals = "";
            int i = 0;
            foreach (var item in dic)
            {
                fiels += item.Key + ",";
                vals += string.Format("{{{0}}},", i++);
            }

            fiels = fiels.TrimEnd(',');
            vals = vals.TrimEnd(',');
            sql = string.Format(sql, tableName, fiels, vals);

            object[] pas = new object[dic.Count];
            dic.Values.CopyTo(pas, 0);
            return ExcuteNonQuery(sql, pas);
        }

        public IDataReader ExcuteDataReader(string cmdText)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            NpgsqlDataReader reader = default(NpgsqlDataReader);
            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(cmdText, conn);
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }
                throw ex;
            }

            return reader;
        }

        public IDataReader ExcuteDataReader(string cmdText, params object[] objParams)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            NpgsqlDataReader reader = default(NpgsqlDataReader);
            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                for (int i = 0; i < objParams.Length; i++)
                {
                    string pa = string.Format("p{0}", i);
                    cmd.Parameters.AddWithValue(pa, objParams[i]);
                    cmdText = cmdText.Replace("{" + i + "}", "@" + pa);
                }
                cmd.CommandText = cmdText;
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }
                throw ex;
            }

            return reader;

        }


        public object ExecuteScalar(string cmdText)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdText;
                    return cmd.ExecuteScalar();
                }
            }
        }

        public object ExecuteScalar(string cmdText, Dictionary<string, string> paramDic)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdText;
                    foreach (var item in paramDic)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

        public object ExecuteScalar(string cmdText, params object[] objParams)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    for (int i = 0; i < objParams.Length; i++)
                    {
                        string pa = string.Format("p{0}", i);
                        cmd.Parameters.AddWithValue(pa, objParams[i]);
                        cmdText = cmdText.Replace("{" + i + "}", "@" + pa);
                    }
                    cmd.CommandText = cmdText;

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
