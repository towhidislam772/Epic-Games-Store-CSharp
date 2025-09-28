using System;

namespace WindowsFormsApp3
{
    partial class StartUp
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartUp));
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2CirclePictureBox2 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2CirclePictureBox3 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnAboutUs = new Guna.UI2.WinForms.Guna2Button();
            this.btnTC = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("guna2PictureBox1.Image")));
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(-1, -1);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(994, 513);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2PictureBox1.TabIndex = 5;
            this.guna2PictureBox1.TabStop = false;
            this.guna2PictureBox1.Click += new System.EventHandler(this.guna2PictureBox1_Click);
            // 
            // guna2Button1
            // 
            this.guna2Button1.BorderRadius = 10;
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(151)))), ((int)(((byte)(234)))));
            this.guna2Button1.Location = new System.Drawing.Point(407, 394);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(174, 41);
            this.guna2Button1.TabIndex = 0;
            this.guna2Button1.Text = "Let\'s Get Started";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("guna2CirclePictureBox1.Image")));
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(358, 86);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(33, 36);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2CirclePictureBox1.TabIndex = 6;
            this.guna2CirclePictureBox1.TabStop = false;
            this.guna2CirclePictureBox1.UseTransparentBackground = true;
            this.guna2CirclePictureBox1.Click += new System.EventHandler(this.guna2CirclePictureBox1_Click);
            // 
            // guna2CirclePictureBox2
            // 
            this.guna2CirclePictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("guna2CirclePictureBox2.Image")));
            this.guna2CirclePictureBox2.ImageRotate = 0F;
            this.guna2CirclePictureBox2.Location = new System.Drawing.Point(482, 86);
            this.guna2CirclePictureBox2.Name = "guna2CirclePictureBox2";
            this.guna2CirclePictureBox2.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox2.Size = new System.Drawing.Size(32, 36);
            this.guna2CirclePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox2.TabIndex = 7;
            this.guna2CirclePictureBox2.TabStop = false;
            this.guna2CirclePictureBox2.UseTransparentBackground = true;
            this.guna2CirclePictureBox2.Click += new System.EventHandler(this.guna2CirclePictureBox2_Click);
            // 
            // guna2CirclePictureBox3
            // 
            this.guna2CirclePictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("guna2CirclePictureBox3.Image")));
            this.guna2CirclePictureBox3.ImageRotate = 0F;
            this.guna2CirclePictureBox3.Location = new System.Drawing.Point(598, 86);
            this.guna2CirclePictureBox3.Name = "guna2CirclePictureBox3";
            this.guna2CirclePictureBox3.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox3.Size = new System.Drawing.Size(36, 36);
            this.guna2CirclePictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2CirclePictureBox3.TabIndex = 8;
            this.guna2CirclePictureBox3.TabStop = false;
            this.guna2CirclePictureBox3.UseTransparentBackground = true;
            this.guna2CirclePictureBox3.Click += new System.EventHandler(this.guna2CirclePictureBox3_Click);
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.BackColor = System.Drawing.Color.Red;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(958, -1);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(35, 28);
            this.guna2ControlBox1.TabIndex = 9;
            // 
            // btnAboutUs
            // 
            this.btnAboutUs.BorderRadius = 10;
            this.btnAboutUs.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnAboutUs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAboutUs.ForeColor = System.Drawing.Color.White;
            this.btnAboutUs.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(151)))), ((int)(((byte)(234)))));
            this.btnAboutUs.Location = new System.Drawing.Point(301, 459);
            this.btnAboutUs.Name = "btnAboutUs";
            this.btnAboutUs.Size = new System.Drawing.Size(150, 26);
            this.btnAboutUs.TabIndex = 0;
            this.btnAboutUs.Text = "About Us";
            this.btnAboutUs.Click += new System.EventHandler(this.btnAboutUs_Click);
            // 
            // btnTC
            // 
            this.btnTC.BorderRadius = 10;
            this.btnTC.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnTC.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTC.ForeColor = System.Drawing.Color.White;
            this.btnTC.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(151)))), ((int)(((byte)(234)))));
            this.btnTC.Location = new System.Drawing.Point(526, 459);
            this.btnTC.Name = "btnTC";
            this.btnTC.Size = new System.Drawing.Size(150, 26);
            this.btnTC.TabIndex = 0;
            this.btnTC.Text = "Terms and Conditions";
            this.btnTC.Click += new System.EventHandler(this.btnTC_Click);
            // 
            // StartUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(992, 512);
            this.Controls.Add(this.guna2ControlBox1);
            this.Controls.Add(this.guna2CirclePictureBox3);
            this.Controls.Add(this.guna2CirclePictureBox2);
            this.Controls.Add(this.guna2CirclePictureBox1);
            this.Controls.Add(this.btnTC);
            this.Controls.Add(this.btnAboutUs);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.guna2PictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StartUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Epic Game Store (Parody)";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox3)).EndInit();
            this.ResumeLayout(false);

        }



        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox2;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox3;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2Button btnAboutUs;
        private Guna.UI2.WinForms.Guna2Button btnTC;
    }
}
