using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class AdminDashboard : Form
    {
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
        private string loggedInEmail;

        public AdminDashboard(string email)
        {
            InitializeComponent();

            loggedInEmail = email;

            // Load user info when form loads
            this.Load += AdminDashboard_Load;

            // hook logout
            btnLogOut.Click += btnLogOut_Click;

            // Button event bindings
            //btnAllGames.Click += btnAllGames_Click;
            //btnAddGames.Click += btnAddGames_Click;
            btnBrowse.Click += btnBrowse_Click;
            btnInsert.Click += btnInsert_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnCoupons.Click += btnCoupons_Click;
            btnInsertCoupon.Click += btnInsertCoupon_Click;
            btnUpdateCoupons.Click += btnUpdateCoupons_Click;
            btnDeleteCoupon.Click += btnDeleteCoupon_Click;
            btnSearchUser.Click += btnSearchUser_Click;
            btnReviews.Click += btnReviews_Click;
            btnManageGames.Click += btnManageGames_Click;
            btnBrowseGame.Click += btnBrowseGame_Click;

            // DataGridView double-click
            guna2DataGridViewAllusers.CellDoubleClick += guna2DataGridViewAllGames_CellDoubleClick;
            DataCoupons.CellDoubleClick += DataCoupons_CellDoubleClick;
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            HideAllPanels();
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

        // ------------------- Panel Navigation -------------------

        // Utility method to hide all panels
private void HideAllPanels()
{
    panelUser.Visible = false;
    panelAddGames.Visible = false;
    pnlUserInfo.Visible = false;
    pnlCoupons.Visible = false;
    pnlReviews.Visible = false;
    panelDataView.Visible = false;
    PanelOrders.Visible = false; // Hide PanelOrders
}

// Show only the requested panel
private void ShowOnlyPanel(Control panel)
{
    HideAllPanels();
    panel.Visible = true;
    panel.BringToFront();
}

private void btnAllGames_Click(object sender, EventArgs e)
{
    ShowOnlyPanel(panelUser);
    LoadGamesToGrid();
}

private void btnAddGames_Click(object sender, EventArgs e)
{
    ShowOnlyPanel(panelAddGames);
    ClearAddGamesForm();
}

private void btnCoupons_Click(object sender, EventArgs e)
{
    ShowOnlyPanel(pnlCoupons);
    LoadCouponsToGrid();
    ClearCouponsForm();
}

        private void btnReviews_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            pnlReviews.Visible = true;
            pnlReviews.BringToFront();
            LoadReviewsToGrid();
        }

private void btnManageGames_Click(object sender, EventArgs e)
{
    ShowOnlyPanel(panelDataView);
    LoadGamesToGrid();
}

private void btnManageUsers_Click(object sender, EventArgs e)
{
    ShowOnlyPanel(panelUser);
    LoadUsersToGrid();
}

// ------------------- Load Data -------------------

        private void LoadGamesToGrid()
        {
            string query = @"SELECT GameID, Name, Price, ImagePath, Type, Description, Windows, Space, GPU, Quantity, OwnerEmail, CreatedAt FROM Games";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridViewAllGames.DataSource = dt;
            }
        }

        private void LoadUsersToGrid()
        {
            string query = "SELECT Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer FROM Users";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridViewAllusers.DataSource = dt;
            }
        }

        private void LoadCouponsToGrid()
        {
            string query = "SELECT CouponID, Code, DiscountPercent, IsActive, CreatedBy, ExpiryDate FROM Coupons";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt); // <-- This line must be active!
                DataCoupons.DataSource = dt;
            }
        }

        private void LoadReviewsToGrid()
        {
            string query = @"
                SELECT ReviewID, GameID, UserEmail, RatingValue, ReviewText, ReviewDate
                FROM Reviews";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                ReviewGrid.DataSource = dt;
            }
        }

        // Utility method to load order details into the grid
private void LoadOrderDetailsToGrid()
{
    string query = @"SELECT OrderDetailID, OrderID, GameID, Quantity, Price FROM OrderDetails";
    using (SqlConnection conn = new SqlConnection(connectionString))
    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
    {
        DataTable dt = new DataTable();
        adapter.Fill(dt);
        guna2DataGridView1.DataSource = dt;
    }
}

        // ------------------- Form Utilities -------------------

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
            dateTimePicker.Value = DateTime.Now;
            pictureBox.Image = null;
            pictureBox.ImageLocation = null;
        }

        private void ClearCouponsForm()
        {
            txtCouponID.Text = "";
            txtCouponCode.Text = "";
            txtDiscountPercent.Text = "";
            CmBxIsActive.SelectedIndex = -1;
            tctCreatedBy.Text = "";
            DatePickerExpiary.Value = DateTime.Now;
        }

        // ------------------- Insert -------------------

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

        // ------------------- Update -------------------

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
                    cmd.Parameters.AddWithValue("@CreatedAt", dateTimePicker.Value);

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

        // ------------------- Delete -------------------

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

        // ------------------- Coupons Panel Navigation -------------------
        //private void btnCoupons_Click(object sender, EventArgs e)
        //{
        //    pnlCoupons.Visible = true;
        //    pnlCoupons.BringToFront();
        //    panelUser.Visible = false;
        //    panelAddGames.Visible = false;
        //    LoadCouponsToGrid();
        //    ClearCouponsForm();
        //}

        // ------------------- DataGridView Double-Click -------------------

        private void guna2DataGridViewAllGames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = guna2DataGridViewAllGames.Rows[e.RowIndex];

            // Show the Add/Edit panel and hide others
            panelAddGames.Visible = true;
            panelAddGames.BringToFront();
            panelDataView.Visible = false;
            panelUser.Visible = false;
            pnlUserInfo.Visible = false;
            pnlCoupons.Visible = false;
            pnlReviews.Visible = false;

            // Fill the fields with the selected game's data
            txtGameID.Text = row.Cells[0].Value?.ToString();         // GameID
            txtName.Text = row.Cells[1].Value?.ToString();           // Name
            txtPrice.Text = row.Cells[2].Value?.ToString();          // Price
            txtType.Text = row.Cells[4].Value?.ToString();           // Type
            txtDescription.Text = row.Cells[5].Value?.ToString();    // Description
            txtWindows.Text = row.Cells[6].Value?.ToString();        // Windows
            txtSpace.Text = row.Cells[7].Value?.ToString();          // Space
            txtGPU.Text = row.Cells[8].Value?.ToString();            // GPU
            txtQuantity.Text = row.Cells[9].Value?.ToString();       // Quantity
            txtEmail.Text = row.Cells[10].Value?.ToString();         // OwnerEmail
            dateTimePicker.Text = row.Cells[11].Value?.ToString();   // CreatedAt

            string imagePath = row.Cells[3].Value?.ToString();       // ImagePath
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

        private void AdminDashboard_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'fawDataSet.Games' table. You can move, or remove it, as needed.
            //this.gamesTableAdapter.Fill(this.fawDataSet.Games);
            // TODO: This line of code loads data into the 'fawDataSet.Coupons' table. You can move, or remove it, as needed.
            //this.couponsTableAdapter.Fill(this.fawDataSet.Coupons);
            // TODO: This line of code loads data into the 'fawDataSet.Users' table. You can move, or remove it, as needed.
            //this.usersTableAdapter.Fill(this.fawDataSet.Users);
            // TODO: This line of code loads data into the 'fawDataSet.Reviews' table. You can move, or remove it, as needed.
            //this.reviewsTableAdapter.Fill(this.fawDataSet.Reviews);

        }

        private void ReviewGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // ------------------- Load Coupons Data -------------------
        //private void LoadCouponsToGrid()
        //{
        //    string query = "SELECT CouponID, Code, DiscountPercent, IsActive, CreatedBy, ExpiryDate FROM Coupons";
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
        //    {
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt); // <-- This line must be active!
        //        DataCoupons.DataSource = dt;
        //    }
        //}

        // ------------------- Clear Coupons Form -------------------
        //private void ClearCouponsForm()
        //{
        //    txtCouponID.Text = "";
        //    txtCouponCode.Text = "";
        //    txtDiscountPercent.Text = "";
        //    CmBxIsActive.SelectedIndex = -1;
        //    tctCreatedBy.Text = "";
        //    DatePickerExpiary.Value = DateTime.Now;
        //}

        // ------------------- Insert Coupon -------------------
        private void btnInsertCoupon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCouponCode.Text) ||
                string.IsNullOrWhiteSpace(txtDiscountPercent.Text) ||
                CmBxIsActive.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(tctCreatedBy.Text))
            {
                MessageBox.Show("⚠️ Please fill all required fields.");
                return;
            }

            try
            {
                int discount = int.TryParse(txtDiscountPercent.Text, out var tempDiscount) ? tempDiscount : 0;
                bool isActive = CmBxIsActive.SelectedItem.ToString().ToLower() == "yes";
                string query = @"INSERT INTO Coupons (Code, DiscountPercent, IsActive, CreatedBy, ExpiryDate)
                                 VALUES (@Code, @DiscountPercent, @IsActive, @CreatedBy, @ExpiryDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", txtCouponCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@DiscountPercent", discount);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", tctCreatedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@ExpiryDate", DatePickerExpiary.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Coupon inserted successfully!");
                ClearCouponsForm();
                LoadCouponsToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        // ------------------- Update Coupon -------------------
        private void btnUpdateCoupons_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCouponID.Text))
            {
                MessageBox.Show("⚠️ Select a coupon first.");
                return;
            }

            try
            {
                int couponId = int.Parse(txtCouponID.Text);
                int discount = int.TryParse(txtDiscountPercent.Text, out var tempDiscount) ? tempDiscount : 0;
                bool isActive = CmBxIsActive.SelectedItem.ToString().ToLower() == "yes";
                string query = @"UPDATE Coupons SET
                                    Code = @Code,
                                    DiscountPercent = @DiscountPercent,
                                    IsActive = @IsActive,
                                    CreatedBy = @CreatedBy,
                                    ExpiryDate = @ExpiryDate
                                 WHERE CouponID = @CouponID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CouponID", couponId);
                    cmd.Parameters.AddWithValue("@Code", txtCouponCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@DiscountPercent", discount);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", tctCreatedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@ExpiryDate", DatePickerExpiary.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("✅ Coupon updated successfully!");
                    else
                        MessageBox.Show("⚠️ No rows updated. Check CouponID.");
                }

                LoadCouponsToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        // ------------------- Delete Coupon -------------------
        private void btnDeleteCoupon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCouponID.Text))
            {
                MessageBox.Show("⚠️ Select a coupon first.");
                return;
            }

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this coupon?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            try
            {
                string query = "DELETE FROM Coupons WHERE CouponID=@CouponID";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CouponID", int.Parse(txtCouponID.Text));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Coupon deleted successfully!");
                ClearCouponsForm();
                LoadCouponsToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        // ------------------- DataCoupons Double-Click -------------------
        private void DataCoupons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = DataCoupons.Rows[e.RowIndex];

            txtCouponID.Text = row.Cells[0].Value?.ToString(); // CouponID
            txtCouponCode.Text = row.Cells[1].Value?.ToString(); // Code
            txtDiscountPercent.Text = row.Cells[2].Value?.ToString(); // DiscountPercent
            
if (row.Cells[3].Value != null && row.Cells[3].Value != DBNull.Value)
{
    bool isActive = false;
    // Handle SQL 'bit' type (usually int: 0 or 1)
    if (row.Cells[3].Value is bool)
        isActive = (bool)row.Cells[3].Value;
    else if (row.Cells[3].Value is int)
        isActive = ((int)row.Cells[3].Value) == 1;
    else
        isActive = row.Cells[3].Value.ToString() == "1" || row.Cells[3].Value.ToString().ToLower() == "true";
    CmBxIsActive.SelectedItem = isActive ? "Yes" : "No";
}
else
{
    CmBxIsActive.SelectedItem = "No";
}
            tctCreatedBy.Text = row.Cells[4].Value?.ToString(); // CreatedBy
            
        }
        private void guna2DataGridViewAllusers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = guna2DataGridViewAllusers.Rows[e.RowIndex];

            panelUser.Visible = false;
            pnlUserInfo.Visible = true;

            // Load image
            string imagePath = row.Cells["ImagePath"].Value?.ToString();
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                pictureBox1.Image = Image.FromFile(imagePath);
            else
                pictureBox1.Image = null;

            CmBxUserType.SelectedItem = row.Cells["Role"].Value?.ToString();
            txtUserName.Text = row.Cells["Name"].Value?.ToString();
            tctUserEmail.Text = row.Cells["Email"].Value?.ToString();
            txtPassword.Text = row.Cells["Password"].Value?.ToString();
            CmBxQstn.SelectedItem = row.Cells["Question"].Value?.ToString();
            txtAnswers.Text = row.Cells["Answer"].Value?.ToString();
            txtCompanyName.Text = row.Cells["Role"].Value?.ToString() == "Owner" ? row.Cells["CompanyName"].Value?.ToString() : "";
        }

        //private void btnManageUsers_Click(object sender, EventArgs e)
        //{
            // Ensure panelUser is brought to front and made visible
            //panelUser.Visible = true;
            //panelUser.BringToFront();

            // Hide other panels
            //pnlUserInfo.Visible = false;
            //panelAddGames.Visible = false;
            //pnlCoupons.Visible = false;

            // Optionally refresh user grid
            //LoadUsersToGrid();
        //}

        private void btnSearchUser_Click(object sender, EventArgs e)
        {
            string category = guna2ComboBox1.SelectedItem?.ToString();
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Select a category and enter search text.");
                return;
            }

            string query = $"SELECT Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer FROM Users WHERE {category} LIKE @search";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + searchText + "%");
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridViewAllusers.DataSource = dt;
            }
        }

        private void btnInsertUser_Click(object sender, EventArgs e)
        {
            string role = CmBxUserType.SelectedItem?.ToString();
            string companyName = role == "Owner" ? txtCompanyName.Text.Trim() : null;
            string query = @"INSERT INTO Users (Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer)
                     VALUES (@Name, @Email, @Role, @CompanyName, @ImagePath, @Password, @Question, @Answer)";
    using (SqlConnection conn = new SqlConnection(connectionString))
    using (SqlCommand cmd = new SqlCommand(query, conn))
    {
        cmd.Parameters.AddWithValue("@Name", txtUserName.Text.Trim());
        cmd.Parameters.AddWithValue("@Email", tctUserEmail.Text.Trim());
        cmd.Parameters.AddWithValue("@Role", role);
        cmd.Parameters.AddWithValue("@CompanyName", (object)companyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ImagePath", pictureBox1.ImageLocation ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
        cmd.Parameters.AddWithValue("@Question", CmBxQstn.SelectedItem?.ToString());
        cmd.Parameters.AddWithValue("@Answer", txtAnswers.Text.Trim());
        conn.Open();
        cmd.ExecuteNonQuery();
    }
    MessageBox.Show("User inserted successfully!");
    LoadUsersToGrid();
}


private void btnUpdateUser_Click(object sender, EventArgs e)
{
    string email = tctUserEmail.Text.Trim();
    string role = CmBxUserType.SelectedItem?.ToString();
    string companyName = role == "Owner" ? txtCompanyName.Text.Trim() : null;
    string query = @"UPDATE Users SET Name=@Name, Role=@Role, CompanyName=@CompanyName, ImagePath=@ImagePath, Password=@Password, Question=@Question, Answer=@Answer WHERE Email=@Email";
    using (SqlConnection conn = new SqlConnection(connectionString))
    using (SqlCommand cmd = new SqlCommand(query, conn))
    {
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Name", txtUserName.Text.Trim());
        cmd.Parameters.AddWithValue("@Role", role);
        cmd.Parameters.AddWithValue("@CompanyName", (object)companyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ImagePath", pictureBox1.ImageLocation ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
        cmd.Parameters.AddWithValue("@Question", CmBxQstn.SelectedItem?.ToString());
        cmd.Parameters.AddWithValue("@Answer", txtAnswers.Text.Trim());
        conn.Open();
        cmd.ExecuteNonQuery();
    }
    MessageBox.Show("User updated successfully!");
    LoadUsersToGrid();
}

private void btnDeleteUser_Click(object sender, EventArgs e)
{
    string email = tctUserEmail.Text.Trim();
    string query = "DELETE FROM Users WHERE Email=@Email";
    using (SqlConnection conn = new SqlConnection(connectionString))
    using (SqlCommand cmd = new SqlCommand(query, conn))
    {
        cmd.Parameters.AddWithValue("@Email", email);
        conn.Open();
        cmd.ExecuteNonQuery();
    }
    MessageBox.Show("User deleted successfully!");
    LoadUsersToGrid();
}

private void btnRemoveUser_Click(object sender, EventArgs e)
{
    if (guna2DataGridViewAllusers.SelectedRows.Count > 0)
    {
        string email = guna2DataGridViewAllusers.SelectedRows[0].Cells["Email"].Value.ToString();
        string query = "DELETE FROM Users WHERE Email=@Email";
        using (SqlConnection conn = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        MessageBox.Show("User removed successfully!");
        LoadUsersToGrid();
    }
    else
    {
        MessageBox.Show("Select a user to remove.");
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
                      WHERE r.GameID = @val";
            cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@val", searchValue);
            break;
        case "GameName":
            query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      WHERE g.Name LIKE @val";
            cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            break;
        case "UserName":
            query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      INNER JOIN Users u ON r.UserEmail = u.Email
                      WHERE u.Name LIKE @val";
            cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
            break;
        case "RatingValue":
            query = @"SELECT r.ReviewID, r.GameID, r.UserEmail, r.RatingValue, r.ReviewText, r.ReviewDate
                      FROM Reviews r
                      INNER JOIN Games g ON r.GameID = g.GameID
                      WHERE r.RatingValue = @val";
            cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@val", searchValue);
            break;
        default:
            MessageBox.Show("Invalid search field.");
            return;
    }

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

        private void btnBrowseGame_Click(object sender, EventArgs e)
        {
            BrowseGame browseGameForm = new BrowseGame(loggedInEmail);
            browseGameForm.Show();
}

    private void btnOrders_Click(object sender, EventArgs e)
    {
        HideAllPanels();
        PanelOrders.Visible = true;
        PanelOrders.BringToFront();
        LoadOrderDetailsToGrid();
    }

    // Search order details by selected field and value
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
        private void btnSales_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            panelSales.Visible = true;
            panelSales.BringToFront();
        }
    }
}
