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
    public partial class Form1 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        string captchaString;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var q = db.Schedules.ToList().Where(x => x.Confirmed).Select(x => new
            {
                Display = $"{x.Date.ToString("dd/MM/yyyy")},{(x.Date + x.Time).ToString("HH:mm")},{x.Route.Airport.IATACode}-{x.Route.Airport1.IATACode}",
                Value = x
            }).ToList();
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = q;

            pictureBox1.Image = GenerateCaptcha();
        }

        public string GenerateString()
        {
            string data = "ABCDEFGHIJKLMNOPQRSTUVVWXYZabcdefghijklmnopqrstuvvwxyz1234567890$@!*&)";
            string result = "";
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                result += data[rand.Next(0, data.Length)];
            }
            return result;
        }

        public Image GenerateCaptcha()
        {
            var bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            var g = Graphics.FromImage(bmp);

            captchaString = GenerateString();
            var width = pictureBox1.Width / 5;
            Random rand = new Random();

            for (int i = 0; i < 5; i++)
            {
                Matrix m = new Matrix();
                Font font = new Font("Arial", 9f);
                var size = g.MeasureString(captchaString[i].ToString(), font);

                m.RotateAt(rand.Next(-30, 31), new PointF(i * width + width / 2 - size.Width / 2, pictureBox1.Height / 2 - size.Height / 2));
                g.Transform = m;
                g.DrawString(captchaString[i].ToString(), font, Brushes.Black, new PointF(-size.Width / 2, -size.Height / 2));
                g.ResetTransform();
            }

            return bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = GenerateCaptcha();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(captchaString))
            {
                MessageBox.Show("Captcha incorrect");
                return;
            }
        }
    }
}
