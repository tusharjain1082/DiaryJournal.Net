using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.Exceptions
{
    public class BreakException : MigraDocXMLException
    {
        public BreakException()
        {
        }

        public BreakException(string message)
            : base(message)
        {
        }

        public BreakException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
