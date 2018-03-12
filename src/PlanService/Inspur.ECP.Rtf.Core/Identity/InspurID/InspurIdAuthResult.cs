using Inspur.ECP.Rtf.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Core.Identity
{
    class InspurIdAuthResult
    {
        /// <summary>
        /// inspurID，唯一标识
        /// </summary>
        public string ID { get; set; }

        public string Code { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public string first_name { get; set; }
        public string last_name { get; set; }

        public Organization Enterprise { get; set; }
    }
}
