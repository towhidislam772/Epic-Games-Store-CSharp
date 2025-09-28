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
    public partial class UserControl2 : UserControl
    {
        private string _userEmail;

        public UserControl2()
        {
            InitializeComponent();
        }

        private void UserControl2_Load(object sender, EventArgs e)
        {

        }

        public void SetReview(string userName, string reviewText, float ratingValue)
        {
            lblUserName.Text = userName;
            lblReviews.Text = reviewText;
            guna2RatingStar2.Value = ratingValue;
            guna2RatingStar2.Enabled = false;
        }
    }
}
