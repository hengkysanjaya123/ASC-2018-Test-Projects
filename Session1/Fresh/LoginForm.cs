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
    public partial class LoginForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        int second = 10;
        int nFault = 0;

        public LoginForm()
        {
            InitializeComponent();
        }

        // function form load
        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        // function to exit
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // function to login
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Validation(container))
            {
                MessageBox.Show("All data must be filled");
                return;
            }

            db = new DataClasses1DataContext();

            var q = db.Users.Where(x => x.Email == textBox1.Text && x.Password == Hash(textBox2.Text)).FirstOrDefault();
            if (q == null)
            {
                if (nFault == 3)
                {
                    MessageBox.Show("You have entered more than 3 times incorrect account, please wait 10 seconds");
                    button1.Enabled = false;
                    label3.Text = $"{second} seconds left";
                    timer1.Start();
                    return;
                }

                MessageBox.Show("Email and password incorrect");
                nFault += 1;
                return;
            }

            if (!q.Active.Value)
            {
                MessageBox.Show("Sorry, your account has been disabled by adminstrator");
                return;
            }

            currentUser = q;
            formLogin = this;

            UserActivity ua = new UserActivity()
            {
                UserID = currentUser.ID,
                Login = DateTime.Now
            };
            currentUserActivity = ua;
            db.UserActivities.InsertOnSubmit(ua);
            db.SubmitChanges();

            var check = db.UserActivities.Where(x => x.UserID == currentUser.ID
                            && x.ID != currentUserActivity.ID
                        ).OrderByDescending(x => x.Login).FirstOrDefault();
            if (check != null)
            {
                if (!check.Logout.HasValue)
                {
                    // No Logout detected
                    NoLogoutDetected form = new NoLogoutDetected(check);
                    this.Hide();
                    form.Show();
                }
                else
                {
                    DoLogin(currentUser);
                }
            }
            else
            {
                DoLogin(currentUser);
            }
        }

        // function to DoLogin based on user role
        public void DoLogin(User u)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            label3.Text = "";
            second = 10;
            nFault = 0;

            if (u.RoleID == 1)
            {
                AdminMenu form = new AdminMenu();
                this.Hide();
                form.Show();
            }
            else if (u.RoleID == 2)
            {
                UserMenu form = new UserMenu();
                this.Hide();
                form.Show();
            }
        }

        // function to countdown timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            second--;
            label3.Text = $"{second} seconds left";
            if (second <= 0)
            {
                label3.Text = "";
                second = 10;
                nFault = 0;
                timer1.Stop();
                button1.Enabled = true;
            }
        }
    }
}
