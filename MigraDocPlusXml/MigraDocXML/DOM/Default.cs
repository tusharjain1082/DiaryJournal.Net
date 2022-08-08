using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    /// <summary>
    /// Defines a set of variable values, of those, only the variables which aren't already defined get defined with the paired value
    /// </summary>
    public class Default : LogicalElement
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
                if (!GetParent().OwnsVariable(def.Key))
                {
                    var value = GetDocument().ScriptRunner.Run(def.Value, s => GetParent().GetVariable(s));
                    GetParent().NewVariable(def.Key, value);
                }
            }
        }
    }
}
