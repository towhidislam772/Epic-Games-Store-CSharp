using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static WindowsFormsApp3.AddtoCart;

namespace WindowsFormsApp3
{
    public partial class Payment : Form
    {
        private string _gameName;
        private string _gamePrice;
        private string _loggedInEmail;
        private int _gameId; // Add this field
        private List<AddtoCart.CartGameInfo> _cartGames;
        private string _totalPrice;
        private List<AddtoCart.CartGameInfo> gamesToBuy;
        private string v;
        private string userEmail;

        public Payment(string gameName, string gamePrice, string loggedInEmail, int gameId)
        {
            InitializeComponent();
            _gameName = gameName;
            _gamePrice = gamePrice;
            _loggedInEmail = loggedInEmail;
            _gameId = gameId;
            label11.Text = "Total Price: $" + _gamePrice;
        }

        // Constructor for cart purchases
        public Payment(List<CartGameInfo> cartGames, string totalPrice, string loggedInEmail)
        {
            InitializeComponent();
            _cartGames = cartGames;
            _totalPrice = totalPrice;
            _loggedInEmail = loggedInEmail;

            label11.Text = "Total Price: $" + _totalPrice;
            // You can display cart items if needed
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            BrowseGame browseGameForm = new BrowseGame(_loggedInEmail);
            browseGameForm.Show();
            this.Close();
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";
            decimal price = decimal.Parse(_totalPrice);
            int quantity = 1; // Single game purchase
            string paymentMethod = guna2ComboBox1.SelectedItem?.ToString() ?? "Unknown";
            string transactionId = Guid.NewGuid().ToString(); // Or get from payment gateway

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Insert Order
                    int orderId;
                    string orderQuery = @"INSERT INTO Orders (UserEmail, OrderDate, Status, TotalAmount)
                                      OUTPUT INSERTED.OrderID
                                      VALUES (@UserEmail, @OrderDate, @Status, @TotalAmount)";
                    using (SqlCommand cmd = new SqlCommand(orderQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", _loggedInEmail);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", "Paid");
                        cmd.Parameters.AddWithValue("@TotalAmount", price * quantity);
                        orderId = (int)cmd.ExecuteScalar();
                    }

                    // 2. Insert OrderDetails
                    if (_cartGames != null && _cartGames.Count > 0)
                    {
                        foreach (var cartGame in _cartGames)
                        {
                            string orderDetailsQuery = @"INSERT INTO OrderDetails (OrderID, GameID, Quantity, Price)
                                         VALUES (@OrderID, @GameID, @Quantity, @Price)";
                            using (SqlCommand cmd = new SqlCommand(orderDetailsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderID", orderId);
                                cmd.Parameters.AddWithValue("@GameID", cartGame.GameID);
                                cmd.Parameters.AddWithValue("@Quantity", cartGame.Quantity);
                                cmd.Parameters.AddWithValue("@Price", cartGame.Price);
                                cmd.ExecuteNonQuery();
                            }

                            // Calculate commission and net amount for this game
                            decimal commissionRate = 0.10m;
                            decimal grossAmount = cartGame.Price * cartGame.Quantity;
                            decimal commissionAmount = grossAmount * commissionRate;
                            decimal netAmount = grossAmount - commissionAmount;

                            // Insert OwnerEarnings for this game
                            string ownerEarningQuery = @"INSERT INTO OwnerEarnings (OwnerEmail, GameID, OrderID, GrossAmount, CommissionAmount, EarningDate)
                             VALUES (@OwnerEmail, @GameID, @OrderID, @GrossAmount, @CommissionAmount, @EarningDate)";
                            using (SqlCommand cmd = new SqlCommand(ownerEarningQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OwnerEmail", cartGame.OwnerEmail);
                                cmd.Parameters.AddWithValue("@GameID", cartGame.GameID);
                                cmd.Parameters.AddWithValue("@OrderID", orderId);
                                cmd.Parameters.AddWithValue("@GrossAmount", grossAmount);
                                cmd.Parameters.AddWithValue("@CommissionAmount", commissionAmount);
                                cmd.Parameters.AddWithValue("@NetAmount", netAmount);
                                cmd.Parameters.AddWithValue("@EarningDate", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        string orderDetailsQuery = @"INSERT INTO OrderDetails (OrderID, GameID, Quantity, Price)
                                             VALUES (@OrderID, @GameID, @Quantity, @Price)";
                        using (SqlCommand cmd = new SqlCommand(orderDetailsQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@GameID", _gameId);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 3. Update Game Quantity
                    string updateGameQuery = @"UPDATE Games SET Quantity = Quantity - @Quantity WHERE GameID = @GameID";
                    using (SqlCommand cmd = new SqlCommand(updateGameQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@GameID", _gameId);
                        cmd.ExecuteNonQuery();
                    }

                    // 4. Track Owner Earnings
                    string getOwnerQuery = @"SELECT OwnerEmail FROM Games WHERE GameID = @GameID";
                    string ownerEmail;
                    using (SqlCommand cmd = new SqlCommand(getOwnerQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@GameID", _gameId);
                        ownerEmail = cmd.ExecuteScalar()?.ToString();
                    }

                    decimal commissionRateSingle = 0.10m; // 10% commission
                    decimal grossAmountSingle = price * quantity;
                    decimal commissionAmountSingle = grossAmountSingle * commissionRateSingle;
                    decimal netAmountSingle = grossAmountSingle - commissionAmountSingle;

                    string ownerEarningQuerySingle = @"INSERT INTO OwnerEarnings (OwnerEmail, GameID, OrderID, GrossAmount, CommissionAmount, EarningDate)
    VALUES (@OwnerEmail, @GameID, @OrderID, @GrossAmount, @CommissionAmount, @EarningDate)";
                    using (SqlCommand cmd = new SqlCommand(ownerEarningQuerySingle, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OwnerEmail", ownerEmail); // Ensure this is not null
                        cmd.Parameters.AddWithValue("@GameID", _gameId);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@GrossAmount", grossAmountSingle);
                        cmd.Parameters.AddWithValue("@CommissionAmount", commissionAmountSingle);
                        cmd.Parameters.AddWithValue("@EarningDate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }

                    // 5. Track Commission
                    string commissionQuery = @"INSERT INTO Commission (GameID, OrderID, CommissionAmount, CommissionDate)
                                          VALUES (@GameID, @OrderID, @CommissionAmount, @CommissionDate)";
                    using (SqlCommand cmd = new SqlCommand(commissionQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@GameID", _gameId);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@CommissionAmount", commissionAmountSingle);
                        cmd.Parameters.AddWithValue("@CommissionDate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }

                    // 6. Track Payment History
                    string paymentHistoryQuery = @"INSERT INTO PaymentHistory (OrderID, UserEmail, PaymentMethod, TransactionID, PaidAmount, PaidDate, Status)
                                               VALUES (@OrderID, @UserEmail, @PaymentMethod, @TransactionID, @PaidAmount, @PaidDate, @Status)";
                    using (SqlCommand cmd = new SqlCommand(paymentHistoryQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@UserEmail", _loggedInEmail);
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@TransactionID", transactionId);
                        cmd.Parameters.AddWithValue("@PaidAmount", grossAmountSingle);
                        cmd.Parameters.AddWithValue("@PaidDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", "Paid");
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Purchase successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error processing payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Payment_Load(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Thanks thanksForm = new Thanks(_loggedInEmail);
            thanksForm.Show();
            this.Close();
        }
    }
}
