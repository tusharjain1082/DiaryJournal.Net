using System;
using System.Collections.Generic;
using System.Text;

namespace EvalScript
{
    public class Runner
    {
        public Interpreting.Interpreter Interpreter { get; private set; }

        public Evaluating.IEvaluator Evaluator { get; private set; }

        public Runner(Interpreting.Interpreter interpreter = null, Evaluating.IEvaluator evaluator = null)
        {
            Interpreter = interpreter ?? new Interpreting.Interpreter();
            Evaluator = evaluator ?? new Evaluating.StandardEvaluator();
        }


        public object Run(string code, Func<string, object> variableLookup)
        {
            var token = Interpreter.Run(code);
            return Evaluator.Run(token, variableLookup);
        }
    }
}
