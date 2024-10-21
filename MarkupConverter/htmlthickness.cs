using System;

namespace MarkupConverter
{
    public class HtmlThickness : IEquatable<HtmlThickness>
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }

        public HtmlThickness(double value)
            : this(value, value, value, value)
        {
        }

        public HtmlThickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public bool Equals(HtmlThickness other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }
    }
}
