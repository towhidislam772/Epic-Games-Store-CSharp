using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class CustomerDashboard : Form
    {
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
        private string loggedInEmail;

        public CustomerDashboard(string email)
        {
            InitializeComponent();
            loggedInEmail = email;

            this.Load += CustomerDashboard_Load;
            btnLogOut.Click += btnLogOut_Click;
            btnOrders.Click += btnOrders_Click;
            btnMyGames.Click += btnMyGames_Click;
            btnReviews.Click += btnReviews_Click;
            btnBrowsegame.Click += btnBrowsegame_Click;
        }

        private void CustomerDashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            LoadGamesToGrid();
        }

        private void LoadUserInfo()
        {
            if (string.IsNullOrWhiteSpace(loggedInEmail))
                return;

            string query = "SELECT Name, ImagePath FROM Users WHERE Email = @Email";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = loggedInEmail;

                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblUserName.Text = reader["Name"] != DBNull.Value
                                               ? reader["Name"].ToString()
                                               : "User";

                            string dbImagePath = reader["ImagePath"] != DBNull.Value
                                                 ? reader["ImagePath"].ToString()
                                                 : null;

                            if (!string.IsNullOrWhiteSpace(dbImagePath))
                            {
                                string absolutePath = dbImagePath;
                                if (!Path.IsPathRooted(absolutePath))
                                    absolutePath = Path.Combine(Application.StartupPath, dbImagePath);

                                if (File.Exists(absolutePath))
                                {
                                    using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
                                    using (Image img = Image.FromStream(fs))
                                    {
                                        PicBoxUser.Image = new Bitmap(img);
                                    }
                                    PicBoxUser.SizeMode = PictureBoxSizeMode.StretchImage;
                                }
                                else
                                {
                                    PicBoxUser.Image = null;
                                }
                            }
                            else
                            {
                                PicBoxUser.Image = null;
                            }
                        }
                        else
                        {
                            lblUserName.Text = "Unknown user";
                            PicBoxUser.Image = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading user info: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            LogIN loginForm = new LogIN();
            loginForm.Show();
            this.Close();
        }

        // Panel Navigation
        private void btnOrders_Click(object sender, EventArgs e)
        {
            panelOrderView.Visible = true;
            panelOrderView.BringToFront();
            panelMyGames.Visible = false;
            pnlReviews.Visible = false;
            LoadUserOrdersToGrid(); // Load only user's orders
        }

        private void btnMyGames_Click(object sender, EventArgs e)
        {
            panelMyGames.Visible = true;
            panelMyGames.BringToFront();
            panelOrderView.Visible = false;
            pnlReviews.Visible = false;
        }

        private void btnReviews_Click(object sender, EventArgs e)
        {
            pnlReviews.Visible = true;
            pnlReviews.BringToFront();
            panelOrderView.Visible = false;
            panelMyGames.Visible = false;
            LoadUserReviewsToGrid(); // Load only user's reviews
        }

        private void btnBrowsegame_Click(object sender, EventArgs e)
        {
            BrowseGame guestForm = new BrowseGame(loggedInEmail);
            guestForm.FormClosed += (s, args) => this.Show();
            guestForm.Show();
            this.Hide();
        }

        private void LoadGamesToGrid()
        {
            string query = "SELECT GameID, Name, Price, Type, Description, Windows, Space, GPU, Quantity, OwnerName, ImagePath FROM Games";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                //adapter.Fill(dt);
                guna2DataGridViewAllGames.DataSource = dt;
            }
        }

        private void LoadUserOrdersToGrid()
        {
            // Show only orders done by the logged-in user
            string query = @"
                SELECT od.OrderDetailID, od.OrderID, od.GameID, od.Quantity, od.Price
                FROM OrderDetails od
                INNER JOIN [Order] o ON od.OrderID = o.OrderID
                WHERE o.UserEmail = @UserEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                DataTable dt = new DataTable();
                //adapter.Fill(dt);
                guna2DataGridViewAllGames.DataSource = dt;
            }
        }

        private void LoadUserReviewsToGrid()
        {
            // Show only reviews given by the logged-in user
            string query = @"SELECT ReviewID, GameID, UserEmail, RatingValue, ReviewText, ReviewDate
                             FROM Reviews
                             WHERE UserEmail = @UserEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                ReviewGrid.DataSource = dt;
            }
        }

        private void btnSearchRev_Click(object sender, EventArgs e)
        {
            string searchBy = CmBxSrch.SelectedItem?.ToString();
            string searchText = txtSearchRev.Text.Trim();

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please select a search category and enter a value.", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "";
            SqlCommand cmd;

            switch (searchBy)
            {
                case "GameID":
                    query = @"SELECT ReviewID, GameID, UserEmail, RatingValue, ReviewText, ReviewDate
                              FROM Reviews
                              WHERE UserEmail = @UserEmail AND GameID = @GameID";
                    break;
                case "GameName":
                    query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                              FROM Reviews r
                              INNER JOIN Games g ON r.GameID = g.GameID
                              WHERE r.UserEmail = @UserEmail AND g.Name LIKE @GameName";
                    break;
                case "RatingValue":
                    query = @"SELECT ReviewID, GameID, UserEmail, RatingValue, ReviewText, ReviewDate
                              FROM Reviews
                              WHERE UserEmail = @UserEmail AND RatingValue = @RatingValue";
                    break;
                default:
                    MessageBox.Show("Invalid search category.", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserEmail", loggedInEmail);

                if (searchBy == "GameID")
                {
                    if (!int.TryParse(searchText, out int gameId))
                    {
                        MessageBox.Show("GameID must be a number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                }
                else if (searchBy == "GameName")
                {
                    cmd.Parameters.AddWithValue("@GameName", "%" + searchText + "%");
                }
                else if (searchBy == "RatingValue")
                {
                    if (!int.TryParse(searchText, out int rating))
                    {
                        MessageBox.Show("RatingValue must be a number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    cmd.Parameters.AddWithValue("@RatingValue", rating);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ReviewGrid.DataSource = dt;
                }
            }
        }
    }
}
