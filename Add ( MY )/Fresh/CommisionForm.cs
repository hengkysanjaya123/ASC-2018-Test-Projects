using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Excel = Microsoft.Office.Interop.Excel;

namespace Fresh
{
    public partial class CommisionForm : core
    {
        DataPointData data;
        bool formReady = false;

        public CommisionForm(DataPointData data)
        {
            InitializeComponent();
            this.data = data;
        }

        private void CommisionForm_Load(object sender, EventArgs e)
        {
            label1.Text = $"User : {data.Data.User}";
            this.Text = $"{data.SeriesName} by {data.Data.User}";

            var periods = data.Data.Tickets.Where(x => x.AmenitiesTickets.Count > 0  || data.SeriesName != "Amenities Sold")
                .Select(x => x.Schedule)
                .Select(x => new DateTime(x.Date.Year, x.Date.Month, 1)).Distinct()
                .Select(x => new Adapter
                {
                    Display = x.Date.ToString("MMMM yyyy"),
                    Value = new DateTime(x.Date.Year, x.Date.Month, 1)
                }).Distinct().ToList();

            comboBox1.DisplayMember = "Display";
            comboBox1.DataSource = periods;

            formReady = true;
            chart1.ChartAreas[0].AxisX.Interval = 1;
            LoadData();
        }

        public void LoadData()
        {
            if (!formReady) return;

            var adapter = (Adapter)comboBox1.SelectedValue;

            var totalDays = DateTime.DaysInMonth(adapter.Value.Year, adapter.Value.Month);

            chart1.Legends[0].Title = "Commission Report";
            chart1.Legends[0].TitleFont = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
            chart1.Legends[0].BorderColor = Color.Black;
            chart1.Legends[0].BorderWidth = 1;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Legends[0].Alignment = StringAlignment.Center;

            chart1.Series.Clear();
            Series s = chart1.Series.Add($"{data.SeriesName} at {adapter.Display}");
            s.ChartType = SeriesChartType.Point;

            decimal totalDataCommision = 0;
            for (int i = 1; i <= totalDays; i++)
            {
                var q = data.Data.Tickets.Where(x => x.Schedule.Date.ToString("MMMM yyyy") == adapter.Display
                                  && x.Schedule.Date.Day == i
                            ).ToList();

                if (data.SeriesName == "Amenities Sold")
                {
                    var q2 = q.SelectMany(x => x.AmenitiesTickets).Count();
                    totalDataCommision += q2;
                    s.Points.AddXY(i.ToString(), q2);
                }
                else if (data.SeriesName == "Tickets Sold")
                {
                    var q2 = q.Count();
                    totalDataCommision += q2;
                    s.Points.AddXY(i.ToString(), q2);
                }
                else if (data.SeriesName == "Commission Earned")
                {
                    var RevenueTickets = q.GroupBy(y => y.Schedule)
                            .Select(y => new
                            {
                                Revenue = y.ToList().GroupBy(z => z.CabinTypeID).Select(z => new
                                {
                                    TotalSold = z.Count(),
                                    Price = z.Key == 1 ? y.Key.EconomyPrice :
                                            z.Key == 2 ? 1.35m * y.Key.EconomyPrice :
                                            1.3m * 1.35m * y.Key.EconomyPrice
                                }).Sum(z => z.TotalSold * z.Price)
                            }).Sum(y => y.Revenue);

                    var revenueAmenities = q.SelectMany(y => y.AmenitiesTickets).Sum(y => y.Price);

                    var total = (revenueAmenities + RevenueTickets) * 0.003m;
                    s.Points.AddXY(i.ToString(), total);
                    totalDataCommision += total;
                }
            }

            label2.Text = $"Total {data.SeriesName} at {adapter.Display} : {totalDataCommision.ToString("#,0.##")}";

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        public class Adapter
        {
            public string Display { get; set; }
            public DateTime Value { get; set; }
        }
    }
}