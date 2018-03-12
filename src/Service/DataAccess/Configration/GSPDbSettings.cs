using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Inspur.GSP.Gsf.DataAccess
{

    [DisplayName("���ݿ���������")]
    public class GSPDbSettings : ICloneable
    {
        /// <summary>���ݿ����ӵ�����С�����ơ�</summary>
        public const string SectionName = "GSPDbConfiguration";

        /// <summary>��̬Root����</summary>
        private static IConfigurationRoot configurationRoot;

        /// <summary>�����ַ����б�</summary>
        private List<GSPDbConfigData> connectionConfigurations;

        /// <summary>
        /// ��̬���캯��
        /// </summary>
        static GSPDbSettings()
        {
            string path = @"Config/database.json";
            configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path)
                .Build();
        }


        /// <summary>
        /// ���캯����
        /// </summary>
        public GSPDbSettings()
        {

        }

        /// <summary>
        ///  ����ϵͳ�����ݿ����ӵ����á�
        /// </summary>
        /// <returns>���ݿ����ӵ����ã������ļ��в��������ý�ʱ��ʵ����һ���µ�<see cref="GSPDbSettings"/>�����ء�</returns>
        /// <remarks>�����ڶ�ȡ�������ݿ����������б�ĳ�����</remarks>
        public static GSPDbSettings GetGSPDbSettings()
        {
            GSPDbSettings settings = configurationRoot.GetSection(GSPDbSettings.SectionName).Get<GSPDbSettings>();
            if (settings != null)
            {
                settings = settings.Clone() as GSPDbSettings;
            }
            else
            {
                settings = new GSPDbSettings();
            }
            return settings;
        }


        /// <summary>
		/// ������������Ϣ�ļ��ϣ���Ӧjson���ã��ⲿ�����ConnectionConfigurations��
		/// </summary>
        public List<GSPDbConfigData> Connections
        {
            get;
            set;
        }

        /// <summary>
        /// Ĭ�ϵ����ݿ����á�
        /// </summary>
        public string DefaultCode
        {
            get;
            set;
        }


        /// <summary>
        /// ��ȡĬ�ϵ�����������Ϣ��
        /// </summary>
        /// <returns>ָ�������ݿ��������ӡ�</returns>
        public GSPDbConfigData GetDefaultConfiguration()
        {
            return this.GetConfigurationByCode(this.DefaultCode);
        }

        /// <summary>
        /// ������������Ϣ�ļ���
        /// </summary>
        public List<GSPDbConfigData> ConnectionConfigurations
        {
            get
            {
                if (this.Connections != null && this.Connections.Count > 0 && this.connectionConfigurations == null)
                {
                    this.connectionConfigurations = new List<GSPDbConfigData>();
                    for (int i = 0; i < this.Connections.Count; i++)
                    {
                        GSPDbConfigData data = this.Connections[i],
                            item = null;
                        string name = string.Format("GSPDbConfiguration:Connections:{0}", i);
                        switch (data.DbType)
                        {
                            case GSPDbType.PostgreSQL:
                                item = configurationRoot.GetSection(name).Get<PostgreSQLConfigData>();
                                break;
                            case GSPDbType.MySQL:
                                break;
                            case GSPDbType.SQLServer:
                                //item = configurationRoot.GetSection(name).Get<>();
                                break;
                            case GSPDbType.Oracle:
                                break;
                            case GSPDbType.Unknown:
                                break;
                            default:
                                break;
                        }
                        if (item != null)
                            this.connectionConfigurations.Add(item);
                    }
                }

                return connectionConfigurations;
            }
        }

        /// <summary>
        /// ��ȡĬ�ϵ�����������Ϣ��
        /// </summary>
        /// <returns>ָ�������ݿ��������ӡ�</returns>
        public GSPDbConfigData GetConfigurationByCode(string dbCode)
        {
            if (this.ConnectionConfigurations != null && this.ConnectionConfigurations.Count > 0)
            {
                return this.ConnectionConfigurations.Find(s => s.Code == dbCode);
            }
            return null;
        }

        #region ICloneable Members
        /// <summary>
        /// ����һ���µ����ݷ��ʶ��������ࡣ
        /// </summary>
        /// <returns>���Ƶ����ݷ��ʶ��������ࡣ</returns>
        public object Clone()
        {
            GSPDbSettings copy = new GSPDbSettings();
            copy.DefaultCode = this.DefaultCode;
            copy.Connections = this.Connections;
            return copy;
        }
        #endregion
    }
}
