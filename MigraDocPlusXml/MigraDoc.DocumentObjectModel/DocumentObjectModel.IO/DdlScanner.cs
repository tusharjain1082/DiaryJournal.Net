#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

/*
  ddl = <document> | <empty>
  
  table-element:
    \table �attributes�opt { �columns-element� �rows-element� }

  table-element:
    \table �attributes�opt { �columns-element� �rows-element� }
*/

namespace MigraDoc.DocumentObjectModel.IO
{
    /// <summary>
    /// DdlScanner
    /// </summary>
    internal class DdlScanner
    {
        /// <summary>
        /// Initializes a new instance of the DdlScanner class.
        /// </summary>
        internal DdlScanner(string documentFileName, string ddl, DdlReaderErrors errors)
        {
            _errors = errors;
            Init(ddl, documentFileName);
        }

        /// <summary>
        /// Initializes a new instance of the DdlScanner class.
        /// </summary>
        internal DdlScanner(string ddl, DdlReaderErrors errors)
            : this("", ddl, errors)
        { }

        /// <summary>
        /// Initializes all members and prepares the scanner.
        /// </summary>
        internal bool Init(string document, string documentFileName)
        {
            _documentPath = documentFileName;
            _strDocument = document;
            _ddlLength = _strDocument.Length;
            _idx = 0;
            _idxLine = 1;
            _idxLinePos = 0;

            _documentFileName = documentFileName;

            _nCurDocumentIndex = _idx;
            _nCurDocumentLine = _idxLine;
            _nCurDocumentLinePos = _idxLinePos;

            ScanNextChar();

            return true;
        }

        /// <summary>
        /// Reads to the next DDL token. Comments are ignored.
        /// </summary>
        /// <returns>
        /// Returns the current symbol.
        /// It is Symbol.Eof if the end of the DDL string is reached.
        /// </returns>
        internal Symbol ReadCode()
        {
        Again:
            _symbol = Symbol.None;
            _tokenType = TokenType.None;
            _token = "";

            MoveToNonWhiteSpace();
            SaveCurDocumentPos();

            if (_currChar == Chars.Null)
            {
                _symbol = Symbol.Eof;
                return Symbol.Eof;
            }

            if (IsIdentifierChar(_currChar, true))
            {
                // Token is identifier.
                _symbol = ScanIdentifier();
                _tokenType = TokenType.Identifier;
                // Some keywords do not start with a backslash: true, false, and null.
                Symbol sym = KeyWords.SymbolFromName(_token);
                if (sym != Symbol.None)
                {
                    _symbol = sym;
                    _tokenType = TokenType.KeyWord;
                }
            }
            else if (_currChar == '"')
            {
                // Token is string literal.
                _token += ScanStringLiteral();
                _symbol = Symbol.StringLiteral;
                _tokenType = TokenType.StringLiteral;
            }
            //NYI: else if (IsNumber())
            //      symbol = ScanNumber(false);
            else if (IsDigit(_currChar) ||
                     _currChar == '-' && IsDigit(_nextChar) ||
                     _currChar == '+' && IsDigit(_nextChar))
            {
                // Token is number literal.
                _symbol = ScanNumber(false);
                _tokenType = _symbol == Symbol.RealLiteral ? TokenType.RealLiteral : TokenType.IntegerLiteral;
            }
            else if (_currChar == '.' && IsDigit(_nextChar))
            {
                // Token is real literal.
                _symbol = ScanNumber(true);
                _tokenType = TokenType.RealLiteral;
            }
            else if (_currChar == '\\')
            {
                // Token is keyword.
                _token = "\\";
                _symbol = ScanKeyword();
                _tokenType = _symbol != Symbol.None ? TokenType.KeyWord : TokenType.None;
            }
            else if (_currChar == '/' && _nextChar == '/')
            {
                // Token is comment. In code comments are ignored.
                ScanSingleLineComment();
                goto Again;
            }
            else if (_currChar == '@' && _nextChar == '"')
            {
                // Token is verbatim string literal.
                ScanNextChar();
                _token += ScanVerbatimStringLiteral();
                _symbol = Symbol.StringLiteral;
                _tokenType = _symbol != Symbol.None ? TokenType.StringLiteral : TokenType.None;
            }
            else
            {
                // Punctuator or syntax error.
                _symbol = ScanPunctuator();
            }
            return _symbol;
        }

        /// <summary>
        /// Gets the next keyword at the current position without touching the DDL cursor.
        /// </summary>
        internal Symbol PeekKeyword()
        {
            Debug.Assert(_currChar == Chars.BackSlash);

            return PeekKeyword(_idx);
        }

        /// <summary>
        /// Gets the next keyword without touching the DDL cursor.
        /// </summary>
        internal Symbol PeekKeyword(int index)
        {
            // Check special keywords
            switch (_strDocument[index])
            {
                case '{':
                case '}':
                case '\\':
                case '-':
                case '(':
                    return Symbol.Character;
            }

            string token = "\\";
            int idx = index;
            int length = _ddlLength - idx;
            while (length > 0)
            {
                char ch = _strDocument[idx++];
                if (IsLetter(ch))
                {
                    token += ch;
                    length--;
                }
                else
                    break;
            }
            return KeyWords.SymbolFromName(token);
        }

        /// <summary>
        /// Gets the next punctuator terminal symbol without touching the DDL cursor.
        /// </summary>
        protected Symbol PeekPunctuator(int index)
        {
            Symbol sym = Symbol.None;
            char ch = _strDocument[index];
            switch (ch)
            {
                case '{':
                    sym = Symbol.BraceLeft;
                    break;

                case '}':
                    sym = Symbol.BraceRight;
                    break;

                case '[':
                    sym = Symbol.BracketLeft;
                    break;

                case ']':
                    sym = Symbol.BracketRight;
                    break;

                case '(':
                    sym = Symbol.ParenLeft;
                    break;

                case ')':
                    sym = Symbol.ParenRight;
                    break;

                case ':':
                    sym = Symbol.Colon;
                    break;

                case ';':
                    sym = Symbol.Semicolon;
                    break;

                case '.':
                    sym = Symbol.Dot;
                    break;

                case ',':
                    sym = Symbol.Comma;
                    break;

                case '%':
                    sym = Symbol.Percent;
                    break;

                case '$':
                    sym = Symbol.Dollar;
                    break;

                case '@':
                    sym = Symbol.At;
                    break;

                case '#':
                    sym = Symbol.Hash;
                    break;

                //case '?':
                //  sym = Symbol.Question;
                //  break;

                case '�':
                    sym = Symbol.Currency; //??? used in DDL?
                    break;

                //case '|':
                //  sym = Symbol.Bar;
                //  break;

                case '=':
                    sym = Symbol.Assign;
                    break;

                case '/':
                    sym = Symbol.Slash;
                    break;

                case '\\':
                    sym = Symbol.BackSlash;
                    break;

                case '+':
                    if (_ddlLength >= index + 1 && _strDocument[index + 1] == '=')
                        sym = Symbol.PlusAssign;
                    else
                        sym = Symbol.Plus;
                    break;

                case '-':
                    if (_ddlLength >= index + 1 && _strDocument[index + 1] == '=')
                        sym = Symbol.MinusAssign;
                    else
                        sym = Symbol.Minus;
                    break;

                case Chars.CR:
                    sym = Symbol.CR;
                    break;

                case Chars.LF:
                    sym = Symbol.LF;
                    break;

                case Chars.Space:
                    sym = Symbol.Blank;
                    break;

                case Chars.Null:
                    sym = Symbol.Eof;
                    break;
            }
            return sym;
        }

        /// <summary>
        /// Gets the next symbol without touching the DDL cursor.
        /// </summary>
        internal Symbol PeekSymbol()
        {
            int idx = _idx - 1;
            int length = _ddlLength - idx;

            // Move to first non whitespace
            char ch = char.MinValue;
            while (length > 0)
            {
                ch = _strDocument[idx++];
                if (!IsWhiteSpace(ch))
                    break;
                length--;
            }

            if (IsLetter(ch))
                return Symbol.Text;
            if (ch == '\\')
                return PeekKeyword(idx);
            return PeekPunctuator(idx - 1);
        }

        /// <summary>
        /// Reads either text or \keyword from current position.
        /// </summary>
        internal Symbol ReadText(bool rootLevel)
        {
            // Previous call encountered an empty line.
            if (_emptyLine)
            {
                _emptyLine = false;
                _symbol = Symbol.EmptyLine;
                _tokenType = TokenType.None;
                _token = "";
                return Symbol.EmptyLine;
            }

            // Init for scanning.
            _prevSymbol = _symbol;
            _symbol = Symbol.None;
            _tokenType = TokenType.None;
            _token = "";

            // Save where we are
            SaveCurDocumentPos();

            // Check for EOF.
            if (_currChar == Chars.Null)
            {
                _symbol = Symbol.Eof;
                return Symbol.Eof;
            }

            // Check for keyword or escaped character.
            if (_currChar == '\\')
            {
                switch (_nextChar)
                {
                    case '\\':
                    case '{':
                    case '}':
                    case '/':
                    case '-':
                        return ReadPlainText(rootLevel);
                }
                // Either key word or syntax error.
                _token = "\\";
                return ScanKeyword();
            }

            // Check for reserved terminal symbols in text.
            switch (_currChar)
            {
                case '{':
                    AppendAndScanNextChar();
                    _symbol = Symbol.BraceLeft;
                    _tokenType = TokenType.OperatorOrPunctuator;
                    return Symbol.BraceLeft;  // Syntax error in any case.

                case '}':
                    AppendAndScanNextChar();
                    _symbol = Symbol.BraceRight;
                    _tokenType = TokenType.OperatorOrPunctuator;
                    return Symbol.BraceRight;
            }

            // Check for end of line.
            if (_currChar == Chars.LF)
            {
                // The line ends here. See if the paragraph continues in the next line.
                if (MoveToNextParagraphContentLine(rootLevel))
                {
                    // Paragraph continues in next line. Simulate the read of a blank to separate words.
                    _token = " ";
                    if (IgnoreLineBreak())
                        _token = "";
                    _symbol = Symbol.Text;
                    return Symbol.Text;
                }
                else
                {
                    // Paragraph ends here. Return NewLine or BraceRight.
                    if (_currChar != Chars.BraceRight)
                    {
                        _symbol = Symbol.EmptyLine;
                        _tokenType = TokenType.None; //???
                        return Symbol.EmptyLine;
                    }
                    else
                    {
                        AppendAndScanNextChar();
                        _symbol = Symbol.BraceRight;
                        _tokenType = TokenType.OperatorOrPunctuator;
                        return Symbol.BraceRight;
                    }
                }
            }
            return ReadPlainText(rootLevel);
        }

        /// <summary>
        /// Returns whether the linebreak should be ignored, because the previous symbol is already a whitespace.
        /// </summary>
        bool IgnoreLineBreak()
        {
            switch (_prevSymbol)
            {
                case Symbol.LineBreak:
                case Symbol.Space:
                case Symbol.Tab:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Read text from current position until block ends or \keyword occurs.
        /// </summary>
        Symbol ReadPlainText(bool rootLevel)
        {
            bool foundSpace = false;
            bool loop = true;
            while (loop && _currChar != Chars.Null)
            {
                // Check for escaped character or keyword.
                if (_currChar == '\\')
                {
                    switch (_nextChar)
                    {
                        case '\\':
                        case '{':
                        case '}':
                        case '/':
                            ScanNextChar();
                            AppendAndScanNextChar();
                            break;

                        case '-':
                            // Treat \- as soft hyphen.
                            ScanNextChar();
                            // Fake soft hyphen and go on as usual.
                            _currChar = Chars.SoftHyphen;
                            break;

                        // Keyword
                        default:
                            loop = false;
                            break;
                    }
                    continue;
                }

                // Check for reserved terminal symbols in text
                switch (_currChar)
                {
                    case '{':
                        // Syntax error any way
                        loop = false;
                        continue;

                    case '}':
                        // Block end
                        loop = false;
                        continue;

                    case '/':
                        if (_nextChar != '/')
                            goto ValidCharacter;
                        ScanToEol();
                        break;
                }

                // Check for end of line.
                if (_currChar == Chars.LF)
                {
                    // The line ends here. See if the paragraph continues in the next line.
                    if (MoveToNextParagraphContentLine(rootLevel))
                    {
                        // Paragraph continues in next line. Add a blank to separate words.
                        if (!_token.EndsWith(" "))
                            _token += ' ';
                        continue;
                    }
                    else
                    {
                        // Paragraph ends here. Remember that for next call except the reason
                        // for end is '}'
                        _emptyLine = _currChar != Chars.BraceRight;
                        break;
                    }
                }

            ValidCharacter:
                // Compress multiple blanks to one
                if (_currChar == ' ')
                {
                    if (foundSpace)
                    {
                        ScanNextChar();
                        continue;
                    }
                    foundSpace = true;
                }
                else
                    foundSpace = false;

                AppendAndScanNextChar();
            }

            _symbol = Symbol.Text;
            _tokenType = TokenType.Text;
            return Symbol.Text;
        }

        /// <summary>
        /// Moves to the next DDL token if Symbol is not set to a valid position.
        /// </summary>
        internal Symbol MoveToCode()
        {
            if (_symbol == Symbol.None || _symbol == Symbol.CR /*|| this .symbol == Symbol.comment*/)
                ReadCode();
            return _symbol;
        }

        /// <summary>
        /// Moves to the first character the content of a paragraph starts with. Empty lines
        /// and comments are skipped. Returns true if such a character exists, and false if the
        /// paragraph ends without content.
        /// </summary>
        internal bool MoveToParagraphContent()
        {
        Again:
            MoveToNonWhiteSpace();
            if (_currChar == Chars.Slash && _nextChar == Chars.Slash)
            {
                MoveBeyondEol();
                goto Again;
            }
            return _currChar != Chars.BraceRight;
        }

        /// <summary>
        /// Moves to the first character of the content of a paragraph beyond an EOL. 
        /// Returns true if such a character exists and belongs to the current paragraph.
        /// Returns false if a new line (at root level) or '}' occurs. If a new line caused
        /// the end of the paragraph, the DDL cursor is moved to the next valid content
        /// character or '}' respectively.
        /// </summary>
        internal bool MoveToNextParagraphContentLine(bool rootLevel)
        {
            Debug.Assert(_currChar == Chars.LF);
            bool loop = true;
            ScanNextChar();
            while (loop)
            {
                // Scan to next EOL and ignore any white space.
                MoveToNonWhiteSpaceOrEol();
                switch (_currChar)
                {
                    case Chars.Null:
                        loop = false;
                        break;

                    case Chars.LF:
                        ScanNextChar(); // read beyond EOL
                        if (rootLevel)
                        {
                            // At nesting level 0 (root level) a new line ends the paragraph content.
                            // Move to next content block or '}' respectively.
                            MoveToParagraphContent();
                            return false;
                        }
                        else
                        {
                            // Skip new lines at the end of the paragraph.
                            if (PeekSymbol() == Symbol.BraceRight)
                            {
                                MoveToNonWhiteSpace();
                                return false;
                            }

                            //TODO NiSc
                            //NYI
                            //Check.NotImplemented("empty line at non-root level");
                        }
                        break;

                    case Chars.Slash:
                        if (_nextChar == Chars.Slash)
                        {
                            // A line with comment is not treated as empty.
                            // Skip this line.
                            MoveBeyondEol();
                        }
                        else
                        {
                            // Current character is a slash.
                            return true;
                        }
                        break;

                    case Chars.BraceRight:
                        return false;

                    default:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If the current character is not a white space, the function immediately returns it.
        /// Otherwise the DDL cursor is moved forward to the first non-white space or EOF.
        /// White spaces are SPACE, HT, VT, CR, and LF.???
        /// </summary>
        internal char MoveToNonWhiteSpaceOrEol()
        {
            while (_currChar != Chars.Null)
            {
                switch (_currChar)
                {
                    case Chars.Space:
                    case Chars.HT:
                    case Chars.VT:
                        ScanNextChar();
                        break;

                    default:
                        return _currChar;
                }
            }
            return _currChar;
        }

        /// <summary>
        /// If the current character is not a white space, the function immediately returns it.
        /// Otherwise the DDL cursor is moved forward to the first non-white space or EOF.
        /// White spaces are SPACE, HT, VT, CR, and LF.
        /// </summary>
        internal char MoveToNonWhiteSpace()
        {
            while (_currChar != Chars.Null)
            {
                switch (_currChar)
                {
                    case Chars.Space:
                    case Chars.HT:
                    case Chars.VT:
                    case Chars.CR:
                    case Chars.LF:
                        ScanNextChar();
                        break;

                    default:
                        return _currChar;
                }
            }
            return _currChar;
        }

        /// <summary>
        /// Moves to the first character beyond the next EOL. 
        /// </summary>
        internal void MoveBeyondEol()
        {
            // Similar to ScanSingleLineComment but do not scan the token.
            ScanNextChar();
            while (_currChar != Chars.Null && _currChar != Chars.LF)
                ScanNextChar();
            ScanNextChar(); // read beyond EOL
        }

        /// <summary>
        /// Reads a single line comment.
        /// </summary>
        internal Symbol ScanSingleLineComment()
        {
            char ch = ScanNextChar();
            while (ch != Chars.Null && ch != Chars.LF)
            {
                _token += _currChar;
                ch = ScanNextChar();
            }
            ScanNextChar(); // read beyond EOL
            return Symbol.Comment;
        }


        /// <summary>
        /// Gets the current symbol.
        /// </summary>
        internal Symbol Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// Gets the current token type.
        /// </summary>
        internal TokenType TokenType
        {
            get { return _tokenType; }
        }

        /// <summary>
        /// Gets the current token.
        /// </summary>
        internal string Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Interpret current token as integer literal.
        /// </summary>
        /// <returns></returns>
        internal int GetTokenValueAsInt()
        {
            if (_symbol == Symbol.IntegerLiteral)
                return Int32.Parse(_token, CultureInfo.InvariantCulture);

            if (_symbol == Symbol.HexIntegerLiteral)
            {
                string number = _token.Substring(2);
                return Int32.Parse(number, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            //TODO NiSc
            //Check.Assert(false);
            return 0;
        }

        /// <summary>
        /// Interpret current token as unsigned integer literal.
        /// </summary>
        /// <returns></returns>
        internal uint GetTokenValueAsUInt()
        {
            if (_symbol == Symbol.IntegerLiteral)
                return UInt32.Parse(_token, CultureInfo.InvariantCulture);

            if (_symbol == Symbol.HexIntegerLiteral)
            {
                string number = _token.Substring(2);
                return UInt32.Parse(number, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            //TODO NiSc
            //Check.Assert(false);
            return 0;
        }

        /// <summary>
        /// Interpret current token as real literal.
        /// </summary>
        /// <returns></returns>
        internal double GetTokenValueAsReal()
        {
            return Double.Parse(_token, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the current character or EOF.
        /// </summary>
        internal char Char
        {
            get { return _currChar; }
        }

        /// <summary>
        /// Gets the character after the current character or EOF.
        /// </summary>
        internal char NextChar
        {
            get { return _nextChar; }
        }

        /// <summary>
        /// Move DDL cursor one character further.
        /// </summary>
        internal char ScanNextChar()
        {
            if (_ddlLength <= _idx)
            {
                _currChar = Chars.Null;
                _nextChar = Chars.Null;
            }
            else
            {
            SkipChar:
                _currChar = _strDocument[_idx++];
                _nextChar = _ddlLength <= _idx ? Chars.Null : _strDocument[_idx];

                ++_idxLinePos;
                switch (_currChar)
                {
                    case Chars.Null:  //???
                        ++_idxLine;
                        _idxLinePos = 0;
                        break;

                    // ignore CR
                    case Chars.CR:
                        if (_nextChar == Chars.LF)
                        {
                            goto SkipChar;
                        }
                        //else
                        //{
                        //    //TODO NiSc
                        //    //NYI: MacOS uses CR only
                        //    //Check.NotImplemented();
                        //}
                        break;

                    case Chars.LF:
                        //NYI: Unix uses LF only
                        _idxLine++;
                        _idxLinePos = 0;
                        break;
                }
            }
            return _currChar;
        }

        /// <summary>
        /// Move DDL cursor to the next EOL (or EOF).
        /// </summary>
        internal void ScanToEol()
        {
            while (!IsEof(_currChar) && _currChar != Chars.LF)
                ScanNextChar();
        }

        /// <summary>
        /// Appends current character to the token and reads next character.
        /// </summary>
        internal char AppendAndScanNextChar()
        {
            _token += _currChar;
            return ScanNextChar();
        }

        /// <summary>
        /// Appends all next characters to current token until end of line or end of file is reached.
        /// CR/LF or EOF is not part of the token.
        /// </summary>
        internal void AppendAndScanToEol()
        {
            char ch = ScanNextChar();
            while (ch != Chars.Null && ch != Chars.CR && ch != Chars.LF)  //BUG Chars.Null == CharLF
            {
                _token += _currChar;
                ch = ScanNextChar();
            }
        }

        /// <summary>
        /// Is character in '0' ... '9'.
        /// </summary>
        internal static bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        /// <summary>
        /// Is character a hexadecimal digit.
        /// </summary>
        internal static bool IsHexDigit(char ch)
        {
            return Char.IsDigit(ch) || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f');
        }

        /// <summary>
        /// Is character an octal digit.
        /// </summary>
        internal static bool IsOctDigit(char ch)
        {
            return Char.IsDigit(ch) && ch < '8';
        }

        /// <summary>
        /// Is character an alphabetic letter.
        /// </summary>
        internal static bool IsLetter(char ch)
        {
            return Char.IsLetter(ch);
        }

        /// <summary>
        /// Is character a white space.
        /// </summary>
        internal static bool IsWhiteSpace(char ch)
        {
            return Char.IsWhiteSpace(ch);
        }

        /// <summary>
        /// Is character an identifier character. First character can be letter or underscore, following
        /// letters, digits or underscores.
        /// </summary>
        internal static bool IsIdentifierChar(char ch, bool firstChar) //IsId..Char
        {
            if (firstChar)
                return Char.IsLetter(ch) | ch == '_';

            return Char.IsLetterOrDigit(ch) | ch == '_';
        }

        /// <summary>
        /// Is character the end of file character.
        /// </summary>
        internal static bool IsEof(char ch)
        {
            return ch == Chars.Null;
        }

        //internal bool IsNumber();
        //internal bool IsFormat();
        //internal bool IsParagraphFormat(Symbol* _docSym /*= null*/);
        //internal bool IsField();
        //internal bool IsFieldSpecifier();
        //internal bool IsSymbol();
        ////bool IsSymbolSpecifier();
        //internal bool IsFootnote();
        //internal bool IsComment();
        //internal bool IsInlineShape();
        //
        //internal bool IsValueSymbole();
        //internal bool IsScriptSymbole(Symbol _docSym);
        //internal bool IsParagraphToken();
        //internal bool IsExtendedParagraphToken();
        //internal bool IsParagraphElement();
        //internal bool IsHardHyphen();
        //internal bool IsNewLine();
        //internal bool IsWhiteSpace(Symbol _docSym);

        /// <summary>
        /// Determines whether the given symbol is a valid keyword for a document element.
        /// </summary>
        internal static bool IsDocumentElement(Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.Paragraph:
                case Symbol.Table:
                case Symbol.Image:
                case Symbol.TextFrame:
                case Symbol.Chart:
                case Symbol.PageBreak:
                case Symbol.Barcode:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given symbol is a valid keyword for a section element.
        /// </summary>
        internal static bool IsSectionElement(Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.Paragraph:
                case Symbol.Table:
                case Symbol.Image:
                case Symbol.TextFrame:
                case Symbol.Chart:
                case Symbol.PageBreak:
                case Symbol.Barcode:
                case Symbol.Header:
                case Symbol.PrimaryHeader:
                case Symbol.FirstPageHeader:
                case Symbol.EvenPageHeader:
                case Symbol.Footer:
                case Symbol.PrimaryFooter:
                case Symbol.FirstPageFooter:
                case Symbol.EvenPageFooter:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given symbol is a valid keyword for a paragraph element.
        /// </summary>
        internal static bool IsParagraphElement(Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.Blank:
                case Symbol.Bold:
                case Symbol.Italic:
                case Symbol.Underline:
                case Symbol.Font:
                case Symbol.FontColor:
                case Symbol.FontSize:
                case Symbol.Field:
                case Symbol.Hyperlink:
                case Symbol.Footnote:
                case Symbol.Image:
                case Symbol.Tab:
                case Symbol.SoftHyphen:
                case Symbol.Space:
                case Symbol.Symbol:
                case Symbol.Chr:
                case Symbol.LineBreak:
                case Symbol.Text:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given symbol is a valid keyword for a header or footer element.
        /// </summary>
        internal static bool IsHeaderFooterElement(Symbol symbol)
        {
            // All paragraph elements.
            if (IsParagraphElement(symbol))
                return true;

            // All document elements except pagebreak.
            if (IsDocumentElement(symbol))
            {
                if (symbol == Symbol.PageBreak)
                    return false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the given symbol is a valid keyword for a footnote element.
        /// </summary>
        internal static bool IsFootnoteElement(Symbol symbol)
        {
            // All paragraph elements except footnote.
            if (IsParagraphElement(symbol))
            {
                if (symbol == Symbol.Footnote)
                    return false;  // BUG: ??? RETURN TRUE
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current filename of the document.
        /// </summary>
        internal string DocumentFileName
        {
            get { return _documentFileName; }
        }

        /// <summary>
        /// Gets the current path of the document.
        /// </summary>
        internal string DocumentPath
        {
            get { return _documentPath; }
        }

        /// <summary>
        /// Gets the current scanner line in the document.
        /// </summary>
        internal int CurrentLine
        {
            get { return _nCurDocumentLine; }
        }

        /// <summary>
        /// Gets the current scanner column in the document.
        /// </summary>
        internal int CurrentLinePos
        {
            get { return _nCurDocumentLinePos; }
        }

        /// <summary>
        /// Scans an identifier.
        /// </summary>
        protected Symbol ScanIdentifier()
        {
            char ch = AppendAndScanNextChar();
            while (IsIdentifierChar(ch, false))
                ch = AppendAndScanNextChar();

            return Symbol.Identifier;
        }

        /// <summary>
        /// Scans an integer or real literal.
        /// </summary>
        protected Symbol ScanNumber(bool mantissa)
        {
            char ch = _currChar;
            _token += _currChar;

            ScanNextChar();
            if (!mantissa && ch == '0' && (_currChar == 'x' || _currChar == 'X'))
                return ReadHexNumber();

            while (_currChar != Chars.Null)
            {
                if (IsDigit(_currChar))
                    AppendAndScanNextChar();
                else if (!mantissa && _currChar == Chars.Period)
                {
                    //token += currChar;
                    return ScanNumber(true);
                }
                else //if (!IsIdentifierChar(currChar))
                    break;
                //else
                //  THROW_COMPILER_ERROR (COMPERR_LEX_NUMBER);
            }
            return mantissa ? Symbol.RealLiteral : Symbol.IntegerLiteral;
        }

        /// <summary>
        /// Scans an hexadecimal literal.
        /// </summary>
        protected Symbol ReadHexNumber()
        {
            _token = "0x";
            ScanNextChar();
            while (_currChar != Chars.Null)
            {
                if (IsHexDigit(_currChar))
                    AppendAndScanNextChar();
                else if (!IsIdentifierChar(_currChar, false)) //???
                    break;
                else
                    //THROW_COMPILER_ERROR (COMPERR_LEX_NUMBER);
                    AppendAndScanNextChar();
            }
            return Symbol.HexIntegerLiteral;
        }

        /// <summary>
        /// Scans a DDL keyword that starts with a backslash.
        /// </summary>
        Symbol ScanKeyword()
        {
            char ch = ScanNextChar();

            // \- is a soft hyphen == char(173).
            if (ch == '-')
            {
                _token += "-";
                ScanNextChar();
                return Symbol.SoftHyphen;
            }

            // \( is a short cut for symbol.
            if (ch == '(')
            {
                _token += "(";
                _symbol = Symbol.Chr;
                return Symbol.Chr; // Short cut for \chr(
            }

            while (!IsEof(ch) && IsIdentifierChar(ch, false))
                ch = AppendAndScanNextChar();

            _symbol = KeyWords.SymbolFromName(_token);
            return _symbol;
        }

        /// <summary>
        /// Scans punctuator terminal symbols.
        /// </summary>
        protected Symbol ScanPunctuator()
        {
            Symbol sym = Symbol.None;
            switch (_currChar)
            {
                case '{':
                    sym = Symbol.BraceLeft;
                    break;

                case '}':
                    sym = Symbol.BraceRight;
                    break;

                case '[':
                    sym = Symbol.BracketLeft;
                    break;

                case ']':
                    sym = Symbol.BracketRight;
                    break;

                case '(':
                    sym = Symbol.ParenLeft;
                    break;

                case ')':
                    sym = Symbol.ParenRight;
                    break;

                case ':':
                    sym = Symbol.Colon;
                    break;

                case ';':
                    sym = Symbol.Semicolon;
                    break;

                case '.':
                    sym = Symbol.Dot;
                    break;

                case ',':
                    sym = Symbol.Comma;
                    break;

                case '%':
                    sym = Symbol.Percent;
                    break;

                case '$':
                    sym = Symbol.Dollar;
                    break;

                case '@':
                    sym = Symbol.At;
                    break;

                case '#':
                    sym = Symbol.Hash;
                    break;

                //case '?':
                //  sym = Symbol.Question;
                //  break;

                case '�':
                    sym = Symbol.Currency; //??? used in DDL?
                    break;

                //case '|':
                //  sym = Symbol.Bar;
                //  break;

                case '=':
                    sym = Symbol.Assign;
                    break;

                case '/':
                    sym = Symbol.Slash;
                    break;

                case '\\':
                    sym = Symbol.BackSlash;
                    break;

                case '+':
                    if (_nextChar == '=')
                    {
                        _token += _currChar;
                        ScanNextChar();
                        sym = Symbol.PlusAssign;
                    }
                    else
                        sym = Symbol.Plus;
                    break;

                case '-':
                    if (_nextChar == '=')
                    {
                        _token += _currChar;
                        ScanNextChar();
                        sym = Symbol.MinusAssign;
                    }
                    else
                        sym = Symbol.Minus;
                    break;

                case Chars.CR:
                    sym = Symbol.CR;
                    break;

                case Chars.LF:
                    sym = Symbol.LF;
                    break;

                case Chars.Space:
                    sym = Symbol.Blank;
                    break;

                case Chars.Null:
                    sym = Symbol.Eof;
                    return sym;
            }
            _token += _currChar;
            ScanNextChar();
            return sym;
        }

        //    protected Symbol ReadValueIdentifier();
        ///// <summary>
        ///// Scans string literals used as identifiers.
        ///// </summary>
        ///// <returns></returns>
        //protected string ReadRawString()  //ScanStringLiteralIdentifier
        //{
        //  string str = "";
        //  char ch = ScanNextChar();
        //  while (!IsEof(ch))
        //  {
        //    if (ch == Chars.QuoteDbl)
        //    {
        //      if (nextChar == Chars.QuoteDbl)
        //      {
        //        str += ch;
        //        ch = ScanNextChar();
        //      }
        //      else
        //        break;
        //    }
        //
        //    str += ch;
        //    ch = ScanNextChar();
        //  }
        //
        //  ScanNextChar();
        //  return str;
        //}


        /// <summary>
        /// Scans verbatim strings like �@"String with ""quoted"" text"�.
        /// </summary>
        protected string ScanVerbatimStringLiteral()
        {
            string str = "";
            char ch = ScanNextChar();
            while (!IsEof(ch))
            {
                if (ch == Chars.QuoteDbl)
                {
                    if (_nextChar == Chars.QuoteDbl)
                        ch = ScanNextChar();
                    else
                        break;
                }

                str += ch;
                ch = ScanNextChar();
            }

            ScanNextChar();
            return str;
        }

        /// <summary>
        /// Scans regular string literals like �"String with \"escaped\" text"�.
        /// </summary>
        protected string ScanStringLiteral()
        {
            Debug.Assert(Char == '\"');
            StringBuilder str = new StringBuilder();
            ScanNextChar();
            while (_currChar != Chars.QuoteDbl && !IsEof(_currChar))
            {
                if (_currChar == '\\')
                {
                    ScanNextChar(); // read escaped characters
                    switch (_currChar)
                    {
                        case 'a':
                            str.Append('\a');
                            break;

                        case 'b':
                            str.Append('\b');
                            break;

                        case 'f':
                            str.Append('\f');
                            break;

                        case 'n':
                            str.Append('\n');
                            break;

                        case 'r':
                            str.Append('\r');
                            break;

                        case 't':
                            str.Append('\t');
                            break;

                        case 'v':
                            str.Append('\v');
                            break;

                        case '\'':
                            str.Append('\'');
                            break;

                        case '\"':
                            str.Append('\"');
                            break;

                        case '\\':
                            str.Append('\\');
                            break;

                        case 'x':
                            {
                                ScanNextChar();
                                int hexNrCount = 0;
                                //string hexString = "0x";
                                while (IsHexDigit(_currChar))
                                {
                                    ++hexNrCount;
                                    //hexString += _currChar;
                                    ScanNextChar();
                                }
                                if (hexNrCount <= 2)
                                    str.Append("?????"); //(char)AscULongFromHexString(hexString);
                                else
                                    throw new DdlParserException(DdlErrorLevel.Error,
                                        DomSR.GetString(DomMsgID.EscapeSequenceNotAllowed), DomMsgID.EscapeSequenceNotAllowed);
                            }
                            break;

                        //NYI: octal numbers
                        //case '0':
                        //{
                        //  ScanNextChar();
                        //  int hexNrCount = 0;
                        //  string hexString = "0x";
                        //  while (IsOctDigit(currChar))
                        //  {
                        //    ++hexNrCount;
                        //    hexString += currChar;
                        //    ScanNextChar();
                        //  }
                        //  if (hexNrCount <=2)
                        //    str += "?????"; //(char)AscULongFromHexString(hexString);
                        //  else
                        //    throw new DdlParserException(DdlErrorLevel.Error, "DdlScanner",DomMsgID.EscapeSequenceNotAllowed, null);
                        //}
                        //  break;

                        default:
                            throw new DdlParserException(DdlErrorLevel.Error,
                              DomSR.GetString(DomMsgID.EscapeSequenceNotAllowed), DomMsgID.EscapeSequenceNotAllowed);
                    }
                }
                else if (_currChar == Chars.Null || _currChar == Chars.CR || _currChar == Chars.LF)
                    throw new DdlParserException(DdlErrorLevel.Error,
                      DomSR.GetString(DomMsgID.NewlineInString), DomMsgID.NewlineInString);
                else
                    str.Append(_currChar);

                ScanNextChar();
            }
            ScanNextChar();  // read '"'
            return str.ToString();
        }

        /// <summary>
        /// Save the current scanner location in the document for error handling.
        /// </summary>
        void SaveCurDocumentPos()
        {
            _nCurDocumentIndex = _idx - 1;
            _nCurDocumentLine = _idxLine;
            _nCurDocumentLinePos = _idxLinePos;
        }

        int _nCurDocumentIndex;
        int _nCurDocumentLine;
        int _nCurDocumentLinePos;

        string _documentFileName;
        string _documentPath;
        string _strDocument;
        int _ddlLength;
        int _idx;
        int _idxLine;
        int _idxLinePos;

        char _currChar;
        char _nextChar;
        string _token = "";
        Symbol _symbol = Symbol.None;
        Symbol _prevSymbol = Symbol.None;
        TokenType _tokenType = TokenType.None;
        bool _emptyLine;

        DdlReaderErrors _errors;
    }
}
