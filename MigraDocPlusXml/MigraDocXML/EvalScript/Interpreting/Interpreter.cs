using EvalScript.Interpreting.Stage1;
using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using EvalScript.Interpreting.Stage4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting
{
    /// <summary>
    /// Responsible for converting code from text into an evaluable object graph
    /// </summary>
    public class Interpreter
    {
        private Dictionary<string, EvaluableToken> _cache = new Dictionary<string, EvaluableToken>();

		
		public char StringLiteralChar { get; set; } = '\'';


        public EvaluableToken Run(string code)
        {
            if (_cache.ContainsKey(code))
                return _cache[code];

            var tokens = new List<Token>()
            {
                new Token(Stage1Types.Text, code)
            };

            //Stage 1
            tokens = new StringLiteralSeparator().Run(this, tokens);
            string format = new FormattingIdentifier().Run(this, tokens);
            tokens = new WhitespaceSeparator().Run(this, tokens);
            tokens = new SymbolSeparator().Run(this, tokens);
            
            //Stage 2
            new BooleanLiteralIdentifier().Run(this, tokens);
            new NullLiteralIdentifier().Run(this, tokens);
            new NumericLiteralIdentifier().Run(this, tokens);
            new OperatorIdentifier().Run(this, tokens);
            new WhitespaceCleaner().Run(this, tokens);
            new NameIdentifier().Run(this, tokens);
            new Stage1TypeMapper().Run(this, tokens);
            new NegativeOperatorIdentifier().Run(this, tokens);

            //Stage 3
            new BracketNester(BracketNester.Types.Brackets).Run(this, tokens);
            new BracketNester(BracketNester.Types.SquareBrackets).Run(this, tokens);
            new CommaSeparatedNester().Run(this, tokens);

            new BinaryOperationNester(Stage2Types.PowerOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.DivideOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.MultiplyOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.PlusOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.MinusOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.ModulusOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.NullCoalesceOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.AsOperator).Run(this, tokens);

            new BinaryOperationNester(Stage2Types.EqualOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.NotEqualOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.LessThanOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.LessThanOrEqualOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.GreaterThanOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.GreaterThanOrEqualOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.AndOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.OrOperator).Run(this, tokens);

            new TernaryNester().Run(this, tokens);
            
            new BinaryOperationNester(Stage2Types.LambdaOperator).Run(this, tokens);
            new BinaryOperationNester(Stage2Types.KeyValueOperator).Run(this, tokens);

            //Stage 4
            var output = RecurseStage4(tokens);

            if(format != null)
                output = new FormattingToken(output, format);

            _cache[code] = output;
            return output;
        }
        

        private EvaluableToken RecurseStage4(List<Token> tokens)
        {
            for(int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is ParentToken)
                {
                    var newToken = RecurseStage4((tokens[i] as ParentToken).Children);
                    if (newToken != null && tokens[i].Type != Stage3Types.Brackets && tokens[i].Type != Stage3Types.SquareBrackets)
                        tokens[i] = newToken;
                }
            }

            new PropertyBuilder().Run(this, tokens);
            new IndexerCallBuilder().Run(this, tokens);
            new DictionaryDefinitionBuilder().Run(this, tokens);
            new ArrayDefinitionBuilder().Run(this, tokens);
            new LiteralBuilder().Run(this, tokens);
            new FunctionCallBuilder().Run(this, tokens);
            new BracketsRemover().Run(this, tokens);
            new ChainBuilder().Run(this, tokens);
            new UnaryOperationBuilder().Run(this, tokens);
            new LambdaExpressionBuilder().Run(this, tokens);
            new TernaryOperationBuilder().Run(this, tokens);
            new KeyValuePairBuilder().Run(this, tokens);
            new BinaryOperationBuilder().Run(this, tokens);

            if (tokens.Count == 1)
                return tokens[0] as EvaluableToken;
            return null;
        }
    }
}
