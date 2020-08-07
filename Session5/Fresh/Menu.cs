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
    public partial class Menu : core
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PurchaseAmenities form = new PurchaseAmenities();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AmenitiesReport form = new AmenitiesReport();
            form.ShowDialog();
        }
    }
}
