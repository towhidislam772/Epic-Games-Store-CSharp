using System;
using System.Drawing;
using System.Windows.Forms;
namespace WindowsFormsApp3
{
    public partial class UserControl4 : UserControl
    {
        public int CouponID { get; set; }
        public string CouponCode
        {
            get { return lblCode.Text; }
            set { lblCode.Text = value; }
        }
        public string DiscountPercent
        {
            get { return lblDiscountPercent.Text; }
            set { lblDiscountPercent.Text = value; }
        }


        public UserControl4()
        {
            InitializeComponent();
        }

        public void SetCoupon(int couponId, string code, string discount, string expiry, bool isActive)
        {
            CouponID = couponId;
            CouponCode = code;
            DiscountPercent = discount;
        }

        private void UserControl4_Load(object sender, EventArgs e)
        {
        }
    }
}
