using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using NpgsqlTypes;
using Npgsql.Logging;
using Inspur.GSP.Gsf.DataAccess;

namespace Inspur.ECP.Rtf.Common
{
    public class PgSqlHelper
    {
        private static string connectionString;
        private static string ConnectionString
        {
            get
            {
                if (connectionString == null)
                {
                    var builder = new ConfigurationBuilder();
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile(@"configs/dbconfig.json");
                    var connectionStringConfig = builder.Build();
                    connectionString = connectionStringConfig.GetConnectionString("default");
                }

                return connectionString;
            }
        }
        public static int ExcuteNonQuery(string cmdText)
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

        public static int ExcuteNonQuery(string cmdText, params object[] objParams)
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

        public static IDataReader ExcuteDataReader(string cmdText)
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

            //reader.ReaderClosed += (s, e) =>
            //{
            //    NLogger.Info("ReaderClosed：" + cmdText);
            //    conn.Close();
            //};

            return reader;
        }


        public static IDataReader ExcuteDataReader(string cmdText, params object[] objParams)
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


        public static object ExecuteScalar(string cmdText)
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

        public static object ExecuteScalar(string cmdText, Dictionary<string, string> paramDic)
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

        public static object ExecuteScalar(string cmdText, params object[] objParams)
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
