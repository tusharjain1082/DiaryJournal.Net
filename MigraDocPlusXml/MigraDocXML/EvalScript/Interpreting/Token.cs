using EvalScript.Interpreting.Stage1;
using EvalScript.Interpreting.Stage2;
using EvalScript.Interpreting.Stage3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Interpreting
{
    /// <summary>
    /// Defines a code object
    /// </summary>
    public class Token
    {
        public int Type { get; private set; }

        public object Value { get; private set; }

        public Token(int type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            if (this.IsStage1())
                return Stage1Types.ToString(this);
            else if (this.IsStage2())
                return Stage2Types.ToString(this);
            else if (this.IsStage3())
                return Stage3Types.ToString(this);
            else
                throw new InvalidOperationException("Unable to find string representation for type " + Type);
        }
    }


    /// <summary>
    /// Defines a code object that contains other, nested, code objects
    /// </summary>
    public class ParentToken : Token
    {
        public List<Token> Children { get; private set; }

        public ParentToken(int type, params Token[] children)
            : base(type)
        {
            Children = children?.ToList() ?? new List<Token>();
        }

        public ParentToken(int type, IEnumerable<Token> children = null)
            : base(type)
        {
            Children = children?.ToList() ?? new List<Token>();
        }
    }
}
