using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Text;
using Elistia.DotNetRtfWriter;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace DiaryJournal.Net
{
    public class textFormatting
    {
        public string familyName = "";
        public string familyList = "";
        public System.Drawing.FontFamily[]? fontFamilies = null;
        public List<String> fontNames = new List<string>();
        public int selStartIndex = 0;
        public int selLength = 0;

        public textFormatting()
        {
            initFonts();
        }

        public void initFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            // Get the array of FontFamily objects.
            fontFamilies = installedFontCollection.Families;

            fontNames.Clear();
            for (long j = 0; j < fontFamilies.LongCount(); ++j)
                fontNames.Add(fontFamilies[j].Name);

        }

        public System.Drawing.FontFamily findFont(String fontName)
        {
            foreach (System.Drawing.FontFamily fontFamily in fontFamilies)
            {
                if (fontFamily.Name == fontName)
                    return fontFamily;
            }
            return null;
        }

        public void formatLeftJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Left;
        }
        public void formatLeftJustify(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            TextSelection selection = rtb.Selection;
            selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
        }
        public void formatRightJustify(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            TextSelection selection = rtb.Selection;
            selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }
        public void formatJustify(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            TextSelection selection = rtb.Selection;
            selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
        }
        public void formatCenterJustify(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            TextSelection selection = rtb.Selection;
            selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        }
        public void formatJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Justify;
        }
        public void formatRightJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Right;
        }
        public void formatCenterJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Center;
        }

        public void formatStrikeout(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Strikeout == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Strikeout);

        }
        public void formatStrikeout(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            if (rtb.Strikeout)
            {
                rtb.Strikeout = false;
                button.Checked = false;
            }
            else
            {
                rtb.Strikeout = true;
                button.Checked = true;
            }
        }


        public void formatUnderline(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Underline == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Underline);

        }
        public void formatForceSetUnsetBold(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Bold)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Bold);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Bold)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Bold);
                }
            }

        }
        public void formatForceSetUnsetBold(WpfRichTextBoxEx rtb, ToolStripMenuItem button, bool set)
        {
            rtb.Bold = set;
        }
        public void formatForceSetUnsetItalics(WpfRichTextBoxEx rtb, ToolStripMenuItem button, bool set)
        {
            rtb.Italic = set;
        }
        public void formatForceSetUnsetUnderline(WpfRichTextBoxEx rtb, ToolStripMenuItem button, bool set)
        {
            rtb.Underline= set;
        }
        public void formatForceSetUnsetStrikeout(WpfRichTextBoxEx rtb, ToolStripMenuItem button, bool set)
        {
            rtb.Strikeout = set;
        }

        public void formatForceSetUnsetItalics(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Italic)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Italic);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Italic)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Italic);
                }
            }
        }
        public void formatForceSetUnsetUnderline(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Underline)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Underline);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Underline)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Underline);
                }
            }
        }
        public void formatForceSetUnsetStrikeout(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Strikeout)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Strikeout);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Strikeout)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Strikeout);
                }
            }
        }
        public void formatForceRemoveAllFormatting(AdvRichTextBox rtb)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                rtb.SelectionFont = new Font(rtb.Font.Name, 14, System.Drawing.FontStyle.Regular);
                rtb.SelectionBackColor = System.Drawing.Color.White;
                rtb.SelectionColor = System.Drawing.Color.Black;
            }
        }
        public void formatForceRemoveAllFormatting(WpfRichTextBoxEx rtb)
        {
            TextSelection selection = rtb.Selection;
            rtb.Italic = rtb.Bold = rtb.Underline = rtb.Strikeout = false;
            rtb.SelectionColor = System.Drawing.Color.Black;
            rtb.SelectionBackColor = System.Drawing.Color.White;
        }
        public void formatBold(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            if (rtb.Bold)
            {
                rtb.Bold = false;
                button.Checked = false;
            }
            else
            {
                rtb.Bold = true;
                button.Checked = true;
            }
        }
        public void formatItalics(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            if (rtb.Italic)
            {
                rtb.Italic = false;
                button.Checked = false;
            }
            else
            {
                rtb.Italic = true;
                button.Checked = true;
            }
        }

        public void formatUnderline(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            if (rtb.Underline)
            {
                rtb.Underline = false;
                button.Checked = false;
            }
            else
            {
                rtb.Underline = true;
                button.Checked = true;
            }
        }

        public void formatBold(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Bold == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Bold);

        }
        public void formatItalics(AdvRichTextBox rtb, ToolStripButton button)
        {
            
            if (rtb.SelectionFont.Italic == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Italic);
            
            /*
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                if (!rtb.SelectionFont.Italic)
                rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | FontStyle.Italic);
            }
            */
        }
        public void formatIncreaseFontSize(AdvRichTextBox rtb)
        {
            if (rtb.SelectionFont == null)
                return;

            if (rtb.SelectionFont.Size >= 299)
                return;

            rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, rtb.SelectionFont.Size + 1, rtb.SelectionFont.Style);
        }
        public void formatIncreaseFontSize(WpfRichTextBoxEx rtb)
        {
            if (rtb.SelectionFontSize >= 299)
                return;

            rtb.SelectionFontSize += 1;
        }
        public void formatDecreaseFontSize(AdvRichTextBox rtb)
        {
            if (rtb.SelectionFont == null)
                return;

            if (rtb.SelectionFont.Size <= 8)
                return;

            rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, rtb.SelectionFont.Size - 1, rtb.SelectionFont.Style);
        }
        public void formatDecreaseFontSize(WpfRichTextBoxEx rtb)
        {
            if (rtb.SelectionFontSize <= 8)
                return;

            rtb.SelectionFontSize -= 1;
        }

        public void formatLineSpacing(AdvRichTextBox rtb, RichTextBoxEx.LineSpaceTypes type)
        {
            rtb.LineSpace(type, false);
        }
        public void formatLineSpacing(WpfRichTextBoxEx rtb, double value)
        {
            rtb.SetValue(Paragraph.LineHeightProperty, value);
        }

        public void formatIndentRight(AdvRichTextBox rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            rtb.SelectionIndent += 30;
            buttonRightIndent.Checked = true;
            buttonLeftIndent.Checked = false;
        }
        public void formatIndentLeft(AdvRichTextBox rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            if (rtb.SelectionIndent > 30)
            {
                rtb.SelectionIndent -= 30;
            }
            else if (rtb.SelectionIndent == 30)
            {
                rtb.SelectionIndent -= 30;
                buttonRightIndent.Checked = false;
                buttonLeftIndent.Checked = true;
            }
        }
        public void formatIndentRight(WpfRichTextBoxEx rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            rtb.SelectionIndent += 30;
            buttonRightIndent.Checked = true;
            buttonLeftIndent.Checked = false;
        }
        public void formatIndentLeft(WpfRichTextBoxEx rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            if (rtb.SelectionIndent > 30)
            {
                rtb.SelectionIndent -= 30;
            }
            else if (rtb.SelectionIndent == 30)
            {
                rtb.SelectionIndent -= 30;
                buttonRightIndent.Checked = false;
                buttonLeftIndent.Checked = true;
            }
        }

        public void formatBullets(AdvRichTextBox rtb, ToolStripButton buttonBullets, ToolStripButton buttonNumberedList)
        {
            if (rtb.SelectionBullet == true)
            {
                buttonBullets.Checked = false;
                rtb.SelectionBullet = false;
            }
            else
            {
                rtb.NumberedBullet(false);
                buttonBullets.Checked = true;
                rtb.SelectionBullet = true;
            }
        }
        public void formatBullets(WpfRichTextBoxEx rtb)
        {
            EditingCommands.ToggleBullets.Execute(null, rtb);
        }
        public void formatNumberedList(WpfRichTextBoxEx rtb)
        {
            EditingCommands.ToggleNumbering.Execute(null, rtb);
        }

        public void formatNumberedList(AdvRichTextBox rtb, ToolStripButton buttonBullets, ToolStripButton buttonNumberedList)
        {
            // rtf insertion
        //    rtb.SelectedRtf = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Calibri;}}" +
          //      @"{\*\generator Msftedit 5.41.21.2510; }\viewkind4\uc1\pard{\pntext\f0 1.\tab}" +
            //                @"{\*\pn\pnlvlbody\pnf0\pnindent0\pnstart1\pndec{\pntxta.} }" +
              //  @"\fi \li720\sa200\sl276\slmult1\lang9\f0\fs22 \par" +
                //@"}";
            
            if (buttonNumberedList.Checked == true)
            {
                buttonNumberedList.Checked = false;
                rtb.NumberedBullet(false);
            }
            else
            {
                rtb.SelectionBullet = false;
                buttonNumberedList.Checked = true;
                rtb.BulletType = RichTextBoxEx.AdvRichTextBulletType.Number;
                rtb.NumberedBullet(true);
            }
            
        }

        public void formatFont(AdvRichTextBox rtb, String fontName)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                rtb.SelectionFont = new Font(fontName, rtb.SelectionFont.Size, rtb.SelectionFont.Style);
            }

        }
        public void formatFont(WpfRichTextBoxEx rtb, String fontName)
        {
            //rtb.Focus();
            //TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            //if (selection == null) return;

            ////System.Windows.Media.FontFamily oldfont = (System.Windows.Media.FontFamily)selection.GetPropertyValue(TextElement.FontFamilyProperty);
            //System.Windows.Media.FontFamily newfont = new System.Windows.Media.FontFamily(fontName);
            //selection.ApplyPropertyValue(TextElement.FontFamilyProperty, newfont);
            rtb.SelectionFont = new Font(fontName, 14);
        }

        public void formatFontSize(AdvRichTextBox rtb, int size)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, size, rtb.SelectionFont.Style);
            }
        }
        public void formatFontSize(WpfRichTextBoxEx rtb, double size)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;

            //int end =  selStartIndex + selLength;
            //int start = selStartIndex;
            //for (int index = start; index < end; index++)
            selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
        }

        public void formatInsertDateTime(AdvRichTextBox rtb, DateTime dateTime)
        {
            rtb.SelectedText = dateTime.ToString("hh:mm:ss tt") + ", " + dateTime.ToString("dddd, dd MMMM yyyy");
        }
        public void formatInsertDateTime(WpfRichTextBoxEx rtb, DateTime dateTime)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;

            selection.Start.GetInsertionPosition(LogicalDirection.Forward).InsertTextInRun(
                dateTime.ToString("hh:mm:ss tt") + ", " + dateTime.ToString("dddd, dd MMMM yyyy"));
        }
        public void formatInsertTime(AdvRichTextBox rtb, DateTime dateTime)
        {
            rtb.SelectedText = dateTime.ToString("hh:mm:ss tt");
        }
        public void formatInsertTime(WpfRichTextBoxEx rtb, DateTime dateTime)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;            

            selection.Start.GetInsertionPosition(LogicalDirection.Forward).InsertTextInRun(dateTime.ToString("hh:mm:ss tt"));
        }
        public void formatInsertDate(AdvRichTextBox rtb, DateTime dateTime)
        {
            rtb.SelectedText = dateTime.ToString("dddd, dd MMMM yyyy");
        }
        public void formatInsertDate(WpfRichTextBoxEx rtb, DateTime dateTime)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;

            selection.Start.GetInsertionPosition(LogicalDirection.Forward).InsertTextInRun(dateTime.ToString("dddd, dd MMMM yyyy"));
        }

        public void formatFontColor(AdvRichTextBox rtb, System.Drawing.Color color)
        {
            rtb.SelectionColor = color;
        }
        public void formatFontColor(WpfRichTextBoxEx rtb, System.Drawing.Color color)
        {
            rtb.SelectionColor = color;

        }
        public void formatBackColor(AdvRichTextBox rtb, System.Drawing.Color color)
        {
            rtb.SelectionBackColor = color;
        }
        public void formatBackColor(WpfRichTextBoxEx rtb, System.Drawing.Color color)
        {
            rtb.SelectionBackColor = color;
        }
        public bool formatInsertTableColumn(WpfRichTextBoxEx rtb, bool left)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            // get selected cell's column width
            TableColumn selCellColumn = table.Columns[cellIndex];

            // got params, now insert
            TableColumn column = new TableColumn();
            column.Width = selCellColumn.Width;
            if (!left) cellIndex++; // insert right to the selected cell
            return rtb.InsertTableColumn(ref table, ref column, ref cell, cellIndex);
        }
        public bool formatDeleteTableColumn(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            // remove the entire column and it's structure from table
            return rtb.RemoveTableColumn(ref table, cellIndex);
        }
        public bool formatDeleteTableRow(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            // remove the entire column and it's structure from table
            return rtb.RemoveTableRow(ref table, rowIndex);
        }
        public bool formatDeleteTable(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            // remove the table from document
            return rtb.Document.Blocks.Remove(table);
        }
        public bool formatInsertTableRow(WpfRichTextBoxEx rtb, bool above)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            // got params, now insert
            TableRow newRow = new TableRow();
            rtb.CopyTableRow(ref row, ref newRow);
            if (!above) rowIndex++; // insert below the selected row
            return rtb.InsertTableRow(ref table, ref newRow, rowIndex);
        }
        public bool formatSetColumnWidth(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            List<string> widths = new List<string>();

            int width = 100;
            widths.Add(50.ToString());
            for (int i = 0; i < 30; i++)
            {
                widths.Add(width.ToString());
                width += 100;
            }

            // get selected cell's column width
            TableColumn selCellColumn = table.Columns[cellIndex];

            // ask user
            String input = selCellColumn.Width.ToString();
            if (userInterface.ShowListInputDialog("choose column width", ref input, ref widths, 2) != DialogResult.OK)
                return false;

            // check validity
            int value = -1;
            if (!int.TryParse(input, out value))
                return false;

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);
            
            // find columns by cells
            List<TableColumn> columns = null;
            rtb.GetTableColumnsByCells(ref table, ref row, ref cells, out columns);

            // finally set the width
            return rtb.SetTableColumnsWidth(ref table, ref columns, value);
        }

        public bool formatCells(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            IEnumerable<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // prepare formatting
            FormInsertTable form = new FormInsertTable();
            form.isEditingCells = true;
            form.Italic = rtb.WpfItalicToWinFormsItalic(cell.FontStyle);
            form.Bold = rtb.WpfBoldToWinFormsBold(cell.FontWeight);
            form.FontColor = ((cell.Foreground != null) ? WpfRichTextBoxEx.WpfBrushToWinFormsColor((SolidColorBrush)cell.Foreground) : System.Drawing.Color.Black);
            form.FontBackColor = ((cell.Background != null) ? WpfRichTextBoxEx.WpfBrushToWinFormsColor((SolidColorBrush)cell.Background) : System.Drawing.Color.White);
            form.font = rtb.WpfFontToWinFormsFont(cell.FontFamily);
            form.fontName = form.font.Name;
            form.fontSize = cell.FontSize;
            if (form.ShowDialog() != DialogResult.OK) return false;
            form.Dispose();

            // configure
            return rtb.FormatTableCell(ref table, ref cells, rtb.WinFormsFontToWpfFont(form.font), (int)form.fontSize,
                form.Bold, form.Italic, form.cellAlignment, rtb.WinFormsColorToWpfColor(form.FontColor), rtb.WinFormsColorToWpfColor(form.FontBackColor),
                new Thickness(form.tableInnerBorderSize), new Thickness(form.tableOuterBorderSize));
        }
        public bool formatCellsRemoveAllFormatting(WpfRichTextBoxEx rtb)
        {
            Table? table = null;
            TableRow? row = null;
            TableCell? cell = null;
            int rowIndex = -1;
            int cellIndex = -1;

            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // get selected cell and other parms
            if (!rtb.GetSelectedTableParams(selection, out table, out row, out cell, out rowIndex, out cellIndex))
                return false;

            IEnumerable<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // configure
            return rtb.FormatTableCellRemoveAllFormatting(ref cells);
        }

        public static DialogResult showInsertTableDialog(bool IsEditingCells, out FormInsertTable formObect)
        {
            FormInsertTable formInsertTable = new FormInsertTable();
            formInsertTable.isEditingCells = IsEditingCells;
            formInsertTable.ShowDialog();
            formObect = formInsertTable;
            return formInsertTable.myResult;

        }

        public bool formatInsertTable(WpfRichTextBoxEx rtb)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return false;

            // prepare formatting
            FormInsertTable form = new FormInsertTable();
            if (form.ShowDialog() != DialogResult.OK) return false;
            form.Dispose();

            // initialize new table
            Table table = new Table();
            table.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            table.BorderThickness = new Thickness(form.tableOuterBorderSize);

            // configure table
            table.FontSize = form.fontSize;
            table.FontFamily = new System.Windows.Media.FontFamily(form.fontName);

            for (int j = 0; j < form.columns; j++)
            {
                table.Columns.Add(new TableColumn());
                rtb.SetTableColumnWidth(ref table, j, form.columnWidth);
            }

            // configure rows and columns

            System.Windows.Documents.TableRowGroup rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            int cellindex = 0;
            for (int i = 0; i < form.rows; i++)
            {
                TableRow row = new TableRow();

                for (int j = 0; j < form.columns; j++)
                {
                    TableCell cell = new TableCell();
                    cell.TextAlignment = form.cellAlignment;
                    //cell.FontSize = form.fontSize;
                    //cell.FontFamily = new System.Windows.Media.FontFamily(form.fontName);
                    //cell.FontStyle = rtb.WinFormsItalicToWpfItalic(form.Italic);
                    //cell.FontWeight = rtb.WinFormsBoldToWpfBold(form.Bold);
                    cell.Foreground = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.FontColor);
                    cell.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.FontBackColor);
                    Paragraph para = new Paragraph();
                    para.TextAlignment = form.cellAlignment;
                    para.FontSize = form.fontSize;
                    para.FontFamily = new System.Windows.Media.FontFamily(form.fontName);
                    para.FontStyle = rtb.WinFormsItalicToWpfItalic(form.Italic);
                    para.FontWeight = rtb.WinFormsBoldToWpfBold(form.Bold);
                    para.Foreground = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.FontColor);
                    para.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.FontBackColor);
                    para.Inlines.Add("cell" + cellindex++.ToString());
                    cell.Blocks.Add(para);
                    cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                    cell.BorderThickness = new Thickness(form.tableInnerBorderSize);
                    row.Cells.Add(cell);
                }
                rowGroup.Rows.Add(row);
            }

            // insert block
            rtb.InsertBlockAtSelection(table);
            return true;
        }

        public void formatInsertImage(AdvRichTextBox rtb, System.Drawing.Image image)
        {
            if (image == null)
                return;

            //using (var bmp = new Bitmap(image)
            //{
                rtb.SelectedRtf = ImageToRtf.Convert(image, System.Drawing.Color.White);
            //}
        }

        public void formatInsertImage(WpfRichTextBoxEx rtb, System.Drawing.Image image)
        {
            if (image == null)
                return;

            var orgdata = System.Windows.Forms.Clipboard.GetDataObject;
            System.Windows.Forms.Clipboard.SetImage(image);
            rtb.Paste();
            System.Windows.Forms.Clipboard.SetDataObject(orgdata);
        }
    }

    public static class ImageToRtf
    {
        public static void Write16(StringBuilder sb, params int[] args)
        {
            foreach (int w in args)
            {
                sb.AppendFormat("{0:X2}{1:X2}", w & 0xff, (w >> 8) & 0xff);
            }
        }

        public static string Convert(System.Drawing.Image img, System.Drawing.Color back)
        {
            int w = img.Width, h = img.Height;
            int picw = w * 2540 / 96, pich = h * 2540 / 96;
            int picwgoal = w * 1440 / 96, pichgoal = h * 1440 / 96;

            var sb = new StringBuilder();
            sb.AppendFormat(
                @"{{\rtf1{{\pict\wmetafile8\picw{0}\pich{1}\picwgoal{2}\pichgoal{3} ",
                picw, pich, picwgoal, pichgoal);

            byte[] bmp;
            using (var bitmap = new Bitmap(w, h))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(back);
                    g.DrawImage(img, 0, 0, w, h);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Bmp);
                    bmp = ms.ToArray();
                }
            }

            int size1 = 14 + (bmp.Length - 14) / 2;
            int size2 = 9 + 5 + 5 + size1 + 3;

            Write16(sb,
                // META_HEADER Record
                1, 9, 0x300, size2, size2 >> 16, 0, size1, size1 >> 16, 0,
                // META_SETWINDOWORG Record
                5, 0, 0x20b, 0, 0,
                // META_SETWINDOWEXT Record
                5, 0, 0x20c, pich, picw,
                // META_STRETCHDIB Record
                size1, size1 >> 16, 0xf43, 0x20, 0xcc, 0, h, w, 0, 0, pich, picw, 0, 0
            );
            sb.Append(BitConverter.ToString(bmp, 14).Replace("-", ""));
            Write16(sb, 3, 0, 0); // META_EOF Record
            sb.Append("}}");

            return sb.ToString();
        }
    }
}
