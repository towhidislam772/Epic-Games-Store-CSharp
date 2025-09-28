using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class AboutUs : Form
    {
        public AboutUs()
        {
            InitializeComponent();
            btnStart.Click += btnStart_Click;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //opens StartUp Form
            StartUp startUpForm = new StartUp();
            startUpForm.Show();
            this.Close();
        }
    }
}
