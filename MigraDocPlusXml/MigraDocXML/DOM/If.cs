using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class If : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();


        private bool? _result = null;
        public bool? GetResult() => _result;


        public override void Run(Action childProcessor)
        {
            var result = GetDocument().ScriptRunner.Run(Test, s => GetParent().GetVariable(s));

            _result = true.Equals(result);

            if (_result == true)
                childProcessor();
        }



        public string Test { get; set; }
    }



    public class ElseIf : If
    {
        public override void Run(Action childProcessor)
        {
            var siblingList = GetParent().Children.ToList();
            var thisIndex = siblingList.IndexOf(this);
            if (thisIndex <= 0)
                throw new Exception("Unable to find previous If/ElseIf element");
            var previousIf = siblingList[thisIndex - 1] as If;
            if (previousIf == null)
                throw new Exception("Unable to find previous If/ElseIf element");
            if (previousIf.GetResult() != false)
                return;

            base.Run(childProcessor);
        }
    }



    public class Else : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();


        public override void Run(Action childProcessor)
        {
            var siblingList = GetParent().Children.ToList();
            var thisIndex = siblingList.IndexOf(this);
            if (thisIndex <= 0)
                throw new Exception("Unable to find previous If/ElseIf element");
            var previousIf = siblingList[thisIndex - 1] as If;
            if (previousIf == null)
                throw new Exception("Unable to find previous If/ElseIf element");
            if (previousIf.GetResult() != false)
                return;

            childProcessor();
        }
    }
}
