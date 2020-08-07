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
    public partial class AmenitiesReport : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<ReportData> listData = new List<ReportData>();

        public AmenitiesReport()
        {
            InitializeComponent();
        }

        private void AmenitiesReport_Load(object sender, EventArgs e)
        {
        }

        public void LoadData()
        {
            ReportDataBindingSource.DataSource = listData.ToList();
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listData = new List<ReportData>();
            LoadData();

            db = new DataClasses1DataContext();

            if (dateTimePicker2.Value.Date < dateTimePicker1.Value.Date)
            {
                MessageBox.Show("To Date must be greater than from date");
                return;
            }

            var q = db.Schedules.ToList().Where(x => x.Confirmed
                            && (x.FlightNumber == textBox1.Text || textBox1.Text == "")
                            && x.Date >= dateTimePicker1.Value.Date
                            && x.Date <= dateTimePicker2.Value.Date
                    ).SelectMany(x => x.Tickets)
                    .Where(x => x.Confirmed).ToList();

            var booked = q.SelectMany(x => x.AmenitiesTickets).Select(x => new ReportData()
            {
                Amenities = x.Amenity.Service,
                CabinType = x.Ticket.CabinType.Name,
                Total = 1
            }).ToList();

            var included = q.SelectMany(x => x.CabinType.AmenitiesCabinTypes).Select(x => new ReportData()
            {
                Amenities = x.Amenity.Service,
                CabinType = x.CabinType.Name,
                Total = 1
            }).ToList();

            listData.AddRange(booked);
            listData.AddRange(included);

            if (listData.Count == 0)
            {
                MessageBox.Show("There is no data");
            }

            LoadData();
        }
    }

    public class ReportData
    {
        public string Amenities { get; set; }
        public string CabinType { get; set; }
        public int Total { get; set; }
    }
}
