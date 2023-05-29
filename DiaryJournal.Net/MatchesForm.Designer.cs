namespace DiaryJournal.Net
{
    partial class MatchesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatchesForm));
            this.sc = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.sc)).BeginInit();
            this.sc.SuspendLayout();
            this.SuspendLayout();
            // 
            // sc
            // 
            this.sc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sc.Location = new System.Drawing.Point(0, 0);
            this.sc.Name = "sc";
            this.sc.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sc.Panel1MinSize = 40;
            this.sc.Size = new System.Drawing.Size(800, 450);
            this.sc.SplitterDistance = 40;
            this.sc.TabIndex = 0;
            // 
            // MatchesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.sc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MatchesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "matches found in the entry";
            this.Load += new System.EventHandler(this.MatchesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sc)).EndInit();
            this.sc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer sc;
    }
}