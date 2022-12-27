namespace DiaryJournal.Net
{
    partial class FormOperation
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
            this.labelDescription = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilesDone = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotalFiles = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelDescription.Location = new System.Drawing.Point(12, 9);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(660, 100);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "description";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 112);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(660, 40);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "files done:";
            // 
            // txtFilesDone
            // 
            this.txtFilesDone.Location = new System.Drawing.Point(106, 161);
            this.txtFilesDone.Name = "txtFilesDone";
            this.txtFilesDone.ReadOnly = true;
            this.txtFilesDone.Size = new System.Drawing.Size(220, 27);
            this.txtFilesDone.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(372, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "total files:";
            // 
            // txtTotalFiles
            // 
            this.txtTotalFiles.Location = new System.Drawing.Point(452, 161);
            this.txtTotalFiles.Name = "txtTotalFiles";
            this.txtTotalFiles.ReadOnly = true;
            this.txtTotalFiles.Size = new System.Drawing.Size(220, 27);
            this.txtTotalFiles.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(522, 194);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(150, 30);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 231);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.txtTotalFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilesDone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelDescription);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOperation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "operation";
            this.Load += new System.EventHandler(this.FormOperation_Load);
            this.Shown += FormOperation_Shown;
            this.FormClosed += FormOperation_FormClosed;
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private Label labelDescription;
        private ProgressBar progressBar;
        private Label label1;
        private TextBox txtFilesDone;
        private Label label2;
        private TextBox txtTotalFiles;
        private Button buttonCancel;
    }
}