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
        List<string> listCombo = new List<string>()
        {
            "Number of Users by Offices",
            "Number of Roles by Offices"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = listCombo;
            label2.Text = "";
            label3.Text = "";
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.Series.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            label3.Text = "";
            DateTime before = DateTime.Now;

            chart1.Series.Clear();
            string selected = comboBox1.SelectedValue.ToString();
            if (selected == listCombo[0])
            {
                var q = db.Offices.Select(x => new
                {
                    Key = x.Title,
                    Total = x.Users.Count
                });

                Series s = chart1.Series.Add("");
                s.ChartType = SeriesChartType.Doughnut;

                int i = 0;
                foreach (var a in q)
                {
                    s.Points.AddXY(a.Key, a.Total);
                    s.Points[i].ToolTip = a.Total.ToString();
                    s.Points[i].LabelToolTip = a.Total.ToString();
                    i++;
                }

                var max = q.Max(x => x.Total);
                var q2 = q.Where(x => x.Total == max).Select(x => x.Key).ToArray();
                label2.Text = $"Office (s) with the most number of users : \n{string.Join(",", q2)}";
            }
            else if (selected == listCombo[1])
            {
                chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

                var q = db.Offices.Select(x => new
                {
                    Office = x.Title,
                    Administrator = x.Users.Where(y => y.RoleID == 1).Count(),
                    User = x.Users.Where(y => y.RoleID == 2).Count(),
                    TotalUser = x.Users.Count
                }).Select(x => new
                {
                    x.Office,
                    x.Administrator,
                    x.User,
                    Percentage = x.TotalUser == 0 ? 0 : x.Administrator / (double)x.TotalUser
                });

                Series s1 = chart1.Series.Add("Administrator");
                foreach (var a in q)
                {
                    s1.Points.AddXY(a.Office, a.Administrator);
                }

                Series s2 = chart1.Series.Add("User");
                foreach (var a in q)
                {
                    s2.Points.AddXY(a.Office, a.User);
                }

                var max = q.Max(x => x.Percentage);
                var q2 = q.Where(x => x.Percentage == max).Select(x => x.Office).ToArray();

                label3.Text = $"Office (s) with the highest percentage of administrators : \n{string.Join(",", q2.ToArray())}";
            }

            var substract = DateTime.Now - before;
            label1.Text = $"Time taken to generate graph : {((int)substract.TotalMinutes)} min {substract.Seconds} sec {substract.Milliseconds} msec";
        }
    }
}
