namespace Colorlink
{
    partial class flowindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(flowindow));
            this.pBoxShowSolution = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxShowSolution)).BeginInit();
            this.SuspendLayout();
            // 
            // pBoxShowSolution
            // 
            this.pBoxShowSolution.InitialImage = ((System.Drawing.Image)(resources.GetObject("pBoxShowSolution.InitialImage")));
            this.pBoxShowSolution.Location = new System.Drawing.Point(12, 922);
            this.pBoxShowSolution.Name = "pBoxShowSolution";
            this.pBoxShowSolution.Size = new System.Drawing.Size(100, 50);
            this.pBoxShowSolution.TabIndex = 1;
            this.pBoxShowSolution.TabStop = false;
            this.pBoxShowSolution.Click += new System.EventHandler(this.ShowSolutionClicked);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblMessage.Location = new System.Drawing.Point(12, 975);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(70, 25);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "label1";
            // 
            // flowindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1894, 1009);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pBoxShowSolution);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MinimumSize = new System.Drawing.Size(480, 360);
            this.Name = "flowindow";
            this.Text = "flow";
            ((System.ComponentModel.ISupportInitialize)(this.pBoxShowSolution)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pBoxShowSolution;
        private System.Windows.Forms.Label lblMessage;
    }
}

