using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.Exceptions
{
    public class UndefinedVariableException : MigraDocXMLException
    {
        public UndefinedVariableException()
            : base()
        {
        }

        public UndefinedVariableException(string message)
            : base(message)
        {
        }

        public UndefinedVariableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
