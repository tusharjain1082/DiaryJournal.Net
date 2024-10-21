using MigraDocXML.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Continue : LogicalElement
    {
        public override void Run(Action childProcessor)
        {
            throw new ContinueException();
        }
    }
}
