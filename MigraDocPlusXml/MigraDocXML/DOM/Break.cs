using MigraDocXML.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Break : LogicalElement
    {
        public override void Run(Action childProcessor)
        {
            throw new BreakException();
        }
    }
}
