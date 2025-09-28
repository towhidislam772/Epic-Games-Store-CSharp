using System;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class T_C : Form
    {
        public T_C()
        {
            InitializeComponent();
            btnStart.Click += btnStart_Click;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartUp startUpForm = new StartUp();
            startUpForm.Show();
            this.Close();
        }
    }
}
