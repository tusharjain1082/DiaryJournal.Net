using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MigraDocXML.Exceptions
{
    public class MigraDocXMLException : Exception
    {
        public MigraDocXMLException()
            : base()
        {
        }

        public MigraDocXMLException(string message)
            : base(message)
        {
        }

        public MigraDocXMLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MigraDocXMLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
