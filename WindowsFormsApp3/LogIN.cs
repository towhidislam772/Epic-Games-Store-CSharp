using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;



namespace WindowsFormsApp3
{
    public partial class LogIN : Form
    {
        string connectionString = @"Data Source=TOWHID\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
        public LogIN()
        {
            InitializeComponent();
            guna2ComboBox.Items.Add("Admin");
            guna2ComboBox.Items.Add("Owner");
            guna2ComboBox.Items.Add("Customer");
            guna2CheckBox.Checked = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnScreenOff_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnScreenMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string selectedRole = guna2ComboBox.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!guna2CheckBox.Checked)
            {
                MessageBox.Show("You must agree to the Terms and Conditions to sign in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string email = txtUserID.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter Email and Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Role 
                                 FROM Users 
                                 WHERE Email=@Email AND Password=@Password AND Role=@Role";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", selectedRole);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string role = reader["Role"].ToString();

                            // ✅ Role-based redirection
                            if (role == "Admin")
                            {
                                AdminDashboard adminForm = new AdminDashboard(email);
                                adminForm.Show();
                                this.Hide();
                            }
                            else if (role == "Customer")
                            {
                                CustomerDashboard customerForm = new CustomerDashboard(email);
                                customerForm.Show();
                                this.Hide();
                            }
                            else if (role == "Owner")
                            {
                                OwnerDashboard ownerForm = new OwnerDashboard(email);
                                ownerForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                // Default fallback (in case role mismatch)
                                BrowseGame browseGameForm = new BrowseGame(email);
                                browseGameForm.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid credentials or role mismatch.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SignUp registrationForm = new SignUp(); 
            registrationForm.Show();              
            this.Hide();
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            // Pass empty string for guest (no email)
            BrowseGame guestForm = new BrowseGame(string.Empty);
            guestForm.FormClosed += (s, args) => this.Show(); // Show login again when BrowseGame closes
            guestForm.Show();
            this.Hide();
        }

        private void btnForgetPassword_Click(object sender, EventArgs e)
        {
            ForgetPassword forgetPasswordForm = new ForgetPassword();
            forgetPasswordForm.Show();
            this.Hide();
        }

        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            UpdatePassword forgetPasswordForm = new UpdatePassword();
            forgetPasswordForm.Show();
            this.Hide();
        }
    }
}
