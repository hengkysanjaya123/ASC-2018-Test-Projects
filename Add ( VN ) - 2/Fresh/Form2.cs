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
    public partial class Form2 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<CheckInData> listCheckIn;
        SeatData seatData;
        Schedule s;
        List<Ticket> listSelected = new List<Ticket>();
        bool allowChange = false;
        int maximum = 0;
        string checkinType;

        public Form2(List<CheckInData> listCheckIn, SeatData seatData, Schedule s, ref List<Ticket> listSelected, string checkinType)
        {
            InitializeComponent();
            this.listCheckIn = listCheckIn;
            this.seatData = seatData;
            this.s = s;
            this.listSelected = listSelected;
            this.checkinType = checkinType;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (checkinType == "Single")
            {
                maximum = 1;
            }
            else
            {
                maximum = 2;
            }
            LoadData();
        }

        private void LoadData()
        {
            allowChange = false;
            db = new DataClasses1DataContext();

            var q = listCheckIn.Select(x => x.Ticket).Select(x => x.ID).ToList();
            var q2 = db.Tickets.ToList().Where(x =>
                        x.CabinType.Name == seatData.CabinType &&
                        x.ScheduleID == s.ID &&
                        !q.Contains(x.ID) &&
                        !listSelected.Select(y => y.ID).Contains(x.ID)
                    ).ToList().Select(x => new
                    {
                        x.PassportNumber,
                        FirstName = x.Firstname,
                        x.Lastname,
                        Code = x.ID.ToString(),
                        obj = x
                    })
                    .Where(x =>
                            (x.Code.StartsWith(textBox1.Text) || textBox1.Text == "") &&
                            (x.FirstName.Contains(textBox2.Text) || textBox2.Text == "") &&
                            (x.PassportNumber.StartsWith(textBox3.Text) || textBox3.Text == "")
                    )
                    .ToList();
            q2.InsertRange(0, listSelected.Select(x => new
            {
                x.PassportNumber,
                FirstName = x.Firstname,
                x.Lastname,
                Code = x.ID.ToString(),
                obj = x
            }));
            dataGridView1.DataSource = q2.ToList();
            dataGridView1.Columns["obj"].Visible = false;
            dataGridView1.CurrentCell = null;
            allowChange = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listSelected = new List<Ticket>();
            LoadData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (listSelected.Count != maximum)
            {
                MessageBox.Show($"Please select {maximum - listSelected.Count} more data");
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            if (allowChange)
            {
                if (dataGridView1.CurrentCell != null)
                {
                    var ticket = (Ticket)dataGridView1.CurrentRow.Cells["obj"].Value;
                    if (listSelected.Where(x => x.ID == ticket.ID).Count() == 0)
                    {
                        if (listSelected.Count == maximum)
                        {
                            MessageBox.Show($"You just can select {maximum} data");
                        }
                        else
                        {
                            listSelected.Add(ticket);
                            dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightBlue;
                        }
                    }
                    allowChange = false;
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;
                    allowChange = true;
                }
            }
        }
    }
}


