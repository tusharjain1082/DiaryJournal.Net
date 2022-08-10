using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.Exceptions
{
    public class QuitException : MigraDocXMLException
    {
        public QuitException()
        {
        }

        public QuitException(string message)
            : base(message)
        {
        }

        public QuitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
