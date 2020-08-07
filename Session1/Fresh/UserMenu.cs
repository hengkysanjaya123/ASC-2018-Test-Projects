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
    public partial class UserMenu : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public UserMenu()
        {
            InitializeComponent();
        }

        // function form load
        private void UserMenu_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = $"Hi {currentUser.FirstName} {currentUser.LastName}, Welcome to AMONIC Airlines Automation System";


                var q = db.UserActivities.ToList().Where(x => x.UserID == currentUser.ID
                                && x.ID != currentUserActivity.ID
                                && x.Login <= DateTime.Now
                                && x.Login >= DateTime.Now.AddDays(-30)
                            ).ToList();

                var timeSpent = q.Where(x => x.Logout.HasValue).Sum(x => (x.Logout.Value - x.Login).TotalSeconds);
                TimeSpan time = TimeSpan.FromSeconds(timeSpent);
                label2.Text = $"Time spent on system : {((int)time.TotalHours).ToString("00")}:{((int)time.Minutes).ToString("00")}:{((int)time.Seconds).ToString("00")}";

                var nCrash = q.Where(x => !x.Logout.HasValue).Count();
                label3.Text = $"Number of crashes : {nCrash}";

                dataGridView1.DataSource = q.OrderByDescending(x => x.Login)
                    .Select(x => new
                    {
                        Date = x.Login.ToString("MM/dd/yyyy"),
                        LoginTime = x.Login.ToString(@"HH\:mm"),
                        LogoutTime = x.Logout.HasValue ? x.Logout.Value.ToString(@"HH\:mm") : "**",
                        TimeSpentOnSystem = x.Logout.HasValue ? $"{((int)(x.Logout.Value - x.Login).TotalHours).ToString("00")}:{((int)(x.Logout.Value - x.Login).Minutes).ToString("00")}:{((int)(x.Logout.Value - x.Login).Seconds).ToString("00")}" : "**",
                        UnsuccessfulLogoutReason = x.Logout.HasValue ? "" : x.Reason
                    }).ToList();

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells["LogoutTime"].Value.ToString() == "**")
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // function to save useractivity data
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var q = db.UserActivities.Where(x => x.ID == currentUserActivity.ID).FirstOrDefault();
                q.Logout = DateTime.Now;
                db.SubmitChanges();
                this.Close();
                formLogin.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
