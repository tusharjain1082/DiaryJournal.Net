using MigraDocXML.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class While : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();


        public override void Run(Action childProcessor)
        {
            while (true.Equals(GetDocument().ScriptRunner.Run(Test, s => GetParent().GetVariable(s))))
            {
                try
                {
                    childProcessor();
                }
                catch (ContinueException)
                {
                }
                catch (BreakException)
                {
                    break;
                }
            }
        }



        public string Test { get; set; }
    }
}
