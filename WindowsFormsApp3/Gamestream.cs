using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Gamestream : Form
    {
        public Gamestream(string loggedInEmail)
        {
            InitializeComponent();
        }

        private void Gamestream_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            var ttt = new TicTacToe();
            ttt.Show();
            this.Close();
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            var ng = new NumberGuessingGame();
            ng.Show();
            this.Close();
        }

        private void btnShowGamePanel_Click(object sender, EventArgs e)
        {
        }

        private void btnShowSettingsPanel_Click(object sender, EventArgs e)
        {
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton2_Click_1(object sender, EventArgs e)
        {
            var ng = new NumberGuessingGame();
            ng.ShowDialog(this);
        }
    }
}
