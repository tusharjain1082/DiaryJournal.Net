namespace DiaryJournal.Net
{
    partial class FormFind
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
            this.components = new System.ComponentModel.Container();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.rtbSearchPattern = new System.Windows.Forms.RichTextBox();
            this.menuRtbSearch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator39 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.insertNewLineAtCaretToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkSearchMultiline = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtbSearchReplace = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSearchMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.chkSearchMatchCase = new System.Windows.Forms.CheckBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxCurrent = new System.Windows.Forms.TextBox();
            this.txtBoxTotal = new System.Windows.Forms.TextBox();
            this.buttonFindPrevious = new System.Windows.Forms.Button();
            this.txtBoxIndex = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonGotoIndex = new System.Windows.Forms.Button();
            this.groupBox7.SuspendLayout();
            this.menuRtbSearch.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.rtbSearchPattern);
            this.groupBox7.Location = new System.Drawing.Point(12, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(500, 110);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "regex (unicode) pattern/text (note: only regex shall be used.)";
            // 
            // rtbSearchPattern
            // 
            this.rtbSearchPattern.ContextMenuStrip = this.menuRtbSearch;
            this.rtbSearchPattern.Location = new System.Drawing.Point(3, 23);
            this.rtbSearchPattern.Name = "rtbSearchPattern";
            this.rtbSearchPattern.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtbSearchPattern.Size = new System.Drawing.Size(491, 81);
            this.rtbSearchPattern.TabIndex = 0;
            this.rtbSearchPattern.Text = "";
            this.rtbSearchPattern.WordWrap = false;
            // 
            // menuRtbSearch
            // 
            this.menuRtbSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator39,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.insertNewLineAtCaretToolStripMenuItem});
            this.menuRtbSearch.Name = "menuRtbSearch";
            this.menuRtbSearch.Size = new System.Drawing.Size(193, 126);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.cutToolStripMenuItem.Text = "cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.copyToolStripMenuItem.Text = "copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.pasteToolStripMenuItem.Text = "paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator39
            // 
            this.toolStripSeparator39.Name = "toolStripSeparator39";
            this.toolStripSeparator39.Size = new System.Drawing.Size(189, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.selectAllToolStripMenuItem.Text = "select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // insertNewLineAtCaretToolStripMenuItem
            // 
            this.insertNewLineAtCaretToolStripMenuItem.Name = "insertNewLineAtCaretToolStripMenuItem";
            this.insertNewLineAtCaretToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.insertNewLineAtCaretToolStripMenuItem.Text = "insert new line at caret";
            this.insertNewLineAtCaretToolStripMenuItem.Click += new System.EventHandler(this.insertNewLineAtCaretToolStripMenuItem_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkSearchMultiline);
            this.groupBox6.Location = new System.Drawing.Point(518, 108);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(188, 90);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "regex";
            // 
            // chkSearchMultiline
            // 
            this.chkSearchMultiline.AutoSize = true;
            this.chkSearchMultiline.Location = new System.Drawing.Point(6, 20);
            this.chkSearchMultiline.Name = "chkSearchMultiline";
            this.chkSearchMultiline.Size = new System.Drawing.Size(92, 24);
            this.chkSearchMultiline.TabIndex = 4;
            this.chkSearchMultiline.Text = "multi-line";
            this.chkSearchMultiline.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rtbSearchReplace);
            this.groupBox3.Location = new System.Drawing.Point(12, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(500, 110);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "replace with (unicode)";
            // 
            // rtbSearchReplace
            // 
            this.rtbSearchReplace.ContextMenuStrip = this.menuRtbSearch;
            this.rtbSearchReplace.Location = new System.Drawing.Point(3, 23);
            this.rtbSearchReplace.Name = "rtbSearchReplace";
            this.rtbSearchReplace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtbSearchReplace.Size = new System.Drawing.Size(491, 81);
            this.rtbSearchReplace.TabIndex = 1;
            this.rtbSearchReplace.Text = "";
            this.rtbSearchReplace.WordWrap = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkSearchMatchWholeWord);
            this.groupBox2.Controls.Add(this.chkSearchMatchCase);
            this.groupBox2.Location = new System.Drawing.Point(518, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 90);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "regex";
            // 
            // chkSearchMatchWholeWord
            // 
            this.chkSearchMatchWholeWord.AutoSize = true;
            this.chkSearchMatchWholeWord.Location = new System.Drawing.Point(6, 40);
            this.chkSearchMatchWholeWord.Name = "chkSearchMatchWholeWord";
            this.chkSearchMatchWholeWord.Size = new System.Drawing.Size(166, 24);
            this.chkSearchMatchWholeWord.TabIndex = 3;
            this.chkSearchMatchWholeWord.Text = "match (all) section(s)";
            this.chkSearchMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // chkSearchMatchCase
            // 
            this.chkSearchMatchCase.AutoSize = true;
            this.chkSearchMatchCase.Location = new System.Drawing.Point(6, 20);
            this.chkSearchMatchCase.Name = "chkSearchMatchCase";
            this.chkSearchMatchCase.Size = new System.Drawing.Size(102, 24);
            this.chkSearchMatchCase.TabIndex = 2;
            this.chkSearchMatchCase.Text = "match case";
            this.chkSearchMatchCase.UseVisualStyleBackColor = true;
            // 
            // buttonFind
            // 
            this.buttonFind.BackColor = System.Drawing.Color.LightGreen;
            this.buttonFind.Location = new System.Drawing.Point(15, 235);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(100, 30);
            this.buttonFind.TabIndex = 5;
            this.buttonFind.Text = "find/reset";
            this.buttonFind.UseVisualStyleBackColor = false;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.BackColor = System.Drawing.Color.LightCoral;
            this.buttonReplace.Location = new System.Drawing.Point(390, 235);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(100, 30);
            this.buttonReplace.TabIndex = 6;
            this.buttonReplace.Text = "replace";
            this.buttonReplace.UseVisualStyleBackColor = false;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.BackColor = System.Drawing.Color.LightCoral;
            this.buttonReplaceAll.Location = new System.Drawing.Point(496, 235);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.Size = new System.Drawing.Size(100, 30);
            this.buttonReplaceAll.TabIndex = 7;
            this.buttonReplaceAll.Text = "replace all";
            this.buttonReplaceAll.UseVisualStyleBackColor = false;
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(602, 235);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 30);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.BackColor = System.Drawing.Color.LightGreen;
            this.buttonFindNext.Location = new System.Drawing.Point(121, 235);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(100, 30);
            this.buttonFindNext.TabIndex = 9;
            this.buttonFindNext.Text = "next";
            this.buttonFindNext.UseVisualStyleBackColor = false;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 306);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "current:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(370, 306);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "total:";
            // 
            // txtBoxCurrent
            // 
            this.txtBoxCurrent.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBoxCurrent.Location = new System.Drawing.Point(114, 307);
            this.txtBoxCurrent.Name = "txtBoxCurrent";
            this.txtBoxCurrent.ReadOnly = true;
            this.txtBoxCurrent.Size = new System.Drawing.Size(250, 33);
            this.txtBoxCurrent.TabIndex = 12;
            // 
            // txtBoxTotal
            // 
            this.txtBoxTotal.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBoxTotal.Location = new System.Drawing.Point(419, 307);
            this.txtBoxTotal.Name = "txtBoxTotal";
            this.txtBoxTotal.ReadOnly = true;
            this.txtBoxTotal.Size = new System.Drawing.Size(250, 33);
            this.txtBoxTotal.TabIndex = 13;
            // 
            // buttonFindPrevious
            // 
            this.buttonFindPrevious.BackColor = System.Drawing.Color.LightGreen;
            this.buttonFindPrevious.Location = new System.Drawing.Point(227, 235);
            this.buttonFindPrevious.Name = "buttonFindPrevious";
            this.buttonFindPrevious.Size = new System.Drawing.Size(100, 30);
            this.buttonFindPrevious.TabIndex = 14;
            this.buttonFindPrevious.Text = "previous";
            this.buttonFindPrevious.UseVisualStyleBackColor = false;
            this.buttonFindPrevious.Click += new System.EventHandler(this.buttonFindPrevious_Click);
            // 
            // txtBoxIndex
            // 
            this.txtBoxIndex.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBoxIndex.Location = new System.Drawing.Point(114, 271);
            this.txtBoxIndex.Name = "txtBoxIndex";
            this.txtBoxIndex.Size = new System.Drawing.Size(250, 33);
            this.txtBoxIndex.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 271);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "goto index:";
            // 
            // buttonGotoIndex
            // 
            this.buttonGotoIndex.BackColor = System.Drawing.Color.LightGreen;
            this.buttonGotoIndex.Location = new System.Drawing.Point(370, 273);
            this.buttonGotoIndex.Name = "buttonGotoIndex";
            this.buttonGotoIndex.Size = new System.Drawing.Size(100, 30);
            this.buttonGotoIndex.TabIndex = 17;
            this.buttonGotoIndex.Text = "goto index";
            this.buttonGotoIndex.UseVisualStyleBackColor = false;
            this.buttonGotoIndex.Click += new System.EventHandler(this.buttonGotoIndex_Click);
            // 
            // FormFind
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(714, 361);
            this.ControlBox = false;
            this.Controls.Add(this.buttonGotoIndex);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBoxIndex);
            this.Controls.Add(this.buttonFindPrevious);
            this.Controls.Add(this.txtBoxTotal);
            this.Controls.Add(this.txtBoxCurrent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonFindNext);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonReplaceAll);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFind";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "find and replace";
            this.Load += new System.EventHandler(this.FormFind_Load);
            this.groupBox7.ResumeLayout(false);
            this.menuRtbSearch.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox7;
        private GroupBox groupBox6;
        private CheckBox chkSearchMultiline;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private CheckBox chkSearchMatchWholeWord;
        private CheckBox chkSearchMatchCase;
        private Button buttonFind;
        private Button buttonReplace;
        private Button buttonReplaceAll;
        private Button buttonClose;
        private ContextMenuStrip menuRtbSearch;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator39;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private Button buttonFindNext;
        public RichTextBox rtbSearchPattern;
        public RichTextBox rtbSearchReplace;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem insertNewLineAtCaretToolStripMenuItem;
        private Label label1;
        private Label label2;
        private TextBox txtBoxCurrent;
        private TextBox txtBoxTotal;
        private Button buttonFindPrevious;
        private TextBox txtBoxIndex;
        private Label label3;
        private Button buttonGotoIndex;
    }
}