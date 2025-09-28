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
    public partial class UserControl3 : UserControl
    {
        public UserControl3()
        {
            InitializeComponent();
        }
        // Properties to access the game data
        private int _gameId;
        public int GameID
        {
            get { return _gameId; }
            set { _gameId = value; }
        }

        public string GameTitle
        {
            get { return label4.Text; }
            set { label4.Text = value; }
        }

        public string GamePrice
        {
            get { return label5.Text; }
            set { label5.Text = value; }
        }

        public Image GameImage
        {
            get { return guna2PictureBox1.Image; }
            set
            {
                guna2PictureBox1.Image = value;
                guna2PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        // Add click event to the entire control for selection
        public event EventHandler GameControlClicked;

        protected virtual void OnGameControlClicked(EventArgs e)
        {
            GameControlClicked?.Invoke(this, e);
        }

        // Make the control clickable
        private void UserControl3_Click(object sender, EventArgs e)
        {
            OnGameControlClicked(EventArgs.Empty);
        }

        // make individual controls trigger the same event
        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            OnGameControlClicked(EventArgs.Empty);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            OnGameControlClicked(EventArgs.Empty);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            OnGameControlClicked(EventArgs.Empty);
        }

        public event EventHandler RemoveFromCartClicked;

        public int CartItemID { get; set; }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Remove from database
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand("DELETE FROM CartItems WHERE CartItemID = @CartItemID", conn))
            {
                cmd.Parameters.AddWithValue("@CartItemID", CartItemID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            RemoveFromCartClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}