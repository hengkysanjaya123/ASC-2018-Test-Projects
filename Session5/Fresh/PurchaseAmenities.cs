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
    public partial class PurchaseAmenities : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<Amenity> listAmenity = new List<Amenity>();
        Ticket currentTicket;
        decimal paidBefore = 0;

        public PurchaseAmenities()
        {
            InitializeComponent();
        }

        private void PurchaseAmenities_Load(object sender, EventArgs e)
        {
            Clear(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void Clear(bool clearFlightList)
        {
            if (clearFlightList)
            {
                comboBox1.DataSource = null;
                button2.Enabled = false;
            }

            button3.Enabled = false;
            label4.Text = "";
            label6.Text = "";
            label8.Text = "";
            label10.Text = "";
            label12.Text = "";
            label14.Text = "";
            listAmenity = new List<Amenity>();
            panel1.Controls.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clear(true);

            db = new DataClasses1DataContext();

            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Please input booking reference");
                return;
            }

            var q = db.Tickets.ToList().Where(x => x.BookingReference == textBox1.Text
                            && x.Confirmed
                            && x.Schedule.Confirmed
                            && ((x.Schedule.Date + x.Schedule.Time) - DateTime.Now).TotalHours >= 24
                        ).ToList();

            if (q.Count == 0)
            {
                MessageBox.Show("Booking Reference not found or the service is unavailable");
                return;
            }

            button2.Enabled = true;

            var q2 = q.Select(x => new
            {
                Display = $"{x.Schedule.FlightNumber}, {x.Schedule.Route.Airport.IATACode}-{x.Schedule.Route.Airport1.IATACode}, {x.Schedule.Date.ToString("dd/MM/yyyy")}, {(x.Schedule.Date + x.Schedule.Time).ToString(@"HH\:mm")}",
                Value = x
            }).ToList();
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = q2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listAmenity = new List<Amenity>();
            panel1.Controls.Clear();
            button3.Enabled = true;

            currentTicket = (Ticket)comboBox1.SelectedValue;

            label4.Text = currentTicket.Firstname + " " + currentTicket.Lastname;
            label6.Text = currentTicket.CabinType.Name;
            label8.Text = currentTicket.PassportNumber;

            var booked = db.AmenitiesTickets.Where(x => x.TicketID == currentTicket.ID).ToList();
            listAmenity.AddRange(booked.Select(x => x.Amenity));
            paidBefore = booked.Sum(x => x.Amenity.Price);

            var included = db.AmenitiesCabinTypes.Where(x => x.CabinTypeID == currentTicket.CabinTypeID).ToList();

            int i = 0;
            int pX = 22;
            int pY = 0;
            foreach (var a in db.Amenities.ToList())
            {
                i++;
                if (i % 2 == 0)
                {
                    pX = 300;
                }
                else
                {
                    pX = 22;
                    pY += 20;
                }
                CheckBox chk = new CheckBox();
                chk.AutoSize = true;
                chk.Location = new System.Drawing.Point(pX, pY);
                chk.Name = "checkBox1";
                chk.Size = new System.Drawing.Size(86, 19);
                chk.TabIndex = 0;
                chk.UseVisualStyleBackColor = true;

                chk.Tag = a;
                chk.Text = $"{a.Service} ( {Math.Floor(a.Price).ToString("C0")} )";

                if (included.Select(x => x.AmenityID).Contains(a.ID))
                {
                    chk.Checked = true;
                    chk.Enabled = false;
                    chk.Text = $"{a.Service} (Free)";
                }

                if (booked.Select(x => x.AmenityID).Contains(a.ID))
                {
                    chk.Checked = true;
                }

                if (a.Price == 0)
                {
                    chk.Enabled = false;
                }
                chk.CheckedChanged += Chk_CheckedChanged;
                panel1.Controls.Add(chk);
            }
            ShowPrice();
        }

        public void ShowPrice()
        {
            var itemsSelected = listAmenity.Sum(x => x.Price);
            var tax = itemsSelected * 0.05m;

            var totalPayable = (itemsSelected - paidBefore) * 1.05m;
            label10.Text = itemsSelected.ToString("C2");
            label12.Text = tax.ToString("C2");
            label14.Text = totalPayable >= 0 ? totalPayable.ToString("C2") : Math.Abs(totalPayable).ToString("C2") + " (Refund)";
        }

        private void Chk_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var amenity = (Amenity)chk.Tag;

            if (chk.Checked)
            {
                listAmenity.Add(amenity);
            }
            else
            {
                listAmenity.Remove(amenity);
            }
            ShowPrice();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var q = db.AmenitiesTickets.Where(x => x.TicketID == currentTicket.ID);
                db.AmenitiesTickets.DeleteAllOnSubmit(q);
                db.SubmitChanges();

                foreach (var a in listAmenity)
                {
                    AmenitiesTicket at = new AmenitiesTicket()
                    {
                        AmenityID = a.ID,
                        TicketID = currentTicket.ID,
                        Price = a.Price
                    };
                    db.AmenitiesTickets.InsertOnSubmit(at);
                    db.SubmitChanges();
                }

                Clear(false);
                MessageBox.Show("Data Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
