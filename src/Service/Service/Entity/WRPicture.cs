using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    /// <summary>
    /// 日志图片
    /// </summary>
    public class WRPicture
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 图片内容（目前base64编码）
        /// </summary>
        public string Content { get; set; }
    }
}
