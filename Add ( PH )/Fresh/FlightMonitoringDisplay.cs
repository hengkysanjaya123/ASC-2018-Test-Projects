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
    public partial class FlightMonitoringDisplay : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        int currentIdx;
        List<ScheduleData> listScheduleData = new List<ScheduleData>();
        bool formReady = false;

        ScheduleData currentScheduleData;

        public FlightMonitoringDisplay()
        {
            InitializeComponent();
        }

        private void FlightMonitoringDisplay_Load(object sender, EventArgs e)
        {
            var q = db.Aircrafts.ToList();
            comboBox1.DisplayMember = "MakeModel";
            comboBox1.ValueMember = "ID";
            comboBox1.DataSource = q;

            formReady = true;
            LoadData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (!formReady) return;

            var currentDate = DateTime.Now;

            listScheduleData = db.Schedules.ToList()
                    .Where(x => x.AircraftID == int.Parse(comboBox1.SelectedValue.ToString())
                            && x.Confirmed
                    ).
                    OrderBy(x => x.Date + x.Time + TimeSpan.FromMinutes(x.Route.FlightTime))
                    .Select((x, i) => new ScheduleData
                    {
                        Schedule = x,
                        Index = i
                    }).ToList();

            var currentSchedule = listScheduleData.Where(x => x.Schedule.Date + x.Schedule.Time + TimeSpan.FromMinutes(x.Schedule.Route.FlightTime) >= currentDate).FirstOrDefault();
            if (currentSchedule == null)
            {
                currentIdx = listScheduleData.Count - 2;
            }
            else
            {
                currentIdx = currentSchedule.Index;
            }

            currentScheduleData = listScheduleData[currentIdx];
            ShowFlight();
            ShowFlightDetail(currentScheduleData);
            ShowPlane();
            timer1.Start();
        }

        public void ShowFlight()
        {
            button2.BackColor = ColorTranslator.FromHtml("#f79420");
            button3.BackColor = ColorTranslator.FromHtml("#f79420");
            button4.BackColor = ColorTranslator.FromHtml("#f79420");

            int btnPosition = 1;

            if (currentIdx - 1 < 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    ShowButton(i, btnPosition);
                    btnPosition += 1;
                }
            }
            else if (currentIdx + 1 > listScheduleData.Count - 1)
            {
                for (int i = listScheduleData.Count - 3; i <= listScheduleData.Count - 1; i++)
                {
                    ShowButton(i, btnPosition);
                    btnPosition += 1;
                }
            }
            else
            {
                for (int i = currentIdx - 1; i <= currentIdx + 1; i++)
                {
                    ShowButton(i, btnPosition);
                    btnPosition += 1;
                }
            }
        }

        public void ShowButton(int i, int btnPosition)
        {
            if (btnPosition == 1)
            {
                button2.Tag = listScheduleData[i];
                button2.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";
                if (i == currentIdx)
                {
                    button2.BackColor = ColorTranslator.FromHtml("#fffacb"); ;
                }
            }
            else if (btnPosition == 2)
            {
                button3.Tag = listScheduleData[i];
                button3.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";
                if (i == currentIdx)
                {
                    button3.BackColor = ColorTranslator.FromHtml("#fffacb"); ;
                }
            }
            else if (btnPosition == 3)
            {
                button4.Tag = listScheduleData[i];
                button4.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";
                if (i == currentIdx)
                {
                    button4.BackColor = ColorTranslator.FromHtml("#fffacb"); ;
                }
            }
        }


        public void ShowFlightDetail(ScheduleData sd)
        {
            var currentSchedule = sd;
            var s = currentSchedule.Schedule;

            var totalDistance = s.Route.Distance * 0.621371;
            var distance1 = totalDistance / 3;
            label11.Text = $"{distance1.ToString("N2")}mi";
            label19.Text = $"{(distance1 * 2).ToString("N2")}mi";
            label15.Text = $"{(distance1 * 3).ToString("N2")}mi";

            var duration1 = (double)s.Route.FlightTime / 3;
            var departureTime = s.Date + s.Time;

            // Flight Line Data        
            label7.Text = $"{departureTime.ToString("HHmm")}H";
            label8.Text = $"{(departureTime + TimeSpan.FromMinutes(duration1)).ToString("HHmm")}H";
            label16.Text = $"{(departureTime + TimeSpan.FromMinutes(duration1 * 2)).ToString("HHmm")}H";
            label13.Text = $"{(departureTime + TimeSpan.FromMinutes(duration1 * 3)).ToString("HHmm")}H";

            // From To
            label2.Text = s.Route.Airport.IATACode;
            label3.Text = s.Route.Airport1.IATACode;

            // Departure Details
            label33.Text = $"{s.Route.Airport.Name}";
            label34.Text = departureTime.ToString("yyyy-MM-dd");
            label35.Text = departureTime.ToString("HHmm") + "H";


            var arrivalTime = s.Date + s.Time + TimeSpan.FromMinutes(s.Route.FlightTime);

            label36.Text = $"{s.Route.Airport1.Name}";
            label37.Text = arrivalTime.ToString("yyyy-MM-dd");
            label38.Text = arrivalTime.ToString("HHmm") + "H";

            label39.Text = totalDistance.ToString("N2") + " Miles";
            label40.Text = s.Route.FlightTime + " Minutes";

            if (arrivalTime > DateTime.Now)
            {
                label32.Text = "ON TIME";
            }
            else
            {
                label32.Text = "ARRIVED";
                //currentIdx += 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Previous
            if (currentIdx > 1)
            {
                currentIdx = currentIdx - 1;
                currentScheduleData = listScheduleData[currentIdx];
                ShowFlight();
                ShowFlightDetail(currentScheduleData);
                ShowPlane();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Next
            if (currentIdx < listScheduleData.Count - 2)
            {
                currentIdx = currentIdx + 1;
                currentScheduleData = listScheduleData[currentIdx];
                ShowFlight();
                ShowFlightDetail(currentScheduleData);
                ShowPlane();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OwnClick((Button)sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OwnClick((Button)sender);
        }

        public void OwnClick(Button btn)
        {
            button2.BackColor = ColorTranslator.FromHtml("#f79420");
            button3.BackColor = ColorTranslator.FromHtml("#f79420");
            button4.BackColor = ColorTranslator.FromHtml("#f79420");

            currentScheduleData = (ScheduleData)btn.Tag;
            ShowFlightDetail(currentScheduleData);
            ShowPlane();
            btn.BackColor = ColorTranslator.FromHtml("#fffacb");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OwnClick((Button)sender);
        }

        public void ShowPlane()
        {
            var current = currentScheduleData.Schedule;

            var currentDate = DateTime.Now;
            var departure = current.Date + current.Time;
            var arrival = departure + TimeSpan.FromMinutes(current.Route.FlightTime);

            var position = 0d;
            var subtract = (currentDate - departure).TotalMinutes;
            if (subtract <= 0)
            {
                position = 0;
            }
            else if (currentDate > arrival)
            {
                position = 1;
            }
            else
            {
                position = subtract / current.Route.FlightTime;
            }

            position = position * 600;

            panel2.Width = (int)position;
            panel3.Location = new Point((int)(panel2.Location.X + position), 40);
            panel3.Width = 600 - (int)position;

            pictureBox2.Visible = true;
            pictureBox2.Location = new Point((((int)position) - pictureBox2.Width / 2) + 66, 30);

            ShowFlightDetail(currentScheduleData);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowPlane();
        }
    }
    public class ScheduleData
    {
        public int Index { get; set; }
        public Schedule Schedule { get; set; }
    }
}
