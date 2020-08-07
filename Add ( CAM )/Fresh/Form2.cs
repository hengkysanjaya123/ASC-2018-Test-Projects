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
    public partial class Form2 : Form
    {   


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            UserControl1 uc = new UserControl1();
            uc.callBack += Uc_callBack;
            uc.Dock = DockStyle.Fill;
            panel1.Controls.Add(uc);
        }

        private void Uc_callBack()
        {
            MessageBox.Show("Message from parent");
        }
    }
}
