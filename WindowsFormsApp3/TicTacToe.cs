using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public class TicTacToe : Form
    {
        private Button[,] buttons = new Button[3, 3];
        private bool isXTurn = true;
        private int moves = 0;

        public TicTacToe()
        {
            this.Text = "Tic Tac Toe";
            this.ClientSize = new Size(320, 340);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            int size = 100;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var btn = new Button
                    {
                        Font = new Font("Segoe UI", 24, FontStyle.Bold),
                        Width = size,
                        Height = size,
                        Left = 10 + j * (size + 2),
                        Top = 10 + i * (size + 2),
                        BackColor = Color.WhiteSmoke,
                        Tag = new Point(i, j)
                    };
                    btn.Click += Button_Click;
                    buttons[i, j] = btn;
                    this.Controls.Add(btn);
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn.Text != "") return;
            btn.Text = isXTurn ? "X" : "O";
            btn.ForeColor = isXTurn ? Color.FromArgb(60, 120, 200) : Color.OrangeRed;
            moves++;
            if (CheckWin())
            {
                MessageBox.Show($"Player {(isXTurn ? "X" : "O")} wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetGame();
                return;
            }
            if (moves == 9)
            {
                MessageBox.Show("It's a draw!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetGame();
                return;
            }
            isXTurn = !isXTurn;
        }

        private bool CheckWin()
        {
            string[,] board = new string[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board[i, j] = buttons[i, j].Text;
            // Rows, columns, diagonals
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != "" && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2]) return true;
                if (board[0, i] != "" && board[0, i] == board[1, i] && board[1, i] == board[2, i]) return true;
            }
            if (board[0, 0] != "" && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2]) return true;
            if (board[0, 2] != "" && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0]) return true;
            return false;
        }

        private void ResetGame()
        {
            foreach (var btn in buttons)
                btn.Text = "";
            isXTurn = true;
            moves = 0;
        }
    }
}
