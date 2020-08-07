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
    public partial class SurveyForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<string> listQuestion = new List<string>()
        {
            "Q1 - Please rate our aircraft flown on AMONIC Airlines",
            "Q2 - How would you rate our flight attendants",
            "Q3 - How would you rate our inflight entertainment",
            "Q4 - Please rate the ticket price for the trip you are taking"
        };

        public SurveyForm()
        {
            InitializeComponent();
        }

        private void SurveyForm_Load(object sender, EventArgs e)
        {
            List<int> listAge = new List<int>();
            for (int i = 10; i <= 60; i++)
            {
                listAge.Add(i);
            }
            comboBox1.DataSource = listAge;

            var q = db.Airports;
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "ID";
            comboBox2.DataSource = q;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("Please choose gender");
                return;
            }

            var gender = radioButton1.Checked ? radioButton1.Text : radioButton2.Text;
            var q1 = panel8.Controls.OfType<RadioButton>().Where(x => x.Checked).FirstOrDefault();
            var q2 = panel9.Controls.OfType<RadioButton>().Where(x => x.Checked).FirstOrDefault();
            var q3 = panel10.Controls.OfType<RadioButton>().Where(x => x.Checked).FirstOrDefault();
            var q4 = panel11.Controls.OfType<RadioButton>().Where(x => x.Checked).FirstOrDefault();

            if (q1 == null)
            {
                MessageBox.Show("Please Answer for question number 1");
                return;
            }

            if (q2 == null)
            {
                MessageBox.Show("Please Answer for question number 2");
                return;
            }

            if (q3 == null)
            {
                MessageBox.Show("Please Answer for question number 3");
                return;
            }

            if (q4 == null)
            {
                MessageBox.Show("Please Answer for question number 4");
                return;
            }

            Confirmation form = new Confirmation();
            if (form.ShowDialog() == DialogResult.OK)
            {
                listSurveyDetail.Add(new SurveyDetail()
                {
                    Gender = gender,
                    Age = (int)comboBox1.SelectedValue,
                    Question = listQuestion[0],
                    Answer = q1.Tag.ToString(),
                    Arrival = comboBox2.Text
                });

                listSurveyDetail.Add(new SurveyDetail()
                {
                    Gender = gender,
                    Age = (int)comboBox1.SelectedValue,
                    Question = listQuestion[1],
                    Answer = q2.Tag.ToString(),
                    Arrival = comboBox2.Text
                });

                listSurveyDetail.Add(new SurveyDetail()
                {
                    Gender = gender,
                    Age = (int)comboBox1.SelectedValue,
                    Question = listQuestion[2],
                    Answer = q3.Tag.ToString(),
                    Arrival = comboBox2.Text
                });

                listSurveyDetail.Add(new SurveyDetail()
                {
                    Gender = gender,
                    Age = (int)comboBox1.SelectedValue,
                    Question = listQuestion[3],
                    Answer = q4.Tag.ToString(),
                    Arrival = comboBox2.Text
                });

                Form2 form2 = new Form2();
                this.Close();
                form2.Show();
            }
        }
    }

    public class SurveyDetail
    {
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Arrival { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
