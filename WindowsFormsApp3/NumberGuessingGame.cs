using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public class NumberGuessingGame : Form
    {
        private int targetNumber;
        private int attempts;
        private Label lblTitle;
        private Label lblHint;
        private TextBox txtGuess;
        private Button btnGuess;
        private Button btnRestart;
        private Random random = new Random();

        public NumberGuessingGame()
        {
            this.Text = "Number Guessing Game";
            this.ClientSize = new Size(350, 260);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            InitializeUI();
            StartNewGame();
        }

        private void InitializeUI()
        {
            lblTitle = new Label
            {
                Text = "Guess the Number (1-100)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 120, 200),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 320,
                Height = 40,
                Top = 20,
                Left = 15
            };
            lblHint = new Label
            {
                Text = "Enter your guess:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 320,
                Height = 30,
                Top = 70,
                Left = 15
            };
            txtGuess = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Width = 120,
                Height = 32,
                Top = 110,
                Left = 110,
                TextAlign = HorizontalAlignment.Center,
                BorderStyle = BorderStyle.FixedSingle
            };
            btnGuess = new Button
            {
                Text = "Guess",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Width = 80,
                Height = 32,
                Top = 155,
                Left = 60,
                BackColor = Color.FromArgb(60, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGuess.FlatAppearance.BorderSize = 0;
            btnGuess.Click += BtnGuess_Click;
            btnRestart = new Button
            {
                Text = "Restart",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Width = 80,
                Height = 32,
                Top = 155,
                Left = 200,
                BackColor = Color.OrangeRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRestart.FlatAppearance.BorderSize = 0;
            btnRestart.Click += (s, e) => StartNewGame();
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblHint);
            this.Controls.Add(txtGuess);
            this.Controls.Add(btnGuess);
            this.Controls.Add(btnRestart);
        }

        private void StartNewGame()
        {
            targetNumber = random.Next(1, 101);
            attempts = 0;
            lblHint.Text = "Enter your guess:";
            txtGuess.Text = "";
            txtGuess.Enabled = true;
            btnGuess.Enabled = true;
            txtGuess.Focus();
        }

        private void BtnGuess_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtGuess.Text.Trim(), out int guess) || guess < 1 || guess > 100)
            {
                lblHint.Text = "Please enter a valid number (1-100).";
                lblHint.ForeColor = Color.OrangeRed;
                return;
            }
            attempts++;
            if (guess == targetNumber)
            {
                lblHint.Text = $"Correct! You guessed in {attempts} tries.";
                lblHint.ForeColor = Color.SeaGreen;
                txtGuess.Enabled = false;
                btnGuess.Enabled = false;
                MessageBox.Show($"Congratulations! You guessed the number in {attempts} attempts.", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (guess < targetNumber)
            {
                lblHint.Text = "Too low! Try a higher number.";
                lblHint.ForeColor = Color.FromArgb(60, 120, 200);
            }
            else
            {
                lblHint.Text = "Too high! Try a lower number.";
                lblHint.ForeColor = Color.FromArgb(60, 120, 200);
            }
            txtGuess.SelectAll();
            txtGuess.Focus();
        }



        private void btnOpenOtherGame_Click(object sender, EventArgs e)
        {

        }

        private void btnShowGamePanel_Click(object sender, EventArgs e)
        {
        }

        private void btnShowSettingsPanel_Click(object sender, EventArgs e)
        {
        }
    }
}
