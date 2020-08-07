using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fresh
{
    public partial class NoLogoutDetected : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        UserActivity ua;
        bool allowClose = false;

        public NoLogoutDetected(UserActivity ua)
        {
            InitializeComponent();
            this.ua = ua;
        }

        // function form load
        private void NoLogoutDetected_Load(object sender, EventArgs e)
        {
            label1.Text = $"No logout detected for your last login on {ua.Login.ToString("dd/MM/yyyy")} at {ua.Login.ToString(@"HH\:mm")}";
        }

        // function to update data
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text.Trim() == "")
                {
                    MessageBox.Show("Please input the reason");
                    return;
                }

                if (richTextBox1.Text.Length > 255)
                {
                    MessageBox.Show("Maximum reason length is 255");
                    return;
                }

                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    MessageBox.Show("Please choose the crash type");
                    return;
                }

                var q = db.UserActivities.Where(x => x.ID == ua.ID).FirstOrDefault();
                q.Reason = richTextBox1.Text;
                q.CrashType = radioButton1.Checked ? radioButton1.Text : radioButton2.Text;
                db.SubmitChanges();
                allowClose = true;
                this.Close();
                formLogin.DoLogin(currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // function to save user activity data
        private void NoLogoutDetected_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                MessageBox.Show("You cannot close this form");
                e.Cancel = true;
            }
        }
    }
}
