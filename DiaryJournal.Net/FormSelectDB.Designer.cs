namespace DiaryJournal.Net
{
    partial class FormSelectDB
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
            groupBox2 = new GroupBox();
            radOFSDB = new RadioButton();
            radSFDBLiteDB = new RadioButton();
            btnOK = new Button();
            btnCancel = new Button();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(radOFSDB);
            groupBox2.Controls.Add(radSFDBLiteDB);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(460, 130);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "choose DB";
            // 
            // radOFSDB
            // 
            radOFSDB.AutoSize = true;
            radOFSDB.Checked = true;
            radOFSDB.Location = new Point(6, 56);
            radOFSDB.Name = "radOFSDB";
            radOFSDB.Size = new Size(157, 24);
            radOFSDB.TabIndex = 2;
            radOFSDB.TabStop = true;
            radOFSDB.Text = "open file system db";
            radOFSDB.UseVisualStyleBackColor = true;
            // 
            // radSFDBLiteDB
            // 
            radSFDBLiteDB.AutoSize = true;
            radSFDBLiteDB.Checked = true;
            radSFDBLiteDB.Location = new Point(6, 26);
            radSFDBLiteDB.Name = "radSFDBLiteDB";
            radSFDBLiteDB.Size = new Size(171, 24);
            radSFDBLiteDB.TabIndex = 1;
            radSFDBLiteDB.TabStop = true;
            radSFDBLiteDB.Text = "single file db (LiteDB)";
            radSFDBLiteDB.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(289, 291);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 13;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(384, 291);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // FormSelectDB
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 331);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(groupBox2);
            Font = new Font("Segoe UI", 11.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSelectDB";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "choose DB";
            Load += FormSelectDB_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox2;
        private Button btnOK;
        private Button btnCancel;
        private RadioButton radOFSDB;
        private RadioButton radSFDBLiteDB;
    }
}