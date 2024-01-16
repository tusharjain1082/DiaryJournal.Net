namespace DiaryJournal.Net
{
    partial class FormTree
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonOK = new Button();
            buttonCancel = new Button();
            tvEntries = new TreeView();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(622, 12);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(150, 30);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "&OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(466, 12);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(150, 30);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tvEntries
            // 
            tvEntries.AllowDrop = true;
            tvEntries.CheckBoxes = true;
            tvEntries.Dock = DockStyle.Fill;
            tvEntries.Font = new Font("Segoe UI", 8.25F);
            tvEntries.FullRowSelect = true;
            tvEntries.HideSelection = false;
            tvEntries.ItemHeight = 16;
            tvEntries.Location = new Point(0, 0);
            tvEntries.Name = "tvEntries";
            tvEntries.ShowNodeToolTips = true;
            tvEntries.Size = new Size(784, 497);
            tvEntries.TabIndex = 3;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvEntries);
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(buttonOK);
            splitContainer1.Panel2.Controls.Add(buttonCancel);
            splitContainer1.Panel2MinSize = 60;
            splitContainer1.Size = new Size(784, 561);
            splitContainer1.SplitterDistance = 497;
            splitContainer1.TabIndex = 4;
            // 
            // FormTree
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(784, 561);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 11.25F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormTree";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "tree";
            Load += FormTree_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonOK;
        private Button buttonCancel;
        public TreeView tvEntries;
        private SplitContainer splitContainer1;
    }
}