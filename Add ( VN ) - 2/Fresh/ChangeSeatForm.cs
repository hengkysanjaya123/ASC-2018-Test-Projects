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
    public partial class ChangeSeatForm : core
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        SeatData sd;

        List<string> listName = new List<string>();

        public ChangeSeatForm(SeatData sd, List<string> listName)
        {
            InitializeComponent();
            this.sd = sd;
            this.listName = listName;
        }

        private void ChangeSeatForm_Load(object sender, EventArgs e)
        {
            label2.Text = ChangeSeatName;
            comboBox1.DataSource = listName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeSeatName = comboBox1.SelectedValue.ToString();
            this.DialogResult = DialogResult.OK;
        }
    }
}
