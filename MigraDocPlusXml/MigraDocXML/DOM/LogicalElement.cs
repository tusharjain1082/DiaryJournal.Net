using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MigraDoc.DocumentObjectModel;

namespace MigraDocXML.DOM
{
    public abstract class LogicalElement : DOMElement
    {
        public LogicalElement()
        {
            IsLogical = true;
            IsPresentable = false;
        }

        public override DocumentObject GetModel() => throw new NotImplementedException();


        public abstract void Run(Action childProcessor);
    }
}
