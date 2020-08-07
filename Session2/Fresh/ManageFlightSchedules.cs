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
    public partial class ManageFlightSchedules : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<string> listSort = new List<string>()
        {
            "Date and Time", "Economy Price", "Confirmed"
        };

        Airport from, to;
        string sortBy, flightNumber;
        DateTime outBound;
        bool outBoundChecked;

        public ManageFlightSchedules()
        {
            InitializeComponent();
        }

        // function form load
        private void ManageFlightSchedules_Load(object sender, EventArgs e)
        {
            var q = db.Airports.ToList();
            q.Insert(0, new Airport()
            {
                ID = 0,
                IATACode = "All Airports"
            });
            comboBox1.DisplayMember = "IATACode";
            comboBox1.DataSource = q;


            var q2 = db.Airports.ToList();
            q2.Insert(0, new Airport()
            {
                ID = 0,
                IATACode = "All Airports"
            });
            comboBox2.DisplayMember = "IATACode";
            comboBox2.DataSource = q2;

            comboBox3.DataSource = listSort;
            dateTimePicker1.Checked = false;

            button1.PerformClick();
        }

        // function to change button caption based on schedule confirmed status
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var s = (Schedule)dataGridView1.CurrentRow.Cells["obj"].Value;
                if (s.Confirmed)
                {
                    button2.Text = "Cancel Flight";
                    button2.BackColor = Color.Red;
                }
                else
                {
                    button2.Text = "Confirm Flight";
                    button2.BackColor = ColorTranslator.FromHtml("#f79420");
                }
            }
            catch
            {
            }
        }

        // function to Confirmed / Cancelled Flight
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please choose the schedule data in the datagridview first");
                    return;
                }

                var s = (Schedule)dataGridView1.CurrentRow.Cells["obj"].Value;

                // Check Overlap
                if (!s.Confirmed)
                {
                    if (!CheckOverlap(s, s.Date + s.Time, s.Date + s.Time + TimeSpan.FromMinutes(s.Route.FlightTime)))
                    {
                        MessageBox.Show("Sorry, schedule overlap");
                        return;
                    }
                }

                s.Confirmed = !s.Confirmed;
                db.SubmitChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // function to open edit flight form
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please choose the schedule data in the datagridview first");
                return;
            }

            var s = (Schedule)dataGridView1.CurrentRow.Cells["obj"].Value;

            // Edit Flight
            ScheduleEditForm form = new ScheduleEditForm(s);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        // function to open apply schedule changes form
        private void button4_Click(object sender, EventArgs e)
        {
            ApplyScheduleChanges form = new ApplyScheduleChanges();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        // function to apply search filter
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "All Airports" && comboBox2.Text != "All Airports")
            {
                if (comboBox1.Text == comboBox2.Text)
                {
                    MessageBox.Show("From and To Airport cannot be same");
                    return;
                }
            }

            from = (Airport)comboBox1.SelectedValue;
            to = (Airport)comboBox2.SelectedValue;
            sortBy = comboBox3.SelectedValue.ToString();
            outBound = dateTimePicker1.Value.Date;
            outBoundChecked = dateTimePicker1.Checked;
            flightNumber = textBox1.Text;
            LoadData();
        }

        // function to load schedule data
        public void LoadData()
        {
            db = new DataClasses1DataContext();

            var q = db.Schedules.ToList().Where(x =>
                        (x.Route.Airport.ID == from.ID || from.ID == 0) &&
                        (x.Route.Airport1.ID == to.ID || to.ID == 0) &&
                        (x.Date == outBound.Date || outBoundChecked == false) &&
                        (x.FlightNumber == flightNumber || flightNumber == "")
                    ).ToList();

            if (sortBy == listSort[0])
            {
                q = q.OrderByDescending(x => x.Date + x.Time).ToList();
            }
            else if (sortBy == listSort[1])
            {
                q = q.OrderByDescending(x => x.EconomyPrice).ToList();
            }
            else if (sortBy == listSort[2])
            {
                q = q.OrderByDescending(x => x.Confirmed).ToList();
            }
            
            

            dataGridView1.DataSource = q.Select(x => new
            {
                Date = x.Date.ToString("dd/MM/yyyy"),
                Time = (x.Date + x.Time).ToString(@"HH\:mm"),
                From = x.Route.Airport.IATACode,
                To = x.Route.Airport1.IATACode,
                x.FlightNumber,
                Aircraft = x.Aircraft.Name,
                EconomyPrice = Math.Floor(x.EconomyPrice),
                BusinessPrice = Math.Floor(1.35m * Math.Floor(x.EconomyPrice)),
                FirstClassPrice = Math.Floor(1.3m * Math.Floor(1.35m * Math.Floor(x.EconomyPrice))),
                obj = x
            }).ToList();
            dataGridView1.Columns["obj"].Visible = false;

            dataGridView1.Columns["EconomyPrice"].DefaultCellStyle.Format = "C0";
            dataGridView1.Columns["BusinessPrice"].DefaultCellStyle.Format = "C0";
            dataGridView1.Columns["FirstClassPrice"].DefaultCellStyle.Format = "C0";

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var s = (Schedule)dataGridView1.Rows[i].Cells["obj"].Value;

                if (!s.Confirmed)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }
    }
}
