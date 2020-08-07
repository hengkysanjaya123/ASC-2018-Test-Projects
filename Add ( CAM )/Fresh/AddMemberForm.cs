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
    public partial class AddMemberForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        string memberCode;

        public AddMemberForm()
        {
            InitializeComponent();
        }

        private void AddMemberForm_Load(object sender, EventArgs e)
        {
            memberCode = "M" + "1".PadLeft(5, '0');
            var q = db.tblMembers.OrderByDescending(x => x.MemberCode).FirstOrDefault();
            if (q != null)
            {
                var code = int.Parse(q.MemberCode.Remove(0, 1)) + 1;
                memberCode = $"M" + code.ToString().PadLeft(5, '0');
            }
            textBox1.Text = memberCode;

            var q2 = db.tblGenders.ToList();
            comboBox1.DisplayMember = "GenderTitle";
            comboBox1.ValueMember = "GenderID";
            comboBox1.DataSource = q2;

            var q3 = db.tblMemberTypes.ToList();
            comboBox2.DisplayMember = "MemberTypeTitle";
            comboBox2.ValueMember = "MemberTypeID";
            comboBox2.DataSource = q3;


        }

        private void LoadStaffInformation()
        {
            var q4 = db.tblStaffFunctions.ToList();
            comboBox3.DisplayMember = "StaffFunctionTitle";
            comboBox3.ValueMember = "StaffFunctionID";
            comboBox3.DataSource = q4;

            var q5 = db.tblStaffDepartments.ToList();
            comboBox4.DisplayMember = "StaffDepartmentTitle";
            comboBox4.ValueMember = "StaffDepartmentID";
            comboBox4.DataSource = q5;
        }

        private void LoadStudentInformation()
        {
            var q6 = db.tblStudentMajors.ToList();
            comboBox6.DisplayMember = "StudentMajorTitle";
            comboBox6.ValueMember = "StudentMajorID";
            comboBox6.DataSource = q6;

            var q7 = db.tblDegrees.ToList();
            comboBox5.DisplayMember = "DegreeTitle";
            comboBox5.ValueMember = "DegreeID";
            comboBox5.DataSource = q7;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.DataSource = null;
            comboBox4.DataSource = null;
            comboBox5.DataSource = null;
            comboBox6.DataSource = null;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            if (comboBox2.Text == "Staff")
            {
                LoadStaffInformation();
                groupBox1.Enabled = true;
            }
            else if (comboBox2.Text == "Student")
            {
                LoadStudentInformation();
                groupBox2.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Validation(container))
            {
                MessageBox.Show("All data must be filled");
                return;
            }


            string password = textBox3.Text;
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters");
                return;
            }
            if (password.Length > 50)
            {
                MessageBox.Show("Maximum length of password is 50");
                return;
            }

            if (!password.Any(char.IsUpper))
            {
                MessageBox.Show("Password must be At least 1 uppercase letter");
                return;
            }

            if (!password.Any(char.IsDigit))
            {
                MessageBox.Show("Password must be At least 1 number/digit");
                return;
            }

            List<string> listSymbol = new List<string>()
            {
                "!","@","#","$","%","^"
            };


            var check = password.Where(x => listSymbol.Contains(x.ToString())).Count();
            if (check == 0)
            {
                MessageBox.Show("Password must be At least 1 of the following symbols: !@#$%^");
                return;
            }

            tblMember member = new tblMember()
            {
                MemberCode = memberCode,
                MemberPasswd = password,
                MemberName = textBox2.Text,
                GenderID = int.Parse(comboBox1.SelectedValue.ToString()),
                MemberDoB = dateTimePicker1.Value.Date.ToString("dd/MM/yyyy"),
                MemberTel = textBox4.Text,
                MemberAddress = textBox5.Text,
                IDNumber = textBox6.Text,
                MemberTypeID = int.Parse(comboBox2.SelectedValue.ToString()),
                StaffFunctionID = comboBox2.Text == "Staff" ? (int?)int.Parse(comboBox3.SelectedValue.ToString()) : null,
                StaffDepartmentID = comboBox2.Text == "Staff" ? (int?)int.Parse(comboBox4.SelectedValue.ToString()) : null,
                StudentMajorID = comboBox2.Text == "Student" ? (int?)int.Parse(comboBox6.SelectedValue.ToString()) : null,
                StudentDegreeID = comboBox2.Text == "Student" ? (int?)int.Parse(comboBox5.SelectedValue.ToString()) : null,
                RecordDate = DateTime.Now.ToString("dd-MMMM-yyyy"),
                MemberDisabled = false,
                UserID = 1
            };
            db.tblMembers.InsertOnSubmit(member);
            db.SubmitChanges();
            MessageBox.Show("Data Saved");
            this.DialogResult = DialogResult.OK;
        }
    }
}
