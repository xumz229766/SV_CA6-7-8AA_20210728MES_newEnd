using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using desay;
using desay.Flow;
using HalconDotNet;
namespace desay.Vision
{
    public partial class frmRealPosCalib : Form
    {
        VisionClass m_Vision;
        RobotWork m_robotWork;
        public frmRealPosCalib()
        {
            InitializeComponent();
        }
        public frmRealPosCalib(VisionClass vision,RobotWork robotWork) : this()
        {
            //m_vision = vision;
            m_robotWork = robotWork;
        }

        private void frmRealPosCalib_Load(object sender, EventArgs e)
        {
            m_Vision = new VisionClass(hWindowControl1);
        }

        private void btnRestDgvData_Click(object sender, EventArgs e)
        {
            this.dgvImgPos.Rows.Clear();
            this.dgvRealPos.Rows.Clear();
        }

        private void btnGetRobotPos_Click(object sender, EventArgs e)
        {
            if (cbManuGetPos.Checked)//手动获取
            {
                if (dgvRealPos.Rows.Count >= 15) return;               
                dgvRealPos.Rows.Add(new object[] { dgvRealPos.RowCount, RealX.Text, RealY.Text });
                dgvImgPos.Rows.Add(new object[] { dgvImgPos.RowCount, ImageX.Text, ImageY.Text });
            }
            else//自动获取
            {
                if (dgvRealPos.Rows.Count >= 15) return;
                //OperateResult<int> Rst1 = m_PLC.ReadInt32('D' + Config.Instance.nReadXDataPos.ToString());
                //OperateResult<int> Rst2 = m_PLC.ReadInt32('D' + Config.Instance.nReadRDataPos.ToString());
                dgvRealPos.Rows.Add(new object[] { dgvRealPos.RowCount, 0.0, 0.0 });
                dgvImgPos.Rows.Add(new object[] { dgvImgPos.RowCount, 0.000, 0.000 });
            }

        }
        public bool UpdataPoints()
        {
            try
            {
                int iIntex = dgvImgPos.RowCount;
                for (int i = 0; i < iIntex-1; i++)
                {

                    //X
                    m_Vision.ImagePoints[i, 0] = Convert.ToDouble(this.dgvImgPos.Rows[i].Cells[1].Value);
                    //Y
                    m_Vision.ImagePoints[i, 1] = Convert.ToDouble(this.dgvImgPos.Rows[i].Cells[2].Value);

                    m_Vision.AxisPoints[i, 0] = Convert.ToDouble(this.dgvRealPos.Rows[i].Cells[1].Value);
                    m_Vision.AxisPoints[i, 1] = Convert.ToDouble(this.dgvRealPos.Rows[i].Cells[2].Value);
                }
                return true;
            }
            catch { return false; }

        }

        private void btnCaculate_Click(object sender, EventArgs e)
        {
            UpdataPoints();
            if (!m_Vision.ImageToAxisMat2D(dgvImgPos.RowCount-1)) MessageBox.Show("标定失败");
            else {
                lbImageRealParam.Text =$"{m_Vision.h_HomMat2D[0].D},{m_Vision.h_HomMat2D[1].D},{m_Vision.h_HomMat2D[2].D}" ;
                lbImageRealParam2.Text = $"{m_Vision.h_HomMat2D[3].D},{m_Vision.h_HomMat2D[4].D},{m_Vision.h_HomMat2D[5].D}";
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
           
            try
            {

                HTuple qqx, qqy;
                m_Vision.ImageToAxis(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), out qqx, out qqy);
                lblVerify.Text = $"验证：机械坐标X:{qqx},机械坐标Y:{qqy}";
            }
            catch (Exception ex)
            { }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            VisionProductData.Instance.HomMat2D = m_Vision.h_HomMat2D;
            VisionProductData.Instance = System.Toolkit.Helpers.SerializerManager<VisionProductData>.Instance.Load(VisionMarking.VisionFileName);
        }
    }
}
