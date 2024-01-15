namespace DiaryJournal.Net
{
    partial class FormInsertTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsertTable));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbColumns = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbRows = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTableWidth = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbCellVAlignment = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbColumnWidth = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRowHeight = new System.Windows.Forms.ComboBox();
            this.cmbTableAlignment = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbCellTextAlignment = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbTableInnerBorder = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbTableInnerBorderSize = new System.Windows.Forms.ComboBox();
            this.cmbTableOuterBorder = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbTableOuterBorderSize = new System.Windows.Forms.ComboBox();
            this.buttonFont = new System.Windows.Forms.Button();
            this.txtboxFont = new System.Windows.Forms.TextBox();
            this.txtboxFontSize = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbColumns);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbRows);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbTableWidth);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 142);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "table size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "columns:";
            // 
            // cmbColumns
            // 
            this.cmbColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColumns.FormattingEnabled = true;
            this.cmbColumns.Location = new System.Drawing.Point(211, 104);
            this.cmbColumns.Name = "cmbColumns";
            this.cmbColumns.Size = new System.Drawing.Size(256, 33);
            this.cmbColumns.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "rows:";
            // 
            // cmbRows
            // 
            this.cmbRows.FormattingEnabled = true;
            this.cmbRows.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "15",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.cmbRows.Location = new System.Drawing.Point(211, 65);
            this.cmbRows.Name = "cmbRows";
            this.cmbRows.Size = new System.Drawing.Size(256, 33);
            this.cmbRows.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "table width:";
            // 
            // cmbTableWidth
            // 
            this.cmbTableWidth.Enabled = false;
            this.cmbTableWidth.FormattingEnabled = true;
            this.cmbTableWidth.Items.AddRange(new object[] {
            "100",
            "200",
            "500",
            "600",
            "700",
            "800",
            "1000",
            "1500",
            "2000",
            "3000"});
            this.cmbTableWidth.Location = new System.Drawing.Point(211, 26);
            this.cmbTableWidth.Name = "cmbTableWidth";
            this.cmbTableWidth.Size = new System.Drawing.Size(256, 33);
            this.cmbTableWidth.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cmbCellVAlignment);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cmbColumnWidth);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cmbRowHeight);
            this.groupBox2.Controls.Add(this.cmbTableAlignment);
            this.groupBox2.Location = new System.Drawing.Point(12, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(476, 190);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "table layout";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(201, 25);
            this.label7.TabIndex = 13;
            this.label7.Text = "cell vertical alignment:";
            // 
            // cmbCellVAlignment
            // 
            this.cmbCellVAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCellVAlignment.Enabled = false;
            this.cmbCellVAlignment.FormattingEnabled = true;
            this.cmbCellVAlignment.Items.AddRange(new object[] {
            "Top",
            "Center",
            "Bottom"});
            this.cmbCellVAlignment.Location = new System.Drawing.Point(211, 149);
            this.cmbCellVAlignment.Name = "cmbCellVAlignment";
            this.cmbCellVAlignment.Size = new System.Drawing.Size(256, 33);
            this.cmbCellVAlignment.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 25);
            this.label6.TabIndex = 11;
            this.label6.Text = "column width:";
            // 
            // cmbColumnWidth
            // 
            this.cmbColumnWidth.FormattingEnabled = true;
            this.cmbColumnWidth.Location = new System.Drawing.Point(211, 110);
            this.cmbColumnWidth.Name = "cmbColumnWidth";
            this.cmbColumnWidth.Size = new System.Drawing.Size(256, 33);
            this.cmbColumnWidth.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 25);
            this.label5.TabIndex = 9;
            this.label5.Text = "row height:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "table alignment:";
            // 
            // cmbRowHeight
            // 
            this.cmbRowHeight.Enabled = false;
            this.cmbRowHeight.FormattingEnabled = true;
            this.cmbRowHeight.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "15",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.cmbRowHeight.Location = new System.Drawing.Point(211, 71);
            this.cmbRowHeight.Name = "cmbRowHeight";
            this.cmbRowHeight.Size = new System.Drawing.Size(256, 33);
            this.cmbRowHeight.TabIndex = 8;
            // 
            // cmbTableAlignment
            // 
            this.cmbTableAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableAlignment.Enabled = false;
            this.cmbTableAlignment.FormattingEnabled = true;
            this.cmbTableAlignment.Items.AddRange(new object[] {
            "Left",
            "Center",
            "Right"});
            this.cmbTableAlignment.Location = new System.Drawing.Point(211, 32);
            this.cmbTableAlignment.Name = "cmbTableAlignment";
            this.cmbTableAlignment.Size = new System.Drawing.Size(256, 33);
            this.cmbTableAlignment.TabIndex = 8;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbCellTextAlignment);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(12, 356);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 157);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "cell formatting";
            // 
            // cmbCellTextAlignment
            // 
            this.cmbCellTextAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCellTextAlignment.FormattingEnabled = true;
            this.cmbCellTextAlignment.Items.AddRange(new object[] {
            "Left",
            "Center",
            "Right",
            "Justify"});
            this.cmbCellTextAlignment.Location = new System.Drawing.Point(211, 27);
            this.cmbCellTextAlignment.Name = "cmbCellTextAlignment";
            this.cmbCellTextAlignment.Size = new System.Drawing.Size(256, 33);
            this.cmbCellTextAlignment.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(171, 25);
            this.label10.TabIndex = 15;
            this.label10.Text = "cell text alignment:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(500, 328);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 25);
            this.label9.TabIndex = 17;
            this.label9.Text = "font size:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(500, 283);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 25);
            this.label8.TabIndex = 15;
            this.label8.Text = "font:";
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(820, 507);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(150, 40);
            this.cmdOk.TabIndex = 10;
            this.cmdOk.Text = "ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(664, 506);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(150, 40);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbTableInnerBorder);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.cmbTableInnerBorderSize);
            this.groupBox4.Controls.Add(this.cmbTableOuterBorder);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.cmbTableOuterBorderSize);
            this.groupBox4.Location = new System.Drawing.Point(494, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(476, 268);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "text formatting";
            // 
            // cmbTableInnerBorder
            // 
            this.cmbTableInnerBorder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableInnerBorder.Enabled = false;
            this.cmbTableInnerBorder.FormattingEnabled = true;
            this.cmbTableInnerBorder.Items.AddRange(new object[] {
            "Single",
            "None"});
            this.cmbTableInnerBorder.Location = new System.Drawing.Point(214, 109);
            this.cmbTableInnerBorder.Name = "cmbTableInnerBorder";
            this.cmbTableInnerBorder.Size = new System.Drawing.Size(256, 33);
            this.cmbTableInnerBorder.TabIndex = 18;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 151);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(206, 25);
            this.label13.TabIndex = 20;
            this.label13.Text = "table inner border size:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(169, 25);
            this.label14.TabIndex = 21;
            this.label14.Text = "table inner border:";
            // 
            // cmbTableInnerBorderSize
            // 
            this.cmbTableInnerBorderSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableInnerBorderSize.FormattingEnabled = true;
            this.cmbTableInnerBorderSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbTableInnerBorderSize.Location = new System.Drawing.Point(214, 151);
            this.cmbTableInnerBorderSize.Name = "cmbTableInnerBorderSize";
            this.cmbTableInnerBorderSize.Size = new System.Drawing.Size(256, 33);
            this.cmbTableInnerBorderSize.TabIndex = 19;
            // 
            // cmbTableOuterBorder
            // 
            this.cmbTableOuterBorder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableOuterBorder.Enabled = false;
            this.cmbTableOuterBorder.FormattingEnabled = true;
            this.cmbTableOuterBorder.Items.AddRange(new object[] {
            "Single",
            "None"});
            this.cmbTableOuterBorder.Location = new System.Drawing.Point(214, 26);
            this.cmbTableOuterBorder.Name = "cmbTableOuterBorder";
            this.cmbTableOuterBorder.Size = new System.Drawing.Size(256, 33);
            this.cmbTableOuterBorder.TabIndex = 14;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(207, 25);
            this.label11.TabIndex = 15;
            this.label11.Text = "table outer border size:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(170, 25);
            this.label12.TabIndex = 17;
            this.label12.Text = "table outer border:";
            // 
            // cmbTableOuterBorderSize
            // 
            this.cmbTableOuterBorderSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableOuterBorderSize.Enabled = false;
            this.cmbTableOuterBorderSize.FormattingEnabled = true;
            this.cmbTableOuterBorderSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbTableOuterBorderSize.Location = new System.Drawing.Point(214, 68);
            this.cmbTableOuterBorderSize.Name = "cmbTableOuterBorderSize";
            this.cmbTableOuterBorderSize.Size = new System.Drawing.Size(256, 33);
            this.cmbTableOuterBorderSize.TabIndex = 14;
            // 
            // buttonFont
            // 
            this.buttonFont.Location = new System.Drawing.Point(508, 506);
            this.buttonFont.Name = "buttonFont";
            this.buttonFont.Size = new System.Drawing.Size(150, 40);
            this.buttonFont.TabIndex = 18;
            this.buttonFont.Text = "font formatting";
            this.buttonFont.UseVisualStyleBackColor = true;
            this.buttonFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // txtboxFont
            // 
            this.txtboxFont.Location = new System.Drawing.Point(708, 286);
            this.txtboxFont.Name = "txtboxFont";
            this.txtboxFont.ReadOnly = true;
            this.txtboxFont.Size = new System.Drawing.Size(256, 33);
            this.txtboxFont.TabIndex = 19;
            // 
            // txtboxFontSize
            // 
            this.txtboxFontSize.Location = new System.Drawing.Point(708, 325);
            this.txtboxFontSize.Name = "txtboxFontSize";
            this.txtboxFontSize.ReadOnly = true;
            this.txtboxFontSize.Size = new System.Drawing.Size(256, 33);
            this.txtboxFontSize.TabIndex = 20;
            // 
            // FormInsertTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.txtboxFontSize);
            this.Controls.Add(this.txtboxFont);
            this.Controls.Add(this.buttonFont);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormInsertTable";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "insert table";
            this.Load += new System.EventHandler(this.FormInsertTable_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private ComboBox cmbTableWidth;
        private Label label2;
        private ComboBox cmbRows;
        private Label label3;
        private ComboBox cmbColumns;
        private GroupBox groupBox2;
        private Label label4;
        private ComboBox cmbTableAlignment;
        private Label label6;
        private ComboBox cmbColumnWidth;
        private Label label5;
        private ComboBox cmbRowHeight;
        private Label label7;
        private ComboBox cmbCellVAlignment;
        private GroupBox groupBox3;
        private Label label9;
        private Label label8;
        private Button cmdOk;
        private Button cmdCancel;
        private Label label10;
        private ComboBox cmbCellTextAlignment;
        private GroupBox groupBox4;
        private ComboBox cmbTableOuterBorder;
        private Label label11;
        private ComboBox cmbTableOuterBorderSize;
        private Label label12;
        private ComboBox cmbTableInnerBorder;
        private Label label13;
        private Label label14;
        private ComboBox cmbTableInnerBorderSize;
        private Button buttonFont;
        private TextBox txtboxFont;
        private TextBox txtboxFontSize;
    }
}