using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using Excel = Microsoft.Office.Interop.Excel;

namespace Fresh
{
    public partial class Form1 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Legends[0].Title = "Commission Report";
            chart1.Legends[0].TitleFont = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
            chart1.Legends[0].BorderColor = Color.Black;
            chart1.Legends[0].BorderWidth = 1;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Legends[0].Alignment = StringAlignment.Center;


            var q = db.Users.ToList().Select(x => new
            {
                User = x.FirstName + " " + x.LastName,
                Schedules = x.Tickets.Where(y => y.Confirmed).Select(y => y.Schedule).Where(y => y.Confirmed).ToList(),
                Tickets = x.Tickets.Where(y => y.Confirmed && y.Schedule.Confirmed).ToList()
            }).Select(x => new
            {
                x.User,
                AmenitiesSold = x.Tickets.SelectMany(y => y.AmenitiesTickets).Count(),
                TicketsSold = x.Tickets.Count,
                RevenueTickets = x.Tickets.GroupBy(y => y.Schedule)
                            .Select(y => new
                            {
                                Revenue = y.ToList().GroupBy(z => z.CabinTypeID).Select(z => new
                                {
                                    TotalSold = z.Count(),
                                    Price = z.Key == 1 ? y.Key.EconomyPrice :
                                            z.Key == 2 ? 1.35m * y.Key.EconomyPrice :
                                            1.3m * 1.35m * y.Key.EconomyPrice
                                }).Sum(z => z.TotalSold * z.Price)
                            }).Sum(y => y.Revenue),
                RevenueAmenities = x.Tickets.SelectMany(y => y.AmenitiesTickets).Sum(y => y.Price),
                x.Tickets,
                x.Schedules
            }).Select(x => new Data
            {
                User = x.User,
                AmenitiesSold = x.AmenitiesSold,
                TicketsSold = x.TicketsSold,
                CommissionEarned = ((x.RevenueAmenities + x.RevenueTickets) * 0.003m),
                Tickets = x.Tickets,
                Schedules = x.Schedules
            }).OrderByDescending(x => x.CommissionEarned).ToList();

            q.Add(new Data()
            {
                User = "Total",
                AmenitiesSold = q.Sum(x => x.AmenitiesSold),
                TicketsSold = q.Sum(x => x.TicketsSold),
                CommissionEarned = q.Sum(x => x.CommissionEarned)
            });


            dataGridView1.DataSource = q.ToList();
            //dataGridView1.Columns["Tickets"].Visible = false;
            //dataGridView1.Columns["Schedules"].Visible = false;
            dataGridView1.Columns["CommissionEarned"].DefaultCellStyle.Format = "N2";

            chart1.Series.Clear();
            Series s = chart1.Series.Add("Amenities Sold");
            Series s2 = chart1.Series.Add("Tickets Sold");
            Series s3 = chart1.Series.Add("Commission Earned");

            int i = 0;
            foreach (var a in q.Where(x => x.User != "Total").ToList())
            {
                s.Points.AddXY(a.User, a.AmenitiesSold);
                s.Points[i].Tag = new DataPointData() { Data = a, SeriesName = "Amenities Sold" };

                s2.Points.AddXY(a.User, a.TicketsSold);
                s2.Points[i].Tag = new DataPointData() { Data = a, SeriesName = "Tickets Sold" };

                s3.Points.AddXY(a.User, a.CommissionEarned);
                s3.Points[i].Tag = new DataPointData() { Data = a, SeriesName = "Commission Earned" };

                i++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            HitTestResult hit = chart1.HitTest(e.X, e.Y, ChartElementType.DataPoint);
            var obj = hit.Object as DataPoint;
            if (obj != null)
            {
                var data = (DataPointData)obj.Tag;
                CommisionForm form = new CommisionForm(data);
                if (form.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook wb = xlApp.Workbooks.Add(Missing.Value);
            Excel.Worksheet ws = wb.ActiveSheet;

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                ws.Cells[1, i + 2].Value = dataGridView1.Rows[i].Cells[0].Value.ToString();
            }

            ws.Cells[2, 1].Value = "Amenities Sold";
            ws.Cells[3, 1].Value = "Tickets Sold";
            ws.Cells[4, 1].Value = "Commission Earned";


            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                ws.Cells[2, i + 2].Value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                ws.Cells[3, i + 2].Value = dataGridView1.Rows[i].Cells[2].Value.ToString();
                ws.Cells[4, i + 2].Value = dataGridView1.Rows[i].Cells[3].Value.ToString();
            }

            Excel.ChartObjects xlObj = ws.ChartObjects(Missing.Value);
            Excel.ChartObject obj = xlObj.Add(10, 50, 450, 250);
            Excel.Chart chart = obj.Chart;

            Excel.Series s = chart.SeriesCollection().Add(ws.Range[ws.Cells[1, 1], ws.Cells[4, dataGridView1.Rows.Count]]);
            //s.XValues = ws.Range[ws.Cells[1, 2], ws.Cells[1, dataGridView1.Rows.Count - 1]];


            //Excel.Range rangeSourceChart = ws.Range[ws.Cells[1, 1], ws.Cells[4, dataGridView1.Rows.Count - 1]];
            //chart.SetSourceData(rangeSourceChart, Excel.XlRowCol.xlRows);

            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.Filter = "Excel Files|*.xlsx";
                sf.FileName = "CommissionReport";
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    wb.SaveAs(sf.FileName);
                    wb.Close();
                    xlApp.Quit();
                }
            }
        }
    }

    public class DataPointData
    {
        public Data Data { get; set; }
        public string SeriesName { get; set; }
    }

    public class Data
    {
        public string User { get; set; }
        public int AmenitiesSold { get; set; }
        public int TicketsSold { get; set; }
        public decimal CommissionEarned { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Schedule> Schedules { get; set; }
    }

}
