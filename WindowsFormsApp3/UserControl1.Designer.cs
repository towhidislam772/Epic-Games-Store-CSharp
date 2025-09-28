namespace WindowsFormsApp3
{
    partial class UserControl1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl1));
            this.pictureBoxPhoto = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.labelPrice = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.labelName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPhoto)).BeginInit();
            this.guna2GradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxPhoto
            // 
            this.pictureBoxPhoto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxPhoto.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxPhoto.Image")));
            this.pictureBoxPhoto.ImageRotate = 0F;
            this.pictureBoxPhoto.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPhoto.Margin = new System.Windows.Forms.Padding(10);
            this.pictureBoxPhoto.Name = "pictureBoxPhoto";
            this.pictureBoxPhoto.Size = new System.Drawing.Size(181, 145);
            this.pictureBoxPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPhoto.TabIndex = 0;
            this.pictureBoxPhoto.TabStop = false;
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.Controls.Add(this.labelName);
            this.guna2GradientPanel1.Controls.Add(this.labelPrice);
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(71)))), ((int)(((byte)(94)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(0, 152);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.Size = new System.Drawing.Size(181, 74);
            this.guna2GradientPanel1.TabIndex = 1;
            // 
            // labelPrice
            // 
            this.labelPrice.BackColor = System.Drawing.Color.Transparent;
            this.labelPrice.ForeColor = System.Drawing.SystemColors.Control;
            this.labelPrice.Location = new System.Drawing.Point(136, 28);
            this.labelPrice.Name = "labelPrice";
            this.labelPrice.Size = new System.Drawing.Size(21, 15);
            this.labelPrice.TabIndex = 1;
            this.labelPrice.Text = "23$";
            // 
            // labelName
            // 
            this.labelName.BackColor = System.Drawing.Color.Transparent;
            this.labelName.ForeColor = System.Drawing.Color.White;
            this.labelName.Location = new System.Drawing.Point(12, 18);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(116, 47);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "label1";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guna2GradientPanel1);
            this.Controls.Add(this.pictureBoxPhoto);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(181, 226);
            this.Click += new System.EventHandler(this.UserControl1_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPhoto)).EndInit();
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2GradientPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2PictureBox pictureBoxPhoto;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelPrice;
        private System.Windows.Forms.Label labelName;
    }
}
