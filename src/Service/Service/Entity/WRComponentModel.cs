using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    /// <summary>
    /// 组件模板
    /// </summary>
    public class WRComponentModel
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public WRType WorkReportType { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public ModelType ModelType { get; set; }

        /// <summary>
        /// 模版顺序
        /// </summary>
        public int Order { get; set; }
    }
}
