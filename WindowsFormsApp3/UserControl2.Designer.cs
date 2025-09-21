namespace WindowsFormsApp3
{
    partial class UserControl2
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUserName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2RatingStar2 = new Guna.UI2.WinForms.Guna2RatingStar();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.lblReviews = new System.Windows.Forms.Label();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(4, 15);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(83, 22);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name";
            // 
            // guna2RatingStar2
            // 
            this.guna2RatingStar2.Location = new System.Drawing.Point(146, 5);
            this.guna2RatingStar2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.guna2RatingStar2.Name = "guna2RatingStar2";
            this.guna2RatingStar2.Size = new System.Drawing.Size(180, 43);
            this.guna2RatingStar2.TabIndex = 6;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.AutoScroll = true;
            this.guna2Panel1.AutoSize = true;
            this.guna2Panel1.Controls.Add(this.lblReviews);
            this.guna2Panel1.Location = new System.Drawing.Point(4, 57);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(326, 113);
            this.guna2Panel1.TabIndex = 7;
            // 
            // lblReviews
            // 
            this.lblReviews.Location = new System.Drawing.Point(7, 6);
            this.lblReviews.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReviews.Name = "lblReviews";
            this.lblReviews.Size = new System.Drawing.Size(310, 101);
            this.lblReviews.TabIndex = 0;
            this.lblReviews.Text = "label1";
            this.lblReviews.UseCompatibleTextRendering = true;
            // 
            // UserControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.guna2RatingStar2);
            this.Controls.Add(this.lblUserName);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UserControl2";
            this.Size = new System.Drawing.Size(334, 183);
            this.Load += new System.EventHandler(this.UserControl2_Load);
            this.guna2Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblUserName;
        private Guna.UI2.WinForms.Guna2RatingStar guna2RatingStar2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label lblReviews;
    }
}
