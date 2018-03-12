using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Api
{
    public class Organization
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string ParentID { get; set; }

        public string FullPath { get; set; }
    }
}
