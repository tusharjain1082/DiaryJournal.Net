using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml.Xsl;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Markup.Primitives;
using DiaryJournal.Net;

namespace System.Windows.Controls
{
    public static class DependencyObjectHelper
    {
        public static List<DependencyProperty> GetDependencyProperties(Object element)
        {
            List<DependencyProperty> properties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.DependencyProperty != null)
                    {
                        properties.Add(mp.DependencyProperty);
                    }
                }
            }

            return properties;
        }

        public static List<DependencyProperty> GetAttachedProperties(Object element)
        {
            List<DependencyProperty> attachedProperties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.IsAttached)
                    {
                        attachedProperties.Add(mp.DependencyProperty);
                    }
                }
            }

            return attachedProperties;
        }
    }

    public class WpfRichTextBoxEx : RichTextBox
    {
        public TextSelection? sel;
        public RichTextBox? dummy = new RichTextBox();

        public const int MAX_TABLE_CELLS = 63;

        public WpfRichTextBoxEx()
            : base()
        {
            IsInactiveSelectionHighlightEnabled = dummy.IsInactiveSelectionHighlightEnabled = true;
            this.LostFocus += WpfRichTextBoxEx_LostFocus;
        }
        private void WpfRichTextBoxEx_LostFocus(object sender, RoutedEventArgs e)
        {
            this.sel = this.Selection;
            e.Handled = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.sel = this.Selection;
            e.Handled = true;
        }

        public Block? ObjectAtPosition(TextPointer ptr)
        {
            var curCaret = this.CaretPosition;
            Block? block = this.Document.Blocks.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
            return block;
        }
        public Block? ObjectAtCaretPosition()
        {
            TextPointer curCaret = this.CaretPosition;
            return ObjectAtPosition(curCaret);
        }
        public Block? ObjectAtSelectionStart()
        {
            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            return ObjectAtPosition(selection.Start);
        }
        public bool WpfBoldToWinFormsBold(FontWeight bold)
        {
            return (bold == FontWeights.Bold);
        }
        public FontWeight WinFormsBoldToWpfBold(bool bold)
        {
            if (bold)
                return FontWeights.Bold;
            else
                return FontWeights.Normal;
        }
        public bool WpfItalicToWinFormsItalic(FontStyle italic)
        {
            return (italic == FontStyles.Italic);
        }
        public FontStyle WinFormsItalicToWpfItalic(bool italic)
        {
            if (italic)
                return FontStyles.Italic;
            else
                return FontStyles.Normal;
        }

        public bool Bold
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(TextElement.FontWeightProperty);
                if (value is null) return false;
                FontWeight bold = ((value == DependencyProperty.UnsetValue) ? FontWeights.Normal : (FontWeight)value);
                if (bold == FontWeights.Bold)
                    return true;
                else
                    return false;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);//rtb is the name of the richtextbox
                if (value)
                    tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                else
                    tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
        }
        public bool Italic
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(TextElement.FontStyleProperty);
                if (value is null) return false;
                FontStyle italic = ((value == DependencyProperty.UnsetValue) ? FontStyles.Normal : (FontStyle)value);
                if (italic == FontStyles.Italic)
                    return true;
                else
                    return false;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);//rtb is the name of the richtextbox
                if (value)
                    tr.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                else
                    tr.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);

            }
        }
        public bool Underline
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                bool underline = false;
                bool strikeout = false;
                GetDecorations(selection, ref underline, ref strikeout);
                return underline;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                toggleSelectionDecorations(selection, value, false, true, false);
            }
        }
        public bool Strikeout
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                bool underline = false;
                bool strikeout = false;
                GetDecorations(selection, ref underline, ref strikeout);
                return strikeout;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                toggleSelectionDecorations(selection, false, value, false, true);
            }
        }
        public TextDecorationCollection? GetDecorations(TextRange selection, ref bool underline, ref bool strikeout)
        {
            TextPointer? startPos = GoToPoint(selection.Start, 0); //GetPoint(start, 0);//selection.Start.GetPositionAtOffset(offset);
            TextPointer? endPos = GoToPoint(startPos, 1);//GetPoint(start, 1);// selection.Start.GetPositionAtOffset(offset + 1);
            TextRange range = new TextRange(startPos, endPos);

            // get selected char's decorations
            Object? obj = GetDecorations(range);
            TextDecorationCollection? decors = (TextDecorationCollection?)obj;
            if (decors == null)
                return null;

            foreach (TextDecoration textDecoration in decors)
            {
                if (textDecoration.Location == TextDecorationLocation.Underline)
                    underline = true;
                else if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                    strikeout = true;
            }
            return decors;
        }
        public TextDecorationCollection? GetDecorations(Object? element, ref bool underline, ref bool strikeout)
        {
            underline = strikeout = false;
            TextDecorationCollection? decorations = null;

            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // get
                decorations = para.TextDecorations;
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                // section has no decoration config
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                //get
                decorations = text.TextDecorations;
            }
            else
            {
            }

            // detect decorations
            if (decorations != null)
            {
                foreach (TextDecoration textDecoration in decorations)
                {
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        underline = true;
                    else if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        strikeout = true;
                }
            }
            return decorations;
        }
        public TextDecorationCollection? GetDecorations(TextRange selection)
        {
            Object? obj = selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (obj == DependencyProperty.UnsetValue) return null;

            TextDecorationCollection? decorations = (TextDecorationCollection?)obj;
            
            Object? entity = selection.Start.Parent;
            TextElement? element = null;
            if (entity is not FlowDocument)
                element = (TextElement)selection.Start.Parent;

            if ((decorations == null) || (decorations.Count == 0))
            {
                while (element != null)
                {
                    bool underline = false;
                    bool strikeout = false;
                    decorations = GetDecorations(element, ref underline, ref strikeout);
                    if ((decorations != null) && (decorations.Count > 0)) break;

                    if (element.Parent is FlowDocument)
                        break;
                    else
                        element = (TextElement)element.Parent;
                }
            }
            return decorations;
        }
        public TextPointer GoToPoint(TextPointer start, int x)
        {
            TextPointer @out = start;
            int i = 0;
            while (i < x)
            {
                if (@out.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text || @out.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                    i += 1;
                if (@out.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return @out;
                else
                    @out = @out.GetPositionAtOffset(1, LogicalDirection.Forward);
            }
            return @out;
        }
        public bool findSelectionDecorations(TextRange selection, ref bool outUnderline, ref bool outStrikeout)
        {
            TextPointer start = selection.Start;
            int offset = 0;// Document.ContentStart.GetOffsetToPosition(selection.Start);

            int done = 0;
            int total = selection.Text.Count();
            if (total == 0) total += 1;

            while (done < total)
            {
                TextPointer? startPos = GoToPoint(start, offset); //GetPoint(start, 0);//selection.Start.GetPositionAtOffset(offset);
                TextPointer? endPos = GoToPoint(start, offset + 1);//GetPoint(start, 1);// selection.Start.GetPositionAtOffset(offset + 1);
                TextRange range = new TextRange(startPos, endPos);

                // get selected char's decorations
                bool underline = false;
                bool strikeout = false;
                Object? obj = GetDecorations(range);
                TextDecorationCollection? decors = (TextDecorationCollection?)obj;
                if (decors == null)
                {
                    // increment counters and positions
                    offset++;
                    done++;
                    continue;
                }
                foreach (TextDecoration textDecoration in decors)
                {
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        underline = true;
                    else if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        strikeout = true;
                }

                if (!outUnderline && underline)
                    outUnderline = true;

                if (!outStrikeout && strikeout)
                    outStrikeout = true;

                // if we find both decorations, abort and return.
                if (outUnderline && outStrikeout)
                    return true;

                // increment counters and positions
                offset++;
                done++;
            }
            return true;
        }
        public bool toggleSelectionAllDecorations(TextRange selection, bool underline, bool strikeout)
        {
            return toggleSelectionDecorations(selection, underline, strikeout, true, true);
        }
        public bool toggleSelectionDecorations(TextRange selection, bool underlineSet, bool strikeoutSet, bool underlineParam, bool strikeoutParam)
        {
            TextPointer start = selection.Start;
            int offset = 0;// Document.ContentStart.GetOffsetToPosition(selection.Start);

            int done = 0;
            int total = selection.Text.Count();
            if (total == 0) total += 1;

            while (done < total)
            {
                TextPointer? startPos = GetPoint(start, offset); //GetPoint(start, 0);//selection.Start.GetPositionAtOffset(offset);
                TextPointer? endPos = GetPoint(startPos, 1);//GetPoint(start, 1);// selection.Start.GetPositionAtOffset(offset + 1);
                TextRange range = new TextRange(startPos, endPos);

                // get selected char's decorations
                bool underline = false;
                bool strikeout = false;
                Object? obj = GetDecorations(range);
                TextDecorationCollection? decors = (TextDecorationCollection?)obj;
                if (decors == null)
                    decors = new TextDecorationCollection();

                foreach (TextDecoration textDecoration in decors)
                {
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        underline = true;
                    else if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        strikeout = true;
                }

                if (underlineParam)
                {
                    if (underlineSet)
                    {
                        decors = decors.Contains(TextDecorations.Underline.First()) ? decors : new TextDecorationCollection(decors.Union(TextDecorations.Underline));
                    }
                    else if (!underlineSet)
                    {
                        decors = decors.Contains(TextDecorations.Underline.First()) ? new TextDecorationCollection(decors.Except(TextDecorations.Underline)) : decors;
                    }
                }

                if (strikeoutParam)
                {
                    if (strikeoutSet)
                    {
                        decors = decors.Contains(TextDecorations.Strikethrough.First()) ? decors : new TextDecorationCollection(decors.Union(TextDecorations.Strikethrough));

                    }
                    else if (!strikeoutSet)
                    {
                        decors = decors.Contains(TextDecorations.Strikethrough.First()) ? new TextDecorationCollection(decors.Except(TextDecorations.Strikethrough)) : decors;
                    }
                }
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, decors);

                // increment counters and positions
                offset++;
                done++;
            }

            return true;
        }
        /*
        private static TextPointer GetPoint(TextPointer start, int x)
        {
            var ret = start;
            var i = 0;
            while (i < x && ret != null)
            {
                if (ret.GetPointerContext(LogicalDirection.Backward) ==
        TextPointerContext.Text ||
                    ret.GetPointerContext(LogicalDirection.Backward) ==
        TextPointerContext.None)
                    i++;
                if (ret.GetPositionAtOffset(1,
        LogicalDirection.Forward) == null)
                    return ret;
                ret = ret.GetPositionAtOffset(1,
        LogicalDirection.Forward);
            }
            return ret;
        }
        */
        private static TextPointer GetPoint(TextPointer start, int x)
        {
            var ret = start;
            var i = 0;
            while (ret != null)
            {
                string stringSoFar = new TextRange(ret, ret.GetPositionAtOffset(i, LogicalDirection.Forward)).Text;
                if (stringSoFar.Length == x)
                    break;

                i++;

                if (ret.GetPositionAtOffset(i, LogicalDirection.Forward) == null)
                    return ret.GetPositionAtOffset(i - 1, LogicalDirection.Forward);

            }
            ret = ret.GetPositionAtOffset(i, LogicalDirection.Forward);
            return ret;
        }
        public void SetDecorations(Object? element, TextDecorationCollection decoration, bool set)
        {
            if (element == null) return;

            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                if (set)
                {
                    para.TextDecorations = para.TextDecorations.Contains(decoration.First())
                        ? para.TextDecorations
                        : new TextDecorationCollection(para.TextDecorations.Union(decoration));
                }
                else
                {
                    para.TextDecorations = para.TextDecorations.Contains(decoration.First())
                        ? new TextDecorationCollection(para.TextDecorations.Except(decoration))
                        : para.TextDecorations;
                }

            }
            else if (element is Run || element is Inline || element is Span)
            {
                Inline text = (Inline)element;

                // set decorations
                if (set)
                {
                    text.TextDecorations = text.TextDecorations.Contains(decoration.First())
                        ? text.TextDecorations
                        : new TextDecorationCollection(text.TextDecorations.Union(decoration));
                }
                else
                {
                    text.TextDecorations = text.TextDecorations.Contains(decoration.First())
                        ? new TextDecorationCollection(text.TextDecorations.Except(decoration)) 
                        : text.TextDecorations;
                }
            }
            else
            {
            }
        }
        public void SetDecorations(TextRange textRange, TextDecorationCollection decoration, bool set)
        {
            Object? value = textRange.GetPropertyValue(Inline.TextDecorationsProperty);
            if (value == null) return;
            TextDecorationCollection? decorations = (TextDecorationCollection?)value;
            if (set)
            {
                decorations = decorations.Contains(decoration.First()) ? decorations : new TextDecorationCollection(decorations.Union(decoration));
            }
            else
            {
                decorations = decorations.Contains(decoration.First()) ? new TextDecorationCollection(decorations.Except(decoration)) : decorations;
            }
            textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
        }
        public double SelectionIndent
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(Paragraph.TextIndentProperty);
                if (value is null) return 0;
                double indent = ((value == DependencyProperty.UnsetValue) ? 0 : (double)value);
                return indent;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                selection.ApplyPropertyValue(
                    Paragraph.TextIndentProperty,
                    (double)value /*pixels to indent by*/
                );

            }
        }
        public double SelectionFontSize
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(TextElement.FontSizeProperty);
                if (value is null) return 14.0;
                if (value != DependencyProperty.UnsetValue)
                    return (double)value;
                else
                    return 14.0;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);//rtb is the name of the richtextbox
                tr.ApplyPropertyValue(TextElement.FontSizeProperty, value);
            }
        }

        public System.Drawing.Font SelectionFont
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(TextElement.FontFamilyProperty);
                Font defaultValue = new Font("Times New Roman", 14);
                if (value is null) return defaultValue;
                System.Windows.Media.FontFamily font = ((value == DependencyProperty.UnsetValue)
                    ? new System.Windows.Media.FontFamily("Times New Roman")
                    : (System.Windows.Media.FontFamily)value);
                return new Font(((System.Windows.Media.FontFamily)font).Source, 14);
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);//rtb is the name of the richtextbox
                System.Windows.Media.FontFamily font = new Media.FontFamily(value.Name);
                tr.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
            }
        }
        public System.Drawing.Font WpfFontToWinFormsFont(System.Windows.Media.FontFamily font)
        {
            return new Drawing.Font(font.Source, 14, Drawing.FontStyle.Regular);
        }
        public System.Windows.Media.FontFamily WinFormsFontToWpfFont(System.Drawing.Font font)
        {
            return new System.Windows.Media.FontFamily(font.Name);
        }
        public System.Drawing.Color SelectionColor
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                System.Drawing.Color defaultValue = System.Drawing.Color.Transparent;
                System.Windows.Media.Brush brush = GetForeground(selection);
                return WpfBrushToWinFormsColor((SolidColorBrush)brush);
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                System.Windows.Media.Brush brush = WinFormsColorToWpfBrush(value);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }
        }
        public System.Drawing.Color SelectionBackColor
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                System.Drawing.Color defaultValue = System.Drawing.Color.Transparent;
                System.Windows.Media.Brush brush = GetBackground(selection);
                return WpfBrushToWinFormsColor((SolidColorBrush)brush);
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                System.Windows.Media.Brush brush = WinFormsColorToWpfBrush(value);
                tr.ApplyPropertyValue(TextElement.BackgroundProperty, brush);
            }
        }
        public System.Windows.Media.Brush? GetBackground(TextRange selection)
        {
            Object? value = selection.GetPropertyValue(TextElement.BackgroundProperty);
            System.Windows.Media.Brush? brush = (System.Windows.Media.Brush)value;// new SolidColorBrush(Colors.Transparent);

            TextElement element = (TextElement)selection.Start.Parent;
            if (brush == null)
            {
                while (element != null) 
                {
                    brush = (System.Windows.Media.Brush)element.Background;
                    if (brush != null) break;

                    element = (TextElement)element.Parent;
                }
            }
            return brush;
        }
        public System.Windows.Media.Brush? GetForeground(TextRange selection)
        {
            Object? value = selection.GetPropertyValue(TextElement.ForegroundProperty);
            System.Windows.Media.Brush? brush = (System.Windows.Media.Brush)value;// new SolidColorBrush(Colors.Transparent);

            TextElement element = (TextElement)selection.Start.Parent;
            if (brush == null)
            {
                while (element != null)
                {
                    brush = (System.Windows.Media.Brush)element.Foreground;
                    if (brush != null) break;

                    element = (TextElement)element.Parent;
                }
            }
            return brush;
        }

        public TextAlignment SelectionAlignment
        {
            get 
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                Object? value = tr.GetPropertyValue(FlowDocument.TextAlignmentProperty);
                if (value is null) return TextAlignment.Left;
                TextAlignment align = ((value == DependencyProperty.UnsetValue)
                    ? TextAlignment.Left
                    : (TextAlignment)value);
                return align;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange tr = new TextRange(selection.Start, selection.End);
                tr.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, value);
            }

        }
        public System.Drawing.Color WpfColorToWinFormsColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }
        public System.Windows.Media.Color WinFormsColorToWpfColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(
                (byte)color.A,
                (byte)color.R,
                (byte)color.G,
                (byte)color.B);
        }
        public static System.Drawing.Color WpfBrushToWinFormsColor(System.Windows.Media.SolidColorBrush br)
        {
            return System.Drawing.Color.FromArgb(
                br.Color.A,
                br.Color.R,
                br.Color.G,
                br.Color.B);
        }
        public static SolidColorBrush WinFormsColorToWpfBrush(System.Drawing.Color color)
        {
            System.Windows.Media.Color destColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            return new SolidColorBrush(destColor);
        }
        public bool SelectionBullets
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange range = new TextRange(selection.Start, selection.End);
                TextMarkerStyle markerStyle = GetSelectionListType(range);
                if (markerStyle == TextMarkerStyle.Disc)
                    return true;
                else
                    return false;
            }
        }
        public bool SelectionNumbering
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextRange range = new TextRange(selection.Start, selection.End);
                TextMarkerStyle markerStyle = GetSelectionListType(range);
                if (markerStyle == TextMarkerStyle.Decimal)
                    return true;
                else
                    return false;
            }
        }

        private List FindListAncestor(DependencyObject element)
        {
            while (element != null)
            {
                List list = element as List;
                if (list != null)
                {
                    return list;
                }

                element = LogicalTreeHelper.GetParent(element);
            }

            return null;
        }

        private TextMarkerStyle GetSelectionListType(TextRange selection)
        {
            List list = FindListAncestor(selection.Start.Parent);
            if (list != null)
            {
                if (list.MarkerStyle == TextMarkerStyle.Disc)
                {
                    // bullets  
                }
                else if (list.MarkerStyle == TextMarkerStyle.Decimal)
                {
                    // numbers  
                }
                return list.MarkerStyle;
            }
            return TextMarkerStyle.None;
        }
        public string Text
        {
            get
            {
                TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                this.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                this.Document.ContentEnd
                );

                // The Text property on a TextRange object returns a string
                // representing the plain text content of the TextRange.
                return textRange.Text;

            }
        }

        public String Rtf
        {
            get
            {
                return ToRtf();
            }
            set
            {
                FromRtf(value);
            }
        }
        public String Xaml
        {
            get
            {
                return ToXaml();
            }
            set
            {
                FromXaml(value);
            }
        }
        public String SelectedRtf
        {
            get
            {
                return SelectionToRtf();
            }
            set
            {
                RtfToSelection(value);
            }
        }
        public String SelectedXaml
        {
            get
            {
                return SelectionToXaml();
            }
            set
            {
                XamlToSelection(value);
            }
        }
        public String SelectedBlock
        {
            get
            {
                return BlockToXaml(ObjectAtSelectionStart());
            }
            set
            {
                XamlToSelectedBlock(value);
            }
        }

        public TextPointer SelectionStart
        {
            get
            {
                return Selection.Start;
            }
            set
            {
                Selection.Select(value, value);
            }
        }
        public int SelectionStartOffset
        {
            get
            {
                return this.Document.ContentStart.GetOffsetToPosition(SelectionStart);
            }
            set
            {
                TextPointer ptr = this.Document.ContentStart.GetPositionAtOffset(value);
                if (ptr != null) this.Selection.Select(ptr, ptr);
            }
        }
        public TextPointer SelectionEnd
        {
            get
            {
                return Selection.End;
            }
            set
            {
                Selection.Select(SelectionStart, value);
            }
        }
        public int SelectionEndOffset
        {
            get
            {
                return this.Document.ContentStart.GetOffsetToPosition(SelectionEnd);
            }
            set
            {
                TextPointer start = SelectionStart;
                TextPointer end = this.Document.ContentStart.GetPositionAtOffset(value);
                if (end != null) Selection.Select(start, end);
            }
        }
        public int SelectionLength
        {
            get
            {
                int start = SelectionStartOffset; //this.Document.ContentStart.GetOffsetToPosition(this.Selection.Start);
                int end = SelectionEndOffset;//this.Document.ContentStart.GetOffsetToPosition(this.Selection.End);
                return (end - start);
            }
            set
            {
                int start = SelectionStartOffset; //this.Document.ContentStart.GetOffsetToPosition(this.Selection.Start);
                int end = start + value;
                TextPointer endptr = this.Document.ContentStart.GetPositionAtOffset(end);
                Selection.Select(SelectionStart, endptr);
            }
        }

        public int LineIndex
        {
            get
            {
                int lineNumber;
                this.CaretPosition.GetLineStartPosition(-int.MaxValue, out lineNumber);
                return lineNumber;
            }
        }
        public int ColumnIndex
        {
            get
            {
                int lineNumber;
                this.CaretPosition.GetLineStartPosition(-int.MaxValue, out lineNumber);
                int columnNumber = this.CaretPosition.GetLineStartPosition(0).GetOffsetToPosition(this.CaretPosition);
                if (lineNumber == 0)
                    columnNumber--;

                return columnNumber;
            }
        }
        public String BlockToRtf(Block? block)
        {
            if (block == null) return "";

            //    System.Windows.Documents.FlowDocument doc = this.Document;
            dummy.Document.Blocks.Clear();
            dummy.Document.Blocks.Add(block);
            TextRange content = new System.Windows.Documents.TextRange(block.ElementStart, block.ElementEnd);

            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, DataFormats.Rtf, true);
            stream.Flush();
            stream.Position = 0;
            String rtf = System.Text.Encoding.UTF8.GetString(stream.ToArray()); //reader.ReadToEnd();
            stream.Close();
            stream.Dispose();
            return rtf;
        }
        public String BlockToXaml(Block? block)
        {
            if (block == null) return "";

            dummy.Document.Blocks.Clear();
            dummy.Document.Blocks.Add(block);
            TextRange content = new System.Windows.Documents.TextRange(block.ElementStart, block.ElementEnd);

            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, DataFormats.XamlPackage, true);
            stream.Flush();
            stream.Position = 0;
            byte[] bytes = stream.ToArray();
            stream.Close();
            stream.Dispose();
            return Convert.ToBase64String(bytes);
        }
        public String RtbDocumentToXaml(RichTextBox rtb)
        {
            //TextRange content = new System.Windows.Documents.TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            XamlWriter.Save(rtb.Document, stream);
            //content.Save(stream, DataFormats.XamlPackage, true);
            stream.Flush();
            stream.Position = 0;
            byte[] bytes = stream.ToArray();
            stream.Close();
            stream.Dispose();
            return Convert.ToBase64String(bytes);
        }

        public void reloadIntoNewDocument(int RightMarginValue)
        {
            List<Block> blocks = Document.Blocks.ToList();
            Document.Blocks.Clear();

            // create a new flowdocument and set it
            SpellCheck.IsEnabled = false;
            FlowDocument flowDoc = new FlowDocument();
            flowDoc.PageWidth = (double)RightMarginValue;
            flowDoc.ColumnWidth = (double)RightMarginValue; //999999.0;
            flowDoc.Blocks.AddRange(blocks);
            Document = flowDoc;

        }
        public void InsertBlockAtSelection(Block block)
        {
            dummy.Document.Blocks.Clear();
            dummy.Document.Blocks.Add(new Paragraph(new Run("")));
            dummy.Document.Blocks.Add(new Paragraph(new Run("")));
            dummy.Document.Blocks.Add(block);
            dummy.Document.Blocks.Add(new Paragraph(new Run("")));
            dummy.Document.Blocks.Add(new Paragraph(new Run("")));
            String xaml = RtbDocumentToXaml(dummy);
            XamlToSelectedBlock(xaml);
        }
        public void RtfToSelectedBlock(String rtf)
        {
            if (rtf == "") return;

            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            Block? block = ObjectAtSelectionStart();

            TextRange content;

            if (block == null)
                content = new System.Windows.Documents.TextRange(selection.Start, selection.End);
            else
                content = new System.Windows.Documents.TextRange(block.ElementStart, block.ElementEnd);

            byte[] bytes = Encoding.UTF8.GetBytes(rtf);
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            content.Load(stream, DataFormats.Rtf);
            stream.Close();
            stream.Dispose();
        }
        public bool XamlToSelectedBlock(String encoded)
        {
            if (encoded == "") return false;

            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            Block? block = ObjectAtSelectionStart();

            TextRange content;

            if (block == null)
                content = new System.Windows.Documents.TextRange(selection.Start, selection.End);
            else
                content = new System.Windows.Documents.TextRange(block.ElementStart, block.ElementEnd);

            byte[] bytes = Convert.FromBase64String(encoded);
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            content.Load(stream, DataFormats.XamlPackage);
            stream.Close();
            stream.Dispose();
            return true;
        }

        public String SelectionToRtf()
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            TextRange content = new System.Windows.Documents.TextRange(selection.Start, selection.End); 

            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, DataFormats.Rtf, true);
            stream.Flush();
            stream.Position = 0;
            String rtf = System.Text.Encoding.UTF8.GetString(stream.ToArray()); //reader.ReadToEnd();
            stream.Close();
            stream.Dispose();
            return rtf;
        }
        public String SelectionToXaml()
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            TextRange content = new System.Windows.Documents.TextRange(selection.Start, selection.End);

            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, DataFormats.XamlPackage, true);
            stream.Flush();
            stream.Position = 0;
            byte[] bytes = stream.ToArray();
            stream.Close();
            stream.Dispose();
            return Convert.ToBase64String(bytes);
        }
        public bool XamlToSelection(String encoded)
        {
            if (encoded == "") return false;

            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            TextRange content = new System.Windows.Documents.TextRange(selection.Start, selection.End);
            byte[] bytes = Convert.FromBase64String(encoded);
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            content.Load(stream, DataFormats.XamlPackage);
            stream.Close();
            stream.Dispose();
            return true;
        }

        public void RtfToSelection(String rtf)
        {
            if (rtf == "") return;

            TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
            TextRange content = new System.Windows.Documents.TextRange(selection.Start, selection.End);
            byte[] bytes = Encoding.UTF8.GetBytes(rtf);
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            content.Load(stream, DataFormats.Rtf);
            stream.Close();
            stream.Dispose();
        }

        public bool FromXaml(String encoded)
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            doc.Blocks.Clear();

            if (encoded == "") return false;

            //TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            byte[] bytes = Convert.FromBase64String(encoded);
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            this.Document = (FlowDocument)XamlReader.Load(stream);
            //content.Load(stream, DataFormats.XamlPackage);
            stream.Close();
            stream.Dispose();
            return true;
        }

        public String ToXaml()
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            //TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            XamlWriter.Save(doc, stream);
            //content.Save(stream, DataFormats.XamlPackage, true);
            stream.Flush();
            stream.Position = 0;
            byte[] bytes = stream.ToArray();
            stream.Close();
            stream.Dispose();
            return Convert.ToBase64String(bytes);
        }

        public static string? GetXmlEncoding(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString)) throw new ArgumentException("The provided string value is null or empty.");

            using (var stringReader = new StringReader(xmlString))
            {
                var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

                using (var xmlReader = XmlReader.Create(stringReader, settings))
                {
                    if (!xmlReader.Read()) throw new ArgumentException(
                        "The provided XML string does not contain enough data to be valid XML (see https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read)");

                    var result = xmlReader.GetAttribute("encoding");
                    return result;
                }
            }
        }

        public Encoding? GetXmlEncodingFromXmlHeader(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

                using (var xmlReader = XmlReader.Create(sr, settings))
                {
                    xmlReader.Read();
                    if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        while (xmlReader.MoveToNextAttribute())
                        {
                            if (xmlReader.Name == "encoding")
                            {
                                // successful, xml file
                                String? result = xmlReader.Value;
                                return Encoding.GetEncoding(result);
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(Stream? stream)
        {
            // Read the BOM
            var bom = new byte[4];
            stream.Position = 0;
            stream.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }
        public void FromRtf(String rtf)
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            doc.Blocks.Clear();

            if (rtf == "") return;

            TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            byte[] bytes = Encoding.UTF8.GetBytes(rtf);
            MemoryStream stream = new MemoryStream(bytes);
            //StreamReader sr = new StreamReader(stream, Encoding.Unicode);
            stream.Position = 0;
            content.Load(stream, DataFormats.Rtf);
            stream.Close();
            stream.Dispose();
        }
        public String ToRtf()
        {
            System.Windows.Documents.FlowDocument doc = this.Document;
            TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, DataFormats.Rtf, true);
            stream.Flush();
            stream.Position = 0;
            Encoding enc = GetEncoding(stream);
            stream.Position = 0;
            //TextReader reader = new StreamReader(stream, Encoding.ASCII);            
            String rtf = enc.GetString(stream.ToArray());//System.Text.Encoding.UTF8.GetString(stream.ToArray()); //reader.ReadToEnd();
            //reader.Close();
            stream.Close();
            stream.Dispose();
            return rtf;
        }

        public bool SetTableColumnWidth(ref Table table, int index, int width) 
        {
            if (index >= table.Columns.Count) return false;

            TableColumn col = table.Columns[index];
            GridLengthConverter glc = new GridLengthConverter();
            col.Width = (GridLength)glc.ConvertFromString(width.ToString());
            return true;
        }
        public bool SetTableColumnsWidth(ref Table table, ref List<TableColumn>? columns, int width)
        {
            foreach (TableColumn column in columns)
            {
                GridLengthConverter glc = new GridLengthConverter();
                column.Width = (GridLength)glc.ConvertFromString(width.ToString());
            }
            return true;
        }
        public bool GetTableColumnsByCells(ref Table table, ref TableRow row, ref List<TableCell>? cells, out List<TableColumn>? outColumns)
        {
            List<TableColumn> columns = new List<TableColumn>();
            foreach (TableCell cell in cells)
            {
                int cellIndex = -1;
                cellIndex = row.Cells.IndexOf(cell);
                if (cellIndex < 0) continue; // cell not found

                TableColumn? column = table.Columns[cellIndex];
                if (column == null) continue;

                // found column by cell
                columns.Add(column);
            }
            outColumns = columns;
            return true;
        }
        public bool GetTableColumnIndexesByCells(ref Table table, ref TableRow row, ref List<TableCell>? cells, out List<int>? outColumnIndexes)
        {
            List<int> list = new List<int>();
            foreach (TableCell cell in cells)
            {
                int cellIndex = -1;
                cellIndex = row.Cells.IndexOf(cell);
                if (cellIndex < 0) continue; // cell not found

                TableColumn? column = table.Columns[cellIndex];
                if (column == null) continue;

                // found column by cell
                list.Add(cellIndex);
            }
            outColumnIndexes = list;
            return true;
        }
        public bool GetTableRowIndexesByCells(ref Table table, ref List<TableCell>? cells, out List<int>? outList)
        {
            List<int> list = new List<int>();
            if (table.RowGroups.Count == 0)
            {
                outList = list;
                return true;
            }
            foreach (TableCell cell in cells)
            {
                int rowIndex = 0;
                foreach (TableRow row in table.RowGroups[0].Rows)
                {
                    int cellIndex = -1;
                    cellIndex = row.Cells.IndexOf(cell);
                    if (cellIndex >= 0)
                    {
                        // found row by cell
                        list.Add(rowIndex);
                    }
                    rowIndex++;
                }
            }
            outList = list;
            return true;
        }
        public bool GetTableRowsByCells(ref Table table, ref List<TableCell>? cells, out List<TableRow>? outList)
        {
            List<int> listIndexes = null;
            List<TableRow> rows = new List<TableRow>();
            if (table.RowGroups.Count == 0)
            {
                outList = rows;
                return true;
            }

            // get indexes
            GetTableRowIndexesByCells(ref table, ref cells, out listIndexes);

            // get rows by indexes
            foreach (int index in listIndexes)
                rows.Add(table.RowGroups[0].Rows.ElementAt(index));

            outList = rows;
            return true;
        }

        public bool GetTableCellsByIndexes(ref Table table, ref TableRow row, ref List<int> columnIndexes, out List<TableCell>? outCells)
        {
            List<TableCell>? list = new List<TableCell>();
            foreach (int columnIndex in columnIndexes)
            {
                if (columnIndex < 0) continue;
                if (columnIndex >= row.Cells.Count) continue;

                TableCell? cell = row.Cells.ElementAt(columnIndex);

                // found cell, add it
                list.Add(cell);
            }
            outCells = list;
            return true;
        }

        public bool AddTableRow(ref Table table, ref TableRow row)
        {
            return InsertTableRowLast(ref table, ref row);
        }
        public bool InsertTableRowFirst(ref Table table, ref TableRow row)
        {
            return InsertTableRow(ref table, ref row, 0);
        }
        public bool InsertTableRowLast(ref Table table, ref TableRow row)
        {
            // insert last
            return InsertTableRow(ref table, ref row, -1);
        }
        public long CountToIndex(long Count)
        {
            if (Count <= 0)
                return 0;
            else 
                return (Count - 1);
        }
        public bool InsertTableRow(ref Table table, ref TableRow row, ref TableRow? target)
        {
            if (table.RowGroups.Count == 0) 
            {
                // auto create row group if there is none
                table.RowGroups.Add(new TableRowGroup());
            }

            int targetIndex = table.RowGroups[0].Rows.IndexOf(target);
            if (targetIndex == -1)
                targetIndex = table.RowGroups[0].Rows.Count;

            table.RowGroups[0].Rows.Insert(targetIndex, row);
            return true;
        }
        public bool InsertTableRow(ref Table table, ref TableRow row, int targetRowIndex)
        {
            if (table.RowGroups.Count == 0)
            {
                // auto create row group if there is none
                table.RowGroups.Add(new TableRowGroup());
                targetRowIndex = 0;
            }

            // -1 means add last
            if (targetRowIndex == -1) targetRowIndex = table.RowGroups[0].Rows.Count;

            // automatically select the last index if given index is beyond scope.
            if (targetRowIndex > table.RowGroups[0].Rows.Count)
                targetRowIndex = table.RowGroups[0].Rows.Count;

            // insert to the collection by before the target index or at the 0 index if there is no items
            table.RowGroups[0].Rows.Insert(targetRowIndex, row);
            return true;
        }
        public void CopyTableRow(ref TableRow src, ref TableRow dest)
        {
            foreach (TableCell srcCell in src.Cells)
            {
                // create new destination cell
                TableCell cell = new TableCell();

                // configure cell
                cell.TextAlignment = srcCell.TextAlignment;
                cell.Background = srcCell.Background;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = srcCell.BorderThickness;

                Paragraph para = new Paragraph();
                cell.Blocks.Add(para);
                Run run = new Run(" ");
                para.Inlines.Add(run);

                // add cell into dest row
                dest.Cells.Add(cell);
            }
        }
        public bool RemoveTableRow(ref Table table, int targetRowIndex)
        {
            if (table.RowGroups.Count == 0) return false;
            if (targetRowIndex >= table.RowGroups[0].Rows.Count) return false;

            table.RowGroups[0].Rows.RemoveAt(targetRowIndex);
            return true;
        }
        public bool RemoveTableRow(ref Table table, ref TableRow row)
        {
            if (table.RowGroups.Count == 0) return false;
            return table.RowGroups[0].Rows.Remove(row);
        }
        public bool AddTableCell(ref TableRow row, ref TableCell cell)
        {
            return InsertTableCellLast(ref row, ref cell);
        }
        public bool InsertTableCellFirst(ref TableRow row, ref TableCell cell)
        {
            return InsertTableCell(ref row, ref cell, 0);
        }
        public bool InsertTableCellLast(ref TableRow row, ref TableCell cell)
        {
            // insert last
            return InsertTableCell(ref row, ref cell, -1);
        }
        public bool InsertTableCell(ref TableRow row, ref TableCell cell, int targetCellIndex)
        {
            // -1 means add last
            if (targetCellIndex == -1) targetCellIndex = row.Cells.Count;

            // automatically select the last index if given index is beyond scope.
            if (targetCellIndex > row.Cells.Count)
                targetCellIndex = row.Cells.Count;

            // insert to the collection by before the target index or at the 0 index if there is no items
            row.Cells.Insert(targetCellIndex, cell);
            return true;
        }
        public bool RemoveTableCell(ref TableRow row, int targetCellIndex)
        {
            if (row.Cells.Count == 0) return false;
            if (targetCellIndex >= row.Cells.Count) return false;

            row.Cells.RemoveAt(targetCellIndex);
            return true;
        }
        public bool InsertEmptyTableCellAllRows(List<TableRow> rows, int tableCellIndex, ref TableCell srcCell)
        {
            foreach (TableRow listedRow in rows)
            {
                TableRow row = listedRow;

                TableCell cell = new TableCell();
                InsertTableCell(ref row, ref cell, tableCellIndex);
                cell.TextAlignment = srcCell.TextAlignment;
                cell.Background = srcCell.Background;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = srcCell.BorderThickness;

                Paragraph para = new Paragraph();
                cell.Blocks.Add(para);
                Run run = new Run(" ");
                para.Inlines.Add(run);
            }
            return true;
        }
        public bool RemoveTableCellAllRows(List<TableRow> rows, int tableCellIndex)
        {
            foreach (TableRow listedRow in rows)
            {
                TableRow row = listedRow;
                RemoveTableCell(ref row, tableCellIndex);
            }
            return true;
        }
        public bool InsertTableColumn(ref Table table, ref TableColumn column, ref TableColumn? target, ref TableCell srcCell)
        {
            int targetColumnIndex = table.Columns.IndexOf(target);
            if (targetColumnIndex == -1)
                targetColumnIndex = table.Columns.Count;

            // insert to the collection by before the target index or at the 0 index if there is no items
            table.Columns.Insert(targetColumnIndex, column);

            // now configure all rows
            if (table.RowGroups.Count > 0)
                InsertEmptyTableCellAllRows(table.RowGroups[0].Rows.ToList(), targetColumnIndex, ref srcCell);

            return true;
        }

        public bool InsertTableColumn(ref Table table, ref TableColumn column, ref TableCell srcCell, int targetColumnIndex)
        {
            // -1 means add last
            if (targetColumnIndex == -1) targetColumnIndex = table.Columns.Count;

            // automatically select the last index if given index is beyond scope.
            if (targetColumnIndex > table.Columns.Count)
                targetColumnIndex = table.Columns.Count;

            // insert to the collection by before the target index or at the 0 index if there is no items
            table.Columns.Insert(targetColumnIndex, column);

            // now configure all rows
            if (table.RowGroups.Count > 0)
                InsertEmptyTableCellAllRows(table.RowGroups[0].Rows.ToList(), targetColumnIndex, ref srcCell);

            return true;
        }
        public bool RemoveTableColumn(ref Table table, ref TableColumn column)
        {
            if (table.Columns.Count == 0) return false;

            // get column's index
            int columnIndex = table.Columns.IndexOf(column);
            if (columnIndex == -1 ) return false;   

            // remove column
            table.Columns.Remove(column);

            // if no item found to configure, return true because objective is done
            if (table.RowGroups.Count == 0) return true;

            // now configure all things
            return RemoveTableCellAllRows(table.RowGroups[0].Rows.ToList(), columnIndex);
        }
        public bool RemoveTableColumn(ref Table table, int targetColumnIndex)
        {
            if (table.Columns.Count == 0) return false;
            if (targetColumnIndex >= table.Columns.Count) return false;

            // remove column
            table.Columns.RemoveAt(targetColumnIndex);

            // if no item found to configure, return true because objective is done
            if (table.RowGroups.Count == 0) return true;

            // now configure all things
            RemoveTableCellAllRows(table.RowGroups[0].Rows.ToList(), targetColumnIndex);

            return true;
        }
        public bool GetAllTextElementsRecursive(Block? block, out List<Object>? outList, int count = -1,
            bool listBlocks = false, bool listNonInlines = false)
        {
            List<Object> list = new List<Object>();
            if (block == null)
            {
                outList = null;
                return false;
            }
            if (count == 0)
            {
                outList = null;
                return false;
            }

            Queue<Object> queue = new Queue<Object>();

            int done = 1;

            // first enqueue base
            queue.Enqueue(block);

            // now list all tree
            while (queue.Count > 0)
            {
                Object? current = queue.Dequeue();

                if (current is Span)
                {
                    Span span = (Span)current;
                    foreach (Inline childNode in span.Inlines)
                        queue.Enqueue(childNode);

                    //if (listNonInlines)
                    list.Add(current);
                }
                else if (current is Paragraph)
                {
                    foreach (Inline item in ((Paragraph)block).Inlines)
                        queue.Enqueue(item);

                    if (listNonInlines)
                        list.Add(current);
                }
                else if (current is Section)
                {
                    foreach (Block item in ((Section)block).Blocks)
                        queue.Enqueue(item);

                    if (listBlocks)
                        list.Add(current);
                }
                else if (current is Inline || current is Run)
                {
                    list.Add(current);
                }

                if (done++ == count)
                    break;
            }

            outList = list;
            return true;
        }
        public bool FormatElementBold(ref Object? element, bool bold)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                if (bold)
                    para.FontWeight = FontWeights.Bold;
                else
                    para.FontWeight = FontWeights.Normal;

            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;

                if (bold)
                    section.FontWeight = FontWeights.Bold;
                else
                    section.FontWeight = FontWeights.Normal;

            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                if (bold)
                    text.FontWeight = FontWeights.Bold;
                else
                    text.FontWeight = FontWeights.Normal;
            }
            else
            {
            }
            return true;
        }
        public bool FormatElementItalic(ref Object? element, bool italic)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                if (italic)
                    para.FontStyle = FontStyles.Italic;
                else
                    para.FontStyle = FontStyles.Normal;

            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;

                if (italic)
                    section.FontStyle = FontStyles.Italic;
                else
                    section.FontStyle = FontStyles.Normal;
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                if (italic)
                    text.FontStyle = FontStyles.Italic;
                else
                    text.FontStyle = FontStyles.Normal;
            }
            else
            {
            }
            return true;
        }
        public bool FormatElementUnderline(ref Object? element, bool underline)
        {
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                SetDecorations(element, TextDecorations.Underline, underline);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                // set decorations
                SetDecorations(element, TextDecorations.Underline, underline);
            }
            else
            {
            }
            return true;
        }
        public bool FormatElementStrikeout(ref Object? element, bool strikeout)
        {
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                SetDecorations(element, TextDecorations.Strikethrough, strikeout);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                // set decorations
                SetDecorations(element, TextDecorations.Strikethrough, strikeout);
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockStrikeoutRecursive(ref Block? block, bool set)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementStrikeout(ref element, set);
            }
            return true;
        }
        public bool FormatElementFont(ref Object? element, System.Windows.Media.FontFamily font)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;
                para.FontFamily = font;
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;
                section.FontFamily = font;
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;
                text.FontFamily = font;
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockFontRecursive(ref Block? block, System.Windows.Media.FontFamily value)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementFont(ref element, value);
            }
            return true;
        }
        public bool FormatElementFontSize(ref Object? element, double value)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;
                para.FontSize = (double)value;
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;
                section.FontSize = (double)value;
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;
                text.FontSize = (double)value;
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockFontSizeRecursive(ref Block? block, double value)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementFontSize(ref element, value);
            }
            return true;
        }
        public bool FormatElementForeground(ref Object? element, System.Windows.Media.Color value)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;
                para.Foreground = new SolidColorBrush(value);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;
                section.Foreground = new SolidColorBrush(value);
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;
                text.Foreground = new SolidColorBrush(value);
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockForegroundRecursive(ref Block? block, System.Windows.Media.Color value)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementForeground(ref element, value);
            }
            return true;
        }
        public bool FormatElementBackground(ref Object? element, System.Windows.Media.Color value)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;
                para.Background = new SolidColorBrush(value);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;
                section.Background = new SolidColorBrush(value);
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;
                text.Background = new SolidColorBrush(value);
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockBackgroundRecursive(ref Block? block, System.Windows.Media.Color value)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementBackground(ref element, value);
            }
            return true;
        }
        public bool FormatElementTextAlignment(ref Object? element, TextAlignment value)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;
                para.TextAlignment = value;
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;
                section.TextAlignment = value;
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;
            }
            else
            {
            }
            return true;
        }
        public bool FormatBlockTextAlignmentRecursive(ref Block? block, TextAlignment value)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementTextAlignment(ref element, value);
            }
            return true;
        }
        public bool FormatBlockUnderlineRecursive(ref Block? block, bool set)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementUnderline(ref element, set);
            }
            return true;
        }
        public bool FormatBlockBoldRecursive(ref Block? block, bool set)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementBold(ref element, set);
            }
            return true;
        }
        public bool FormatBlockItalicRecursive(ref Block? block, bool set)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElementItalic(ref element, set);
            }
            return true;
        }

        public bool FormatElement(ref Object? element, System.Windows.Media.FontFamily font,
            int fontSize, bool bold, bool italic, bool underline, bool strikeout,
            System.Windows.Media.Color foreground, System.Windows.Media.Color background, TextAlignment align)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                SetDecorations(element, TextDecorations.Underline, underline);
                SetDecorations(element, TextDecorations.Strikethrough, strikeout);

                para.FontFamily = font;
                para.FontSize = (double)fontSize;

                if (bold)
                    para.FontWeight = FontWeights.Bold;
                else
                    para.FontWeight = FontWeights.Normal;

                if (italic)
                    para.FontStyle = FontStyles.Italic;
                else
                    para.FontStyle = FontStyles.Normal;
                
                para.TextAlignment = align;

                para.Foreground = new SolidColorBrush(foreground);
                para.Background = new SolidColorBrush(background);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;

                // set decorations
                SetDecorations(element, TextDecorations.Underline, underline);
                SetDecorations(element, TextDecorations.Strikethrough, strikeout);

                section.FontFamily = font;
                section.FontSize = (double)fontSize;

                if (bold)
                    section.FontWeight = FontWeights.Bold;
                else
                    section.FontWeight = FontWeights.Normal;

                if (italic)
                    section.FontStyle = FontStyles.Italic;
                else
                    section.FontStyle = FontStyles.Normal;

                section.TextAlignment = align;

                section.Foreground = new SolidColorBrush(foreground);
                section.Background = new SolidColorBrush(background);

            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                // set decorations
                SetDecorations(element, TextDecorations.Underline, underline);
                SetDecorations(element, TextDecorations.Strikethrough, strikeout);

                text.FontFamily = font;
                text.FontSize = (double)fontSize;

                if (bold)
                    text.FontWeight = FontWeights.Bold;
                else
                    text.FontWeight = FontWeights.Normal;

                if (italic)
                    text.FontStyle = FontStyles.Italic;
                else
                    text.FontStyle = FontStyles.Normal;

                text.Foreground = new SolidColorBrush(foreground);
                text.Background = new SolidColorBrush(background);
            }
            else
            {
            }
            return true;
        }

        public bool FormatBlockRecursive(ref Block? block, System.Windows.Media.FontFamily font,
            int fontSize, bool bold, bool italic, bool underline, bool strikeout,
            System.Windows.Media.Color foreground, System.Windows.Media.Color background, TextAlignment align)
        {
            // get all inlines recursively
            List<Object>? elements = null;
            if (!GetAllTextElementsRecursive(block, out elements, -1, true, true))
                return false;

            foreach (Object? thisElement in elements)
            {
                Object? element = thisElement;
                FormatElement(ref element, font, fontSize, bold, italic, underline, strikeout, foreground, background, align);
            }
            return true;
        }
    
    public bool FormatTableCell(ref Table table, ref List<TableCell>? cells, System.Windows.Media.FontFamily font, int fontSize,
            bool bold, bool italic, bool underline, bool strikeout, 
            TextAlignment align, System.Windows.Media.Color foreground, System.Windows.Media.Color background,
            Thickness cellBorder, Thickness tableBorder)
        {
            // configure all selected cells
            foreach (TableCell? cell in cells)
            {
                cell.TextAlignment = align;
                cell.Background = new SolidColorBrush(background);
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = cellBorder;
            }

            // now configure everything of cells
            foreach (TableCell? cell in cells)
            {
                // configure cell's all blocks and their content
                Block? block = cell.Blocks.FirstOrDefault();
                while (block != null)
                {
                    FormatBlockRecursive(ref block, font, fontSize, bold, italic, underline, strikeout, foreground, background, align);
                    block = block.NextBlock;
                }
            }
            return true;
        }
        public bool FormatTableCellRemoveAllFormatting(ref List<TableCell>? cells)
        {
            System.Windows.Media.Brush forebrush = new SolidColorBrush(Colors.Black);
            System.Windows.Media.Brush backbrush = new SolidColorBrush(Colors.White);

            foreach (TableCell? cell in cells)
            {
                cell.TextAlignment = TextAlignment.Left;
                cell.Background = backbrush;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = new Thickness(1.0);

                Block? block = cell.Blocks.FirstOrDefault();// listedblock;
                if (block == null) continue;
                while (block != null)
                {
                    FormatBlockFontRecursive(ref block, new Media.FontFamily("Times New Roman"));
                    FormatBlockFontSizeRecursive(ref block, 14.0);
                    FormatBlockForegroundRecursive(ref block, Media.Colors.Black);
                    FormatBlockBackgroundRecursive(ref block, Media.Colors.White);
                    FormatBlockBoldRecursive(ref block, false);
                    FormatBlockItalicRecursive(ref block, false);
                    FormatBlockUnderlineRecursive(ref block, false);
                    FormatBlockStrikeoutRecursive(ref block, false);
                    FormatBlockTextAlignmentRecursive(ref block, TextAlignment.Left);
                    block = block.NextBlock;
                }
            }
            return true;
        }

        public bool GetSelectedTableParams(TextSelection selection, out Table? outTable, out TableRow? outRow, out TableCell? outCell,
            out int outRowIndex, out int outCellIndex)
        {
            var curCaret = CaretPosition;
            Table? table = this.Document.Blocks.OfType<Table>().Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
            if (table == null)
            {
                // no table at selection
                outTable = null;
                outRow = null;
                outCell = null;
                outRowIndex = outCellIndex = -1;
                return false;
            }
            if (table.RowGroups.Count == 0)
            {
                // table has no row groups, error
                outTable = null;
                outRow = null;
                outCell = null;
                outRowIndex = outCellIndex = -1;
                return false;
            }

            // find the first selected items and return them
            TextPointer current = selection.Start;//.GetInsertionPosition(LogicalDirection.Forward);
            TableCell? cell = GetParentTableCell(current);
            if (cell != null)
            {
                // found a cell
                outCell = (TableCell)cell;
                outRow = (TableRow)outCell.Parent;
                outTable = table;

                // find cell and row index
                outCellIndex = outRow.Cells.IndexOf(outCell);
                outRowIndex = table.RowGroups[0].Rows.IndexOf(outRow);

                // done
                return true;
            }
            // not found anything
            outTable = null;
            outRow = null;
            outCell = null;
            outRowIndex = outCellIndex = -1;
            return false;
        }
        public static TableCell? GetParentTableCell(TextPointer position)
        {
            var direction = LogicalDirection.Backward;
            for (; position != null; position = position.GetNextContextPosition(direction))
            {
                if (position.GetAdjacentElement(direction) is TableCell cell) { return cell; }
            }
            return null;
        }
        public static TableCell? GetParentTableCell(TextPointer position, TextSelection selection)
        {
            var direction = LogicalDirection.Backward;
            for (; !selection.Contains(position); position = position.GetNextContextPosition(direction))
            {
                if (position == null) return null;
                if (position.GetAdjacentElement(direction) is TableCell cell) { return cell; }
            }
            return null;
        }
        private bool isBetween(TextPointer first, TextPointer last, TextPointer current)
        {
            return new TextRange(first, last).Contains(current);
        }

        public void SaveFlowDocumentToXamlFile(string fileName)
        {
            // Open or create the output file.
            FileStream xamlFile = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            // Save the contents of the FlowDocumentReader to the file stream that was just opened.
            XamlWriter.Save(Document, xamlFile);
            xamlFile.Close();
        }


        /*
        private bool isBetween(TextPointer first, TextPointer last, TextPointer current)
        {
            bool isAfterFirst = first.GetOffsetToPosition(current) >= 0;
            bool isBeforeLast = last.GetOffsetToPosition(current) <= 0;

            return isAfterFirst && isBeforeLast;
        }
        */

        //public List<TableCell> GetSelectedCells(TextSelection selection)
        // {
        //     return flowDoc.Blocks.OfType<TableCell>().Where(w => selection.Contains(w.ContentStart)).ToList();
        //}

    }
}
