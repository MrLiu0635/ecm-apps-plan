using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    /// <summary>
    /// 日志实体
    /// </summary>
    public class WorkReport
    {
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public WRType WorkReportType { get; set; }

        /// <summary>
        /// 日志组件列表
        /// </summary>
        public List<WRComponent> WorkReportComps { get; set; }

        /// <summary>
        /// 日志图片
        /// </summary>
        public List<WRPicture> WorkReportPics { get; set; }

        /// <summary>
        /// 日志接收人列表
        /// </summary>
        public List<SysUser> WorkReportRecipients { get; set; }

        /// <summary>
        /// 日志填写日期
        /// </summary>
        public DateTime DateOfFilling { get; set; }

        /// <summary>
        /// 日志创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 日志创建人名称
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 日志最后修改人
        /// </summary>
        public string LastModifier { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// 日志最后修改人名称
        /// </summary>
        public string LastModifierName { get; set; }

        /// <summary>
        /// 日志填报地点
        /// </summary>
        public string Location { get; set; }
    }
}
