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
    public partial class SearchForFlights : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        List<List<Schedule>> listHeader = new List<List<Schedule>>(), listDetail = new List<List<Schedule>>();
        CabinType selectedCabinType;
        DateTime outBoundDateTime, returnDateTime;
        bool returnStatus;

        public SearchForFlights()
        {
            InitializeComponent();
        }

        private void SearchForFlights_Load(object sender, EventArgs e)
        {
            var q = db.Airports.ToList();
            comboBox1.DisplayMember = "IATACode";
            comboBox1.ValueMember = "IATACode";
            comboBox1.DataSource = q;


            var q2 = db.Airports.ToList();
            comboBox2.DisplayMember = "IATACode";
            comboBox2.ValueMember = "IATACode";
            comboBox2.DataSource = q2;

            var q3 = db.CabinTypes;
            comboBox3.DisplayMember = "Name";
            comboBox3.DataSource = q3;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label5.Visible = radioButton1.Checked;
            dateTimePicker2.Visible = radioButton1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = radioButton1.Checked;

            listHeader.Clear();
            listDetail.Clear();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;

            string from = comboBox1.SelectedValue.ToString();
            string to = comboBox2.SelectedValue.ToString();
            selectedCabinType = (CabinType)comboBox3.SelectedValue;
            outBoundDateTime = dateTimePicker1.Value.Date;
            returnDateTime = dateTimePicker2.Value.Date;
            returnStatus = radioButton1.Checked;

            if (from == to)
            {
                MessageBox.Show("From and To Airport cannot be same");
                return;
            }

            if (returnStatus)
            {
                if (dateTimePicker2.Value.Date < dateTimePicker1.Value.Date)
                {
                    MessageBox.Show("Return date must be greater than outbound date");
                    return;
                }
            }

            for (int i = -3; i <= 3; i++)
            {
                GetRoute(from, to, outBoundDateTime.AddDays(i), new List<Schedule>(), ref listHeader);
            }

            if (returnStatus)
            {
                for (int i = -3; i <= 3; i++)
                {
                    GetRoute(to, from, returnDateTime.AddDays(i), new List<Schedule>(), ref listDetail);
                }
                panel2.Visible = true;
            }
            else
            {
                panel2.Visible = false;
            }

            ShowHeader();
            ShowDetail();
        }

        public void ShowHeader()
        {
            if (checkBox1.Checked)
            {
                dataGridView1.DataSource = listHeader.Select(x => new
                {
                    From = x.FirstOrDefault().Route.Airport.IATACode,
                    To = x.LastOrDefault().Route.Airport1.IATACode,
                    Date = x.FirstOrDefault().Date.ToString("dd/MM/yyyy"),
                    Time = (x.FirstOrDefault().Date + x.FirstOrDefault().Time).ToString(@"HH\:mm"),
                    FlightNumber = string.Join(" - ", x.Select(y => y.FlightNumber).ToArray()),
                    CabinPrice = GetCabinPrice(x, selectedCabinType),
                    NumberOfStops = x.Count - 1,
                    listSchedule = new Rute() { listSchedule = x }
                }).ToList();
            }
            else
            {
                dataGridView1.DataSource = listHeader.Where(x => x.FirstOrDefault().Date == outBoundDateTime.Date)
                    .Select(x => new
                    {
                        From = x.FirstOrDefault().Route.Airport.IATACode,
                        To = x.LastOrDefault().Route.Airport1.IATACode,
                        Date = x.FirstOrDefault().Date.ToString("dd/MM/yyyy"),
                        Time = (x.FirstOrDefault().Date + x.FirstOrDefault().Time).ToString(@"HH\:mm"),
                        FlightNumber = string.Join(" - ", x.Select(y => y.FlightNumber).ToArray()),
                        CabinPrice = GetCabinPrice(x, selectedCabinType),
                        NumberOfStops = x.Count - 1,
                        listSchedule = new Rute() { listSchedule = x }
                    }).ToList();
            }
            dataGridView1.Columns["listSchedule"].Visible = false;
            dataGridView1.Columns["CabinPrice"].DefaultCellStyle.Format = "C0";

            dataGridView1.Columns["FlightNumber"].HeaderText = "Flight Number(s)";
            dataGridView1.Columns["CabinPrice"].HeaderText = "Cabin Price";
            dataGridView1.Columns["NumberOfStops"].HeaderText = "Number of stops";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShowHeader();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ShowDetail();
        }

        public bool CheckSeat(List<Schedule> listSchedule, CabinType cabinType, int nPassenger)
        {
            foreach (var a in listSchedule)
            {
                int totalSeat = 0;
                if (cabinType.ID == 1)
                {
                    totalSeat = a.Aircraft.EconomySeats;
                }
                else if (cabinType.ID == 2)
                {
                    totalSeat = a.Aircraft.BusinessSeats;
                }
                else if (cabinType.ID == 3)
                {
                    totalSeat = a.Aircraft.TotalSeats - a.Aircraft.EconomySeats - a.Aircraft.BusinessSeats;
                }

                var q = db.Tickets.Where(x => x.Confirmed
                            && x.ScheduleID == a.ID
                            && x.CabinTypeID == cabinType.ID
                        ).Count();

                if (nPassenger > (totalSeat - q))
                {
                    return false;
                }
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please input Number of passengers");
                return;
            }

            int nPassengers = 0;
            if (!int.TryParse(textBox1.Text, out nPassengers))
            {
                MessageBox.Show("Number of passengers must be numeric and not too high");
                return;
            }

            if (nPassengers <= 0)
            {
                MessageBox.Show("number of passengers must be greater than 0");
                return;
            }

            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select the data in the datagridview first");
                return;
            }

            var header = listHeader.Where(x => x == (dataGridView1.CurrentRow.Cells["listSchedule"].Value as Rute).listSchedule).FirstOrDefault();
            if (!CheckSeat(header, selectedCabinType, nPassengers))
            {
                MessageBox.Show("Sorry, not enough seat for selected outbound flight");
                return;
            }

            List<Schedule> detail = new List<Schedule>();
            if (returnStatus)
            {
                detail = listDetail.Where(x => x == (dataGridView2.CurrentRow.Cells["listSchedule"].Value as Rute).listSchedule).FirstOrDefault();
                if (!CheckSeat(detail, selectedCabinType, nPassengers))
                {
                    MessageBox.Show("Sorry, not enough seat for selected return flight");
                    return;
                }

                var maxHeader = header.OrderByDescending(x => x.Date + x.Time).FirstOrDefault();
                var minDetail = detail.OrderBy(x => x.Date + x.Time).FirstOrDefault();

                var arrivedHeader = maxHeader.Date + maxHeader.Time + TimeSpan.FromMinutes(maxHeader.Route.FlightTime);
                var departureDetail = minDetail.Date + minDetail.Time;

                if (departureDetail < arrivedHeader)
                {
                    MessageBox.Show("Selected return flight's datetime must be greater than selected outbound flight's datetime");
                    return;
                }
            }

            // Booking Confirmation
            BookingConfirmation form = new BookingConfirmation(header, detail, selectedCabinType, nPassengers);
            if (form.ShowDialog() == DialogResult.OK)
            {
                listHeader.Clear();
                listDetail.Clear();
                dataGridView1.DataSource = null;
                dataGridView2.DataSource = null;
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                textBox1.Text = "";
            }
        }

        public void ShowDetail()
        {
            if (checkBox2.Checked)
            {
                dataGridView2.DataSource = listDetail.Select(x => new
                {
                    From = x.FirstOrDefault().Route.Airport.IATACode,
                    To = x.LastOrDefault().Route.Airport1.IATACode,
                    Date = x.FirstOrDefault().Date.ToString("dd/MM/yyyy"),
                    Time = (x.FirstOrDefault().Date + x.FirstOrDefault().Time).ToString(@"HH\:mm"),
                    FlightNumber = string.Join(" - ", x.Select(y => y.FlightNumber).ToArray()),
                    CabinPrice = GetCabinPrice(x, selectedCabinType),
                    NumberOfStops = x.Count - 1,
                    listSchedule = new Rute() { listSchedule = x }
                }).ToList();
            }
            else
            {
                dataGridView2.DataSource = listDetail.Where(x => x.FirstOrDefault().Date == returnDateTime.Date)
                    .Select(x => new
                    {
                        From = x.FirstOrDefault().Route.Airport.IATACode,
                        To = x.LastOrDefault().Route.Airport1.IATACode,
                        Date = x.FirstOrDefault().Date.ToString("dd/MM/yyyy"),
                        Time = (x.FirstOrDefault().Date + x.FirstOrDefault().Time).ToString(@"HH\:mm"),
                        FlightNumber = string.Join(" - ", x.Select(y => y.FlightNumber).ToArray()),
                        CabinPrice = GetCabinPrice(x, selectedCabinType),
                        NumberOfStops = x.Count - 1,
                        listSchedule = new Rute() { listSchedule = x }
                    }).ToList();
            }
            dataGridView2.Columns["listSchedule"].Visible = false;
            dataGridView2.Columns["CabinPrice"].DefaultCellStyle.Format = "C0";

            dataGridView2.Columns["FlightNumber"].HeaderText = "Flight Number(s)";
            dataGridView2.Columns["CabinPrice"].HeaderText = "Cabin Price";
            dataGridView2.Columns["NumberOfStops"].HeaderText = "Number of stops";
        }

        public void GetRoute(string from, string to, DateTime outBound, List<Schedule> visited, ref List<List<Schedule>> listSchedules)
        {
            if (from == to)
            {
                listSchedules.Add(visited);
                return;
            }

            if (visited.Where(x => x.Route.Airport.IATACode == from).Count() > 0) return;

            var q = db.Schedules.ToList().Where(x => x.Confirmed
                            && x.Route.Airport.IATACode == from
                            && x.Date == outBound.Date
                            && x.Date + x.Time > outBound
                            && x.Date + x.Time <= outBound.AddHours(24)
                            );
            foreach (var a in q)
            {
                List<Schedule> newVisited = new List<Schedule>(visited);
                newVisited.Add(a);

                DateTime newOutbound = a.Date + a.Time + TimeSpan.FromMinutes(a.Route.FlightTime);

                GetRoute(a.Route.Airport1.IATACode, to, newOutbound, newVisited, ref listSchedules);
            }
        }
    }

    public class Rute
    {
        public List<Schedule> listSchedule { get; set; }
    }
}
