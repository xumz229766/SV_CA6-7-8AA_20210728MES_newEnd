using HalconDotNet;
using desay.Vision;
using PylonLiveViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Toolkit.Helpers;
using System.Windows.Forms;
using Vision.HalconAps;
using ViewWindow.Model;
namespace desay
{
    public partial class frmGrab : Form
    {
        BaslerCam m_Cam=null;
        LightControl lightControl = null;
        HObject hPylonImage = new HObject();
        private VisionClass m_Vision = null;
        int imgWidth, imgHeight;

        public HObject ho_Image = null;//原图像
        public HWindow hv_WindowHandle = new HWindow();//窗口句柄
        public HWndCtrl viewController; //显示
        public ROIController roiController;

        public frmGrab()
        {
            InitializeComponent();
        }
        public frmGrab(BaslerCam cam,LightControl ctsLightControl) : this()
        {
            if (cam != null)
                m_Cam = cam;
            if (ctsLightControl != null)
                lightControl = ctsLightControl;
        }


        #region 按钮操作
        private void tkbExposure_Scroll(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            int value = tkbExposure.Value;
            m_Cam.SetExposureTime(value);
            txtExposure.Text = Convert.ToString(value);
        }

        private void tkbGain_Scroll(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            int value = tkbGain.Value;
            m_Cam.SetGain(value);
            txtGain.Text = Convert.ToString(value);
        }

        private void tkbLight_Scroll(object sender, EventArgs e)
        {
            if (lightControl == null) return;
            int value = tkbLight.Value;
            lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, value);
            txtLight.Text = Convert.ToString(value);
        }

        private void txtExposure_TextChanged(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            int value = 0;
            int.TryParse(txtExposure.Text, out value);
            if (value < tkbExposure.Minimum || value > tkbExposure.Maximum)
                return;
            tkbExposure.Value = value;
            m_Cam.SetExposureTime(value);
        }

        private void txtGain_TextChanged(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            int value = 0;
            int.TryParse(txtGain.Text, out value);
            if (value < tkbGain.Minimum || value > tkbGain.Maximum)
                return;
            tkbGain.Value = value;
            m_Cam.SetGain(value);
        }

        private void txtLight_TextChanged(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            int value = 0;
            int.TryParse(txtLight.Text, out value);
            if (value < tkbLight.Minimum || value > tkbLight.Maximum)
                return;
            tkbLight.Value = value;

        }

        private void SetBottonEnable(bool flag)
        {
            btnStart.Enabled = flag;
            btnStop.Enabled = !flag;
            btnSingleFrame.Enabled = flag;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            m_Cam.StopGrabbing();
            m_Cam.SetFreerun();
            if (m_Cam.StartGrabbing())
                SetBottonEnable(false);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            m_Cam.StopGrabbing();
            SetBottonEnable(true);
        }

        private void btnSingleFrame_Click(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            m_Cam.StopGrabbing();
            m_Cam.SetSoftwareTrigger();//第一步设置软触发模式
            m_Cam.StartGrabbing();//第二部采集开始
            m_Cam.SendSoftwareExecute();//第三部触发
        }

        private void btnLoadImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dg = new OpenFileDialog();
            dg.Multiselect = true;//该值确定是否可以选择多个文件
            dg.Title = "请选择文件夹";
            dg.Filter = "图像文件(*.bmp;*.jpg; *.jpg; *.jpeg; *.gif; *.png)| *.jpg; *.jpeg; *.gif; *.png;*.bmp";
            if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dg.FileName;
                m_Vision.ReadImg(file);
                //hPylonImage.Dispose();
                //hPylonImage = m_Vision.ho_Image.Clone();
            }
            //imgHeight = m_Vision.hv_Height.I;
            //imgWidth = m_Vision.hv_Width.I;
            //m_Vision.hv_WindowHandle.SetPart(0, 0, imgHeight, imgWidth);
            //m_Vision.hv_WindowHandle.DispObj(hPylonImage);
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog dg = new SaveFileDialog();
            dg.Title = "请选择文件夹";
            dg.Filter = "图像文件(*.bmp;*.jpg; *.jpeg; *.gif; *.png)| *.bmp;*.jpg; *.jpeg; *.gif; *.png";
            if (dg.ShowDialog() == DialogResult.OK)
            {
                string file = dg.FileName;
                string filter = dg.Filter;
                m_Vision.SaveImg(file, filter);
            }
            MessageBox.Show("图像保存成功！");
        }

        private void btnSaveParam_Click(object sender, EventArgs e)
        {
            if (m_Cam == null) return;
            VisionProductData.Instance.nExposure = tkbExposure.Value;
            VisionProductData.Instance.nGain = tkbGain.Value;
            VisionProductData.Instance.nLight = tkbLight.Value;
            VisionProductData.Instance.bSaveImg = chkSaveImg.Checked;
            VisionProductData.Instance.nLightChanel = 1;
            SerializerManager<VisionProductData>.Instance.Save(VisionMarking.VisionFileName, VisionProductData.Instance);
           
        }

        #endregion

        private void frmGrab_Load(object sender, EventArgs e)
        {
            m_Vision = new VisionClass(hWindowControl1);

            if (m_Cam != null)
            {
                if (VisionMarking.IsCameraOpen /*|| m_Cam.OpenCam()*/)           // 打开并初始化相机
                {
                    m_Cam.eventProcessImage += processHImage1;         // 注册halcon显示回调函数
                    //m_Cam.numWindowIndex = 1;                          // 相机1 PYLON自带窗体显示窗体序号

                    txtModelName.Text = VisionMarking.ModelName;
                    txtSN.Text = VisionMarking.SN;
                    txtUserID.Text = VisionMarking.UserID;
                    tkbExposure.Minimum =(int) m_Cam.minExposureTime;
                    tkbExposure.Maximum = (int)m_Cam.maxExposureTime;
                    tkbGain.Minimum = (int)m_Cam.minGain;
                    tkbGain.Maximum = (int)m_Cam.maxGain;

                    chkSaveImg.Checked = VisionProductData.Instance.bSaveImg;
                    tkbExposure.Value = VisionProductData.Instance.nExposure;
                    m_Cam.SetExposureTime(VisionProductData.Instance.nExposure);
                    txtExposure.Text = Convert.ToString(VisionProductData.Instance.nExposure);
                    tkbGain.Value = VisionProductData.Instance.nGain;
                    m_Cam.SetGain(VisionProductData.Instance.nGain);
                    txtGain.Text = Convert.ToString(VisionProductData.Instance.nGain);
                    tkbLight.Value = VisionProductData.Instance.nLight;
                    txtLight.Text = Convert.ToString(VisionProductData.Instance.nLight);
                }
                else
                {
                    //SetBottonEnable(false);
                    //btnStop.Enabled = false;
                }
            }


         

        }

        private void frmGrab_FormClosed(object sender, FormClosedEventArgs e)
        {
            hPylonImage.Dispose();
        }

        private void frmGrab_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_Cam == null) return;
            m_Cam.StopGrabbing();
            m_Cam.SetSoftwareTrigger();
            m_Cam.StartGrabbing();//第二部采集开始
            m_Cam.eventProcessImage -= processHImage1;         // 注册halcon显示回调函数
        }

        // 相机1 halcon窗体显示图像
        private void processHImage1(HObject ho_Image)
        {          
            //HTuple hv_Width = new HTuple();
            //HTuple hv_Height = new HTuple();
            //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
        

            m_Vision.UpdataImg(ho_Image);
            try
            {
                HTuple hv_width = 0, hv_height = 0;
                HOperatorSet.GetImageSize(m_Vision.ho_Image, out hv_width, out hv_height);
                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "yellow");
                HOperatorSet.DispLine(hWindowControl1.HalconWindow, hv_height / 2, 0, hv_height / 2, hv_width);
                HOperatorSet.DispLine(hWindowControl1.HalconWindow, 0, hv_width / 2, hv_height, hv_width / 2);
            }
            catch (Exception ex) { }
            //hPylonImage.Dispose();
        }

    }
}
