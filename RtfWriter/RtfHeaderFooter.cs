using System;
using System.Collections;
using System.Text;

namespace Elistia.DotNetRtfWriter
{
    /// <summary>
    /// Summary description for RtfHeaderFooter
    /// </summary>
    public class RtfHeaderFooter : RtfBlockList
    {
        private Hashtable _magicWords;
        private HeaderFooterType _type;

        internal RtfHeaderFooter(HeaderFooterType type)
            : this(type, ReadingDirection.LeftToRight)
        {
        }

        internal RtfHeaderFooter(HeaderFooterType type, ReadingDirection direction)
            : base(true, false, true, true, false)
        {
            _magicWords = new Hashtable();
            _type = type;
            ReadingDirection = direction;
        }

        public override string Render()
        {
            StringBuilder result = new StringBuilder();

            if (_type == HeaderFooterType.Header) {
                result.AppendLine(@"{\header");
            } else if (_type == HeaderFooterType.Footer) {
                result.AppendLine(@"{\footer");
            } else {
                throw new Exception("Invalid HeaderFooterType");
            }
            result.AppendLine();
            for (int i = 0; i < base.Blocks.Count; i++) {
                RtfBlock block = base.Blocks[i];
                if (base.DefaultCharFormat != null && block.DefaultCharFormat != null) {
                    block.DefaultCharFormat.CopyFrom(base.DefaultCharFormat);
                }
                result.AppendLine(block.Render());
            }
            result.AppendLine("}");
            return result.ToString();
        }
    }
}
