using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiaryJournal.Net
{
    public partial class FormTreeResetOptions : Form
    {

        public bool resetFont = false;
        public bool resetFontSize = false;
        public bool resetItalics = false;
        public bool resetBold = false;
        public bool resetStrikeout = false;
        public bool resetUnderline = false;
        public bool resetBackColor = false;
        public bool resetForeColor = false;


        public FormTreeResetOptions()
        {
            InitializeComponent();
        }

        private void FormTreeResetOptions_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            resetFont = chkFont.Checked;
            resetFontSize = chkFontSize.Checked;
            resetItalics = chkItalics.Checked;
            resetBold = chkBold.Checked;
            resetStrikeout = chkStrikeout.Checked;
            resetUnderline = chkUnderline.Checked;
            resetBackColor = chkBackColor.Checked;
            resetForeColor = chkForeColor.Checked;
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            chkFont.Checked = chkFontSize.Checked = chkItalics.Checked = chkBold.Checked = chkStrikeout.Checked = chkUnderline.Checked = 
                chkBackColor.Checked = chkForeColor.Checked = true;
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            chkFont.Checked = chkFontSize.Checked = chkItalics.Checked = chkBold.Checked = chkStrikeout.Checked = chkUnderline.Checked =
                chkBackColor.Checked = chkForeColor.Checked = false;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
