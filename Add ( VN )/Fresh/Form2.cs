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
        SeatData sd;
        Schedule s;
        List<Ticket> listTickets = new List<Ticket>();
        int maxSelected = 0;
        bool allowChange = false;

        public Form2(SeatData sd, Schedule s, ref List<Ticket> listTickets)
        {
            InitializeComponent();
            this.sd = sd;
            this.s = s;
            this.listTickets = listTickets;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (sd.StatusSeat == "EmptyAlone" || sd.StatusSeat == "Empty")
            {
                maxSelected = 1;
            }
            else
            {
                maxSelected = 2;
            }
            LoadData();
        }

        private void LoadData(bool reset = false)
        {
            allowChange = false;
            var q = db.Tickets.ToList().Where(x => x.ScheduleID == s.ID
                                        && x.CabinType.Name == sd.CabinType
                                    ).Where(x =>
                                        (x.ID.ToString().Contains(textBox1.Text) || textBox1.Text == "") &&
                                        (x.Firstname.Contains(textBox2.Text) || textBox2.Text == "") &&
                                        (x.PassportNumber.Contains(textBox3.Text) || textBox3.Text == "")
                                    )
                                    .Select(x => new
                                    {
                                        x.PassportNumber,
                                        x.Firstname,
                                        x.Lastname,
                                        Code = x.ID,
                                        obj = x
                                    }).ToList();
            dataGridView1.DataSource = q;
            dataGridView1.Columns["obj"].Visible = false;
            dataGridView1.CurrentCell = null;

            if (reset)
            {
                listTickets = new List<Ticket>();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }
            allowChange = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (allowChange)
            {
                var ticket = (Ticket)dataGridView1.CurrentRow.Cells["obj"].Value;
                if (listTickets.Where(x => x.ID == ticket.ID).Count() == 0)
                {
                    if (listTickets.Count == maxSelected)
                    {
                        MessageBox.Show($"Maximum selected data is { maxSelected}");
                        return;
                    }
                    else
                    {
                        listTickets.Add(ticket);
                        dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            this.DialogResult = DialogResult.OK;
        }
    }
}
