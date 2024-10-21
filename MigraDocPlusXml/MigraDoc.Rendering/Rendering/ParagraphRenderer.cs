﻿#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering.Resources;
using PdfSharp.Pdf.Advanced;

namespace MigraDoc.Rendering
{
    internal struct TabOffset
    {
        internal TabOffset(TabLeader leader, XUnit offset)
        {
            Leader = leader;
            Offset = offset;
        }
        internal TabLeader Leader;
        internal XUnit Offset;
    }

    /// <summary>
    /// Summary description for ParagraphRenderer.
    /// </summary>
    internal class ParagraphRenderer : Renderer
    {
        /// <summary>
        /// Process phases of the renderer.
        /// </summary>
        private enum Phase
        {
            Formatting,
            Rendering
        }

        /// <summary>
        /// Results that can occur when processing a paragraph element
        /// during formatting.
        /// </summary>
        private enum FormatResult
        {
            /// <summary>
            /// Ignore the current element during formatting.
            /// </summary>
            Ignore,

            /// <summary>
            /// Continue with the next element within the same line.
            /// </summary>
            Continue,

            /// <summary>
            /// Start a new line from the current object on.
            /// </summary>
            NewLine,

            /// <summary>
            /// Break formatting and continue in a new area (e.g. a new page).
            /// </summary>
            NewArea
        }
        Phase _phase;

        /// <summary>
        /// Initializes a ParagraphRenderer object for formatting.
        /// </summary>
        /// <param name="gfx">The XGraphics object to do measurements on.</param>
        /// <param name="paragraph">The paragraph to format.</param>
        /// <param name="fieldInfos">The field infos.</param>
        internal ParagraphRenderer(XGraphics gfx, Paragraph paragraph, FieldInfos fieldInfos)
            : base(gfx, paragraph, fieldInfos)
        {
            _paragraph = paragraph;

            ParagraphRenderInfo parRenderInfo = new ParagraphRenderInfo();
            parRenderInfo.DocumentObject = _paragraph;
            ((ParagraphFormatInfo)parRenderInfo.FormatInfo)._widowControl = _paragraph.Format.WidowControl;

            _renderInfo = parRenderInfo;
        }

        /// <summary>
        /// Initializes a ParagraphRenderer object for rendering.
        /// </summary>
        /// <param name="gfx">The XGraphics object to render on.</param>
        /// <param name="renderInfo">The render info object containing information necessary for rendering.</param>
        /// <param name="fieldInfos">The field infos.</param>
        internal ParagraphRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _paragraph = (Paragraph)renderInfo.DocumentObject;
        }

        /// <summary>
        /// Renders the paragraph.
        /// </summary>
        internal override void Render()
        {
            InitRendering();
            if ((int)_paragraph.Format.OutlineLevel >= 1 && _gfx.PdfPage != null) // Don't call GetOutlineTitle() in vain
                _documentRenderer.AddOutline((int)_paragraph.Format.OutlineLevel, GetOutlineTitle(), _gfx.PdfPage, GetDestinationPosition());

            RenderShading();
            RenderBorders();

            ParagraphFormatInfo parFormatInfo = (ParagraphFormatInfo)_renderInfo.FormatInfo;
            for (int idx = 0; idx < parFormatInfo.LineCount; ++idx)
            {
                LineInfo lineInfo = parFormatInfo.GetLineInfo(idx);
                _isLastLine = (idx == parFormatInfo.LineCount - 1);

                _lastTabPosition = 0;
                if (lineInfo.ReMeasureLine)
                    ReMeasureLine(ref lineInfo);

                RenderLine(lineInfo);
            }
        }

        bool IsRenderedField(DocumentObject docObj)
        {
            if (docObj is NumericFieldBase)
                return true;

            if (docObj is DocumentInfo)
                return true;

            if (docObj is DateField)
                return true;

            return false;
        }

        string GetFieldValue(DocumentObject field)
        {
            NumericFieldBase numericFieldBase = field as NumericFieldBase;
            if (numericFieldBase != null)
            {
                int number = -1;
                PageRefField refField = field as PageRefField;
                if (refField != null)
                {
                    PageRefField pageRefField = refField;
                    number = _fieldInfos.GetShownPageNumber(pageRefField.Name);
                    if (number <= 0)
                    {
                        if (_phase == Phase.Formatting)
                            return "XX";
                        return Messages2.BookmarkNotDefined(pageRefField.Name);
                    }
                }
                else if (field is SectionField)
                {
                    number = _fieldInfos.Section;
                    if (number <= 0)
                        return "XX";
                }
                else if (field is PageField)
                {
                    number = _fieldInfos.DisplayPageNr;
                    if (number <= 0)
                        return "XX";
                }
                else if (field is NumPagesField)
                {
                    number = _fieldInfos.NumPages;
                    if (number <= 0)
                        return "XXX";
                }
                else if (field is SectionPagesField)
                {
                    number = _fieldInfos.SectionPages;
                    if (number <= 0)
                        return "XX";
                }
                return NumberFormatter.Format(number, numericFieldBase.Format);
            }
            else
            {
                DateField dateField = field as DateField;
                if (dateField != null)
                {
                    DateTime dt = (_fieldInfos.Date);
                    if (dt == DateTime.MinValue)
                        dt = DateTime.Now;

                    return _fieldInfos.Date.ToString(dateField.Format);
                }

                InfoField infoField = field as InfoField;
                if (infoField != null)
                    return GetDocumentInfo(infoField.Name);

                Debug.Assert(false, "Given parameter must be a rendered Field");
            }

            return "";
        }

        string GetOutlineTitle()
        {
            ParagraphIterator iter = new ParagraphIterator(_paragraph.Elements);
            iter = iter.GetFirstLeaf();

            bool ignoreBlank = true;
            string title = "";
            while (iter != null)
            {
                DocumentObject current = iter.Current;
                if (!ignoreBlank && (IsBlank(current) || IsTab(current) || IsLineBreak(current)))
                {
                    title += " ";
                    ignoreBlank = true;
                }
                else if (current is Text)
                {
                    title += ((Text)current).Content;
                    ignoreBlank = false;
                }
                else if (IsRenderedField(current))
                {
                    title += GetFieldValue(current);
                    ignoreBlank = false;
                }
                else if (IsSymbol(current))
                {
                    title += GetSymbol((Character)current);
                    ignoreBlank = false;
                }

                if (title.Length > 64)
                    break;
                iter = iter.GetNextLeaf();
            }
            return title;
        }

        /// <summary>
        /// Gets a layout info with only margin and break information set.
        /// It can be taken before the paragraph is formatted.
        /// </summary>
        /// <remarks>
        /// The following layout information is set properly:<br />
        /// MarginTop, MarginLeft, MarginRight, MarginBottom, KeepTogether, KeepWithNext, PagebreakBefore.
        /// </remarks>
        internal override LayoutInfo InitialLayoutInfo
        {
            get
            {
                LayoutInfo layoutInfo = new LayoutInfo();
                layoutInfo.PageBreakBefore = _paragraph.Format.PageBreakBefore;
                layoutInfo.MarginTop = _paragraph.Format.SpaceBefore.Point;
                layoutInfo.MarginBottom = _paragraph.Format.SpaceAfter.Point;
                //Don't confuse margins with left or right indent.
                //Indents are invisible for the layouter.
                layoutInfo.MarginRight = 0;
                layoutInfo.MarginLeft = 0;
                layoutInfo.KeepTogether = _paragraph.Format.KeepTogether;
                layoutInfo.KeepWithNext = _paragraph.Format.KeepWithNext;
                return layoutInfo;
            }
        }

        /// <summary>
        /// Adjusts the current x position to the given tab stop if possible.
        /// </summary>
        /// <returns>True, if the text doesn't fit the line any more and the tab causes a line break.</returns>
        FormatResult FormatTab()
        {
            // For Tabs in Justified context
            if (_paragraph.Format.Alignment == ParagraphAlignment.Justify)
                _reMeasureLine = true;
            TabStop nextTabStop = GetNextTabStop();
            _savedWordWidth = 0;
            if (nextTabStop == null)
                return FormatResult.NewLine;

            bool notFitting = false;
            XUnit xPositionBeforeTab = _currentXPosition;
            switch (nextTabStop.Alignment)
            {
                case TabAlignment.Left:
                    _currentXPosition = ProbeAfterLeftAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Right:
                    _currentXPosition = ProbeAfterRightAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Center:
                    _currentXPosition = ProbeAfterCenterAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Decimal:
                    _currentXPosition = ProbeAfterDecimalAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;
            }
            if (!notFitting)
            {
                // For correct right paragraph alignment with tabs
                if (!IgnoreHorizontalGrowth)
                    _currentLineWidth += _currentXPosition - xPositionBeforeTab;

                _tabOffsets.Add(new TabOffset(nextTabStop.Leader, _currentXPosition - xPositionBeforeTab));
                if (_currentLeaf != null)
                    _lastTab = _currentLeaf.Current;
            }

            return notFitting ? FormatResult.NewLine : FormatResult.Continue;
        }

        bool IsLineBreak(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                if (((Character)docObj).SymbolName == SymbolName.LineBreak)
                    return true;
            }
            return false;
        }

        bool IsBlank(DocumentObject docObj)
        {
            if (docObj is Text)
            {
                if (((Text)docObj).Content == " ")
                    return true;
            }
            return false;
        }

        bool IsTab(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                if (((Character)docObj).SymbolName == SymbolName.Tab)
                    return true;
            }
            return false;
        }

        bool IsSoftHyphen(DocumentObject docObj)
        {
            Text text = docObj as Text;
            if (text != null)
                return text.Content == "­";

            return false;
        }

        /// <summary>
        /// Probes the paragraph elements after a left aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterLeftAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;
            _currentXPosition = _formattingArea.X + tabStopPosition.Point;

            notFitting = ProbeAfterTab();
            if (!notFitting)
                xPositionAfterTab = _formattingArea.X + tabStopPosition;

            //--- Restore ---------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterRightAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;

            notFitting = ProbeAfterTab();
            if (!notFitting && xPosition + _currentLineWidth <= _formattingArea.X + tabStopPosition)
                xPositionAfterTab = _formattingArea.X + tabStopPosition - _currentLineWidth;

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        Hyperlink GetHyperlink()
        {
            DocumentObject elements = DocumentRelations.GetParent(_currentLeaf.Current);
            DocumentObject parent = DocumentRelations.GetParent(elements);
            while (!(parent is Paragraph))
            {
                Hyperlink hyperlink = parent as Hyperlink;
                if (hyperlink != null)
                    return hyperlink;
                elements = DocumentRelations.GetParent(parent);
                parent = DocumentRelations.GetParent(elements);
            }
            return null;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterCenterAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;
            notFitting = ProbeAfterTab();

            if (!notFitting)
            {
                if (xPosition + _currentLineWidth / 2.0 <= _formattingArea.X + tabStopPosition)
                {
                    Rectangle rect = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);
                    if (_formattingArea.X + tabStopPosition + _currentLineWidth / 2.0 > rect.X + rect.Width - RightIndent)
                    {
                        //the text is too long on the right hand side of the tabstop => align to right indent.
                        xPositionAfterTab = rect.X +
                          rect.Width -
                          RightIndent -
                          _currentLineWidth;
                    }
                    else
                        xPositionAfterTab = _formattingArea.X + tabStopPosition - _currentLineWidth / 2;
                }
            }

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterDecimalAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            notFitting = false;
            ParagraphIterator savedLeaf = _currentLeaf;

            //Extra for auto tab after list symbol
            if (IsTab(_currentLeaf.Current))
                _currentLeaf = _currentLeaf.GetNextLeaf();
            if (_currentLeaf == null)
            {
                _currentLeaf = savedLeaf;
                return _currentXPosition + tabStopPosition;
            }
            VerticalLineInfo newVerticalInfo = CalcCurrentVerticalInfo();
            Rectangle fittingRect = _formattingArea.GetFittingRect(_currentYPosition, newVerticalInfo.Height);
            if (fittingRect == null)
            {
                notFitting = true;
                _currentLeaf = savedLeaf;
                return _currentXPosition;
            }

            if (IsPlainText(_currentLeaf.Current))
            {
                Text text = (Text)_currentLeaf.Current;
                string word = text.Content;
                int lastIndex = text.Content.LastIndexOfAny(new char[] { ',', '.' });
                if (lastIndex > 0)
                    word = word.Substring(0, lastIndex);

                XUnit wordLength = MeasureString(word);
                notFitting = _currentXPosition + wordLength >= _formattingArea.X + _formattingArea.Width + Tolerance;
                if (!notFitting)
                    return _formattingArea.X + tabStopPosition - wordLength;

                return _currentXPosition;
            }
            _currentLeaf = savedLeaf;
            return ProbeAfterRightAlignedTab(tabStopPosition, out notFitting);
        }

        void SaveBeforeProbing(out ParagraphIterator paragraphIter, out int blankCount, out XUnit wordsWidth, out XUnit xPosition, out XUnit lineWidth, out XUnit blankWidth)
        {
            paragraphIter = _currentLeaf;
            blankCount = _currentBlankCount;
            xPosition = _currentXPosition;
            lineWidth = _currentLineWidth;
            wordsWidth = _currentWordsWidth;
            blankWidth = _savedBlankWidth;
        }

        void RestoreAfterProbing(ParagraphIterator paragraphIter, int blankCount, XUnit wordsWidth, XUnit xPosition, XUnit lineWidth, XUnit blankWidth)
        {
            _currentLeaf = paragraphIter;
            _currentBlankCount = blankCount;
            _currentXPosition = xPosition;
            _currentLineWidth = lineWidth;
            _currentWordsWidth = wordsWidth;
            _savedBlankWidth = blankWidth;
        }

        /// <summary>
        /// Probes the paragraph after a tab.
        /// Caution: This Function resets the word count and line width before doing its work.
        /// </summary>
        /// <returns>True if the tab causes a linebreak.</returns>
        bool ProbeAfterTab()
        {
            _currentLineWidth = 0;
            _currentBlankCount = 0;
            //Extra for auto tab after list symbol

            //TODO: KLPO4KLPO: Check if this conditional statement is still required
            if (_currentLeaf != null && IsTab(_currentLeaf.Current))
                _currentLeaf = _currentLeaf.GetNextLeaf();

            bool wordAppeared = false;
            while (_currentLeaf != null && !IsLineBreak(_currentLeaf.Current) && !IsTab(_currentLeaf.Current))
            {
                FormatResult result = FormatElement(_currentLeaf.Current);
                if (result != FormatResult.Continue)
                    break;

                wordAppeared = wordAppeared || IsWordLikeElement(_currentLeaf.Current);
                _currentLeaf = _currentLeaf.GetNextLeaf();
            }
            return _currentLeaf != null && !IsLineBreak(_currentLeaf.Current) &&
              !IsTab(_currentLeaf.Current) && !wordAppeared;
        }

        /// <summary>
        /// Gets the next tab stop following the current x position.
        /// </summary>
        /// <returns>The searched tab stop.</returns>
        private TabStop GetNextTabStop()
        {
            ParagraphFormat format = _paragraph.Format;
            TabStops tabStops = format.TabStops;
            XUnit lastPosition = 0;

            foreach (TabStop tabStop in tabStops)
            {
                if (tabStop.Position.Point > _formattingArea.Width - RightIndent + Tolerance)
                    break;

                // Compare with "Tolerance" to prevent rounding errors from taking us one tabstop too far.
                if (tabStop.Position.Point + _formattingArea.X > _currentXPosition + Tolerance)
                    return tabStop;

                lastPosition = tabStop.Position.Point;
            }
            //Automatic tab stop: FirstLineIndent < 0 => automatic tab stop at LeftIndent.

            if (format.FirstLineIndent < 0 || 
                (format._listInfo != null && !format._listInfo.IsNull() && format.ListInfo.NumberPosition < format.LeftIndent))
            {
                XUnit leftIndent = format.LeftIndent.Point;
                if (_isFirstLine && _currentXPosition < leftIndent + _formattingArea.X)
                    return new TabStop(leftIndent.Point);
            }
            XUnit defaultTabStop = "1.25cm";
            if (!_paragraph.Document._defaultTabStop.IsNull)
                defaultTabStop = _paragraph.Document.DefaultTabStop.Point;

            XUnit currTabPos = defaultTabStop;
            while (currTabPos + _formattingArea.X <= _formattingArea.Width - RightIndent)
            {
                if (currTabPos > lastPosition && currTabPos + _formattingArea.X > _currentXPosition + Tolerance)
                    return new TabStop(currTabPos.Point);

                currTabPos += defaultTabStop;
            }
            return null;
        }

        /// <summary>
        /// Gets the horizontal position to start a new line.
        /// </summary>
        /// <returns>The position to start the line.</returns>
        XUnit StartXPosition
        {
            get
            {
                XUnit xPos = 0;

                if (_phase == Phase.Formatting)
                {
                    xPos = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height).X;
                    xPos += LeftIndent;
                }
                else //if (phase == Phase.Rendering)
                {
                    Area contentArea = _renderInfo.LayoutInfo.ContentArea;
                    //next lines for non fitting lines that produce an empty fitting rect:
                    XUnit rectX = contentArea.X;
                    XUnit rectWidth = contentArea.Width;

                    Rectangle fittingRect = contentArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);
                    if (fittingRect != null)
                    {
                        rectX = fittingRect.X;
                        rectWidth = fittingRect.Width;
                    }
                    switch (_paragraph.Format.Alignment)
                    {
                        case ParagraphAlignment.Left:
                        case ParagraphAlignment.Justify:
                            xPos = rectX;
                            xPos += LeftIndent;
                            break;

                        case ParagraphAlignment.Right:
                            xPos = rectX + rectWidth - RightIndent;
                            xPos -= _currentLineWidth;
                            break;

                        case ParagraphAlignment.Center:
                            xPos = rectX + (rectWidth + LeftIndent - RightIndent - _currentLineWidth) / 2.0;
                            break;
                    }
                }
                return xPos;
            }
        }

        /// <summary>
        /// Renders a single line.
        /// </summary>
        /// <param name="lineInfo"></param>
        void RenderLine(LineInfo lineInfo)
        {
            _currentVerticalInfo = lineInfo.Vertical;
            _currentLeaf = lineInfo.StartIter;
            _startLeaf = lineInfo.StartIter;
            _endLeaf = lineInfo.EndIter;
            _currentBlankCount = lineInfo.BlankCount;
            _currentLineWidth = lineInfo.LineWidth;
            _currentWordsWidth = lineInfo.WordsWidth;
            _currentXPosition = StartXPosition;
            _tabOffsets = lineInfo.TabOffsets;
            _lastTabPassed = lineInfo.LastTab == null;
            _lastTab = lineInfo.LastTab;

            _tabIdx = 0;

            bool ready = _currentLeaf == null;
            if (_isFirstLine)
                RenderListSymbol();

            while (!ready)
            {
                if (_currentLeaf.Current == lineInfo.EndIter.Current)
                    ready = true;

                if (_currentLeaf.Current == lineInfo.LastTab)
                    _lastTabPassed = true;
                RenderElement(_currentLeaf.Current);
                _currentLeaf = _currentLeaf.GetNextLeaf();
            }
            _currentYPosition += lineInfo.Vertical.Height;
            _isFirstLine = false;
        }

        void ReMeasureLine(ref LineInfo lineInfo)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            bool origLastTabPassed = _lastTabPassed;
            //------------------------------------------
            _currentLeaf = lineInfo.StartIter;
            _endLeaf = lineInfo.EndIter;
            _formattingArea = _renderInfo.LayoutInfo.ContentArea;
            _tabOffsets = new List<TabOffset>();
            _currentLineWidth = 0;
            _currentWordsWidth = 0;

            Rectangle fittingRect = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);
            if (fittingRect != null)
            {
                _currentXPosition = fittingRect.X + LeftIndent;
                FormatListSymbol();
                bool goOn = true;
                while (goOn && _currentLeaf != null)
                {
                    if (_currentLeaf.Current == lineInfo.LastTab)
                        _lastTabPassed = true;

                    FormatElement(_currentLeaf.Current);

                    goOn = _currentLeaf != null && _currentLeaf.Current != _endLeaf.Current;
                    if (goOn)
                        _currentLeaf = _currentLeaf.GetNextLeaf();
                }
                lineInfo.LineWidth = _currentLineWidth;
                lineInfo.WordsWidth = _currentWordsWidth;
                lineInfo.BlankCount = _currentBlankCount;
                lineInfo.TabOffsets = _tabOffsets;
                lineInfo.ReMeasureLine = false;
                _lastTabPassed = origLastTabPassed;
            }
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
        }

        XUnit CurrentWordDistance
        {
            get
            {
                if (_phase == Phase.Rendering &&
                  _paragraph.Format.Alignment == ParagraphAlignment.Justify && _lastTabPassed)
                {
                    if (_currentBlankCount >= 1 && !(_isLastLine && _renderInfo.FormatInfo.IsEnding))
                    {
                        Area contentArea = _renderInfo.LayoutInfo.ContentArea;
                        XUnit width = contentArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height).Width;
                        if (_lastTabPosition > 0)
                        {
                            width -= (_lastTabPosition -
                            contentArea.X);
                        }
                        else
                            width -= LeftIndent;

                        width -= RightIndent;
                        return (width - _currentWordsWidth) / (_currentBlankCount);
                    }
                }
                return MeasureString(" ");
            }
        }

        void RenderElement(DocumentObject docObj)
        {
            string typeName = docObj.GetType().Name;
            switch (typeName)
            {
                case "Text":
                    if (IsBlank(docObj))
                        RenderBlank();
                    else if (IsSoftHyphen(docObj))
                        RenderSoftHyphen();
                    else
                        RenderText((Text)docObj);
                    break;

                case "Character":
                    RenderCharacter((Character)docObj);
                    break;

                case "DateField":
                    RenderDateField((DateField)docObj);
                    break;

                case "InfoField":
                    RenderInfoField((InfoField)docObj);
                    break;

                case "NumPagesField":
                    RenderNumPagesField((NumPagesField)docObj);
                    break;

                case "PageField":
                    RenderPageField((PageField)docObj);
                    break;

                case "SectionField":
                    RenderSectionField((SectionField)docObj);
                    break;

                case "SectionPagesField":
                    RenderSectionPagesField((SectionPagesField)docObj);
                    break;

                case "BookmarkField":
                    RenderBookmarkField((BookmarkField)docObj);
                    break;

                case "PageRefField":
                    RenderPageRefField((PageRefField)docObj);
                    break;

                case "Image":
                    RenderImage((Image)docObj);
                    break;
                //        default:
                //          throw new NotImplementedException(typeName + " is coming soon...");
            }
        }

        void RenderImage(Image image)
        {
            RenderInfo renderInfo = CurrentImageRenderInfo;
            XUnit top = CurrentBaselinePosition;
            Area contentArea = renderInfo.LayoutInfo.ContentArea;
            top -= contentArea.Height;
            RenderByInfos(_currentXPosition, top, new RenderInfo[] { renderInfo });

            RenderUnderline(contentArea.Width, true);
            RealizeHyperlink(contentArea.Width);

            _currentXPosition += contentArea.Width;
        }

        void RenderDateField(DateField dateField)
        {
            RenderWord(_fieldInfos.Date.ToString(dateField.Format));
        }

        void RenderInfoField(InfoField infoField)
        {
            RenderWord(GetDocumentInfo(infoField.Name));
        }

        void RenderNumPagesField(NumPagesField numPagesField)
        {
            RenderWord(GetFieldValue(numPagesField));
        }

        void RenderPageField(PageField pageField)
        {
            RenderWord(GetFieldValue(pageField));
        }

        void RenderSectionField(SectionField sectionField)
        {
            RenderWord(GetFieldValue(sectionField));
        }

        void RenderSectionPagesField(SectionPagesField sectionPagesField)
        {
            RenderWord(GetFieldValue(sectionPagesField));
        }

        void RenderBookmarkField(BookmarkField bookmarkField)
        {
            // Add also a named destination, if a PdfDocument is rendered.
            var pdfDocument = _gfx?.PdfPage?.Owner;
            if (pdfDocument != null)
            {
                var pageNr = pdfDocument.PageCount; // Magic: Pages are added while rendering, so the current page number equals pdfDocument.PageCount.
                Debug.Assert(pageNr >= 1);

                var destinationName = bookmarkField.Name;
                var position = GetDestinationPosition();
                pdfDocument.AddNamedDestination(destinationName, pageNr, PdfNamedDestinationParameters.CreatePosition(position));
            }

            RenderUnderline(0, false);
        }

        /// <summary>
        /// Gets the current position for destinations in PDF world space units.
        /// The position is moved by a margin value to leave space between the window and the content that is located at the destination
        /// and it is transformed to PDF world space units.
        /// </summary>
        XPoint GetDestinationPosition()
        {
            var margin = XUnit.FromCentimeter(0.5);
            var x = _currentXPosition > margin ? _currentXPosition - margin : 0;
            var y = _currentYPosition > margin ? _currentYPosition - margin : 0;
            var destinationPosition = new XPoint(x, y);

            var pdfPosition = _gfx.Transformer.WorldToDefaultPage(destinationPosition);
            return pdfPosition;
        }

        void RenderPageRefField(PageRefField pageRefField)
        {
            RenderWord(GetFieldValue(pageRefField));
        }

        void RenderCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                case SymbolName.Em:
                case SymbolName.Em4:
                case SymbolName.En:
                    RenderSpace(character);
                    break;
                case SymbolName.LineBreak:
                    RenderLinebreak();
                    break;

                case SymbolName.Tab:
                    RenderTab();
                    break;

                default:
                    RenderSymbol(character);
                    break;
            }
        }

        void RenderSpace(Character character)
        {
            XUnit width = GetSpaceWidth(character);
            RenderUnderline(width, false);
            RealizeHyperlink(width);
            _currentXPosition += width;
        }

        void RenderLinebreak()
        {
            RenderUnderline(0, false);
            RealizeHyperlink(0);
        }

        void RenderSymbol(Character character)
        {
            string sym = GetSymbol(character);
            string completeWord = sym;
            for (int idx = 1; idx < character.Count; ++idx)
                completeWord += sym;

            RenderWord(completeWord);
        }

        void RenderTab()
        {
            TabOffset tabOffset = NextTabOffset();
            RenderUnderline(tabOffset.Offset, false);
            RenderTabLeader(tabOffset);
            RealizeHyperlink(tabOffset.Offset);
            _currentXPosition += tabOffset.Offset;
            if (_currentLeaf.Current == _lastTab)
                _lastTabPosition = _currentXPosition;
        }

        void RenderTabLeader(TabOffset tabOffset)
        {
            string leaderString;
            switch (tabOffset.Leader)
            {
                case TabLeader.Dashes:
                    leaderString = "-";
                    break;

                case TabLeader.Dots:
                    leaderString = ".";
                    break;

                case TabLeader.Heavy:
                case TabLeader.Lines:
                    leaderString = "_";
                    break;

                case TabLeader.MiddleDot:
                    leaderString = "·";
                    break;

                default:
                    return;
            }
            XUnit leaderWidth = MeasureString(leaderString);
            XUnit xPosition = _currentXPosition;
            string drawString = "";

            while (xPosition + leaderWidth <= _currentXPosition + tabOffset.Offset)
            {
                drawString += leaderString;
                xPosition += leaderWidth;
            }
            Font font = CurrentDomFont;
            XFont xFont = CurrentFont;
            if (font.Subscript || font.Superscript)
                xFont = FontHandler.ToSubSuperFont(xFont);

            _gfx.DrawString(drawString, xFont, CurrentBrush, _currentXPosition, CurrentBaselinePosition);
        }

        TabOffset NextTabOffset()
        {
            TabOffset offset = _tabOffsets.Count > _tabIdx ? _tabOffsets[_tabIdx] :
              new TabOffset(0, 0);

            ++_tabIdx;
            return offset;
        }
        int _tabIdx;

        bool IgnoreBlank()
        {
            if (_currentLeaf == _startLeaf)
                return true;

            if (_endLeaf != null && _currentLeaf.Current == _endLeaf.Current)
                return true;

            ParagraphIterator nextIter = _currentLeaf.GetNextLeaf();
            while (nextIter != null && (IsBlank(nextIter.Current) || nextIter.Current is BookmarkField))
            {
                nextIter = nextIter.GetNextLeaf();
            }
            if (nextIter == null)
                return true;

            if (IsTab(nextIter.Current))
                return true;

            ParagraphIterator prevIter = _currentLeaf.GetPreviousLeaf();
            // Can be null if currentLeaf is the first leaf
            DocumentObject obj = prevIter != null ? prevIter.Current : null;
            while (obj != null && obj is BookmarkField)
            {
                prevIter = prevIter.GetPreviousLeaf();
                obj = prevIter != null ? prevIter.Current : null;
            }
            if (obj == null)
                return true;

            return IsBlank(obj) || IsTab(obj);
        }

        void RenderBlank()
        {
            if (!IgnoreBlank())
            {
                XUnit wordDistance = CurrentWordDistance;
                RenderUnderline(wordDistance, false);
                RealizeHyperlink(wordDistance);
                _currentXPosition += wordDistance;
            }
            else
            {
                RenderUnderline(0, false);
                RealizeHyperlink(0);
            }
        }

        void RenderSoftHyphen()
        {
            if (_currentLeaf.Current == _endLeaf.Current)
                RenderWord("-");
        }

        void RenderText(Text text)
        {
            RenderWord(text.Content);
        }

        void RenderWord(string word)
        {
            Font font = CurrentDomFont;
            XFont xFont = CurrentFont;
            if (font.Subscript || font.Superscript)
                xFont = FontHandler.ToSubSuperFont(xFont);

            _gfx.DrawString(word, xFont, CurrentBrush, _currentXPosition, CurrentBaselinePosition);
            XUnit wordWidth = MeasureString(word);
            RenderUnderline(wordWidth, true);
            RealizeHyperlink(wordWidth);
            _currentXPosition += wordWidth;
        }

        void StartHyperlink(XUnit left, XUnit top)
        {
            _hyperlinkRect = new XRect(left, top, 0, 0);
        }

        void EndHyperlink(Hyperlink hyperlink, XUnit right, XUnit bottom)
        {
            _hyperlinkRect.Width = right - _hyperlinkRect.X;
            _hyperlinkRect.Height = bottom - _hyperlinkRect.Y;
            PdfPage page = _gfx.PdfPage;
            if (page != null)
            {
                XRect rect = _gfx.Transformer.WorldToDefaultPage(_hyperlinkRect);

                switch (hyperlink.Type)
                {
                    case HyperlinkType.Local:

                        // Try to use named destination, if a document is rendered.
                        var pdfDocument = _gfx.PdfPage.Owner;
                        if (pdfDocument != null)
                        {
                            page.AddDocumentLink(new PdfRectangle(rect), hyperlink.BookmarkName);
                        }
                        // Otherwise use page from bookmarks's fieldInfo.
                        else
                        {
                            var pageRef = _fieldInfos.GetPhysicalPageNumber(hyperlink.BookmarkName);
                        if (pageRef > 0)
                            page.AddDocumentLink(new PdfRectangle(rect), pageRef);
                        }
                        break;

                    case HyperlinkType.ExternalBookmark:
                        page.AddDocumentLink(new PdfRectangle(rect), hyperlink.Filename, hyperlink.BookmarkName, ConvertHyperlinkTargetWindow(hyperlink.NewWindow));
                        break;

                    case HyperlinkType.EmbeddedDocument:
                        page.AddEmbeddedDocumentLink(new PdfRectangle(rect), hyperlink.Filename, hyperlink.BookmarkName, ConvertHyperlinkTargetWindow(hyperlink.NewWindow));
                        break;

                    case HyperlinkType.Web:
                        page.AddWebLink(new PdfRectangle(rect), hyperlink.Filename);
                        break;

                    case HyperlinkType.File:
                        page.AddFileLink(new PdfRectangle(rect), hyperlink.Filename);
                        break;
                }
                _hyperlinkRect = new XRect();
            }
        }

        bool? ConvertHyperlinkTargetWindow(HyperlinkTargetWindow hyperlinkTargetWindow)
        {
            switch (hyperlinkTargetWindow)
            {
                case HyperlinkTargetWindow.NewWindow:
                    return true;
                case HyperlinkTargetWindow.SameWindow:
                    return false;
                case HyperlinkTargetWindow.UserPreference:
                default:
                    return null;
            }
        }

        void RealizeHyperlink(XUnit width)
        {
            XUnit top = _currentYPosition;
            XUnit left = _currentXPosition;
            XUnit bottom = top + _currentVerticalInfo.Height;
            XUnit right = left + width;
            Hyperlink hyperlink = GetHyperlink();

            bool hyperlinkChanged = _currentHyperlink != hyperlink;

            if (hyperlinkChanged)
            {
                if (_currentHyperlink != null)
                    EndHyperlink(_currentHyperlink, left, bottom);

                if (hyperlink != null)
                    StartHyperlink(left, top);

                _currentHyperlink = hyperlink;
            }

            if (_currentLeaf.Current == _endLeaf.Current)
            {
                if (_currentHyperlink != null)
                    EndHyperlink(_currentHyperlink, right, bottom);

                _currentHyperlink = null;
            }
        }
        Hyperlink _currentHyperlink;
        XRect _hyperlinkRect;

        XUnit CurrentBaselinePosition
        {
            get
            {
                VerticalLineInfo verticalInfo = _currentVerticalInfo;
                XUnit position = _currentYPosition;

                Font font = CurrentDomFont;
                XFont xFont = CurrentFont;

                // $MaOs BUG: For LineSpacingRule.AtLeast the text should be positioned at the line bottom. Maybe verticalInfo.InherentlineSpace does not contain the lineSpacing value in this case.
                double setLineSpace = verticalInfo.InherentlineSpace;
                double standardFontLineSpace = xFont.GetHeight();

                // Set position to bottom of text.
                position += setLineSpace;
                if (font.Subscript)
                {
                    // Move sub-/superscaled descender up.
                    position -= FontHandler.GetSubSuperScaling(CurrentFont) * FontHandler.GetDescent(xFont);
                }
                else if (font.Superscript)
                {
                    // Set position to top of text.
                    position -= standardFontLineSpace;
                    // Move sub-/superscaled LineSpace down and descender up.
                    position += FontHandler.GetSubSuperScaling(CurrentFont) * (standardFontLineSpace - FontHandler.GetDescent(xFont));
                }
                else
                    // Move descender up.
                    position -= verticalInfo.Descent;

                return position;
            }
        }

        XBrush CurrentBrush
        {
            get
            {
                if (_currentLeaf != null)
                    return FontHandler.FontColorToXBrush(CurrentDomFont);

                return null;
            }
        }

        private void InitRendering()
        {
            _phase = Phase.Rendering;

            ParagraphFormatInfo parFormatInfo = (ParagraphFormatInfo)_renderInfo.FormatInfo;
            if (parFormatInfo.LineCount == 0)
                return;
            _isFirstLine = parFormatInfo.IsStarting;

            LineInfo lineInfo = parFormatInfo.GetFirstLineInfo();
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            _currentYPosition = contentArea.Y + TopBorderOffset;
            // StL: GetFittingRect liefert manchmal null
            Rectangle rect = contentArea.GetFittingRect(_currentYPosition, lineInfo.Vertical.Height);
            if (rect != null)
                _currentXPosition = rect.X;
            _currentLineWidth = 0;
        }

        /// <summary>
        /// Initializes this instance for formatting.
        /// </summary>
        /// <param name="area">The area for formatting</param>
        /// <param name="previousFormatInfo">A previous format info.</param>
        /// <returns>False, if nothing of the paragraph will fit the area any more.</returns>
        private bool InitFormat(Area area, FormatInfo previousFormatInfo)
        {
            _phase = Phase.Formatting;

            _tabOffsets = new List<TabOffset>();

            ParagraphFormatInfo prevParaFormatInfo = (ParagraphFormatInfo)previousFormatInfo;
            if (previousFormatInfo == null || prevParaFormatInfo.LineCount == 0)
            {
                ((ParagraphFormatInfo)_renderInfo.FormatInfo)._isStarting = true;
                ParagraphIterator parIt = new ParagraphIterator(_paragraph.Elements);
                _currentLeaf = parIt.GetFirstLeaf();
                _isFirstLine = true;
            }
            else
            {
                _currentLeaf = prevParaFormatInfo.GetLastLineInfo().EndIter.GetNextLeaf();
                _isFirstLine = false;
                ((ParagraphFormatInfo)_renderInfo.FormatInfo)._isStarting = false;
            }

            _startLeaf = _currentLeaf;
            _currentVerticalInfo = CalcCurrentVerticalInfo();
            _currentYPosition = area.Y + TopBorderOffset;
            _formattingArea = area;
            Rectangle rect = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);
            if (rect == null)
                return false;

            _currentXPosition = rect.X + LeftIndent;
            if (_isFirstLine)
                FormatListSymbol();

            return true;
        }

        /// <summary>
        /// Gets information necessary to render or measure the list symbol.
        /// </summary>
        /// <param name="symbol">The text to list symbol to render or measure</param>
        /// <param name="font">The font to use for rendering or measuring.</param>
        /// <returns>True, if a symbol needs to be rendered.</returns>
        bool GetListSymbol(out string symbol, out XFont font)
        {
            font = null;
            symbol = null;
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo)_renderInfo.FormatInfo;
            if (_phase == Phase.Formatting)
            {
                ParagraphFormat format = _paragraph.Format;
                if (format._listInfo != null && !format._listInfo.IsNull())
                {
                    ListInfo listInfo = format.ListInfo;
                    double size = format.Font.Size;
                    XFontStyle style = FontHandler.GetXStyle(format.Font);

                    switch (listInfo.ListType)
                    {
                        case ListType.BulletList1:
                            symbol = "·";
                            font = new XFont("Symbol", size, style);
                            break;

                        case ListType.BulletList2:
                            symbol = "o";
                            font = new XFont("Courier New", size, style);
                            break;

                        case ListType.BulletList3:
                            symbol = "§";
                            font = new XFont("Wingdings", size, style);
                            break;

                        case ListType.NumberList1:
                            symbol = _documentRenderer.NextListNumber(listInfo) + ".";
                            font = FontHandler.FontToXFont(format.Font, _gfx.MUH);
                            break;

                        case ListType.NumberList2:
                            symbol = _documentRenderer.NextListNumber(listInfo) + ")";
                            font = FontHandler.FontToXFont(format.Font, _gfx.MUH);
                            break;

                        case ListType.NumberList3:
                            symbol = NumberFormatter.Format(_documentRenderer.NextListNumber(listInfo), "alphabetic") + ")";
                            font = FontHandler.FontToXFont(format.Font, _gfx.MUH);
                            break;
                    }
                    formatInfo.ListFont = font;
                    formatInfo.ListSymbol = symbol;
                    return true;
                }
            }
            else
            {
                if (formatInfo.ListFont != null && formatInfo.ListSymbol != null)
                {
                    font = formatInfo.ListFont;
                    symbol = formatInfo.ListSymbol;
                    return true;
                }
            }
            return false;
        }

        XUnit LeftIndent
        {
            get
            {
                ParagraphFormat format = _paragraph.Format;
                XUnit leftIndent = format.LeftIndent.Point;
                if (_isFirstLine)
                {
                    if (format._listInfo != null && !format._listInfo.IsNull())
                    {
                        if (!format.ListInfo._numberPosition.IsNull)
                            return format.ListInfo.NumberPosition.Point;
                        if (format._firstLineIndent.IsNull)
                            return 0;
                    }
                    return leftIndent + _paragraph.Format.FirstLineIndent.Point;
                }
                return leftIndent;
            }
        }

        XUnit RightIndent
        {
            get
            {
                return _paragraph.Format.RightIndent.Point;
            }
        }

        /// <summary>
        /// Formats the paragraph by performing line breaks etc.
        /// </summary>
        /// <param name="area">The area in which to render.</param>
        /// <param name="previousFormatInfo">The format info that was obtained on formatting the same paragraph on a previous area.</param>
        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ParagraphFormatInfo formatInfo = ((ParagraphFormatInfo)_renderInfo.FormatInfo);
            if (!InitFormat(area, previousFormatInfo))
            {
                formatInfo._isStarting = false;
                return;
            }
            formatInfo._isEnding = true;

            FormatResult lastResult = FormatResult.Continue;
            while (_currentLeaf != null)
            {
                FormatResult result = FormatElement(_currentLeaf.Current);
                switch (result)
                {
                    case FormatResult.Ignore:
                        _currentLeaf = _currentLeaf.GetNextLeaf();
                        break;

                    case FormatResult.Continue:
                        lastResult = result;
                        _currentLeaf = _currentLeaf.GetNextLeaf();
                        break;

                    case FormatResult.NewLine:
                        lastResult = result;
                        StoreLineInformation();
                        if (!StartNewLine())
                        {
                            result = FormatResult.NewArea;
                            formatInfo._isEnding = false;
                        }
                        break;
                }
                if (result == FormatResult.NewArea)
                {
                    lastResult = result;
                    formatInfo._isEnding = false;
                    break;
                }
            }
            if (formatInfo.IsEnding && lastResult != FormatResult.NewLine)
                StoreLineInformation();

            formatInfo.ImageRenderInfos = _imageRenderInfos;
            FinishLayoutInfo();
        }

        /// <summary>
        /// Finishes the layout info by calculating starting and trailing heights.
        /// </summary>
        private void FinishLayoutInfo()
        {
            LayoutInfo layoutInfo = _renderInfo.LayoutInfo;
            ParagraphFormat format = _paragraph.Format;
            ParagraphFormatInfo parInfo = (ParagraphFormatInfo)_renderInfo.FormatInfo;
            layoutInfo.MinWidth = _minWidth;
            layoutInfo.KeepTogether = format.KeepTogether;

            if (parInfo.IsComplete)
            {
                int limitOfLines = 1;
                if (parInfo._widowControl)
                    limitOfLines = 3;

                if (parInfo.LineCount <= limitOfLines)
                    layoutInfo.KeepTogether = true;
            }
            if (parInfo.IsStarting)
            {
                layoutInfo.MarginTop = format.SpaceBefore.Point;
                layoutInfo.PageBreakBefore = format.PageBreakBefore;
            }
            else
            {
                layoutInfo.MarginTop = 0;
                layoutInfo.PageBreakBefore = false;
            }

            if (parInfo.IsEnding)
            {
                layoutInfo.MarginBottom = _paragraph.Format.SpaceAfter.Point;
                layoutInfo.KeepWithNext = _paragraph.Format.KeepWithNext;
            }
            else
            {
                layoutInfo.MarginBottom = 0;
                layoutInfo.KeepWithNext = false;
            }
            if (parInfo.LineCount > 0)
            {
                XUnit startingHeight = parInfo.GetFirstLineInfo().Vertical.Height;
                if (parInfo._isStarting && _paragraph.Format.WidowControl && parInfo.LineCount >= 2)
                    startingHeight += parInfo.GetLineInfo(1).Vertical.Height;

                layoutInfo.StartingHeight = startingHeight;

                XUnit trailingHeight = parInfo.GetLastLineInfo().Vertical.Height;

                if (parInfo.IsEnding && _paragraph.Format.WidowControl && parInfo.LineCount >= 2)
                    trailingHeight += parInfo.GetLineInfo(parInfo.LineCount - 2).Vertical.Height;

                layoutInfo.TrailingHeight = trailingHeight;
            }
        }


        private XUnit PopSavedBlankWidth()
        {
            XUnit width = _savedBlankWidth;
            _savedBlankWidth = 0;
            return width;
        }

        private void SaveBlankWidth(XUnit blankWidth)
        {
            _savedBlankWidth = blankWidth;
        }
        XUnit _savedBlankWidth = 0;

        /// <summary>
        /// Processes the elements when formatting.
        /// </summary>
        /// <param name="docObj"></param>
        /// <returns></returns>
        FormatResult FormatElement(DocumentObject docObj)
        {
            switch (docObj.GetType().Name)
            {
                case "Text":
                    if (IsBlank(docObj))
                        return FormatBlank();
                    if (IsSoftHyphen(docObj))
                        return FormatSoftHyphen();
                    return FormatText((Text)docObj);

                case "Character":
                    return FormatCharacter((Character)docObj);

                case "DateField":
                    return FormatDateField((DateField)docObj);

                case "InfoField":
                    return FormatInfoField((InfoField)docObj);

                case "NumPagesField":
                    return FormatNumPagesField((NumPagesField)docObj);

                case "PageField":
                    return FormatPageField((PageField)docObj);

                case "SectionField":
                    return FormatSectionField((SectionField)docObj);

                case "SectionPagesField":
                    return FormatSectionPagesField((SectionPagesField)docObj);

                case "BookmarkField":
                    return FormatBookmarkField((BookmarkField)docObj);

                case "PageRefField":
                    return FormatPageRefField((PageRefField)docObj);

                case "Image":
                    return FormatImage((Image)docObj);

                default:
                    return FormatResult.Continue;
            }
        }

        FormatResult FormatImage(Image image)
        {
            XUnit width = CurrentImageRenderInfo.LayoutInfo.ContentArea.Width;
            return FormatAsWord(width);
        }

        RenderInfo CalcImageRenderInfo(Image image)
        {
            Renderer renderer = Create(_gfx, _documentRenderer, image, _fieldInfos);
            renderer.Format(new Rectangle(0, 0, double.MaxValue, double.MaxValue), null);

            return renderer.RenderInfo;
        }

        bool IsPlainText(DocumentObject docObj)
        {
            if (docObj is Text)
                return !IsSoftHyphen(docObj) && !IsBlank(docObj);

            return false;
        }

        bool IsSymbol(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                return !IsSpaceCharacter(docObj) && !IsTab(docObj) && !IsLineBreak(docObj);
            }
            return false;
        }

        bool IsSpaceCharacter(DocumentObject docObj)
        {
            Character character = docObj as Character;
            if (character != null)
            {
                switch ((character).SymbolName)
                {
                    case SymbolName.Blank:
                    case SymbolName.Em:
                    case SymbolName.Em4:
                    case SymbolName.En:
                        return true;
                }
            }
            return false;
        }

        bool IsWordLikeElement(DocumentObject docObj)
        {
            if (IsPlainText(docObj))
                return true;

            if (IsRenderedField(docObj))
                return true;

            if (IsSymbol(docObj))
                return true;


            return false;
        }

        FormatResult FormatBookmarkField(BookmarkField bookmarkField)
        {
            _fieldInfos.AddBookmark(bookmarkField.Name);
            return FormatResult.Ignore;
        }

        FormatResult FormatPageRefField(PageRefField pageRefField)
        {
            _reMeasureLine = true;
            string fieldValue = GetFieldValue(pageRefField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatNumPagesField(NumPagesField numPagesField)
        {
            _reMeasureLine = true;
            string fieldValue = GetFieldValue(numPagesField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatPageField(PageField pageField)
        {
            _reMeasureLine = true;
            string fieldValue = GetFieldValue(pageField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatSectionField(SectionField sectionField)
        {
            _reMeasureLine = true;
            string fieldValue = GetFieldValue(sectionField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatSectionPagesField(SectionPagesField sectionPagesField)
        {
            _reMeasureLine = true;
            string fieldValue = GetFieldValue(sectionPagesField);
            return FormatWord(fieldValue);
        }

        /// <summary>
        /// Helper function for formatting word-like elements like text and fields.
        /// </summary>
        FormatResult FormatWord(string word)
        {
            XUnit width = MeasureString(word);
            return FormatAsWord(width);
        }

        XUnit _savedWordWidth = 0;

        /// <summary>
        /// When rendering a justified paragraph, only the part after the last tab stop needs remeasuring.
        /// </summary>
        private bool IgnoreHorizontalGrowth
        {
            get
            {
                return _phase == Phase.Rendering && _paragraph.Format.Alignment == ParagraphAlignment.Justify &&
                    !_lastTabPassed;
            }
        }

        FormatResult FormatAsWord(XUnit width)
        {
            VerticalLineInfo newVertInfo = CalcCurrentVerticalInfo();

            Rectangle rect = _formattingArea.GetFittingRect(_currentYPosition, newVertInfo.Height + BottomBorderOffset);
            if (rect == null)
                return FormatResult.NewArea;

            if (_currentXPosition + width <= rect.X + rect.Width - RightIndent + Tolerance)
            {
                _savedWordWidth = width;
                _currentXPosition += width;
                // For Tabs in justified context
                if (!IgnoreHorizontalGrowth)
                    _currentWordsWidth += width;
                if (_savedBlankWidth > 0)
                {
                    // For Tabs in justified context
                    if (!IgnoreHorizontalGrowth)
                        ++_currentBlankCount;
                }
                // For Tabs in justified context
                if (!IgnoreHorizontalGrowth)
                    _currentLineWidth += width + PopSavedBlankWidth();
                _currentVerticalInfo = newVertInfo;
                _minWidth = Math.Max(_minWidth, width);
                return FormatResult.Continue;
            }
            else
            {
                _savedWordWidth = width;
                return FormatResult.NewLine;
            }
        }

        FormatResult FormatDateField(DateField dateField)
        {
            _reMeasureLine = true;
            string estimatedFieldValue = DateTime.Now.ToString(dateField.Format);
            return FormatWord(estimatedFieldValue);
        }

        FormatResult FormatInfoField(InfoField infoField)
        {
            string fieldValue = GetDocumentInfo(infoField.Name);
            if (fieldValue != "")
                return FormatWord(fieldValue);

            return FormatResult.Continue;
        }

        string GetDocumentInfo(string name)
        {
            string valueName;
            switch (name.ToLower())
            {
                case "title":
                    valueName = "Title";
                    break;

                case "author":
                    valueName = "Author";
                    break;

                case "keywords":
                    valueName = "Keywords";
                    break;

                case "subject":
                    valueName = "Subject";
                    break;

                default:
                    return String.Empty;
            }
            return _paragraph.Document.Info.GetValue(valueName).ToString();

            //string docInfoValue = "";
            //string[] enumNames = Enum.GetNames(typeof(InfoFieldType));
            //foreach (string enumName in enumNames)
            //{
            //  if (String.Compare(name, enumName, true) == 0)
            //  {
            //    docInfoValue = paragraph.Document.Info.GetValue(enumName).ToString();
            //    break;
            //  }
            //}
            //return docInfoValue;
        }

        Area GetShadingArea()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ParagraphFormat format = _paragraph.Format;
            XUnit left = contentArea.X;
            left += format.LeftIndent;
            if (format.FirstLineIndent < 0)
                left += format.FirstLineIndent;

            XUnit top = contentArea.Y;
            XUnit bottom = contentArea.Y + contentArea.Height;
            XUnit right = contentArea.X + contentArea.Width;
            right -= format.RightIndent;

            if (_paragraph.Format._borders != null && !_paragraph.Format._borders.IsNull())
            {
                Borders borders = format.Borders;
                BordersRenderer bordersRenderer = new BordersRenderer(borders, _gfx);

                if (_renderInfo.FormatInfo.IsStarting)
                    top += bordersRenderer.GetWidth(BorderType.Top);
                if (_renderInfo.FormatInfo.IsEnding)
                    bottom -= bordersRenderer.GetWidth(BorderType.Bottom);

                left -= borders.DistanceFromLeft;
                right += borders.DistanceFromRight;
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }

        void RenderShading()
        {
            if (_paragraph.Format._shading == null || _paragraph.Format._shading.IsNull())
                return;

            ShadingRenderer shadingRenderer = new ShadingRenderer(_gfx, _paragraph.Format.Shading);
            Area area = GetShadingArea();

            shadingRenderer.Render(area.X, area.Y, area.Width, area.Height);
        }


        void RenderBorders()
        {
            if (_paragraph.Format._borders == null || _paragraph.Format._borders.IsNull())
                return;

            Area shadingArea = GetShadingArea();
            XUnit left = shadingArea.X;
            XUnit top = shadingArea.Y;
            XUnit bottom = shadingArea.Y + shadingArea.Height;
            XUnit right = shadingArea.X + shadingArea.Width;

            Borders borders = _paragraph.Format.Borders;
            BordersRenderer bordersRenderer = new BordersRenderer(borders, _gfx);
            XUnit borderWidth = bordersRenderer.GetWidth(BorderType.Left);
            if (borderWidth > 0)
            {
                left -= borderWidth;
                bordersRenderer.RenderVertically(BorderType.Left, left, top, bottom - top);
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Right);
            if (borderWidth > 0)
            {
                bordersRenderer.RenderVertically(BorderType.Right, right, top, bottom - top);
                right += borderWidth;
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Top);
            if (_renderInfo.FormatInfo.IsStarting && borderWidth > 0)
            {
                top -= borderWidth;
                bordersRenderer.RenderHorizontally(BorderType.Top, left, top, right - left);
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Bottom);
            if (_renderInfo.FormatInfo.IsEnding && borderWidth > 0)
            {
                bordersRenderer.RenderHorizontally(BorderType.Bottom, left, bottom, right - left);
            }
        }

        XUnit MeasureString(string word)
        {
            XFont xFont = CurrentFont;
            XUnit width = _gfx.MeasureString(word, xFont, StringFormat).Width;
            Font font = CurrentDomFont;

            if (font.Subscript || font.Superscript)
                width *= FontHandler.GetSubSuperScaling(xFont);

            return width;
        }

        XUnit GetSpaceWidth(Character character)
        {
            XUnit width = 0;
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                    width = MeasureString(" ");
                    break;
                case SymbolName.Em:
                    width = MeasureString("m");
                    break;
                case SymbolName.Em4:
                    width = 0.25 * MeasureString("m");
                    break;
                case SymbolName.En:
                    width = MeasureString("n");
                    break;
            }
            return width * character.Count;
        }

        void RenderListSymbol()
        {
            string symbol;
            XFont font;
            if (GetListSymbol(out symbol, out font))
            {
                XBrush brush = FontHandler.FontColorToXBrush(_paragraph.Format.Font);
                _gfx.DrawString(symbol, font, brush, _currentXPosition, CurrentBaselinePosition);
                _currentXPosition += _gfx.MeasureString(symbol, font, StringFormat).Width;
                TabOffset tabOffset = NextTabOffset();
                _currentXPosition += tabOffset.Offset;
                _lastTabPosition = _currentXPosition;
            }
        }

        void FormatListSymbol()
        {
            string symbol;
            XFont font;
            if (GetListSymbol(out symbol, out font))
            {
                _currentVerticalInfo = CalcVerticalInfo(font);
                _currentXPosition += _gfx.MeasureString(symbol, font, StringFormat).Width;
                FormatTab();
            }
        }

        FormatResult FormatSpace(Character character)
        {
            XUnit width = GetSpaceWidth(character);
            return FormatAsWord(width);
        }

        static string GetSymbol(Character character)
        {
            char ch;
            switch (character.SymbolName)
            {
                case SymbolName.Euro:
                    ch = '€';
                    break;

                case SymbolName.Copyright:
                    ch = '©';
                    break;

                case SymbolName.Trademark:
                    ch = '™';
                    break;

                case SymbolName.RegisteredTrademark:
                    ch = '®';
                    break;

                case SymbolName.Bullet:
                    ch = '•';
                    break;

                case SymbolName.Not:
                    ch = '¬';
                    break;
                //REM: Non-breakable blanks are still ignored.
                //        case SymbolName.SymbolNonBreakableBlank:
                //          return "\xA0";
                //          break;

                case SymbolName.EmDash:
                    ch = '—';
                    break;

                case SymbolName.EnDash:
                    ch = '–';
                    break;

                default:
                    char c = character.Char;
                    char[] chars = Encoding.UTF8.GetChars(new byte[] { (byte)c });
                    //#if !SILVERLIGHT
                    //                    char[] chars = System.Text.Encoding.Default.GetChars(new byte[] { (byte)c });
                    //#else
                    //                    char[] chars = System.Text.Encoding.UTF8.GetChars(new byte[] { (byte)c });
                    //#endif
                    ch = chars[0];
                    break;
            }
            string returnString = "";
            returnString += ch;
            int count = character.Count;
            while (--count > 0)
                returnString += ch;
            return returnString;
        }

        FormatResult FormatSymbol(Character character)
        {
            return FormatWord(GetSymbol(character));
        }

        /// <summary>
        /// Processes (measures) a special character within text.
        /// </summary>
        /// <param name="character">The character to process.</param>
        /// <returns>True if the character should start at a new line.</returns>
        FormatResult FormatCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                case SymbolName.Em:
                case SymbolName.Em4:
                case SymbolName.En:
                    return FormatSpace(character);

                case SymbolName.LineBreak:
                    return FormatLineBreak();

                case SymbolName.Tab:
                    return FormatTab();

                default:
                    return FormatSymbol(character);
            }
        }

        /// <summary>
        /// Processes (measures) a blank.
        /// </summary>
        /// <returns>True if the blank causes a line break.</returns>
        FormatResult FormatBlank()
        {
            if (IgnoreBlank())
                return FormatResult.Ignore;

            _savedWordWidth = 0;
            XUnit width = MeasureString(" ");
            VerticalLineInfo newVertInfo = CalcCurrentVerticalInfo();
            Rectangle rect = _formattingArea.GetFittingRect(_currentYPosition, newVertInfo.Height + BottomBorderOffset);
            if (rect == null)
                return FormatResult.NewArea;

            if (width + _currentXPosition <= rect.X + rect.Width + Tolerance)
            {
                _currentXPosition += width;
                _currentVerticalInfo = newVertInfo;
                SaveBlankWidth(width);
                return FormatResult.Continue;
            }
            return FormatResult.NewLine;
        }

        FormatResult FormatLineBreak()
        {
            if (_phase != Phase.Rendering)
                _currentLeaf = _currentLeaf.GetNextLeaf();

            _savedWordWidth = 0;
            return FormatResult.NewLine;
        }

        /// <summary>
        /// Processes a text element during formatting.
        /// </summary>
        /// <param name="text">The text element to measure.</param>
        FormatResult FormatText(Text text)
        {
            return FormatWord(text.Content);
        }

        FormatResult FormatSoftHyphen()
        {
            if (_currentLeaf.Current == _startLeaf.Current)
                return FormatResult.Continue;

            ParagraphIterator nextIter = _currentLeaf.GetNextLeaf();
            ParagraphIterator prevIter = _currentLeaf.GetPreviousLeaf();
            // nextIter can be null if the soft hyphen is at the end of a paragraph. To prevent a crash, we jump out if nextIter is null.
            if (!IsWordLikeElement(prevIter.Current) || nextIter == null || !IsWordLikeElement(nextIter.Current))
                return FormatResult.Continue;

            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------
            _currentLeaf = nextIter;
            FormatResult result = FormatElement(nextIter.Current);

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            if (result == FormatResult.Continue)
                return FormatResult.Continue;

            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            Rectangle fittingRect = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);

            XUnit hyphenWidth = MeasureString("-");
            if (xPosition + hyphenWidth <= fittingRect.X + fittingRect.Width + Tolerance
                // If one word fits, but not the hyphen, the formatting must continue with the next leaf
                || prevIter.Current == _startLeaf.Current)
            {
                // For Tabs in justified context
                if (!IgnoreHorizontalGrowth)
                {
                    _currentWordsWidth += hyphenWidth;
                    _currentLineWidth += hyphenWidth;
                }
                _currentLeaf = nextIter;
                return FormatResult.NewLine;
            }
            else
            {
                _currentWordsWidth -= _savedWordWidth;
                _currentLineWidth -= _savedWordWidth;
                _currentLineWidth -= GetPreviousBlankWidth(prevIter);
                _currentLeaf = prevIter;
                return FormatResult.NewLine;
            }
        }

        XUnit GetPreviousBlankWidth(ParagraphIterator beforeIter)
        {
            XUnit width = 0;
            ParagraphIterator savedIter = _currentLeaf;
            _currentLeaf = beforeIter.GetPreviousLeaf();
            while (_currentLeaf != null)
            {
                if (_currentLeaf.Current is BookmarkField)
                    _currentLeaf = _currentLeaf.GetPreviousLeaf();
                else if (IsBlank(_currentLeaf.Current))
                {
                    if (!IgnoreBlank())
                        width = CurrentWordDistance;

                    break;
                }
                else
                    break;
            }
            _currentLeaf = savedIter;
            return width;
        }

        void HandleNonFittingLine()
        {
            if (_currentLeaf != null)
            {
                if (_savedWordWidth > 0)
                {
                    _currentWordsWidth = _savedWordWidth;
                    _currentLineWidth = _savedWordWidth;
                }
                _currentLeaf = _currentLeaf.GetNextLeaf();
                _currentYPosition += _currentVerticalInfo.Height;
                _currentVerticalInfo = new VerticalLineInfo();
            }
        }

        /// <summary>
        /// Starts a new line by resetting measuring values.
        /// Do not call before the first first line is formatted!
        /// </summary>
        /// <returns>True, if the new line may fit the formatting area.</returns>
        bool StartNewLine()
        {
            _tabOffsets = new List<TabOffset>();
            _lastTab = null;
            _lastTabPosition = 0;
            _currentYPosition += _currentVerticalInfo.Height;

            Rectangle rect = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height + BottomBorderOffset);
            if (rect == null)
                return false;

            _isFirstLine = false;
            _currentXPosition = StartXPosition; // depends on "currentVerticalInfo"
            _currentVerticalInfo = new VerticalLineInfo();
            _currentVerticalInfo = CalcCurrentVerticalInfo();
            _startLeaf = _currentLeaf;
            _currentBlankCount = 0;
            _currentWordsWidth = 0;
            _currentLineWidth = 0;
            return true;
        }
        /// <summary>
        /// Stores all line information.
        /// </summary>
        void StoreLineInformation()
        {
            PopSavedBlankWidth();

            XUnit topBorderOffset = TopBorderOffset;
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            if (topBorderOffset > 0)//May only occure for the first line.
                contentArea = _formattingArea.GetFittingRect(_formattingArea.Y, topBorderOffset);

            if (contentArea == null)
                contentArea = _formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height);
            else
                contentArea = contentArea.Unite(_formattingArea.GetFittingRect(_currentYPosition, _currentVerticalInfo.Height));

            XUnit bottomBorderOffset = BottomBorderOffset;
            if (bottomBorderOffset > 0)
                contentArea = contentArea.Unite(_formattingArea.GetFittingRect(_currentYPosition + _currentVerticalInfo.Height, bottomBorderOffset));

            LineInfo lineInfo = new LineInfo();
            lineInfo.Vertical = _currentVerticalInfo;

            if (_startLeaf != null && _startLeaf == _currentLeaf)
                HandleNonFittingLine();

            lineInfo.LastTab = _lastTab;
            _renderInfo.LayoutInfo.ContentArea = contentArea;

            lineInfo.StartIter = _startLeaf;

            if (_currentLeaf == null)
                lineInfo.EndIter = new ParagraphIterator(_paragraph.Elements).GetLastLeaf();
            else
                lineInfo.EndIter = _currentLeaf.GetPreviousLeaf();

            lineInfo.BlankCount = _currentBlankCount;

            lineInfo.WordsWidth = _currentWordsWidth;

            lineInfo.LineWidth = _currentLineWidth;
            lineInfo.TabOffsets = _tabOffsets;
            lineInfo.ReMeasureLine = _reMeasureLine;

            _savedWordWidth = 0;
            _reMeasureLine = false;
            ((ParagraphFormatInfo)_renderInfo.FormatInfo).AddLineInfo(lineInfo);
        }

        /// <summary>
        /// Gets the top border offset for the first line, else 0.
        /// </summary>
        XUnit TopBorderOffset
        {
            get
            {
                XUnit offset = 0;
                if (_isFirstLine && _paragraph.Format._borders != null && !_paragraph.Format._borders.IsNull())
                {
                    offset += _paragraph.Format.Borders.DistanceFromTop;
                    if (_paragraph.Format._borders != null && !_paragraph.Format._borders.IsNull())
                    {
                        BordersRenderer bordersRenderer = new BordersRenderer(_paragraph.Format.Borders, _gfx);
                        offset += bordersRenderer.GetWidth(BorderType.Top);
                    }
                }
                return offset;
            }
        }

        bool IsLastVisibleLeaf
        {
            get
            {
                // REM: Code is missing here for blanks, bookmarks etc. which might be invisible.
                if (_currentLeaf.IsLastLeaf)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// Gets the bottom border offset for the last line, else 0.
        /// </summary>
        XUnit BottomBorderOffset
        {
            get
            {
                XUnit offset = 0;
                //while formatting, it is impossible to determine whether we are in the last line until the last visible leaf is reached.
                if ((_phase == Phase.Formatting && (_currentLeaf == null || IsLastVisibleLeaf))
                  || (_phase == Phase.Rendering && (_isLastLine)))
                {
                    if (_paragraph.Format._borders != null && !_paragraph.Format._borders.IsNull())
                    {
                        offset += _paragraph.Format.Borders.DistanceFromBottom;
                        BordersRenderer bordersRenderer = new BordersRenderer(_paragraph.Format.Borders, _gfx);
                        offset += bordersRenderer.GetWidth(BorderType.Bottom);
                    }
                }
                return offset;
            }
        }

        VerticalLineInfo CalcCurrentVerticalInfo()
        {
            return CalcVerticalInfo(CurrentFont);
        }

        VerticalLineInfo CalcVerticalInfo(XFont font)
        {
            ParagraphFormat paragraphFormat = _paragraph.Format;
            LineSpacingRule spacingRule = paragraphFormat.LineSpacingRule;
            XUnit lineHeight = 0;

            XUnit descent = FontHandler.GetDescent(font);
            descent = Math.Max(_currentVerticalInfo.Descent, descent);

            XUnit singleLineSpace = font.GetHeight();
            RenderInfo imageRenderInfo = CurrentImageRenderInfo;
            if (imageRenderInfo != null)
                singleLineSpace = singleLineSpace - FontHandler.GetAscent(font) + imageRenderInfo.LayoutInfo.ContentArea.Height;

            XUnit inherentLineSpace = Math.Max(_currentVerticalInfo.InherentlineSpace, singleLineSpace);
            switch (spacingRule)
            {
                case LineSpacingRule.Single:
                    lineHeight = singleLineSpace;
                    break;

                case LineSpacingRule.OnePtFive:
                    lineHeight = 1.5 * singleLineSpace;
                    break;

                case LineSpacingRule.Double:
                    lineHeight = 2.0 * singleLineSpace;
                    break;

                case LineSpacingRule.Multiple:
                    lineHeight = _paragraph.Format.LineSpacing * singleLineSpace;
                    break;

                case LineSpacingRule.AtLeast:
                    lineHeight = Math.Max(singleLineSpace, _paragraph.Format.LineSpacing);
                    break;

                case LineSpacingRule.Exactly:
                    lineHeight = new XUnit(_paragraph.Format.LineSpacing);
                    inherentLineSpace = _paragraph.Format.LineSpacing.Point;
                    break;
            }
            lineHeight = Math.Max(_currentVerticalInfo.Height, lineHeight);
            if (MaxElementHeight > 0)
                lineHeight = Math.Min(MaxElementHeight - Tolerance, lineHeight);

            return new VerticalLineInfo(lineHeight, descent, inherentLineSpace);
        }

        /// <summary>
        /// The font used for the current paragraph element.
        /// </summary>
        private XFont CurrentFont
        {
            get { return FontHandler.FontToXFont(CurrentDomFont, /*_documentRenderer.PrivateFonts,*/ _gfx.MUH); }
        }

        private Font CurrentDomFont
        {
            get
            {
                if (_currentLeaf != null)
                {
                    DocumentObject parent = DocumentRelations.GetParent(_currentLeaf.Current);
                    parent = DocumentRelations.GetParent(parent);
                    
                    FormattedText formattedText = parent as FormattedText;
                    if (formattedText != null)
                        return formattedText.Font;
                    
                    Hyperlink hyperlink = parent as Hyperlink;
                    if (hyperlink != null)
                        return hyperlink.Font;
                }
                return _paragraph.Format.Font;
            }
        }

        /// <summary>
        /// Help function to receive a line height on empty paragraphs.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="gfx">The GFX.</param>
        /// <param name="renderer">The renderer.</param>
        internal static XUnit GetLineHeight(ParagraphFormat format, XGraphics gfx, DocumentRenderer renderer)
        {
            XFont font = FontHandler.FontToXFont(format.Font, /*renderer.PrivateFonts,*/ gfx.MUH);
            XUnit singleLineSpace = font.GetHeight();
            switch (format.LineSpacingRule)
            {
                case LineSpacingRule.Exactly:
                    return format.LineSpacing.Point;

                case LineSpacingRule.AtLeast:
                    return Math.Max(format.LineSpacing.Point, font.GetHeight());  // old: GetHeight(gfx));

                case LineSpacingRule.Multiple:
                    return format.LineSpacing * format.Font.Size;

                case LineSpacingRule.OnePtFive:
                    return 1.5 * singleLineSpace;

                case LineSpacingRule.Double:
                    return 2.0 * singleLineSpace;

                case LineSpacingRule.Single:
                default:
                    return singleLineSpace;
            }
        }

        void RenderUnderline(XUnit width, bool isWord)
        {
            XPen pen = GetUnderlinePen(isWord);

            bool penChanged = UnderlinePenChanged(pen);
            if (penChanged)
            {
                if (_currentUnderlinePen != null)
                    EndUnderline(_currentUnderlinePen, _currentXPosition);

                if (pen != null)
                    StartUnderline(_currentXPosition);

                _currentUnderlinePen = pen;
            }

            if (_currentLeaf.Current == _endLeaf.Current)
            {
                if (_currentUnderlinePen != null)
                    EndUnderline(_currentUnderlinePen, _currentXPosition + width);

                _currentUnderlinePen = null;
            }
        }

        void StartUnderline(XUnit xPosition)
        {
            _underlineStartPos = xPosition;
        }

        void EndUnderline(XPen pen, XUnit xPosition)
        {
            XUnit yPosition = CurrentBaselinePosition;
            yPosition += 0.33 * _currentVerticalInfo.Descent;
            _gfx.DrawLine(pen, _underlineStartPos, yPosition, xPosition, yPosition);
        }

        XPen _currentUnderlinePen;
        XUnit _underlineStartPos;

        bool UnderlinePenChanged(XPen pen)
        {
            if (pen == null && _currentUnderlinePen == null)
                return false;

            if (pen == null && _currentUnderlinePen != null)
                return true;

            if (pen != null && _currentUnderlinePen == null)
                return true;

            if (pen != null && pen.Color != _currentUnderlinePen.Color)
                return true;

            return pen.Width != _currentUnderlinePen.Width;
        }

        RenderInfo CurrentImageRenderInfo
        {
            get
            {
                if (_currentLeaf != null && _currentLeaf.Current is Image)
                {
                    Image image = (Image)_currentLeaf.Current;
                    if (_imageRenderInfos != null && _imageRenderInfos.ContainsKey(image))
                        return _imageRenderInfos[image];
                    else
                    {
                        if (_imageRenderInfos == null)
                            _imageRenderInfos = new Dictionary<Image, RenderInfo>();

                        RenderInfo renderInfo = CalcImageRenderInfo(image);
                        _imageRenderInfos.Add(image, renderInfo);
                        return renderInfo;
                    }
                }
                return null;
            }
        }
        XPen GetUnderlinePen(bool isWord)
        {
            Font font = CurrentDomFont;
            Underline underlineType = font.Underline;
            if (underlineType == Underline.None)
                return null;

            if (underlineType == Underline.Words && !isWord)
                return null;

#if noCMYK
      XPen pen = new XPen(XColor.FromArgb(font.Color.Argb), font.Size / 16);
#else
            XPen pen = new XPen(ColorHelper.ToXColor(font.Color, _paragraph.Document.UseCmykColor), font.Size / 16);
#endif
            switch (font.Underline)
            {
                case Underline.DotDash:
                    pen.DashStyle = XDashStyle.DashDot;
                    break;

                case Underline.DotDotDash:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    break;

                case Underline.Dash:
                    pen.DashStyle = XDashStyle.Dash;
                    break;

                case Underline.Dotted:
                    pen.DashStyle = XDashStyle.Dot;
                    break;

                case Underline.Single:
                default:
                    pen.DashStyle = XDashStyle.Solid;
                    break;
            }
            return pen;
        }

        private static XStringFormat StringFormat
        {
            get { return _stringFormat ?? (_stringFormat = XStringFormats.Default); }
        }

        /// <summary>
        /// The paragraph to format or render.
        /// </summary>
        readonly Paragraph _paragraph;
        XUnit _currentWordsWidth;
        int _currentBlankCount;
        XUnit _currentLineWidth;
        bool _isFirstLine;
        bool _isLastLine;
        VerticalLineInfo _currentVerticalInfo;
        Area _formattingArea;
        XUnit _currentYPosition;
        XUnit _currentXPosition;
        ParagraphIterator _currentLeaf;
        ParagraphIterator _startLeaf;
        ParagraphIterator _endLeaf;
        static XStringFormat _stringFormat;
        bool _reMeasureLine;
        XUnit _minWidth = 0;
        Dictionary<Image, RenderInfo> _imageRenderInfos;
        List<TabOffset> _tabOffsets;
        DocumentObject _lastTab;
        bool _lastTabPassed;
        XUnit _lastTabPosition;
    }
}
