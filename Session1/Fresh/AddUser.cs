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
    public partial class AddUser : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public AddUser()
        {
            InitializeComponent();
        }

        // function form load
        private void AddUser_Load(object sender, EventArgs e)
        {
            var q = db.Offices.ToList();
            comboBox1.DisplayMember = "Title";
            comboBox1.ValueMember = "ID";
            comboBox1.DataSource = q;
        }

        // function to cancel 
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        // function to add user data
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation(container))
                {
                    MessageBox.Show("All data must be filled");
                    return;
                }

                if (textBox1.Text.Length > 150)
                {
                    MessageBox.Show("Sorry, Email length cannot more than 150");
                    return;
                }

                if (!IsValidEmail(textBox1.Text))
                {
                    MessageBox.Show("Please input email with correct format");
                    return;
                }

                if (textBox2.Text.Length > 50)
                {
                    MessageBox.Show("Sorry, Firstname length cannot more than 50");
                    return;
                }

                if (textBox3.Text.Length > 50)
                {
                    MessageBox.Show("Sorry, Lastname length cannot more than 50");
                    return;
                }

                if (dateTimePicker1.Value.Date > DateTime.Now.Date)
                {
                    MessageBox.Show("Birthdate cannot more than today");
                    return;
                }
                
                var check = db.Users.Where(x => x.Email == textBox1.Text).Count();
                if (check > 0)
                {
                    MessageBox.Show("Email already exist");
                    return;
                }


                int newId = 1;
                var q = db.Users.OrderByDescending(x => x.ID).FirstOrDefault();
                if (q != null)
                {
                    newId = q.ID + 1;
                }

                User u = new User()
                {
                    ID = newId,
                    RoleID = 2,
                    Email = textBox1.Text,
                    Password = Hash(textBox4.Text),
                    FirstName = textBox2.Text,
                    LastName = textBox3.Text,
                    OfficeID = int.Parse(comboBox1.SelectedValue.ToString()),
                    Birthdate = dateTimePicker1.Value.Date,
                    Active = true
                };
                db.Users.InsertOnSubmit(u);
                db.SubmitChanges();
                MessageBox.Show("Data Saved");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
