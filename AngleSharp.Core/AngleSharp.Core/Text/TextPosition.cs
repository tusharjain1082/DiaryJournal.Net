namespace AngleSharp.Text
{
    using System;

    /// <summary>
    /// The position in the source code.
    /// </summary>
    public readonly struct TextPosition : IEquatable<TextPosition>, IComparable<TextPosition>
    {
        #region Fields

        /// <summary>
        /// An empty position (0, 0, 0).
        /// </summary>
        public static readonly TextPosition Empty = new();

        private readonly UInt16 _line;
        private readonly UInt16 _column;
        private readonly Int32 _position;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new text position.
        /// </summary>
        /// <param name="line">The line of the character.</param>
        /// <param name="column">The column of the character.</param>
        /// <param name="position">The index of the character.</param>
        public TextPosition(UInt16 line, UInt16 column, Int32 position)
        {
            _line = line;
            _column = column;
            _position = position;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the line within the document.
        /// The line is 1-based, so the first line has value 1.
        /// </summary>
        public Int32 Line => _line;

        /// <summary>
        /// Gets the column within the document.
        /// The column is 1-based, so the first column has value 1.
        /// </summary>
        public Int32 Column => _column;

        /// <summary>
        /// Gets the position within the source.
        /// The position is 1-based, so the first character is at position 1.
        /// </summary>
        public Int32 Position => _position;

        /// <summary>
        /// Gets the index within the source.
        /// The index is 0-based, so the first character is at index 0.
        /// </summary>
        public Int32 Index => _position - 1;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a new text position that includes the given offset.
        /// </summary>
        /// <param name="columns">The number of columns to shift.</param>
        /// <returns>The new text position.</returns>
        public TextPosition Shift(Int32 columns) => new(_line, (UInt16)(_column + columns), _position + columns);

        /// <summary>
        /// Returns a new text position that is after the given character.
        /// </summary>
        /// <param name="chr">The character to analyze.</param>
        /// <returns>The new text position.</returns>
        public TextPosition After(Char chr)
        {
            var line = _line;
            var column = _column;

            if (chr == Symbols.LineFeed)
            {
                ++line;
                column = 0;
            }

            return new TextPosition(line, ++column, _position + 1);
        }

        /// <summary>
        /// Returns a new text position that is after the given string.
        /// </summary>
        /// <param name="str">The string to analyze.</param>
        /// <returns>The new text position.</returns>
        public TextPosition After(String str)
        {
            var line = _line;
            var column = _column;

            foreach (var chr in str)
            {
                if (chr == Symbols.LineFeed)
                {
                    ++line;
                    column = 0;
                }

                ++column;
            }

            return new TextPosition(line, column, _position + str.Length);
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Returns a string representation of the position in the text.
        /// </summary>
        /// <returns>
        /// A string that contains the contained line and column.
        /// </returns>
        public override String ToString() => $"Ln {_line}, Col {_column}, Pos {_position}";

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// An integer that is the hash code for this instance.
        /// </returns>
        public override Int32 GetHashCode() => _position ^ (_line | _column) + _line;

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current instance.
        /// </param>
        /// <returns>
        /// True if the given object is a text position with the same values,
        /// otherwise false.
        /// </returns>
        public override Boolean Equals(Object? obj) => obj is TextPosition other && Equals(other);

        /// <summary>
        /// Indicates whether the current position is equal to the given
        /// position.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// True if the given position has the same values, otherwise false.
        /// </returns>
        public Boolean Equals(TextPosition other) => _position == other._position &&
            _column == other._column &&
            _line == other._line;

        /// <summary>
        /// Compares the two positions by their absolute positions in the text
        /// source.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>
        /// True if the position of the first operand is greater than the
        /// second operand.
        /// </returns>
        public static Boolean operator >(TextPosition a, TextPosition b) => a._position > b._position;

        /// <summary>
        /// Compares the two positions by their absolute positions in the text
        /// source.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>
        /// True if the position of the first operand is less than the second
        /// operand.
        /// </returns>
        public static Boolean operator <(TextPosition a, TextPosition b) => a._position < b._position;

        /// <summary>
        /// Compares the current position with another position.
        /// </summary>
        /// <param name="other">The position to compare to.</param>
        /// <returns>
        /// A mathematical representation of the relation (1 = greater, -1 =
        /// less, 0 = equal).
        /// </returns>
        public Int32 CompareTo(TextPosition other) => Equals(other) ? 0 : (this > other ? 1 : -1);

        #endregion
    }
}
