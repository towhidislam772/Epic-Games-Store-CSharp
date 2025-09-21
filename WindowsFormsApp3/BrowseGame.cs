using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class BrowseGame : Form
    {
        private string loggedInEmail;

        public BrowseGame(string email)
        {
            InitializeComponent();
            loggedInEmail = email;
            LoadUserInfo();
            LoadProducts();

            btnAction2.Click += btnAction2_Click;
            btnAdventure2.Click += btnAdventure2_Click;
            btnHorror2.Click += btnHorror2_Click;
            btnSports2.Click += btnSports2_Click;
            btnOthers2.Click += btnOthers2_Click;
            btnAll.Click += btnAll_Click;
            btnCart.Click += btnCart_Click;
            btnLogOut.Click += btnLogOut_Click;
        }

        private void LoadUserInfo()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            string query = "SELECT Name, ImagePath FROM Users WHERE Email = @Email";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Email", loggedInEmail);

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
                                    PuictureBoxUserPicture.Image = Image.FromFile(absolutePath);
                                    PuictureBoxUserPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                                }
                                else
                                {
                                    PuictureBoxUserPicture.Image = null;
                                }
                            }
                            else
                            {
                                PuictureBoxUserPicture.Image = null;
                            }
                        }
                        else
                        {
                            lblUserName.Text = "Unknown user";
                            PuictureBoxUserPicture.Image = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading user info: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadProducts()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            string query = "SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU FROM Games";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }

        public void LoadProductsByType(string type)
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            string query = "SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU FROM Games WHERE Type = @Type";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Type", type);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }

        private void btnAction2_Click(object sender, EventArgs e) => LoadProductsByType("Action");
        private void btnAdventure2_Click(object sender, EventArgs e) => LoadProductsByType("Adventure");
        private void btnHorror2_Click(object sender, EventArgs e) => LoadProductsByType("Horror");
        private void btnSports2_Click(object sender, EventArgs e) => LoadProductsByType("Sports");
        private void btnOthers2_Click(object sender, EventArgs e) => LoadProductsByType("Others");
        private void btnAll_Click(object sender, EventArgs e) => LoadProducts();

        private void lblUserName_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void btnFilter_Click(object sender, EventArgs e) { }

        private void BrowseGame_Load(object sender, EventArgs e)
        {

        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            // Prevent multiple AddtoCart forms from opening for this user
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is AddtoCart addToCart)
                {
                    // If the form is for the same user, just bring it to front
                    if (addToCart.UserEmail == loggedInEmail)
                    {
                        addToCart.BringToFront();
                        return;
                    }
                }
            }

            // Open a new AddtoCart form for the current user
            AddtoCart addToCartForm = new AddtoCart(loggedInEmail);
            addToCartForm.FormClosed += (s, args) => this.Show();
            addToCartForm.Show();
            this.Hide();
        }
        public string GetLoggedInEmail()
        {
            return loggedInEmail;
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            // Show the login form and close the current BrowseGame form
            LogIN loginForm = new LogIN();
            loginForm.Show();
            this.Close();
        }
        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            string searchTerm = guna2TextBox1.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadProducts(); // Show all games if search is empty
            }
            else
            {
                SearchGames(searchTerm);
            }
        }

        // Add this method to perform the search and show results
        private void SearchGames(string searchTerm)
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            string query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                             FROM Games WHERE Name LIKE @SearchTerm";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = guna2ComboBox2.SelectedItem?.ToString();
            string query = "";

            switch (selected)
            {
                case "LowToHigh":
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games ORDER BY Price ASC";
                    break;
                case "HighToLow":
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games ORDER BY Price DESC";
                    break;
                case "Under10":
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games WHERE Price < 10 ORDER BY Price ASC";
                    break;
                case "Under100":
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games WHERE Price < 100 ORDER BY Price ASC";
                    break;
                case "HighestPrice":
                    query = @"SELECT TOP 1 GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games ORDER BY Price DESC";
                    break;
                case "LowestPrice":
                    query = @"SELECT TOP 1 GameID, Name, Price, ImagePath, Description, Windows, Space, GPU 
                              FROM Games ORDER BY Price ASC";
                    break;
                default:
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU FROM Games";
                    break;
            }

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = guna2ComboBox1.SelectedItem?.ToString();
            string query = "";

            switch (selected)
            {
                case "0-1":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                              FROM Games g
                              INNER JOIN (
                                  SELECT GameID, AVG(CAST(RatingValue AS FLOAT)) AS AvgRating
                                  FROM Reviews
                                  GROUP BY GameID
                              ) r ON g.GameID = r.GameID
                              WHERE r.AvgRating >= 0 AND r.AvgRating < 1";
                    break;
                case "1-2":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                              FROM Games g
                              INNER JOIN (
                                  SELECT GameID, AVG(CAST(RatingValue AS FLOAT)) AS AvgRating
                                  FROM Reviews
                                  GROUP BY GameID
                              ) r ON g.GameID = r.GameID
                              WHERE r.AvgRating >= 1 AND r.AvgRating < 2";
                    break;
                case "2-3":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                              FROM Games g
                              INNER JOIN (
                                  SELECT GameID, AVG(CAST(RatingValue AS FLOAT)) AS AvgRating
                                  FROM Reviews
                                  GROUP BY GameID
                              ) r ON g.GameID = r.GameID
                              WHERE r.AvgRating >= 2 AND r.AvgRating < 3";
                    break;
                case "3-4":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                              FROM Games g
                              INNER JOIN (
                                  SELECT GameID, AVG(CAST(RatingValue AS FLOAT)) AS AvgRating
                                  FROM Reviews
                                  GROUP BY GameID
                              ) r ON g.GameID = r.GameID
                              WHERE r.AvgRating >= 3 AND r.AvgRating < 4";
                    break;
                case "4-5":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                              FROM Games g
                              INNER JOIN (
                                  SELECT GameID, AVG(CAST(RatingValue AS FLOAT)) AS AvgRating
                                  FROM Reviews
                                  GROUP BY GameID
                              ) r ON g.GameID = r.GameID
                              WHERE r.AvgRating >= 4 AND r.AvgRating <= 5";
                    break;
                default:
                    query = @"SELECT GameID, Name, Price, ImagePath, Description, Windows, Space, GPU FROM Games";
                    break;
            }

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }

        private void btnTopSold_Click(object sender, EventArgs e)
        {
            // Show top 10 sold games based on total quantity sold from OrderDetails table
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            string query = @"
                SELECT TOP 10 
                    g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU,
                    SUM(od.Quantity) AS TotalSold
                FROM OrderDetails od
                INNER JOIN Games g ON od.GameID = g.GameID
                GROUP BY g.GameID, g.Name, g.Price, g.ImagePath, g.Description, g.Windows, g.Space, g.GPU
                ORDER BY TotalSold DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flowLayoutPanel1.Controls.Clear();
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string imagePath = reader["ImagePath"].ToString();
                        string description = reader["Description"].ToString();
                        string windows = reader["Windows"].ToString();
                        string space = reader["Space"].ToString();
                        string gpu = reader["GPU"].ToString();

                        var productControl = new UserControl1();
                        productControl.SetProduct(gameId, name, price, imagePath, description, windows, space, gpu);

                        flowLayoutPanel1.Controls.Add(productControl);
                    }
                }
            }
        }
    }
}
