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
    public partial class AdminMenu : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        bool formReady = false;

        public AdminMenu()
        {
            InitializeComponent();
        }

        // function form load
        private void AdminMenu_Load(object sender, EventArgs e)
        {
            var q = db.Offices.ToList();
            q.Insert(0, new Office()
            {
                ID = 0,
                Title = "All Offices"
            });
            comboBox1.DisplayMember = "Title";
            comboBox1.ValueMember = "ID";
            comboBox1.DataSource = q;

            formReady = true;
            LoadData();
        }

        // function to user data
        public void LoadData()
        {
            if (!formReady) return;

            db = new DataClasses1DataContext();

            var q = db.Users.ToList().Where(x => x.OfficeID == int.Parse(comboBox1.SelectedValue.ToString())
                            || comboBox1.SelectedValue.ToString() == "0"
                    )
                    .Select(x => new
                    {
                        Name = x.FirstName,
                        Lastname = x.LastName,
                        Age = DateTime.Now.Year - x.Birthdate.Value.Year,
                        UserRole = x.Role.Title,
                        EmailAddress = x.Email,
                        Office = x.Office.Title,
                        obj = x
                    }).ToList();
            dataGridView1.DataSource = q;
            dataGridView1.Columns["obj"].Visible = false;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var u = (User)dataGridView1.Rows[i].Cells["obj"].Value;

                if (!u.Active.Value)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (u.RoleID == 1)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
            }
        }

        // function to load user data based on office id
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        // function to change button caption based on user active status
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var u = (User)dataGridView1.CurrentRow.Cells["obj"].Value;
                button2.Text = u.Active.Value ? "Suspend Account" : "Unsuspend Account";
            }
            catch
            {
                button2.Text = "Suspend / Unsuspend Account";
            }
        }

        // function to suspend / unsuspend account
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please choose user data in the datagridview first");
                    return;
                }

                var u = (User)dataGridView1.CurrentRow.Cells["obj"].Value;
                if (u.ID == currentUser.ID)
                {
                    MessageBox.Show("You cannot suspend your own account");
                    return;
                }

                u.Active = !u.Active;
                db.SubmitChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void container_Paint(object sender, PaintEventArgs e)
        {

        }

        // function to  open change role form
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please choose user data in the datagridview first");
                return;
            }

            var u = (User)dataGridView1.CurrentRow.Cells["obj"].Value;
            if (u.ID == currentUser.ID)
            {
                MessageBox.Show("You cannot change your own role");
                return;
            }

            // Change Role Form
            EditRole form = new EditRole(u);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        // function to open add user form
        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddUser form = new AddUser();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
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
