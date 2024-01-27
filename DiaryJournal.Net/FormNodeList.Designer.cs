namespace DiaryJournal.Net
{
    partial class FormNodeList
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
            lvNodeList = new ListView();
            CHSearchFullPath = new ColumnHeader();
            CHSearchDateTime = new ColumnHeader();
            CHSearchCDT = new ColumnHeader();
            CHSearchMDT = new ColumnHeader();
            CHSearchDDT = new ColumnHeader();
            CHSearchType = new ColumnHeader();
            CHSearchSpecialNodeType = new ColumnHeader();
            CHSearchNodeType = new ColumnHeader();
            CHSearchPID = new ColumnHeader();
            CHSearchID = new ColumnHeader();
            CHSearchTitle = new ColumnHeader();
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
            splitContainer1.Panel1.Controls.Add(lvNodeList);
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
            // lvNodeList
            // 
            lvNodeList.CheckBoxes = true;
            lvNodeList.Columns.AddRange(new ColumnHeader[] { CHSearchFullPath, CHSearchDateTime, CHSearchCDT, CHSearchMDT, CHSearchDDT, CHSearchType, CHSearchSpecialNodeType, CHSearchNodeType, CHSearchPID, CHSearchID, CHSearchTitle });
            lvNodeList.Dock = DockStyle.Fill;
            lvNodeList.Font = new Font("Segoe UI", 8.25F);
            lvNodeList.FullRowSelect = true;
            lvNodeList.GridLines = true;
            lvNodeList.Location = new Point(0, 0);
            lvNodeList.Name = "lvNodeList";
            lvNodeList.ShowItemToolTips = true;
            lvNodeList.Size = new Size(784, 497);
            lvNodeList.TabIndex = 2;
            lvNodeList.UseCompatibleStateImageBehavior = false;
            lvNodeList.View = View.Details;
            // 
            // CHSearchFullPath
            // 
            CHSearchFullPath.Text = "entry full path";
            CHSearchFullPath.Width = 1000;
            // 
            // CHSearchDateTime
            // 
            CHSearchDateTime.Text = "entry date time";
            CHSearchDateTime.Width = 250;
            // 
            // CHSearchCDT
            // 
            CHSearchCDT.Text = "true creation date time";
            CHSearchCDT.Width = 250;
            // 
            // CHSearchMDT
            // 
            CHSearchMDT.Text = "last modified date time";
            CHSearchMDT.Width = 250;
            // 
            // CHSearchDDT
            // 
            CHSearchDDT.Text = "deletion date time";
            CHSearchDDT.Width = 250;
            // 
            // CHSearchType
            // 
            CHSearchType.Text = "entry type";
            CHSearchType.Width = 100;
            // 
            // CHSearchSpecialNodeType
            // 
            CHSearchSpecialNodeType.Text = "sp. node type";
            CHSearchSpecialNodeType.Width = 100;
            // 
            // CHSearchNodeType
            // 
            CHSearchNodeType.Text = "node type";
            CHSearchNodeType.Width = 100;
            // 
            // CHSearchPID
            // 
            CHSearchPID.Text = "parent id";
            CHSearchPID.Width = 100;
            // 
            // CHSearchID
            // 
            CHSearchID.Text = "id";
            CHSearchID.Width = 100;
            // 
            // CHSearchTitle
            // 
            CHSearchTitle.Text = "title";
            CHSearchTitle.Width = 1000;
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
            // FormNodeList
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(784, 561);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 11.25F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormNodeList";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "nodes list";
            Load += FormNodeList_Load;
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
        private ListView lvNodeList;
        private ColumnHeader CHSearchFullPath;
        private ColumnHeader CHSearchDateTime;
        private ColumnHeader CHSearchCDT;
        private ColumnHeader CHSearchMDT;
        private ColumnHeader CHSearchDDT;
        private ColumnHeader CHSearchType;
        private ColumnHeader CHSearchSpecialNodeType;
        private ColumnHeader CHSearchNodeType;
        private ColumnHeader CHSearchPID;
        private ColumnHeader CHSearchID;
        private ColumnHeader CHSearchTitle;
        private Button buttonAll;
        private Button buttonNone;
    }
}