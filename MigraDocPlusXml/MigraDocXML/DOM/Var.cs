using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Var : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();


        private Dictionary<string, string> _definitions = new Dictionary<string, string>();


        public override void SetUnknownAttribute(string name, object value)
        {
            _definitions[name] = value?.ToString();
        }


        public override void Run(Action childProcessor)
        {
            foreach(var def in _definitions)
            {
                var value = GetDocument().ScriptRunner.Run(def.Value, s => GetParent().GetVariable(s));
                GetParent().NewVariable(def.Key, value);
            }
        }
    }
}
