using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    /// <summary>
    /// 组件
    /// </summary>
    public class WRComponent
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 组件模板
        /// </summary>
        public WRComponentModel Model { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
