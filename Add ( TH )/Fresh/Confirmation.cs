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

using System.Speech.Synthesis;

namespace Fresh
{
    public partial class Confirmation : core
    {
        string dataCaptcha;

        public Confirmation()
        {
            InitializeComponent();
        }

        public string GenerateString()
        {
            string data = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890$@!*&";
            Random rand = new Random();
            int nChar = rand.Next(4, 7);
            string result = "";

            for (int i = 0; i < nChar; i++)
            {
                result += data[rand.Next(0, data.Length)];
            }
            return result;
        }

        public Image GenerateCaptcha()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            var g = Graphics.FromImage(bmp);

            dataCaptcha = GenerateString();

            List<string> listFamily = new List<string>()
            {
                "Arial Black", "Arial", "Calibri", "Times New Roman", "Microsoft Sans Serif"
            };

            var width = pictureBox1.Width / dataCaptcha.Length;

            Random rand = new Random();

            var hatchstyle = (HatchStyle[])Enum.GetValues(typeof(HatchStyle));

            g.FillRectangle(new HatchBrush(hatchstyle[rand.Next(0, hatchstyle.Length)], Color.LightSlateGray, Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);

            for (int i = 0; i < dataCaptcha.Length; i++)
            {
                var font = new Font(listFamily[rand.Next(0, listFamily.Count)], 25f);
                var size = g.MeasureString(dataCaptcha[i].ToString(), font);
                g.DrawString(dataCaptcha[i].ToString(), font, Brushes.Black, new PointF(i * width + (width / 2) - (size.Width / 2), (pictureBox1.Height / 2) - size.Height / 2));
            }
            return bmp;
        }

        private void Confirmation_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = GenerateCaptcha();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = GenerateCaptcha();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var read = string.Join(" ", dataCaptcha.ToCharArray());

            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Speak(read.Replace("!", "Exclamation Mark"));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Captcha must be filled");
                return;
            }

            if (!textBox1.Text.Equals(dataCaptcha))
            {
                MessageBox.Show("Captcha incorrect");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
