using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Api
{
    public class SysUser
    {
        /// <summary>
        /// inspurID，唯一标识
        /// </summary>
        public string ID { get; set; }

        public string InspurID { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string SerialNumber { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        private string first_name { get; set; }
        private string last_name { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 英文名字
        /// </summary>
        public string GlobalName { get; set; }

        public Organization org { get; set; }
    }
}
