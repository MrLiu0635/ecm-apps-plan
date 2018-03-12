using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.EcmCloud.Apps.Plan.Service.Entity
{
    public class User
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 姓名全称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属组织名称
        /// </summary>
        public string OrgName { get; set; }
    }
}
