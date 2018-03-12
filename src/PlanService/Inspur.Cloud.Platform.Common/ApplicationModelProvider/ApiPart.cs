using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Common.ApplicationModelProvider
{
    internal class ApiPart
    {
        public string ModuleName { get; set; }

        public string Assembly { get; set; }

        public List<ControllerInfo> Controllers { get; set; }
    }
}
