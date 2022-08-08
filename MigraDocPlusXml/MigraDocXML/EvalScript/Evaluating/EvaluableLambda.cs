using EvalScript.Interpreting.Stage4;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript.Evaluating
{
    public class EvaluableLambda
    {
        public LambdaExpressionToken Token { get; private set; }

        public IEvaluator Evaluator { get; private set; }

        public Func<string, object> VariableLookup { get; private set; }

        public EvaluableLambda(LambdaExpressionToken token, IEvaluator evaluator, Func<string, object> variableLookup)
        {
            Token = token;
            Evaluator = evaluator;
            VariableLookup = variableLookup;
        }
    }
}
