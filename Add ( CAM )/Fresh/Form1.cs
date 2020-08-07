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
        DataClasses1DataContext db = new DataClasses1DataContext();

        public Form1()
        {
            InitializeComponent();
        }

        private void AddMember_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            db = new DataClasses1DataContext();

            dataGridView1.DataSource = db.tblMembers.ToList().Select(x => new
            {
                Code = x.MemberCode,
                Name = x.MemberName,
                Gender = x.tblGender.GenderTitle,
                DateOfBirth = x.MemberDoB,
                Phone = x.MemberTel,
                Address = x.MemberAddress,
                MemberType = x.tblMemberType.MemberTypeTitle,
                Disabled = x.MemberDisabled.Value,
                obj = x
            }).ToList();
            dataGridView1.Columns["obj"].Visible = false;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var obj = (tblMember)dataGridView1.Rows[i].Cells["obj"].Value;

                if (obj.MemberDisabled.Value)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please choose the data in the datagridview first");
                    return;
                }
                var member = (tblMember)dataGridView1.CurrentRow.Cells["obj"].Value;
                member.MemberDisabled = !member.MemberDisabled;
                db.SubmitChanges();

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMemberForm form = new AddMemberForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}
