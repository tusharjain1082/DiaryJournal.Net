using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DiaryJournal.Net
{
    public partial class CustomFontDialog : Form
    {
        public Font font = null;

        public double size = 0;
        public bool bold = false;
        public bool italic = false;
        public bool underline = false;
        public bool strikeout = false;
        public Color FontColor = Color.Black;
        public Color FontBackColor = Color.White;
        public textFormatting formatting = null;

        public CustomFontDialog()
        {
            InitializeComponent();

            for (int size = 6; size <= 300; size++)
                lstSize.Items.Add(size);

            txtSize.Text = Convert.ToString(10);
        }
        private void lstFont_SelectedFontFamilyChanged(object sender, EventArgs e)
        {
            UpdateSampleText();
        }

        private void lstSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSize.SelectedItem != null)
                txtSize.Text = lstSize.SelectedItem.ToString();
        }

        private void txtSize_TextChanged(object sender, EventArgs e)
        {
            UpdateSampleText();
        }

        private void txtSize_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void UpdateSampleText()
        {
            double size = txtSize.Text != "" ? double.Parse(txtSize.Text) : 1;

            FontStyle style = chbBold.Checked ? FontStyle.Bold : FontStyle.Regular;
            if (chbItalic.Checked) style |= FontStyle.Italic;
            if (chbStrikeout.Checked) style |= FontStyle.Strikeout;
            if (chbUnderline.Checked) style |= FontStyle.Underline;

            if (lvFonts.SelectedItems.Count <= 0)
                return;

            ListViewItem item = lvFonts.SelectedItems[0];
            Font newFont = new Font(item.Name, (float)size, style);
            lblSampleText.Font = newFont;

            // update colors
            lblSampleText.ForeColor = FontColor;
            lblSampleText.BackColor = FontBackColor;
        }

        /// <summary>
        /// Handles CheckedChanged event for Bold, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chb_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSampleText();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            size = txtSize.Text != "" ? double.Parse(txtSize.Text) : 1;
            bold = chbBold.Checked;
            italic = chbItalic.Checked;
            underline = chbUnderline.Checked;
            strikeout = chbStrikeout.Checked;
            font = lblSampleText.Font;
        }

        private void CustomFontDialog_Load(object sender, EventArgs e)
        {
            txtSize.Text = size.ToString();
            lstSize.SelectedIndex = lstSize.FindString(size.ToString());
            lstSize.TopIndex = lstSize.SelectedIndex;
            chbBold.Checked = bold;
            chbItalic.Checked = italic;
            chbStrikeout.Checked = strikeout;
            chbUnderline.Checked = underline;

            var webColors = commonMethods.getWebColors();// typeof(Color));
            foreach (Color knownColor in webColors)
            {
                ListViewItem item1 = new ListViewItem();
                item1.Text = knownColor.Name;
                item1.Name = knownColor.Name;
                item1.Tag = knownColor;
                item1.BackColor = knownColor;
                lvFontColor.Items.Add(item1);
                ListViewItem item2 = new ListViewItem();
                item2.Text = knownColor.Name;
                item2.Name = knownColor.Name;
                item2.Tag = knownColor;
                item2.BackColor = knownColor;
                lvFontBackColor.Items.Add(item2);
            }

            // set font colors
            Color fontColor = Color.Black;
            Color fontBackColor = Color.White;
            foreach (Color knownColor in webColors)
            {
                int knownColorArgb = knownColor.ToArgb();

                int fontColorArgb = FontColor.ToArgb();
                if (knownColorArgb == fontColorArgb)
                    fontColor = knownColor;

                int fontBackColorArgb = FontBackColor.ToArgb();
                if (fontBackColorArgb == knownColorArgb)
                    fontBackColor = knownColor;
            }
            ListViewItem[] items = lvFontColor.Items.Find(fontColor.Name, false);
            ListViewItem defaultItem1 = items[0];
            items = lvFontBackColor.Items.Find(fontBackColor.Name, false);
            ListViewItem defaultItem2 = items[0];
            defaultItem1.Selected = true;
            defaultItem2.Selected = true;
            defaultItem1.EnsureVisible();
            defaultItem2.EnsureVisible();

            // initialize all formatting config
            formatting = new textFormatting();
            foreach (String fontName in formatting.fontNames)
            {
                Font font = new Font(fontName, 12, FontStyle.Regular);
                ListViewItem item1 = new ListViewItem();
                item1.Text = fontName;
                item1.Name = fontName;
                item1.Font = font;
                lvFonts.Items.Add(item1);
            }

            if (this.font == null)
                this.font = new Font("Arial", 14, FontStyle.Regular);

            ListViewItem[] matchingFonts = lvFonts.Items.Find(font.Name, false);
            if (matchingFonts.Length > 0)
            {
                matchingFonts[0].Selected = true;
                matchingFonts[0].EnsureVisible();
                lvFonts.Focus();
                lvFonts.Select();
            }
        }

        private void lvFontColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = null;
            if (lvFontColor.SelectedItems.Count > 0)
                item = lvFontColor.SelectedItems[0];

            if (item == null)
                return;

            FontColor = (System.Drawing.Color) item.Tag;

            UpdateSampleText();

        }

        private void lvFontBackColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = null;
            if (lvFontBackColor.SelectedItems.Count > 0)
                item = lvFontBackColor.SelectedItems[0];

            if (item == null)
                return;

            FontBackColor = (System.Drawing.Color)item.Tag;

            UpdateSampleText();

        }

        private void chbUnderline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSampleText();
        }

        private void lvFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = null;
            if (lvFonts.SelectedItems.Count > 0)
                item = lvFonts.SelectedItems[0];

            if (item == null)
                return;

            txtFont.Text = item.Text;

            UpdateSampleText();
        }
    }
}
