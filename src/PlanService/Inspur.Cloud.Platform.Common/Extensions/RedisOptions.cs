using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Extensions
{
    [Serializable]
    public class RedisOptions
    {
        public bool Enable { get; set; }
        public string Configuration { get; set; }
        public string InstanceName { get; set; }
    }
}
