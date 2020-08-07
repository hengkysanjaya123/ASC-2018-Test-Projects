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
    public partial class BookingConfirmation : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<Schedule> header = new List<Schedule>(), detail = new List<Schedule>();
        CabinType cabinType;
        int nPasenger;

        List<PassengerData> listPassengerData = new List<PassengerData>();

        public BookingConfirmation(List<Schedule> header, List<Schedule> detail, CabinType cabinType, int nPasenger)
        {
            InitializeComponent();
            this.header = header;
            this.detail = detail;
            this.cabinType = cabinType;
            this.nPasenger = nPasenger;
        }

        public bool IsAlphanumeric(string text)
        {
            foreach (var a in text)
            {
                if (!char.IsLetter(a) && !char.IsDigit(a))
                {
                    return false;
                }
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listPassengerData.Count == nPasenger)
            {
                MessageBox.Show("You have entered all passenger data");
                return;
            }

            maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            string phone = maskedTextBox1.Text;

            if (!Validation(groupBox3) || phone == "")
            {
                MessageBox.Show("All data must be filled");
                return;
            }

            if (textBox1.Text.Length > 50 || textBox2.Text.Length > 50)
            {
                MessageBox.Show("Maximal length for Firstname and lastname is 50");
                return;
            }

            if (!IsAlphanumeric(textBox3.Text))
            {
                MessageBox.Show("Passport number should be alphanumeric");
                return;
            }

            if (textBox3.Text.Length < 6 || textBox3.Text.Length > 9)
            {
                MessageBox.Show("Passport number should be 6 - 9 alphanumeric");
                return;
            }

            if (dateTimePicker1.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Birthdate cannot more than today");
                return;
            }

            maskedTextBox1.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            phone = maskedTextBox1.Text;

            if (phone.Contains("_"))
            {
                MessageBox.Show("Phone must be 10 digit");
                return;
            }

            var q = listPassengerData.Where(x => x.PassportNumber == textBox3.Text).Count();
            if (q > 0)
            {
                MessageBox.Show("Passport number already exist");
                return;
            }

            listPassengerData.Add(new PassengerData()
            {
                Firstname = textBox1.Text,
                Lastname = textBox2.Text,
                Birthdate = dateTimePicker1.Value.Date,
                PassportNumber = textBox3.Text,
                CountryID = int.Parse(comboBox1.SelectedValue.ToString()),
                PassportCountry = comboBox1.Text,
                Phone = phone
            });
            LoadData();
        }


        public void LoadData()
        {
            dataGridView1.DataSource = listPassengerData.ToList();
            dataGridView1.Columns["Birthdate"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dataGridView1.Columns["CountryID"].Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                listPassengerData.RemoveAt(dataGridView1.CurrentRow.Index);
                LoadData();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (nPasenger > listPassengerData.Count)
            {
                MessageBox.Show($"Please add {nPasenger - listPassengerData.Count} more passenger data");
                return;
            }

            // Billing Confirmation
            BillingConfirmation form = new BillingConfirmation(header, detail, cabinType, listPassengerData);
            if (form.ShowDialog() == DialogResult.OK)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BookingConfirmation_Load(object sender, EventArgs e)
        {
            foreach (var a in header)
            {
                DestinationItem item = new DestinationItem(a, cabinType);
                flowLayoutPanel1.Controls.Add(item);
                flowLayoutPanel1.SetFlowBreak(item, true);
            }

            if (detail.Count > 0)
            {
                foreach (var a in detail)
                {
                    DestinationItem item = new DestinationItem(a, cabinType);
                    flowLayoutPanel2.Controls.Add(item);
                    flowLayoutPanel2.SetFlowBreak(item, true);
                }
            }
            else
            {
                groupBox2.Visible = false;
            }

            var q = db.Countries.ToList();
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "Name";
            comboBox1.DataSource = q;

            button2.BackColor = Color.Red;
            SetStyle(container);
        }
    }

    public class PassengerData
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Birthdate { get; set; }
        public string PassportNumber { get; set; }
        public string PassportCountry { get; set; }
        public int CountryID { get; set; }
        public string Phone { get; set; }
    }
}
