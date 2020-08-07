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
    public partial class Form2 : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        List<string> listAnswer = new List<string>()
        {
            "Outstanding", "Very Good", "Good", "Adequate", "Needs Improvement", "Poor", "Don't know"
        };
        List<string> listChartType = new List<string>()
        {
            "Column", "Line",
        };

        List<string> listQuestion = new List<string>()
        {
            "Q1 - Please rate our aircraft flown on AMONIC Airlines",
            "Q2 - How would you rate our flight attendants",
            "Q3 - How would you rate our inflight entertainment",
            "Q4 - Please rate the ticket price for the trip you are taking"
        };

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            var q = db.Airports;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "ID";
            comboBox1.DataSource = q;

            comboBox2.DataSource = listChartType;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Question", "Question");
            for (int i = 0; i < listAnswer.Count; i++)
            {
                dataGridView1.Columns.Add(listAnswer[i], listAnswer[i]);
            }

            for (int i = 0; i < listQuestion.Count; i++)
            {
                dataGridView1.Rows.Add(listQuestion[i]);
            }

            var q = listSurveyDetail.Where(x => x.Arrival == comboBox1.Text).ToList();

            for (int j = 1; j < dataGridView1.Columns.Count; j++)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    var q2 = q.Where(x => x.Question == dataGridView1.Rows[i].Cells[0].Value.ToString()
                                && x.Answer == dataGridView1.Columns[j].HeaderText
                            ).Count();
                    dataGridView1.Rows[i].Cells[j].Value = q2;
                }
            }
            LoadChart();
        }

        public void LoadChart()
        {
            chart1.Series.Clear();

            for (int j = 1; j < dataGridView1.Columns.Count; j++)
            {
                Series s = chart1.Series.Add(dataGridView1.Columns[j].HeaderText);

                if (comboBox2.SelectedValue.ToString() == listChartType[0])
                {
                    s.ChartType = SeriesChartType.Column;
                }
                else if (comboBox2.SelectedValue.ToString() == listChartType[1])
                {
                    s.ChartType = SeriesChartType.Line;
                }

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    s.Points.AddXY(dataGridView1.Rows[i].Cells[0].Value.ToString().Substring(0, 2), dataGridView1.Rows[i].Cells[j].Value);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void button2_Click(object sender, EventArgs e)
        {   
            this.Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            SurveyForm form = new SurveyForm();
            form.Show();
        }
    }
}
