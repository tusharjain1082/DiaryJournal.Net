using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.Exceptions
{
    public class ContinueException : MigraDocXMLException
    {
        public ContinueException()
        {
        }

        public ContinueException(string message)
            : base(message)
        {
        }

        public ContinueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
