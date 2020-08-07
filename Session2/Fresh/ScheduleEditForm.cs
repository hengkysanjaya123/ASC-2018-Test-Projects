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
    public partial class ScheduleEditForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        Schedule s;

        public ScheduleEditForm(Schedule s)
        {
            InitializeComponent();
            this.s = s;
        }

        // function form load
        private void ScheduleEditForm_Load(object sender, EventArgs e)
        {
            label2.Text = s.Route.Airport.IATACode;
            label4.Text = s.Route.Airport1.IATACode;
            label6.Text = s.Aircraft.Name;

            dateTimePicker1.Value = s.Date.Date;
            dateTimePicker2.Value = s.Date.Date + s.Time;
            textBox1.Text = Math.Floor(s.EconomyPrice).ToString();
        }

        // function to cancel form
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        // function to edit schedule data
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                decimal price = 0;
                if (!decimal.TryParse(textBox1.Text, out price))
                {
                    MessageBox.Show("Please input Economy Price with correct format");
                    return;
                }

                if (price < 0 || price > 922000000000000)
                {
                    MessageBox.Show("Sorry, the price is too higher or less than 0");
                    return;
                }

                DateTime start = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
                DateTime end = start + TimeSpan.FromMinutes(s.Route.FlightTime);

                if (s.Confirmed)
                {
                    if (!CheckOverlap(s, start, end))
                    {
                        MessageBox.Show("Sorry, schedule is overlap");
                        return;
                    }
                }

                var q = db.Schedules.Where(x => x.ID == s.ID).FirstOrDefault();
                q.Date = dateTimePicker1.Value.Date;
                q.Time = dateTimePicker2.Value.TimeOfDay;
                q.EconomyPrice = price;
                db.SubmitChanges();

                MessageBox.Show("Schedule updated successfully");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
