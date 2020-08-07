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
    public partial class Form1 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        bool formReady = false;
        List<ScheduleData> listScheduleData = new List<ScheduleData>();

        int currentIdx;
        ScheduleData currentScheduleData;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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

        public void LoadData()
        {
            if (!formReady) return;

            listScheduleData = db.Schedules.ToList().Where(x => x.Confirmed
                        && x.AircraftID == int.Parse(comboBox1.SelectedValue.ToString())
                    ).OrderBy(x => x.Date + x.Time + TimeSpan.FromMinutes(x.Route.FlightTime))
                    .Select((x, i) => new ScheduleData
                    {
                        Schedule = x,
                        Index = i
                    }).ToList();

            var currentSchedule = listScheduleData.Where(x => x.Schedule.Date + x.Schedule.Time + TimeSpan.FromMinutes(x.Schedule.Route.FlightTime) >= DateTime.Now).FirstOrDefault();
            if (currentSchedule == null)
            {
                currentIdx = listScheduleData.Count - 2;
            }
            else
            {
                currentIdx = currentSchedule.Index;
            }

            if (currentIdx < 0)
            {
                currentIdx = 0;
                currentScheduleData = listScheduleData[0];
            }
            else
            {
                currentScheduleData = listScheduleData[currentIdx];
            }
            ShowFlight();
            ShowFlightDetail();
            ShowPlane();
            timer1.Start();
        }

        public void ShowFlightDetail()
        {
            var data = currentScheduleData;
            var s = data.Schedule;
            label1.Text = s.Route.Airport.IATACode;
            label3.Text = s.Route.Airport1.IATACode;

            var departureTime = s.Date + s.Time;
            var arrivalTime = s.Date + s.Time + TimeSpan.FromMinutes(s.Route.FlightTime);

            var totalDistance = s.Route.Distance * 0.621371;
            var distance = totalDistance / 3;
            label10.Text = distance.ToString("N2") + "mi";
            label14.Text = (distance * 2).ToString("N2") + "mi";
            label18.Text = totalDistance.ToString("N2") + "mi";

            var totalDuration = s.Route.FlightTime;
            var duration = totalDuration / 3;
            label6.Text = departureTime.ToString("HHmm") + "H";
            label8.Text = (departureTime + TimeSpan.FromMinutes(duration)).ToString("HHmm") + "H";
            label12.Text = (departureTime + TimeSpan.FromMinutes(duration * 2)).ToString("HHmm") + "H";
            label16.Text = arrivalTime.ToString("HHmm") + "H";

            // departure details
            label23.Text = s.Route.Airport.Name;
            label24.Text = departureTime.ToString("yyyy-MM-dd");
            label25.Text = departureTime.ToString("HHmm") + "H";

            // arrival details
            label33.Text = s.Route.Airport1.Name;
            label35.Text = arrivalTime.ToString("yyyy-MM-dd");
            label37.Text = arrivalTime.ToString("HHmm") + "H";


            label28.Text = totalDistance.ToString("N2") + " Miles";
            label30.Text = totalDuration + " Minutes";

            if (arrivalTime > DateTime.Now)
            {
                label39.Text = "ON TIME";
            }
            else
            {
                label39.Text = "Arrived";
            }
        }

        public void ShowFlight()
        {
            button2.BackColor = SystemColors.Control;
            button3.BackColor = SystemColors.Control;
            button4.BackColor = SystemColors.Control;

            int buttonPosition = 1;
            if (currentIdx - 1 < 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    ShowButton(i, buttonPosition);
                    buttonPosition += 1;
                }
            }
            else if (currentIdx + 1 > listScheduleData.Count - 1)
            {
                for (int i = listScheduleData.Count - 3; i <= listScheduleData.Count - 1; i++)
                {
                    ShowButton(i, buttonPosition);
                    buttonPosition += 1;
                }
            }
            else
            {
                for (int i = currentIdx - 1; i <= currentIdx + 1; i++)
                {
                    ShowButton(i, buttonPosition);
                    buttonPosition += 1;
                }
            }
        }

        public void ShowButton(int i, int btnPosition)
        {
            if (btnPosition == 1)
            {
                if (i <= listScheduleData.Count - 1)
                {
                    button2.Tag = listScheduleData[i];
                    button2.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";

                    if (i == currentIdx)
                    {
                        button2.BackColor = Color.White;
                    }
                }
                else
                {
                    button2.Tag = null;
                    button2.Text = "No Data";
                }
            }
            else if (btnPosition == 2)
            {
                if (i <= listScheduleData.Count - 1)
                {
                    button3.Tag = listScheduleData[i];
                    button3.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";

                    if (i == currentIdx)
                    {
                        button3.BackColor = Color.White;
                    }
                }
                else
                {
                    button3.Tag = null;
                    button3.Text = "No Data";
                }
            }
            else if (btnPosition == 3)
            {
                if (i <= listScheduleData.Count - 1)
                {
                    button4.Tag = listScheduleData[i];
                    button4.Text = $"{listScheduleData[i].Schedule.Date.ToString("MMM dd yyyy")}\nFlight ID : {listScheduleData[i].Schedule.FlightNumber}";

                    if (i == currentIdx)
                    {
                        button4.BackColor = Color.White;
                    }
                }
                else
                {
                    button4.Tag = null;
                    button4.Text = "No Data";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentIdx > 1)
            {
                currentIdx = currentIdx - 1;
                currentScheduleData = listScheduleData[currentIdx];
                ShowFlight();
                ShowFlightDetail();
                ShowPlane();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (currentIdx < listScheduleData.Count - 2)
            {
                currentIdx = currentIdx + 1;
                currentScheduleData = listScheduleData[currentIdx];
                ShowFlight();
                ShowFlightDetail();
                ShowPlane();
            }
        }

        private void OwnClick(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                button2.BackColor = SystemColors.Control;
                button3.BackColor = SystemColors.Control;
                button4.BackColor = SystemColors.Control;

                if (btn.Tag != null)
                {
                    currentScheduleData = (ScheduleData)btn.Tag;
                    ShowFlightDetail();
                    btn.BackColor = Color.White;
                    ShowPlane();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowPlane()
        {
            var data = currentScheduleData;
            var s = data.Schedule;

            var departure = s.Date + s.Time;
            var arrival = departure + TimeSpan.FromMinutes(s.Route.FlightTime);

            var substract = (DateTime.Now - departure).TotalMinutes;

            var position = 0d;
            if (substract <= 0)
            {
                position = 0;
            }
            else if (DateTime.Now > arrival)
            {
                position = 1;
            }
            else
            {
                position = substract / s.Route.FlightTime;
            }

            position = position * 600;

            pictureBox2.Location = new Point((int)position - pictureBox2.Width / 2 + 87, 35);
            panel2.Width = (int)position;

            panel3.Location = new Point((int)(panel2.Right + position), 43);
            panel3.Width = 600 - (int)position;

            ShowFlightDetail();
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
