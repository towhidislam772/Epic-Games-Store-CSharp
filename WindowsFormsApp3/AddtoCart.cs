using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class AddtoCart : Form
    {
        public string UserEmail { get; private set; }
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=Faw;Integrated Security=True";

        public AddtoCart(string userEmail)
        {
            InitializeComponent();
            UserEmail = userEmail;
            LoadCartItems();
        }

        // Load all cart items for the current user
        private void LoadCartItems()
        {
            flowLayoutPanel1.Controls.Clear();
            decimal totalPrice = 0;

            string query = @"
                    SELECT ci.CartItemID, g.GameID, g.Name, g.Price, g.ImagePath, ci.Quantity
                    FROM CartItems ci
                    INNER JOIN Cart c ON ci.CartID = c.CartID
                    INNER JOIN Games g ON ci.GameID = g.GameID
                    WHERE c.UserEmail = @UserEmail";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cartItemControl = new UserControl3();
                        cartItemControl.GameTitle = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        cartItemControl.GamePrice = "$" + price.ToString("0.00");
                        cartItemControl.CartItemID = Convert.ToInt32(reader["CartItemID"]);

                        string imagePath = reader["ImagePath"].ToString();
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            cartItemControl.GameImage = Image.FromFile(imagePath);
                        else
                            cartItemControl.GameImage = null;

                        // Optionally show quantity
                        // cartItemControl.ToolTipText = $"Quantity: {quantity}";

                        // Remove from UI and update total price when deleted
                        cartItemControl.RemoveFromCartClicked += (s, e) =>
                        {
                            flowLayoutPanel1.Controls.Remove(cartItemControl);
                            totalPrice -= price * quantity;
                            UpdateTotalPriceLabel(totalPrice);
                        };

                        flowLayoutPanel1.Controls.Add(cartItemControl);
                        totalPrice += price * quantity;
                    }
                }
            }
            UpdateTotalPriceLabel(totalPrice);
        }

        private void UpdateTotalPriceLabel(decimal totalPrice)
        {
            lebelTotalPrice.Text = "Total: $" + totalPrice.ToString("0.00");
        }

        // Refresh the cart display
        public void RefreshCart()
        {
            LoadCartItems();
        }

        // Optional: Search games in cart by name
        private void SearchCartGames(string searchTerm)
        {
            flowLayoutPanel1.Controls.Clear();
            decimal totalPrice = 0;

            string query = @"
                    SELECT ci.CartItemID, g.GameID, g.Name, g.Price, g.ImagePath, ci.Quantity
                    FROM CartItems ci
                    INNER JOIN Cart c ON ci.CartID = c.CartID
                    INNER JOIN Games g ON ci.GameID = g.GameID
                    WHERE c.UserEmail = @UserEmail AND g.Name LIKE @SearchTerm";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cartItemControl = new UserControl3();
                        cartItemControl.GameTitle = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        cartItemControl.GamePrice = "$" + price.ToString("0.00");
                        cartItemControl.CartItemID = Convert.ToInt32(reader["CartItemID"]);

                        string imagePath = reader["ImagePath"].ToString();
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            cartItemControl.GameImage = Image.FromFile(imagePath);
                        else
                            cartItemControl.GameImage = null;

                        // cartItemControl.ToolTipText = $"Quantity: {quantity}";

                        cartItemControl.RemoveFromCartClicked += (s, e) =>
                        {
                            flowLayoutPanel1.Controls.Remove(cartItemControl);
                            totalPrice -= price * quantity;
                            UpdateTotalPriceLabel(totalPrice);
                        };

                        flowLayoutPanel1.Controls.Add(cartItemControl);
                        totalPrice += price * quantity;
                    }
                }
            }
            UpdateTotalPriceLabel(totalPrice);
        }

        private void btnBuyNow_Click(object sender, EventArgs e)
        {
            // Gather all games in the cart to pass to Payment frame
            var gamesToBuy = new List<CartGameInfo>();
            decimal totalPrice = 0;

            string query = @"
        SELECT g.GameID, g.Name, g.Price, ci.Quantity, g.OwnerEmail
        FROM CartItems ci
        INNER JOIN Cart c ON ci.CartID = c.CartID
        INNER JOIN Games g ON ci.GameID = g.GameID
        WHERE c.UserEmail = @UserEmail";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int gameId = Convert.ToInt32(reader["GameID"]);
                        string gameName = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        string ownerEmail = reader["OwnerEmail"].ToString();
                        totalPrice += price * quantity;

                        gamesToBuy.Add(new CartGameInfo
                        {
                            GameID = gameId,
                            GameName = gameName,
                            Price = price,
                            Quantity = quantity,
                            OwnerEmail = ownerEmail // Set OwnerEmail
                        });
                    }
                }
            }

            // Pass the cart games and user email to Payment frame
            Payment paymentForm = new Payment(gamesToBuy, totalPrice.ToString("0.00"), UserEmail);
            paymentForm.Show();
            this.Hide();
        }

        private void AddtoCart_Load(object sender, EventArgs e)
        {

        }

        private void btnEpicGames_Click(object sender, EventArgs e)
        {
            // Show the BrowseGame frame and pass the logged-in email
            BrowseGame browseGameForm = new BrowseGame(UserEmail);
            browseGameForm.Show();
            this.Close();
        }

        // Helper class for cart game info
        public class CartGameInfo
        {
            public int GameID { get; set; }
            public string GameName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string OwnerEmail { get; set; } // Add this property
        }
    }
}