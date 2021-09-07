using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HalconDotNet;
using System.Toolkit;
using System.Toolkit.Helpers;
using ViewWindow.Model;
//using ImageAcquition;
using System.IO;
using desay.ProductData;
using desay.Vision;
using PylonLiveViewer;
namespace desay
{
    public partial class frmModelBuild : Form
    {
        private readonly Action _Refreshing;
        private Type m_Type;
        private VisionClass m_Vision = null;
        private BaslerCam m_Camera = null;
        private DbVisSetting dbVisSetting = null;
        private VisCameraCalib m_VisCamera;
        HObject region;
        private string m_ModelName;
        private ROICircle CircleROI = new ROICircle();
        private ROICircle PointROI = new ROICircle();
        private ROICircle2 ModelROI = new ROICircle2();
        private ROICircle2 GlueROI = new ROICircle2();
        private ROICircle2 GrayROI = new ROICircle2();
        private ROICircle2 PolarROI = new ROICircle2();
        LightControl lightControl = null;
        bool bShowShiZhi = false;//显示十字
        public frmModelBuild()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 模板创建构造函数
        /// </summary>
        /// <param name="vision">true:为上相机</param>
        /// <param name="fileName"></param>
        /// <param name="ModelName"></param>
        public frmModelBuild(BaslerCam camera,LightControl lightc, DbVisSetting dbvisSetting, string ModelName, Type type, Action Refreshing = null) : this()
        {
            try
            {
                m_Vision = new VisionClass(hWindowControl1);
                m_Camera = camera;
                lightControl = lightc;
                dbVisSetting = dbvisSetting;

                m_ModelName = ModelName;
                m_Type = type;
                _Refreshing = Refreshing;

                m_VisCamera = Relationship.Instance.CameraCalib;

                if (m_Camera != null)
                {
                    if (VisionMarking.IsCameraOpen /*|| m_Cam.OpenCam()*/)           // 打开并初始化相机
                    {
                        m_Camera.eventProcessImage += processHImage1;         // 注册halcon显示回调函数
                       
                    }
                }
                nudLight.Value = dbVisSetting.LightControlvalue;
                tkbLight.Value = dbVisSetting.LightControlvalue;
                if (lightControl!=null)
                {
                    lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, dbVisSetting.LightControlvalue);
                }
            }
            catch { MessageBox.Show("加载相机失败"); }

        }
        private void frmModelBuild_Load(object sender, EventArgs e)
        {
            //dbVisSetting = SerializerManager<DbVisSetting>.Instance.Load(filePathName);
            dbVisSetting.strID = m_ModelName;
            // var strmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{dbVisSetting.strModelPath}");

            var strmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{Config.Instance.CurrentProductType}{dbVisSetting.strID}Model.sbm");
            dbVisSetting.strModelPath = $"{Config.Instance.CurrentProductType}{dbVisSetting.strID}Model.sbm";
            m_Vision.ReadShapeModel(strmodelpath);

            tbpPole.Parent = null;//角度隐藏
            tpgPoint.Parent = null;//隐藏
            tbgPN.Parent = null;//隐藏
            if (m_ModelName == "GlueVisionParam")
            {
                dbVisSetting.isUseGlue = true;
                tbpMode.Parent=null;//隐藏
                tbpCricle.Parent = null;//隐藏
            }
            else
            {
                dbVisSetting.isUseGlue = false;
                tbgGlue.Parent = null;//隐藏
            }
            //'none', 'interpolation', 'least_squares', 'least_squares_high', 'least_squares_very_high',
            // 'ignore_color_polarity', 'ignore_global_polarity', 'ignore_local_polarity', 'use_polarity'
            cbxModelMetric.Items.Add("ignore_color_polarity");
            cbxModelMetric.Items.Add("ignore_global_polarity");
            cbxModelMetric.Items.Add("ignore_local_polarity");
            cbxModelMetric.Items.Add("use_polarity");

            cbxModelSubPixel.Items.Add("none");
            cbxModelSubPixel.Items.Add("interpolation");
            cbxModelSubPixel.Items.Add("least_squares");
            cbxModelSubPixel.Items.Add("least_squares_high");
            cbxModelSubPixel.Items.Add("least_squares_very_high");

            cbxCircleTransition.Items.Add("all");
            cbxCircleTransition.Items.Add("positive");
            cbxCircleTransition.Items.Add("negative");

            cbxCircleSelect.Items.Add("all");
            cbxCircleSelect.Items.Add("first");
            cbxCircleSelect.Items.Add("last");
            chxIsUerROI.Checked = dbVisSetting.IsModelUse;
            chxModelDisplay.Checked = dbVisSetting.IsModelDisplay;
            ModelDisplay();
            chxCircleDisplay.Checked = dbVisSetting.IsCircleDisplay;
            CircleDisplay();
            chxIsUsePoint.Checked = dbVisSetting.IsPointUse;
            chxPointDisplay.Checked = dbVisSetting.IsPointDisplay;
            lblPointrow.Text = dbVisSetting.PointCenterRow.ToString();
            lblPointcol.Text = dbVisSetting.PointCenterCol.ToString();
            lblPointr.Text = dbVisSetting.PointRadius.ToString();
            tbrPointMin.Value = (int)dbVisSetting.PointGrayMin;
            lblPointMin.Text = dbVisSetting.PointGrayMin.ToString();
            tbrPointMax.Value = (int)dbVisSetting.PointGrayMax;
            lblPointMax.Text = dbVisSetting.PointGrayMax.ToString();
            chxIsUsePN.Checked = dbVisSetting.IsGraylUse;
            chxGrayDisplay.Checked = dbVisSetting.IsGrayDisplay;
            ndnGrayMinTolerance.Value = (decimal)dbVisSetting.GrayMinTolerance;
            ndnGrayMaxTolerance.Value = (decimal)dbVisSetting.GrayMaxTolerance;

            chxPolarDisplay.Checked = dbVisSetting.IsPolarDisplay;
            chxPolarUse.Checked = dbVisSetting.IsPolarlUse;
            trackBar1.Value = (int)dbVisSetting.PolarminGray;
            lblPolarminGray.Text = trackBar1.Value.ToString();
            trackBar2.Value = (int)dbVisSetting.PolarmaxGray;
            lblPolarmaxGray.Text = trackBar2.Value.ToString();
            ndnminLenght.Value = (int)dbVisSetting.minRegionLenght;
            ndnmaxLenght.Value = (int)dbVisSetting.maxRegionLenght;
            ndnminArea.Value = (int)dbVisSetting.minPolarArea;
            ndnmaxArea.Value = (int)dbVisSetting.maxPolarArea;

            chxIsNGimageSave.Checked = dbVisSetting.NgImageSave;
            chxIsOKimageSave.Checked = dbVisSetting.OkImageSave;

            //new  add
            nudGlueWidth.Value = dbVisSetting.GlueWidth;
            nudGlueRadius.Value = dbVisSetting.GlueInnerCircle;

            nudGlueOutValue.Value = dbVisSetting.glueOverflowOutter;
            nudGlueInValue.Value = dbVisSetting.glueOverflowInner;
            nudGlueOutNoglueValue.Value = dbVisSetting.glueLackOutter;
            nudGlueInNoglueValue.Value = dbVisSetting.glueLackInner;
            nudGlueOffsetValue.Value = dbVisSetting.glueOffset;

            nudOpenCloseValue.Value = (decimal)dbVisSetting.kernel;
            nudGlueThresholdValue.Value = (decimal)dbVisSetting.tol;
            nudGlueAreaMinValue.Value = dbVisSetting.area;

            tabControl2.SelectedTab = tbpMode;
            displayTool(tabControl2.SelectedTab);

            //隐藏 西威用不上 打点这些

            timer1.Enabled = true;
        }
        private void frmModelBuild_FormClosing(object sender, FormClosingEventArgs e)
        {
            HOperatorSet.GenEmptyObj(out region);
            region.Dispose();
            if (_Refreshing != null) _Refreshing();
            try
            {
                if (m_Camera != null)
                {
                    if (VisionMarking.IsCameraOpen /*|| m_Cam.OpenCam()*/)
                    {
                        m_Camera.StopGrabbing();
                        m_Camera.SetSoftwareTrigger();
                        m_Camera.StartGrabbing();//第二部采集开始
                        m_Camera.eventProcessImage -= processHImage1;         // 注册halcon显示回调函数
                    }
                }
            }
            catch { }
        }

        // 相机1 halcon窗体显示图像
        private void processHImage1(HObject ho_Image)
        {
            //HTuple hv_Width = new HTuple();
            //HTuple hv_Height = new HTuple();
            //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);


            m_Vision.UpdataImg(ho_Image);

            //hPylonImage.Dispose();
        }
        private void btnReadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dg = new OpenFileDialog();
            dg.Multiselect = true;//该值确定是否可以选择多个文件
            dg.Title = "请选择文件夹";
            dg.Filter = "图像文件(*.bmp;*.jpg; *.jpg; *.jpeg; *.gif; *.png)| *.jpg; *.jpeg; *.gif; *.png;*.bmp";
            if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dg.FileName;
                m_Vision.ReadImg(file);
                displayTool(tabControl2.SelectedTab);

                if (bShowShiZhi )
                {
                    try
                    {
                        HTuple hv_width = 0, hv_height = 0;
                        HOperatorSet.GetImageSize(m_Vision.ho_Image, out hv_width, out hv_height);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "yellow");
                        HOperatorSet.DispLine(hWindowControl1.HalconWindow, hv_height / 2, 0, hv_height / 2, hv_width);
                        HOperatorSet.DispLine(hWindowControl1.HalconWindow, 0, hv_width / 2, hv_height, hv_width / 2);
                    }
                    catch (Exception ex) { }
                }
            }

        }
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog dg = new SaveFileDialog();
            dg.Title = "请选择文件夹";
            dg.Filter = "图像文件(*.bmp;*.jpg; *.jpg; *.jpeg; *.gif; *.png)| *.jpg; *.jpeg; *.gif; *.png;*.bmp";
            if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dg.FileName;
                m_Vision.SaveImg(file);


            }
        }

        private void btnCameraTrigger_Click(object sender, EventArgs e)
        {
            m_Camera.StopGrabbing();
            m_Camera.SetSoftwareTrigger();//第一步设置软触发模式
            m_Camera.StartGrabbing();//第二部采集开始
            m_Camera.SendSoftwareExecute();//第三部触发
            Thread.Sleep(300);
            m_Vision.UpdataImg(m_Camera.ho_Image);
            if (bShowShiZhi)
            {
                try
                {
                    HTuple hv_width = 0, hv_height = 0;
                    HOperatorSet.GetImageSize(m_Vision.ho_Image, out hv_width, out hv_height);
                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "yellow");
                    HOperatorSet.DispLine(hWindowControl1.HalconWindow, hv_height / 2, 0, hv_height / 2, hv_width);
                    HOperatorSet.DispLine(hWindowControl1.HalconWindow, 0, hv_width / 2, hv_height, hv_width / 2);
                }
                catch (Exception ex) { }
            }
        }
        private void btnSaveROI_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r1 = 0.0;
            var r2 = 0.0;
            GetModelROI(out row, out col, out r1, out r2);
            dbVisSetting.IsModelUse = chxIsUerROI.Checked;
            dbVisSetting.IsModelDisplay = chxModelDisplay.Checked;
            dbVisSetting.ModelCenterRow = Math.Round(row, 3);
            dbVisSetting.ModelCenterCol = Math.Round(col, 3);
            dbVisSetting.ModelRadius1 = Math.Round(r1, 3);
            dbVisSetting.ModelRadius2 = Math.Round(r2, 3);
            dbVisSetting.ShapeModel.StartAngle = (double)ndnModelStartAngle.Value;
            dbVisSetting.ShapeModel.EndAngle = (double)ndnModelEndAngle.Value;
            dbVisSetting.ShapeModel.MaxOverlap = (double)ndnModelMaxOverlap.Value;
            dbVisSetting.ShapeModel.NumMatches = (int)ndnModelNumMatches.Value;
            dbVisSetting.ShapeModel.NumLevels = (int)ndnModelNumLevels.Value;
            dbVisSetting.ShapeModel.MinScore = (double)ndnModelMinScore.Value;
            dbVisSetting.ShapeModel.Greediness = (double)ndnModelGreediness.Value;
            dbVisSetting.ShapeModel.Metric = cbxModelMetric.Text == "" ? "use_polarity" : cbxModelMetric.Text;
            dbVisSetting.ShapeModel.SubPixel = cbxModelSubPixel.Text == "" ? "least_squares_high" : cbxModelSubPixel.Text;
            HOperatorSet.GenEmptyObj(out region);
            region.Dispose();
            GetCircle2ROI(out region);
            m_Vision.CreateShapeModel(m_Vision.ho_Image, region, dbVisSetting.ShapeModel, out m_Vision.ModelId);
            if (m_Type == typeof(DbModelParam))
            {
                System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
                var strmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{Config.Instance.CurrentProductType}{dbVisSetting.strID}Model.sbm");
                dbVisSetting.strModelPath = $"{Config.Instance.CurrentProductType}{dbVisSetting.strID}Model.sbm";
                m_Vision.WriteShapeModel(strmodelpath);
            }
            
            else MessageBox.Show("参数类型不正确，无法保存模板!");
        }
        private void btnCheckROI_Click(object sender, EventArgs e)
        {
            RetShapeModel ret = new RetShapeModel();
            m_Vision.FindShapeModel(m_Vision.ho_Image, dbVisSetting.ShapeModel, true, out ret);
            lblcRow.Text = ret.CenterRow.ToString("0.000");
            lblcCol.Text = ret.CenterCol.ToString("0.000");
            lblca.Text = ret.Angle.ToString("0.000");
            lblcs.Text = ret.Score.ToString("0.0");
            lblmodeltime.Text = m_Vision.modeltime.ToString() + "ms";
        }
        private void btnCircleSave_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r = 0.0;
            GetCircleROI(out row, out col, out r);
            dbVisSetting.IsCircleDisplay = chxCircleDisplay.Checked;
            dbVisSetting.FindCircle.CenterRow = Math.Round(row, 3);
            dbVisSetting.FindCircle.CenterCol = Math.Round(col, 3);
            dbVisSetting.FindCircle.Radius = Math.Round(r, 3);
            dbVisSetting.FindCircle.StartAngle = (double)ndnCircleStartAngle.Value;
            dbVisSetting.FindCircle.EndAngle = (double)ndnCircleEndAngle.Value;
            dbVisSetting.FindCircle.CalliperLength = (double)ndnCircleCalliperLength.Value;
            dbVisSetting.FindCircle.CalliperWidth = (double)ndnCircleCalliperWidth.Value;
            dbVisSetting.FindCircle.CalliperNum = (int)ndnCircleCalliperNum.Value;
            dbVisSetting.FindCircle.TargetNum = (int)ndnCircleTargetNum.Value;
            dbVisSetting.FindCircle.MinScore = (double)ndnCircleMinScore.Value;
            dbVisSetting.FindCircle.Sigma = (double)ndnCircleSigma.Value == 0 ? 0.4 : (double)ndnCircleSigma.Value;
            dbVisSetting.FindCircle.Threshold = trbThreshold.Value;
            dbVisSetting.FindCircle.Transition = cbxCircleTransition.Text == "" ? "all" : cbxCircleTransition.Text;
            dbVisSetting.FindCircle.Select = cbxCircleSelect.Text == "" ? "all" : cbxCircleSelect.Text;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }
        private void ModelDisplay()
        {
            chxIsUerROI.Checked = dbVisSetting.IsModelUse;
            ndnModelStartAngle.Value = (decimal)dbVisSetting.ShapeModel.StartAngle;
            ndnModelEndAngle.Value = (decimal)dbVisSetting.ShapeModel.EndAngle;
            ndnModelMaxOverlap.Value = (decimal)dbVisSetting.ShapeModel.MaxOverlap;
            ndnModelNumMatches.Value = (decimal)dbVisSetting.ShapeModel.NumMatches;
            ndnModelNumLevels.Value = (decimal)dbVisSetting.ShapeModel.NumLevels;
            ndnModelMinScore.Value = (decimal)dbVisSetting.ShapeModel.MinScore;
            ndnModelGreediness.Value = (decimal)dbVisSetting.ShapeModel.Greediness;
            cbxModelMetric.Text = dbVisSetting.ShapeModel.Metric;
            cbxModelSubPixel.Text = dbVisSetting.ShapeModel.SubPixel;
        }
        private void CircleDisplay()
        {
            ndnCircleStartAngle.Value = (decimal)dbVisSetting.FindCircle.StartAngle;
            ndnCircleEndAngle.Value = (decimal)dbVisSetting.FindCircle.EndAngle;
            ndnCircleCalliperLength.Value = (decimal)dbVisSetting.FindCircle.CalliperLength;
            ndnCircleCalliperWidth.Value = (decimal)dbVisSetting.FindCircle.CalliperWidth;
            ndnCircleCalliperNum.Value = (decimal)dbVisSetting.FindCircle.CalliperNum;
            ndnCircleTargetNum.Value = (decimal)dbVisSetting.FindCircle.TargetNum;
            ndnCircleMinScore.Value = (decimal)dbVisSetting.FindCircle.MinScore;
            ndnCircleSigma.Value = (decimal)dbVisSetting.FindCircle.Sigma;
            trbThreshold.Value = (int)dbVisSetting.FindCircle.Threshold;
            cbxCircleTransition.Text = dbVisSetting.FindCircle.Transition;
            cbxCircleSelect.Text = dbVisSetting.FindCircle.Select;
            lblThreshold.Text = trbThreshold.Value.ToString();
        }
        private void trbThreshold_Scroll(object sender, EventArgs e)
        {
            lblThreshold.Text = trbThreshold.Value.ToString();
        }

        private void tbrPointMin_Scroll(object sender, EventArgs e)
        {
            lblPointMin.Text = tbrPointMin.Value.ToString();
        }

        private void tbrPointMax_Scroll(object sender, EventArgs e)
        {
            lblPointMax.Text = tbrPointMax.Value.ToString();
        }
        private void btnPNSave_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r1 = 0.0;
            var r2 = 0.0;
            GetGrayROI(out row, out col, out r1, out r2);
            dbVisSetting.IsGraylUse = chxIsUsePN.Checked;
            dbVisSetting.IsGrayDisplay = chxGrayDisplay.Checked;
            dbVisSetting.GrayCenterRow = Math.Round(row, 3);
            dbVisSetting.GrayCenterCol = Math.Round(col, 3);
            dbVisSetting.GrayRadius1 = Math.Round(r1, 3);
            dbVisSetting.GrayRadius2 = Math.Round(r2, 3);
            dbVisSetting.GrayMinTolerance = (double)ndnGrayMinTolerance.Value;
            dbVisSetting.GrayMaxTolerance = (double)ndnGrayMaxTolerance.Value;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }
        private void btnPointSave_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r = 0.0;
            GetPointROI(out row, out col, out r);
            dbVisSetting.IsPointUse = chxIsUsePoint.Checked;
            dbVisSetting.IsPointDisplay = chxPointDisplay.Checked;
            dbVisSetting.PointCenterRow = Math.Round(row, 3);
            dbVisSetting.PointCenterCol = Math.Round(col, 3);
            dbVisSetting.PointRadius = Math.Round(r, 3);
            dbVisSetting.PointGrayMin = tbrPointMin.Value;
            dbVisSetting.PointGrayMax = tbrPointMax.Value;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }

        private void btnPointTest_Click(object sender, EventArgs e)
        {
            double regionGray;
            var ret = m_Vision.JudgePointImg(m_Vision.ho_Image, dbVisSetting.PointCenterRow, dbVisSetting.PointCenterCol,
                    dbVisSetting.PointRadius, dbVisSetting.PointGrayMin,
                    dbVisSetting.PointGrayMax, true, out regionGray);
            lblPointOkNg.Text = ret ? "OK" : "NG";
            lblPointGray.Text = regionGray.ToString("F3");
            lblPointTime.Text = m_Vision.graytime.ToString() + "ms";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            lblModelCenterRow.Text = dbVisSetting.ModelCenterRow.ToString();
            lblModelCenterCol.Text = dbVisSetting.ModelCenterCol.ToString();
            lblModelCenterR1.Text = dbVisSetting.ModelRadius1.ToString();
            lblModelCenterR2.Text = dbVisSetting.ModelRadius2.ToString();


            lblGlueCenterX.Text = dbVisSetting.GlueCenterCol.ToString();
            lblGlueCenterY.Text = dbVisSetting.GlueCenterRow.ToString();
            lblGlueR1.Text = dbVisSetting.GlueRadius1.ToString();
            lblGlueR2.Text = dbVisSetting.GlueRadius2.ToString();

            lblCircleCenterRow.Text = dbVisSetting.FindCircle.CenterRow.ToString();
            lblCircleCenterCol.Text = dbVisSetting.FindCircle.CenterCol.ToString();
            lblCircleCenterR.Text = dbVisSetting.FindCircle.Radius.ToString();

            lblPNCenterRow.Text = dbVisSetting.GrayCenterRow.ToString();
            lblPNCenterCol.Text = dbVisSetting.GrayCenterCol.ToString();
            lblPNCenterR1.Text = dbVisSetting.GrayRadius1.ToString();
            lblPNCenterR2.Text = dbVisSetting.GrayRadius2.ToString();

            lblRoiRow.Text = dbVisSetting.PolarRow.ToString();
            lblRoiCol.Text = dbVisSetting.PolarCol.ToString();
            lblRoiR1.Text = dbVisSetting.PolarInnerR.ToString();
            lblRoiR2.Text = dbVisSetting.PolarOuterR.ToString();
            timer1.Enabled = true;
        }
        private void ClearObject(HObject m_object)
        {
            if (m_object != null)
                m_object.Dispose();
        }
        private void displayTool(TabPage tabPage)
        {
            if (tbpMode == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r1 = 0.0;
                var r2 = 0.0;
                if (dbVisSetting.ModelCenterRow == 0 &&
                    dbVisSetting.ModelCenterCol == 0)
                {
                    row = 200;
                    col = 200;
                    r1 = 50;
                    r2 = 100;
                }
                else
                {
                    row = dbVisSetting.ModelCenterRow;
                    col = dbVisSetting.ModelCenterCol;
                    r1 = dbVisSetting.ModelRadius1;
                    r2 = dbVisSetting.ModelRadius2;
                }
                ShowModelROI(row, col, r1, r2);
            }
            else if (tbpCricle == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r = 0.0;
                if (dbVisSetting.FindCircle.CenterRow == 0 &&
                    dbVisSetting.FindCircle.CenterCol == 0)
                {
                    row = 200;
                    col = 200;
                    r = 100;
                }
                else
                {
                    row = dbVisSetting.FindCircle.CenterRow;
                    col = dbVisSetting.FindCircle.CenterCol;
                    r = dbVisSetting.FindCircle.Radius;
                }
                ShowCircleROI(row, col, r);
            }
            else if (tbgGlue == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r1 = 0.0;
                var r2 = 0.0;
                if (dbVisSetting.GlueCenterRow == 0 &&
                    dbVisSetting.GlueCenterCol == 0)
                {
                    row = 200;
                    col = 200;
                    r1 = 50;
                    r2 = 100;
                }
                else
                {
                    row = dbVisSetting.GlueCenterRow;
                    col = dbVisSetting.GlueCenterCol;
                    r1 = dbVisSetting.GlueRadius1;
                    r2 = dbVisSetting.GlueRadius2;
                }
                ShowGlueROI(row, col, r1,r2);
            }
            else if (tpgPoint == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r = 0.0;
                if (dbVisSetting.PointCenterRow == 0 &&
                    dbVisSetting.PointCenterCol == 0)
                {
                    row = 200;
                    col = 200;
                    r = 100;
                }
                else
                {
                    row = dbVisSetting.PointCenterRow;
                    col = dbVisSetting.PointCenterCol;
                    r = dbVisSetting.PointRadius;
                }
                ShowPointROI(row, col, r);
            }
            else if (tbgPN == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r1 = 0.0;
                var r2 = 0.0;
                if (dbVisSetting.GrayCenterRow == 0 &&
                    dbVisSetting.GrayCenterCol == 0)
                {
                    row = 200;
                    col = 200;
                    r1 = 50;
                    r2 = 100;
                }
                else
                {
                    row = dbVisSetting.GrayCenterRow;
                    col = dbVisSetting.GrayCenterCol;
                    r1 = dbVisSetting.GrayRadius1;
                    r2 = dbVisSetting.GrayRadius2;
                }
                ShowGrayROI(row, col, r1, r2);
            }
            else if (tbpPole == tabPage)
            {
                var row = 0.0;
                var col = 0.0;
                var r1 = 0.0;
                var r2 = 0.0;
                if ((dbVisSetting.PolarRow == 0 || dbVisSetting.PolarCol == 0))
                {
                    row = 200;
                    col = 200;
                    r1 = 50;
                    r2 = 100;
                }
                else
                {
                    row = dbVisSetting.PolarRow;
                    col = dbVisSetting.PolarCol;
                    r1 = dbVisSetting.PolarInnerR;
                    r2 = dbVisSetting.PolarOuterR;
                }
                ShowPolarROI(row, col, r1, r2);
            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayTool(tabControl2.SelectedTab);
        }
        private void btnCircleTest_Click(object sender, EventArgs e)
        {
            RetFitCircle ret = new RetFitCircle();
            m_Vision.FindCircleMetrology(m_Vision.ho_Image, dbVisSetting.FindCircle, true, out ret);
            lblCircleRow.Text = ret.CenterRow.ToString("F3");
            lblCircleCol.Text = ret.CenterCol.ToString("F3");
            lblCircleR.Text = ret.Radius.ToString("F3");
            lblRealRadius.Text = (ret.Radius* m_VisCamera.ResolutionX).ToString("F3");
            lblcircletime.Text = m_Vision.circletime.ToString() + "ms";
        }

        private void btnPNTest_Click(object sender, EventArgs e)
        {
            double regionGray;
            var ret = m_Vision.JudgeDirectorImg(m_Vision.ho_Image, dbVisSetting.GrayCenterRow, dbVisSetting.GrayCenterCol,
                    dbVisSetting.GrayRadius1, dbVisSetting.GrayRadius2, dbVisSetting.GrayMinTolerance,
                    dbVisSetting.GrayMaxTolerance, false, out regionGray);
            lblPNJugger.Text = ret ? "正" : "反";
            lblGrayValue.Text = regionGray.ToString("F3");
            lblgraytime.Text = m_Vision.graytime.ToString() + "ms";
        }
        private void btnRoiSave_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r1 = 0.0;
            var r2 = 0.0;
            GetPolarROI(out row, out col, out r1, out r2);
            dbVisSetting.IsPolarlUse = chxPolarUse.Checked;
            dbVisSetting.IsPolarDisplay = chxPolarDisplay.Checked;
            dbVisSetting.PolarRow = Math.Round(row, 3);
            dbVisSetting.PolarCol = Math.Round(col, 3);
            dbVisSetting.PolarInnerR = Math.Round(r1, 3);
            dbVisSetting.PolarOuterR = Math.Round(r2, 3);
            dbVisSetting.PolarminGray = trackBar1.Value;
            dbVisSetting.PolarmaxGray = trackBar2.Value;
            dbVisSetting.minRegionLenght = (int)ndnminLenght.Value;
            dbVisSetting.maxRegionLenght = (int)ndnmaxLenght.Value;
            dbVisSetting.minPolarArea = (int)ndnminArea.Value;
            dbVisSetting.maxPolarArea = (int)ndnmaxArea.Value;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }

        private void btnPolarTest_Click(object sender, EventArgs e)
        {
            MatchResult ret;
            HTuple area = 0;
            m_Vision.FindAngle(m_Vision.ho_Image, dbVisSetting.PolarRow, dbVisSetting.PolarCol, dbVisSetting.PolarInnerR,
                dbVisSetting.PolarOuterR, dbVisSetting.PolarminGray, dbVisSetting.PolarmaxGray, dbVisSetting.minRegionLenght,
                dbVisSetting.maxRegionLenght, dbVisSetting.minPolarArea, dbVisSetting.maxPolarArea, true, out ret, out area);
            lblPolarRow.Text = ((double)ret.hv_RowCheck).ToString("F3");
            lblPolarCol.Text = ((double)ret.hv_ColumnCheck).ToString("F3");
            lblPolarA.Text = ((double)ret.hv_AngleCheck).ToString("F2");
            lblPolarS.Text = ((double)area).ToString("F2");
            lblPolarL.Text = ((double)ret.hv_Score).ToString("F2");
            lblPolartime.Text = ((double)m_Vision.polartime).ToString("F2");
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
           m_Vision.VisionRun(dbVisSetting,m_VisCamera);

            DispResult(m_Vision.VisionResult, m_Vision.vistime);
        }
        private void DispResult(VisionResult ret, double tt)
        {
            var num = dgvResult.Rows.Count;
            dgvResult.Rows.Add(new object[]
            {
                (num+1).ToString(),
                m_Vision.VisionResult.Result,
                m_Vision.VisionResult.Column.ToString("f3"),
                m_Vision.VisionResult.Row.ToString("f3"),
                m_Vision.VisionResult.Angle.ToString("f3"),
                m_Vision.VisionResult.m_Score.ToString("f3"),
                m_Vision.VisionResult.p_Score.ToString("f3"),
                m_Vision.VisionResult.IsPositive?"正":"反",
                m_Vision.VisionResult.regionGray.ToString("f3"),
                tt.ToString()
            });
        }
        private void chxIsOKimageSave_CheckedChanged(object sender, EventArgs e)
        {
            dbVisSetting.OkImageSave = chxIsOKimageSave.Checked;
        }

        private void chxIsNGimageSave_CheckedChanged(object sender, EventArgs e)
        {
            dbVisSetting.NgImageSave = chxIsNGimageSave.Checked;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblPolarminGray.Text = trackBar1.Value.ToString();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            lblPolarmaxGray.Text = trackBar2.Value.ToString();
        }
        #region 模板显示描绘工具
        /// <summary>
        /// 显示模板工具
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private bool ShowModelROI(double row, double col, double r1, double r2)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                ModelROI = new ROICircle2(row, col, r1, r2);
                m_Vision.roiController.displayCircle2(ModelROI, row, col, r1, r2);
            }
            return true;
        }
        /// <summary>
        /// 显示模板工具
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private bool ShowGlueROI(double row, double col, double r1, double r2)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                GlueROI = new ROICircle2(row, col, r1, r2);
                m_Vision.roiController.displayCircle2(GlueROI, row, col, r1, r2);
            }
            return true;
        }
        /// <summary>
        /// 显示找圆工具
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool ShowCircleROI(double row, double col, double r)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                CircleROI = new ROICircle(row, col, r);
                m_Vision.roiController.displayCircle1(CircleROI, row, col, r);
            }
            return true;
        }
        private bool ShowPointROI(double row, double col, double r)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                PointROI = new ROICircle(row, col, r);
                m_Vision.roiController.displayCircle1(PointROI, row, col, r);
            }
            return true;
        }
        /// <summary>
        /// 显示灰度工具
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private bool ShowGrayROI(double row, double col, double r1, double r2)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                GrayROI = new ROICircle2(row, col, r1, r2);
                m_Vision.roiController.displayCircle2(GrayROI, row, col, r1, r2);
            }
            return true;
        }
        /// <summary>
        /// 显示模板工具
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private bool ShowPolarROI(double row, double col, double r1, double r2)
        {
            if (m_Vision.ho_Image != null && m_Vision.ho_Image.Key != IntPtr.Zero)
            {
                if (m_Vision.viewController != null)
                {
                    m_Vision.viewController.resetAll();
                }
                m_Vision.viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
                PolarROI = new ROICircle2(row, col, r1, r2);
                m_Vision.roiController.displayCircle2(PolarROI, row, col, r1, r2);
            }
            return true;
        }
        /// <summary>
        /// 获取圆ROI半径及中心坐标  ROI变黄才生效
        /// </summary>
        /// <param name="dCenterRow"></param>
        /// <param name="dCenterCol"></param>
        /// <param name="dRadius"></param>
        /// <returns></returns>
        private bool GetCircleROI(out double dCenterRow, out double dCenterCol, out double dRadius)
        {
            dCenterRow = dCenterCol = dRadius = 0;
            if (CircleROI != null)
            {
                dRadius = CircleROI.Radius;
                dCenterRow = CircleROI.Row;
                dCenterCol = CircleROI.Column;
            }
            return true;
        }
        private bool GetPointROI(out double dCenterRow, out double dCenterCol, out double dRadius)
        {                    
            dCenterRow = dCenterCol = dRadius = 0;
            if (PointROI != null)
            {
                dRadius = PointROI.Radius;
                dCenterRow = PointROI.Row;
                dCenterCol = PointROI.Column;
            }
            return true;
        }
        /// <summary>
        /// 获取模板ROI半径及中心坐标  ROI变黄才生效
        /// </summary>
        /// <param name="dCenterRow"></param>
        /// <param name="dCenterCol"></param>
        /// <param name="dRadius1"></param>
        /// <param name="dRadius2"></param>
        /// <returns></returns>
        private bool GetModelROI(out double dCenterRow, out double dCenterCol, out double dRadius1, out double dRadius2)
        {
            dCenterRow = dCenterCol = dRadius1 = dRadius2 = 0;
            if (ModelROI != null)
            {
                dCenterRow = ModelROI.Row;
                dCenterCol = ModelROI.Column;
                dRadius1 = ModelROI.Radius1;
                dRadius2 = ModelROI.Radius2;
            }
            return true;
        }
        /// <summary>
        /// 获取模板ROI半径及中心坐标  ROI变黄才生效
        /// </summary>
        /// <param name="dCenterRow"></param>
        /// <param name="dCenterCol"></param>
        /// <param name="dRadius1"></param>
        /// <param name="dRadius2"></param>
        /// <returns></returns>
        private bool GetGlueROI(out double dCenterRow, out double dCenterCol, out double dRadius1, out double dRadius2)
        {
            dCenterRow = dCenterCol = dRadius1 = dRadius2 = 0;
            if (GlueROI != null)
            {
                dCenterRow = GlueROI.Row;
                dCenterCol = GlueROI.Column;
                dRadius1 = GlueROI.Radius1;
                dRadius2 = GlueROI.Radius2;
            }
            return true;
        }
        /// <summary>
        /// 获取灰度ROI半径及中心坐标  ROI变黄才生效
        /// </summary>
        /// <param name="dCenterRow"></param>
        /// <param name="dCenterCol"></param>
        /// <param name="dRadius1"></param>
        /// <param name="dRadius2"></param>
        /// <returns></returns>
        private bool GetGrayROI(out double dCenterRow, out double dCenterCol, out double dRadius1, out double dRadius2)
        {
            dCenterRow = dCenterCol = dRadius1 = dRadius2 = 0;
            if (GrayROI != null)
            {
                dCenterRow = GrayROI.Row;
                dCenterCol = GrayROI.Column;
                dRadius1 = GrayROI.Radius1;
                dRadius2 = GrayROI.Radius2;
            }
            return true;
        }
        /// <summary>
        /// 获取模板ROI半径及中心坐标  ROI变黄才生效
        /// </summary>
        /// <param name="dCenterRow"></param>
        /// <param name="dCenterCol"></param>
        /// <param name="dRadius1"></param>
        /// <param name="dRadius2"></param>
        /// <returns></returns>
        private bool GetPolarROI(out double dCenterRow, out double dCenterCol, out double dRadius1, out double dRadius2)
        {
            dCenterRow = dCenterCol = dRadius1 = dRadius2 = 0;
            if (PolarROI != null)
            {
                dCenterRow = PolarROI.Row;
                dCenterCol = PolarROI.Column;
                dRadius1 = PolarROI.Radius1;
                dRadius2 = PolarROI.Radius2;
            }
            return true;
        }
        /// <summary>
        /// 获取模板区域大小
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool GetCircle2ROI(out HObject region)
        {
            HObject _region;
            HOperatorSet.GenEmptyObj(out _region);
            _region.Dispose();
            if (ModelROI != null)
            {
                _region = ModelROI.getRegion();
            }
            region = _region;
            return true;
        }
        #endregion

        private void btn_OpenCam_Click(object sender, EventArgs e)
        {
            //if(m_Camera.Initialize()) btn_OpenCam.Enabled=false;
            //else btn_OpenCam.Enabled = true;
        }

        private void cb_Show_CheckedChanged(object sender, EventArgs e)
        {
            m_Camera.CloseCam();
            btn_OpenCam.Enabled = true;
        }

        //private void tbrProductExplor_Scroll(object sender, EventArgs e)
        //{
        //    m_Camera.SetExposureTime(tbrProductExplor.Value * 1000.00);
        //    lblProductExplor.Text = (tbrProductExplor.Value * 1000.00).ToString("f2");
        //    Relationship.Instance.LensCameraCCDParam.Exposure = tbrProductExplor.Value * 1000.00;
        //}

        private void tbrProductGain_Scroll(object sender, EventArgs e)
        {
            //m_Camera.SetGain(tbrProductGain.Value/100.00);
            //lblProductGain.Text = (tbrProductGain.Value/100.00).ToString("f2");
        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (m_Vision.ho_Image == null /*||*/ /*bShowImagePoint == false *//*|| bVideo == true*/)
                return;
            try
            {
                HTuple htuple;
                HOperatorSet.CountChannels(m_Vision.ho_Image, out htuple);
                double row;
                double column;
                int button;

                hWindowControl1.HalconWindow.GetMpositionSubPix(out row, out column, out button);
                string ss1 = string.Format("Y: {0:0000.0},X: {1:0000.0}", row, column);
                if (column >= 0.0 && column < m_Vision.hv_Width && (row >= 0.0 && row < m_Vision.hv_Height))
                {
                    string ss2;
                    if ((htuple) == 1)
                    //ss2 = string.Format("Val: {0:000.0}", himage.GetGrayval((int)row, (int)column));
                    {
                        HTuple value = 0;
                        HOperatorSet.GetGrayval(m_Vision.ho_Image, (int)row, (int)column, out value);
                        ss2 = string.Format("灰度值: {0:000.0}", value);
                    }
                    else if ((htuple) == 3)
                    {

                        //HImage R = himage.AccessChannel(1);
                        //HImage G = himage.AccessChannel(2);
                        //HImage B = himage.AccessChannel(3);
                        HObject R, G, B;
                        HOperatorSet.GenEmptyObj(out R);
                        HOperatorSet.GenEmptyObj(out G);
                        HOperatorSet.GenEmptyObj(out B);
                        HOperatorSet.AccessChannel(m_Vision.ho_Image, out R, 1);
                        HOperatorSet.AccessChannel(m_Vision.ho_Image, out G, 2);
                        HOperatorSet.AccessChannel(m_Vision.ho_Image, out B, 3);
                        HTuple grayval1;
                        HTuple grayval2;
                        HTuple grayval3;

                        HOperatorSet.GetGrayval(R, (int)row, (int)column, out grayval1);
                        HOperatorSet.GetGrayval(G, (int)row, (int)column, out grayval2);
                        HOperatorSet.GetGrayval(B, (int)row, (int)column, out grayval3);
                        //double grayval1 = R.GetGrayval((int)row, (int)column);
                        //double grayval2 = G.GetGrayval((int)row, (int)column);
                        //double grayval3 = B.GetGrayval((int)row, (int)column);
                        (R).Dispose();
                        (G).Dispose();
                        (B).Dispose();
                        ss2 = string.Format("灰度值: ({0:000.0}, {1:000.0}, {2:000.0})", grayval1, grayval2, grayval3);
                    }
                    else
                        ss2 = "";
                    m_CtrlHStatusLabelCtrl.Text = $"{ m_Vision.hv_Width} * {m_Vision.hv_Height}" + "    " + ss1 + "    " + ss2; /*+ "  Xpxiel" + Config.Instance.CameraPixelMM_X + "  Ypxiel" + Config.Instance.CameraPixelMM_Y;*/
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnNoGlueImg_Click(object sender, EventArgs e)
        {
            m_Camera.StopGrabbing();
            m_Camera.SetSoftwareTrigger();//第一步设置软触发模式
            m_Camera.StartGrabbing();//第二部采集开始
            m_Camera.SendSoftwareExecute();//第三部触发
            Thread.Sleep(300);
            m_Vision.UpdataImg_Noglue(m_Camera.ho_Image);
        }

        private void btnReadNoglueImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dg = new OpenFileDialog();
            dg.Multiselect = true;//该值确定是否可以选择多个文件
            dg.Title = "请选择文件夹";
            dg.Filter = "图像文件(*.bmp;*.jpg; *.jpg; *.jpeg; *.gif; *.png)| *.jpg; *.jpeg; *.gif; *.png;*.bmp";
            if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dg.FileName;
                m_Vision.ReadImg_Noglue(file);
                displayTool(tabControl2.SelectedTab);
            }
        }

        private void btnSaveGlueValue_Click(object sender, EventArgs e)
        {
            var row = 0.0;
            var col = 0.0;
            var r1 = 0.0;
            var r2 = 0.0;
            GetGlueROI(out row, out col, out r1, out r2);
            //lblGlueCenterX.Text = col.ToString("0.000");
            //lblGlueCenterY.Text = row.ToString("0.000");
            //lblGlueR1.Text = r1.ToString("0.000");
            //lblGlueR2.Text = r2.ToString("0.000");
            lblGlueWith.Text = Math.Abs(r2 - r1).ToString("0.000");


            dbVisSetting.GlueCenterRow = Math.Round(row, 3);
            dbVisSetting.GlueCenterCol = Math.Round(col, 3);
            dbVisSetting.GlueRadius1 = Math.Round(r1, 3);
            dbVisSetting.GlueRadius2 = Math.Round(r2, 3);

            dbVisSetting.GlueWidth =(int) nudGlueWidth.Value;
            dbVisSetting.GlueInnerCircle = (int)nudGlueRadius.Value;

            dbVisSetting.glueOverflowOutter = (int)nudGlueOutValue.Value;
            dbVisSetting.glueOverflowInner = (int)nudGlueInValue.Value;
            dbVisSetting.glueLackOutter = (int)nudGlueOutNoglueValue.Value;
            dbVisSetting.glueLackInner = (int)nudGlueInNoglueValue.Value;
            dbVisSetting.glueOffset = (int)nudGlueOffsetValue.Value;

            dbVisSetting.kernel = (int)nudOpenCloseValue.Value;
            dbVisSetting.tol = (double)nudGlueThresholdValue.Value;
            dbVisSetting.area = (long)nudGlueAreaMinValue.Value;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }

        private void btnGlueVision_Click(object sender, EventArgs e)
        {
            
            double[] center = new double[2];
            center[0] = dbVisSetting.GlueCenterCol;
            center[1] = dbVisSetting.GlueCenterRow;
            m_Vision.GlueVisionAction(m_Vision.ho_Image_Noglue,m_Vision.ho_Image, center[0], center[1],dbVisSetting);
        }

        private void cb_Show_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cb_Show.Checked)
            {
                m_Camera.StopGrabbing();
                m_Camera.SetFreerun();
                Thread.Sleep(10);
                m_Camera.StartGrabbing();
            }
            else m_Camera.StopGrabbing();
        }

        private void tkbLight_Scroll(object sender, EventArgs e)
        {
            if (lightControl == null) return;
            int value = tkbLight.Value;
            lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, value);
            nudLight.Value = value;
        }

        private void btnSaveLight_Click(object sender, EventArgs e)
        {
            dbVisSetting.LightControlvalue =(int) nudLight.Value;
            System.Toolkit.Helpers.SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
        }

        private void cbShowSZ_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowSZ.Checked)
            {
                bShowShiZhi = true;
            }
            else bShowShiZhi = false;
        }

        private void btnGlueFindParmset_Click(object sender, EventArgs e)
        {
            frmGlueFindValue frmglue = new frmGlueFindValue();
            frmglue.ShowDialog();
            
        }
    }
}