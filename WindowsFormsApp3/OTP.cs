using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class OTP : Form
    {
        private string userEmail;
        private string userRole;
        private string generatedOtp;
        private DateTime otpExpiry;
        private Timer otpTimer;

        public OTP(string email, string role)
        {
            InitializeComponent();
            userEmail = email;
            userRole = role;
            this.guna2Button1.Click += guna2Button1_Click;
            this.guna2Button2.Click += guna2Button2_Click;
            this.picboxBack.Click += picboxBack_Click;
            SendOtp();
        }

        private void SendOtp()
        {
            var rand = new Random();
            generatedOtp = rand.Next(100000, 999999).ToString();
            otpExpiry = DateTime.Now.AddMinutes(2);

            try
            {
                // Gmail SMTP with App Password
                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("towhidislam3624@gmail.com", "swto gvzr lqcx zgvr\r\n"), // Use App Password here
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                var mail = new MailMessage("towhidislam3624@gmail.com", userEmail, "Your OTP Code", $"Your OTP is: {generatedOtp}");
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send OTP: " + ex.Message);
            }

            if (otpTimer != null)
                otpTimer.Stop();

            otpTimer = new Timer();
            otpTimer.Interval = 1000;
            otpTimer.Tick += OtpTimer_Tick;
            otpTimer.Start();
        }

        private void OtpTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now > otpExpiry)
            {
                generatedOtp = null;
                otpTimer.Stop();
                MessageBox.Show("OTP expired. Please request a new one.");
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (generatedOtp == null || DateTime.Now > otpExpiry)
            {
                MessageBox.Show("OTP expired. Please request a new one.");
                return;
            }
            if (guna2TextBox1.Text.Trim() == generatedOtp)
            {
                otpTimer?.Stop();
                if (userRole == "Owner")
                {
                    new OwnerDashboard(userEmail).Show();
                    this.Close();
                }
                else if (userRole == "Customer")
                {
                    new CustomerDashboard(userEmail).Show();
                    this.Close();
                }
                else
                {
                    new AdminDashboard(userEmail).Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid OTP.");
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            SendOtp();
            MessageBox.Show("OTP resent to your email.");
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void picboxBack_Click(object sender, EventArgs e)
        {
            LogIN loginForm = new LogIN();
            loginForm.Show();
            this.Close();
        }
    }
}
