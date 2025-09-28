using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class AddtoCart : Form
    {
        public string UserEmail { get; private set; }
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";

        public AddtoCart(string userEmail)
        {
            InitializeComponent();
            UserEmail = userEmail;
            LoadCartItems();
        }

        //This will Load all cart items for the current user
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
                        cartItemControl.GameID = Convert.ToInt32(reader["GameID"]);
                        cartItemControl.GameTitle = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        cartItemControl.GamePrice = "$" + price.ToString("0.00");
                        cartItemControl.CartItemID = Convert.ToInt32(reader["CartItemID"]);

                        string imagePath = reader["ImagePath"].ToString();
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            cartItemControl.GameImage = Image.FromFile(imagePath);
                        else
                            cartItemControl.GameImage = null;

                        cartItemControl.RemoveFromCartClicked += (s, e) =>
                        {
                            flowLayoutPanel1.Controls.Remove(cartItemControl);
                            totalPrice -= price;
                            UpdateTotalPriceLabel(totalPrice);
                        };

                        flowLayoutPanel1.Controls.Add(cartItemControl);
                        totalPrice += price;
                    }
                }
            }
            UpdateTotalPriceLabel(totalPrice);
        }

        private void UpdateTotalPriceLabel(decimal totalPrice)
        {
            lebelTotalPrice.Text = "Total: $" + totalPrice.ToString("0.00");
        }

        //This will Refresh the cart display
        public void RefreshCart()
        {
            LoadCartItems();
        }

        //This will handle the Buy Now button click event
        private void btnBuyNow_Click(object sender, EventArgs e)
        {
            // Get the total price from the label 
            string totalText = lebelTotalPrice.Text.Replace("Total: $", "").Trim();
            string totalPrice = totalText;

            // Collect all GameIDs, names, and prices in the cart
            var gameIds = new System.Collections.Generic.List<int>();
            var gameNames = new System.Collections.Generic.List<string>();
            var gamePrices = new System.Collections.Generic.List<decimal>();

            foreach (UserControl3 cartItem in flowLayoutPanel1.Controls)
            {
                gameIds.Add(cartItem.GameID); // <-- Collect GameID from each cart item
                gameNames.Add(cartItem.GameTitle);
                decimal price = 0;
                decimal.TryParse(cartItem.GamePrice.Replace("$", ""), out price);
                gamePrices.Add(price);
            }

            // Open Payment form
            Payment paymentForm = new Payment(gameIds, gameNames, gamePrices, totalPrice, UserEmail);
            paymentForm.Show();
            this.Close();
        }

        private void AddtoCart_Load(object sender, EventArgs e)
        {

        }

        private void btnEpicGames_Click(object sender, EventArgs e)
        {
            BrowseGame browseGameForm = new BrowseGame(UserEmail);
            browseGameForm.Show();
            this.Close();
        }
    }
}