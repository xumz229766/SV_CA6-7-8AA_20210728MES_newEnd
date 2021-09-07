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
using System.Toolkit.Helpers;
namespace desay.Vision
{
    public partial class frmGlueFindValue : Form
    {
        public frmGlueFindValue()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否保存点胶识别高级参数", "", MessageBoxButtons.YesNo)) return;
            GlueFindParam.Instance.proj_match_points_ransac_Tolenrance=(int) nudTolenrance.Value ;
            GlueFindParam.Instance.abs_diff_image_value = (double)nudabsdisimage.Value ;
            GlueFindParam.Instance.pow_image_value=(double) nudpowimage.Value ;
            GlueFindParam.Instance.median_image_value=(int) nudmedianimage.Value ;
            GlueFindParam.Instance.fill_up_shape= (int)nudfillupvalue.Value;
            GlueFindParam.Instance.PH_sigama = (double)nudPH_Sigama.Value;
            GlueFindParam.Instance.PH_Threshold = (int)nudPH_Threshold.Value;
            GlueFindParam.Instance.ScaleImg = cbPowImg.Checked;
            GlueFindParam.Instance.Gmin = (int)nudGmin.Value;
            GlueFindParam.Instance.sub1 = (double)nudSub1.Value;
            GlueFindParam.Instance.subT = (int)nudSubT.Value;
            SerializerManager<GlueFindParam>.Instance.Save(AppConfig.ConfigGlueFindName, GlueFindParam.Instance);
        }

        private void frmGlueFindValue_Load(object sender, EventArgs e)
        {
            nudTolenrance.Value = GlueFindParam.Instance.proj_match_points_ransac_Tolenrance;
            nudabsdisimage.Value = (decimal)GlueFindParam.Instance.abs_diff_image_value;
            nudpowimage.Value = (decimal)GlueFindParam.Instance.pow_image_value;
            nudmedianimage.Value = (decimal)GlueFindParam.Instance.median_image_value;
            nudfillupvalue.Value = (decimal)GlueFindParam.Instance.fill_up_shape;
            nudPH_Sigama.Value = (decimal)GlueFindParam.Instance.PH_sigama;
            nudPH_Threshold.Value= (decimal)GlueFindParam.Instance.PH_Threshold ;
            cbPowImg.Checked= GlueFindParam.Instance.ScaleImg ;
            nudGmin.Value= GlueFindParam.Instance.Gmin;
            nudSub1.Value = (decimal)GlueFindParam.Instance.sub1 ;
            nudSubT.Value= (decimal)GlueFindParam.Instance.subT;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
