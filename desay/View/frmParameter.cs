using System;
using System.Threading;
using System.Windows.Forms;
using desay.ProductData;
using Motion.Enginee;
using System.Toolkit;
using System.Toolkit.Helpers;
namespace desay
{
    public partial class frmParameter : Form
    {
        //private Plateform m_plateform;
        public frmParameter()
        {
            InitializeComponent();
        }
       
        private void frmParameter_Load(object sender, EventArgs e)
        {
            #region 基本参数

           
            cbUseMesLock.Checked = Position.Instance.IsUseMesLock;
            tbMesA2C.Text= Position.Instance.A2C ;
            cbUSeGlueFind.Checked = Position.Instance.IsUseGlueFind ;

            nudLensCleanAngle.Value = Position.Instance.LensCleanAngle;
            nudCloneCleanAngle.Value = Position.Instance.HoldersCleanAngle;
            nudCleanFireTime.Value = Position.Instance.CleanFireTime;
            nudCleanLensFireTime.Value = Position.Instance.CleanLensFireTime;
            nudGlueFindNGTime.Value = Position.Instance.GlueFindNGTime;
            nudGlueLoadNGTime.Value = Position.Instance.GlueLoadNGTime;
            nudAALightTime.Value = Position.Instance.AALightOnTime;

            chkCleanHolderShiled.Checked= Position.Instance.CleanHolderShield;
            chkCleanLensShiled.Checked= Position.Instance.CleanLensShield;
            #endregion

            #region 装配参数
            //chxCleanBuffer.Checked = Position.Instance.IsCleanZBuffer;
            //ndnCleanBufferSpeed.Value = (decimal)Position.Instance.CleanZBufferSpeed;
            //ndnCleanBufferDistance.Value = (decimal)Position.Instance.CleanZBufferDistance;

            //chxGlueGetBuffer.Checked = Position.Instance.IsGlueZBuffer;
            //ndnGlueBufferSpeed.Value = (decimal)Position.Instance.GlueZBufferSpeed;
            //ndnGlueBufferDistance.Value = (decimal)Position.Instance.GlueZBufferDistance;

            ndnStartGlueAngle.Value = Position.Instance.StartGlueAngle;
            ndnStartGlueDelay.Value = (decimal)Position.Instance.StartGlueDelay;
            ndnStopGlueDelay.Value = (decimal)Position.Instance.StopGlueDelay;
            ndnDragGlueAngle.Value = Position.Instance.DragGlueAngle;
            ndnDragGlueHeight.Value = (decimal)Position.Instance.DragGlueHeight;
            ndnDragGlueSpeed.Value = (decimal)Position.Instance.DragGlueSpeed;
            ndnDragGlueDelay.Value = (decimal)Position.Instance.DragGlueDelay;
            ndnSecondGlueAngle.Value = Position.Instance.SecondGlueAngle;
            ndnGlueOffsetX.Value = (decimal)AxisParameter.Instance.GlueOffsetX;
            ndnGlueOffsetY.Value = (decimal)AxisParameter.Instance.GlueOffsetY;
            ndnGlueHeight.Value = (decimal)Position.Instance.GlueHeight;
            ndnGlueRadius.Value = (decimal)Position.Instance.GlueRadius;
            ndnGlueBaseHeight.Value = (decimal)Position.Instance.GlueBaseHeight;

            ndnGlueOffsetXMax.Value = (decimal)Position.Instance.MaxNeedleOffsetX;
            ndnGlueOffsetXMin.Value = (decimal)Position.Instance.MinNeedleOffsetX;
            ndnGlueOffsetYMax.Value = (decimal)Position.Instance.MaxNeedleOffsetY;
            ndnGlueOffsetYMin.Value = (decimal)Position.Instance.MinNeedleOffsetY;
            #endregion

            #region 轴速度参数
            ndnLZspeed.Maximum = (decimal)AxisParameter.Instance.LZvelocityMax;
            ndnLXspeed.Maximum = (decimal)AxisParameter.Instance.LXvelocityMax;
            ndnLYspeed.Maximum = (decimal)AxisParameter.Instance.LYvelocityMax;

            ndnRZspeed.Maximum = (decimal)AxisParameter.Instance.RZvelocityMax;
            ndnRXspeed.Maximum = (decimal)AxisParameter.Instance.RXvelocityMax;
            ndnRYspeed.Maximum = (decimal)AxisParameter.Instance.RYvelocityMax;

            ndnLZspeed.Value = (decimal)AxisParameter.Instance.LZspeed.Maxvel;
            ndnLXspeed.Value = (decimal)AxisParameter.Instance.LXspeed.Maxvel;
            ndnLYspeed.Value = (decimal)AxisParameter.Instance.LYspeed.Maxvel;

            ndnRZspeed.Value = (decimal)AxisParameter.Instance.RZspeed.Maxvel;
            ndnRXspeed.Value = (decimal)AxisParameter.Instance.RXspeed.Maxvel;
            ndnRYspeed.Value = (decimal)AxisParameter.Instance.RYspeed.Maxvel;

            ndnCleanPathSpeed.Maximum = 80;
            ndnGluePathSpeed.Maximum = 80;
            ndnCleanPathSpeed.Value = (decimal)Position.Instance.CleanPathSpeed;
            ndnGluePathSpeed.Value = (decimal)Position.Instance.GluePathSpeed;

        
            #endregion

            nudAAOutJIgsTimeDelay.Value= AxisParameter.Instance.AAOutJigsDelay;
            nudAAPowerDelayTime.Value= AxisParameter.Instance.AAPowerOpenDelayTime ;
            nudGluePowerDelayTime.Value= AxisParameter.Instance.GluePowerOpenDelayTime ;
            nudCleanPosDelay.Value = AxisParameter.Instance.CleanPosDelay ;
            nudGluePosDelay.Value= AxisParameter.Instance.GluePosDelay ;
            nudStopilePosDelay.Value= AxisParameter.Instance.StopilePosDelay ;
            nudAAPosDelay.Value= AxisParameter.Instance.AAPosDelay ;
             tbJigsSN.Text = AxisParameter.Instance.JigsSN ;
            tbHolderSN.Text = AxisParameter.Instance.HolderSN ;
            cbAAallowPutJigs.Checked= AxisParameter.Instance.AAallowStockpilePutJigs ;
            cbAAalarm.Checked = AxisParameter.Instance.AAalrmAutoReset;
            cbUseAAflowstop.Checked= AxisParameter.Instance.AAingAAstockpStop;
            tbName.Text = AxisParameter.Instance.MachineName;
            nudPlasamaWorkTime.Value =(decimal) AxisParameter.Instance.PlasmaWorkingTime ;
            nudCamerDelay.Value= AxisParameter.Instance.CameraDelayTriger;
            cbTanZhenCly.Checked = AxisParameter.Instance.IsUseTanZhenCyl ;
            nudPlcReflashTime.Value = AxisParameter.Instance.PlcReflashTime;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否需要保存参数设置的数据", "保存", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }

            #region 基本参数
      
           
            Position.Instance.IsUseMesLock = cbUseMesLock.Checked;
            Position.Instance.A2C = tbMesA2C.Text;
            Position.Instance.IsUseGlueFind = cbUSeGlueFind.Checked;

            Position.Instance.LensCleanAngle = (int)nudLensCleanAngle.Value;
            Position.Instance.HoldersCleanAngle = (int)nudCloneCleanAngle.Value;
            Position.Instance.CleanFireTime = (int)nudCleanFireTime.Value;
            Position.Instance.CleanLensFireTime = (int)nudCleanLensFireTime.Value;
            Position.Instance.AALightOnTime= (int)nudAALightTime.Value;
            Position.Instance.GlueLoadNGTime = (int)nudGlueLoadNGTime.Value;
            Position.Instance.GlueFindNGTime = (int)nudGlueFindNGTime.Value;
            Position.Instance.CleanHolderShield = chkCleanHolderShiled.Checked;
            Position.Instance.CleanLensShield = chkCleanLensShiled.Checked;
            #endregion

            #region 装配参数
            //Position.Instance.IsCleanZBuffer = chxCleanBuffer.Checked;
            //Position.Instance.CleanZBufferSpeed = (double)ndnCleanBufferSpeed.Value;
            //Position.Instance.CleanZBufferDistance = (double)ndnCleanBufferDistance.Value;

            //Position.Instance.IsGlueZBuffer = chxGlueGetBuffer.Checked;
            //Position.Instance.GlueZBufferSpeed = (double)ndnGlueBufferSpeed.Value;
            //Position.Instance.GlueZBufferDistance = (double)ndnGlueBufferDistance.Value;

            Position.Instance.StartGlueAngle = (int)ndnStartGlueAngle.Value;
            Position.Instance.StartGlueDelay = (double)ndnStartGlueDelay.Value;
            Position.Instance.StopGlueDelay = (double)ndnStopGlueDelay.Value;
            Position.Instance.DragGlueAngle = (int)ndnDragGlueAngle.Value;
            Position.Instance.DragGlueHeight = (double)ndnDragGlueHeight.Value;
            Position.Instance.DragGlueSpeed = (double)ndnDragGlueSpeed.Value;
            Position.Instance.DragGlueDelay = (double)ndnDragGlueDelay.Value;
            Position.Instance.SecondGlueAngle = (int)ndnSecondGlueAngle.Value;
            AxisParameter.Instance.GlueOffsetX = (double)ndnGlueOffsetX.Value;
            AxisParameter.Instance.GlueOffsetY = (double)ndnGlueOffsetY.Value;
            Position.Instance.GlueHeight = (double)ndnGlueHeight.Value;
            Position.Instance.GlueRadius = (double)ndnGlueRadius.Value;
            Position.Instance.GlueBaseHeight = (double)ndnGlueBaseHeight.Value;

            Position.Instance.MaxNeedleOffsetX = (double)ndnGlueOffsetXMax.Value;
            Position.Instance.MinNeedleOffsetX = (double)ndnGlueOffsetXMin.Value;
            Position.Instance.MaxNeedleOffsetY = (double)ndnGlueOffsetYMax.Value;
            Position.Instance.MinNeedleOffsetY= (double)ndnGlueOffsetYMin.Value;
            #endregion

            #region 轴速度参数
            AxisParameter.Instance.LZspeed.Maxvel = (double)ndnLZspeed.Value;
            AxisParameter.Instance.LXspeed.Maxvel = (double)ndnLXspeed.Value;
            AxisParameter.Instance.LYspeed.Maxvel = (double)ndnLYspeed.Value;

            AxisParameter.Instance.RZspeed.Maxvel = (double)ndnRZspeed.Value;
            AxisParameter.Instance.RXspeed.Maxvel = (double)ndnRXspeed.Value;
            AxisParameter.Instance.RYspeed.Maxvel = (double)ndnRYspeed.Value;
            AxisParameter.Instance.AAOutJigsDelay = (int)nudAAOutJIgsTimeDelay.Value;
            AxisParameter.Instance.AAPowerOpenDelayTime = (int)nudAAPowerDelayTime.Value;
            AxisParameter.Instance.GluePowerOpenDelayTime = (int)nudGluePowerDelayTime.Value;
            AxisParameter.Instance.CleanPosDelay = (int)nudCleanPosDelay.Value;
            AxisParameter.Instance.GluePosDelay = (int)nudGluePosDelay.Value;
            AxisParameter.Instance.StopilePosDelay = (int)nudStopilePosDelay.Value;
            AxisParameter.Instance.AAPosDelay = (int)nudAAPosDelay.Value;
            AxisParameter.Instance.JigsSN = tbJigsSN.Text;
            AxisParameter.Instance.HolderSN = tbHolderSN.Text;
            AxisParameter.Instance.AAallowStockpilePutJigs = cbAAallowPutJigs.Checked;
            AxisParameter.Instance.AAalrmAutoReset = cbAAalarm.Checked;
            AxisParameter.Instance.AAingAAstockpStop = cbUseAAflowstop.Checked;
            AxisParameter.Instance.MachineName= tbName.Text;
            AxisParameter.Instance.PlasmaWorkingTime = (double)nudPlasamaWorkTime.Value;
            AxisParameter.Instance.CameraDelayTriger = (int)nudCamerDelay.Value;
            Position.Instance.CleanPathSpeed = (double)ndnCleanPathSpeed.Value;
            Position.Instance.GluePathSpeed = (double)ndnGluePathSpeed.Value;

            AxisParameter.Instance.IsUseTanZhenCyl = cbTanZhenCly.Checked;
            #endregion

            AxisParameter.Instance.PlcReflashTime = (int)nudPlcReflashTime.Value;
            SerializerManager<AxisParameter>.Instance.Save(AppConfig.ConfigAxisName, AxisParameter.Instance);
            SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnTransResult_Click(object sender, EventArgs e)
        {
            frmShow sh = new frmShow();
            sh.ShowDialog();
            Marking.ShowResultHaveThrans = true;
            
        }
    }
}
