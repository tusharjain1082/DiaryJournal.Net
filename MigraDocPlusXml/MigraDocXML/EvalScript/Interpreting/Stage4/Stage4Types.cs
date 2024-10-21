using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EvalScript.Interpreting.Stage2;

namespace EvalScript.Interpreting.Stage4
{
    public static class Stage4Types
    {
        //Accessors
        public const int FunctionCall = 3001;
        public const int Property = 3002;
        public const int LambdaExpression = 3003;
        public const int IndexerCall = 3004;
        public const int Literal = 3005;
        public const int TernaryOperation = 3006;

        public const int Chain = 3101;

        public const int ArrayDefinition = 3201;
        public const int DictionaryDefinition = 3202;
        public const int KeyValuePair = 3303;

        public const int Formatting = 3401;

        public static bool IsStage4(int type) => type >= 3000 && type < 4000;
    }

    /// <summary>
    /// Base class of all stage 4 types, all evaluable tokens can be run by the evaluator to return a value
    /// </summary>
    public class EvaluableToken : Token
    {
        public EvaluableToken(int type, object value = null) 
            : base(type, value)
        {
        }
    }

    /// <summary>
    /// Represents a literal value in code
    /// </summary>
    public class LiteralToken : EvaluableToken
    {
        public LiteralToken(object value)
            : base(Stage4Types.Literal, value)
        {
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// Represents a binary operation, such as addition, multiplication, equality tests etc.
    /// </summary>
    public class BinaryOperationToken : EvaluableToken
    {
        public EvaluableToken Left { get; private set; }
        
        public EvaluableToken Right { get; private set; }

        public BinaryOperationToken(EvaluableToken left, int type, EvaluableToken right)
            : base(type)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override string ToString()
        {
            return Left.ToString() + base.ToString() + Right.ToString();
        }
    }

    /// <summary>
    /// Represents a ternary operation
    /// </summary>
    public class TernaryOperationToken : EvaluableToken
    {
        public EvaluableToken Test { get; private set; }

        public EvaluableToken TrueResult { get; private set; }

        public EvaluableToken FalseResult { get; private set; }

        public TernaryOperationToken(EvaluableToken test, EvaluableToken trueResult, EvaluableToken falseResult)
            : base(Stage4Types.TernaryOperation)
        {
            Test = test ?? throw new ArgumentNullException(nameof(test));
            TrueResult = trueResult ?? throw new ArgumentNullException(nameof(trueResult));
            FalseResult = falseResult ?? throw new ArgumentNullException(nameof(falseResult));
        }

        public override string ToString()
        {
            return Test.ToString() + " ? " + TrueResult.ToString() + " : " + FalseResult.ToString();
        }
    }

    /// <summary>
    /// Represents a unary operation, such as the not (!) or negative (-) operations
    /// </summary>
    public class UnaryOperationToken : EvaluableToken
    {
        public EvaluableToken Operand { get; private set; }

        public UnaryOperationToken(EvaluableToken operand, int type)
            : base(type)
        {
            Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        }

        public override string ToString()
        {
            return base.ToString() + Operand.ToString();
        }
    }

    /// <summary>
    /// Stores the name and parameters of a function call
    /// </summary>
    public class FunctionCallToken : EvaluableToken
    {
        public string Name { get; private set; }

        public IEnumerable<EvaluableToken> Parameters { get; private set; }

        public FunctionCallToken(string name, IEnumerable<EvaluableToken> parameters)
            : base(Stage4Types.FunctionCall)
        {
            Name = name;
            Parameters = parameters ?? new List<EvaluableToken>();
        }

        public override string ToString()
        {
            return Name + "(" + string.Join("", Parameters.Select(x => x.ToString()).ToArray()) + ")";
        }
    }

    /// <summary>
    /// Stores the name of a property being accessed
    /// </summary>
    public class PropertyToken : EvaluableToken
    {
        public string Name { get; private set; }

        public PropertyToken(string name)
            : base(Stage4Types.Property)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public class FormattingToken : EvaluableToken
    {
        public EvaluableToken Input { get; private set; }

        public string Format { get; private set; }

        public FormattingToken(EvaluableToken input, string format)
            : base(Stage4Types.Formatting)
        {
            Input = input;
            Format = format;
        }

        public override string ToString()
        {
            return Input.ToString() + " #" + Format;
        }
    }

    /// <summary>
    /// Stores the parameters and body of a lambda expression
    /// </summary>
    public class LambdaExpressionToken : EvaluableToken
    {
        public IEnumerable<string> Parameters { get; private set; }

        public EvaluableToken Body { get; private set; }

        public LambdaExpressionToken(IEnumerable<string> parameters, EvaluableToken body)
            : base(Stage4Types.LambdaExpression)
        {
            Parameters = parameters ?? new List<string>();
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public override string ToString()
        {
            string output = null;
            if (Parameters.Count() == 1)
                output = Parameters.First().ToString();
            else
                output = "(" + string.Join(",", Parameters.Select(x => x.ToString()).ToArray()) + ")";
            output += "=>";
            output += Body.ToString();
            return output;
        }
    }

    /// <summary>
    /// Stores the parameter name of an indexer call
    /// </summary>
    public class IndexerCallToken : EvaluableToken
    {
        public EvaluableToken Parameter { get; private set; }

        public IndexerCallToken(EvaluableToken parameter)
            : base(Stage4Types.IndexerCall)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public override string ToString()
        {
            return "[" + Parameter.ToString() + "]";
        }
    }

    /// <summary>
    /// Stores the items of an accessor chain
    /// Example of chains: Model.Product.Type.Name  or  Model.Items.Where(x => x.Quantity LT 10).Order(x => x.Name)
    /// </summary>
    public class ChainToken : EvaluableToken
    {
        public IEnumerable<EvaluableToken> Items { get; private set; }

        public ChainToken(IEnumerable<EvaluableToken> items)
            : base(Stage4Types.Chain)
        {
            Items = items.ToList();
        }

        public override string ToString()
        {
            return string.Join(".", Items.Select(x => x.ToString()).ToArray());
        }
    }

    /// <summary>
    /// Stores the items of an array definition
    /// </summary>
    public class ArrayDefinitionToken : EvaluableToken
    {
        public IEnumerable<EvaluableToken> Items { get; private set; }

        public ArrayDefinitionToken(IEnumerable<EvaluableToken> items)
            : base(Stage4Types.ArrayDefinition)
        {
            Items = items.ToList();
        }

        public override string ToString()
        {
            return "[" + string.Join(",", Items.Select(x => x.ToString()).ToArray()) + "]";
        }
    }

    /// <summary>
    /// Stores an evaluable token against a string key
    /// </summary>
    public class KeyValuePairToken : EvaluableToken
    {
        public string Key { get; private set; }

        public EvaluableToken ValueToken { get; private set; }

        public KeyValuePairToken(string key, EvaluableToken value)
            : base(Stage4Types.KeyValuePair)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ValueToken = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
        {
            return Key + ":" + Value.ToString();
        }
    }

    /// <summary>
    /// Stores the key value pairs of a dictionary definition
    /// </summary>
    public class DictionaryDefinitionToken : EvaluableToken
    {
        public IEnumerable<KeyValuePairToken> Items { get; private set; }

        public DictionaryDefinitionToken(IEnumerable<KeyValuePairToken> items)
            : base(Stage4Types.DictionaryDefinition)
        {
            Items = items.ToList();
        }

        public override string ToString()
        {
            return "[" + string.Join(",", Items.Select(x => x.ToString()).ToArray()) + "]";
        }
    }
}
