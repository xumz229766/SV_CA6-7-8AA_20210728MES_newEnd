using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using desay.ProductData;
namespace desay
{
    public partial class frmjobnum : Form
    {
        public frmjobnum()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Trim().Length == Config.Instance.SVjobNumberLength1|| txtPassword.Text.Trim().Length == Config.Instance.SVjobNumberLength2)
            {
                Marking.SvJobNumber = txtPassword.Text.Trim();
                this.Close();
            }
            else
            {
                MessageBox.Show("员工账号长度校验不一致，请重新输入");
            }
        }

        private void frmjobnum_Load(object sender, EventArgs e)
        {
            this.txtPassword.Focus();
        }
    }
}
