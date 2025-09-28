namespace WindowsFormsApp3
{
    partial class UserControl4
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblDiscountPercent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Discount Percent";
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(138, 15);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(35, 13);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "label1";
            // 
            // lblDiscountPercent
            // 
            this.lblDiscountPercent.AutoSize = true;
            this.lblDiscountPercent.Location = new System.Drawing.Point(138, 45);
            this.lblDiscountPercent.Name = "lblDiscountPercent";
            this.lblDiscountPercent.Size = new System.Drawing.Size(35, 13);
            this.lblDiscountPercent.TabIndex = 1;
            this.lblDiscountPercent.Text = "label2";
            // 
            // UserControl4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.lblDiscountPercent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.label1);
            this.Name = "UserControl4";
            this.Size = new System.Drawing.Size(287, 71);
            this.Load += new System.EventHandler(this.UserControl4_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Label lblDiscountPercent;
    }
}
