using System;
using System.Collections.Generic;
using System.Text;

namespace Inspur.ECP.Rtf.Api
{
    public class RtfException : Exception
    {
        public RtfException() : base()
        { }

        public RtfException(string message) : base(message)
        { }

        public RtfException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
