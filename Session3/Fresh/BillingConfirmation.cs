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
    public partial class BillingConfirmation : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<Schedule> header = new List<Schedule>(), detail = new List<Schedule>();
        CabinType cabinType;
        List<PassengerData> listPassengerData = new List<PassengerData>();

        public BillingConfirmation(List<Schedule> header, List<Schedule> detail, CabinType cabinType, List<PassengerData> listPassengerData)
        {
            InitializeComponent();
            this.header = header;
            this.detail = detail;
            this.cabinType = cabinType;
            this.listPassengerData = listPassengerData;
        }

        private void BillingConfirmation_Load(object sender, EventArgs e)
        {
            var q1 = GetCabinPrice(header, cabinType) * listPassengerData.Count;
            var q2 = GetCabinPrice(detail, cabinType) * listPassengerData.Count;
            var total = q1 + q2;
            label2.Text = Math.Floor(total).ToString("C0");
        }

        public string GetBookingReference()
        {
            string data = "ABCDEFGHIJKLNNOPQRSTUVWXYZ1234567890";
            Random rand = new Random();
            string result = "";
            bool unique = false;

            while (!unique)
            {
                for (int i = 0; i < 6; i++)
                {
                    result += data[rand.Next(0, data.Length)];
                }

                var q = db.Tickets.Where(x => x.BookingReference == result).Count();
                if (q > 0)
                {
                    result = "";
                }
                else
                {
                    unique = true;
                }
            }

            return result;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string bookRef = GetBookingReference();

                foreach (var s in header)
                {
                    foreach (var p in listPassengerData)
                    {
                        Ticket t = new Ticket()
                        {
                            UserID = 1,
                            ScheduleID = s.ID,
                            CabinTypeID = cabinType.ID,
                            Firstname = p.Firstname,
                            Lastname = p.Lastname,
                            Phone = p.Phone,
                            PassportNumber = p.PassportNumber,
                            PassportCountryID = p.CountryID,
                            BookingReference = bookRef,
                            Confirmed = true
                        };
                        db.Tickets.InsertOnSubmit(t);
                        db.SubmitChanges();
                    }
                }


                foreach (var s in detail)
                {
                    foreach (var p in listPassengerData)
                    {
                        Ticket t = new Ticket()
                        {
                            UserID = 1,
                            ScheduleID = s.ID,
                            CabinTypeID = cabinType.ID,
                            Firstname = p.Firstname,
                            Lastname = p.Lastname,
                            Phone = p.Phone,
                            PassportNumber = p.PassportNumber,
                            PassportCountryID = p.CountryID,
                            BookingReference = bookRef,
                            Confirmed = true
                        };
                        db.Tickets.InsertOnSubmit(t);
                        db.SubmitChanges();
                    }
                }

                MessageBox.Show("Data Saved");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
