namespace DiaryJournal.Net
{
    partial class FormSortOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioSortWithoutId = new System.Windows.Forms.RadioButton();
            this.radioSortByIdExtra = new System.Windows.Forms.RadioButton();
            this.radioSortById = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioModificationDateTime = new System.Windows.Forms.RadioButton();
            this.radioCreationDateTime = new System.Windows.Forms.RadioButton();
            this.radioCommonDateTime = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioSortAscending = new System.Windows.Forms.RadioButton();
            this.radioSortDescending = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSortWithoutId);
            this.groupBox1.Controls.Add(this.radioSortByIdExtra);
            this.groupBox1.Controls.Add(this.radioSortById);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "id options";
            // 
            // radioSortWithoutId
            // 
            this.radioSortWithoutId.AutoSize = true;
            this.radioSortWithoutId.Location = new System.Drawing.Point(346, 26);
            this.radioSortWithoutId.Name = "radioSortWithoutId";
            this.radioSortWithoutId.Size = new System.Drawing.Size(98, 24);
            this.radioSortWithoutId.TabIndex = 2;
            this.radioSortWithoutId.Text = "no id at all";
            this.radioSortWithoutId.UseVisualStyleBackColor = true;
            // 
            // radioSortByIdExtra
            // 
            this.radioSortByIdExtra.AutoSize = true;
            this.radioSortByIdExtra.Location = new System.Drawing.Point(133, 26);
            this.radioSortByIdExtra.Name = "radioSortByIdExtra";
            this.radioSortByIdExtra.Size = new System.Drawing.Size(204, 24);
            this.radioSortByIdExtra.TabIndex = 1;
            this.radioSortByIdExtra.Text = "sort by id with extra config";
            this.radioSortByIdExtra.UseVisualStyleBackColor = true;
            // 
            // radioSortById
            // 
            this.radioSortById.AutoSize = true;
            this.radioSortById.Checked = true;
            this.radioSortById.Location = new System.Drawing.Point(6, 26);
            this.radioSortById.Name = "radioSortById";
            this.radioSortById.Size = new System.Drawing.Size(121, 24);
            this.radioSortById.TabIndex = 0;
            this.radioSortById.TabStop = true;
            this.radioSortById.Text = "sort by id only";
            this.radioSortById.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioModificationDateTime);
            this.groupBox2.Controls.Add(this.radioCreationDateTime);
            this.groupBox2.Controls.Add(this.radioCommonDateTime);
            this.groupBox2.Location = new System.Drawing.Point(12, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 121);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "date and time options (extra config)";
            // 
            // radioModificationDateTime
            // 
            this.radioModificationDateTime.AutoSize = true;
            this.radioModificationDateTime.Location = new System.Drawing.Point(6, 86);
            this.radioModificationDateTime.Name = "radioModificationDateTime";
            this.radioModificationDateTime.Size = new System.Drawing.Size(209, 24);
            this.radioModificationDateTime.TabIndex = 2;
            this.radioModificationDateTime.Text = "modification date and time";
            this.radioModificationDateTime.UseVisualStyleBackColor = true;
            // 
            // radioCreationDateTime
            // 
            this.radioCreationDateTime.AutoSize = true;
            this.radioCreationDateTime.Location = new System.Drawing.Point(6, 56);
            this.radioCreationDateTime.Name = "radioCreationDateTime";
            this.radioCreationDateTime.Size = new System.Drawing.Size(178, 24);
            this.radioCreationDateTime.TabIndex = 1;
            this.radioCreationDateTime.Text = "creation date and time";
            this.radioCreationDateTime.UseVisualStyleBackColor = true;
            // 
            // radioCommonDateTime
            // 
            this.radioCommonDateTime.AutoSize = true;
            this.radioCommonDateTime.Checked = true;
            this.radioCommonDateTime.Location = new System.Drawing.Point(6, 26);
            this.radioCommonDateTime.Name = "radioCommonDateTime";
            this.radioCommonDateTime.Size = new System.Drawing.Size(183, 24);
            this.radioCommonDateTime.TabIndex = 0;
            this.radioCommonDateTime.TabStop = true;
            this.radioCommonDateTime.Text = "common date and time";
            this.radioCommonDateTime.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(289, 291);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 291);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioSortAscending);
            this.groupBox3.Controls.Add(this.radioSortDescending);
            this.groupBox3.Location = new System.Drawing.Point(12, 215);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(460, 70);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "other sorting options (required)";
            // 
            // radioSortAscending
            // 
            this.radioSortAscending.AutoSize = true;
            this.radioSortAscending.Checked = true;
            this.radioSortAscending.Location = new System.Drawing.Point(144, 26);
            this.radioSortAscending.Name = "radioSortAscending";
            this.radioSortAscending.Size = new System.Drawing.Size(123, 24);
            this.radioSortAscending.TabIndex = 1;
            this.radioSortAscending.TabStop = true;
            this.radioSortAscending.Text = "sort ascending";
            this.radioSortAscending.UseVisualStyleBackColor = true;
            // 
            // radioSortDescending
            // 
            this.radioSortDescending.AutoSize = true;
            this.radioSortDescending.Location = new System.Drawing.Point(6, 26);
            this.radioSortDescending.Name = "radioSortDescending";
            this.radioSortDescending.Size = new System.Drawing.Size(132, 24);
            this.radioSortDescending.TabIndex = 0;
            this.radioSortDescending.Text = "sort descending";
            this.radioSortDescending.UseVisualStyleBackColor = true;
            // 
            // FormSortOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 331);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSortOptions";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "sorting options";
            this.Load += new System.EventHandler(this.FormSortOptions_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton radioSortById;
        private RadioButton radioSortWithoutId;
        private RadioButton radioSortByIdExtra;
        private GroupBox groupBox2;
        private RadioButton radioCommonDateTime;
        private RadioButton radioModificationDateTime;
        private RadioButton radioCreationDateTime;
        private Button btnOK;
        private Button btnCancel;
        private GroupBox groupBox3;
        private RadioButton radioSortAscending;
        private RadioButton radioSortDescending;
    }
}