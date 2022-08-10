﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public partial class FormFind : Form
    {
        public AdvRichTextBox rtb;
        public Form myParentForm;
        String searchPattern = "";
        String replacement = "";
        Match? currentMatch = null;
        Regex regex;
        int foundIndex = -1;
        int foundLength = -1;

        public FormFind()
        {
            InitializeComponent();
        }

        private void FormFind_Load(object sender, EventArgs e)
        {
            rtbSearchPattern.Select();
            rtbSearchPattern.Select(0, 0);
            rtbSearchPattern.ScrollToCaret();

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

        public Regex prepareRegex()
        {
            searchPattern = rtbSearchPattern.Text;
            replacement = rtbSearchReplace.Text;

            // prepare regex
            RegexOptions regexOptions = new RegexOptions();
            if (!chkSearchMatchCase.Checked)
                regexOptions |= RegexOptions.IgnoreCase;

            if (chkSearchMultiline.Checked)
                regexOptions |= RegexOptions.Multiline;
            else
                regexOptions |= RegexOptions.Singleline;

            // prepare to match whole word if user requires. but we can use it through manual pattern as well.
            if (chkSearchMatchWholeWord.Checked)
                searchPattern = String.Format(@"\b{0}\b", searchPattern);

            // successfully prepared all config
            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);
            return regex;
        }

        public void initSearch(bool doFind, bool doReplace, bool doReplaceAll)
        {
            regex = prepareRegex();

            if (doFind)
            {
                updateSelectionPosition();
                findNext();
            }
            else if (doReplace)
                replace();
            else if (doReplaceAll)
                replaceAll();

        }

        public void resetRtbSearchingConfig()
        {
            AdvRichTextBox.SetSelectionEx(rtb, 0, 0);
            rtb.ScrollToCaret();
            rtb.Focus();
            foundIndex = foundLength = -1;
            currentMatch = null;
        }

        public void updateSelectionPosition()
        {
            // invalid if no match has yet been found. so we abort his method.
            if (foundIndex < 0)
                return;

            // invalid if match has 0 length, so abort.
            if (foundLength <= 0)
                return;

            // advance the character selection position for next find/replace
            AdvRichTextBox.SetSelectionEx(rtb, rtb.SelectionStart + rtb.SelectionLength, 0);
        }

        public bool findNext()
        {
            currentMatch = regex.Match(rtb.Text, rtb.SelectionStart);
            if (!currentMatch.Success)
            {
                // end of file but no match found in this area, so reset to 0 index
                resetRtbSearchingConfig();
                return false;
            }
            // setup
            AdvRichTextBox.SetSelectionEx(rtb, currentMatch.Index, currentMatch.Length);//foundIndex, currentMatch.Length);
            foundIndex = currentMatch.Index;
            foundLength = currentMatch.Length;
            rtb.ScrollToCaret();
            rtb.Focus();
            return true;
        }

        public void replace()
        {
            // assuming find was done successfully and the selection was set, so we proceed replacing that selection.
            AdvRichTextBox.ReplaceSelectedUnicodeText(rtb, replacement);
        }

        public void replaceAll()
        {
            // first reset to 0 index
            resetRtbSearchingConfig();

            // now indefinately replace all matching text found till false is returned.

            bool found = true;
            while (found)
            {
                updateSelectionPosition();
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
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.SelectAll();
        }

        private void rtbSearchPattern_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
