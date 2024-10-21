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
    public partial class FormSelectDB : Form
    {

        public bool SingleFileDBLiteDB = false;
        public bool OpenFileSystemDB = false;
        public DatabaseType selectedDBType = DatabaseType.SingleFileDB;


        public FormSelectDB()
        {
            InitializeComponent();
        }

        private void FormSelectDB_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SingleFileDBLiteDB = radSFDBLiteDB.Checked;
            OpenFileSystemDB = radOFSDB.Checked;

            if (SingleFileDBLiteDB) selectedDBType = DatabaseType.SingleFileDB;
            else if (OpenFileSystemDB) selectedDBType = DatabaseType.OpenFSDB;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
