using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class CustomerDashboard : Form
    {
        private string connectionString = "Data Source=TOWHID\\SQLEXPRESS;Initial Catalog=ProjectFinal;Integrated Security=True;";
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
            CmBxQstn.Items.Add("Your pet Name");
            CmBxQstn.Items.Add("Your Nick Name");
            CmBxQstn.Items.Add("Name of your Favourite Person");
            InitializeChatbotUI();
            AddChatbotButton();
            LoadGamesToGrid();
            btnInfo.Click += btnInfo_Click;
            //btnBrowseUserImage.Click += btnBrowseUserImage_Click;
            //guna2Button3.Click += guna2Button3_Click;
        }

        private void CustomerDashboard_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'projectFinalDataSet.Reviews' table. You can move, or remove it, as needed.
            this.reviewsTableAdapter1.Fill(this.projectFinalDataSet.Reviews);
            // TODO: This line of code loads data into the 'projectFinalDataSet.OrderDetails' table. You can move, or remove it, as needed.
            this.orderDetailsTableAdapter1.Fill(this.projectFinalDataSet.OrderDetails);
            // TODO: This line of code loads data into the 'projectFinalDataSet.Games' table. You can move, or remove it, as needed.
            this.gamesTableAdapter1.Fill(this.projectFinalDataSet.Games);
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
            var loginForm = new LogIN();
            loginForm.Show();
            this.Close();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelOrderView);
            LoadUserOrdersToGrid();
        }

        private void btnMyGames_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(panelMyGames);
        }

        private void btnReviews_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(pnlReviews);
            LoadUserReviewsToGrid();
        }

        private void btnBrowsegame_Click(object sender, EventArgs e)
        {
            var browseGameForm = new BrowseGame(loggedInEmail);
            browseGameForm.Show();
            this.Close();
        }

        private void LoadGamesToGrid()
        {
            // Show only games purchased by the logged-in user
            string query = @"
                SELECT DISTINCT g.GameID, g.Name, g.Price, g.Type, g.Description, g.Windows, g.Space, g.GPU, g.Quantity, g.OwnerEmail, g.ImagePath
                FROM Games g
                INNER JOIN OrderDetails od ON g.GameID = od.GameID
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                WHERE o.UserEmail = @UserEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                guna2DataGridView1.DataSource = dt;
            }
        }

        private void LoadUserOrdersToGrid()
        {
            // Show only orders done by the logged-in user
            string query = @"
                SELECT od.OrderDetailID, od.OrderID, od.GameID, od.Quantity, od.Price
                FROM OrderDetails od
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                WHERE o.UserEmail = @UserEmail";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@UserEmail", loggedInEmail);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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


        // --- Chatbot UI and Logic ---
        private Panel panelChatbot;
        private TextBox txtChatInput;
        private Button btnSendChat;
        private RichTextBox rtbChatHistory;
        private Label lblChatbotHelp;
        private Point chatbotDragOffset;
        private bool chatbotDragging = false;

        private void InitializeChatbotUI()
        {
            panelChatbot = new Panel
            {
                Width = 340,
                Height = 320,
                Top = this.ClientSize.Height - 340,
                Left = 20,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            lblChatbotHelp = new Label
            {
                Top = 0,
                Left = 0,
                Width = 308,
                Height = 32,
                Text = "\uD83E\uDD16 Chatbot: show all games, price of [game]",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 120, 200),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            Button btnCloseChatbot = new Button
            {
                Text = "✖",
                Width = 32,
                Height = 32,
                Top = 0,
                Left = 308,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCloseChatbot.FlatAppearance.BorderSize = 0;
            btnCloseChatbot.Click += (s, e) => panelChatbot.Visible = false;
            rtbChatHistory = new RichTextBox
            {
                Top = 38,
                Left = 10,
                Width = 320,
                Height = 200,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            txtChatInput = new TextBox
            {
                Top = 250,
                Left = 10,
                Width = 230,
                Height = 32,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            btnSendChat = new Button
            {
                Top = 250,
                Left = 250,
                Width = 80,
                Height = 32,
                Text = "Send",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSendChat.FlatAppearance.BorderSize = 0;
            btnSendChat.Click += BtnSendChat_Click;
            panelChatbot.Controls.Add(lblChatbotHelp);
            panelChatbot.Controls.Add(btnCloseChatbot);
            panelChatbot.Controls.Add(rtbChatHistory);
            panelChatbot.Controls.Add(txtChatInput);
            panelChatbot.Controls.Add(btnSendChat);
            this.Controls.Add(panelChatbot);
            panelChatbot.BringToFront();
            panelChatbot.Visible = false;
            // Make panel movable
            panelChatbot.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { chatbotDragging = true; chatbotDragOffset = e.Location; } };
            panelChatbot.MouseMove += (s, e) => { if (chatbotDragging) { panelChatbot.Left += e.X - chatbotDragOffset.X; panelChatbot.Top += e.Y - chatbotDragOffset.Y; } };
            panelChatbot.MouseUp += (s, e) => { chatbotDragging = false; };
        }

        private void ShowChatbotPanel()
        {
            panelChatbot.Visible = true;
            panelChatbot.BringToFront();
        }

        private void BtnSendChat_Click(object sender, EventArgs e)
        {
            string userMsg = txtChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMsg)) return;
            rtbChatHistory.AppendText($"You: {userMsg}\n");
            string botReply = GetChatbotReply(userMsg);
            rtbChatHistory.AppendText($"Bot: {botReply}\n");
            txtChatInput.Clear();
        }

        private string GetChatbotReply(string input)
        {
            input = input.ToLower();
            if (input.Contains("show all games") || input.Contains("list games"))
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
                        return games.Count > 0 ? string.Join(", ", games.Take(20)) + (games.Count > 20 ? ", ..." : "") : "No games found.";
                    }
                }
            }
            if (input.StartsWith("price of ") || input.Contains("what's the price of "))
            {
                string gameName = input.Replace("price of ", "").Replace("what's the price of ", "").Trim('?', ' ');
                string query = "SELECT Price FROM Games WHERE LOWER(Name) = @Name";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", gameName);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null ? $"The price of {gameName} is ${result}" : $"Game '{gameName}' not found.";
                }
            }
            if (input.Contains("help"))
            {
                return "Commands: show all games, price of [game]";
            }
            return "Sorry, I can only answer: show all games, price of [game].";
        }

        private Button btnChatbot;
        private Point chatbotButtonDragOffset;
        private bool chatbotButtonDragging = false;

        private void AddChatbotButton()
        {
            btnChatbot = new Button
            {
                Text = "💬 Chatbot",
                Width = 100,
                Height = 36,
                Top = 10,
                Left = this.Width - 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnChatbot.FlatAppearance.BorderSize = 0;
            btnChatbot.Click += (s, e) => ShowChatbotPanel();
            // Make button movable
            btnChatbot.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { chatbotButtonDragging = true; chatbotButtonDragOffset = e.Location; } };
            btnChatbot.MouseMove += (s, e) => { if (chatbotButtonDragging) { btnChatbot.Left += e.X - chatbotButtonDragOffset.X; btnChatbot.Top += e.Y - chatbotButtonDragOffset.Y; } };
            btnChatbot.MouseUp += (s, e) => { chatbotButtonDragging = false; };
            this.Controls.Add(btnChatbot);
            btnChatbot.BringToFront();
        }

        // Utility method to hide all panels
        private void HideAllPanels()
        {
            panelOrderView.Visible = false;
            panelMyGames.Visible = false;
            pnlReviews.Visible = false;
            pnlUserInfo.Visible = false;
        }

        // Show only the requested panel
        private void ShowOnlyPanel(Control panel)
        {
            HideAllPanels();
            panel.Visible = true;
            panel.BringToFront();
        }

        // Show user info panel and populate fields
        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowOnlyPanel(pnlUserInfo);
            // Load user info from database as before...
            string query = "SELECT Name, Email, Password, ImagePath, Role, Question, Answer FROM Users WHERE Email = @Email";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Email", loggedInEmail);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtUserName.Text = reader["Name"]?.ToString();
                        tctUserEmail.Text = reader["Email"]?.ToString();
                        txtPassword.Text = reader["Password"]?.ToString();
                        CmBxUserType.SelectedItem = reader["Role"]?.ToString();
                        CmBxQstn.SelectedItem = reader["Question"]?.ToString();
                        txtAnswers.Text = reader["Answer"]?.ToString();

                        string imagePath = reader["ImagePath"]?.ToString();
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
                }
            }
        }

        // Browse for a new user image
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

        // Update user info in the database
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            string query = @"UPDATE Users SET 
                                Name = @Name, 
                                Password = @Password, 
                                ImagePath = @ImagePath, 
                                Role = @Role, 
                                Question = @Question, 
                                Answer = @Answer 
                            WHERE Email = @Email";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Name", txtUserName.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@ImagePath", pictureBox1.ImageLocation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", CmBxUserType.SelectedItem?.ToString() ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Question", CmBxQstn.SelectedItem?.ToString() ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Answer", txtAnswers.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", tctUserEmail.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("User info updated successfully!");
            btnInfo_Click(null, null);
        }
    }
}
