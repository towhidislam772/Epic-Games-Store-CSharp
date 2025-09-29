using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp3
{
    public partial class AdminDashboard : Form
    {
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
        private string loggedInEmail;

        public AdminDashboard(string email)
        {
            InitializeComponent();
            loggedInEmail = email;
            this.Load += AdminDashboard_Load;
            btnLogOut.Click += btnLogOut_Click;
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
            guna2DataGridViewAllusers.CellDoubleClick += guna2DataGridViewAllGames_CellDoubleClick;
            DataCoupons.CellDoubleClick += DataCoupons_CellDoubleClick;
            InitializeChatbotUI();
            AddChatbotButton();
            CmBxQstn.Items.Add("Your pet Name");
            CmBxQstn.Items.Add("Your Nick Name");
            CmBxQstn.Items.Add("Name of your Favourite Person");
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            HideAllPanels();
            UpdateAdminSalesLabels();
            InitializeReviewSearchFields();
            LoadDailySalesBarChart();
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

        //method to hide all panels
        private void HideAllPanels()
        {
            panelUser.Visible = false;
            pnlUserInfo.Visible = false;
            pnlCoupons.Visible = false;
            pnlReviews.Visible = false;
            panelDataView.Visible = false;
            PanelOrders.Visible = false;
            panelChatbot.Visible = false;
            panelSales.Visible = false;
            panelAddGames.Visible = false;
        }

        // Utility method to hide all panels except the one you want to show
        private void ShowOnlyPanel(Control panel)
        {
            panelUser.Visible = false;
            pnlUserInfo.Visible = false;
            pnlCoupons.Visible = false;
            pnlReviews.Visible = false;
            panelDataView.Visible = false;
            PanelOrders.Visible = false;
            panelChatbot.Visible = false;
            panelSales.Visible = false;
            panelAddGames.Visible = false;
            if (panel != null)
            {
                panel.Visible = true;
                panel.BringToFront();
            }
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
            ShowOnlyPanel(pnlReviews);
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

        //load all games
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

        //load all user
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

        //load all coupon
        private void LoadCouponsToGrid()
        {
            string query = "SELECT CouponID, Code, DiscountPercent, IsActive, CreatedBy, ExpiryDate FROM Coupons";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt); 
                DataCoupons.DataSource = dt;
            }
        }

        //load reviews
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

        // method to load order details into the grid
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

        //clear form after insert and update
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

        //clear cupons form
        private void ClearCouponsForm()
        {
            txtCouponID.Text = "";
            txtCouponCode.Text = "";
            txtDiscountPercent.Text = "";
            CmBxIsActive.SelectedIndex = -1;
            tctCreatedBy.Text = "";
            DatePickerExpiary.Value = DateTime.Now;
        }

        //Insert Game
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


        //Update Game
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameID1.Text))
            {
                MessageBox.Show("⚠️ Select a game first.");
                return;
            }

            try
            {
                if (!int.TryParse(txtGameID1.Text, out int gameId))
                {
                    MessageBox.Show("⚠️ Invalid Game ID.");
                    return;
                }

                decimal price;
                if (!decimal.TryParse(txtPrice1.Text, out price))
                {
                    MessageBox.Show("⚠️ Please enter a valid price (e.g., 29.99).");
                    return;
                }
                int quantity = int.TryParse(txtQuantity1.Text, out var tempQty) ? tempQty : 0;

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
                    cmd.Parameters.AddWithValue("@Name", txtName1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(pictureBox2.ImageLocation) ? (object)DBNull.Value : pictureBox2.ImageLocation);
                    cmd.Parameters.AddWithValue("@Type", txtType1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Windows", txtWindows1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Space", txtSpace1.Text.Trim());
                    cmd.Parameters.AddWithValue("@GPU", txtGPU1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@OwnerEmail", txtEmail1.Text.Trim());
                    cmd.Parameters.AddWithValue("@CreatedAt", dateTimePicker1.Value);

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

        //Delete Game
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

            HideAllPanels();
            panelAddGames.Visible = true;
            panelAddGames.BringToFront();

            // Fill the fields with the selected game's data
            txtGameID1.Text = row.Cells[0].Value?.ToString();
            txtName1.Text = row.Cells[1].Value?.ToString();       
            txtPrice1.Text = row.Cells[2].Value?.ToString();    
            txtType1.Text = row.Cells[4].Value?.ToString();          
            txtDescription1.Text = row.Cells[5].Value?.ToString();  
            txtWindows1.Text = row.Cells[6].Value?.ToString();    
            txtSpace1.Text = row.Cells[7].Value?.ToString();      
            txtGPU1.Text = row.Cells[8].Value?.ToString();        
            txtQuantity1.Text = row.Cells[9].Value?.ToString();    
            txtEmail1.Text = row.Cells[10].Value?.ToString();       
            dateTimePicker1.Text = row.Cells[11].Value?.ToString();  

            string imagePath = row.Cells[3].Value?.ToString();     
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pictureBox2.Image = Image.FromFile(imagePath);
                pictureBox2.ImageLocation = imagePath;
            }
            else
            {
                pictureBox2.Image = null;
                pictureBox2.ImageLocation = null;
            }
        }

        private void AdminDashboard_Load_1(object sender, EventArgs e)
        {

            this.orderDetailsTableAdapter.Fill(this.projectFinalDataSet.OrderDetails);
            this.couponsTableAdapter1.Fill(this.projectFinalDataSet.Coupons);
            this.usersTableAdapter1.Fill(this.projectFinalDataSet.Users);
            this.reviewsTableAdapter1.Fill(this.projectFinalDataSet.Reviews);
            this.gamesTableAdapter1.Fill(this.projectFinalDataSet.Games);

        }

        private void ReviewGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        //Insert Coupon
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

            int newCouponId = -1;
            string code = txtCouponCode.Text.Trim();
            decimal discount = decimal.TryParse(txtDiscountPercent.Text, out var tempDiscount) ? tempDiscount : 0;

            try
            {
                bool isActive = CmBxIsActive.SelectedItem.ToString().ToLower() == "yes";
                string query = @"INSERT INTO Coupons (Code, DiscountPercent, IsActive, CreatedBy, ExpiryDate)
                                 OUTPUT INSERTED.CouponID
                                 VALUES (@Code, @DiscountPercent, @IsActive, @CreatedBy, @ExpiryDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@DiscountPercent", discount);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", tctCreatedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@ExpiryDate", DatePickerExpiary.Value);

                    conn.Open();
                    newCouponId = (int)cmd.ExecuteScalar();
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

        //Update Coupon
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

        //  Delete Coupon
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

        // DataCoupons Double-Click
        private void DataCoupons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = DataCoupons.Rows[e.RowIndex];

            txtCouponID.Text = row.Cells[0].Value?.ToString(); 
            txtCouponCode.Text = row.Cells[1].Value?.ToString();
            txtDiscountPercent.Text = row.Cells[2].Value?.ToString(); 

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
            tctCreatedBy.Text = row.Cells[4].Value?.ToString();

        }
        private void guna2DataGridViewAllusers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = guna2DataGridViewAllusers.Rows[e.RowIndex];

            HideAllPanels();
            pnlUserInfo.Visible = true;
            pnlUserInfo.BringToFront();

            // Fill user info fields (keep numbers, just change these two)
            CmBxUserType.SelectedItem = row.Cells[4].Value?.ToString();
            txtUserName.Text = row.Cells[1].Value?.ToString();      
            tctUserEmail.Text = row.Cells[2].Value?.ToString();     
            txtPassword.Text = row.Cells[3].Value?.ToString();      
            CmBxQstn.SelectedItem = row.Cells[7].Value?.ToString();  
            txtAnswers.Text = row.Cells[8].Value?.ToString();          
            txtCompanyName.Text = row.Cells[5].Value?.ToString();    

            // Load image
            string imagePath = row.Cells[9].Value?.ToString();     
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.ImageLocation = imagePath;
            }
            else
            {
                pictureBox1.Image = null;
                pictureBox1.ImageLocation = null;
            }
        }

        // Browse user image
        private void btnBrowseUserImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                    pictureBox1.ImageLocation = ofd.FileName;
                }
            }
        }

        // Insert new user
        private void guna2Button2_Click(object sender, EventArgs e)
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


        // Delete user data from the table
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string email = tctUserEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("⚠️ Select a user first.");
                return;
            }

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

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

        // Update user data in the database
        private void guna2Button3_Click(object sender, EventArgs e)
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

        // Delete selected review from the Reviews table
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

        // Populate CmBxSrch with search fields (call in constructor or form load)
        private void InitializeReviewSearchFields()
        {
            CmBxSrch.Items.Clear();
            CmBxSrch.Items.Add("GameID");
            CmBxSrch.Items.Add("GameName");
            CmBxSrch.Items.Add("UserName");
            CmBxSrch.Items.Add("RatingValue");
        }

        // Search reviews based on selected field and value
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
            var browseGameForm = new BrowseGame(loggedInEmail);
            browseGameForm.Show();
            this.Close();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(PanelOrders);
            LoadOrderDetailsToGrid();
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
        private void btnSales_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelSales);
        }

        private void UpdateAdminSalesLabels()
        {
            // Today's commission
            string queryToday = @"SELECT ISNULL(SUM(CommissionAmount), 0) FROM Commission WHERE CAST(CommissionDate AS DATE) = CAST(GETDATE() AS DATE)";
            // Monthly commission
            string queryMonth = @"SELECT ISNULL(SUM(CommissionAmount), 0) FROM Commission WHERE YEAR(CommissionDate) = YEAR(GETDATE()) AND MONTH(CommissionDate) = MONTH(GETDATE())";
            // Yearly commission
            string queryYear = @"SELECT ISNULL(SUM(CommissionAmount), 0) FROM Commission WHERE YEAR(CommissionDate) = YEAR(GETDATE())";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Today's Commission
                using (SqlCommand cmd = new SqlCommand(queryToday, conn))
                {
                    decimal todayCommission = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelTodaysSale.Text = $"Today's Commission: ${todayCommission:0.00}";
                }

                // Monthly Commission
                using (SqlCommand cmd = new SqlCommand(queryMonth, conn))
                {
                    decimal monthCommission = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelMonthlySale.Text = $"Monthly Commission: ${monthCommission:0.00}";
                }

                // Yearly Commission
                using (SqlCommand cmd = new SqlCommand(queryYear, conn))
                {
                    decimal yearCommission = Convert.ToDecimal(cmd.ExecuteScalar());
                    lebelYearlySale.Text = $"Yearly Commission: ${yearCommission:0.00}";
                }
            }
        }

        // --- Chatbot UI and Logic ---

        private Panel panelChatbot;
        private TextBox txtChatInput;
        private Button btnSendChat;
        private ListBox lstChatHistory;
        private Label lblChatbotHelp;
        private Point chatbotDragOffset;
        private bool chatbotDragging = false;

        private void InitializeChatbotUI()
        {
            panelChatbot = new Panel { Width = 260, Height = 260, Top = 40, Left = 700, BorderStyle = BorderStyle.FixedSingle };
            lstChatHistory = new ListBox { Top = 30, Left = 10, Width = 240, Height = 140, HorizontalScrollbar = true, ScrollAlwaysVisible = true };
            txtChatInput = new TextBox { Top = 180, Left = 10, Width = 170, Height = 25 };
            btnSendChat = new Button { Top = 180, Left = 190, Width = 60, Height = 25, Text = "Send" };
            btnSendChat.Click += BtnSendChat_Click;
            lblChatbotHelp = new Label
            {
                Top = 0,
                Left = 0,
                Width = 230,
                Height = 30,
                Text = "Commands: total revenue, user count, list games",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                BackColor = Color.LightYellow,
                TextAlign = ContentAlignment.MiddleLeft
            };
            // Close button
            Button btnCloseChatbot = new Button
            {
                Text = "✖",
                Width = 30,
                Height = 30,
                Top = 0,
                Left = 230,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCloseChatbot.FlatAppearance.BorderSize = 0;
            btnCloseChatbot.Click += (s, e) => panelChatbot.Visible = false;

            panelChatbot.Controls.Add(lblChatbotHelp);
            panelChatbot.Controls.Add(btnCloseChatbot);
            panelChatbot.Controls.Add(lstChatHistory);
            panelChatbot.Controls.Add(txtChatInput);
            panelChatbot.Controls.Add(btnSendChat);
            this.Controls.Add(panelChatbot);
            panelChatbot.BringToFront();
            panelChatbot.Visible = false;
            panelChatbot.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { chatbotDragging = true; chatbotDragOffset = e.Location; } };
            panelChatbot.MouseMove += (s, e) => { if (chatbotDragging) { panelChatbot.Left += e.X - chatbotDragOffset.X; panelChatbot.Top += e.Y - chatbotDragOffset.Y; } };
            panelChatbot.MouseUp += (s, e) => { chatbotDragging = false; };
        }

        private void ShowChatbotPanel()
        {
            HideAllPanels();
            panelChatbot.Visible = true;
        }

        private void BtnSendChat_Click(object sender, EventArgs e)
        {
            string userMsg = txtChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMsg)) return;
            lstChatHistory.Items.Add("You: " + userMsg);
            string botReply = GetChatbotReply(userMsg);
            lstChatHistory.Items.Add("Bot: " + botReply);
            txtChatInput.Clear();
        }

        private string GetChatbotReply(string input)
        {
            input = input.ToLower();
            try
            {
                if (input.Contains("total revenue") || input.Contains("total sales"))
                {
                    // Use Commission table for total revenue
                    string query = "SELECT ISNULL(SUM(CommissionAmount),0) FROM Commission";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        decimal total = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                        return $"Total revenue is ${total:0.00}";
                    }
                }
                if (input.Contains("user count") || input.Contains("how many users"))
                {
                    string query = "SELECT COUNT(*) FROM Users";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        return $"There are {result} users.";
                    }
                }
                if (input.Contains("list games") || input.Contains("all games"))
                {
                    string query = "SELECT Name FROM Games";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var games = new System.Collections.Generic.List<string>();
                            while (reader.Read()) games.Add(reader[0].ToString());
                            return games.Count > 0
                                ? "Games: " + string.Join(", ", games.Take(10)) + (games.Count > 10 ? ", ..." : "")
                                : "No games found.";
                        }
                    }
                }
                if (input.Contains("help"))
                {
                    return "Ask me about total revenue, user count, or list games.";
                }
                return "Sorry, I don't understand. Try asking about total revenue, user count, or list games.";
            }
            catch (Exception)
            {
                return "An error occurred while processing your request. Please check your database connection and try again.";
            }
        }

        // Add a button to show the chatbot
        private Button btnChatbot;
        private void AddChatbotButton()
        {
            btnChatbot = new Button { Text = "Chatbot", Width = 80, Height = 30, Top = 10, Left = 600 };
            btnChatbot.Click += (s, e) => ShowChatbotPanel();
            this.Controls.Add(btnChatbot);
        }

        // Browse image for the game
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = Image.FromFile(ofd.FileName);
                    pictureBox2.ImageLocation = ofd.FileName;
                }
            }
        }

        // Insert new game into the table
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName1.Text) ||
                string.IsNullOrWhiteSpace(txtPrice1.Text) ||
                string.IsNullOrWhiteSpace(txtType1.Text) ||
                string.IsNullOrWhiteSpace(txtEmail1.Text))
            {
                MessageBox.Show("⚠️ Please fill all required fields (Name, Price, Type, OwnerEmail).");
                return;
            }

            try
            {
                int price = int.TryParse(txtPrice1.Text, out var tempPrice) ? tempPrice : 0;
                int quantity = int.TryParse(txtQuantity1.Text, out var tempQty) ? tempQty : 0;

                string query = @"INSERT INTO Games 
            (Name, Price, ImagePath, Type, Description, Windows, Space, GPU, Quantity, OwnerEmail, CreatedAt)
            VALUES (@Name, @Price, @ImagePath, @Type, @Description, @Windows, @Space, @GPU, @Quantity, @OwnerEmail, @CreatedAt)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtName1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(pictureBox2.ImageLocation) ? (object)DBNull.Value : pictureBox2.ImageLocation);
                    cmd.Parameters.AddWithValue("@Type", txtType1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Windows", txtWindows1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Space", txtSpace1.Text.Trim());
                    cmd.Parameters.AddWithValue("@GPU", txtGPU1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@OwnerEmail", txtEmail1.Text.Trim());
                    cmd.Parameters.AddWithValue("@CreatedAt", dateTimePicker1.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Game inserted successfully!");
                LoadGamesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        // Update game data in the table
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameID1.Text))
            {
                MessageBox.Show("⚠️ Select a game first.");
                return;
            }

            try
            {
                if (!int.TryParse(txtGameID1.Text, out int gameId))
                {
                    MessageBox.Show("⚠️ Invalid Game ID.");
                    return;
                }
                decimal price;
                if (!decimal.TryParse(txtPrice1.Text, out price))
                {
                    MessageBox.Show("⚠️ Please enter a valid price (e.g., 29.99).");
                    return;
                }
                int quantity = int.TryParse(txtQuantity1.Text, out var tempQty) ? tempQty : 0;

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
                    cmd.Parameters.AddWithValue("@Name", txtName1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(pictureBox2.ImageLocation) ? (object)DBNull.Value : pictureBox2.ImageLocation);
                    cmd.Parameters.AddWithValue("@Type", txtType1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Windows", txtWindows1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Space", txtSpace1.Text.Trim());
                    cmd.Parameters.AddWithValue("@GPU", txtGPU1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@OwnerEmail", txtEmail1.Text.Trim());
                    cmd.Parameters.AddWithValue("@CreatedAt", dateTimePicker1.Value);

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

        // Delete the selected game
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameID1.Text))
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
                    cmd.Parameters.AddWithValue("@GameID", int.Parse(txtGameID1.Text));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Game deleted successfully!");
                LoadGamesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        private void LoadDailySalesBarChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.ChartAreas.Add(new ChartArea("MainArea"));

            var series = new Series("Daily Sales")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            // Query: Daily sales (sum of CommissionAmount per day for the last 7 days)
            string query = @"
                SELECT CAST(CommissionDate AS DATE) AS SaleDate, ISNULL(SUM(CommissionAmount),0) AS Total
                FROM Commission
                WHERE CommissionDate >= DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
                GROUP BY CAST(CommissionDate AS DATE)
                ORDER BY SaleDate";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
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
            chart1.ChartAreas[0].AxisY.Title = "Sales (Commission)";
            chart1.Titles.Clear();
            chart1.Titles.Add("Daily Sales (Last 7 Days)");
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

            // Query: Number of orders per day for the last 7 days
            string query = @"
                SELECT CAST(OrderDate AS DATE) AS OrderDay, COUNT(*) AS OrderCount
                FROM Orders
                WHERE OrderDate >= DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
                GROUP BY CAST(OrderDate AS DATE)
                ORDER BY OrderDay";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
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

        private void btnSearchUser_Click(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim(); 
            string selectedField = guna2ComboBox1.SelectedItem?.ToString(); 

            if (string.IsNullOrWhiteSpace(searchValue) || string.IsNullOrWhiteSpace(selectedField))
            {
                MessageBox.Show("Please select a search field and enter a value.", "Search User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "";
            SqlCommand cmd;

            switch (selectedField)
            {
                case "Name":
                    query = "SELECT Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer FROM Users WHERE Name LIKE @val";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
                    break;
                case "Email":
                    query = "SELECT Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer FROM Users WHERE Email LIKE @val";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
                    break;
                case "Role":
                    query = "SELECT Name, Email, Role, CompanyName, ImagePath, Password, Question, Answer FROM Users WHERE Role LIKE @val";
                    cmd = new SqlCommand(query);
                    cmd.Parameters.AddWithValue("@val", "%" + searchValue + "%");
                    break;
                default:
                    MessageBox.Show("Invalid search field.", "Search User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                guna2DataGridViewAllusers.DataSource = dt;
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            var loginForm = new LogIN();
            loginForm.Show();
            this.Close();
        }
    }
}
