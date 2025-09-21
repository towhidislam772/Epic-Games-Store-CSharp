using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp3
{
    public partial class SignUp : Form
    {
        string connectionString = @"Data Source=TOWHID\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
        private string imagePath = "";       // path selected by user (original)
        private string savedImagePath = "";

        public SignUp()
        {
            InitializeComponent();
            guna2ComboBox.Items.Add("Owner");
            guna2ComboBox.Items.Add("Customer");

            CmBxQstn.Items.Add("Your pet Name");
            CmBxQstn.Items.Add("Your Nick Name");
            CmBxQstn.Items.Add("Name of your Favourite Person");


            guna2HtmlLabel7.Visible = false;
            txtCompanyName.Visible = false;
            guna2CheckBox.Checked = false;

        }

        private void guna2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2ComboBox.SelectedItem.ToString() == "Owner")
            {
                txtCompanyName.Visible = true;
                guna2HtmlLabel7.Visible = true;
            }
            else
            {
                txtCompanyName.Visible = false;
                guna2HtmlLabel7.Visible = false;
            }
        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCompanyName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = guna2ComboBox.SelectedItem?.ToString();
            string companyName = txtCompanyName.Text.Trim();
            string question = CmBxQstn.Text.Trim();
            string answer = guna2TextBox1.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) ||
                string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                MessageBox.Show("Please fill all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!guna2CheckBox.Checked)
            {
                MessageBox.Show("You must agree to the Terms and Conditions to sign up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Step 1: Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Email already exists. Please use another email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Step 2: Insert new record with Question, Answer, ImagePath
                string query = @"INSERT INTO Users 
                                 (Name, Email, [Password], Role, CompanyName, CreatedAt, Question, Answer, ImagePath) 
                                 VALUES (@Name, @Email, @Password, @Role, @CompanyName, GETDATE(), @Question, @Answer, @ImagePath)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", userName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // TODO: hash later
                    cmd.Parameters.AddWithValue("@Role", role);

                    if (role == "GameOwner")
                        cmd.Parameters.AddWithValue("@CompanyName", companyName);
                    else
                        cmd.Parameters.AddWithValue("@CompanyName", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Question", question);
                    cmd.Parameters.AddWithValue("@Answer", answer);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registration Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // After signup → go back to login form
                        LogIN loginForm = new LogIN();
                        loginForm.Show();
                        this.Hide();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            LogIN loginForm = new LogIN(); // create Form1
            loginForm.Show();              // show login form
            this.Hide();
        }

        private void btnScreenMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnScreenOff_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                ofd.Title = "Select a Profile Picture";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imagePath = ofd.FileName; // save the selected file path
                    pictureBox.Image = Image.FromFile(imagePath); // show image
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            LogIN loginForm = new LogIN();
            loginForm.Show();
            this.Hide();
        }
    }
}
