using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Setters : LogicalElement
    {
        private Dictionary<string, string> _items = new Dictionary<string, string>();

        public Dictionary<string, string> GetItems() => _items;


        public Setters()
        {
            NewVariable("Setters", this);
        }


        public override void SetUnknownAttribute(string name, object value)
        {
            _items[name] = value?.ToString();
        }


        public override void Run(Action childProcessor)
        {
        }
    }
}
