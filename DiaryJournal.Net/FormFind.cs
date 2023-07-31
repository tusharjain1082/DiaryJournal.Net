#define UNICODE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace DiaryJournal.Net
{
    public partial class FormFind : Form
    {
        public WpfRichTextBoxEx rtb;
        public Form myParentForm;
        String searchPattern = "";
        String replacement = "";
        FindReplaceFramework.MatchedTextCollection col;
        RegexOptions regexOptions = new RegexOptions();

        public FormFind()
        {
            InitializeComponent();
        }

        private void FormFind_Load(object sender, EventArgs e)
        {
            rtbSearchPattern.Select();
            rtbSearchPattern.Select(0, 0);
            rtbSearchPattern.ScrollToCaret();

            //rtbSearchPattern.KeyDown += RtbSearchPattern_KeyDown;
            //rtbSearchReplace.KeyDown += RtbSearchReplace_KeyDown;

            //            rtbSearchReplace.AppendText(Environment.NewLine);
            //rtbSearchPattern.AppendText(Environment.NewLine);

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            FrmJournal parentForm = (FrmJournal)myParentForm;
            parentForm.myFormFind = null;
            this.Close();
            this.Dispose();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            initSearch(true, false, false);
        }

        public void prepare()
        {
            searchPattern = rtbSearchPattern.Text;
            replacement = rtbSearchReplace.Text;

            // prepare regex
            regexOptions = new();
            if (!chkSearchMatchCase.Checked)
                regexOptions |= RegexOptions.IgnoreCase;

            if (chkSearchMultiline.Checked)
                regexOptions |= RegexOptions.Multiline;
            else
                regexOptions |= RegexOptions.Singleline;

            if (chkSearchMatchWholeWord.Checked)
            {
                String flattened = Regex.Escape(searchPattern);
                searchPattern = @"\b" + flattened + @"";//[^.*\r*\n*]";
            }

            // successfully prepared all config
            // verify if the pattern is valid.
            if (!commonMethods.IsValidRegex(searchPattern))
                return;

            // initialize search and generate collection
            col = FindReplaceFramework.MatchedTextCollection.initializeSearch(ref regexOptions, rtb.Document, searchPattern);
            txtBoxTotal.Text = col.Count.ToString();
        }

        public void initSearch(bool doFind, bool doReplace, bool doReplaceAll)
        {
            if (rtbSearchPattern.TextLength <= 0)
                return;

            if (doFind)
            {
                // configure, initialize search and generate collection
                prepare();
                findNext();
            }
            else if (doReplace)
                replace();
            else if (doReplaceAll)
                replaceAll();

        }

        public void resetTo0()
        {
            col.resetTo0();
            rtb.Focus();
            rtb.Selection.Select(rtb.Document.ContentStart, rtb.Document.ContentStart);
            txtBoxCurrent.Text = "-1";

        }
        public void resetToLast()
        {
            col.resetToLast();
            rtb.Focus();
            if (col.current == null)
            {
                resetTo0();
                return;
            }
            rtb.Selection.Select(col.current.start, col.current.end);
            txtBoxCurrent.Text = col.current.index.ToString();

            Block selPreviousBlock = col.current.start.Paragraph.NextBlock;//GetNextInsertionPosition(LogicalDirection.Forward);

            if (selPreviousBlock != null)
            {
                var characterRect = selPreviousBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                //RichTextBox.ScrollToHorizontalOffset(RichTextBox.HorizontalOffset + characterRect.Left - RichTextBox.ActualWidth / 2d);
                rtb.ScrollToVerticalOffset(rtb.VerticalOffset + characterRect.Top - rtb.ActualHeight / 2d);
            }

        }

        public bool findNext()
        {
            if (col == null) return false;
            col.Next(true);
            if (col.current == null)
            {
                // end of file but no match found in this area, so reset to 0 index
                resetTo0();
                return false;
            }

            rtb.Focus();

            // advance the character selection position for next find/replace
            rtb.Selection.Select(col.current.start, col.current.end);
            txtBoxCurrent.Text = col.current.index.ToString();

            Block selNextBlock = col.current.start.Paragraph.NextBlock;//GetNextInsertionPosition(LogicalDirection.Forward);

            if (selNextBlock != null)
            {
                var characterRect = selNextBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                //RichTextBox.ScrollToHorizontalOffset(RichTextBox.HorizontalOffset + characterRect.Left - RichTextBox.ActualWidth / 2d);
                rtb.ScrollToVerticalOffset(rtb.VerticalOffset + characterRect.Top - rtb.ActualHeight / 2d);
            }

            return true;
        }

        public bool findPrevious()
        {
            if (col == null) return false;

            if (col.current == null)
            {
                // there is no item at this location, so reset to 0 index
                resetToLast();
                return true;
            }

            col.Previous(true);
            if (col.current == null)
            {
                // there is no item at this location, so reset to 0 index
                resetTo0();
                return false;
            }

            rtb.Focus();

            // advance the character selection position for next find/replace
            rtb.Selection.Select(col.current.start, col.current.end);
            txtBoxCurrent.Text = col.current.index.ToString();

            Block selPreviousBlock = col.current.start.Paragraph.NextBlock;//GetNextInsertionPosition(LogicalDirection.Forward);

            if (selPreviousBlock != null)
            {
                var characterRect = selPreviousBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                //RichTextBox.ScrollToHorizontalOffset(RichTextBox.HorizontalOffset + characterRect.Left - RichTextBox.ActualWidth / 2d);
                rtb.ScrollToVerticalOffset(rtb.VerticalOffset + characterRect.Top - rtb.ActualHeight / 2d);
            }

            return true;
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            if (col == null) 
                initSearch(true, false, false);
            else
               findNext();
        }

        public void replace()
        {
            if (col == null) return;
            if (col.current == null) return;

            // assuming find was done successfully and the selection was set, so we proceed replacing that selection.
            TextRange selection = new TextRange(col.current.start, col.current.end);
            selection.Text = replacement;
            col.Remove(ref col.current);
        }

        public void replaceAll()
        {
            if (col == null) return;

            // first reset to 0 index
            resetTo0();

            // now indefinately replace all matching text found till false is returned.

            bool found = true;
            while (found)
            {
                found = findNext();
                if (found)
                    replace();
            }
        }
        private void buttonReplace_Click(object sender, EventArgs e)
        {

            initSearch(false, true, false);
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {

            initSearch(false, false, true);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.SelectAll();
        }

        private void rtbSearchPattern_TextChanged(object sender, EventArgs e)
        {

        }

        private void insertNewLineAtCaretToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.SelectedText = Environment.NewLine;
        }

        private void buttonFindPrevious_Click(object sender, EventArgs e)
        {
            if (col == null)
                initSearch(true, false, false);
            else
                findPrevious();

        }

        public bool gotoIndex(int index)
        {
            if (col == null) return false;
            if (index >= col.Count) return false;

            col.Goto(index, true);
            if (col.current == null)
            {
                // end of file but no match found in this area, so reset to 0 index
                resetTo0();
                return false;
            }

            rtb.Focus();

            // advance the character selection position for next find/replace
            rtb.Selection.Select(col.current.start, col.current.end);
            txtBoxCurrent.Text = col.current.index.ToString();

            Block selNextBlock = col.current.start.Paragraph.NextBlock;//GetNextInsertionPosition(LogicalDirection.Forward);

            if (selNextBlock != null)
            {
                var characterRect = selNextBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                //RichTextBox.ScrollToHorizontalOffset(RichTextBox.HorizontalOffset + characterRect.Left - RichTextBox.ActualWidth / 2d);
                rtb.ScrollToVerticalOffset(rtb.VerticalOffset + characterRect.Top - rtb.ActualHeight / 2d);
            }

            return true;

        }

        private void buttonGotoIndex_Click(object sender, EventArgs e)
        {
            int value = -1;
            if (!int.TryParse(txtBoxIndex.Text, out value))
                return;

            gotoIndex(value);
        }
    }
}
