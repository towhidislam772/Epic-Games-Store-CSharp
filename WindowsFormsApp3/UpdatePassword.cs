using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class UpdatePassword : Form
    {
        public UpdatePassword()
        {
            InitializeComponent();
        }

        private void ForgetPassword_Load(object sender, EventArgs e)
        {
            ShowOnlyPanel(pnlQstn);
            // Add items to combo boxes
            CmBxQstn.Items.Add("Your pet Name");
            CmBxQstn.Items.Add("Your Nick Name");
            CmBxQstn.Items.Add("Name of your Favourite Person");
            CmBxYN.Items.Add("Yes");
            CmBxYN.Items.Add("No");
            // Hide new password controls by default
            lblNew.Visible = false;
            txtNewPassword.Visible = false;
            btnChange.Visible = false;
            // Bind ComboBox event
            CmBxYN.SelectedIndexChanged += CmBxYN_SelectedIndexChanged;
        }

        private void pnlAns_Paint(object sender, PaintEventArgs e)
        {
        }

        private void OpenFormAndCloseCurrent(Form newForm)
        {
            newForm.Show();
            this.Close();
        }

        private void ShowOnlyPanel(params Panel[] panelsToShow)
        {
            // List all your main panels here. Add more if you have them.
            Panel[] allPanels = { pnlQstn, pnlAns };
            foreach (var panel in allPanels)
                panel.Visible = false;
            foreach (var panel in panelsToShow)
                panel.Visible = true;
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new LogIN());
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string question = CmBxQstn.SelectedItem?.ToString();
            string answer = txtAnswer.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection("Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;"))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email AND Question=@Question AND Answer=@Answer";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Question", question);
                    cmd.Parameters.AddWithValue("@Answer", answer);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ShowOnlyPanel(pnlAns);
                        lblPassword.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Invalid Email, Question or Answer. Please try again.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtAnswer.Clear();
                    }
                }
            }
        }

        private void CmBxYN_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmBxYN.SelectedItem != null && CmBxYN.SelectedItem.ToString() == "Yes")
            {
                lblNew.Visible = true;
                txtNewPassword.Visible = true;
                btnChange.Visible = true;
            }
            else
            {
                lblNew.Visible = false;
                txtNewPassword.Visible = false;
                btnChange.Visible = false;
            }
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new LogIN());
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Please enter a new password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection("Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;"))
            {
                con.Open();
                string query = "UPDATE Users SET Password=@Password WHERE Email=@Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Show updated password now
                        lblPassword.Text = "New Password: " + newPassword;

                        txtNewPassword.Clear();
                        lblNew.Visible = false;
                        txtNewPassword.Visible = false;
                        btnChange.Visible = false;
                        CmBxYN.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update password. Please check your email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void picboxBack_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new LogIN());
        }
    }
}
