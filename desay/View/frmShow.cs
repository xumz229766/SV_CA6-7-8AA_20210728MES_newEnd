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
    public partial class frmShow : Form
    {
        public frmShow()
        {
            InitializeComponent();
        }
        #region 数据表格初始化
        public string getdata(int datanum)
        {
            try
            {
                double tol = Config.Instance.ProductTotal == 0 ? 0 : (double)(datanum * 1.0 / Config.Instance.ProductTotal);
                return (tol).ToString("0.00%");
            }
            catch { return "0.00%"; }
        }
        private void InitdgvResultShows()
        {
            Config.Instance.AllAAProductNgTotal = Config.Instance.CleanMesLockNgTotal + Config.Instance.GlueWbNgTotal_LightNG + Config.Instance.GlueWbNgTotal_ParticeNG +
                Config.Instance.GlueLocationNgTotal + Config.Instance.GlueFindNgTotal + Config.Instance.AALightNgTotal + Config.Instance.AALightLossFrameNgTotal +
                Config.Instance.AAMoveNgTotal + Config.Instance.AAOCNgTotal + Config.Instance.AATILT_TUNENgTotal + Config.Instance.AASerchNgTotal +
                Config.Instance.AASNNgTotal + Config.Instance.AAUVAfterNgTotal + Config.Instance.AAUVBeforeNgTotal + Config.Instance.NoneNgTotal;
            this.dgvResultShow.Rows.Clear();
            //in a real scenario, you may need to add different rows

            dgvResultShow.Rows.Add(new object[] {
                    "总数",
                    Config.Instance.ProductTotal.ToString(),
                    getdata(Config.Instance.ProductTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "OK",
                    Config.Instance.AllAAProductOkTotal.ToString(),
                      getdata(Config.Instance.AllAAProductOkTotal)
                });

            dgvResultShow.Rows.Add(new object[] {
                    "互锁NG",
                    Config.Instance.CleanMesLockNgTotal.ToString(),
                    getdata(Config.Instance.CleanMesLockNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "白板点亮",
                    Config.Instance.GlueWbNgTotal_LightNG.ToString(),
                      getdata(Config.Instance.GlueWbNgTotal_LightNG)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "白板脏污",
                    Config.Instance.GlueWbNgTotal_ParticeNG.ToString(),
                      getdata(Config.Instance.GlueWbNgTotal_ParticeNG)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "定位NG",
                    Config.Instance.GlueLocationNgTotal.ToString(),
                      getdata(Config.Instance.GlueLocationNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "识别NG",
                    Config.Instance.GlueFindNgTotal.ToString(),
                      getdata(Config.Instance.GlueFindNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "点亮NG",
                    Config.Instance.AALightNgTotal.ToString(),
                     getdata(Config.Instance.AALightNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "点亮丢帧",
                    Config.Instance.AALightLossFrameNgTotal.ToString(),
                     getdata(Config.Instance.AALightLossFrameNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "运控NG",
                    Config.Instance.AAMoveNgTotal.ToString(),
                      getdata(Config.Instance.AAMoveNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "搜索NG",
                    Config.Instance.AASerchNgTotal.ToString(),
                      getdata(Config.Instance.AASerchNgTotal)
                });

            dgvResultShow.Rows.Add(new object[] {
                    "OCNG",
                    Config.Instance.AAOCNgTotal.ToString(),
                      getdata(Config.Instance.AAOCNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "倾斜NG",
                    Config.Instance.AATILT_TUNENgTotal.ToString(),
                      getdata(Config.Instance.AATILT_TUNENgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "UV前NG",
                    Config.Instance.AAUVBeforeNgTotal.ToString(),
                      getdata(Config.Instance.AAUVBeforeNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "UV后NG",
                    Config.Instance.AAUVAfterNgTotal.ToString(),
                      getdata(Config.Instance.AAUVAfterNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "条码NG",
                    Config.Instance.AASNNgTotal.ToString(),
                      getdata(Config.Instance.AASNNgTotal)
                });
            dgvResultShow.Rows.Add(new object[] {
                    "其余NG",
                    Config.Instance.NoneNgTotal.ToString(),
                      getdata(Config.Instance.NoneNgTotal)
                });
        }
        #endregion

        private void frmShow_Load(object sender, EventArgs e)
        {
            InitdgvResultShows();
            nudOKnum.Value = Config.Instance.AllAAProductOkTotal;
            nudMesLockNGnum.Value = Config.Instance.CleanMesLockNgTotal;
            nudWbLightNGnum.Value = Config.Instance.GlueWbNgTotal_LightNG;
            nudWbParticeNGnum.Value = Config.Instance.GlueWbNgTotal_ParticeNG;
            nudLocationNGnum.Value = Config.Instance.GlueLocationNgTotal;
            nudGlueFindNGnum.Value = Config.Instance.GlueFindNgTotal;
            nudAALightNGnum.Value = Config.Instance.AALightNgTotal;
            nudAALightLoseFrameNGnum.Value = Config.Instance.AALightLossFrameNgTotal;

            nudAAMotionNGnum.Value = Config.Instance.AAMoveNgTotal;
            nudAAOCNGnum.Value = Config.Instance.AAOCNgTotal;
            nudAAQinXieNGnum.Value = Config.Instance.AATILT_TUNENgTotal;
            nudAASerchNGnum.Value = Config.Instance.AASerchNgTotal;
            nudAASNNGnum.Value = Config.Instance.AASNNgTotal;
            nudAAUVAfterNGnum.Value = Config.Instance.AAUVAfterNgTotal;
            nudAAUVbeforeNGnum.Value = Config.Instance.AAUVBeforeNgTotal;
            nudNullNGnum.Value = Config.Instance.NoneNgTotal;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Config.Instance.AllAAProductOkTotal = (int) nudOKnum.Value ;

            Config.Instance.CleanMesLockNgTotal= (int)  nudMesLockNGnum.Value ;
            Config.Instance.GlueWbNgTotal_LightNG= (int)  nudWbLightNGnum.Value;
            Config.Instance.GlueWbNgTotal_ParticeNG = (int)  nudWbParticeNGnum.Value ;
            Config.Instance.GlueLocationNgTotal = (int)  nudLocationNGnum.Value  ;
            Config.Instance.GlueFindNgTotal = (int)  nudGlueFindNGnum.Value;
            Config.Instance.AALightNgTotal= (int)  nudAALightNGnum.Value;
            Config.Instance.AALightLossFrameNgTotal= (int)  nudAALightLoseFrameNGnum.Value ;

            Config.Instance.AAMoveNgTotal = (int)  nudAAMotionNGnum.Value ;
            Config.Instance.AAOCNgTotal= (int)  nudAAOCNGnum.Value ;
            Config.Instance.AATILT_TUNENgTotal = (int)  nudAAQinXieNGnum.Value ;
            Config.Instance.AASerchNgTotal = (int)  nudAASerchNGnum.Value ;
            Config.Instance.AASNNgTotal= (int)  nudAASNNGnum.Value ;
            Config.Instance.AAUVAfterNgTotal= (int)  nudAAUVAfterNGnum.Value ;
            Config.Instance.AAUVBeforeNgTotal = (int)  nudAAUVbeforeNGnum.Value ;
            Config.Instance.NoneNgTotal = (int)  nudNullNGnum.Value ;

            Config.Instance.AllAAProductNgTotal = Config.Instance.CleanMesLockNgTotal + Config.Instance.GlueWbNgTotal_LightNG + Config.Instance.GlueWbNgTotal_ParticeNG +
                Config.Instance.GlueLocationNgTotal + Config.Instance.GlueFindNgTotal + Config.Instance.AALightNgTotal + Config.Instance.AALightLossFrameNgTotal +
                Config.Instance.AAMoveNgTotal + Config.Instance.AAOCNgTotal + Config.Instance.AATILT_TUNENgTotal + Config.Instance.AASerchNgTotal +
                Config.Instance.AASNNgTotal + Config.Instance.AAUVAfterNgTotal + Config.Instance.AAUVBeforeNgTotal + Config.Instance.NoneNgTotal;
            InitdgvResultShows();
        }
    }
}
