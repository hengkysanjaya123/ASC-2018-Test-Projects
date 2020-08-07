using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fresh
{
    public partial class CheckInSystemForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        Schedule currentSchedule;
        string textCaptcha;
        List<CheckInData> listCheckInData = new List<CheckInData>()
        {
            new CheckInData() { ScheduleID = 1, SeatName = "1A", StatusSeat = "Check-In", CabinType = "First Class"}
        };
        Dictionary<string, string> listSeatName = new Dictionary<string, string>();

        Panel panelFirst, panelBusiness, panelEconomy;
        Button currentButton;

        public CheckInSystemForm()
        {
            InitializeComponent();
        }

        public string GenerateString()
        {
            string data = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890$@!*&)";
            Random rand = new Random();
            string result = "";

            for (int i = 0; i < 5; i++)
            {
                result += data[rand.Next(0, data.Length)];
            }
            return result;
        }

        public Image GenerateCaptcha()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            var g = Graphics.FromImage(bmp);
            Random rand = new Random();

            textCaptcha = GenerateString();
            var width = pictureBox1.Width / textCaptcha.Length;

            g.FillRectangle(Brushes.White, 0, 0, pictureBox1.Width, pictureBox1.Height);
            for (int i = 0; i < 5; i++)
            {
                Matrix m = new Matrix();
                var size = g.MeasureString(textCaptcha[i].ToString(), new Font("Microsoft Sans Serif", 15f));

                m.RotateAt(rand.Next(-30, 31), new PointF((i * width + width / 2) - size.Width / 2, pictureBox1.Height / 2 - size.Height / 2));
                g.Transform = m;
                g.DrawString(textCaptcha[i].ToString(), new Font("Microsoft Sans Serif", 15f), Brushes.Black, new PointF(i * width + width / 2 - size.Width / 2, pictureBox1.Height / 2 - size.Height / 2));
                g.ResetTransform();
            }
            return bmp;
        }

        private void CheckInSystemForm_Load(object sender, EventArgs e)
        {
            listSeatName.Add("A", "B");
            listSeatName.Add("B", "A");
            listSeatName.Add("C", "D");
            listSeatName.Add("D", "C");

            var q = db.Schedules.ToList().Select(x => new
            {
                Display = $"{x.Date.ToString("dd/MM/yyyy")}, {(x.Date + x.Time).ToString("HH:mm")}, {x.Route.Airport.IATACode}-{x.Route.Airport1.IATACode}",
                Value = x
            }).ToList();
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = q;

            pictureBox1.Image = GenerateCaptcha();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = GenerateCaptcha();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //if (textBox1.Text.Trim() == "")
            //{
            //    MessageBox.Show("Please input the captcha");
            //    return;
            //}

            //if (!textBox1.Text.Equals(textCaptcha))
            //{
            //    MessageBox.Show("Invalid Captcha");
            //    return;
            //}

            currentSchedule = (Schedule)comboBox1.SelectedValue;
            var totalFirstSeat = currentSchedule.Aircraft.TotalSeats - currentSchedule.Aircraft.EconomySeats - currentSchedule.Aircraft.BusinessSeats;
            var totalBusinesSeat = currentSchedule.Aircraft.BusinessSeats;
            var totalEconomySeat = currentSchedule.Aircraft.EconomySeats;

            panelFirst = LoadSeat(totalFirstSeat);
            panelBusiness = LoadSeat(totalBusinesSeat);
            panelEconomy = LoadSeat(totalEconomySeat);

            ChangeAllColor();
        }

        private void ChangeAllColor()
        {
            ChangeButtonColor(panelFirst, "First Class");
            ChangeButtonColor(panelBusiness, "Business");
            ChangeButtonColor(panelEconomy, "Economy");
        }

        private void ChangeButtonColor(Panel pnl1, string CabinType)
        {
            foreach (Button btn in pnl1.Controls.OfType<Button>())
            {
                string text = btn.Text;
                var numberSeat = text.Substring(0, text.Length - 1);
                var charSeat = text.LastOrDefault().ToString();

                var checkNext = pnl1.Controls.OfType<Button>().Where(x => x.Text == $"{numberSeat}{listSeatName[charSeat]}").FirstOrDefault();
                if (checkNext == null)
                {
                    btn.BackColor = Color.Orange; // Empty
                    btn.Tag = new SeatData()
                    {
                        CabinType = CabinType,
                        StatusSeat = "EmptyAlone"
                    };
                }
                else
                {
                    var q = listCheckInData.Where(x => x.SeatName == text && x.CabinType == CabinType).FirstOrDefault();
                    var q2 = listCheckInData.Where(x => x.SeatName == $"{numberSeat}{listSeatName[charSeat]}" && x.CabinType == CabinType).FirstOrDefault();

                    if (q == null && q2 == null)
                    {
                        btn.BackColor = Color.Peru; // Dual Empty
                        btn.Tag = new SeatData()
                        {
                            CabinType = CabinType,
                            StatusSeat = "Dual Empty"
                        };
                    }
                    else
                    {
                        if (q != null)
                        {
                            btn.BackColor = Color.DeepSkyBlue; // Check-In
                            btn.Tag = new SeatData()
                            {
                                CabinType = CabinType,
                                StatusSeat = "Check-In"
                            };
                        }
                        else if (q == null)
                        {
                            btn.BackColor = Color.Orange; // Empty
                            btn.Tag = new SeatData()
                            {
                                CabinType = CabinType,
                                StatusSeat = "Empty"
                            };
                        }
                    }
                }

                btn.MouseClick += Btn_MouseClick;
            }
        }

        private void checkInSingleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = (SeatData)currentButton.Tag;
            List<Ticket> listTickets = new List<Ticket>();
            Form2 form = new Form2(data, currentSchedule, ref listTickets);
            if (form.ShowDialog() == DialogResult.OK)
            {
                listCheckInData.Add(new CheckInData()
                {
                    CabinType = data.CabinType,
                    StatusSeat = data.StatusSeat,
                    ScheduleID = currentSchedule.ID,
                    SeatName = currentButton.Text,
                    Ticket = listTickets[0]
                });
                ChangeAllColor();
            }
        }

        private void checkInDualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Btn_MouseClick(object sender, MouseEventArgs e)
        {
            changeSeatToolStripMenuItem.Visible = true;
            checkInDualToolStripMenuItem.Visible = true;
            checkInSingleToolStripMenuItem.Visible = true;

            var btn = (Button)sender;
            currentButton = btn;
            var data = (SeatData)btn.Tag;

            if (data.StatusSeat != "Check-In")
            {
                changeSeatToolStripMenuItem.Visible = false;

                if (data.StatusSeat == "EmptyAlone" || data.StatusSeat == "Empty")
                {
                    checkInDualToolStripMenuItem.Visible = false;
                }
            }
            else
            {
                checkInDualToolStripMenuItem.Visible = false;
                checkInSingleToolStripMenuItem.Visible = false;
            }

            contextMenuStrip1.Show(btn, e.X, e.Y);
        }

        private Panel LoadSeat(int total)
        {
            Panel pnl1 = new Panel();
            pnl1.Margin = new Padding(0);
            pnl1.BackColor = Color.LightBlue;
            pnl1.AutoSize = true;

            var totalRow1 = (int)Math.Ceiling((double)total / 4);
            var totalLeft1 = total % 4;
            var row1 = 0;


            char name = 'A';
            for (int j = 0; j < totalLeft1; j++)
            {
                Button btn = new Button();
                btn.Location = new System.Drawing.Point(j * 75 + 15 + ((j + 1) > 2 ? 75 : 0), row1 * 23 + 0);
                btn.Name = "button3";
                btn.Size = new System.Drawing.Size(75, 23);
                btn.TabIndex = 0;
                btn.Text = $"{row1 + 1}{name}";
                btn.UseVisualStyleBackColor = true;
                

                pnl1.Controls.Add(btn);
                name++;
            }

            for (int i = 1; i < totalRow1; i++)
            {
                name = 'A';
                for (int j = 0; j < 4; j++)
                {
                    Button btn = new Button();
                    btn.Location = new System.Drawing.Point(j * 75 + 15 + ((j + 1) > 2 ? 75 : 0), i * 23 + 0);
                    btn.Name = "button3";
                    btn.Size = new System.Drawing.Size(75, 23);
                    btn.TabIndex = 0;
                    btn.Text = $"{i + 1}{name}";
                    btn.UseVisualStyleBackColor = true;

                    pnl1.Controls.Add(btn);
                    name++;
                }
            }
            flowLayoutPanel1.Controls.Add(pnl1);
            return pnl1;
        }
    }

    public class SeatData
    {
        public string StatusSeat { get; set; }
        public string CabinType { get; set; }
    }

    public class CheckInData
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int ScheduleID { get; set; }
        public Ticket Ticket { get; set; }
        public string SeatName { get; set; }
        public string StatusSeat { get; set; }
        public string CabinType { get; set; }
    }
}
