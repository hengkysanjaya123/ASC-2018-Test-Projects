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

namespace Fresh
{
    public partial class Form1 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        Schedule currentSchedule;
        Dictionary<string, string> listSeatName = new Dictionary<string, string>();
        List<CheckInData> listCheckInData = new List<CheckInData>();
        Button currentButton;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listSeatName.Add("A", "B");
            listSeatName.Add("B", "A");
            listSeatName.Add("C", "D");
            listSeatName.Add("D", "C");

            var q = db.Schedules.ToList().Where(x => x.Confirmed)
                    .Select(x => new
                    {
                        Display = $"{x.Date.ToString("dd/MM/yyyy")},{(x.Date + x.Time).ToString("HH:mm")}, {x.Route.Airport.IATACode}-{x.Route.Airport1.IATACode}",
                        Value = x
                    }).ToList();
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = q;
        }

        public void ClearButton()
        {
            panelFirstClass.Controls.Clear();
            panelBusiness.Controls.Clear();
            panelEconomy.Controls.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearButton();
            currentSchedule = (Schedule)comboBox1.SelectedValue;

            LoadData();
        }

        private void LoadData()
        {
            ClearButton();
            var totalFirstSeat = currentSchedule.Aircraft.TotalSeats - currentSchedule.Aircraft.EconomySeats - currentSchedule.Aircraft.BusinessSeats;
            var totalBusinessSeat = currentSchedule.Aircraft.BusinessSeats;
            var totalEconomySeat = currentSchedule.Aircraft.EconomySeats;

            panelFirstClass.AutoSize = false;
            panelBusiness.AutoSize = false;
            panelEconomy.AutoSize = false;

            LoadSeat(totalFirstSeat, "First Class", panelFirstClass);
            LoadSeat(totalBusinessSeat, "Business", panelBusiness);
            LoadSeat(totalEconomySeat, "Economy", panelEconomy);

            panelFirstClass.AutoSize = true;
            panelBusiness.AutoSize = true;
            panelEconomy.AutoSize = true;

            LoadButtonColor(panelFirstClass, "First Class");
            LoadButtonColor(panelBusiness, "Business");
            LoadButtonColor(panelEconomy, "Economy");

            LoadInfo();
            LoadChart();
        }

        public void LoadSeat(int total, string cabinType, Panel pnl)
        {
            var totalRow = (int)Math.Ceiling((double)total / 4);
            var totalLeft = total % 4;
            int row = 0;
            char name = 'A';

            for (int i = 0; i < totalLeft; i++)
            {
                Button btn = new Button();
                btn.FlatStyle = FlatStyle.Flat;
                btn.Location = new Point(i * 75 + 15 + (i > 1 ? 75 : 0), 0);
                btn.Name = "button3";
                btn.Size = new System.Drawing.Size(75, 23);
                btn.TabIndex = 0;
                btn.Text = $"{row + 1}{name}";
                btn.UseVisualStyleBackColor = true;
                btn.Tag = new SeatData()
                {
                    CabinType = cabinType,
                    Column = i,
                    Row = row,
                    StatusSeat = ""
                };
                btn.MouseClick += Btn_MouseClick;

                pnl.Controls.Add(btn);
                name++;
            }

            for (int i = 1; i < totalRow; i++)
            {
                name = 'A';
                for (int j = 0; j < 4; j++)
                {
                    Button btn = new Button();
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Location = new Point(j * 75 + 15 + (j > 1 ? 75 : 0), i * 23);
                    btn.Name = "button3";
                    btn.Size = new System.Drawing.Size(75, 23);
                    btn.TabIndex = 0;
                    btn.Text = $"{i + 1}{name}";
                    btn.UseVisualStyleBackColor = true;
                    btn.Tag = new SeatData()
                    {
                        CabinType = cabinType,
                        Column = j,
                        Row = i,
                        StatusSeat = ""
                    };
                    btn.MouseClick += Btn_MouseClick;

                    pnl.Controls.Add(btn);
                    name++;
                }
            }
            flowLayoutPanel1.Controls.Add(pnl);
        }

        private void Btn_MouseClick(object sender, MouseEventArgs e)
        {
            var btn = (Button)sender;
            var data = (SeatData)btn.Tag;

            currentButton = btn;

            changeSeatToolStripMenuItem.Visible = false;
            dualCheckInToolStripMenuItem.Visible = false;
            singleCheckInToolStripMenuItem.Visible = false;

            if (data.StatusSeat == "Check-In")
            {
                changeSeatToolStripMenuItem.Visible = true;
            }
            else
            {
                if (data.StatusSeat == "Dual Empty")
                {
                    dualCheckInToolStripMenuItem.Visible = true;
                    singleCheckInToolStripMenuItem.Visible = true;
                }
                else if (data.StatusSeat == "Empty")
                {
                    singleCheckInToolStripMenuItem.Visible = true;
                }
            }

            contextMenuStrip1.Show(btn, new Point(e.X, e.Y));
        }

        public void LoadButtonColor(Panel pnl, string cabinType)
        {
            foreach (Button btn in pnl.Controls.OfType<Button>())
            {
                var data = (SeatData)btn.Tag;
                string text = btn.Text;
                var numberSeat = text.Substring(0, text.Length - 1);
                var charSeat = text.LastOrDefault().ToString();

                var checkNext = pnl.Controls.OfType<Button>().Where(x => x.Text == $"{numberSeat}{listSeatName[charSeat]}").FirstOrDefault();
                if (checkNext == null)
                {
                    btn.BackColor = Color.Orange; // Empty
                    data.StatusSeat = "Empty";
                }
                else
                {
                    var q = listCheckInData.Where(x => x.SeatName == text && x.CabinType == cabinType).FirstOrDefault();
                    var q2 = listCheckInData.Where(x => x.SeatName == $"{numberSeat}{listSeatName[charSeat]}" && x.CabinType == cabinType).FirstOrDefault();

                    if (q == null && q2 == null)
                    {
                        btn.BackColor = Color.IndianRed; // Dual Empty
                        data.StatusSeat = "Dual Empty";
                    }
                    else
                    {
                        if (q != null)
                        {
                            btn.BackColor = Color.DeepSkyBlue; // Check-In
                            data.StatusSeat = "Check-In";
                        }
                        else if (q == null)
                        {
                            btn.BackColor = Color.Orange; // Empty
                            data.StatusSeat = "Empty";
                        }
                    }

                }

                btn.Tag = data;
                //btn.MouseClick += Btn_MouseClick;
            }
        }

        public void LoadInfo()
        {
            label4.Text = panelFirstClass.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat != "Check-In").Count().ToString();
            label5.Text = (panelFirstClass.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat == "Dual Empty").Count() / 2).ToString();


            label6.Text = panelBusiness.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat != "Check-In").Count().ToString();
            label7.Text = (panelBusiness.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat == "Dual Empty").Count() / 2).ToString();


            label8.Text = panelEconomy.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat != "Check-In").Count().ToString();
            label9.Text = (panelEconomy.Controls.OfType<Button>()
                        .Where(x => (x.Tag as SeatData).StatusSeat == "Dual Empty").Count() / 2).ToString();
        }

        public void LoadChart()
        {
            // Columnnya 0 dan 1 ( Mulai dari 0 )

            var q = flowLayoutPanel1.Controls.OfType<Panel>()
                        .Select(x => x.Controls.OfType<Button>())
                        .SelectMany(x => x)
                        .Select(x => x.Tag as SeatData);
            var left = q.Where(x => x.Column < 2 && x.StatusSeat == "Check-In").Count();
            var right = q.Where(x => x.Column > 1 && x.StatusSeat == "Check-In").Count();

            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Titles.Add("");
            chart1.Titles[0].Text = "Number of Checked-In Seat";
            chart1.Titles[0].Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);

            Series s = chart1.Series.Add("Left");
            s.Points.AddXY("", left);

            Series s2 = chart1.Series.Add("Right");
            s2.Points.AddXY("", right);

            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private void dualCheckInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = (SeatData)currentButton.Tag;

            List<Ticket> listTicket = new List<Ticket>();
            Form2 form = new Form2(listCheckInData, data, currentSchedule, ref listTicket, "Dual");
            if (form.ShowDialog() == DialogResult.OK)
            {
                string text = currentButton.Text;
                var numberSeat = text.Substring(0, text.Length - 1);
                var charSeat = text.LastOrDefault().ToString();

                var q = flowLayoutPanel1.Controls.OfType<Panel>()
                        .SelectMany(x => x.Controls.OfType<Button>())
                        .Select(x => new
                        {
                            Tag = x.Tag as SeatData,
                            x
                        })
                        .Where(x => x.Tag.CabinType == data.CabinType
                                && x.x.Text == $"{numberSeat}{listSeatName[charSeat]}"
                        ).FirstOrDefault().x;

                data.Ticket = listTicket[0];

                var data2 = (SeatData)q.Tag;
                data2.Ticket = listTicket[1];

                currentButton.Tag = data;
                q.Tag = data2;

                listCheckInData.Add(new CheckInData()
                {
                    CabinType = data.CabinType,
                    ScheduleID = currentSchedule.ID,
                    SeatName = currentButton.Text,
                    StatusSeat = data.StatusSeat,
                    Ticket = listTicket[0]
                });

                listCheckInData.Add(new CheckInData()
                {
                    CabinType = data2.CabinType,
                    ScheduleID = currentSchedule.ID,
                    SeatName = q.Text,
                    StatusSeat = data2.StatusSeat,
                    Ticket = listTicket[1]
                });
                LoadData();
            }
        }

        private void singleCheckInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = (SeatData)currentButton.Tag;
            List<Ticket> listTicket = new List<Ticket>();
            Form2 form = new Form2(listCheckInData, data, currentSchedule, ref listTicket, "Single");
            if (form.ShowDialog() == DialogResult.OK)
            {
                string text = currentButton.Text;

                data.Ticket = listTicket[0];


                currentButton.Tag = data;

                listCheckInData.Add(new CheckInData()
                {
                    CabinType = data.CabinType,
                    ScheduleID = currentSchedule.ID,
                    SeatName = currentButton.Text,
                    StatusSeat = data.StatusSeat,
                    Ticket = listTicket[0]
                });
                LoadData();
            }
        }

        private void changeSeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = (SeatData)currentButton.Tag;

            var listAvailable = flowLayoutPanel1.Controls.OfType<Panel>()
                    .SelectMany(x => x.Controls.OfType<Button>())
                    .Select(x => new
                    {
                        Tag = x.Tag as SeatData,
                        x
                    })
                    .Where(x => x.Tag.CabinType == data.CabinType
                                && x.Tag.StatusSeat != "Check-In"
                    ).Select(x => x.x.Text).ToList();

            ChangeSeatName = currentButton.Text;
            ChangeSeatForm form = new ChangeSeatForm(data, listAvailable);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var q = listCheckInData.Where(x => x.CabinType == data.CabinType
                            && x.SeatName == currentButton.Text
                    ).FirstOrDefault();
                q.SeatName = ChangeSeatName;
                LoadData();
            }
        }
    }

    public class SeatData
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string CabinType { get; set; }
        public string StatusSeat { get; set; }
        public Ticket Ticket { get; set; }
    }

    public class CheckInData
    {
        public int ScheduleID { get; set; }
        public Ticket Ticket { get; set; }
        public string SeatName { get; set; }
        public string StatusSeat { get; set; }
        public string CabinType { get; set; }
    }
}
