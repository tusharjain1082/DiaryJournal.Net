namespace DiaryJournal.Net
{
    partial class CustomFontDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFontDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstSize = new System.Windows.Forms.ListBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.chbBold = new System.Windows.Forms.CheckBox();
            this.chbItalic = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chbUnderline = new System.Windows.Forms.CheckBox();
            this.chbStrikeout = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSampleText = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lvFontColor = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvFontBackColor = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvFonts = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.txtFont = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Size:";
            // 
            // lstSize
            // 
            this.lstSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSize.FormattingEnabled = true;
            this.lstSize.ItemHeight = 15;
            this.lstSize.Location = new System.Drawing.Point(222, 58);
            this.lstSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstSize.Name = "lstSize";
            this.lstSize.Size = new System.Drawing.Size(116, 137);
            this.lstSize.TabIndex = 4;
            this.lstSize.SelectedIndexChanged += new System.EventHandler(this.lstSize_SelectedIndexChanged);
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(222, 35);
            this.txtSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(116, 23);
            this.txtSize.TabIndex = 5;
            this.txtSize.TextChanged += new System.EventHandler(this.txtSize_TextChanged);
            this.txtSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSize_KeyDown);
            // 
            // chbBold
            // 
            this.chbBold.AutoSize = true;
            this.chbBold.Location = new System.Drawing.Point(24, 31);
            this.chbBold.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbBold.Name = "chbBold";
            this.chbBold.Size = new System.Drawing.Size(50, 19);
            this.chbBold.TabIndex = 6;
            this.chbBold.Text = "Bold";
            this.chbBold.UseVisualStyleBackColor = true;
            this.chbBold.CheckedChanged += new System.EventHandler(this.chb_CheckedChanged);
            // 
            // chbItalic
            // 
            this.chbItalic.AutoSize = true;
            this.chbItalic.Location = new System.Drawing.Point(24, 58);
            this.chbItalic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbItalic.Name = "chbItalic";
            this.chbItalic.Size = new System.Drawing.Size(51, 19);
            this.chbItalic.TabIndex = 7;
            this.chbItalic.Text = "Italic";
            this.chbItalic.UseVisualStyleBackColor = true;
            this.chbItalic.CheckedChanged += new System.EventHandler(this.chb_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chbUnderline);
            this.groupBox1.Controls.Add(this.chbStrikeout);
            this.groupBox1.Controls.Add(this.chbBold);
            this.groupBox1.Controls.Add(this.chbItalic);
            this.groupBox1.Location = new System.Drawing.Point(364, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(137, 182);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Font Style";
            // 
            // chbUnderline
            // 
            this.chbUnderline.AutoSize = true;
            this.chbUnderline.Location = new System.Drawing.Point(24, 109);
            this.chbUnderline.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbUnderline.Name = "chbUnderline";
            this.chbUnderline.Size = new System.Drawing.Size(77, 19);
            this.chbUnderline.TabIndex = 9;
            this.chbUnderline.Text = "Underline";
            this.chbUnderline.UseVisualStyleBackColor = true;
            this.chbUnderline.CheckedChanged += new System.EventHandler(this.chbUnderline_CheckedChanged);
            // 
            // chbStrikeout
            // 
            this.chbStrikeout.AutoSize = true;
            this.chbStrikeout.Location = new System.Drawing.Point(24, 84);
            this.chbStrikeout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbStrikeout.Name = "chbStrikeout";
            this.chbStrikeout.Size = new System.Drawing.Size(73, 19);
            this.chbStrikeout.TabIndex = 8;
            this.chbStrikeout.Text = "Strikeout";
            this.chbStrikeout.UseVisualStyleBackColor = true;
            this.chbStrikeout.CheckedChanged += new System.EventHandler(this.chb_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSampleText);
            this.groupBox2.Location = new System.Drawing.Point(222, 203);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(279, 128);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sample Text";
            // 
            // lblSampleText
            // 
            this.lblSampleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSampleText.Location = new System.Drawing.Point(7, 18);
            this.lblSampleText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSampleText.Name = "lblSampleText";
            this.lblSampleText.Size = new System.Drawing.Size(267, 100);
            this.lblSampleText.TabIndex = 0;
            this.lblSampleText.Text = "AaBbCcXxYyZz";
            this.lblSampleText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(413, 473);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(318, 473);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lvFontColor
            // 
            this.lvFontColor.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lvFontColor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvFontColor.FullRowSelect = true;
            this.lvFontColor.Location = new System.Drawing.Point(15, 337);
            this.lvFontColor.MultiSelect = false;
            this.lvFontColor.Name = "lvFontColor";
            this.lvFontColor.Size = new System.Drawing.Size(240, 130);
            this.lvFontColor.TabIndex = 12;
            this.lvFontColor.UseCompatibleStateImageBehavior = false;
            this.lvFontColor.View = System.Windows.Forms.View.Details;
            this.lvFontColor.SelectedIndexChanged += new System.EventHandler(this.lvFontColor_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "select font color";
            this.columnHeader1.Width = 400;
            // 
            // lvFontBackColor
            // 
            this.lvFontBackColor.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lvFontBackColor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvFontBackColor.FullRowSelect = true;
            this.lvFontBackColor.Location = new System.Drawing.Point(261, 337);
            this.lvFontBackColor.MultiSelect = false;
            this.lvFontBackColor.Name = "lvFontBackColor";
            this.lvFontBackColor.Size = new System.Drawing.Size(240, 130);
            this.lvFontBackColor.TabIndex = 13;
            this.lvFontBackColor.UseCompatibleStateImageBehavior = false;
            this.lvFontBackColor.View = System.Windows.Forms.View.Details;
            this.lvFontBackColor.SelectedIndexChanged += new System.EventHandler(this.lvFontBackColor_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "select font back color";
            this.columnHeader2.Width = 400;
            // 
            // lvFonts
            // 
            this.lvFonts.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lvFonts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvFonts.FullRowSelect = true;
            this.lvFonts.Location = new System.Drawing.Point(17, 58);
            this.lvFonts.MultiSelect = false;
            this.lvFonts.Name = "lvFonts";
            this.lvFonts.Size = new System.Drawing.Size(198, 273);
            this.lvFonts.TabIndex = 14;
            this.lvFonts.UseCompatibleStateImageBehavior = false;
            this.lvFonts.View = System.Windows.Forms.View.Details;
            this.lvFonts.SelectedIndexChanged += new System.EventHandler(this.lvFonts_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "fonts";
            this.columnHeader3.Width = 400;
            // 
            // txtFont
            // 
            this.txtFont.Location = new System.Drawing.Point(17, 35);
            this.txtFont.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFont.Name = "txtFont";
            this.txtFont.Size = new System.Drawing.Size(198, 23);
            this.txtFont.TabIndex = 15;
            // 
            // CustomFontDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 506);
            this.Controls.Add(this.txtFont);
            this.Controls.Add(this.lvFonts);
            this.Controls.Add(this.lvFontBackColor);
            this.Controls.Add(this.lvFontColor);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.lstSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomFontDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Font";
            this.Load += new System.EventHandler(this.CustomFontDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstSize;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.CheckBox chbBold;
        private System.Windows.Forms.CheckBox chbItalic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbStrikeout;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSampleText;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private ListView lvFontColor;
        private ColumnHeader columnHeader1;
        private ListView lvFontBackColor;
        private ColumnHeader columnHeader2;
        private CheckBox chbUnderline;
        private ListView lvFonts;
        private ColumnHeader columnHeader3;
        private TextBox txtFont;
    }
}