using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.Exceptions
{
    internal class XmlStackTraceException : MigraDocXMLException
    {
        internal List<string> Stack { get; private set; } = new List<string>();

        public XmlStackTraceException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }


        internal Exception GenerateStackedException()
        {
            StringBuilder stb = new StringBuilder(InnerException.Message + " at: ");
            for(int i = Stack.Count - 1; i >= 0; i--)
            {
                stb.Append(Stack[i]);
                if (i > 0)
                    stb.Append("/");
            }
            Type exType = InnerException.GetType();
            return Activator.CreateInstance(exType, stb.ToString(), InnerException) as Exception;
        }
    }
}
