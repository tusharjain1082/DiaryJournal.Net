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
using System.Windows.Markup;
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

        public WpfRichTextBoxEx()
            : base()
        {
            IsInactiveSelectionHighlightEnabled = dummy.IsInactiveSelectionHighlightEnabled = true;
            //Document.PageWidth = 10000;
            //Document.MinPageWidth = 10000;
            //Document.MaxPageWidth = 10000;
            this.LostFocus += WpfRichTextBoxEx_LostFocus;
            //this.SelectionChanged += WpfRichTextBoxEx_SelectionChanged;
        }

        private void WpfRichTextBoxEx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            /*
            if (this.RichText.Selection != null)
            {
                var currentValue = new TextRange(this.RichText.Selection.Start, this.RichText.Selection.End);
                if (currentValue.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Underline)
                {
                    this.UnderlineButton.IsChecked = true;
                }
                else
                {
                    this.UnderlineButton.IsChecked = false;
                }
            }
            */
        }

        private void WpfRichTextBoxEx_LostFocus(object sender, RoutedEventArgs e)
        {
            this.sel = this.Selection;
            e.Handled = true;
        }


        protected override void OnLostFocus(RoutedEventArgs e)
        {
            // When the RichTextBox loses focus the user can no longer see the selection.
            // This is a hack to make the RichTextBox think it did not lose focus.
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
        public bool Bold
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return false; // empty document or no text selected or in selection
                FontWeight? bold = null;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    bold = inline.FontWeight;
                else
                    bold = para.FontWeight;

                if (bold == FontWeights.Bold)
                    return true;
                else
                    return false;

                /*
                Object obj = selection.GetPropertyValue(TextElement.FontWeightProperty);
                if (obj is not System.Windows.FontWeight)
                    return false;

                System.Windows.FontWeight weight = (System.Windows.FontWeight)obj;
                if (weight == FontWeights.Bold)
                    return true;
                else
                    return false;
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //if (value)
                //    selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                //else
                //    selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                // get paragraph and configure it
                //Paragraph para = selection.Start.Paragraph;
                //Block? block = (Block?)para;

                // get all blocks in selection
                List<Paragraph> paragraphs = Document.WalkTextRange<Paragraph>(selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;   
                    FormatBlockBoldRecursive(ref block, value);
                }
            }
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

        public bool Italic
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return false; // empty document or no text selected or in selection
                FontStyle? italic = null;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    italic = inline.FontStyle;
                else
                    italic = para.FontStyle;

                if (italic == FontStyles.Italic)
                    return true;
                else
                    return false;

                /*
                Object obj = selection.GetPropertyValue(TextElement.FontStyleProperty);
                if (obj is not System.Windows.FontStyle)
                    return false;
                
                System.Windows.FontStyle style = (System.Windows.FontStyle)obj;
                if (style == FontStyles.Italic)
                    return true;
                else
                    return false;
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                // get all blocks in selection
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockItalicRecursive(ref block, value);
                }

                //if (value)
                //    selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                //else
                //   selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);

            }
        }
        public bool Underline
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return false; // empty document or no text selected or in selection

                bool underline = false;
                bool strikeout = false;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    GetDecorations(inline, ref underline, ref strikeout);
                else
                    GetDecorations(para, ref underline, ref strikeout);

                // configure
                return underline;


                /*
                TextRange range = new TextRange(selection.Start, selection.End);
                TextDecorationCollection? decorations = GetDecorations(range);//(TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                if (decorations == null) return false;

                foreach (TextDecoration textDecoration in decorations)
                {
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        return true;
                }
                return false;
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //TextRange range = new TextRange(selection.Start, selection.End);
                //this.SetDecorations(range, TextDecorations.Underline, value);

                // get all blocks in selection
                List<Paragraph> paragraphs = Document.WalkTextRange<Paragraph>(selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockUnderlineRecursive(ref block, value);
                }

            }
        }
        public bool Strikeout
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return false; // empty document or no text selected or in selection
                bool underline = false;
                bool strikeout = false;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    GetDecorations(inline, ref underline, ref strikeout);
                else
                    GetDecorations(para, ref underline, ref strikeout);

                // configure
                return strikeout;

                /*
                TextRange range = new TextRange(selection.Start, selection.End);
                TextDecorationCollection? decorations = GetDecorations(range);//(TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                if (decorations == null) return false;

                foreach (TextDecoration textDecoration in decorations)
                {
                    if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        return true;
                }
                return false;
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //TextRange range = new TextRange(selection.Start, selection.End);
                //this.SetDecorations(range, TextDecorations.Strikethrough, value);

                // get all blocks in selection
                List<Paragraph> paragraphs = Document.WalkTextRange<Paragraph>(selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockStrikeoutRecursive(ref block, value);
                }
            }
        }
        public void GetDecorations(TextElement element, ref bool underline, ref bool strikeout)
        {
            underline = strikeout = false;

            // get decorations
            TextRange range = new TextRange(element.ContentStart, element.ContentEnd);
            TextDecorationCollection? decorations = GetDecorations(range);
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
        }
        public TextDecorationCollection? GetDecorations(TextRange selection)
        {
            TextDecorationCollection decors = selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
            if (decors == null || decors.Count == 0)
            {
                if (selection.Start.Parent is Run run)
                {
                    if (run.Parent is Span span)
                    {
                        decors = span.TextDecorations;
                    }
                    else if (run.Parent is Paragraph para)
                    {
                        decors = para.TextDecorations;
                    }
                }
            }

            if (decors is TextDecorationCollection tdc)
            {
                return decors;
                // TODO: Processing decorations...  
            }
            else
            {
                return null;
            }
        }
        public void SetDecorations(TextRange textRange, TextDecorationCollection decoration, bool set)
        {
            //var decorations = textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection
            //                  ?? new TextDecorationCollection();
            TextDecorationCollection? decorations = GetDecorations(textRange);

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

                double value = (double)selection.GetPropertyValue(Paragraph.TextIndentProperty);
                return value;
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

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return 14; // empty document or no text selected or in selection
                double value = 0;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    value = inline.FontSize;
                else
                    value = para.FontSize; // empty paragraph, so set paragraph's own foreground

                if (value <= 0)
                    value = 14.0;

                return value;

                /*
                Object obj = selection.GetPropertyValue(TextElement.FontSizeProperty);
                if (obj is double)
                {
                    double value = (double)obj;
                    return value;
                }
                return 14;
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                // get all blocks in selection
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockFontSizeRecursive(ref block, value);
                }

                //                selection.ApplyPropertyValue(TextElement.FontSizeProperty, value);
            }
        }

        public TextDecorationCollection SelectionDecorations
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                TextDecorationCollection decorations = (TextDecorationCollection)selection.GetPropertyValue(Inline.TextDecorationsProperty);
                return decorations;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value);
            }
        }
        public System.Drawing.Font SelectionFont
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return new Font("Times New Roman", 14); // empty document or no text selected or in selection
                System.Windows.Media.FontFamily value = null;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    value = inline.FontFamily;
                else
                    value = para.FontFamily; // empty paragraph, so set paragraph's own foreground

                if (value == null)
                    value = new Media.FontFamily("Times New Roman");

                // configure
                Font destvalue = WpfFontToWinFormsFont(value);
                return destvalue;

                /*
                Object obj = selection.GetPropertyValue(TextElement.FontFamilyProperty);
                if (obj is System.Windows.Media.FontFamily)
                {
                    System.Windows.Media.FontFamily font = (System.Windows.Media.FontFamily)obj;
                    System.Drawing.Font value = new Drawing.Font(font.Source, 14, Drawing.FontStyle.Regular);
                    return value;
                }
                else
                {
                    System.Drawing.Font value = new Drawing.Font("Times New Roman", 14, Drawing.FontStyle.Regular);
                    return value;
                }
                */
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                System.Windows.Media.FontFamily font = new System.Windows.Media.FontFamily(value.Name);

                // get all blocks in selection
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockFontRecursive(ref block, font);
                }

                /*
                try
                {
                    if (this.Document != null) 
                        selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);

                } catch { }
                */
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
                /*
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                System.Windows.Media.Brush brush = (System.Windows.Media.Brush)selection.GetPropertyValue(TextElement.ForegroundProperty);

                //SolidColorBrush brush = (SolidColorBrush)this.SelectionBrush;

                if (brush == null)
                    brush = new SolidColorBrush(Colors.Black);

                System.Drawing.Color destcolor = WpfBrushToWinFormsColor((SolidColorBrush)brush);
                return destcolor;
                */
                System.Windows.Media.Brush? defaultBrush = new SolidColorBrush(Media.Colors.Black);
                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return WpfBrushToWinFormsColor((SolidColorBrush)defaultBrush); // empty document or no text selected or in selection
                //var curCaret = this.CaretPosition; // absolute position cursor visible on exact text selected.
                System.Windows.Media.Brush? brush = null;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    brush = inline.Foreground;
                else
                    brush = para.Foreground; // empty paragraph, so set paragraph's own foreground

                // if brush is null meaning empty paragraph and paragraph has no foreground brush/color set,
                // or inline exists, but inline has no foreground brush/color set, so auto-select default foreground white.
                if (brush == null)
                    brush = defaultBrush;

                // configure
                System.Drawing.Color destcolor = WpfBrushToWinFormsColor((SolidColorBrush)brush);
                return destcolor;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //                SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B));
                //                selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);

                // get all blocks in selection
                System.Windows.Media.Color color = WinFormsColorToWpfColor(value);
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockForegroundRecursive(ref block, color);
                }
            }
        }
        public System.Drawing.Color SelectionBackColor
        {
            get
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //SolidColorBrush brush = (SolidColorBrush)selection.GetPropertyValue(System.Windows.Documents.TextElement.BackgroundProperty);////).GetPropertyValue(TextElement.BackgroundProperty);

                /*
                SolidColorBrush? brush = null;
                TextRange selectionTextRange = new TextRange(selection.Start, selection.End);
                SolidColorBrush? brush0 = (SolidColorBrush)selectionTextRange.GetPropertyValue(TextElement.BackgroundProperty);
                //SolidColorBrush brush1 = null;
                //brush1 = (SolidColorBrush)selection.Start.Paragraph.Background;
                SolidColorBrush? brush2 = null;
                try
                {
                    brush2 = (SolidColorBrush)((Span)((Run)selection.Start.Parent).Parent).Background;
                }
                catch 
                {
                    //Selection Parent is Run
                    //Run Parent can be Span, Paragraph, etc.
                    //Parent is not Span
                }

                // last resort object
                SolidColorBrush brush3 = null;
                Block? block = ObjectAtSelectionStart();
                if (block != null) brush3 = (SolidColorBrush)block.Background;

                if (brush0 != null)
                    brush = brush0;
                //else if (brush1 != null)
                //    brush = brush1;
                else if (brush2 != null)
                    brush = brush2;
                else if (brush3 != null)
                    brush = brush3;
                else // no matches so return default background color brush which is always white
                    brush = new SolidColorBrush(Colors.White);

                */
                //                    brush = new SolidColorBrush(Colors.White);

                System.Windows.Media.Brush? defaultBrush = new SolidColorBrush(Media.Colors.White);
                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return WpfBrushToWinFormsColor((SolidColorBrush)defaultBrush); // empty document or no text selected or in selection
                var curCaret = this.CaretPosition; // absolute position cursor visible on exact text selected.
                System.Windows.Media.Brush? brush = null;
                Inline? inline = para.Inlines.FirstOrDefault();//para.Inlines.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (inline != null)
                    brush = inline.Background;
                else
                    brush = para.Background; // empty paragraph, so set paragraph's own background

                // if brush is null meaning empty paragraph and paragraph has no background brush/color set,
                // or inline exists, but inline has no background brush/color set, so auto-select default background white.
                if (brush == null)
                    brush = defaultBrush;

                // configure
                System.Drawing.Color destcolor = WpfBrushToWinFormsColor((SolidColorBrush)brush);
                return destcolor;
            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));
                //                SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B));
                //              selection.ApplyPropertyValue(TextElement.BackgroundProperty, brush);

                // get all blocks in selection
                System.Windows.Media.Color color = WinFormsColorToWpfColor(value);
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockBackgroundRecursive(ref block, color);
                }

            }
        }

        public TextAlignment SelectionAlignment
        {
            get 
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                Paragraph? para = this.Selection.Start.Paragraph;
                if (para == null) return TextAlignment.Left; // empty document or no text selected or in selection
                TextAlignment align = para.TextAlignment;
                if (align == null) return TextAlignment.Left;
                return align;
                /*
                TextSelection selection = this.Selection;
                Object obj = selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
                if (obj is TextAlignment)
                {
                    TextAlignment value = (TextAlignment)obj;
                    return value;
                }
                return TextAlignment.Left;
                */

            }
            set
            {
                this.Focus();
                TextSelection selection = ((this.IsSelectionActive) ? this.Selection : ((this.sel != null) ? this.sel : this.Selection));

                // get all blocks in selection
                List<Paragraph> paragraphs = FlowDocumentHelper.WalkTextRange<Paragraph>(Document, selection.Start, selection.End).ToList();
                foreach (Paragraph paragraph in paragraphs)
                {
                    // configure
                    Block? block = (Block?)paragraph;
                    FormatBlockTextAlignmentRecursive(ref block, value);
                }

                /*
                TextSelection selection = this.Selection;
                selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value);
                */
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
            //set => base.Text = value;
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
                cell.Background = srcCell.Background;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = srcCell.BorderThickness;

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
                cell.Background = srcCell.Background;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = srcCell.BorderThickness;

                InsertTableCell(ref row, ref cell, tableCellIndex);
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
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                TextRange range = new TextRange(para.ContentStart, para.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;

                // set decorations
                TextRange range = new TextRange(section.ContentStart, section.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                // set decorations
                TextRange range = new TextRange(text.ContentStart, text.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
            }
            else
            {
            }
            return true;
        }
        public bool FormatElementStrikeout(ref Object? element, bool strikeout)
        {
            // firstly configure the paragraph
            if (element is Paragraph)
            {
                Paragraph para = (Paragraph)element;

                // set decorations
                TextRange range = new TextRange(para.ContentStart, para.ContentEnd);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);
            }
            else if (element is Section)
            {
                // section contains paragraphs as children
                Section section = (Section)element;

                // set decorations
                TextRange range = new TextRange(section.ContentStart, section.ContentEnd);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);
            }
            else if (element is Run || element is Inline || element is Span)
            {
                // text inline or run
                Inline text = (Inline)element;

                // set decorations
                TextRange range = new TextRange(text.ContentStart, text.ContentEnd);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);
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
                TextRange range = new TextRange(para.ContentStart, para.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);

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
                TextRange range = new TextRange(section.ContentStart, section.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);

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
                TextRange range = new TextRange(text.ContentStart, text.ContentEnd);
                SetDecorations(range, TextDecorations.Underline, underline);
                SetDecorations(range, TextDecorations.Strikethrough, strikeout);

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
            //table.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            //table.BorderThickness = tableBorder;

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
                //cell.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");
                //cell.FontSize = 14.0;
                //cell.FontWeight = FontWeights.Normal;
                //cell.FontStyle = FontStyles.Normal;
                cell.TextAlignment = TextAlignment.Left;
                //cell.Foreground = forebrush;
                cell.Background = backbrush;
                cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                cell.BorderThickness = new Thickness(1.0);

                foreach (Block listedblock in cell.Blocks)
                {
                    Block? block = listedblock;
                    FormatBlockFontRecursive(ref block, new Media.FontFamily("Times New Roman"));
                    FormatBlockFontSizeRecursive(ref block, 14.0);
                    FormatBlockForegroundRecursive(ref block, Media.Colors.Black);
                    FormatBlockBackgroundRecursive(ref block, Media.Colors.White);
                    FormatBlockBoldRecursive(ref block, false);
                    FormatBlockItalicRecursive(ref block, false);
                    FormatBlockUnderlineRecursive(ref block, false);
                    FormatBlockStrikeoutRecursive(ref block, false);
                    FormatBlockTextAlignmentRecursive(ref block, TextAlignment.Left);
                }
            }
            return true;
        }

        public void initDocumentTables()
        {
            int index = 0;
            int rows = 5;
            int columns = 4;
            Table table = new Table();
            TableRowGroup rg = new TableRowGroup();
            int cellname = 0;
            for (int c = 0; c < columns; c++)
            {
                TableColumn column = new TableColumn();
                column.Name = "column" + c.ToString();
                table.Columns.Add(column);
            }

            for (int r = 0; r < rows; r++) 
            {
                TableRow row = new TableRow();
                for (int col = 0; col < columns; col++)
                {
                    TableCell cell = new TableCell();
                    cell.Name = "cell" + cellname++.ToString();
                    cell.BorderBrush = new SolidColorBrush(Colors.Black);
                    cell.BorderThickness = new Thickness(1);
                    cell.TextAlignment= TextAlignment.Center;
                    Paragraph cellpara = new Paragraph();
                    cellpara.Inlines.Add(cell.Name);
                    cell.Blocks.Add(cellpara);
                    //TextBlock text= new TextBlock();
                    //text.Inlines.Add("hello world");
                    //text.VerticalAlignment = VerticalAlignment.Center;
                    //Paragraph cellpara = new Paragraph();
                    //cellpara.Inlines.Add(cell.Name);
                    //cell.Blocks.Add(cellpara);
                    //BlockUIContainer container = new BlockUIContainer(text);
                    //cell.Blocks.Add(container);
                    cell.Focusable = true;
                    row.Cells.Add(cell);     
                }
                rg.Rows.Add(row);
            }
            table.RowGroups.Add(rg);

            SetTableColumnWidth(ref table, 3, 400);

            Document.Blocks.Clear();
            Document.Blocks.Add(table);

            Document.Blocks.Clear();
            Document.Blocks.Add(table);

            Table table1 = (Table)Document.Blocks.First();
            TableRow customrow = new TableRow();
            for (int col = 0; col < columns; col++)
            {
                TableCell cell = new TableCell();
                cell.Name = "cell" + cellname++.ToString();
                cell.BorderBrush = new SolidColorBrush(Colors.Black);
                cell.BorderThickness = new Thickness(1);
                Paragraph cellpara = new Paragraph();
                cellpara.Inlines.Add(cell.Name);
                cell.Blocks.Add(cellpara);
                cell.Focusable = true;
                customrow.Cells.Add(cell);
            }
            //InsertTableRow(ref table1, ref customrow, 8);
            InsertTableRow(ref table1, ref customrow, -1);
            //TableColumn column1 = new TableColumn();
            //column1.Width = new GridLength(100);
            //TableCell? cell1 = table1.RowGroups[0].Rows[0].Cells.First();
            //InsertTableColumn(ref table1, ref column1, ref cell1, 0);
            RemoveTableColumn(ref table1, 0);
            RemoveTableColumn(ref table1, 0);

            String encoded = ToXaml();
            bool result = FromXaml(encoded);

            return;
            //table1.Columns.RemoveAt(0);
            //InsertTableRow(ref table1, ref customrow, -1);
            /*
            foreach (Table table in Document.Blocks.OfType<Table>().ToList())

            {
                table.Name = "tableindex" + index++;
                foreach (TableRowGroup rowgroup in table.RowGroups)
                {
                    foreach (TableRow row in rowgroup.Rows)
                    {
                        foreach (TableCell cell in row.Cells)
                        {
                            cell.Focusable= true;
                            cell.GotFocus += Cell_GotFocus;
                            cell.LostFocus += Cell_LostFocus;
                        }
                    }
                }
            }
            */

        }

        private void Cell_LostFocus(object sender, RoutedEventArgs e)
        {
            TableCell? cell = (TableCell?)sender;
            if (cell == null) return;

            cell.Tag = false;

        }

        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            TableCell? cell = (TableCell?)sender;
            if (cell == null) return;

            cell.Tag = true;
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


        public List<TableCell> GetSelectedCells(TextSelection selection)
        {
            List<TableCell> cells = new List<TableCell>();
            var curCaret = CaretPosition;
            Table table = this.Document.Blocks.OfType<Table>().Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
            if (table == null) return cells;
            if (table.RowGroups.Count == 0) return cells;


            TextPointer pointer = selection.Start;// (LogicalDirection.Forward);
            int start = Document.ContentStart.DocumentStart.GetOffsetToPosition(selection.Start);
            int end = Document.ContentStart.DocumentStart.GetOffsetToPosition(selection.End);

            // find all matches on the basis of pattern
            TextRange content = new TextRange(selection.Start, selection.End);
            TextPointer current = content.Start;//.GetNextContextPosition(LogicalDirection.Forward);//.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                if (current.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    // If so, is the tag a Run?
                    if (current.Parent is TableCell)
                    {
                        TableCell cell = (TableCell)current.Parent;
                        //if (cell.Blocks.Count == 0) continue;
                        //Paragraph cellpara = (Paragraph)cell.Blocks.First();
                        //if (content.Contains(cellpara.ContentStart))// && content.Contains(cellpara.ElementEnd))
                            cells.Add((TableCell)current.Parent);
                    }
                }

                current = current.GetNextContextPosition(LogicalDirection.Forward);
                //current = current.GetNextInsertionPosition(LogicalDirection.Forward);
                if (!content.Contains(current)) break;
            }
            return cells;
            /*
            TextRange range = new TextRange(selection.Start, selection.End);

            TextPointer position = range.Start;// range.Start;
            int rangestart = range.Start.GetOffsetToPosition(range.Start);

            // Traverse content in forward direction until the position is immediately after the opening 
            // tag of a Run element, or the end of content is encountered.
            while (position != null)
            {
                //if (position.CompareTo(range.End) > 0) break;
                // Is the current position just after an opening element tag?
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    // If so, is the tag a Run?
                    if (position.Parent is TableCell)
                        cells.Add((TableCell)position.Parent);
                }

                // Not what we're looking for; on to the next position.
                position = position.GetNextContextPosition(LogicalDirection.Forward);
                //if (position.CompareTo(range.End) > 0) break;  
                if (!range.Contains(position)) break;
            }
            */
            /*
            foreach (TableRowGroup rowGroup in table.RowGroups)
            {
                foreach (TableRow row in rowGroup.Rows)
                {
                    //row.GotFocus
                    foreach (TableCell cell in row.Cells)
                    {
                        int cellend = Document.ContentStart.GetOffsetToPosition(cell.ContentEnd);
                        if (cellend > start &&  cellend <= end)
                            cells.Add(cell);

                        //if (selection.Contains(pos)) cells.Add(cell);
                    }
                    /*
                    // option 1, this also yields additional unselected cells
                    List<TableCell> selcells = row.Cells.Where(w => selection.Contains(w.ContentStart) && selection.Contains(w.ContentEnd)).ToList();

                    // option 2, this also yields additional unselected cells.
                    foreach (TableCell cell in selcells)
                    {
                        if (selection.Contains(cell.ElementStart) && selection.Contains(cell.ElementEnd))
                            cells.Add(cell);
                    }
                    */
            //            }
            //       }

            return cells;
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
