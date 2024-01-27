namespace DiaryJournal.Net
{
    partial class FormList
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
            splitContainer1 = new SplitContainer();
            lvList = new ListView();
            CHCode = new ColumnHeader();
            CHTitle = new ColumnHeader();
            buttonNone = new Button();
            buttonAll = new Button();
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
            buttonOK.Click += buttonOK_Click;
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
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvList);
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(buttonNone);
            splitContainer1.Panel2.Controls.Add(buttonAll);
            splitContainer1.Panel2.Controls.Add(buttonOK);
            splitContainer1.Panel2.Controls.Add(buttonCancel);
            splitContainer1.Panel2MinSize = 60;
            splitContainer1.Size = new Size(784, 561);
            splitContainer1.SplitterDistance = 497;
            splitContainer1.TabIndex = 4;
            // 
            // lvList
            // 
            lvList.CheckBoxes = true;
            lvList.Columns.AddRange(new ColumnHeader[] { CHCode, CHTitle });
            lvList.Dock = DockStyle.Fill;
            lvList.Font = new Font("Segoe UI", 8.25F);
            lvList.FullRowSelect = true;
            lvList.GridLines = true;
            lvList.Location = new Point(0, 0);
            lvList.Name = "lvList";
            lvList.ShowItemToolTips = true;
            lvList.Size = new Size(784, 497);
            lvList.TabIndex = 2;
            lvList.UseCompatibleStateImageBehavior = false;
            lvList.View = View.Details;
            // 
            // CHCode
            // 
            CHCode.Text = "code/id/name/title";
            CHCode.Width = 400;
            // 
            // CHTitle
            // 
            CHTitle.Text = "title/detail/info/summary";
            CHTitle.Width = 1000;
            // 
            // buttonNone
            // 
            buttonNone.Location = new Point(168, 12);
            buttonNone.Name = "buttonNone";
            buttonNone.Size = new Size(150, 30);
            buttonNone.TabIndex = 4;
            buttonNone.Text = "none";
            buttonNone.UseVisualStyleBackColor = true;
            buttonNone.Click += buttonNone_Click;
            // 
            // buttonAll
            // 
            buttonAll.Location = new Point(12, 12);
            buttonAll.Name = "buttonAll";
            buttonAll.Size = new Size(150, 30);
            buttonAll.TabIndex = 3;
            buttonAll.Text = "all";
            buttonAll.UseVisualStyleBackColor = true;
            buttonAll.Click += buttonAll_Click;
            // 
            // FormList
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(784, 561);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 11.25F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormList";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "list";
            Load += FormList_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonOK;
        private Button buttonCancel;
        private SplitContainer splitContainer1;
        private ListView lvList;
        private ColumnHeader CHCode;
        private ColumnHeader CHTitle;
        private Button buttonAll;
        private Button buttonNone;
    }
}