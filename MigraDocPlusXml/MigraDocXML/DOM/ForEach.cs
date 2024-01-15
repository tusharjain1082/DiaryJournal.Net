using MigraDocXML.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class ForEach : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();


        public override void Run(Action childProcessor)
        {
            object itemsObj = GetDocument().ScriptRunner.Run(In, s => GetParent().GetVariable(s));
            if (itemsObj == null)
                return;

            var itemsEnum = itemsObj as IEnumerable;
            NewVariable(Var, null);
            foreach(var item in itemsEnum)
            {
                SetVariable(Var, item);
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



        public string Var { get; set; }

        public string In { get; set; }
    }
}
