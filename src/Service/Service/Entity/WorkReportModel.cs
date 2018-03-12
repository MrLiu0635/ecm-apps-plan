using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class WorkReportModel
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 组件模板
        /// </summary>
        public List<WRComponentModel> ComponentModels { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public WRType WRType { get; set; }

    }
}
