using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.ComponentModel.DataAnnotations;

namespace Fresh
{
    public partial class core : Form
    {
        PrivateFontCollection pfc = new PrivateFontCollection();

        public core()
        {
            InitializeComponent();
        }

        // function form load
        private void core_Load(object sender, EventArgs e)
        {
            try
            {
                pfc.AddFontFile(Application.StartupPath + @"\texgyreadventor-regular.otf");
                container.Font = new Font(pfc.Families[0], 9f);
            }
            catch
            {
                try
                {
                    pfc.AddFontFile(Application.StartupPath + @"\verdana.ttf");
                    container.Font = new Font(pfc.Families[0], 9f);
                }
                catch
                {
                }
            }

            container.BackColor = ColorTranslator.FromHtml("#196aa6");
            container.ForeColor = ColorTranslator.FromHtml("#fffacb");
            SetStyle(container);
        }

        // function to set component style
        public void SetStyle(Control ctrl)
        {
            foreach (Control a in ctrl.Controls)
            {
                if (a is Button)
                {
                    var btn = (Button)a;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Height = 29;
                    btn.ForeColor = ColorTranslator.FromHtml("#132b4f");

                    var text = btn.Text.ToLower();
                    if (text.Contains("delete") || text.Contains("cancel"))
                    {
                        btn.BackColor = Color.Red;
                    }
                    else
                    {
                        btn.BackColor = ColorTranslator.FromHtml("#f79420");
                    }
                }
                else if (a is ComboBox)
                {
                    var combo = (ComboBox)a;
                    combo.DropDownStyle = ComboBoxStyle.DropDownList;
                }

                if (a.HasChildren)
                {
                    SetStyle(a);
                }
            }
        }

        // function to Validate not null
        public bool Validation(Control ctrl)
        {
            var q = ctrl.Controls.OfType<TextBox>().Where(x => x.Text.Trim() == "").Count();
            if (q > 0)
            {
                return false;
            }

            var q2 = ctrl.Controls.OfType<MaskedTextBox>().Where(x => x.Text.Trim() == "").Count();
            if (q2 > 0)
            {
                return false;
            }

            return true;
        }

        // function to hash string
        public string Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            byte[] byt = md5.Hash;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byt.Length; i++)
            {
                sb.Append(byt[i].ToString("x2"));
            }
            return sb.ToString();
        }

        // function to validate email
        public bool IsValidEmail(string text)
        {
            try
            {
                bool result = new EmailAddressAttribute().IsValid(text);

                if (result)
                {
                    if (text.LastOrDefault() == '.')
                    {
                        return false;
                    }
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        public decimal GetCabinPrice(List<Schedule> listSchedule, CabinType cabinType)
        {
            decimal total = 0;

            foreach (var a in listSchedule)
            {
                if (cabinType.ID == 1)
                {
                    total += Math.Floor(a.EconomyPrice);
                }
                else if (cabinType.ID == 2)
                {
                    total += Math.Floor(1.35m * Math.Floor(a.EconomyPrice));
                }
                else if (cabinType.ID == 3)
                {
                    total += Math.Floor(1.3m * Math.Floor(1.35m * Math.Floor(a.EconomyPrice)));
                }
            }
            return total;
        }
    }
}
