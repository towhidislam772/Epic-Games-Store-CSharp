using System;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class StartUp : Form
    {
        public StartUp()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Open Form2 (Admin Dashboard)
            LogIN adminDashboard = new LogIN();
            adminDashboard.Show();
            this.Hide();
        }

        // Social media links (LinkLabel version)
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.twitter.com/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/");
        }

        // Optional: Guna2 Circle PictureBox social media links
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/");
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/");
        }

        private void guna2CirclePictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.twitter.com/");
        }

        // Placeholder events for other controls
        private void Form1_Load(object sender, EventArgs e) { }

        private void pictureBoxLogo_Click(object sender, EventArgs e) { }

        private void guna2PictureBox1_Click(object sender, EventArgs e) { }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }

        private void Form1_Load_1(object sender, EventArgs e) { }

        private void btnGetStarted_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Let's Get Started!", "Epic Game Store Parody");
        }

        private void btnAboutUs_Click(object sender, EventArgs e)
        {
            AboutUs aboutUsForm = new AboutUs();
            aboutUsForm.Show();
            this.Hide();
        }

        private void btnTC_Click(object sender, EventArgs e)
        {
            T_C tncForm = new T_C();
            tncForm.Show();
            this.Hide();
        }
    }
}
