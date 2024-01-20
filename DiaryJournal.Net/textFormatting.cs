using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

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
        public void formatUnderline(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;

            if (rtb.Underline)
            {
                //                rtb.Underline = false;
                //                rtb.toggleSelectionAllDecorations(selection, false, rtb.Strikeout);
                rtb.toggleSelectionDecorations(selection, false, false, true, false);
                button.Checked = false;
            }
            else
            {
                rtb.toggleSelectionDecorations(selection, true, false, true, false);
                //                rtb.Underline = true;
                button.Checked = true;
            }
        }
        public void formatStrikeout(WpfRichTextBoxEx rtb, ToolStripButton button)
        {
            rtb.Focus();
            TextSelection selection = ((rtb.IsSelectionActive) ? rtb.Selection : ((rtb.sel != null) ? rtb.sel : rtb.Selection));
            if (selection == null) return;

            if (rtb.Strikeout)
            {
                //                rtb.Strikeout = false;
                rtb.toggleSelectionDecorations(selection, false, false, false, true);
                button.Checked = false;
            }
            else
            {
                //                rtb.Strikeout = true;
                rtb.toggleSelectionDecorations(selection, false, true, false, true);
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
        public void formatPasteRawText(WpfRichTextBoxEx rtb)
        {
            String text = System.Windows.Clipboard.GetText();
            rtb.Text = text;
        }
        public void formatConvertToRawText(WpfRichTextBoxEx rtb)
        {
            rtb.SelectAll();
            String text = rtb.Text;
            rtb.Document.Blocks.Clear();
            rtb.SelectAll();
            rtb.Text = text;
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
            rtb.SelectionBackColor = System.Drawing.Color.Transparent;
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
        public void formatLineSpacing(System.Windows.Controls.RichTextBox rtb, double value)
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

            // columns config
            int maxColumns = WpfRichTextBoxEx.MAX_TABLE_CELLS;
            int totalColumns = table.Columns.Count;

            // validate
            if (totalColumns >= maxColumns) return false;

            // prepare input box
            List<Object> numColStrings = new List<Object>();
            for (int i = 1; i <= (maxColumns - totalColumns); i++)
                numColStrings.Add(i.ToString());

            // ask user
            String input = 1.ToString();
            if (userInterface.ShowListInputDialog("insert how many columns?", ref input, ref numColStrings, -1) != DialogResult.OK)
                return false;

            // check validity
            int numColumns = -1;
            if (!int.TryParse(input, out numColumns))
                return false;

            // validate
            if (numColumns <= 0) return false;
            if ((numColumns + totalColumns) > maxColumns) return false;

            if (!left) cellIndex++; // insert right to the selected cell
            TableColumn? target = ((cellIndex < table.Columns.Count) ? table.Columns[cellIndex] : null);

            // insert a number of columns
            for (int i = 0; i < numColumns; i++)
            {
                // got params, now insert
                TableColumn column = new TableColumn();
                column.Width = selCellColumn.Width;
                rtb.InsertTableColumn(ref table, ref column, ref target, ref cell);
            }
            return true;
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

            List<TableCell>? cells = selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // find columns by cells
            List<TableColumn> columns = null;
            if (!rtb.GetTableColumnsByCells(ref table, ref row, ref cells, out columns)) return false;

            // remove the entire column and it's structure from table
            foreach (TableColumn listedColumn in columns)
            {
                TableColumn col = listedColumn;
                rtb.RemoveTableColumn(ref table, ref col);
            }
            return true;
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

            List<TableCell>? cells = selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // get all selected rows
            List<TableRow>? rows = null;
            rtb.GetTableRowsByCells(ref table, ref cells, out rows);

            // remove selected rows
            foreach (TableRow listedRow in rows)
            {
                TableRow thisRow = listedRow;
                rtb.RemoveTableRow(ref table, ref thisRow);
            }
            return true;
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

            if (rtb.ObjectAtSelectionStart() is not Table)
                return false; // we cannot insert table inside table.

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

            // prepare input box
            List<Object> numRowsStrings = new List<Object>();
            for (int i = 1; i <= 1000; i++)
                numRowsStrings.Add(i.ToString());

            // ask user
            String input = 1.ToString();
            if (userInterface.ShowListInputDialog("insert how many rows?", ref input, ref numRowsStrings, -1) != DialogResult.OK)
                return false;

            // check validity
            int numRows = -1;
            if (!int.TryParse(input, out numRows))
                return false;

            if (!above) rowIndex++; // insert below the selected row
            TableRow? target = ((rowIndex < table.RowGroups[0].Rows.Count) ? table.RowGroups[0].Rows[rowIndex] : null);

            // insert a number of rows
            for (int i = 0; i < numRows; i++)
            {
                // got params, now insert
                TableRow newRow = new TableRow();
                rtb.CopyTableRow(ref row, ref newRow);
                rtb.InsertTableRow(ref table, ref newRow, ref target);
            }
            return true;
        }
        public bool formatSelectTableRow(WpfRichTextBoxEx rtb)
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

            // prepare input box
            List<Object> numRowsStrings = new List<Object>();
            for (int i = 1; i <= 1000; i++)
                numRowsStrings.Add(i.ToString());

            // ask user
            String input = 1.ToString();
            if (userInterface.ShowListInputDialog("select how many rows?", ref input, ref numRowsStrings, -1) != DialogResult.OK)
                return false;

            // check validity
            int numRows = -1;
            if (!int.TryParse(input, out numRows))
                return false;

            // get all selected rows
            List<TableRow>? rows = null;
            if (!rtb.GetTableRows(ref table, ref row, numRows, out rows))
                return false;

            // select
            TableRow? lastRow = rows.LastOrDefault();
            rtb.Selection.Select(row.ElementStart, lastRow.ElementEnd);
            return true;
        }

        public void cutParagraphRaw(WpfRichTextBoxEx rtb)      
        {
            rtb.Focus();

            var curCaret = rtb.CaretPosition;
            Block? block = rtb.Selection.Start.Paragraph;
            if (block == null) return;
            Paragraph? p = null;
            if (block is Paragraph)
                p = (Paragraph?)block;

            String text = "";
            TextRange range = new TextRange(p.ContentStart, p.ContentEnd);
            text = range.Text;
            rtb.Document.Blocks.Remove(block);
            if (text == "") return;
            System.Windows.Forms.Clipboard.SetText(text);
        }
        public bool selectTable(WpfRichTextBoxEx rtb)
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

            // if selection contains no table, return error
            if (table == null) return false;

            // set selection
            rtb.Selection.Select(table.ElementStart, table.ElementEnd);
            return true;
        }
        public bool formatSetColumnWidth(WpfRichTextBoxEx rtb, bool allColumns)
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

            List<Object> widths = new List<Object>();

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
            if (userInterface.ShowListInputDialog("choose column width", ref input, ref widths, -1) != DialogResult.OK)
                return false;

            // check validity
            int value = -1;
            if (!int.TryParse(input, out value))
                return false;

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);
            
            // find columns by cells
            List<TableColumn> columns = null;

            if (allColumns)
                columns = table.Columns.ToList();
            else
                rtb.GetTableColumnsByCells(ref table, ref row, ref cells, out columns);

            // finally set the width
            return rtb.SetTableColumnsWidth(ref table, ref columns, value);
        }

        public bool showTableFormattingDialog(WpfRichTextBoxEx rtb, ref TableCell cell, ref FormInsertTable? outForm)
        {
            if (cell == null) return false;

            // get the base paragraph and inline of first selected cell
            Paragraph para = null;
            para = (Paragraph)cell.Blocks.FirstOrDefault();

            System.Windows.Media.Brush? forebrush = null;
            System.Windows.Media.Brush? backbrush = null;
            TextRange range = null; 
            Inline? inline = para.Inlines.Where(x => x is Run || x is Inline || x is Span).FirstOrDefault();
            if (inline != null)
            {
                forebrush = inline.Foreground;
                backbrush = inline.Background;
                range = new TextRange(inline.ContentStart, inline.ContentEnd);
            }
            else
            {
                forebrush = para.Foreground; // empty paragraph, so set paragraph's own foreground
                backbrush = para.Background;
                range = new TextRange(para.ContentStart, para.ContentEnd);
            }

            // we pick paragraph's background if inline does not provides it
            if (backbrush == null) backbrush = para.Background;

            // if brush is null meaning empty paragraph and paragraph has no brush/color set,
            // or inline exists, but inline has no brush/color set, so auto-select default colors.
            if (forebrush == null) forebrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            if (backbrush == null) backbrush = new SolidColorBrush(System.Windows.Media.Colors.White);

            // get decorations
            bool underline = false;
            bool strikeout = false;
            rtb.GetDecorations(inline, ref underline, ref strikeout);

            // get colors
            System.Drawing.Color forecolor = WpfRichTextBoxEx.WpfBrushToWinFormsColor((SolidColorBrush)forebrush);
            System.Drawing.Color backcolor = WpfRichTextBoxEx.WpfBrushToWinFormsColor((SolidColorBrush)backbrush);

            // prepare formatting, we preselect first selected cell's config as default
            FormInsertTable form = new FormInsertTable();
            form.isEditingCells = true;
            form.cellAlignment = ((para != null) ? para.TextAlignment : TextAlignment.Left);
            form.Italic = ((para != null) ? rtb.WpfItalicToWinFormsItalic(para.FontStyle) : rtb.WpfItalicToWinFormsItalic(FontStyles.Normal));
            form.Bold = ((para != null) ? rtb.WpfBoldToWinFormsBold(para.FontWeight) : rtb.WpfBoldToWinFormsBold(FontWeights.Normal));
            form.Underline = underline;
            form.Strikeout = strikeout;
            form.fontColor = forecolor;
            form.fontBackColor = backcolor;
            form.font = ((para != null) ? rtb.WpfFontToWinFormsFont(para.FontFamily) : rtb.WpfFontToWinFormsFont(new System.Windows.Media.FontFamily("Times New Roman")));
            form.fontName = form.font.Name;
            form.fontSize = ((para != null) ? (int)para.FontSize : (int)cell.FontSize);

            // show table formatting dialog
            if (form.ShowDialog() != DialogResult.OK) return false;
            form.Dispose();

            // output
            outForm = form;
            return true;
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

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // display table formatting dialog
            FormInsertTable? form = null;
            if (!showTableFormattingDialog(rtb, ref cell, ref form)) return false;

            // format the current row's target cells
            return rtb.FormatTableCell(ref table, ref cells, rtb.WinFormsFontToWpfFont(form.font), (int)form.fontSize,
                form.Bold, form.Italic, form.Underline, form.Strikeout, form.cellAlignment, rtb.WinFormsColorToWpfColor(form.fontColor), rtb.WinFormsColorToWpfColor(form.fontBackColor),
                new Thickness(form.tableInnerBorderSize), new Thickness(form.tableOuterBorderSize));
        }

        public bool formatColumns(WpfRichTextBoxEx rtb, bool removeFormatting)
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

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // find columns by cells
            List<int> columns = null;
            rtb.GetTableColumnIndexesByCells(ref table, ref row, ref cells, out columns);

            // display table formatting dialog
            FormInsertTable? form = null;

            if (!removeFormatting)
            {
                if (!showTableFormattingDialog(rtb, ref cell, ref form)) return false;
            }

            // configure
            foreach (TableRow listedRow in table.RowGroups[0].Rows)
            {
                TableRow? thisRow = listedRow;
                List<TableCell>? rowCells = null;

                // get current row's target cells
                rtb.GetTableCellsByIndexes(ref table, ref thisRow, ref columns, out rowCells);

                if (removeFormatting)
                {
                    // remove all formatting of current row's cells
                    rtb.FormatTableCellRemoveAllFormatting(ref rowCells);
                }
                else
                {
                    // format the current row's target cells
                    rtb.FormatTableCell(ref table, ref rowCells, rtb.WinFormsFontToWpfFont(form.font), (int)form.fontSize,
                        form.Bold, form.Italic, form.Underline, form.Strikeout, form.cellAlignment, rtb.WinFormsColorToWpfColor(form.fontColor), rtb.WinFormsColorToWpfColor(form.fontBackColor),
                        new Thickness(form.tableInnerBorderSize), new Thickness(form.tableOuterBorderSize));
                }
            }
            return true;
        }
        public bool formatTable(WpfRichTextBoxEx rtb, bool removeFormatting)
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

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // display table formatting dialog
            FormInsertTable? form = null;

            if (!removeFormatting)
            {
                if (!showTableFormattingDialog(rtb, ref cell, ref form)) return false;
            }

            // configure whole table
            for (int rowGroupIndex = 0; rowGroupIndex < table.RowGroups.Count; rowGroupIndex++)
            {
                TableRowGroup rowGroup = table.RowGroups[rowGroupIndex];
                foreach (TableRow listedRow in rowGroup.Rows)
                {
                    TableRow? thisRow = listedRow;
                    List<TableCell>? rowCells = thisRow.Cells.ToList();

                    if (removeFormatting)
                    {
                        // remove all formatting of current row's cells
                        rtb.FormatTableCellRemoveAllFormatting(ref rowCells);
                    }
                    else
                    {
                        // format the current row's cells
                        rtb.FormatTableCell(ref table, ref rowCells, rtb.WinFormsFontToWpfFont(form.font), (int)form.fontSize,
                            form.Bold, form.Italic, form.Underline, form.Strikeout, form.cellAlignment, rtb.WinFormsColorToWpfColor(form.fontColor), rtb.WinFormsColorToWpfColor(form.fontBackColor),
                            new Thickness(form.tableInnerBorderSize), new Thickness(form.tableOuterBorderSize));
                    }
                }
            }
            return true;
        }

        public bool formatRows(WpfRichTextBoxEx rtb, bool removeFormatting)
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

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);


            // display table formatting dialog
            FormInsertTable? form = null;

            if (!removeFormatting)
            {
                if (!showTableFormattingDialog(rtb, ref cell, ref form)) return false;
            }

            // get all selected rows
            List<TableRow>? rows = null;
            rtb.GetTableRowsByCells(ref table, ref cells, out rows);
            // configure rows
            foreach (TableRow listedRow in rows)
            {
                TableRow? thisRow = listedRow;
                List<TableCell>? rowCells = thisRow.Cells.ToList();

                if (removeFormatting)
                {
                    // remove all formatting of current row's cells
                    rtb.FormatTableCellRemoveAllFormatting(ref rowCells);
                }
                else
                {
                    // format the current row's cells
                    rtb.FormatTableCell(ref table, ref rowCells, rtb.WinFormsFontToWpfFont(form.font), (int)form.fontSize,
                        form.Bold, form.Italic, form.Underline, form.Strikeout, form.cellAlignment, rtb.WinFormsColorToWpfColor(form.fontColor), rtb.WinFormsColorToWpfColor(form.fontBackColor),
                        new Thickness(form.tableInnerBorderSize), new Thickness(form.tableOuterBorderSize));
                }
            }
            return true;
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

            List<TableCell>? cells = rtb.Selection.EnumerateElements<TableCell>().ToList();
            //IEnumerable<string> cellValues = cells.Select(cell => new TextRange(cell.ContentStart, cell.ContentEnd).Text);

            // format the current row's target cells
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

            if (rtb.ObjectAtSelectionStart() is Table)
                return false; // we cannot insert table inside table.

            // prepare formatting
            FormInsertTable form = new FormInsertTable();
            if (form.ShowDialog() != DialogResult.OK) return false;
            form.Dispose();
            
            // validate
            if (form.columns <= 0 || form.columns > WpfRichTextBoxEx.MAX_TABLE_CELLS)
                return false;

            // initialize new table
            Table table = new Table();

            // configure table
            /*
            table.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            table.BorderThickness = new Thickness(form.tableOuterBorderSize);
            table.TextAlignment = form.cellAlignment;
            table.FontSize = form.fontSize;
            table.FontFamily = new System.Windows.Media.FontFamily(form.fontName);
            table.FontStyle = rtb.WinFormsItalicToWpfItalic(form.Italic);
            table.FontWeight = rtb.WinFormsBoldToWpfBold(form.Bold);
            table.Foreground = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontColor);
            table.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontBackColor);
            */
            table.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontBackColor);

            // phase 1 - insert table block set

            var curCaret = rtb.CaretPosition;
            Block? currentSelBlock = rtb.Selection.Start.Paragraph;//rtb.Document.Blocks.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
            Block paraTop = new Paragraph(new Run(""));
            Block paraBottom = new Paragraph(new Run(""));

            if (currentSelBlock != null)
                rtb.Document.Blocks.InsertAfter(currentSelBlock, paraTop);
            else
                rtb.Document.Blocks.Add(paraTop);

            rtb.Document.Blocks.InsertAfter(paraTop, table);
            rtb.Document.Blocks.InsertAfter(table, paraBottom);

            // phase 2 - configure table's elements

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
                rowGroup.Rows.Add(row);
                //row.FontSize = form.fontSize;
                //row.FontFamily = new System.Windows.Media.FontFamily(form.fontName);
                //row.FontStyle = rtb.WinFormsItalicToWpfItalic(form.Italic);
                //row.FontWeight = rtb.WinFormsBoldToWpfBold(form.Bold);
                //row.Foreground = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontColor);
                //row.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontBackColor);

                for (int j = 0; j < form.columns; j++)
                {
                    TableCell cell = new TableCell();
                    row.Cells.Add(cell);

                    cell.TextAlignment = form.cellAlignment;
                    //cell.FontSize = form.fontSize;
                    //cell.FontFamily = new System.Windows.Media.FontFamily(form.fontName);
                    //cell.FontStyle = rtb.WinFormsItalicToWpfItalic(form.Italic);
                    //cell.FontWeight = rtb.WinFormsBoldToWpfBold(form.Bold);
                    //cell.Foreground = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontColor);
                    cell.Background = WpfRichTextBoxEx.WinFormsColorToWpfBrush(form.fontBackColor);
                    cell.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                    cell.BorderThickness = new Thickness(form.tableInnerBorderSize);
                }
            }

            // phase 3 - formatting

            cellindex = 0;
            foreach (TableRow row in table.RowGroups[0].Rows)
            {
                foreach (TableCell cell in row.Cells)
                {

                    Paragraph para = new Paragraph();
                    Block? block = (Block?)para;
                    cell.Blocks.Add(para);
                    Run run = new Run("cell" + cellindex++.ToString());
                    para.Inlines.Add(run);

                    rtb.FormatBlockItalicRecursive(ref block, form.Italic);
                    rtb.FormatBlockBoldRecursive(ref block, form.Bold);
                    rtb.FormatBlockUnderlineRecursive(ref block, form.Underline);
                    rtb.FormatBlockStrikeoutRecursive(ref block, form.Strikeout);
                    rtb.FormatBlockForegroundRecursive(ref block, rtb.WinFormsColorToWpfColor(form.fontColor));
                    rtb.FormatBlockBackgroundRecursive(ref block, rtb.WinFormsColorToWpfColor(form.fontBackColor));
                    rtb.FormatBlockFontRecursive(ref block, new System.Windows.Media.FontFamily(form.fontName));
                    rtb.FormatBlockFontSizeRecursive(ref block, form.fontSize);
                    rtb.FormatBlockTextAlignmentRecursive(ref block, form.cellAlignment);
                }
            }

            return true;
        }

        public void formatInsertParagraph(WpfRichTextBoxEx rtb)
        {
            TextPointer curCaret = rtb.CaretPosition;
            Block? currentSelBlock = rtb.ObjectAtCaretPosition();
            Block paraTop = new Paragraph(new Run(" "));
            //Block paraBottom = new Paragraph(new Run(""));

            if (currentSelBlock == null)
                currentSelBlock = rtb.Selection.Start.Paragraph;
            
            if (currentSelBlock != null)
                rtb.Document.Blocks.InsertAfter(currentSelBlock, paraTop);
            else
                rtb.Document.Blocks.Add(paraTop);

            //rtb.Document.Blocks.InsertAfter(paraTop, paraBottom);

            rtb.CaretPosition = paraTop.ContentStart;
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

            var curCaret = rtb.CaretPosition;
            Block? currentSelBlock = rtb.Selection.Start.Paragraph;
            Block paraTop = new Paragraph(new Run(""));

            if (currentSelBlock != null)
                rtb.Document.Blocks.InsertAfter(currentSelBlock, paraTop);
            else
                rtb.Document.Blocks.Add(paraTop);

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
