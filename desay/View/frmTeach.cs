using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Motion.Enginee;
using System.Threading;
using Motion.AdlinkAps;
using Motion.Interfaces;
using log4net;
using System.IO;
using System.Toolkit.Helpers;
using desay.Flow;
using desay.ProductData;
using System.Toolkit;
using System.Diagnostics;
using HalconDotNet;
using desay.Vision;
namespace desay
{
    public partial class frmTeach : Form
    {
        static ILog log = LogManager.GetLogger(typeof(frmTeach));
        GluePlateform m_GluePlateform;
        CleanPlateform m_CleanPlateform;
        TransBackJigs m_TransBackJigs;
        AAstation m_AAPlateform;
        AAStockpile m_AAStockpilePlateform;
        MES m_MES;
        #region 气缸
        private CylinderOperate CleanRotateOperate, CleanClampOperate, CleanUpDownOperate;

        //private CylinderOperate LightUpDownOperate, LightingTestOperate;

        private CylinderOperate CleanStopOperate, CleanUpOperate;
        private CylinderOperate GlueStopOperate, GlueUpOperate, GlueUpOperate_Small;
        //private CylinderOperate MoveOperate, CarrierUpOperate, CarrierClampOperate, CarrierStopOperate;
        private CylinderOperate CleanAndGlueStopOperate, AAJigsStopOperate, AAJigsUpOperate, AAJigsUpOperate_Small, AAStockpileUpOperate, AAStockpileStopOperate,
            AABackFlowStopOperate, AAUVUpStopOperate, AAClampClawOperate;
        #endregion

        public StationInitialize stationInitialize;
        public StationOperate stationOperate;
        public External externalSign;


        HObject ho_ImageT;
        Point<double> Center = new Point<double> { };
        bool isUseGlue = false;
        bool isUsePlasma = false;
        public frmTeach()
        {
            InitializeComponent();
        }

        public frmTeach(GluePlateform glueplateform, CleanPlateform cleanPlateform, TransBackJigs transback,AAstation aastaion, AAStockpile aastockpile, MES m_mes) : this()
        {
            m_GluePlateform = glueplateform;
            m_CleanPlateform = cleanPlateform;
            m_TransBackJigs = transback;
            m_AAPlateform = aastaion;
            m_AAStockpilePlateform = aastockpile;
            m_MES = m_mes;
            HOperatorSet.GenEmptyObj(out ho_ImageT);
        }
        private void frmTeach_Load(object sender, EventArgs e)
        {
            tbX.Text = AxisParameter.Instance.CameraAndNeedleOffset.X.ToString("0.000");
            tbY.Text = AxisParameter.Instance.CameraAndNeedleOffset.Y.ToString("0.000");
            nudOffsetX.Value = (decimal)AxisParameter.Instance.GlueOffsetX;
            nudOffsetY.Value = (decimal)AxisParameter.Instance.GlueOffsetY;
            InitdgvCleanPositionRows();
            InitdgvGluePositionRows();
            lblJogSpeed.Text = "点动速度:" + tbrJogSpeed.Value.ToString("0.00") + "mm/s";

            //清洗气缸
            CleanStopOperate = new CylinderOperate(() => { m_CleanPlateform.CleanStopCylinder.ManualExecute(); }) { CylinderName = "清洗阻挡气缸" };
            CleanUpOperate = new CylinderOperate(() => { m_CleanPlateform.CleanUpCylinder.ManualExecute(); }) { CylinderName = "清洗顶升气缸" };
            CleanClampOperate = new CylinderOperate(() => { m_CleanPlateform.CleanClampCylinder.ManualExecute(); }) { CylinderName = "清洗夹紧气缸" };
            CleanRotateOperate = new CylinderOperate(() => { m_CleanPlateform.CleanRotateCylinder.ManualExecute(); }) { CylinderName = "清洗旋转气缸" };
            CleanUpDownOperate = new CylinderOperate(() => { m_CleanPlateform.CleanUpDownCylinder.ManualExecute(); }) { CylinderName = "清洗上下气缸" };
            //LightUpDownOperate = new CylinderOperate(() => { m_CleanPlateform.LightUpDownCylinder.ManualExecute(); }) { CylinderName = "光源上下气缸" };
            //LightingTestOperate = new CylinderOperate(() => { m_CleanPlateform.LightingTestCylinder.ManualExecute(); }) { CylinderName = "点亮测试气缸" };

            //点胶气缸
            GlueStopOperate = new CylinderOperate(() => { m_GluePlateform.GlueStopCylinder.ManualExecute(); }) { CylinderName = "点胶阻挡气缸" };
            GlueUpOperate = new CylinderOperate(() => { m_GluePlateform.GlueUpCylinder.ManualExecute(); }) { CylinderName = "点胶顶升气缸" };
            GlueUpOperate_Small = new CylinderOperate(() => { m_GluePlateform.GlueUpCylinder_Small.ManualExecute(); }) { CylinderName = "点胶探针顶升气缸" };

            CleanAndGlueStopOperate = new CylinderOperate(() => { m_TransBackJigs.CleanGlueBackStopCylinder.ManualExecute(); }) { CylinderName = "点胶回流阻挡气缸" };
            AABackFlowStopOperate = new CylinderOperate(() => { m_TransBackJigs.AAbackHomeStopCylinder.ManualExecute(); }) { CylinderName = "AA回流阻挡气缸" };
            //AA


            AAJigsStopOperate = new CylinderOperate(() => { m_AAPlateform.AAJigsStopCylinder.ManualExecute(); }) { CylinderName = "AA夹具阻挡气缸" };
            AAJigsUpOperate = new CylinderOperate(() => { m_AAPlateform.AAJigsUpCylinder.ManualExecute(); }) { CylinderName = "AA夹具顶升气缸" };
            AAJigsUpOperate_Small = new CylinderOperate(() => { m_AAPlateform.AAJigsCylinder_Small.ManualExecute(); }) { CylinderName = "AA夹具探针顶升气缸" };
            AAStockpileUpOperate = new CylinderOperate(() => { m_AAStockpilePlateform.AAStockpileUpCylinder.ManualExecute(); }) { CylinderName = "AA堆料顶升气缸" };
            AAStockpileStopOperate = new CylinderOperate(() => { m_AAStockpilePlateform.AAStockpileStopCylinder.ManualExecute(); }) { CylinderName = "AA堆料阻挡气缸" };
            
            //AAUVUpStopOperate = new CylinderOperate(() => { m_AAPlateform.AAUVUpDownCylinder.ManualExecute(); }) { CylinderName = "UV灯上下气缸" };
            //AAClampClawOperate = new CylinderOperate(() => { m_AAPlateform.AAClampClawCylinder.ManualExecute(); }) { CylinderName = "AA气夹爪气缸" };

            flpCylinder.Controls.Add(CleanStopOperate);
            flpCylinder.Controls.Add(CleanUpOperate);
            flpCylinder.Controls.Add(CleanClampOperate);
            flpCylinder.Controls.Add(CleanRotateOperate);
            flpCylinder.Controls.Add(CleanUpDownOperate);
          
            flpCylinder2.Controls.Add(GlueStopOperate);
            flpCylinder2.Controls.Add(GlueUpOperate);
            flpCylinder2.Controls.Add(CleanAndGlueStopOperate);
            flpCylinder2.Controls.Add(AABackFlowStopOperate);
            if (AxisParameter.Instance.IsUseTanZhenCyl) flpCylinder2.Controls.Add(GlueUpOperate_Small);

            flpCylinder3.Controls.Add(AAStockpileUpOperate);
            flpCylinder3.Controls.Add(AAStockpileStopOperate);
            flpCylinder3.Controls.Add(AAJigsStopOperate);
            flpCylinder3.Controls.Add(AAJigsUpOperate);
            if (AxisParameter.Instance.IsUseTanZhenCyl) flpCylinder3.Controls.Add(AAJigsUpOperate_Small);
            //flpCylinder3.Controls.Add(AAUVUpStopOperate);
            //flpCylinder3.Controls.Add(AAClampClawOperate);

            //tbAALightZPos.Text = Position.Instance.AAZWorkPos.ToString("0.000");

            nudGlueUseIndex.Value = Config.Instance.GlueUseNumsIndex;
            nudGlueUseAllNums.Value= Config.Instance.GlueUseAllNums ;
            nudGlueUseAlarmNums.Value= Config.Instance.GlueUseAlarmNums ;
            nudGlueHAveUseTime.Value = (decimal)Config.Instance.GlueTimeAlarm ;
            ndnGlueHeight.Value = (decimal)Position.Instance.GlueHeight;
            timer1.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            #region 气缸状态
            //清洗平台
            CleanStopOperate.InOrigin = m_CleanPlateform.CleanStopCylinder.OutOriginStatus;
            CleanStopOperate.InMove = m_CleanPlateform.CleanStopCylinder.OutMoveStatus;
            CleanStopOperate.OutMove = m_CleanPlateform.CleanStopCylinder.IsOutMove;
            CleanUpOperate.InOrigin = m_CleanPlateform.CleanUpCylinder.OutOriginStatus;
            CleanUpOperate.InMove = m_CleanPlateform.CleanUpCylinder.OutMoveStatus;         
            CleanUpOperate.OutMove = m_CleanPlateform.CleanUpCylinder.IsOutMove;
            CleanClampOperate.InOrigin = m_CleanPlateform.CleanClampCylinder.OutOriginStatus;
            CleanClampOperate.InMove = m_CleanPlateform.CleanClampCylinder.OutMoveStatus;
            CleanClampOperate.OutMove = m_CleanPlateform.CleanClampCylinder.IsOutMove;
            CleanRotateOperate.InOrigin = m_CleanPlateform.CleanRotateCylinder.OutOriginStatus;
            CleanRotateOperate.InMove = m_CleanPlateform.CleanRotateCylinder.OutMoveStatus;
            CleanRotateOperate.OutMove = m_CleanPlateform.CleanRotateCylinder.IsOutMove;
            CleanUpDownOperate.InOrigin = m_CleanPlateform.CleanUpDownCylinder.OutOriginStatus;
            CleanUpDownOperate.InMove = m_CleanPlateform.CleanUpDownCylinder.OutMoveStatus;
            CleanUpDownOperate.OutMove = m_CleanPlateform.CleanUpDownCylinder.IsOutMove;


            //点胶平台
            GlueStopOperate.InOrigin = m_GluePlateform.GlueStopCylinder.OutOriginStatus;
            GlueStopOperate.InMove = m_GluePlateform.GlueStopCylinder.OutMoveStatus;
            GlueStopOperate.OutMove = m_GluePlateform.GlueStopCylinder.IsOutMove;
            GlueUpOperate.InOrigin = m_GluePlateform.GlueUpCylinder.OutOriginStatus;
            GlueUpOperate.InMove = m_GluePlateform.GlueUpCylinder.OutMoveStatus;
            GlueUpOperate.OutMove = m_GluePlateform.GlueUpCylinder.IsOutMove;
            if (AxisParameter.Instance.IsUseTanZhenCyl) {

                GlueUpOperate_Small.InOrigin = m_GluePlateform.GlueUpCylinder_Small.OutOriginStatus;
                GlueUpOperate_Small.InMove = m_GluePlateform.GlueUpCylinder_Small.OutMoveStatus;
                GlueUpOperate_Small.OutMove = m_GluePlateform.GlueUpCylinder_Small.IsOutMove;

                AAJigsUpOperate_Small.InOrigin = m_AAPlateform.AAJigsCylinder_Small.OutOriginStatus;
                AAJigsUpOperate_Small.InMove = m_AAPlateform.AAJigsCylinder_Small.OutMoveStatus;
                AAJigsUpOperate_Small.OutMove = m_AAPlateform.AAJigsCylinder_Small.IsOutMove;
            }
            
            //流水线
            AABackFlowStopOperate.InOrigin = m_TransBackJigs.AAbackHomeStopCylinder.OutOriginStatus;
            AABackFlowStopOperate.InMove = m_TransBackJigs.AAbackHomeStopCylinder.OutMoveStatus;
            AABackFlowStopOperate.OutMove = m_TransBackJigs.AAbackHomeStopCylinder.IsOutMove;
            CleanAndGlueStopOperate.InOrigin = m_TransBackJigs.CleanGlueBackStopCylinder.OutOriginStatus;
            CleanAndGlueStopOperate.InMove = m_TransBackJigs.CleanGlueBackStopCylinder.OutMoveStatus;
            CleanAndGlueStopOperate.OutMove = m_TransBackJigs.CleanGlueBackStopCylinder.IsOutMove;

           
            //AA平台

            AAJigsStopOperate.InOrigin = m_AAPlateform.AAJigsStopCylinder.OutOriginStatus;
            AAJigsStopOperate.InMove = m_AAPlateform.AAJigsStopCylinder.OutMoveStatus;
            AAJigsStopOperate.OutMove = m_AAPlateform.AAJigsStopCylinder.IsOutMove;

            AAJigsUpOperate.InOrigin = m_AAPlateform.AAJigsUpCylinder.OutOriginStatus;
            AAJigsUpOperate.InMove = m_AAPlateform.AAJigsUpCylinder.OutMoveStatus;
            AAJigsUpOperate.OutMove = m_AAPlateform.AAJigsUpCylinder.IsOutMove;
           

            AAStockpileUpOperate.InOrigin = m_AAStockpilePlateform.AAStockpileUpCylinder.OutOriginStatus;
            AAStockpileUpOperate.InMove = m_AAStockpilePlateform.AAStockpileUpCylinder.OutMoveStatus;
            AAStockpileUpOperate.OutMove = m_AAStockpilePlateform.AAStockpileUpCylinder.IsOutMove;

            AAStockpileStopOperate.InOrigin = m_AAStockpilePlateform.AAStockpileStopCylinder.OutOriginStatus;
            AAStockpileStopOperate.InMove = m_AAStockpilePlateform.AAStockpileStopCylinder.OutMoveStatus;
            AAStockpileStopOperate.OutMove = m_AAStockpilePlateform.AAStockpileStopCylinder.IsOutMove;

            ////AAUVUpStopOperate.InOrigin = m_AAPlateform.AAUVUpDownCylinder.OutOriginStatus;
            ////AAUVUpStopOperate.InMove = m_AAPlateform.AAUVUpDownCylinder.OutMoveStatus;
            ////AAUVUpStopOperate.OutMove = m_AAPlateform.AAUVUpDownCylinder.IsOutMove;

            ////AAClampClawOperate.InOrigin = m_AAPlateform.AAClampClawCylinder.OutOriginStatus;
            ////AAClampClawOperate.InMove = m_AAPlateform.AAClampClawCylinder.OutMoveStatus;
            ////AAClampClawOperate.OutMove = m_AAPlateform.AAClampClawCylinder.IsOutMove;



            #endregion

            #region 轴状态
            chxLX.Checked = m_CleanPlateform.Xaxis.IsServon;
            chxLY.Checked = m_CleanPlateform.Yaxis.IsServon;
            chxLZ.Checked = m_CleanPlateform.Zaxis.IsServon;
            chxRX.Checked = m_GluePlateform.Xaxis.IsServon;
            chxRY.Checked = m_GluePlateform.Yaxis.IsServon;
            chxRZ.Checked = m_GluePlateform.Zaxis.IsServon;

            lblLXCurrentpos.Text = m_CleanPlateform.Xaxis.CurrentPos.ToString("0.000");
            lblLYCurrentpos.Text = m_CleanPlateform.Yaxis.CurrentPos.ToString("0.000");
            lblLZCurrentpos.Text = m_CleanPlateform.Zaxis.CurrentPos.ToString("0.000");
            lblRXCurrentpos.Text = m_GluePlateform.Xaxis.CurrentPos.ToString("0.000");
            lblRYCurrentpos.Text = m_GluePlateform.Yaxis.CurrentPos.ToString("0.000");
            lblRZCurrentpos.Text = m_GluePlateform.Zaxis.CurrentPos.ToString("0.000");

            //lblAAYCurrentpos.Text = m_AAPlateform.MoveSixYaxis.CurrentPos.ToString("0.000");
            //lblAAZCurrentpos.Text = m_AAPlateform.LightZaxis.CurrentPos.ToString("0.000");

            lblLXCurrentSpeed.Text = m_CleanPlateform.Xaxis.CurrentSpeed.ToString("0.000");
            lblLYCurrentSpeed.Text = m_CleanPlateform.Yaxis.CurrentSpeed.ToString("0.000");
            lblLZCurrentSpeed.Text = m_CleanPlateform.Zaxis.CurrentSpeed.ToString("0.000");
            lblRXCurrentSpeed.Text = m_GluePlateform.Xaxis.CurrentSpeed.ToString("0.000");
            lblRYCurrentSpeed.Text = m_GluePlateform.Yaxis.CurrentSpeed.ToString("0.000");
            lblRZCurrentSpeed.Text = m_GluePlateform.Zaxis.CurrentSpeed.ToString("0.000");

            //lblAAYCurrentSpeed.Text = m_AAPlateform.MoveSixYaxis.CurrentSpeed.ToString("0.000");
            //lblAAZCurrentSpeed.Text = m_AAPlateform.LightZaxis.CurrentSpeed.ToString("0.000");

            #endregion
            #region 气缸界面显示刷新
            #endregion

            timer1.Enabled = true;
        }


        private void tbrJogSpeed_Scroll(object sender, EventArgs e)
        {
            if (Global.IsLocating) return;
            this.Invoke(new Action(()=> {
                lblJogSpeed.Text = "点动速度:" + tbrJogSpeed.Value.ToString("0.00") + "mm/s";
                var JogSpeed = (double)tbrJogSpeed.Value;
                Global.LXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                Global.LYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                Global.LZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                Global.RXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                Global.RYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                Global.RZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            }));

        }

        #region 数据表格初始化
        /// <summary>
        /// 数据初始化   
        /// </summary>
        private void InitdgvCleanPositionRows()
        {
            this.dgvCleanPosition.Rows.Clear();
            //in a real scenario, you may need to add different rows
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗安全位置",
                    Position.Instance.CleanSafePosition.X.ToString("0.000"),
                    Position.Instance.CleanSafePosition.Y.ToString("0.000"),
                    Position.Instance.CleanSafePosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });      
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜筒轨迹第一点位置",
                    Position.Instance.CleanConeFirstPosition.X.ToString("0.000"),
                    Position.Instance.CleanConeFirstPosition.Y.ToString("0.000"),
                    Position.Instance.CleanConeFirstPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜筒轨迹第二点位置",
                    Position.Instance.CleanConeSecondPosition.X.ToString("0.000"),
                    Position.Instance.CleanConeSecondPosition.Y.ToString("0.000"),
                    Position.Instance.CleanConeSecondPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜筒轨迹第三点位置",
                    Position.Instance.CleanConeThirdPositon.X.ToString("0.000"),
                    Position.Instance.CleanConeThirdPositon.Y.ToString("0.000"),
                    Position.Instance.CleanConeThirdPositon.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜片轨迹第一点位置",
                    Position.Instance.CleanLensFirstPosition.X.ToString("0.000"),
                    Position.Instance.CleanLensFirstPosition.Y.ToString("0.000"),
                    Position.Instance.CleanLensFirstPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜片轨迹第二点位置",
                    Position.Instance.CleanLensSecondPosition.X.ToString("0.000"),
                    Position.Instance.CleanLensSecondPosition.Y.ToString("0.000"),
                    Position.Instance.CleanLensSecondPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvCleanPosition.Rows.Add(new object[] {
                    "清洗镜片轨迹第三点位置",
                    Position.Instance.CleanLensThirdPositon.X.ToString("0.000"),
                    Position.Instance.CleanLensThirdPositon.Y.ToString("0.000"),
                    Position.Instance.CleanLensThirdPositon.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
        }
        private void InitdgvGluePositionRows()
        {
            this.dgvGluePosition.Rows.Clear();
            //in a real scenario, you may need to add different rows
            dgvGluePosition.Rows.Add(new object[] {
                    "点胶安全位置",
                    Position.Instance.GlueSafePosition.X.ToString("0.000"),
                    Position.Instance.GlueSafePosition.Y.ToString("0.000"),
                    Position.Instance.GlueSafePosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
   
            dgvGluePosition.Rows.Add(new object[] {
                    "点胶定位相机拍照位置",
                    Position.Instance.GlueCameraPosition.X.ToString("0.000"),
                    Position.Instance.GlueCameraPosition.Y.ToString("0.000"),
                    Position.Instance.GlueCameraPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvGluePosition.Rows.Add(new object[] {
                    "点胶识别相机拍照位置",
                    Position.Instance.GlueCheckCameraPosition.X.ToString("0.000"),
                    Position.Instance.GlueCheckCameraPosition.Y.ToString("0.000"),
                    Position.Instance.GlueCheckCameraPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            //dgvGluePosition.Rows.Add(new object[] {
            //        "点胶对针位置",
            //        Position.Instance.GlueAdjustPinPosition.X.ToString("0.000"),
            //        Position.Instance.GlueAdjustPinPosition.Y.ToString("0.000"),
            //        Position.Instance.GlueAdjustPinPosition.Z.ToString("0.000"),
            //        "Save",
            //        "GotoZ",
            //        "GotoZero"
            //    });
            //dgvGluePosition.Rows.Add(new object[] {
            //        "点胶割胶起始位置",
            //        Position.Instance.CutGlueStartPosition.X.ToString("0.000"),
            //        Position.Instance.CutGlueStartPosition.Y.ToString("0.000"),
            //        Position.Instance.CutGlueStartPosition.Z.ToString("0.000"),
            //        "Save",
            //        "GotoZ",
            //        "GotoZero"
            //    });
            //dgvGluePosition.Rows.Add(new object[] {
            //        "点胶割胶结束位置",
            //        Position.Instance.CutGlueEndPosition.X.ToString("0.000"),
            //        Position.Instance.CutGlueEndPosition.Y.ToString("0.000"),
            //        Position.Instance.CutGlueEndPosition.Z.ToString("0.000"),
            //        "Save",
            //        "GotoZ",
            //        "GotoZero"
            //    });
            //dgvGluePosition.Rows.Add(new object[] {
            //        "胶重点检位置",
            //        Position.Instance.WeightGluePosition.X.ToString("0.000"),
            //        Position.Instance.WeightGluePosition.Y.ToString("0.000"),
            //        Position.Instance.WeightGluePosition.Z.ToString("0.000"),
            //        "Save",
            //        "GotoZ",
            //        "GotoZero"
            //    });

            dgvGluePosition.Rows.Add(new object[] {
                    "白板测试位置",
                    Position.Instance.AdjustLightPosition.X.ToString("0.000"),
                    Position.Instance.AdjustLightPosition.Y.ToString("0.000"),
                    Position.Instance.AdjustLightPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvGluePosition.Rows.Add(new object[] {
                    "点胶标定相机拍照位置",
                    Position.Instance.GlueCameraCalibPosition.X.ToString("0.000"),
                    Position.Instance.GlueCameraCalibPosition.Y.ToString("0.000"),
                    Position.Instance.GlueCameraCalibPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
            dgvGluePosition.Rows.Add(new object[] {
                    "视觉标定点胶位置",
                    Position.Instance.VisionCalibGluePosition.X.ToString("0.000"),
                    Position.Instance.VisionCalibGluePosition.Y.ToString("0.000"),
                    Position.Instance.VisionCalibGluePosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
        }
        #endregion

        #region 数据表格刷新
        /// <summary>
        /// 准数据刷新
        /// </summary>
        private void RefreshdgvCleanPositionRows(int i)
        {
            switch (i)
            {
                case 0:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗安全位置",
                        Position.Instance.CleanSafePosition.X.ToString("0.000"),
                        Position.Instance.CleanSafePosition.Y.ToString("0.000"),
                        Position.Instance.CleanSafePosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break; 
                case 1:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜筒轨迹第一点位置",
                        Position.Instance.CleanConeFirstPosition.X.ToString("0.000"),
                        Position.Instance.CleanConeFirstPosition.Y.ToString("0.000"),
                        Position.Instance.CleanConeFirstPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 2:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜筒轨迹第二点位置",
                        Position.Instance.CleanConeSecondPosition.X.ToString("0.000"),
                        Position.Instance.CleanConeSecondPosition.Y.ToString("0.000"),
                        Position.Instance.CleanConeSecondPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 3:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜筒轨迹第三点位置",
                        Position.Instance.CleanConeThirdPositon.X.ToString("0.000"),
                        Position.Instance.CleanConeThirdPositon.Y.ToString("0.000"),
                        Position.Instance.CleanConeThirdPositon.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 4:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜片轨迹第一点位置",
                        Position.Instance.CleanLensFirstPosition.X.ToString("0.000"),
                        Position.Instance.CleanLensFirstPosition.Y.ToString("0.000"),
                        Position.Instance.CleanLensFirstPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 5:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜片轨迹第二点位置",
                        Position.Instance.CleanLensSecondPosition.X.ToString("0.000"),
                        Position.Instance.CleanLensSecondPosition.Y.ToString("0.000"),
                        Position.Instance.CleanLensSecondPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 6:
                    dgvCleanPosition.Rows[i].SetValues(new object[] {
                        "清洗镜片轨迹第三点位置",
                        Position.Instance.CleanLensThirdPositon.X.ToString("0.000"),
                        Position.Instance.CleanLensThirdPositon.Y.ToString("0.000"),
                        Position.Instance.CleanLensThirdPositon.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                default: break;
            }
        }
        private void RefreshdgvRightPlatePositionRows(int i)
        {
            switch (i)
            {
                case 0:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                        "点胶安全位置",
                        Position.Instance.GlueSafePosition.X.ToString("0.000"),
                        Position.Instance.GlueSafePosition.Y.ToString("0.000"),
                        Position.Instance.GlueSafePosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
       
                case 1:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                        "点胶定位相机拍照位置",
                        Position.Instance.GlueCameraPosition.X.ToString("0.000"),
                        Position.Instance.GlueCameraPosition.Y.ToString("0.000"),
                        Position.Instance.GlueCameraPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 2:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                    "点胶识别相机拍照位置",
                    Position.Instance.GlueCheckCameraPosition.X.ToString("0.000"),
                    Position.Instance.GlueCheckCameraPosition.Y.ToString("0.000"),
                    Position.Instance.GlueCheckCameraPosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                    });
                    break;
       

                case 3:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                        "白板测试位置",
                        Position.Instance.AdjustLightPosition.X.ToString("0.000"),
                        Position.Instance.AdjustLightPosition.Y.ToString("0.000"),
                        Position.Instance.AdjustLightPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 4:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                        "点胶标定相机拍照位置",
                        Position.Instance.GlueCameraCalibPosition.X.ToString("0.000"),
                        Position.Instance.GlueCameraCalibPosition.Y.ToString("0.000"),
                        Position.Instance.GlueCameraCalibPosition.Z.ToString("0.000"),
                        "Save",
                        "GotoZ",
                        "GotoZero"
                    });
                    break;
                case 5:
                    dgvGluePosition.Rows[i].SetValues(new object[] {
                        "视觉标定点胶位置",
                    Position.Instance.VisionCalibGluePosition.X.ToString("0.000"),
                    Position.Instance.VisionCalibGluePosition.Y.ToString("0.000"),
                    Position.Instance.VisionCalibGluePosition.Z.ToString("0.000"),
                    "Save",
                    "GotoZ",
                    "GotoZero"
                });
                    break;
                   
            default: break;
            }
        }
        #endregion

        #region 数据表格操作
        private void dgvCleanPosition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Global.IsLocating) return;
            var JogSpeed = (double)tbrJogSpeed.Value;
            Global.LXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            Global.LYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            Global.LZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            switch (e.ColumnIndex)
            {
                case 4:
                    if (MessageBox.Show($"是否保存{dgvCleanPosition.Rows[e.RowIndex].Cells[0].Value}的数据",
                        "保存", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvCleanPosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Save")
                    {
                        switch (e.RowIndex)
                        {
                            case 0://清洗安全位置
                                Position.Instance.CleanSafePosition.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanSafePosition.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanSafePosition.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                break;
                            case 1://清洗镜筒第一位置
                                Position.Instance.CleanConeFirstPosition.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanConeFirstPosition.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanConeFirstPosition.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                break;
                            case 2://清洗镜筒第二位置
                                Position.Instance.CleanConeSecondPosition.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanConeSecondPosition.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanConeSecondPosition.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                break;
                            case 3://清洗镜筒第三位置
                                Position.Instance.CleanConeThirdPositon.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanConeThirdPositon.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanConeThirdPositon.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                CalculateConeCenter();
                                break;
                            case 4://清洗镜片第一位置
                                Position.Instance.CleanLensFirstPosition.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanLensFirstPosition.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanLensFirstPosition.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                break;
                            case 5://清洗镜片第二位置
                                Position.Instance.CleanLensSecondPosition.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanLensSecondPosition.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanLensSecondPosition.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                break;
                            case 6://清洗镜片第三位置
                                Position.Instance.CleanLensThirdPositon.X = m_CleanPlateform.Xaxis.CurrentPos;
                                Position.Instance.CleanLensThirdPositon.Y = m_CleanPlateform.Yaxis.CurrentPos;
                                Position.Instance.CleanLensThirdPositon.Z = m_CleanPlateform.Zaxis.CurrentPos;
                                CalculateLensCenter();
                                break;
                            default: break;
                        }
                        RefreshdgvCleanPositionRows(e.RowIndex);

                        SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                    }
                    break;
                case 5:
                    if (MessageBox.Show($"是否定位到{dgvCleanPosition.Rows[e.RowIndex].Cells[0].Value},Z位置",
                        "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvCleanPosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "GotoZ")
                    {
                        var ret = 0;
                        switch (e.RowIndex)
                        {
                            case 0://清洗安全位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanSafePosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanSafePosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanSafePosition.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;                
                            case 1://清洗镜筒轨迹第一点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeFirstPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeFirstPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanConeFirstPosition.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 2://清洗镜筒轨迹第二点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeSecondPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeSecondPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanConeSecondPosition.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 3://清洗镜筒轨迹第三点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeThirdPositon.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeThirdPositon.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanConeThirdPositon.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 4://清洗镜片轨迹第一点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensFirstPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensFirstPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanLensFirstPosition.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 5://清洗镜片轨迹第二点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensSecondPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensSecondPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanLensSecondPosition.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 6://清洗镜片轨迹第三点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensThirdPositon.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensThirdPositon.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanLensThirdPositon.Z, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            default: break;
                        }
                        switch (ret)
                        {
                            case -2:
                                MessageBox.Show("伺服定位异常失败！");
                                break;
                            case -3:
                                MessageBox.Show("伺服未使能,或伺服状态不在安全位置");
                                break;
                            case -4:
                                MessageBox.Show("伺服忙碌中！");
                                break;
                        }
                    }
                    break;
                case 6:
                    if (MessageBox.Show($"是否定位到{dgvCleanPosition.Rows[e.RowIndex].Cells[0].Value},Z=0.5位置",
                        "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvCleanPosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "GotoZero")
                    {
                        var ret = 0;
                        switch (e.RowIndex)
                        {
                            case 0://清洗安全位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanSafePosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanSafePosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            //case 1://有无料判断位置
                            //    ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.LensDetectPosition.X, Global.LXmanualSpeed,
                            //        m_CleanPlateform.Yaxis, Position.Instance.LensDetectPosition.Y, Global.LYmanualSpeed,
                            //        m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                            //        () =>
                            //        {
                            //            return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                            //                     | m_CleanPlateform.stationInitialize.InitializeDone;
                            //        });
                            //    break;
         
                            case 1://清洗镜筒轨迹第一点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeFirstPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeFirstPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 2://清洗镜筒轨迹第二点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeSecondPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeSecondPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 3://清洗镜筒轨迹第三点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeThirdPositon.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeThirdPositon.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 4://清洗镜片轨迹第一点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensFirstPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensFirstPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 5://清洗镜片轨迹第二点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensSecondPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensSecondPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 6://清洗镜片轨迹第三点位置
                                ret = MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensThirdPositon.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensThirdPositon.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, 0.5, Global.LZmanualSpeed,
                                    () => {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            default: break;
                        }
                        switch (ret)
                        {
                            case -2:
                                MessageBox.Show("伺服定位异常失败！");
                                break;
                            case -3:
                                MessageBox.Show("伺服未使能,或伺服状态不在安全位置");
                                break;
                            case -4:
                                MessageBox.Show("伺服忙碌中！");
                                break;
                        }
                    }
                    break;
                default: break;
            }
        }
        private void dgvGluePosition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Global.IsLocating) return;
            var JogSpeed = (double)tbrJogSpeed.Value;
            Global.RXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            Global.RYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            Global.RZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            switch (e.ColumnIndex)
            {
                case 4:
                    if (MessageBox.Show($"是否保存{dgvGluePosition.Rows[e.RowIndex].Cells[0].Value}的数据",
                        "保存", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvGluePosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Save")
                    {
                        switch (e.RowIndex)
                        {
                            case 0://点胶安全位置
                                Position.Instance.GlueSafePosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.GlueSafePosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.GlueSafePosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;
                        
                            case 1://点胶定位相机拍照位置
                                Position.Instance.GlueCameraPosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.GlueCameraPosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.GlueCameraPosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;

                            case 2://点胶识别相机拍照位置
                                Position.Instance.GlueCheckCameraPosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.GlueCheckCameraPosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.GlueCheckCameraPosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;
                           
       
                            case 3://白板测试位置
                                Position.Instance.AdjustLightPosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.AdjustLightPosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.AdjustLightPosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;
                            case 4://点胶标定相机拍照位置
                                Position.Instance.GlueCameraCalibPosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.GlueCameraCalibPosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.GlueCameraCalibPosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;
                            case 5://视觉标定点胶位置
                                Position.Instance.VisionCalibGluePosition.X = m_GluePlateform.Xaxis.CurrentPos;
                                Position.Instance.VisionCalibGluePosition.Y = m_GluePlateform.Yaxis.CurrentPos;
                                Position.Instance.VisionCalibGluePosition.Z = m_GluePlateform.Zaxis.CurrentPos;
                                break;
                            default: break;
                        }
                        RefreshdgvRightPlatePositionRows(e.RowIndex);

                        SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                    }
                    break;
                case 5:
                    if (MessageBox.Show($"是否定位到{dgvGluePosition.Rows[e.RowIndex].Cells[0].Value},Z位置",
                        "保存", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvGluePosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "GotoZ")
                    {
                        var ret = 0;
                        switch (e.RowIndex)
                        {
                            case 0://点胶安全位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueSafePosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueSafePosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                       
                            case 1://点胶定位相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCameraPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 2://点胶识别相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCheckCameraPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCheckCameraPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.GlueCheckCameraPosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                          
                           

                            case 3://白板测试位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.AdjustLightPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.AdjustLightPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.AdjustLightPosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 4://点胶标定相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCameraCalibPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCameraCalibPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.GlueCameraCalibPosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 5://视觉标定点胶位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.VisionCalibGluePosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.VisionCalibGluePosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.VisionCalibGluePosition.Z, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            default: break;
                        }
                        switch (ret)
                        {
                            case -2:
                                MessageBox.Show("伺服定位异常失败！");
                                break;
                            case -3:
                                MessageBox.Show("伺服未使能,或伺服状态不在安全位置");
                                break;
                            case -4:
                                MessageBox.Show("伺服忙碌中！");
                                break;
                        }
                    }
                    break;
                case 6:
                    if (MessageBox.Show($"是否定位到{dgvGluePosition.Rows[e.RowIndex].Cells[0].Value},Z=0.5位置",
                        "保存", MessageBoxButtons.OKCancel) == DialogResult.Cancel) break;
                    if (dgvGluePosition.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "GotoZero")
                    {
                        var ret = 0;
                        switch (e.RowIndex)
                        {
                            case 0://点胶安全位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueSafePosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueSafePosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, 0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                   
                            case 1://点胶定位相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCameraPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, 0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 2://点胶识别相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCheckCameraPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCheckCameraPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis,0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                           
                            case 3://白板测试位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.AdjustLightPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.AdjustLightPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis,0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 4://点胶标定相机拍照位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCameraCalibPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.GlueCameraCalibPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, 0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;
                            case 5://视觉标定点胶位置
                                ret = MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.VisionCalibGluePosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.VisionCalibGluePosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, 0.5, Global.RZmanualSpeed,
                                    () => {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                                break;

                            default: break;
                        }
                        switch (ret)
                        {
                            case -2:
                                MessageBox.Show("伺服定位异常失败！");
                                break;
                            case -3:
                                MessageBox.Show("伺服未使能,或伺服状态不在安全位置");
                                break;
                            case -4:
                                MessageBox.Show("伺服忙碌中！");
                                break;
                        }
                    }
                    break;
                default: break;
            }
        }
        #endregion

        #region 轴运动操作
        private void BtnLXdec_MouseDown(object sender, MouseEventArgs e)
        {
            picLXdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                   m_CleanPlateform.Xaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Xaxis.Negative();
                }
                else
                {
                    Global.LXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Xaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.LXmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnLXadd_MouseDown(object sender, MouseEventArgs e)
        {
            picLXadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_CleanPlateform.Xaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Xaxis.Postive();
                }
                else
                {
                    Global.LXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Xaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.LXmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnLX_MouseUp(object sender, MouseEventArgs e)
        {
            picLXdec.BackColor = Color.Transparent;
            picLXadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_CleanPlateform.Xaxis.Stop();
            }
            catch (Exception ex) { }
        }

        private void BtnLYdec_MouseDown(object sender, MouseEventArgs e)
        {
            picLYdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_CleanPlateform.Yaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Yaxis.Negative();
                }
                else
                {
                    Global.LYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Yaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.LYmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnLYadd_MouseDown(object sender, MouseEventArgs e)
        {
            picLYadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_CleanPlateform.Yaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Yaxis.Postive();
                }
                else
                {
                    Global.LYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Yaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.LYmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnLY_MouseUp(object sender, MouseEventArgs e)
        {
            picLYdec.BackColor = Color.Transparent;
            picLYadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_CleanPlateform.Yaxis.Stop();
            }
            catch (Exception ex) { }
        }

        private void BtnLZdec_MouseDown(object sender, MouseEventArgs e)
        {
            picLZdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_CleanPlateform.Zaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Zaxis.Negative();
                }
                else
                {
                    Global.LZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Zaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.LZmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnLZadd_MouseDown(object sender, MouseEventArgs e)
        {
            picLZadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_CleanPlateform.Zaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Zaxis.Postive();
                }
                else
                {
                    Global.LZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_CleanPlateform.Zaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.LZmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnLIZ_MouseUp(object sender, MouseEventArgs e)
        {
            picLZdec.BackColor = Color.Transparent;
            picLZadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_CleanPlateform.Zaxis.Stop();
            }
            catch (Exception ex) { }
        }

        private void BtnRXdec_MouseDown(object sender, MouseEventArgs e)
        {
            picRXdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Xaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Xaxis.Postive();
                }
                else
                {
                    Global.RXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Xaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.RXmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnRXadd_MouseDown(object sender, MouseEventArgs e)
        {
            picRXadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Xaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Xaxis.Negative();
                }
                else
                {
                    Global.RXmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Xaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.RXmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnRX_MouseUp(object sender, MouseEventArgs e)
        {
            picRXdec.BackColor = Color.Transparent;
            picRXadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_GluePlateform.Xaxis.Stop();
            }
            catch (Exception ex) { }
        }

        private void BtnRYdec_MouseDown(object sender, MouseEventArgs e)
        {
            picRYdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Yaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Yaxis.Postive();
                }
                else
                {
                    Global.RYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Yaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.RYmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }
        private void BtnRYadd_MouseDown(object sender, MouseEventArgs e)
        {
            picRYadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Yaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Yaxis.Negative();
                }
                else
                {
                    Global.RYmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Yaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.RYmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnRY_MouseUp(object sender, MouseEventArgs e)
        {
            picRYdec.BackColor = Color.Transparent;
            picRYadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_GluePlateform.Yaxis.Stop();
            }
            catch (Exception ex) { }
        }

        private void BtnRZdec_MouseDown(object sender, MouseEventArgs e)
        {
            picRZdec.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Zaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Zaxis.Negative();
                }
                else
                {
                    Global.RZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Zaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.RZmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnRZadd_MouseDown(object sender, MouseEventArgs e)
        {
            picRZadd.BackColor = Color.YellowGreen;
            if (Global.IsLocating) return;
            try
            {
                var JogSpeed = (double)tbrJogSpeed.Value;
                if (moveSelectHorizontal1.MoveMode.Continue)
                {
                    m_GluePlateform.Zaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Zaxis.Postive();
                }
                else
                {
                    Global.RZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
                    m_GluePlateform.Zaxis.MoveDelta(1 * moveSelectHorizontal1.MoveMode.Distance, Global.RZmanualSpeed);
                }
            }
            catch (Exception ex) { }
        }

        private void BtnRZ_MouseUp(object sender, MouseEventArgs e)
        {
            picRZdec.BackColor = Color.Transparent;
            picRZadd.BackColor = Color.Transparent;
            if (Global.IsLocating) return;
            try
            {
                if (!moveSelectHorizontal1.MoveMode.Continue) return;
                m_GluePlateform.Zaxis.Stop();
            }
            catch (Exception ex) { }
        }
        #endregion

        #region 轴使能操作
        private void chxLX_Click(object sender, EventArgs e)
        {
            m_CleanPlateform.Xaxis.IsServon = !m_CleanPlateform.Xaxis.IsServon;
        }

        private void chxLY_Click(object sender, EventArgs e)
        {
            m_CleanPlateform.Yaxis.IsServon = !m_CleanPlateform.Yaxis.IsServon;
        }

        private void chxLZ_Click(object sender, EventArgs e)
        {
            m_CleanPlateform.Zaxis.IsServon = !m_CleanPlateform.Zaxis.IsServon;
        }

        private void chxRX_Click(object sender, EventArgs e)
        {
            m_GluePlateform.Xaxis.IsServon = !m_GluePlateform.Xaxis.IsServon;
        }

        private void chxRY_Click(object sender, EventArgs e)
        {
            m_GluePlateform.Yaxis.IsServon = !m_GluePlateform.Yaxis.IsServon;
        }

        private void chxRZ_Click(object sender, EventArgs e)
        {
            m_GluePlateform.Zaxis.IsServon = !m_GluePlateform.Zaxis.IsServon;
        }
        #endregion

        #region 单按钮操作
        private void btnCleanOpen_Click(object sender, EventArgs e)
        {
            if(!IoPoints.IDO16.Value)
            {
                if (DialogResult.No == MessageBox.Show("是否清洗打开并保证是安全位置", "", MessageBoxButtons.YesNo)) return;

                if (m_CleanPlateform.Xaxis.CurrentPos >40 &&
                    m_CleanPlateform.Yaxis.CurrentPos > 10)
                {
                    IoPoints.IDO16.Value = true;
                    btnCleanOpen.Text = "清洁已打开";
                }
                else { MessageBox.Show("Y没有大于10或X位置没有大于40");return; }
            }
            else
            {
                IoPoints.IDO16.Value = false;
                btnCleanOpen.Text = "清洁已关闭";
            }
        }

        private void btnCleanCone_Click(object sender, EventArgs e)
        {
            //if (!m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.X)
            //    && !m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Y)
            //    && !m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Z))
            //{
            //    MessageBox.Show("未在镜筒清洗起始位！");
            //    return;
            //}
            Int32 Dimension = 2;
            Int32[] Axis_ID_Array_For_2Axes_ArcMove = new Int32[2] { m_CleanPlateform.Xaxis.NoId, m_CleanPlateform.Yaxis.NoId };
            // Int32[] Center_Pos_Array = new Int32[2] { 10000, 10000 };
            Int32[] Center_Pos_Array = new Int32[2] { Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.X/ AxisParameter.Instance.LXTransParams.PulseEquivalent),
                Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.Y  / AxisParameter.Instance.LYTransParams.PulseEquivalent) };//去掉了除以脉冲当量的计算
            Int32 Max_Arc_Speed =(int)Position.Instance.CleanPathSpeed* (int)AxisParameter.Instance.LYTransParams.EquivalentPulse;
            Int32 Angle = Position.Instance.HoldersCleanAngle;
            //IoPoints.m_ApsController.MoveArc2Absolute(m_CleanPlateform.Xaxis.NoId, m_CleanPlateform.Yaxis.NoId,
            //    Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.X / AxisParameter.Instance.LXTransParams.PulseEquivalent),
            //    Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.Y / AxisParameter.Instance.LYTransParams.PulseEquivalent),
            //    360, new VelocityCurve() { Maxvel = 5000 });
            //APS168.APS_absolute_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, Max_Arc_Speed, Angle);
            var step = 0;
            bool itrue = true;
            Stopwatch wath = new Stopwatch();
            wath.Start();
            while (itrue)
            {
                switch (step)
                {
                    case 0:
                        MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanConeFirstPosition.X, Global.RXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanConeFirstPosition.Y, Global.RYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanConeFirstPosition.Z, Global.RZmanualSpeed,
                                    () =>
                                    {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                        wath.Reset();
                        step = 10;
                        break;
                    case 10:
                        if (m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.X)
                            && m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Y)
                            && m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Z))
                        {
                            if (isUsePlasma) { IoPoints.IDO16.Value = true;Thread.Sleep(Position.Instance.CleanFireTime); }
                            APS168.APS_absolute_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, Max_Arc_Speed, Angle);
                            wath.Reset();
                            Thread.Sleep(200);
                            step = 20;
                        }
                        else if (wath.ElapsedMilliseconds/1000>60)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                    case 20:
                        if (m_CleanPlateform.Xaxis.IsDone && m_CleanPlateform.Yaxis.IsDone)
                        {
                            IoPoints.IDO16.Value = false;
                            MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanSafePosition.X, Global.RXmanualSpeed,
                                 m_CleanPlateform.Yaxis, Position.Instance.CleanSafePosition.Y, Global.RYmanualSpeed,
                                 m_CleanPlateform.Zaxis, Position.Instance.CleanSafePosition.Z, Global.RZmanualSpeed,
                                 () =>
                                 {
                                     return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                              | m_CleanPlateform.stationInitialize.InitializeDone;
                                 });
                           
                            step = 30;
                           
                        }
                        else if (wath.ElapsedMilliseconds / 1000 > 120)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                    case 30:
                        if (m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanSafePosition.X)
                           && m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanSafePosition.Y)
                           && m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z))
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        else if (wath.ElapsedMilliseconds / 1000 > 60)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                }
            }
        }

        private void btnCleanLens_Click(object sender, EventArgs e)
        {
            //if (!m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.X)
            //    && !m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Y)
            //    && !m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Z))
            //{
            //    MessageBox.Show("未在镜片清洗起始位！");
            //    return;
            //}
            //CalculateConeCenter();
            Int32 Dimension = 2;
            Int32[] Axis_ID_Array_For_2Axes_ArcMove = new Int32[2] { m_CleanPlateform.Xaxis.NoId, m_CleanPlateform.Yaxis.NoId };
            // Int32[] Center_Pos_Array = new Int32[2] { 10000, 10000 };
            Int32[] Center_Pos_Array = new Int32[2] { Convert.ToInt32(Position.Instance.CleanLensCenterPositionReal.X/ AxisParameter.Instance.LXTransParams.PulseEquivalent),
                Convert.ToInt32(Position.Instance.CleanLensCenterPositionReal.Y/ AxisParameter.Instance.LYTransParams.PulseEquivalent) };//去掉了除以脉冲当量的计算
            Int32 Max_Arc_Speed = (int)Position.Instance.CleanPathSpeed * (int)AxisParameter.Instance.LYTransParams.EquivalentPulse;
            Int32 Angle = Position.Instance.LensCleanAngle;
            //APS168.APS_absolute_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, Max_Arc_Speed, Angle);

            var step = 0;
            bool itrue = true;
            Stopwatch wath = new Stopwatch();
            wath.Start();
            while (itrue)
            {
                switch (step)
                {
                    case 0:
                        MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanLensFirstPosition.X, Global.LXmanualSpeed,
                                    m_CleanPlateform.Yaxis, Position.Instance.CleanLensFirstPosition.Y, Global.LYmanualSpeed,
                                    m_CleanPlateform.Zaxis, Position.Instance.CleanLensFirstPosition.Z, Global.LZmanualSpeed,
                                    () =>
                                    {
                                        return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                                 | m_CleanPlateform.stationInitialize.InitializeDone;
                                    });
                        wath.Reset();
                        step = 10;
                        break;
                    case 10:
                        if (m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.X)
                            && m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Y)
                            && m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Z))
                        {
                            if (isUsePlasma) { IoPoints.IDO16.Value = true; Thread.Sleep(Position.Instance.CleanFireTime); }
                            APS168.APS_absolute_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, Max_Arc_Speed, Angle);
                            wath.Reset();
                            Thread.Sleep(200);
                            step = 20;
                        }
                        else if (wath.ElapsedMilliseconds / 1000 > 60)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                    case 20:
                        if (m_CleanPlateform.Xaxis.IsDone && m_CleanPlateform.Yaxis.IsDone)
                        {
                            IoPoints.IDO16.Value = false;
                            MoveToPoint(m_CleanPlateform.Xaxis, Position.Instance.CleanSafePosition.X, Global.LXmanualSpeed,
                                 m_CleanPlateform.Yaxis, Position.Instance.CleanSafePosition.Y, Global.LYmanualSpeed,
                                 m_CleanPlateform.Zaxis, Position.Instance.CleanSafePosition.Z, Global.LZmanualSpeed,
                                 () =>
                                 {
                                     return !m_CleanPlateform.stationInitialize.Running | !m_CleanPlateform.stationOperate.Running
                                              | m_CleanPlateform.stationInitialize.InitializeDone;
                                 });

                            step = 30;

                        }
                        else if (wath.ElapsedMilliseconds / 1000 > 120)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                    case 30:
                        if (m_CleanPlateform.Xaxis.IsInPosition(Position.Instance.CleanSafePosition.X)
                           && m_CleanPlateform.Yaxis.IsInPosition(Position.Instance.CleanSafePosition.Y)
                           && m_CleanPlateform.Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z))
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        else if (wath.ElapsedMilliseconds / 1000 > 60)
                        {
                            wath.Reset();
                            itrue = false;
                            step = 40;
                        }
                        break;
                }
            }
            }

        private void btnGlueOpen_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO19.Value)
            {
                if (DialogResult.No == MessageBox.Show("是否在安全出胶地方打开出胶电磁阀", "", MessageBoxButtons.YesNo)) return;

                IoPoints.IDO19.Value = true;
                btnGlueOpen.Text = "点胶已打开";
            }
            else
            {
                IoPoints.IDO19.Value = false;
                btnGlueOpen.Text = "点胶已关闭";
            }
        }

        private void btnArcMove_Click(object sender, EventArgs e)
        {
            //CalculateGlueCenter();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 50;
                                break;
                            case 50:
                                MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueStartPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, Position.Instance.GlueStartPosition.Y, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, Position.Instance.GlueStartPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                Thread.Sleep(500);
                                step = 60;
                                break;
                            case 60://起始空胶
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueStartPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueStartPosition.Y)
                                    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueStartPosition.Z))
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                { (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                          (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                  (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                                    Thread.Sleep(1);
                                    step = 70;
                                }
                                break;
                            case 70://点胶第一圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, 360);
                                    if(isUseGlue) IoPoints.IDO19.Value = true;
                                    else IoPoints.IDO19.Value = false;
                                    //Thread.Sleep(1);
                                    step = 80;
                                }
                                break;
                            case 80://点胶第二圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                                    Thread.Sleep((int)Position.Instance.StopGlueDelay);
                                    step = 90;
                                }
                                break;
                            case 90://点胶拖胶
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    APS168.APS_absolute_move(m_GluePlateform.Zaxis.NoId, (int)((m_GluePlateform.Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                        1000);
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                                    Thread.Sleep(1);
                                    step = 100;
                                }
                                break;
                            case 100://点胶结束
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone &&
                                    m_GluePlateform.Xaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Yaxis.CurrentSpeed == 0 && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    
                                    step = 110;
                                }
                                break;
                            case 110://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 120;
                                break;
                            case 120:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    
                                    itrue = false;
                                    step = 0;
                                }
                                break;
                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private void btnConfirmNeedle_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"是否更新对针数据", "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;
            Position.Instance.NeedleOffset.X = m_GluePlateform.Xaxis.CurrentPos - Position.Instance.GlueAdjustPinPosition.X;
            Position.Instance.NeedleOffset.Y = m_GluePlateform.Yaxis.CurrentPos - Position.Instance.GlueAdjustPinPosition.Y;

            SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
        }

        //视觉点胶
        private void btnAirOpen_Click(object sender, EventArgs e)
        {
             if (DialogResult.No == MessageBox.Show("是否进行视觉点胶", "", MessageBoxButtons.YesNo)) return;

            try
            {

                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                }

            }
            catch { MessageBox.Show("加载视觉模板失败"); return; }
            AutoGlue();
        }

        private void btnGlueWeight_Click(object sender, EventArgs e)
        {
            CalculateGlueCenter();
            double offsetX = Position.Instance.WeightGluePosition.X - Position.Instance.GlueStartPosition.X;
            double offsetY = Position.Instance.WeightGluePosition.Y - Position.Instance.GlueStartPosition.Y;
            var step = 0;
            bool itrue = true;
            while (itrue)
            {
                switch (step)
                {
                    case 0://移至胶重点检位
                        MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.WeightGluePosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.WeightGluePosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.WeightGluePosition.Z, Global.RZmanualSpeed,
                                    () =>
                                    {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                        step = 5;
                        break;
                    case 5://起始空胶
                        if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.WeightGluePosition.X)
                            && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.WeightGluePosition.Y)
                            && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.WeightGluePosition.Z))
                        {
                            APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                        { (int)((Position.Instance.GlueCenterPosition.X + offsetX) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                          (int)((Position.Instance.GlueCenterPosition.Y + offsetY) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                          (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                            Thread.Sleep(1);
                            step = 10;
                        }
                        break;
                    case 10://点胶第一圈
                        if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                            && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                            && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                        {
                            APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                            { (int)((Position.Instance.GlueCenterPosition.X + offsetX) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y + offsetY) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                            (int)Position.Instance.GluePathSpeed * 1000, 360);
                            IoPoints.IDO19.Value = true;
                            //Thread.Sleep(1);
                            step = 20;
                        }
                        break;
                    case 20://点胶第二圈
                        if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                            && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                            && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                        {
                            APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                            {  (int)((Position.Instance.GlueCenterPosition.X + offsetX) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y + offsetY) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                            (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                            Thread.Sleep((int)Position.Instance.StopGlueDelay);
                            step = 30;
                        }
                        break;
                    case 30://点胶拖胶
                        if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                            && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                            && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                        {
                            IoPoints.IDO19.Value = false;
                            APS168.APS_absolute_move(m_GluePlateform.Zaxis.NoId, (int)((m_GluePlateform.Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                1000);
                            APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                            { (int)((Position.Instance.GlueCenterPosition.X + offsetX) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.GlueCenterPosition.Y + offsetY) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                            (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                            Thread.Sleep(1);
                            step = 40;
                        }
                        break;
                    case 40://点胶结束
                        if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone &&
                            m_GluePlateform.Xaxis.CurrentSpeed == 0
                            && m_GluePlateform.Yaxis.CurrentSpeed == 0 && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                        {
                            IoPoints.IDO19.Value = false;
                            itrue = false;
                            step = 0;
                        }
                        break;

                }
            }
        }

        private void btnTapping_Click(object sender, EventArgs e)
        {
            var step = 0;
            bool itrue = true;
            while (itrue)
            {
                switch (step)
                {
                    case 0://移至胶重点检位
                        MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.CutGlueStartPosition.X, Global.RXmanualSpeed,
                                    m_GluePlateform.Yaxis, Position.Instance.CutGlueStartPosition.Y, Global.RYmanualSpeed,
                                    m_GluePlateform.Zaxis, Position.Instance.CutGlueStartPosition.Z, Global.RZmanualSpeed,
                                    () =>
                                    {
                                        return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                 | m_GluePlateform.stationInitialize.InitializeDone;
                                    });
                        step = 5;
                        break;
                    case 5://起始空胶
                        if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.CutGlueStartPosition.X)
                            && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Y)
                            && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Z))
                        {
                            m_GluePlateform.Xaxis.MoveTo(Position.Instance.CutGlueEndPosition.X, Global.RXmanualSpeed);
                            m_GluePlateform.Yaxis.MoveTo(Position.Instance.CutGlueEndPosition.Y, Global.RYmanualSpeed);
                            Thread.Sleep(1);
                            step = 10;
                        }
                        break;
                    case 10:
                        if (m_CleanPlateform.Xaxis.IsDone && m_CleanPlateform.Yaxis.IsDone)
                        {
                            itrue = false;
                            step = 30;
                        }
                        break;
                }
            }
        }

        private void btnCleanXHome_Click(object sender, EventArgs e)
        {
            //if (m_CleanPlateform.Xaxis.IsServon)
            //    m_CleanPlateform.Xaxis.BackHome();
            if (!m_CleanPlateform.Xaxis.IsServon) return;
            if (m_CleanPlateform.Zaxis.CurrentPos == 0&&Marking.CleanZHomeFinish)
                m_CleanPlateform.BackHome(m_CleanPlateform.Xaxis, IoPoints.TDO3, IoPoints.TDI11);
            else MessageBox.Show("Z轴不在0位");
        }

        private void btnCleanXStop_Click(object sender, EventArgs e)
        {
            if (m_CleanPlateform.Xaxis.IsServon)
                m_CleanPlateform.Xaxis.Stop();
        }

        private void btnCleanYHome_Click(object sender, EventArgs e)
        {
           
            if (!m_CleanPlateform.Yaxis.IsServon) return;
            if (m_CleanPlateform.Zaxis.CurrentPos == 0 && Marking.CleanZHomeFinish)
                m_CleanPlateform.BackHome(m_CleanPlateform.Yaxis, IoPoints.TDO4, IoPoints.TDI12);
            else MessageBox.Show("Z轴不在0位");
        }

        private void btnCleanYStop_Click(object sender, EventArgs e)
        {
            if (m_CleanPlateform.Yaxis.IsServon)
                m_CleanPlateform.Yaxis.Stop();
        }

        private void btnCleanZHome_Click(object sender, EventArgs e)
        {
      
            if (!m_CleanPlateform.Zaxis.IsServon) return;
           m_CleanPlateform.BackHome(m_CleanPlateform.Zaxis,IoPoints.TDO5,IoPoints.TDI13);
          
        }

        private void btnCleanZStop_Click(object sender, EventArgs e)
        {
            if (m_CleanPlateform.Zaxis.IsServon)
                m_CleanPlateform.Zaxis.Stop();
        }

        private void btnGlueXHome_Click(object sender, EventArgs e)
        {
            //if (m_GluePlateform.Xaxis.IsServon)
            //    m_GluePlateform.Xaxis.BackHome();
            if (!m_GluePlateform.Xaxis.IsServon) return;
            if (m_GluePlateform.Zaxis.CurrentPos == 0 && Marking.GlueZHomeFinish)
                m_GluePlateform.BackHome(m_GluePlateform.Xaxis, IoPoints.TDO0, IoPoints.TDI8);
            else MessageBox.Show("Z轴不在0位");
        }

        private void btnGlueXStop_Click(object sender, EventArgs e)
        {
            if (m_GluePlateform.Xaxis.IsServon)
                m_GluePlateform.Xaxis.Stop();
        }

        private void btnGlueYHome_Click(object sender, EventArgs e)
        {
            //if (m_GluePlateform.Yaxis.IsServon)
            //    m_GluePlateform.Yaxis.BackHome();
            if (!m_GluePlateform.Yaxis.IsServon) return;
            if (m_GluePlateform.Zaxis.CurrentPos == 0 && Marking.GlueZHomeFinish)
                m_GluePlateform.BackHome(m_GluePlateform.Yaxis, IoPoints.TDO1, IoPoints.TDI9);
            else MessageBox.Show("Z轴不在0位");
        }

        private void btnGlueYStop_Click(object sender, EventArgs e)
        {
            if (m_GluePlateform.Yaxis.IsServon)
                m_GluePlateform.Yaxis.Stop();
        }

        private void btnGlueZHome_Click(object sender, EventArgs e)
        {
            //if (m_GluePlateform.Zaxis.IsServon)
            //    m_GluePlateform.Zaxis.BackHome();
            if (!m_GluePlateform.Zaxis.IsServon) return;
            m_GluePlateform.BackHome(m_GluePlateform.Zaxis, IoPoints.TDO2, IoPoints.TDI10);
        }

        private void btnGlueRStop_Click(object sender, EventArgs e)
        {
            if (m_GluePlateform.Zaxis.IsServon)
                m_GluePlateform.Zaxis.Stop();
        }

        #endregion

        private int MoveToPoint(ServoAxis Xaxis, double X, VelocityCurve XvelocityCurve, ServoAxis Zaxis, double Z, VelocityCurve ZvelocityCurve, Func<bool> Condition = null)
        {
            if (!Xaxis.IsServon || !Zaxis.IsServon) return -3;
            if (!Condition()) return -4;
            Global.IsLocating = true;
            Task.Factory.StartNew(() =>
            {
                try
                {            //判断Z轴是否在零点
                    if (!Zaxis.IsInPosition(0.5))
                        Zaxis.MoveTo(0.5, ZvelocityCurve);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (Zaxis.IsInPosition(0.5)) break;
                        if (ServoAxisIsReady(Zaxis) || ServoAxisIsReady(Zaxis))
                        {
                            Zaxis.Stop();
                            Global.IsLocating = false;
                            return -2;
                        }
                    }
                    //将X、Y移动到指定位置
                    if (!Xaxis.IsInPosition(X)) Xaxis.MoveTo(X, XvelocityCurve);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (Xaxis.IsInPosition(X)) break;
                        if (ServoAxisIsReady(Xaxis))
                        {
                            Xaxis.Stop();
                            Global.IsLocating = false;
                            return -2;
                        }
                    }
                    //将Z轴移动到指定位置
                    Zaxis.MoveTo(Z, ZvelocityCurve);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (Zaxis.IsInPosition(Z)) break;
                        if (ServoAxisIsReady(Zaxis))
                        {
                            Zaxis.Stop();
                            Global.IsLocating = false;
                            return -2;
                        }
                    }
                    Global.IsLocating = false;
                    return 0;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    //log.Fatal("设备驱动程序异常", ex);
                    return -2;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return 0;
        }

        private void btnNeedleCalib_Click(object sender, EventArgs e)
        {
            var watchPointCT = new Stopwatch();
            watchPointCT.Start();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var vsstep = 0;
                    int Speed, Value, X0 = 0, X1 = 0, Y0 = 0, Y1 = 0, Z0 = 0;
                    while (true)
                    {
                        if (m_GluePlateform.Xaxis.IsAlarmed || m_GluePlateform.Xaxis.IsEmg || !m_GluePlateform.Xaxis.IsServon)
                        {
                            m_GluePlateform.Xaxis.Stop();
                            LogHelper.Debug("点胶X轴异常停止，请复位！");
                            return;
                        }
                        if (m_GluePlateform.Yaxis.IsAlarmed || m_GluePlateform.Yaxis.IsEmg || !m_GluePlateform.Yaxis.IsServon)
                        {
                            m_GluePlateform.Yaxis.Stop();
                            LogHelper.Debug("点胶Y轴异常停止，请复位！");
                            return;

                        }
                        if (m_GluePlateform.Zaxis.IsAlarmed || m_GluePlateform.Zaxis.IsEmg || !m_GluePlateform.Zaxis.IsServon)
                        {
                            m_GluePlateform.Zaxis.Stop();
                            LogHelper.Debug("点胶Z轴异常停止，请复位！");
                            return;

                        }
                        if (m_GluePlateform.stationOperate.Status == StationStatus.模组报警)
                        {
                            MessageBox.Show("模组报警中");
                            return;
                        }

                        switch (vsstep)
                        {

                            case 0://移动到对针位置
                                MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueAdjustPinPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, Position.Instance.GlueAdjustPinPosition.Y, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, Position.Instance.GlueAdjustPinPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                vsstep = 1;
                                break;
                            case 1:

                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.Y)
                                    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.Z))
                                {
                                    vsstep = 5;
                                }
                                break;
                            case 5://判断X方向是否位于感应区
                                if (IoPoints.IDI27.Value)
                                {
                                    watchPointCT.Restart();
                                    vsstep = 10;
                                }
                                else
                                {
                                    vsstep = 200;//判断Y方向是否在感应区
                                }
                                break;
                            case 10:
                                if (m_GluePlateform.Xaxis.CurrentPos < Position.Instance.GlueAdjustPinPosition.X + 5)
                                {
                                    Speed = 5000;
                                    Value = 1;
                                    Value *= 1;
                                    APS168.APS_relative_move(0, Value, Speed);
                                    vsstep = 20;
                                }
                                else
                                {
                                    LogHelper.Debug("点胶X轴正向偏移值不够,开始负向偏移！");
                                    vsstep = 100;
                                }
                                break;
                            case 20:
                                if (!IoPoints.IDI27.Value)
                                {

                                }
                                break;
                            case 170://XY轴移到位对针OK
                                break;
                            case 180: //Z返回原位
                                vsstep = 0;
                                return;

                            default:
                                vsstep = 0;
                                return;
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private void btnAAYHome_Click(object sender, EventArgs e)
        {
            //if (m_AAPlateform.MoveSixYaxis.IsServon)
            //    m_AAPlateform.MoveSixYaxis.BackHome();
        }

        private void btnAAZHome_Click(object sender, EventArgs e)
        {
            //需要防呆
            //if (IoPoints.IDI923.Value) { MessageBox.Show("平行光管支架到位感应无法回零！");return; }
            //if (m_AAPlateform.LightZaxis.IsServon)
            //    m_AAPlateform.LightZaxis.BackHome();
        }

        private void btnAAYStop_Click(object sender, EventArgs e)
        {
            //if (m_AAPlateform.MoveSixYaxis.IsServon)
            //    m_AAPlateform.MoveSixYaxis.Stop();
        }

        private void btnAAZStop_Click(object sender, EventArgs e)
        {
            //if (m_AAPlateform.LightZaxis.IsServon)
            //    m_AAPlateform.LightZaxis.Stop();
        }

        private void chxAAY_CheckedChanged(object sender, EventArgs e)
        {
            //m_AAPlateform.MoveSixYaxis.IsServon = !m_AAPlateform.MoveSixYaxis.IsServon;
        }

        private void chxAAZ_CheckedChanged(object sender, EventArgs e)
        {
            //m_AAPlateform.LightZaxis.IsServon = !m_AAPlateform.LightZaxis.IsServon;
        }

        private void btnCleanMotionPostive_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO9.Value)
            {
                IoPoints.IDO9.Value = true;
                btnCleanMotionPostive.Text = "清洗送料线电机正转";
            }
            else
            {
                IoPoints.IDO9.Value = false;
                btnCleanMotionPostive.Text = "清洗送料线电机停止";
            }
        }

        private void btnGlueBackMotionIN_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO8.Value)
            {
                IoPoints.IDO8.Value = true;
                btnGlueBackMotionIN.Text = "点胶回流线电机反转";
            }
            else
            {
                IoPoints.IDO8.Value = false;
                btnGlueBackMotionIN.Text = "点胶回流线电机停止";
            }
        }

        private void btnAAPositvie_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO90.Value)
            {
                IoPoints.IDO90.Value = true;
                btnAAPositvie.Text = "AA送料线电机正转";
            }
            else
            {
                IoPoints.IDO90.Value = false;
                btnAAPositvie.Text = "AA送料线电机停止";
            }
        }

        private void btnAABackIN_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO914.Value)
            {
                IoPoints.IDO914.Value = true;
                btnAABackIN.Text = "AA回流线电机反转";
            }
            else
            {
                IoPoints.IDO914.Value = false;
                btnAABackIN.Text = "AA回流线电机停止";
            }
        }

        private void btnGlueWbContol_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO14.Value)
            {
                IoPoints.IDO14.Value = true;
                btnGlueWbContol.Text = "已启动点胶段解串板电源";
            }
            else
            {
                IoPoints.IDO14.Value = false;
                btnGlueWbContol.Text = "已关闭点胶段解串板电源";
            }
        }

        private void btnDoor_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO22.Value)
            {
                IoPoints.IDO22.Value = true;
                btnDoor.Text = "门禁线圈打开";
            }
            else
            {
                IoPoints.IDO22.Value = false;
                btnDoor.Text = "门禁线圈关闭";
            }
        }

        private void btnLight_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO23.Value)
            {
                IoPoints.IDO23.Value = true;
                btnLight.Text = "照明灯管打开";
            }
            else
            {
                IoPoints.IDO23.Value = false;
                btnLight.Text = "照明灯管关闭";
            }
        }

        private void btnRunPlasma_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO11.Value)
            {
                IoPoints.IDO11.Value = true;
                btnRunPlasma.Text = "Plasma上电";
            }
            else
            {
                IoPoints.IDO11.Value = false;
                btnRunPlasma.Text = "Plasma上电关闭";
            }
        }

        private void btnAAzDec_MouseDown(object sender, MouseEventArgs e)
        {
            //if (IoPoints.IDI923.Value) { MessageBox.Show("平行光管支架到位感应不能操作！");return; }
            ////picAAZdec.BackColor = Color.YellowGreen;
            //if (Global.IsLocating) return;
            //try
            //{
            //    var JogSpeed = (double)tbrJogSpeed.Value;
            //    if (moveSelectHorizontal1.MoveMode.Continue)
            //    {
            //        m_AAPlateform.LightZaxis.Speed = JogSpeed == 0 ? 10 : JogSpeed;
            //        m_AAPlateform.LightZaxis.Negative();
            //    }
            //    else
            //    {
            //        Global.AAZmanualSpeed.Maxvel = JogSpeed == 0 ? 10 : JogSpeed;
            //        m_AAPlateform.LightZaxis.MoveDelta(-1 * moveSelectHorizontal1.MoveMode.Distance, Global.AAZmanualSpeed);
            //    }
            //}
            //catch (Exception ex) { }
        }

        private void btnAAzDec_MouseUp(object sender, MouseEventArgs e)
        {
            //picAAZdec.BackColor = Color.Transparent;
            //picAAZadd.BackColor = Color.Transparent;
            //if (Global.IsLocating) return;
            //try
            //{
            //    if (!moveSelectHorizontal1.MoveMode.Continue) return;
            //    m_AAPlateform.LightZaxis.Stop();
            //}
            //catch (Exception ex) { }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //m_MES.HeightDectector.WriteDetectHeightCommand();
        }

        private void btnOpenCyl_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO13.Value)
            {
                IoPoints.IDO13.Value = true;
                btnOpenCyl.Text = "总气阀打开";
            }
            else
            {
                IoPoints.IDO13.Value = false;
                btnOpenCyl.Text = "总气阀关闭";
            }
        }

        private void btnCalib_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否进行自动点胶拍照标定", "", MessageBoxButtons.YesNo)) return;
            
            try
            {
                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.CalibGlueVisionParam.strModelPath}");
               m_GluePlateform.CalibglueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.CalibGlueVisionParam.LightControlvalue);
                }
            }
            catch { MessageBox.Show("加载标定模板失败");return; }

            CalibAuto();
        }

        private void CalibAuto()
        {
            Point3D<double> GlueStartPosition, GlueSecondPosition, GlueThirdPositon;
            GlueStartPosition.X = Position.Instance.VisionCalibGluePosition.X - Position.Instance.GlueRadius;
            GlueStartPosition.Y = Position.Instance.VisionCalibGluePosition.Y;
           
            GlueSecondPosition.X = Position.Instance.VisionCalibGluePosition.X;
            GlueSecondPosition.Y = Position.Instance.VisionCalibGluePosition.Y - Position.Instance.GlueRadius;
            GlueThirdPositon.X = Position.Instance.VisionCalibGluePosition.X + Position.Instance.GlueRadius;
            GlueThirdPositon.Y = Position.Instance.VisionCalibGluePosition.Y;

            GlueStartPosition.Z = Position.Instance.VisionCalibGluePosition.Z;
            GlueSecondPosition.Z = Position.Instance.VisionCalibGluePosition.Z;
            GlueThirdPositon.Z = Position.Instance.VisionCalibGluePosition.Z;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;
                            case 20:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {

                                    step = 50;
                                }
                                break;
                            case 50:
                                MoveToPoint(m_GluePlateform.Xaxis, GlueStartPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, GlueStartPosition.Y, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, GlueStartPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                Thread.Sleep(100);
                                step = 60;
                                break;
                            case 60://起始空胶
                                if (m_GluePlateform.Xaxis.IsInPosition(GlueStartPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(GlueStartPosition.Y)
                                    && m_GluePlateform.Zaxis.IsInPosition(GlueStartPosition.Z))
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                { (int)((Position.Instance.VisionCalibGluePosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                          (int)((Position.Instance.VisionCalibGluePosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                  (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                                    Thread.Sleep(1);
                                    step = 70;
                                }
                                break;
                            case 70://点胶第一圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.VisionCalibGluePosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.VisionCalibGluePosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, 360);
                                    if (isUseGlue) IoPoints.IDO19.Value = true;
                                    //Thread.Sleep(1);
                                    step = 80;
                                }
                                break;
                            case 80://点胶第二圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.VisionCalibGluePosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.VisionCalibGluePosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                                    Thread.Sleep((int)Position.Instance.StopGlueDelay);
                                    step = 90;
                                }
                                break;
                            case 90://点胶拖胶
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    APS168.APS_absolute_move(m_GluePlateform.Zaxis.NoId, (int)((m_GluePlateform.Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                        1000);
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((Position.Instance.VisionCalibGluePosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((Position.Instance.VisionCalibGluePosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                                    Thread.Sleep(1);
                                    step = 100;
                                }
                                break;
                            case 100://点胶结束
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone &&
                                    m_GluePlateform.Xaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Yaxis.CurrentSpeed == 0 && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    //itrue = false;
                                    step = 110;
                                }
                                break;
                            case 110:
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 120;
                                break;
                            case 120:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.Y, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Y))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Y)
                                     && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Z))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                    //触发相机拍照
                                    if (m_GluePlateform.baslerCamera.ho_Image != null)
                                    {
                                        if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                    }
                                    else
                                    {
                                        HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                    }
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                    {
                                        Marking.LoadOffset.Result = "";
                                        m_GluePlateform.CalibglueVisionClass.VisionResult.Result = "";
                                         m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                        log.Info($"相机触发拍照！");

                                    step = 170;
                                    }
                                break;
                            case 170://接收数据
                             
                                    //判断相机是否读到图像，加载图像，运行处理
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                    {
                                        try
                                        {
                                        /// Task.Run(() => {
                                        m_GluePlateform.CalibglueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.CalibglueVisionClass.VisionRun(DbModelParam.Instance.CalibGlueVisionParam, 
                                                Relationship.Instance.CameraCalib);
                                            Marking.LoadOffset.Result = m_GluePlateform.CalibglueVisionClass.VisionResult.Result;
                                            Marking.LoadOffset.X = m_GluePlateform.CalibglueVisionClass.VisionResult.Column;
                                            Marking.LoadOffset.Y = m_GluePlateform.CalibglueVisionClass.VisionResult.Row;
                                            Marking.LoadOffset.A = m_GluePlateform.CalibglueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 180;
                                        }
                                        catch
                                        {
                                            Marking.LoadOffset.Result = "NG";
                                            Marking.LoadOffset.X = 0.0;
                                            Marking.LoadOffset.Y = 0.0;
                                            Marking.LoadOffset.A = 0.0;
                                            log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                    }
                          
                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 200;
                                        log.Debug($"OK");
                                       
                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;
                            case 200://计算
                                var X= Position.Instance.VisionCalibGluePosition.X - Position.Instance.GlueCameraCalibPosition.X - Marking.LoadOffset.X;
                                var Y = Position.Instance.VisionCalibGluePosition.Y - Position.Instance.GlueCameraCalibPosition.Y - Marking.LoadOffset.Y;
                                this.Invoke(new Action(()=>{
                                    tbX.Text = X.ToString("0.000");
                                    tbY.Text = Y.ToString("0.000");
                                  }));                              
                                step = 0;
                                itrue = false;
                                log.Debug($"标定OK");
                                break;
                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private void CalibAutoYanZheng()
        {
            Point3D<double> GlueCenterPosition,GlueStartPosition, GlueSecondPosition, GlueThirdPositon;

            GlueCenterPosition.X = Position.Instance.GlueCameraCalibPosition.X - AxisParameter.Instance.CameraAndNeedleOffset.X;
            GlueCenterPosition.Y = Position.Instance.GlueCameraCalibPosition.Y - AxisParameter.Instance.CameraAndNeedleOffset.Y;
          

            GlueStartPosition.X = Position.Instance.GlueCenterPosition.X - Position.Instance.GlueRadius;
            GlueStartPosition.Y = Position.Instance.GlueCenterPosition.Y;

            GlueSecondPosition.X = Position.Instance.GlueCenterPosition.X;
            GlueSecondPosition.Y = Position.Instance.GlueCenterPosition.Y - Position.Instance.GlueRadius;
            GlueThirdPositon.X = Position.Instance.GlueCenterPosition.X + Position.Instance.GlueRadius;
            GlueThirdPositon.Y = Position.Instance.GlueCenterPosition.Y;

            GlueStartPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueSecondPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueThirdPositon.Z = Position.Instance.GlueCenterPosition.Z;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;
                         
                            case 20:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.Y, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Y))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraCalibPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Y)
                                     && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraCalibPosition.Z))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.LoadOffset.Result = "";
                                    m_GluePlateform.CalibglueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 170;
                                }
                                break;
                            case 170://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        m_GluePlateform.CalibglueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.CalibglueVisionClass.VisionRun(DbModelParam.Instance.CalibGlueVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = m_GluePlateform.CalibglueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = m_GluePlateform.CalibglueVisionClass.VisionResult.Column;
                                        Marking.LoadOffset.Y = m_GluePlateform.CalibglueVisionClass.VisionResult.Row;
                                        Marking.LoadOffset.A = m_GluePlateform.CalibglueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 180;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 200;
                                        var X = Position.Instance.GlueCameraCalibPosition.X + AxisParameter.Instance.CameraAndNeedleOffset.X + Marking.LoadOffset.X;
                                        var Y = Position.Instance.GlueCameraCalibPosition.Y + AxisParameter.Instance.CameraAndNeedleOffset.Y + Marking.LoadOffset.Y;
                                        lblCalib.Text = $"Msg:验证数据X:{X}，Y{Y}";
                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;
                            case 200://计算
                                
                                step = 0;
                                itrue = false;
                               
                                break;
                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 自动拍照位 定位并点胶
        /// </summary>
        private void AutoGlue()
        {
            Point3D<double> GlueCenterPosition, GlueStartPosition, GlueSecondPosition, GlueThirdPositon;

            GlueCenterPosition.X = Position.Instance.GlueCameraCalibPosition.X - AxisParameter.Instance.CameraAndNeedleOffset.X;
            GlueCenterPosition.Y = Position.Instance.GlueCameraCalibPosition.Y - AxisParameter.Instance.CameraAndNeedleOffset.Y;


            GlueStartPosition.X = Position.Instance.GlueCenterPosition.X - Position.Instance.GlueRadius;
            GlueStartPosition.Y = Position.Instance.GlueCenterPosition.Y;

            GlueSecondPosition.X = Position.Instance.GlueCenterPosition.X;
            GlueSecondPosition.Y = Position.Instance.GlueCenterPosition.Y - Position.Instance.GlueRadius;
            GlueThirdPositon.X = Position.Instance.GlueCenterPosition.X + Position.Instance.GlueRadius;
            GlueThirdPositon.Y = Position.Instance.GlueCenterPosition.Y;

            GlueStartPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueSecondPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueThirdPositon.Z = Position.Instance.GlueCenterPosition.Z;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;

                            case 20:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraPosition.Y, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y))
                                {
                                     m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测  定位位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y)
                                     && (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z)))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.LoadOffset.Result = "";
                                    m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 170;
                                }
                                break;
                            case 170://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = m_GluePlateform.glueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = m_GluePlateform.glueVisionClass.VisionResult.Column;
                                        Marking.LoadOffset.Y = m_GluePlateform.glueVisionClass.VisionResult.Row;
                                        Marking.LoadOffset.A = m_GluePlateform.glueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 180;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 190;
                                        var X = Position.Instance.GlueCameraPosition.X + AxisParameter.Instance.CameraAndNeedleOffset.X + Marking.LoadOffset.X + AxisParameter.Instance.GlueOffsetX;
                                        var Y = Position.Instance.GlueCameraPosition.Y + AxisParameter.Instance.CameraAndNeedleOffset.Y + Marking.LoadOffset.Y + AxisParameter.Instance.GlueOffsetY;
                                        GlueCenterPosition.X = X;
                                        GlueCenterPosition.Y = Y;
                                        GlueStartPosition.X = GlueCenterPosition.X - Position.Instance.GlueRadius;
                                        GlueStartPosition.Y = GlueCenterPosition.Y;
                                        GlueStartPosition.Z =Position.Instance.GlueHeight;
                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;
                            case 190:
                                MoveToPoint(m_GluePlateform.Xaxis, GlueStartPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, GlueStartPosition.Y, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, GlueStartPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                Thread.Sleep(500);
                                step = 200;
                                break;
                            case 200://起始空胶
                                if (m_GluePlateform.Xaxis.IsInPosition(GlueStartPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(GlueStartPosition.Y)
                                    && m_GluePlateform.Zaxis.IsInPosition(GlueStartPosition.Z))
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                { (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                          (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                  (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                                    Thread.Sleep(1);
                                    step = 210;
                                }
                                break;
                            case 210://点胶第一圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, 360);
                                    if (isUseGlue) IoPoints.IDO19.Value = true;
                                    else IoPoints.IDO19.Value = false;
                                    Thread.Sleep(1);
                                    step = 220;
                                }
                                break;
                            case 220://点胶第二圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                                    Thread.Sleep((int)Position.Instance.StopGlueDelay);
                                    step = 230;
                                }
                                break;
                            case 230://点胶拖胶
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    APS168.APS_absolute_move(m_GluePlateform.Zaxis.NoId, (int)((m_GluePlateform.Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                        1000);
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                                    Thread.Sleep(1);
                                    step = 240;
                                }
                                break;
                            case 240://点胶结束
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone &&
                                    m_GluePlateform.Xaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Yaxis.CurrentSpeed == 0 && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;

                                    step = 250;
                                }
                                break;
                            case 250://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 260;
                                break;
                            case 260:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {

                                    itrue = false;
                                    step = 0;
                                }
                                break;
 
                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// 自动拍照位 定位并点胶 至开始位 zero
        /// </summary>
        private void AutoGlueZeroStartPos()
        {
            Point3D<double> GlueCenterPosition, GlueStartPosition, GlueSecondPosition, GlueThirdPositon;

            GlueCenterPosition.X = Position.Instance.GlueCameraCalibPosition.X - AxisParameter.Instance.CameraAndNeedleOffset.X;
            GlueCenterPosition.Y = Position.Instance.GlueCameraCalibPosition.Y - AxisParameter.Instance.CameraAndNeedleOffset.Y;


            GlueStartPosition.X = Position.Instance.GlueCenterPosition.X - Position.Instance.GlueRadius;
            GlueStartPosition.Y = Position.Instance.GlueCenterPosition.Y;

            GlueSecondPosition.X = Position.Instance.GlueCenterPosition.X;
            GlueSecondPosition.Y = Position.Instance.GlueCenterPosition.Y - Position.Instance.GlueRadius;
            GlueThirdPositon.X = Position.Instance.GlueCenterPosition.X + Position.Instance.GlueRadius;
            GlueThirdPositon.Y = Position.Instance.GlueCenterPosition.Y;

            GlueStartPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueSecondPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueThirdPositon.Z = Position.Instance.GlueCenterPosition.Z;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;

                            case 20:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraPosition.Y, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测  定位位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y)
                                     && (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z)))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.LoadOffset.Result = "";
                                    m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 170;
                                }
                                break;
                            case 170://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = m_GluePlateform.glueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = m_GluePlateform.glueVisionClass.VisionResult.Column;
                                        Marking.LoadOffset.Y = m_GluePlateform.glueVisionClass.VisionResult.Row;
                                        Marking.LoadOffset.A = m_GluePlateform.glueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 180;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 190;
                                        var X = Position.Instance.GlueCameraPosition.X + AxisParameter.Instance.CameraAndNeedleOffset.X + Marking.LoadOffset.X+ AxisParameter.Instance.GlueOffsetX;
                                        var Y = Position.Instance.GlueCameraPosition.Y + AxisParameter.Instance.CameraAndNeedleOffset.Y + Marking.LoadOffset.Y + AxisParameter.Instance.GlueOffsetY;
                                        GlueCenterPosition.X = X;
                                        GlueCenterPosition.Y = Y;
                                        GlueStartPosition.X = GlueCenterPosition.X - Position.Instance.GlueRadius;
                                        GlueStartPosition.Y = GlueCenterPosition.Y;
                                        GlueStartPosition.Z = Position.Instance.GlueHeight;
                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;
                            case 190://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 200;
                                break;
                            case 200:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    step = 210;
                                }
                                break;
                            case 210:
                                m_GluePlateform.Xaxis.MoveTo(GlueStartPosition.X, Global.RXmanualSpeed);
                                m_GluePlateform.Yaxis.MoveTo(GlueStartPosition.Y, Global.RYmanualSpeed);
                                Thread.Sleep(500);
                                step = 220;
                                break;
                            case 220:
                                if (m_GluePlateform.Xaxis.IsInPosition(GlueStartPosition.X)&& m_GluePlateform.Yaxis.IsInPosition(GlueStartPosition.Y))
                                {

                                    itrue = false;
                                    step = 0;
                                }
                                break;

                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }
        private void btnCalibYZ_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否进行标定验证", "", MessageBoxButtons.YesNo)) return;

            try
            {
                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.CalibGlueVisionParam.strModelPath}");
                m_GluePlateform.CalibglueVisionClass.ReadShapeModel(strLensmodelpath);

            }
            catch { MessageBox.Show("加载标定模板失败"); return; }
            CalibAutoYanZheng();
        }

        private void cbUseGlue_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseGlue.Checked)
            {
                isUseGlue = true;
            }
            else
                isUseGlue = false;
        }

        private void btnAAOpenWb_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO915.Value)
            {
                IoPoints.IDO915.Value = true;
                btnAAOpenWb.Text = "已启动AA段解串板电源";
            }
            else
            {
                IoPoints.IDO915.Value = false;
                btnAAOpenWb.Text = "已关闭AA段解串板电源";
            }
        }

        private void cbUsePlasma_CheckedChanged(object sender, EventArgs e)
        {
            if(cbUsePlasma.Checked)
            {
                if (DialogResult.No == MessageBox.Show("是否清洗打开并保证是安全位置", "", MessageBoxButtons.YesNo)) { cbUsePlasma.Checked = false; return; }
                isUsePlasma = true;
            }
            else isUsePlasma = false;
        }

        private void btnVisonGlueZero_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否进行视觉点胶", "", MessageBoxButtons.YesNo)) return;

            try
            {

                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                }

            }
            catch { MessageBox.Show("加载标定模板失败"); return; }
            AutoGlueZeroStartPos();
        }

        private void btnSaveCalib_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show($"是否保存当前标定值X:{tbX.Text}Y:{tbY.Text}以及定位偏差X{nudOffsetX.Value},Y{nudOffsetY.Value}", "", MessageBoxButtons.YesNo)) return;
            AxisParameter.Instance.CameraAndNeedleOffset.X = Convert.ToDouble(tbX.Text);
            AxisParameter.Instance.CameraAndNeedleOffset.Y = Convert.ToDouble(tbY.Text);
            AxisParameter.Instance.GlueOffsetX = Convert.ToDouble(nudOffsetX.Value);
            AxisParameter.Instance.GlueOffsetY = Convert.ToDouble(nudOffsetY.Value);
            Position.Instance.GlueHeight = (double)ndnGlueHeight.Value;
            SerializerManager<AxisParameter>.Instance.Save(AppConfig.ConfigAxisName, AxisParameter.Instance);
            SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
            MessageBox.Show("保存标定值成功");

        }

        private void btnCalibStop_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show($"是否静态拍照标定", "", MessageBoxButtons.YesNo)) return;
            if (m_GluePlateform.Xaxis.CurrentPos== Position.Instance.GlueCameraCalibPosition.X&& m_GluePlateform.Yaxis.CurrentPos ==Position.Instance.GlueCameraCalibPosition.Y)
            {
                try
                {
                    var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.CalibGlueVisionParam.strModelPath}");
                    m_GluePlateform.CalibglueVisionClass.ReadShapeModel(strLensmodelpath);
                    if (m_GluePlateform.lightControl != null)
                    {
                        m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.CalibGlueVisionParam.LightControlvalue);
                    }
                }
                catch { MessageBox.Show("加载标定模板失败"); return; }
                m_GluePlateform.baslerCamera.ho_Image.Dispose();
                m_GluePlateform.baslerCamera.SendSoftwareExecute();
                Thread.Sleep(300);
                m_GluePlateform.CalibglueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                m_GluePlateform.CalibglueVisionClass.VisionRun(DbModelParam.Instance.CalibGlueVisionParam,
                        Relationship.Instance.CameraCalib);
                Marking.LoadOffset.Result = m_GluePlateform.CalibglueVisionClass.VisionResult.Result;
                Marking.LoadOffset.X = m_GluePlateform.CalibglueVisionClass.VisionResult.Column;
                Marking.LoadOffset.Y = m_GluePlateform.CalibglueVisionClass.VisionResult.Row;
                Marking.LoadOffset.A = m_GluePlateform.CalibglueVisionClass.VisionResult.Angle;
                var X = Position.Instance.VisionCalibGluePosition.X - Position.Instance.GlueCameraCalibPosition.X - Marking.LoadOffset.X;
                var Y = Position.Instance.VisionCalibGluePosition.Y - Position.Instance.GlueCameraCalibPosition.Y - Marking.LoadOffset.Y;
                this.Invoke(new Action(() => {
                    tbX.Text = X.ToString("0.000");
                    tbY.Text = Y.ToString("0.000");
                }));

            }
            else MessageBox.Show($"不在标定拍照位");
        }

        private void btnGlueUSeSetZero_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show($"是否胶水当前次数清零", "", MessageBoxButtons.YesNo)) return;
            Config.Instance.GlueUseNumsIndex = 0;
            nudGlueUseIndex.Value = 0;
            SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
            if (DialogResult.Yes == MessageBox.Show($"是否同时当前时间设为胶水使用开始记录时间", "", MessageBoxButtons.YesNo)) Config.Instance.GlueDataTime = DateTime.Now;
            
        }

        private void btnGlueUseNewSet_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show($"是否重设胶水参数", "", MessageBoxButtons.YesNo)) return;
            Config.Instance.GlueUseNumsIndex = (int)nudGlueUseIndex.Value;
            Config.Instance.GlueUseAllNums = (int)nudGlueUseAllNums.Value;
            Config.Instance.GlueUseAlarmNums = (int)nudGlueUseAlarmNums.Value;
            Config.Instance.GlueTimeAlarm = (int)nudGlueHAveUseTime.Value;
            SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
            MessageBox.Show($"已设置胶水当前使用次数为{Config.Instance.GlueUseNumsIndex},胶水报警次数{Config.Instance.GlueUseAlarmNums},胶水使用总次数{Config.Instance.GlueUseAllNums},胶水使用时间大于{Config.Instance.GlueTimeAlarm}将报警提示");
        }

        private void btnCamerXY_Click(object sender, EventArgs e)
        {
            //if (m_GluePlateform.stationInitialize.InitializeDone == false) { MessageBox.Show("未复位完成！"); return; }
            if (DialogResult.No == MessageBox.Show($"是否解析度标定", "", MessageBoxButtons.YesNo)) return;

            try
            {

                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                }

            }
            catch { MessageBox.Show("加载视觉模板失败"); return; }
            AutoCamera_mm();

        }

        private void btnGlueAndFind_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否进行视觉点胶", "", MessageBoxButtons.YesNo)) return;

            try
            {

                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                }

            }
            catch { MessageBox.Show("加载视觉模板失败"); return; }
            AutoGlueFind();
        }
        /// <summary>
        /// 自动拍照位 定位并点胶
        /// </summary>
        private void AutoGlueFind()
        {
            Point3D<double> GlueCenterPosition, GlueStartPosition, GlueSecondPosition, GlueThirdPositon;

            GlueCenterPosition.X = Position.Instance.GlueCameraCalibPosition.X - AxisParameter.Instance.CameraAndNeedleOffset.X;
            GlueCenterPosition.Y = Position.Instance.GlueCameraCalibPosition.Y - AxisParameter.Instance.CameraAndNeedleOffset.Y;


            GlueStartPosition.X = Position.Instance.GlueCenterPosition.X - Position.Instance.GlueRadius;
            GlueStartPosition.Y = Position.Instance.GlueCenterPosition.Y;

            GlueSecondPosition.X = Position.Instance.GlueCenterPosition.X;
            GlueSecondPosition.Y = Position.Instance.GlueCenterPosition.Y - Position.Instance.GlueRadius;
            GlueThirdPositon.X = Position.Instance.GlueCenterPosition.X + Position.Instance.GlueRadius;
            GlueThirdPositon.Y = Position.Instance.GlueCenterPosition.Y;

            GlueStartPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueSecondPosition.Z = Position.Instance.GlueCenterPosition.Z;
            GlueThirdPositon.Z = Position.Instance.GlueCenterPosition.Z;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;

                            case 20:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraPosition.Y, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测  定位位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y)
                                     && (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z)))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.LoadOffset.Result = "";
                                    m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 170;
                                }
                                break;
                            case 170://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        ///  ho_ImageT.Dispose();


                                        m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = m_GluePlateform.glueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = m_GluePlateform.glueVisionClass.VisionResult.Column;
                                        Marking.LoadOffset.Y = m_GluePlateform.glueVisionClass.VisionResult.Row;
                                        Marking.LoadOffset.A = m_GluePlateform.glueVisionClass.VisionResult.Angle;
                                        Center.X = m_GluePlateform.glueVisionClass.VisionResult.Column_Pixel;
                                        Center.Y = m_GluePlateform.glueVisionClass.VisionResult.Row_Pixel;
                                        // });


                                        step = 180;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        //if (Config.Instance.IsTeachGlueFind) step = 260;
                                        //else step = 190;

                                        step = 260;

                                        var X = Position.Instance.GlueCameraPosition.X + AxisParameter.Instance.CameraAndNeedleOffset.X + Marking.LoadOffset.X + AxisParameter.Instance.GlueOffsetX;
                                        var Y = Position.Instance.GlueCameraPosition.Y + AxisParameter.Instance.CameraAndNeedleOffset.Y + Marking.LoadOffset.Y + AxisParameter.Instance.GlueOffsetY;
                                        GlueCenterPosition.X = X;
                                        GlueCenterPosition.Y = Y;
                                        GlueStartPosition.X = GlueCenterPosition.X - Position.Instance.GlueRadius;
                                        GlueStartPosition.Y = GlueCenterPosition.Y;
                                        GlueStartPosition.Z = Position.Instance.GlueHeight;
                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;

                            #region 空胶图
                            case 260://定位完成后，再拍照   拍无胶水前的 图片 
                                //if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Y, Global.RXmanualSpeed);
                                    step = 270;

                                }
                                break;
                            case 270:
                                if (m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Y)
                                                                && m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.X))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Z, Global.RXmanualSpeed);
                                    if (m_GluePlateform.lightControl != null)
                                    {
                                        m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueFindVisionParam.LightControlvalue);
                                    }
                                    step = 280;
                                }
                                break;
                            case 280://CCD定位抓图
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Z))
                                {

                                    //稳定后再拍照
                                    Thread.Sleep(AxisParameter.Instance.CameraDelayTriger);


                                    //Thread.Sleep(Position.Instance.cameratime);
                                    Marking.GetNoGlueImg = false;
                                    //触发相机拍照
                                    if (m_GluePlateform.baslerCamera.ho_ImageNoglue != null)
                                    {
                                        if (m_GluePlateform.baslerCamera.ho_ImageNoglue.Key != IntPtr.Zero)
                                            m_GluePlateform.baslerCamera.ho_ImageNoglue.Dispose();
                                    }
                                    else
                                    {
                                        HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_ImageNoglue);
                                        m_GluePlateform.baslerCamera.ho_ImageNoglue.Dispose();
                                    }
                                    if (m_GluePlateform.baslerCamera.ho_ImageNoglue.Key == IntPtr.Zero)
                                    {
                                        Marking.LoadOffset.Result = "";
                                        m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                        m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                        log.Info($"空胶水相机触发拍照！");

                                        step = 290;
                                    }


                                }
                                break;

                            case 290://接收数据


                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_ImageNoglue.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        /// 
                                        ho_ImageT.Dispose();
                                        HOperatorSet.CopyImage(m_GluePlateform.baslerCamera.ho_ImageNoglue, out ho_ImageT);
                                        Marking.GetNoGlueImg = m_GluePlateform.glueVisionClass_GlueText.UpdataImg_Noglue(m_GluePlateform.baslerCamera.ho_ImageNoglue);

                                        Thread.Sleep(300);
                                        step = 190;

                                    }
                                    catch
                                    {
                                        Marking.GlueFindVisionResult = false;


                                    }
                                }

                                break;
                            #endregion
                            case 190:
                                MoveToPoint(m_GluePlateform.Xaxis, GlueStartPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, GlueStartPosition.Y, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, GlueStartPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                Thread.Sleep(500);
                                step = 200;
                                break;
                            case 200://起始空胶
                                if (m_GluePlateform.Xaxis.IsInPosition(GlueStartPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(GlueStartPosition.Y)
                                    && m_GluePlateform.Zaxis.IsInPosition(GlueStartPosition.Z))
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                { (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                          (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                  (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                                    Thread.Sleep(1);
                                    step = 210;
                                }
                                break;
                            case 210://点胶第一圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, 360);
                                    if (isUseGlue) IoPoints.IDO19.Value = true;
                                    else IoPoints.IDO19.Value = false;
                                    Thread.Sleep(1);
                                    step = 220;
                                }
                                break;
                            case 220://点胶第二圈
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                                    Thread.Sleep((int)Position.Instance.StopGlueDelay);
                                    step = 230;
                                }
                                break;
                            case 230://点胶拖胶
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone
                                    && m_GluePlateform.Xaxis.CurrentSpeed == 0 && m_GluePlateform.Yaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;
                                    APS168.APS_absolute_move(m_GluePlateform.Zaxis.NoId, (int)((m_GluePlateform.Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                        1000);
                                    APS168.APS_absolute_arc_move(2, new Int32[2] { m_GluePlateform.Xaxis.NoId, m_GluePlateform.Yaxis.NoId }, new Int32[2]
                                    {  (int)((GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                            (int)((GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                    (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                                    Thread.Sleep(1);
                                    step = 240;
                                }
                                break;
                            case 240://点胶结束
                                if (m_GluePlateform.Xaxis.IsDone && m_GluePlateform.Yaxis.IsDone && m_GluePlateform.Zaxis.IsDone &&
                                    m_GluePlateform.Xaxis.CurrentSpeed == 0
                                    && m_GluePlateform.Yaxis.CurrentSpeed == 0 && m_GluePlateform.Zaxis.CurrentSpeed == 0)
                                {
                                    IoPoints.IDO19.Value = false;

                                    step = 250;
                                }
                                break;
                            case 250://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                if (Config.Instance.IsTeachGlueFind)
                                    step = 300;
                                else
                                {
                                    itrue = false;
                                    step = 0;
                                }
                                break;

                            case 300:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Y, Global.RYmanualSpeed);
                                    step = 310;
                                    //itrue = false;
                                    //step = 0;
                                }
                                break;

                            case 310:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Y))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Z, Global.RZmanualSpeed);
                                    step = 320;
                                }
                                break;
                            case 320://CCD拍照检测  
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Y)
                                     && (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Z)))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 330;
                                }
                                break;
                            case 330:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.GlueOffset.Result = "";
                                    m_GluePlateform.glueVisionClass_GlueText.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 340;
                                }
                                break;
                            case 340://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        m_GluePlateform.glueVisionClass_GlueText.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        //glueVisionClass_GlueText.VisionRun(DbModelParam.Instance.GlueFindVisionParam,
                                        //    Relationship.Instance.CameraCalib);
                                        Marking.GlueOffset.Result = m_GluePlateform.glueVisionClass_GlueText.GlueVisionAction(ho_ImageT, //glueVisionClass_GlueText.ho_Image_Noglue
                                            m_GluePlateform.baslerCamera.ho_Image, Center.X, Center.Y, DbModelParam.Instance.GlueFindVisionParam) ? "OK" : "NG";

                                        Thread.Sleep(100);
                                        m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                        step = 0;
                                        itrue = false;

                                    }
                                    catch
                                    {
                                        m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                        Marking.GlueOffset.Result = "NG";
                                        Marking.GlueOffset.X = 0.0;
                                        Marking.GlueOffset.Y = 0.0;
                                        Marking.GlueOffset.A = 0.0;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;

                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// 自动拍照计算解析度
        /// </summary>
        private void AutoCamera_mm()
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var step = 0;
                    bool itrue = true;
                    double mm_X = 0.0, mm_Y = 0.0;
                    while (itrue)
                    {
                        switch (step)
                        {
                            case 0:
                                step = 10;
                                break;
                            case 10://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 20;
                                break;

                            case 20:// XY轴前往拍照位置
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed);
                                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.GlueCameraPosition.Y+3, Global.RYmanualSpeed);
                                    step = 130;
                                }
                                break;
                            case 130:// Z轴前往拍照位置
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y+3))
                                {
                                    m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed);
                                    step = 140;
                                }
                                break;
                            case 140://CCD拍照检测  定位位置
                                if ( (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z)))
                                {
                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 160;
                                }
                                break;
                            case 160:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.LoadOffset.Result = "";
                                    m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 170;
                                }
                                break;
                            case 170://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = m_GluePlateform.glueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = m_GluePlateform.glueVisionClass.VisionResult.Column_Pixel;
                                        Marking.LoadOffset.Y = m_GluePlateform.glueVisionClass.VisionResult.Row_Pixel;
                                        Marking.LoadOffset.A = m_GluePlateform.glueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 180;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        log.Info($"相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 180://读取相机处理数据
                                if (Marking.LoadOffset.Result != "")
                                {
                                    if (Marking.LoadOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 190;

                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;
                            case 190:
                                MoveToPoint(m_GluePlateform.Xaxis, Position.Instance.GlueCameraPosition.X, Global.RXmanualSpeed,
                                            m_GluePlateform.Yaxis, Position.Instance.GlueCameraPosition.Y - 3, Global.RYmanualSpeed,
                                            m_GluePlateform.Zaxis, Position.Instance.GlueCameraPosition.Z, Global.RZmanualSpeed,
                                            () =>
                                            {
                                                return !m_GluePlateform.stationInitialize.Running | !m_GluePlateform.stationOperate.Running
                                                         | m_GluePlateform.stationInitialize.InitializeDone;
                                            });
                                Thread.Sleep(500);
                                step = 200;
                                break;
                            case 200:
                                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X)
                                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y - 3)
                                    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z))
                                {

                                    //稳定后触发拍照信号 
                                    Thread.Sleep(500);
                                    step = 210;
                                }
                                break;
                            case 210:
                                //触发相机拍照
                                if (m_GluePlateform.baslerCamera.ho_Image != null)
                                {
                                    if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                        m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                else
                                {
                                    HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
                                }
                                if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
                                {
                                    Marking.GlueOffset.Result = "";
                                    m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                                    m_GluePlateform.baslerCamera.SendSoftwareExecute();
                                    log.Info($"相机触发拍照！");

                                    step = 220;
                                }
                                break;
                            case 220://接收数据

                                //判断相机是否读到图像，加载图像，运行处理
                                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
                                        m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                                Relationship.Instance.CameraCalib);
                                        Marking.GlueOffset.Result = m_GluePlateform.glueVisionClass.VisionResult.Result;
                                        Marking.GlueOffset.X = m_GluePlateform.glueVisionClass.VisionResult.Column_Pixel;
                                        Marking.GlueOffset.Y = m_GluePlateform.glueVisionClass.VisionResult.Row_Pixel;
                                        Marking.GlueOffset.A = m_GluePlateform.glueVisionClass.VisionResult.Angle;
                                        // });
                                        step = 230;
                                    }
                                    catch
                                    {
                                        Marking.GlueOffset.Result = "NG";
                                        Marking.GlueOffset.X = 0.0;
                                        Marking.GlueOffset.Y = 0.0;
                                        Marking.GlueOffset.A = 0.0;
                                        log.Info($"相机处理异常！");
                                        step = 0;
                                        itrue = false;
                                    }
                                }

                                break;
                            case 230://读取相机处理数据
                                if (Marking.GlueOffset.Result != "")
                                {
                                    if (Marking.GlueOffset.Result == "OK" || stationOperate.SingleRunning)
                                    {
                                        step = 250;

                                        log.Debug($"OK");

                                    }
                                    else
                                    {

                                        step = 0;
                                        itrue = false;
                                        log.Debug($"NG");
                                    }
                                }
                                break;

                            case 250://Z先回安全位
                                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
                                step = 260;
                                break;
                            case 260:
                                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                                {
                                    mm_X = 6.0 / Math.Sqrt((Marking.GlueOffset.X - Marking.LoadOffset.X) * (Marking.GlueOffset.X - Marking.LoadOffset.X) + (Marking.GlueOffset.Y - Marking.LoadOffset.Y) * (Marking.GlueOffset.Y - Marking.LoadOffset.Y));
                                    mm_Y = mm_X;
                                    step = 270;
                                }
                                break;
                            case 270:
                                if (DialogResult.Yes == MessageBox.Show($"当前解析度为X={Relationship.Instance.CameraCalib.ResolutionX}Y={Relationship.Instance.CameraCalib.ResolutionY},是否从新更新为X={ mm_X}Y={ mm_Y}", "", MessageBoxButtons.YesNo))
                                {
                                    Relationship.Instance.CameraCalib.ResolutionX = mm_X;
                                    Relationship.Instance.CameraCalib.ResolutionY = mm_Y;
                                    SerializerManager<Relationship>.Instance.Save(AppConfig.ConfigCameraName, Relationship.Instance);
                                    MessageBox.Show($"解析度已更改请重新点胶标定！");
                                }
                                itrue = false;
                                step = 0;
                                break;

                            default:
                                step = 0;
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private void btnCam_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show($"是否定位拍照并图像处理", "", MessageBoxButtons.YesNo)) return;
            try
            {

                var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);
                if (m_GluePlateform.lightControl != null)
                {
                    m_GluePlateform.lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                }

            }
            catch { MessageBox.Show("加载视觉模板失败"); return; }
            //触发相机拍照
            if (m_GluePlateform.baslerCamera.ho_Image != null)
            {
                if (m_GluePlateform.baslerCamera.ho_Image.Key != IntPtr.Zero)
                    m_GluePlateform.baslerCamera.ho_Image.Dispose();
            }
            else
            {
                HOperatorSet.GenEmptyObj(out m_GluePlateform.baslerCamera.ho_Image);
                m_GluePlateform.baslerCamera.ho_Image.Dispose();
            }
            if (m_GluePlateform.baslerCamera.ho_Image.Key == IntPtr.Zero)
            {
                Marking.LoadOffset.Result = "";
                m_GluePlateform.glueVisionClass.VisionResult.Result = "";
                m_GluePlateform.baslerCamera.SendSoftwareExecute();
                log.Info($"相机触发拍照！");
            }
            Thread.Sleep(300);
            m_GluePlateform.glueVisionClass.UpdataImg(m_GluePlateform.baslerCamera.ho_Image);
            m_GluePlateform.glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                    Relationship.Instance.CameraCalib);
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            Marking.CarrierStart = true;
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            Marking.CarrierStart = false;
        }

        private int MoveToPoint(ServoAxis Xaxis, double X, VelocityCurve XvelocityCurve,
            ServoAxis Yaxis, double Y, VelocityCurve YvelocityCurve,
            ServoAxis Zaxis = null, double Z = 0, VelocityCurve ZvelocityCurve = null,
            Func<bool> Condition = null)
        {
            if (!Xaxis.IsServon || !Yaxis.IsServon || !Zaxis.IsServon) return -3;
            if (!Condition()) return -4;
            Global.IsLocating = true;

            Task.Factory.StartNew(() =>
            {
                try
                {            //判断Z轴是否在零点
                    if (Zaxis != null)
                    {
                        if (!Zaxis.IsInPosition(0.5))
                            Zaxis.MoveTo(0.5, ZvelocityCurve ?? new VelocityCurve()
                            {
                                Strvel = 0,
                                Maxvel = Zaxis.Speed ?? 10,
                                Tacc = 0.1,
                                Tdec = 0.1,
                                VelocityCurveType = CurveTypes.T
                            });
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Zaxis.IsInPosition(0.5)) break;
                            if (ServoAxisIsReady(Zaxis))
                            {
                                Zaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                    }
                    //将X、Y移动到指定位置
                    if (!Xaxis.IsInPosition(X)) Xaxis.MoveTo(X, XvelocityCurve);
                    if (!Yaxis.IsInPosition(Y)) Yaxis.MoveTo(Y, YvelocityCurve);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (Xaxis.IsInPosition(X) && Yaxis.IsInPosition(Y)) break;
                        if (ServoAxisIsReady(Xaxis) || ServoAxisIsReady(Yaxis))
                        {
                            Xaxis.Stop();
                            Yaxis.Stop();
                            Global.IsLocating = false;
                            return -2;
                        }
                    }
                    //将Z轴移动到指定位置
                    if (Zaxis != null)
                    {
                        Zaxis.MoveTo(Z, ZvelocityCurve ?? new VelocityCurve()
                        {
                            Strvel = 0,
                            Maxvel = Zaxis.Speed ?? 10,
                            Tacc = 0.1,
                            Tdec = 0.1,
                            VelocityCurveType = CurveTypes.T
                        });
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Zaxis.IsInPosition(Z)) break;
                            if (ServoAxisIsReady(Zaxis))
                            {
                                Zaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                        Global.IsLocating = false;
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    log.Fatal("设备驱动程序异常", ex);
                    return -2;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return 0;
        }
        private int MoveToPoint(ServoAxis Zaxis = null, double Z = 0, VelocityCurve ZvelocityCurve = null,
    Func<bool> Condition = null)
        {
            if (!Zaxis.IsServon) return -3;
            if (!Condition()) return -4;
            Global.IsLocating = true;

            Task.Factory.StartNew(() =>
            {
                try
                {    
                    //将Z轴移动到指定位置
                    if (Zaxis != null)
                    {
                        Zaxis.MoveTo(Z, ZvelocityCurve ?? new VelocityCurve()
                        {
                            Strvel = 0,
                            Maxvel = Zaxis.Speed ?? 10,
                            Tacc = 0.1,
                            Tdec = 0.1,
                            VelocityCurveType = CurveTypes.T
                        });
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Zaxis.IsInPosition(Z)) break;
                            if (ServoAxisIsReady(Zaxis))
                            {
                                Zaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                        Global.IsLocating = false;
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    log.Fatal("设备驱动程序异常", ex);
                    return -2;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return 0;
        }
        private int MoveToPoint(ServoAxis Xaxis, double X, VelocityCurve XvelocityCurve,
            Func<bool> Condition = null, ServoAxis Yaxis = null, double Y = 0, VelocityCurve YvelocityCurve = null)
        {
            if (!Xaxis.IsServon || !Yaxis.IsServon) return -3;
            if (!Condition()) return -4;
            Global.IsLocating = true;

            Task.Factory.StartNew(() =>
            {
                try
                {            //判断Z轴是否在零点
                    if (Yaxis != null)
                    {
                        //将X、Y移动到指定位置
                        if (!Xaxis.IsInPosition(X)) Xaxis.MoveTo(X, XvelocityCurve);
                        if (!Yaxis.IsInPosition(Y)) Yaxis.MoveTo(Y, YvelocityCurve);
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Xaxis.IsInPosition(X) && Yaxis.IsInPosition(Y)) break;
                            if (ServoAxisIsReady(Xaxis) || ServoAxisIsReady(Yaxis))
                            {
                                Xaxis.Stop();
                                Yaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                    }
                    else
                    {
                        if (!Xaxis.IsInPosition(X)) Xaxis.MoveTo(X, XvelocityCurve);
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Xaxis.IsInPosition(X)) break;
                            if (ServoAxisIsReady(Xaxis))
                            {
                                Xaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                    }
                    Global.IsLocating = false;
                    return 0;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    log.Fatal("设备驱动程序异常", ex);
                    return -2;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return 0;
        }
        private int MoveToPoint(ServoAxis Xaxis, double X, VelocityCurve XvelocityCurve,
            ServoAxis IXaxis, double IX, VelocityCurve IXvelocityCurve,
            ServoAxis Yaxis, double Y, VelocityCurve YvelocityCurve,
            ServoAxis Zaxis = null, double Z = 0, VelocityCurve ZvelocityCurve = null,
            Func<bool> Condition = null)
        {
            if (!Xaxis.IsServon || !IXaxis.IsServon ||
                !Yaxis.IsServon || !Zaxis.IsServon) return -3;
            if (!Condition()) return -4;
            Global.IsLocating = true;

            Task.Factory.StartNew(() =>
            {
                try
                {   //判断Z轴是否在零点
                    if (Zaxis != null)
                    {
                        if (!Zaxis.IsInPosition(0))
                            Zaxis.MoveTo(0, ZvelocityCurve ?? new VelocityCurve()
                            {
                                Strvel = 0,
                                Maxvel = Zaxis.Speed ?? 10,
                                Tacc = 0.1,
                                Tdec = 0.1,
                                VelocityCurveType = CurveTypes.T
                            });
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Zaxis.IsInPosition(0)) break;
                            if (ServoAxisIsReady(Zaxis))
                            {
                                Zaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                    }
                    //将X、Y移动到指定位置
                    if (!Xaxis.IsInPosition(X)) Xaxis.MoveTo(X, XvelocityCurve);
                    if (!IXaxis.IsInPosition(IX)) Xaxis.MoveTo(IX, IXvelocityCurve);
                    if (!Yaxis.IsInPosition(Y)) Yaxis.MoveTo(Y, YvelocityCurve);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (Xaxis.IsInPosition(X) && IXaxis.IsInPosition(IX) && Yaxis.IsInPosition(Y)) break;
                        if (ServoAxisIsReady(Xaxis) || ServoAxisIsReady(IXaxis) || ServoAxisIsReady(Yaxis))
                        {
                            Xaxis.Stop();
                            IXaxis.Stop();
                            Yaxis.Stop();
                            Global.IsLocating = false;
                            return -2;
                        }
                    }
                    //将Z轴移动到指定位置
                    if (Zaxis != null)
                    {
                        Zaxis.MoveTo(Z, ZvelocityCurve ?? new VelocityCurve()
                        {
                            Strvel = 0,
                            Maxvel = Zaxis.Speed ?? 10,
                            Tacc = 0.1,
                            Tdec = 0.1,
                            VelocityCurveType = CurveTypes.T
                        });
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Zaxis.IsInPosition(Z)) break;
                            if (ServoAxisIsReady(Zaxis))
                            {
                                Zaxis.Stop();
                                Global.IsLocating = false;
                                return -2;
                            }
                        }
                        Global.IsLocating = false;
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    log.Fatal("设备驱动程序异常", ex);
                    return -2;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return 0;
        }
        private bool ServoAxisIsReady(ServoAxis axis) => !axis.IsServon | axis.IsAlarmed | axis.IsEmg | axis.IsMEL | axis.IsPEL;

        /// <summary>
        /// 计算镜筒清洗轨迹中心
        /// </summary>
        public void CalculateConeCenter()
        {
            try
            {
                //1次清洗轨迹圆心计算
                Position.Instance.CleanConeFirstPositionReal.X = Convert.ToSingle(Position.Instance.CleanConeFirstPosition.X);
                Position.Instance.CleanConeFirstPositionReal.Y = Convert.ToSingle(Position.Instance.CleanConeFirstPosition.Y);
                Position.Instance.CleanConeSecondPositionReal.X = Convert.ToSingle(Position.Instance.CleanConeSecondPosition.X);
                Position.Instance.CleanConeSecondPositionReal.Y = Convert.ToSingle(Position.Instance.CleanConeSecondPosition.Y);
                Position.Instance.CleanConeThirdPositonReal.X = Convert.ToSingle(Position.Instance.CleanConeThirdPositon.X);
                Position.Instance.CleanConeThirdPositonReal.Y = Convert.ToSingle(Position.Instance.CleanConeThirdPositon.Y);
                //计算镜筒清洗中心
                AreaCalculate(Position.Instance.CleanConeFirstPositionReal, Position.Instance.CleanConeSecondPositionReal,
                    Position.Instance.CleanConeThirdPositonReal, ref Position.Instance.CleanConeCenterPositionReal);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// 计算镜片清洗轨迹中心
        /// </summary>
        public void CalculateLensCenter()
        {
            try
            {
                //2次清洗轨迹圆心计算
                Position.Instance.CleanLensFirstPositionReal.X = Convert.ToSingle(Position.Instance.CleanLensFirstPosition.X);
                Position.Instance.CleanLensFirstPositionReal.Y = Convert.ToSingle(Position.Instance.CleanLensFirstPosition.Y);
                Position.Instance.CleanLensSecondPositionReal.X = Convert.ToSingle(Position.Instance.CleanLensSecondPosition.X);
                Position.Instance.CleanLensSecondPositionReal.Y = Convert.ToSingle(Position.Instance.CleanLensSecondPosition.Y);
                Position.Instance.CleanLensThirdPositonReal.X = Convert.ToSingle(Position.Instance.CleanLensThirdPositon.X);
                Position.Instance.CleanLensThirdPositonReal.Y = Convert.ToSingle(Position.Instance.CleanLensThirdPositon.Y);
                //计算镜片清洗中心
                AreaCalculate(Position.Instance.CleanLensFirstPositionReal, Position.Instance.CleanLensSecondPositionReal,
                    Position.Instance.CleanLensThirdPositonReal, ref Position.Instance.CleanLensCenterPositionReal);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// 计算点胶轨迹中心
        /// </summary>
        public void CalculateGlueCenter()
        {
            try
            {
                //点胶轨迹圆心计算
                Position.Instance.GlueFirstPositionReal.X = Convert.ToSingle(Position.Instance.GlueStartPosition.X);
                Position.Instance.GlueFirstPositionReal.Y = Convert.ToSingle(Position.Instance.GlueStartPosition.Y);
           

                //Position.Instance.GlueFirstPositionReal.X = 5;
                //Position.Instance.GlueFirstPositionReal.Y = 0;
                //Position.Instance.GlueSecondPositionReal.X = 0;
                //Position.Instance.GlueSecondPositionReal.Y =5;
                //Position.Instance.GlueThirdPositonReal.X =-5;
                //Position.Instance.GlueThirdPositonReal.Y = 0;
                //点胶中心
                AreaCalculate(Position.Instance.GlueFirstPositionReal, Position.Instance.GlueSecondPositionReal,
                    Position.Instance.GlueThirdPositonReal, ref Position.Instance.GlueCenterPositionReal);
                Position.Instance.GlueCenterPosition.X = Position.Instance.GlueCenterPositionReal.X;
                Position.Instance.GlueCenterPosition.Y = Position.Instance.GlueCenterPositionReal.Y;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// 通过三点坐标计算圆心坐标
        /// </summary>
        /// <param name="startPoint">第一点坐标</param>
        /// <param name="secondPoint">第二点坐标</param>
        /// <param name="endPoint">第三点坐标</param>
        /// <param name="centerPoint">圆心坐标</param>
        /// <returns>圆心坐标</returns>
        public static Point<float> AreaCalculate(Point<float> startPoint, Point<float> secondPoint, Point<float> endPoint, ref Point<float> centerPoint)
        {
            float tempA1, tempA2, tempB1, tempB2, tempC1, tempC2, temp, x, y;
            try
            {
                if (startPoint.X == secondPoint.X && startPoint.Y == secondPoint.Y)
                    throw new Exception("第一组数据与第二组数据相同！");
                if (startPoint.X == endPoint.X && startPoint.Y == endPoint.Y)
                    throw new Exception("第一组数据与第三组数据相同！");
                if (secondPoint.X == endPoint.X && secondPoint.Y == endPoint.Y)
                    throw new Exception("第二组数据与第三组数据相同！");
                // else throw new Exception("三个点坐标在同一直线上！");

                tempA1 = 2 * (secondPoint.X - startPoint.X);
                tempB1 = 2 * (secondPoint.Y - startPoint.Y);
                tempC1 = secondPoint.X * secondPoint.X + secondPoint.Y * secondPoint.Y - startPoint.X * startPoint.X - startPoint.Y * startPoint.Y;
                tempA2 = 2 * (endPoint.X - secondPoint.X);
                tempB2 = 2 * (endPoint.Y - secondPoint.Y);
                tempC2 = endPoint.X * endPoint.X - secondPoint.X * secondPoint.X + endPoint.Y * endPoint.Y - secondPoint.Y * secondPoint.Y;
                temp = tempA1 * tempB2 - tempA2 * tempB1;
                if (temp == 0)
                {
                    x = startPoint.X;
                    y = startPoint.Y;
                }
                else
                {
                    x = ((tempC1 * tempB2) - (tempC2 * tempB1)) / temp;
                    y = ((tempA1 * tempC2) - (tempA2 * tempC1)) / temp;
                }

                centerPoint.X = x;
                centerPoint.Y = y;

            }
            catch (Exception ex)
            {

                ;//throw new Exception(ex.ToString());
            }
            return centerPoint;

        }
    }
}
