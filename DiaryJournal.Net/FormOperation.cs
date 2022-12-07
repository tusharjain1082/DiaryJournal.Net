using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DiaryJournal.Net
{
    public partial class FormOperation : Form
    {
        public String desc = "";
        public long progress = 0;
        public long total = 0;
        public long totalFiles = 0;
        public long filesDone = 0;
        public bool cancelEnabled = false;
        public bool shown = false;
        public FrmJournal? parent = null;

        // delegates
        public delegate void __updateProgressBarDelegate(long progess, long total);
        public __updateProgressBarDelegate updateProgressBarSafe;
        public delegate void __updateFilesStatusDelegate(long progess, long total);
        public __updateFilesStatusDelegate updateFilesStatusSafe;
        public delegate void __updateDescriptionDelegate(String value);
        public __updateDescriptionDelegate updateDescriptionSafe;
        public delegate void __closeDelegate();
        public __closeDelegate closeSafe;

        public static FormOperation? showForm(FrmJournal? parent, String desc, long progress, long total, long filesDone, long totalFiles)
        {
            Thread thread = new Thread(FormOperationThreadMethod);
            FormOperation form = new FormOperation();
            form.parent = parent;
            form.desc = desc;
            form.progress = progress;
            form.total = total;
            form.filesDone = filesDone;
            form.totalFiles = totalFiles;
            thread.Start(form);

            // unless form and all it's controls are completely loaded and working,
            // we cannot progress with any operation because it results error and termination.
            while(true)
            {
                if (form.shown)
                    break; // everything ready, so exit loop

                // not ready
                Thread.Sleep(100);
            }

            // everything ready
            return form;
        }

        public static void FormOperationThreadMethod(object argument)
        {
            FormOperation? form = (FormOperation?)argument;
            form.ShowDialog();
        }

        public void __close()
        {
            this.Close();
            this.Dispose();
        }
        public void close()
        {
            this.Invoke(closeSafe);
        }
        public void __updateDescription(String value)
        {
            desc = value;
            labelDescription.Text = value;  
        }
        public void updateDescription(String value)
        {
            this.Invoke(updateDescriptionSafe, value);
        }

        public void __updateFilesStatus(long progress, long total)
        {
            filesDone = progress;
            totalFiles = total;
            txtFilesDone.Text = progress.ToString();
            txtTotalFiles.Text = total.ToString();
        }
        public void updateFilesStatus(long progress, long total)
        {
            this.Invoke(updateFilesStatusSafe, progress, total);
        }

        public void __updateProgressBar(long progress, long total)
        {
            this.progress = progress;
            this.total = total;
            progressBar.Value = (int)Math.Round((double)(100 * progress) / total);
        }

        public void updateProgressBar(long progress, long total)
        {
            this.Invoke(updateProgressBarSafe, progress, total);
        }

        public FormOperation()
        {
            // setup delegates
            updateProgressBarSafe = new __updateProgressBarDelegate(__updateProgressBar);
            updateFilesStatusSafe = new __updateFilesStatusDelegate(__updateFilesStatus);
            updateDescriptionSafe = new __updateDescriptionDelegate(__updateDescription);
            closeSafe = new __closeDelegate(__close);

            InitializeComponent();
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {

            // configure
            buttonCancel.Enabled = cancelEnabled;

            // initialize initial progress
            this.Invoke(updateProgressBar, progress, total);
            this.Invoke(updateFilesStatus, filesDone, totalFiles);
            this.Invoke(updateDescription, desc);
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            shown = true;
        }

        private void FormOperation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (parent!= null) 
                parent.Invoke(parent.showForm);
        }


    }
}
