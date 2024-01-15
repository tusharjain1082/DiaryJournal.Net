using System;
using System.Text;

namespace Elistia.DotNetRtfWriter
{
    /// <summary>
    /// Summary description for RtfFootnote
    /// </summary>
    public class RtfFootnote : RtfBlockList
    {
        private int _position;

        internal RtfFootnote(int position, int textLength)
            : this(position, textLength, ReadingDirection.LeftToRight)
        {
        }

        internal RtfFootnote(int position, int textLength, ReadingDirection direction)
            : base(true, false, false, true, false)
        {
            if (position < 0 || position >= textLength) {
                throw new Exception("Invalid footnote position: " + position
                                    + " (text length=" + textLength + ")");
            }
            _position = position;
            ReadingDirection = direction;
        }

        internal int Position
        {
            get {
                return _position;
            }
        }

        public override string Render()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(@"{\super\chftn}");
            result.AppendLine(@"{\footnote\plain\chftn");
            base.Blocks[base.Blocks.Count - 1].BlockTail = "}";
            result.Append(base.Render());
            result.AppendLine("}");
            return result.ToString();
        }
    }
}
