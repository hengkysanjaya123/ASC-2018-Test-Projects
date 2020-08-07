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
    public partial class Form1 : core
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void viewResultsSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryForm form = new SummaryForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(form);
        }

        private void viewDetailedResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DetailedForm form = new DetailedForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(form);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            foreach (var a in db.Surveys)
            {
                if (a.Gender == "F")
                {
                    a.Gender = "Female";
                }
                else if (a.Gender == "M")
                {
                    a.Gender = "Male";
                }
            }
            db.SubmitChanges();
        }
    }
}
