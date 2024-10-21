using EvalScript.Interpreting.Stage4;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Evaluating
{
    public interface IEvaluator
    {
        Dictionary<string, Func<object[], object>> Functions { get; }

        object Run(EvaluableToken token, Func<string, object> variableLookup);
    }
}
