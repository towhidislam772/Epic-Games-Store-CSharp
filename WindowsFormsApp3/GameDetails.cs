using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class GameDetails : Form
    {
        private int _gameId;
        private string _gameName;
        private decimal _price;
        private string _imagePath;
        private string _description;
        private string _windows;
        private string _space;
        private string _gpu;
        private string loggedInEmail; // Set this when opening GameDetails
        private int gameId;           // Set this to the current game's ID

        public GameDetails(
            int gameId,
            string name,
            decimal price,
            string imagePath,
            string description,
            string windows,
            string space,
            string gpu,
            string userEmail)
        {
            InitializeComponent();
            _gameId = gameId;
            _gameName = name;
            _price = price;
            _imagePath = imagePath;
            _description = description;
            _windows = windows;
            _space = space;
            _gpu = gpu;
            loggedInEmail = userEmail;
        }

        private void GameDetails_Load(object sender, EventArgs e)
        {
            // Show all details from constructor
            lblName.Text = _gameName;
            lblPrice.Text = _price.ToString("C");
            lblDetails.Text = _description ?? "";
            lblWindow.Text = _windows ?? "";
            lblSpace.Text = _space ?? "";
            lblGPU.Text = _gpu ?? "";

            if (!string.IsNullOrEmpty(_imagePath) && File.Exists(_imagePath))
            {
                pictureboxGameCover.Image = Image.FromFile(_imagePath);
            }
            else
            {
                pictureboxGameCover.Image = null;
            }
            // Load reviews for this game
            LoadReviews();
            ShowAverageRating();
        }

        private bool UserOwnsGame()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = @"
        SELECT COUNT(*) FROM Orders o
        INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
        WHERE o.UserEmail = @UserEmail AND od.GameID = @GameID";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                cmd.Parameters.AddWithValue("@GameID", _gameId);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private bool GameAlreadyInCart()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            int cartId = -1;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Get cart id
                string selectCartQuery = "SELECT CartID FROM Cart WHERE UserEmail = @UserEmail";
                using (var selectCmd = new SqlCommand(selectCartQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                    var result = selectCmd.ExecuteScalar();
                    if (result == null) return false;
                    cartId = Convert.ToInt32(result);
                }
                // Check if game is already in cart
                string selectItemQuery = "SELECT COUNT(*) FROM CartItems WHERE CartID = @CartID AND GameID = @GameID";
                using (var selectItemCmd = new SqlCommand(selectItemQuery, conn))
                {
                    selectItemCmd.Parameters.AddWithValue("@CartID", cartId);
                    selectItemCmd.Parameters.AddWithValue("@GameID", _gameId);
                    int count = (int)selectItemCmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Add this helper method to your GameDetails class
        private int GetGameQuantity()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = "SELECT Quantity FROM Games WHERE GameID = @GameID";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", _gameId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                return 0;
            }
        }

        private void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (GetGameQuantity() <= 0)
            {
                MessageBox.Show("This game is out of stock and cannot be purchased.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (UserOwnsGame())
            {
                MessageBox.Show("You already own this game.", "Purchase Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Prepare lists for single game purchase
            var gameIds = new System.Collections.Generic.List<int> { _gameId };
            var gameNames = new System.Collections.Generic.List<string> { _gameName };
            var gamePrices = new System.Collections.Generic.List<decimal> { _price };
            string totalPrice = _price.ToString("0.00");

            // Open the payment frame for this game
            Payment paymentForm = new Payment(gameIds, gameNames, gamePrices, totalPrice, loggedInEmail);
            paymentForm.Show();
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string userEmail = txtEmail.Text.Trim();
            string reviewText = txtReview.Text.Trim();
            int ratingValue = (int)guna2RatingStar1.Value;
            DateTime reviewDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(reviewText))
            {
                MessageBox.Show("Please enter your email and review text.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = @"INSERT INTO Reviews (GameID, UserEmail, RatingValue, ReviewText, ReviewDate)
                                 VALUES (@GameID, @UserEmail, @RatingValue, @ReviewText, @ReviewDate)";

            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", _gameId);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                cmd.Parameters.AddWithValue("@RatingValue", ratingValue);
                cmd.Parameters.AddWithValue("@ReviewText", reviewText);
                cmd.Parameters.AddWithValue("@ReviewDate", reviewDate);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtReview.Text = "";
                    guna2RatingStar1.Value = 0;
                    txtEmail.Text = "";
                    // Refresh reviews panel and average rating
                    LoadReviews();
                    ShowAverageRating();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error submitting review: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadReviews()
        {
            flowLayoutPanel1.Controls.Clear();

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = @"
                    SELECT u.Name AS UserName, r.ReviewText, r.RatingValue
                    FROM Reviews r
                    INNER JOIN Users u ON r.UserEmail = u.Email
                    WHERE r.GameID = @GameID";

            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", _gameId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string userName = reader["UserName"].ToString();
                        string reviewText = reader["ReviewText"].ToString();
                        int ratingValue = Convert.ToInt32(reader["RatingValue"]);

                        var reviewControl = new UserControl2();
                        reviewControl.SetReview(userName, reviewText, ratingValue);
                        flowLayoutPanel1.Controls.Add(reviewControl);
                    }
                }
            }
        }

        private void ShowAverageRating()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = @"SELECT AVG(CAST(RatingValue AS FLOAT)) AS AvgRating FROM Reviews WHERE GameID = @GameID";

            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", _gameId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    double avgRating = Convert.ToDouble(result);
                    double rounded = Math.Round(avgRating * 2, MidpointRounding.AwayFromZero) / 2.0;
                    guna2RatingStar2.Value = (float)rounded;
                }
                else
                {
                    guna2RatingStar2.Value = 0;
                }
                guna2RatingStar2.Enabled = false;
            }
        }

        private void userControl21_Load(object sender, EventArgs e)
        {

        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (GetGameQuantity() <= 0)
            {
                MessageBox.Show("This game is out of stock and cannot be added to the cart.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(loggedInEmail))
            {
                MessageBox.Show("User email is not set. Please log in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (UserOwnsGame())
            {
                MessageBox.Show("You already own this game. No need to add to cart.", "Cart Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GameAlreadyInCart())
            {
                MessageBox.Show("This game is already in your cart.", "Cart Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            int cartId = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if cart exists for user
                string selectCartQuery = "SELECT CartID FROM Cart WHERE UserEmail = @UserEmail";
                using (SqlCommand selectCmd = new SqlCommand(selectCartQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                    var result = selectCmd.ExecuteScalar();
                    if (result != null)
                    {
                        cartId = Convert.ToInt32(result);
                    }
                    else
                    {
                        // Create new cart
                        string insertCartQuery = "INSERT INTO Cart (UserEmail) OUTPUT INSERTED.CartID VALUES (@UserEmail)";
                        using (SqlCommand insertCmd = new SqlCommand(insertCartQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                            cartId = (int)insertCmd.ExecuteScalar();
                        }
                    }
                }

                // Insert new item (since already checked not in cart)
                string insertItemQuery = "INSERT INTO CartItems (CartID, GameID, Quantity) VALUES (@CartID, @GameID, @Quantity)";
                using (SqlCommand insertItemCmd = new SqlCommand(insertItemQuery, conn))
                {
                    insertItemCmd.Parameters.AddWithValue("@CartID", cartId);
                    insertItemCmd.Parameters.AddWithValue("@GameID", _gameId);
                    insertItemCmd.Parameters.AddWithValue("@Quantity", 1);
                    insertItemCmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Game added to cart and saved!", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            BrowseGame browseGameForm = new BrowseGame(loggedInEmail);
            browseGameForm.Show();
            this.Close();
        }
    }
}
