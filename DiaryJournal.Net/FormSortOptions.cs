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
    public partial class FormSortOptions : Form
    {

        public bool sortById = true;
        public bool sortByIdExtra = false;
        public bool sortByCommonDateTime = true;
        public bool sortByCreationDateTime = false;
        public bool sortByModificationDateTime = false;
        public bool descending = false;

        public FormSortOptions()
        {
            InitializeComponent();
        }

        private void FormSortOptions_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sortById = ((radioSortById.Checked) ? true : false);
            sortByIdExtra = radioSortByIdExtra.Checked;
            sortByCommonDateTime = radioCommonDateTime.Checked;
            sortByCreationDateTime = radioCreationDateTime.Checked;
            sortByModificationDateTime = radioModificationDateTime.Checked; 
            descending = radioSortDescending.Checked;   
        }
    }
}
