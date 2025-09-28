namespace WindowsFormsApp3
{
    partial class AddtoCart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddtoCart));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.userControl31 = new WindowsFormsApp3.UserControl3();
            this.btnEpicGames = new Guna.UI2.WinForms.Guna2ImageButton();
            this.guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.btnBuyNow = new Guna.UI2.WinForms.Guna2Button();
            this.lebelTotalPrice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.guna2CustomGradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.userControl31);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(26, 118);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(950, 203);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // userControl31
            // 
            this.userControl31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.userControl31.CartItemID = 0;
            this.userControl31.GameID = 0;
            this.userControl31.GameImage = null;
            this.userControl31.GamePrice = "";
            this.userControl31.GameTitle = "";
            this.userControl31.Location = new System.Drawing.Point(3, 3);
            this.userControl31.Name = "userControl31";
            this.userControl31.Size = new System.Drawing.Size(379, 75);
            this.userControl31.TabIndex = 0;
            // 
            // userCouponsTableAdapter1
            // 
            // 
            // btnEpicGames
            // 
            this.btnEpicGames.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.btnEpicGames.HoverState.ImageSize = new System.Drawing.Size(64, 64);
            this.btnEpicGames.Image = ((System.Drawing.Image)(resources.GetObject("btnEpicGames.Image")));
            this.btnEpicGames.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnEpicGames.ImageRotate = 0F;
            this.btnEpicGames.Location = new System.Drawing.Point(29, 12);
            this.btnEpicGames.Name = "btnEpicGames";
            this.btnEpicGames.PressedState.ImageSize = new System.Drawing.Size(64, 64);
            this.btnEpicGames.Size = new System.Drawing.Size(78, 88);
            this.btnEpicGames.TabIndex = 2;
            this.btnEpicGames.Click += new System.EventHandler(this.btnEpicGames_Click);
            // 
            // guna2CustomGradientPanel1
            // 
            this.guna2CustomGradientPanel1.Controls.Add(this.btnBuyNow);
            this.guna2CustomGradientPanel1.Controls.Add(this.lebelTotalPrice);
            this.guna2CustomGradientPanel1.Controls.Add(this.label1);
            this.guna2CustomGradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(60)))), ((int)(((byte)(114)))));
            this.guna2CustomGradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(82)))), ((int)(((byte)(152)))));
            this.guna2CustomGradientPanel1.FillColor3 = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(254)))));
            this.guna2CustomGradientPanel1.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.guna2CustomGradientPanel1.Location = new System.Drawing.Point(26, 351);
            this.guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            this.guna2CustomGradientPanel1.Size = new System.Drawing.Size(950, 111);
            this.guna2CustomGradientPanel1.TabIndex = 3;
            // 
            // btnBuyNow
            // 
            this.btnBuyNow.AccessibleRole = System.Windows.Forms.AccessibleRole.Sound;
            this.btnBuyNow.Animated = true;
            this.btnBuyNow.BackColor = System.Drawing.Color.Transparent;
            this.btnBuyNow.Checked = true;
            this.btnBuyNow.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBuyNow.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBuyNow.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBuyNow.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBuyNow.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnBuyNow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBuyNow.ForeColor = System.Drawing.Color.White;
            this.btnBuyNow.ImageSize = new System.Drawing.Size(28, 28);
            this.btnBuyNow.IndicateFocus = true;
            this.btnBuyNow.Location = new System.Drawing.Point(808, 39);
            this.btnBuyNow.Name = "btnBuyNow";
            this.btnBuyNow.Size = new System.Drawing.Size(88, 27);
            this.btnBuyNow.TabIndex = 4;
            this.btnBuyNow.Text = "Buy Now";
            this.btnBuyNow.UseTransparentBackground = true;
            this.btnBuyNow.Click += new System.EventHandler(this.btnBuyNow_Click);
            // 
            // lebelTotalPrice
            // 
            this.lebelTotalPrice.AutoSize = true;
            this.lebelTotalPrice.BackColor = System.Drawing.Color.Transparent;
            this.lebelTotalPrice.ForeColor = System.Drawing.Color.White;
            this.lebelTotalPrice.Location = new System.Drawing.Point(107, 47);
            this.lebelTotalPrice.Name = "lebelTotalPrice";
            this.lebelTotalPrice.Size = new System.Drawing.Size(64, 13);
            this.lebelTotalPrice.TabIndex = 0;
            this.lebelTotalPrice.Text = "Total Price :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(37, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Price :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Uighur", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(130, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 36);
            this.label2.TabIndex = 0;
            this.label2.Text = "Your Cart";
            // 
            // AddtoCart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(994, 514);
            this.Controls.Add(this.guna2CustomGradientPanel1);
            this.Controls.Add(this.btnEpicGames);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddtoCart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddtoCart";
            this.Load += new System.EventHandler(this.AddtoCart_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.guna2CustomGradientPanel1.ResumeLayout(false);
            this.guna2CustomGradientPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Guna.UI2.WinForms.Guna2ImageButton btnEpicGames;
        private UserControl3 userControl31;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lebelTotalPrice;
        private Guna.UI2.WinForms.Guna2Button btnBuyNow;
        private System.Windows.Forms.Label label2;
    }
}