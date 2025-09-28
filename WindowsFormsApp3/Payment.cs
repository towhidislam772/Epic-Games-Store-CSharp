using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Payment : Form
    {
        private List<int> _gameIds;
        private List<string> _gameNames;
        private List<decimal> _gamePrices;
        private string _totalPrice;
        private string _loggedInEmail;
        private Label lblGameName;
        private Label lblGamePrice;
        private decimal _discountPercent = 0;
        private string _appliedCouponCode = null;

        public Payment(
            List<int> gameIds,
            List<string> gameNames,
            List<decimal> gamePrices,
            string totalPrice,
            string loggedInEmail)
        {
            InitializeComponent();
            LoadCouponsToPanel();

            _gameIds = gameIds;
            _gameNames = gameNames;
            _gamePrices = gamePrices;
            _totalPrice = totalPrice;
            _loggedInEmail = loggedInEmail;
            int yOffset = 43;
            for (int i = 0; i < _gameNames.Count; i++)
            {
                var lblName = new Label();
                lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblName.ForeColor = Color.Black;
                lblName.AutoSize = true;
                lblName.Location = new Point(20, yOffset);
                lblName.Text = $"Game: {_gameNames[i]} (ID: {_gameIds[i]})";
                guna2GroupBox1.Controls.Add(lblName);
                yOffset += 40;
            }

            lblGamePrice = new Label();
            lblGamePrice.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblGamePrice.ForeColor = Color.DarkGreen;
            lblGamePrice.AutoSize = true;
            lblGamePrice.Location = new Point(20, yOffset);
            lblGamePrice.Text = "Total Price: $" + _totalPrice;
            guna2GroupBox1.Controls.Add(lblGamePrice);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox2_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox3_TextChanged(object sender, EventArgs e) { }
        private void guna2HScrollBar1_Scroll(object sender, ScrollEventArgs e) { }
        private void payment_Load(object sender, EventArgs e) { }
        private void guna2ControlBox1_Click(object sender, EventArgs e) { this.Close(); }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void guna2Panel2_Paint(object sender, PaintEventArgs e) { }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all payment panels first
            panelBkash.Visible = false;
            panelVisa.Visible = false;
            panelPaypal.Visible = false;

            string selected = guna2ComboBox1.SelectedItem?.ToString();

            switch (selected)
            {
                case "Bkash":
                    panelBkash.Visible = true;
                    panelBkash.BringToFront();
                    break;
                case "Visa card":
                    panelVisa.Visible = true;
                    panelVisa.BringToFront();
                    break;
                case "Paypal":
                    panelPaypal.Visible = true;
                    panelPaypal.BringToFront();
                    break;
            }
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            OpenFormAndCloseCurrent(new AddtoCart(_loggedInEmail));
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            // Validate payment method selection
            if (guna2ComboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.", "Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selected = guna2ComboBox1.SelectedItem.ToString();

            // Validate Bkash panel
            if (selected == "Bkash")
            {
                if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || txtPhoneNumber.Text.Length != 11 || !txtPhoneNumber.Text.All(char.IsDigit) || !txtPhoneNumber.Text.StartsWith("01"))
                {
                    MessageBox.Show("Please enter a valid Bangladeshi 11-digit phone number starting with '01'.", "Bkash Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            // Validate Visa panel
            else if (selected == "Visa card")
            {
                if (string.IsNullOrWhiteSpace(guna2TextBox11.Text) || // Name
                    string.IsNullOrWhiteSpace(guna2TextBox9.Text) || guna2TextBox9.Text.Length != 17 || !guna2TextBox9.Text.All(char.IsDigit) || // Card number
                    string.IsNullOrWhiteSpace(guna2TextBox10.Text) || // Last name
                    string.IsNullOrWhiteSpace(guna2TextBox8.Text) || guna2TextBox8.Text.Length != 3 || !guna2TextBox8.Text.All(char.IsDigit)) // CVV
                {
                    MessageBox.Show("Please fill all Visa card fields correctly:\n- Name\n- 17-digit card number\n- Last name\n- 3-digit CVV.", "Visa Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            // Validate Paypal panel
            else if (selected == "Paypal")
            {
                if (string.IsNullOrWhiteSpace(guna2TextBox1.Text) || // Card Name
                    string.IsNullOrWhiteSpace(guna2TextBox5.Text) || // Card number
                    string.IsNullOrWhiteSpace(guna2TextBox6.Text) || // Expiry date
                    string.IsNullOrWhiteSpace(guna2TextBox7.Text))   // Expiry code
                {
                    MessageBox.Show("Please fill all Paypal fields:\n- Card Name\n- Card number\n- Expiry date\n- Expiry code.", "Paypal Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            SaveOrderAndDetails();
            // Show the Thanks form
            Thanks thanksForm = new Thanks(_loggedInEmail);
            thanksForm.Show();
            this.Close();
        }

        private void SaveOwnerEarningsAndCommission(int gameId, int orderId, decimal price, string ownerEmail, SqlConnection conn)
        {
            decimal commissionAmount;
            if (price < 50m)
                commissionAmount = Math.Round(price * 0.10m, 2); // 10% commission
            else
                commissionAmount = Math.Round(price * 0.15m, 2); ;

            decimal grossAmount = price;
            DateTime earningDate = DateTime.Now;

            // Insert into OwnerEarnings
            string insertOwnerEarnings = @"
                INSERT INTO OwnerEarnings (OwnerEmail, GameID, OrderID, GrossAmount, CommissionAmount, EarningDate)
                VALUES (@OwnerEmail, @GameID, @OrderID, @GrossAmount, @CommissionAmount, @EarningDate)";
            using (SqlCommand cmd = new SqlCommand(insertOwnerEarnings, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerEmail", ownerEmail);
                cmd.Parameters.AddWithValue("@GameID", gameId);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@GrossAmount", grossAmount);
                cmd.Parameters.AddWithValue("@CommissionAmount", commissionAmount);
                cmd.Parameters.AddWithValue("@EarningDate", earningDate);
                cmd.ExecuteNonQuery();
            }

            // Insert into Commission
            string insertCommission = @"
                INSERT INTO Commission (GameID, OrderID, CommissionAmount, CommissionDate)
                VALUES (@GameID, @OrderID, @CommissionAmount, @CommissionDate)";
            using (SqlCommand cmd = new SqlCommand(insertCommission, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", gameId);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@CommissionAmount", commissionAmount);
                cmd.Parameters.AddWithValue("@CommissionDate", earningDate);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveOrderAndDetails()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            int orderId = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert into Orders table
                string insertOrderQuery = @"
                    INSERT INTO Orders (UserEmail, OrderDate, Status, TotalAmount)
                    OUTPUT INSERTED.OrderID
                    VALUES (@UserEmail, @OrderDate, @Status, @TotalAmount)";
                using (SqlCommand cmdOrder = new SqlCommand(insertOrderQuery, conn))
                {
                    cmdOrder.Parameters.AddWithValue("@UserEmail", _loggedInEmail);
                    cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    cmdOrder.Parameters.AddWithValue("@Status", "Paid");
                    cmdOrder.Parameters.AddWithValue("@TotalAmount", decimal.Parse(_totalPrice));
                    orderId = (int)cmdOrder.ExecuteScalar();
                }

                // Insert each game in the order
                for (int i = 0; i < _gameIds.Count; i++)
                {
                    int gameId = _gameIds[i];

                    if (!GameExists(gameId, conn))
                    {
                        MessageBox.Show($"GameID {gameId} does not exist in the Games table. Skipping.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    string ownerEmail = GetOwnerEmailByGameId(gameId, conn);
                    int quantity = GetCartItemQuantity(_loggedInEmail, gameId, conn);

                    string insertDetailQuery = @"
                        INSERT INTO OrderDetails (OrderID, GameID, OwnerEmail, Quantity, Price)
                        VALUES (@OrderID, @GameID, @OwnerEmail, @Quantity, @Price)";
                    using (SqlCommand cmdDetail = new SqlCommand(insertDetailQuery, conn))
                    {
                        cmdDetail.Parameters.AddWithValue("@OrderID", orderId);
                        cmdDetail.Parameters.AddWithValue("@GameID", gameId);
                        cmdDetail.Parameters.AddWithValue("@OwnerEmail", ownerEmail);
                        cmdDetail.Parameters.AddWithValue("@Quantity", quantity);
                        cmdDetail.Parameters.AddWithValue("@Price", _gamePrices[i]);
                        cmdDetail.ExecuteNonQuery();
                    }

                    // Update the game quantity
                    string updateQuantityQuery = @"
    UPDATE Games
    SET Quantity = Quantity - @Qty
    WHERE GameID = @GameID AND Quantity >= @Qty";
                    using (SqlCommand cmdUpdateQty = new SqlCommand(updateQuantityQuery, conn))
                    {
                        cmdUpdateQty.Parameters.AddWithValue("@GameID", gameId);
                        cmdUpdateQty.Parameters.AddWithValue("@Qty", quantity);
                        cmdUpdateQty.ExecuteNonQuery();
                    }

                    // Store earnings and commission for this game
                    SaveOwnerEarningsAndCommission(gameId, orderId, _gamePrices[i], ownerEmail, conn);
                }

                // 3. Optionally, clear the user's cart after purchase
                ClearUserCart(_loggedInEmail, conn);
            }
        }

        // Helper to check if GameID exists
        private bool GameExists(int gameId, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM Games WHERE GameID = @GameID";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", gameId);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Helper to get OwnerEmail by GameID
        private string GetOwnerEmailByGameId(int gameId, SqlConnection conn)
        {
            string query = "SELECT OwnerEmail FROM Games WHERE GameID = @GameID";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GameID", gameId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "";
            }
        }

        // Helper to get quantity from CartItems for this user and game
        private int GetCartItemQuantity(string userEmail, int gameId, SqlConnection conn)
        {
            string query = @"
                SELECT ci.Quantity
                FROM CartItems ci
                INNER JOIN Cart c ON ci.CartID = c.CartID
                WHERE c.UserEmail = @UserEmail AND ci.GameID = @GameID";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                cmd.Parameters.AddWithValue("@GameID", gameId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 1;
            }
        }

        // Helper to clear user's cart after purchase
        private void ClearUserCart(string userEmail, SqlConnection conn)
        {
            // Get CartID
            string getCartIdQuery = "SELECT CartID FROM Cart WHERE UserEmail = @UserEmail";
            int cartId = -1;
            using (SqlCommand cmd = new SqlCommand(getCartIdQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    cartId = Convert.ToInt32(result);
            }

            if (cartId > 0)
            {
                // Delete CartItems
                string deleteItemsQuery = "DELETE FROM CartItems WHERE CartID = @CartID";
                using (SqlCommand cmd = new SqlCommand(deleteItemsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CartID", cartId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadCouponsToPanel()
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            // Only select coupons that are active and not expired
            string query = "SELECT CouponID, Code, DiscountPercent, ExpiryDate, IsActive FROM Coupons WHERE IsActive = 1 AND ExpiryDate >= GETDATE()";

            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var couponControl = new UserControl4();
                        couponControl.SetCoupon(
                            Convert.ToInt32(reader["CouponID"]),
                            reader["Code"].ToString(),
                            reader["DiscountPercent"].ToString(),
                            Convert.ToDateTime(reader["ExpiryDate"]).ToShortDateString(),
                            Convert.ToBoolean(reader["IsActive"])
                        );
                        flowLayoutPanel1.Controls.Add(couponControl);
                    }
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string couponCode = txtCoupon.Text.Trim();
            if (string.IsNullOrEmpty(couponCode))
            {
                MessageBox.Show("Please enter a coupon code.", "Coupon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
            string query = "SELECT DiscountPercent FROM Coupons WHERE Code = @Code AND IsActive = 1 AND ExpiryDate >= GETDATE()";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Code", couponCode);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    _discountPercent = Convert.ToDecimal(result);
                    _appliedCouponCode = couponCode;

                    // Apply discount to each game price
                    decimal discountedTotal = 0;
                    for (int i = 0; i < _gamePrices.Count; i++)
                    {
                        decimal originalPrice = _gamePrices[i];
                        decimal discountedPrice = originalPrice - (originalPrice * _discountPercent / 100);
                        _gamePrices[i] = discountedPrice;
                        discountedTotal += discountedPrice;
                    }
                    _totalPrice = discountedTotal.ToString("0.00");

                    lblGamePrice.Text = "Total Price: $" + _totalPrice;
                    MessageBox.Show($"Coupon applied! Discount: {_discountPercent}%\nNew Total: ${_totalPrice}", "Coupon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Invalid or expired coupon code.", "Coupon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenFormAndCloseCurrent(Form newForm)
        {
            newForm.Show();
            this.Close();
        }
    }
}
