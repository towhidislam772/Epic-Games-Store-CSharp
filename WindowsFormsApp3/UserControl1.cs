using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class UserControl1 : UserControl
    {
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private int gameId;
        private string gameName;
        private decimal Price;
        private string GImage;
        private string description;
        private string windows;
        private string space;
        private string gpu;

        public UserControl1()
        {
            InitializeComponent();
            this.Click += UserControl1_Click;
            labelName.Click += UserControl1_Click;
            labelPrice.Click += UserControl1_Click;
            pictureBoxPhoto.Click += UserControl1_Click;
            guna2GradientPanel1.Click += UserControl1_Click;
        }

        public void SetProduct(string name, decimal price, string imagePath)
        {
            gameName = name;
            Price = price;
            GImage = imagePath;

            labelName.Text = name;
            labelPrice.Text = price.ToString("C");

            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
            {
                pictureBoxPhoto.Image = Image.FromFile(imagePath);
            }
            else
            {
                pictureBoxPhoto.Image = null;
            }
        }

        public void SetProduct(int gameId, string name, decimal price, string imagePath, string description, string windows, string space, string gpu)
        {
            this.gameId = gameId;
            this.gameName = name;
            this.Price = price;
            this.GImage = imagePath;
            this.description = description;
            this.windows = windows;
            this.space = space;
            this.gpu = gpu;

            labelName.Text = name;
            labelPrice.Text = price.ToString("C");

            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
            {
                pictureBoxPhoto.Image = Image.FromFile(imagePath);
            }
            else
            {
                pictureBoxPhoto.Image = null;
            }
        }

        private void UserControl1_Click(object sender, EventArgs e)
        {
            string userEmail = null;
            var parentForm = this.FindForm() as BrowseGame;
            if (parentForm != null)
                userEmail = parentForm.GetLoggedInEmail();

            GameDetails details = new GameDetails(gameId, gameName, Price, GImage, description, windows, space, gpu, userEmail);
            details.Show();
        }
    }
}
