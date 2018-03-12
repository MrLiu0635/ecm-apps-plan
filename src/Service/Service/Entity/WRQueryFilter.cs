using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    /// <summary>
    /// 请求实体
    /// </summary>
    public class WRQueryFilter
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public List<string> WorkReportSenders { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public List<string> WorkReportRecipients { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
