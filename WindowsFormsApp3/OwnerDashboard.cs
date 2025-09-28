using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp3
{
    public partial class OwnerDashboard : Form
    {
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
        private string loggedInEmail;

        public OwnerDashboard(string email)
        {
            InitializeComponent();
            loggedInEmail = email;
            this.Load += AdminDashboard_Load;
            btnLogOut.Click += btnLogOut_Click;
            btnAllGames.Click += btnAllGames_Click;
            btnAddGames.Click += btnAddGames_Click;
            btnBrowse.Click += btnBrowse_Click;
            btnInsert.Click += btnInsert_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnDeleteReviews.Click += btnDeleteReviews_Click;
            btnSearchRev.Click += btnSearchRev_Click;
            btnSearchGames.Click += btnSearchGames_Click;
            guna2DataGridViewAllGames.CellDoubleClick += guna2DataGridViewAllGames_CellDoubleClick;
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            LoadGamesToGrid();
            UpdateSalesLabels();
            LoadDailyEarningsBarChart();
            LoadDailyOrdersLineChart();
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

        private void OpenFormAndCloseCurrent(Form newForm)
        {
            newForm.Show();
            this.Close();
        }

        private void ShowOnlyPanel(params Panel[] panelsToShow)
        {
            // List all your main panels here
            Panel[] allPanels = { panelDataView, panelAddGames, guna2Panel2, PanelOrders, panelSales };
            foreach (var panel in allPanels)
                panel.Visible = false;
            foreach (var panel in panelsToShow)
                panel.Visible = true;
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new LogIN());
        }

        private void btnAllGames_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelDataView);
            LoadGamesToGrid();
        }

        private void btnAddGames_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelAddGames);
            ClearAddGamesForm();
        }


        private void LoadGamesToGrid()
        {
            string query = @"SELECT GameID, Name, Price, ImagePath, Type, Description, Windows, Space, GPU, Quantity, OwnerEmail, CreatedAt 
                             FROM Games
                             WHERE OwnerEmail = @OwnerEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridViewAllGames.DataSource = dt;
            }
        }

        private void LoadOrderDetailsToGrid()
        {
            string query = @"
        SELECT od.OrderDetailID, od.OrderID, od.GameID, od.Quantity, od.Price
        FROM OrderDetails od
        INNER JOIN Games g ON od.GameID = g.GameID
        WHERE g.OwnerEmail = @OwnerEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridView1.DataSource = dt;
            }
        }


        private void ClearAddGamesForm()
        {
            txtGameID.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            txtType.Text = "";
            txtDescription.Text = "";
            txtWindows.Text = "";
            txtSpace.Text = "";
            txtGPU.Text = "";
            txtQuantity.Text = "";
            txtEmail.Text = "";
            dateTimePicker.Text = "";
            pictureBox.Image = null;
            pictureBox.ImageLocation = null;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox.Image = Image.FromFile(ofd.FileName);
                    pictureBox.ImageLocation = ofd.FileName;
                }
            }
        }


        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtType.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("⚠️ Please fill all required fields (Name, Price, Type, OwnerEmail).");
                return;
            }

            try
            {
                int price = int.TryParse(txtPrice.Text, out var tempPrice) ? tempPrice : 0;
                int quantity = int.TryParse(txtQuantity.Text, out var tempQty) ? tempQty : 0;

                string query = @"INSERT INTO Games 
                        (Name, Price, ImagePath, Type, Description, Windows, Space, GPU, Quantity, OwnerEmail, CreatedAt)
                        VALUES (@Name, @Price, @ImagePath, @Type, @Description, @Windows, @Space, @GPU, @Quantity, @OwnerEmail, @CreatedAt)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(pictureBox.ImageLocation) ? (object)DBNull.Value : pictureBox.ImageLocation);
                    cmd.Parameters.AddWithValue("@Type", txtType.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Windows", txtWindows.Text.Trim());
                    cmd.Parameters.AddWithValue("@Space", txtSpace.Text.Trim());
                    cmd.Parameters.AddWithValue("@GPU", txtGPU.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@OwnerEmail", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Game inserted successfully!");
                ClearAddGamesForm();
                LoadGamesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameID.Text))
            {
                MessageBox.Show("⚠️ Select a game first.");
                return;
            }

            try
            {
                if (!int.TryParse(txtGameID.Text, out int gameId))
                {
                    MessageBox.Show("⚠️ Invalid Game ID.");
                    return;
                }

                int price = int.TryParse(txtPrice.Text, out var tempPrice) ? tempPrice : 0;
                int quantity = int.TryParse(txtQuantity.Text, out var tempQty) ? tempQty : 0;

                string query = @"
        UPDATE Games 
        SET 
            Name = @Name,
            Price = @Price,
            ImagePath = @ImagePath,
            Type = @Type,
            Description = @Description,
            Windows = @Windows,
            Space = @Space,
            GPU = @GPU,
            Quantity = @Quantity,
            OwnerEmail = @OwnerEmail,
            CreatedAt = @CreatedAt
        WHERE GameID = @GameID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(pictureBox.ImageLocation) ? (object)DBNull.Value : pictureBox.ImageLocation);
                    cmd.Parameters.AddWithValue("@Type", txtType.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Windows", txtWindows.Text.Trim());
                    cmd.Parameters.AddWithValue("@Space", txtSpace.Text.Trim());
                    cmd.Parameters.AddWithValue("@GPU", txtGPU.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@OwnerEmail", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@CreatedAt", string.IsNullOrWhiteSpace(dateTimePicker.Text) ? DateTime.Now : DateTime.Parse(dateTimePicker.Text));

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("✅ Game updated successfully!");
                    else
                        MessageBox.Show("⚠️ No rows updated. Check GameID.");
                }

                LoadGamesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameID.Text))
            {
                MessageBox.Show("⚠️ Select a game first.");
                return;
            }

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this game?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM Games WHERE GameID=@GameID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", int.Parse(txtGameID.Text));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Game deleted successfully!");
                ClearAddGamesForm();
                LoadGamesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        private void guna2DataGridViewAllGames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = guna2DataGridViewAllGames.Rows[e.RowIndex];

            panelAddGames.Visible = true;
            panelAddGames.BringToFront();
            panelDataView.Visible = false;

            txtGameID.Text = row.Cells[0].Value?.ToString();
            txtName.Text = row.Cells[1].Value?.ToString();
            txtPrice.Text = row.Cells[2].Value?.ToString();
            txtType.Text = row.Cells[4].Value?.ToString();
            txtDescription.Text = row.Cells[5].Value?.ToString();
            txtWindows.Text = row.Cells[6].Value?.ToString();
            txtSpace.Text = row.Cells[7].Value?.ToString();
            txtGPU.Text = row.Cells[8].Value?.ToString();
            txtQuantity.Text = row.Cells[9].Value?.ToString();
            txtEmail.Text = row.Cells[10].Value?.ToString();
            dateTimePicker.Text = row.Cells[11].Value?.ToString();

            string imagePath = row.Cells[3].Value?.ToString();
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.ImageLocation = imagePath;
            }
            else
            {
                pictureBox.Image = null;
                pictureBox.ImageLocation = null;
            }
        }

        private void OwnerDashboard_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowsegame_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new BrowseGame(loggedInEmail));
        }

        private void btnBrowseGame_Click_1(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new BrowseGame(loggedInEmail));
        }

        private void OwnerDashboard_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'projectFinalDataSet.Games' table. You can move, or remove it, as needed.
            this.gamesTableAdapter1.Fill(this.projectFinalDataSet.Games);
            // TODO: This line of code loads data into the 'projectFinalDataSet.Reviews' table. You can move, or remove it, as needed.
            this.reviewsTableAdapter1.Fill(this.projectFinalDataSet.Reviews);
            // TODO: This line of code loads data into the 'projectFinalDataSet.OrderDetails' table. You can move, or remove it, as needed.
            this.orderDetailsTableAdapter1.Fill(this.projectFinalDataSet.OrderDetails);
            LoadOrderDetailsToGrid();

        }

        private void btnReviews_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(guna2Panel2);
            LoadReviewsToGrid();
        }

        private void LoadReviewsToGrid()
        {
            string query = @"
        SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
        FROM Reviews r
        INNER JOIN Games g ON r.GameID = g.GameID
        WHERE g.OwnerEmail = @OwnerEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ReviewGrid.DataSource = dt;
                }
            }
        }

        private void btnDeleteReviews_Click(object sender, EventArgs e)
        {
            if (ReviewGrid.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a review to delete.");
                return;
            }

            int rowIndex = ReviewGrid.SelectedCells[0].RowIndex;
            var reviewIdObj = ReviewGrid.Rows[rowIndex].Cells[0].Value;
            if (reviewIdObj == null)
            {
                MessageBox.Show("Invalid selection.");
                return;
            }

            int reviewId = Convert.ToInt32(reviewIdObj);

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this review?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            string query = "DELETE FROM Reviews WHERE ReviewID = @ReviewID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ReviewID", reviewId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Review deleted successfully!");
            LoadReviewsToGrid();
        }

        private void btnSearchRev_Click(object sender, EventArgs e)
        {
            string searchValue = txtSearchRev.Text.Trim();
            string selectedField = CmBxSrch.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(searchValue) || string.IsNullOrWhiteSpace(selectedField))
            {
                MessageBox.Show("Please select a search field and enter a value.");
                return;
            }

            string query = "";
            SqlCommand cmd;

            switch (selectedField)
            {
                case "GameID":
                    query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      WHERE r.GameID = @val AND g.OwnerEmail = @OwnerEmail";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", searchValue);
                    break;
                case "GameName":
                    query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      WHERE g.Name LIKE @val AND g.OwnerEmail = @OwnerEmail";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
                    break;
                case "UserName":
                    query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      INNER JOIN Users u ON r.UserEmail = u.Email
                      WHERE u.Name LIKE @val AND g.OwnerEmail = @OwnerEmail";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
                    break;
                case "RatingValue":
                    query = @"SELECT g.GameID, g.Name, g.Price, g.ImagePath, g.Type, g.Description, g.Windows, g.Space, g.GPU, g.Quantity, g.OwnerEmail, g.CreatedAt, r.RatingValue
              FROM Games g
              INNER JOIN Reviews r ON g.GameID = r.GameID
              WHERE r.RatingValue = @val";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", searchValue);
                    break;
                default:
                    MessageBox.Show("Invalid search field.");
                    return;
            }

            cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ReviewGrid.DataSource = dt;
                }
            }
        }

        private void btnGameDelete_Click(object sender, EventArgs e)
        {
            if (guna2DataGridViewAllGames.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a game to delete.", "Delete Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var gameIdObj = guna2DataGridViewAllGames.SelectedRows[0].Cells["GameID"].Value;
            if (gameIdObj == null)
            {
                MessageBox.Show("Invalid selection.", "Delete Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int gameId = Convert.ToInt32(gameIdObj);
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE FROM Games WHERE GameID = @GameID", conn))
            {
                cmd.Parameters.AddWithValue("@GameID", gameId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Game deleted successfully!", "Delete Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadGamesToGrid();
        }

        private void btnSearchGames_Click(object sender, EventArgs e)
        {
            string selectedField = guna2ComboBox1.SelectedItem?.ToString();
            string searchValue = txtSearchGames.Text.Trim();

            if (string.IsNullOrWhiteSpace(selectedField) || string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Please select a search field and enter a value.", "Search Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "";
            SqlCommand cmd;

            if (selectedField == "GameID")
            {
                query = "SELECT * FROM Games WHERE GameID = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "GameName")
            {
                query = "SELECT * FROM Games WHERE Name LIKE @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            }
            else if (selectedField == "Type")
            {
                query = "SELECT * FROM Games WHERE Type LIKE @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            }
            else if (selectedField == "RatingValue")
            {
                query = "SELECT * FROM Games WHERE RatingValue = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "Windows")
            {
                query = "SELECT * FROM Games WHERE Windows LIKE @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            }
            else if (selectedField == "Space")
            {
                query = "SELECT * FROM Games WHERE Space LIKE @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            }
            else if (selectedField == "GPU")
            {
                query = "SELECT * FROM Games WHERE GPU LIKE @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            }
            else
            {
                MessageBox.Show("Invalid search field.", "Search Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    adapter.Fill(dt);
                }
                guna2DataGridViewAllGames.DataSource = dt;
            }
        }
        private void btnSearchOrder_Click(object sender, EventArgs e)
        {
            string selectedField = guna2ComboBox2.SelectedItem?.ToString();
            string searchValue = txtSearchOrder.Text.Trim();

            if (string.IsNullOrWhiteSpace(selectedField) || string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Please select a search field and enter a value.", "Search Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "";
            SqlCommand cmd;

            if (selectedField == "OrderDetailID")
            {
                query = "SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails WHERE OrderDetailID = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "OrderID")
            {
                query = "SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails WHERE OrderID = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "GameID")
            {
                query = "SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails WHERE GameID = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "Quantity")
            {
                query = "SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails WHERE Quantity = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else if (selectedField == "Price")
            {
                query = "SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails WHERE Price = @val";
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@val", searchValue);
            }
            else
            {
                MessageBox.Show("Invalid search field.", "Search Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    adapter.Fill(dt);
                }
                guna2DataGridView1.DataSource = dt;
            }
        }

        private void btnPanelOrders_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(PanelOrders);
            LoadOrderDetailsToGrid();
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelSales);
        }

        private void UpdateSalesLabels()
        {
            string queryToday = @"SELECT ISNULL(SUM(NetAmount), 0) FROM OwnerEarnings WHERE OwnerEmail = @OwnerEmail AND CAST(EarningDate AS DATE) = CAST(GETDATE() AS DATE)";
            string queryMonth = @"SELECT ISNULL(SUM(NetAmount), 0) FROM OwnerEarnings WHERE OwnerEmail = @OwnerEmail AND YEAR(EarningDate) = YEAR(GETDATE()) AND MONTH(EarningDate) = MONTH(GETDATE())";
            string queryYear = @"SELECT ISNULL(SUM(NetAmount), 0) FROM OwnerEarnings WHERE OwnerEmail = @OwnerEmail AND YEAR(EarningDate) = YEAR(GETDATE())";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Today's Sale
                using (SqlCommand cmd = new SqlCommand(queryToday, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                    decimal todaySale = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelTodaysSale.Text = $"Today's Sale: ${todaySale:0.00}";
                }

                // Monthly Sale
                using (SqlCommand cmd = new SqlCommand(queryMonth, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                    decimal monthSale = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelMonthlySale.Text = $"Monthly Sale: ${monthSale:0.00}";
                }

                // Yearly Sale
                using (SqlCommand cmd = new SqlCommand(queryYear, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                    decimal yearSale = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelYearlySale.Text = $"Yearly Sale: ${yearSale:0.00}";
                }
            }
        }

        private void LoadDailyEarningsBarChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.ChartAreas.Add(new ChartArea("MainArea"));

            var series = new Series("Daily Earnings")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            // Query: Daily net earnings for the last 7 days for the owner
            string query = @"
        SELECT CAST(EarningDate AS DATE) AS SaleDate, ISNULL(SUM(NetAmount),0) AS Total
        FROM OwnerEarnings
        WHERE OwnerEmail = @OwnerEmail AND EarningDate >= DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
        GROUP BY CAST(EarningDate AS DATE)
        ORDER BY SaleDate";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime saleDate = reader.GetDateTime(0);
                        decimal total = reader.GetDecimal(1);
                        series.Points.AddXY(saleDate.ToString("MMM dd"), total);
                    }
                }
            }

            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisX.Title = "Date";
            chart1.ChartAreas[0].AxisY.Title = "Net Earnings";
            chart1.Titles.Clear();
            chart1.Titles.Add("Daily Net Earnings (Last 7 Days)");
        }

        private void LoadDailyOrdersLineChart()
        {
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();
            chart2.ChartAreas.Add(new ChartArea("MainArea"));

            var series = new Series("Orders Per Day")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.MediumVioletRed,
                IsValueShownAsLabel = true
            };

            // Query: Number of orders per day for the last 7 days for this owner
            string query = @"
        SELECT CAST(o.OrderDate AS DATE) AS OrderDay, COUNT(DISTINCT o.OrderID) AS OrderCount
        FROM Orders o
        INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
        INNER JOIN Games g ON od.GameID = g.GameID
        WHERE g.OwnerEmail = @OwnerEmail AND o.OrderDate >= DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
        GROUP BY CAST(o.OrderDate AS DATE)
        ORDER BY OrderDay";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerEmail", loggedInEmail);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime orderDay = reader.GetDateTime(0);
                        int orderCount = reader.GetInt32(1);
                        series.Points.AddXY(orderDay.ToString("MMM dd"), orderCount);
                    }
                }
            }

            chart2.Series.Add(series);
            chart2.ChartAreas[0].AxisX.Title = "Date";
            chart2.ChartAreas[0].AxisY.Title = "Number of Orders";
            chart2.Titles.Clear();
            chart2.Titles.Add("Orders Per Day (Last 7 Days)");
        }
    }
}