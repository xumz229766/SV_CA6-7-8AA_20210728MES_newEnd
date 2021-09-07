using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using desay.Flow;
using Motion.Enginee;
using desay.ProductData;

namespace desay
{
    public partial class frmStationDebug : Form
    {
        CleanPlateform m_CleanPlateform;
        GluePlateform m_GluePlateform;
        TransBackJigs m_TransBack;
        AAstation m_AAstationPlateform;
        AAStockpile m_AAstockpilePlateform;
        ModelOperate mopCleanPlateform, mopGluePlateform, mopTransBackPlateform,mopAAstationPlateform, mopAAStockpilePlateform;

        #region 气缸
        private CylinderOperate CleanRotateOperate, CleanClampOperate, CleanUpDownOperate;

        private CylinderOperate LightUpDownOperate, LightingTestOperate;

        private CylinderOperate CleanStopOperate, CleanUpOperate;

        private void btnCarrier1_Click(object sender, EventArgs e)
        {
            if (IoPoints.IDO9.Value)
            {
                IoPoints.IDO9.Value = false;
            }
            else
            {
                IoPoints.IDO9.Value = true;
            }
        }

        private void btnCarrier2_Click(object sender, EventArgs e)
        {
            if (IoPoints.IDO8.Value)
            {
                IoPoints.IDO8.Value = false;
            }
            else
            {
                IoPoints.IDO8.Value = true;
            }
        }

        private void btnCarrier3_Click(object sender, EventArgs e)
        {
            if (IoPoints.IDO0.Value)
            {
                IoPoints.IDO0.Value = false;
            }
            else
            {
                IoPoints.IDO1.Value = false;
                IoPoints.IDO0.Value = true;
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void btnCarrier4_Click(object sender, EventArgs e)
        {
            if (IoPoints.IDO1.Value)
            {
                IoPoints.IDO1.Value = false;
            }
            else
            {
                IoPoints.IDO0.Value = false;
                IoPoints.IDO1.Value = true;
            }
        }

        private CylinderOperate GlueStopOperate, GlueUpOperate;
        private CylinderOperate MoveOperate, CarrierUpOperate, CarrierClampOperate, CarrierStopOperate;
        #endregion

        public frmStationDebug()
        {
            InitializeComponent();
        }

        public frmStationDebug(CleanPlateform leftPlateform, GluePlateform rightPlateform, TransBackJigs assembly,AAstation AAstationPlateform,AAStockpile aastockpile) : this()
        {
            m_CleanPlateform = leftPlateform;
            m_GluePlateform = rightPlateform;
            m_TransBack = assembly;
            m_AAstationPlateform = AAstationPlateform;
            m_AAstockpilePlateform = aastockpile;

            mopCleanPlateform = new ModelOperate();
            mopCleanPlateform.Dock = DockStyle.Fill;
            mopGluePlateform = new ModelOperate();
            mopGluePlateform.Dock = DockStyle.Fill;
            mopTransBackPlateform = new ModelOperate();
            mopTransBackPlateform.Dock = DockStyle.Fill;
            mopAAstationPlateform = new ModelOperate();
            mopAAstationPlateform.Dock = DockStyle.Fill;
            mopAAStockpilePlateform = new ModelOperate();
            mopAAStockpilePlateform.Dock = DockStyle.Fill;
        }

        private void frmStationDebug_Load(object sender, EventArgs e)
        {
            mopCleanPlateform.StationIni = m_CleanPlateform.stationInitialize;
            mopCleanPlateform.StationOpe = m_CleanPlateform.stationOperate;

            mopGluePlateform.StationIni = m_GluePlateform.stationInitialize;
            mopGluePlateform.StationOpe = m_GluePlateform.stationOperate;

            mopTransBackPlateform.StationIni = m_TransBack.stationInitialize;
            mopTransBackPlateform.StationOpe = m_TransBack.stationOperate;

            mopAAstationPlateform.StationIni = m_AAstationPlateform.stationInitialize;
            mopAAstationPlateform.StationOpe = m_AAstationPlateform.stationOperate;

            mopAAStockpilePlateform.StationIni = m_AAstockpilePlateform.stationInitialize;
            mopAAStockpilePlateform.StationOpe = m_AAstockpilePlateform.stationOperate;

            gbxLeftplateform.Controls.Add(mopCleanPlateform);
            gbxRightplateform.Controls.Add(mopGluePlateform);
            gbxAssembly.Controls.Add(mopTransBackPlateform);
            gbxAAstation.Controls.Add(mopAAstationPlateform);
            gbxAAstockpile.Controls.Add(mopAAStockpilePlateform);

            //清洗气缸
            CleanStopOperate = new CylinderOperate(() => { m_CleanPlateform.CleanStopCylinder.ManualExecute(); })
            {
                CylinderName = "清洗阻挡气缸",
                NoOriginShield = m_CleanPlateform.CleanStopCylinder.Condition.NoOriginShield,
                NoMoveShield = m_CleanPlateform.CleanStopCylinder.Condition.NoMoveShield
            };
            CleanUpOperate = new CylinderOperate(() => { m_CleanPlateform.CleanUpCylinder.ManualExecute(); })
            {
                CylinderName = "清洗顶升气缸",
                NoOriginShield = m_CleanPlateform.CleanUpCylinder.Condition.NoOriginShield,
                NoMoveShield = m_CleanPlateform.CleanUpCylinder.Condition.NoMoveShield
            };
            CleanClampOperate = new CylinderOperate(() => { m_CleanPlateform.CleanClampCylinder.ManualExecute(); })
            {
                CylinderName = "清洗夹紧气缸",
                NoOriginShield = m_CleanPlateform.CleanClampCylinder.Condition.NoOriginShield,
                NoMoveShield = m_CleanPlateform.CleanClampCylinder.Condition.NoMoveShield
            };
            CleanRotateOperate = new CylinderOperate(() => { m_CleanPlateform.CleanRotateCylinder.ManualExecute(); })
            {
                CylinderName = "清洗旋转气缸",
                NoOriginShield = m_CleanPlateform.CleanRotateCylinder.Condition.NoOriginShield,
                NoMoveShield = m_CleanPlateform.CleanRotateCylinder.Condition.NoMoveShield
            };
            CleanUpDownOperate = new CylinderOperate(() => { m_CleanPlateform.CleanUpDownCylinder.ManualExecute(); })
            {
                CylinderName = "清洗上下气缸",
                NoOriginShield = m_CleanPlateform.CleanUpDownCylinder.Condition.NoOriginShield,
                NoMoveShield = m_CleanPlateform.CleanUpDownCylinder.Condition.NoMoveShield
            };
            //LightUpDownOperate = new CylinderOperate(() => { m_CleanPlateform.LightUpDownCylinder.ManualExecute(); })
            //{
            //    CylinderName = "光源上下气缸",
            //    NoOriginShield = m_CleanPlateform.LightUpDownCylinder.Condition.NoOriginShield,
            //    NoMoveShield = m_CleanPlateform.LightUpDownCylinder.Condition.NoMoveShield
            //};
            //点胶气缸
            GlueStopOperate = new CylinderOperate(() => { m_GluePlateform.GlueStopCylinder.ManualExecute(); })
            {
                CylinderName = "点胶阻挡气缸",
                NoOriginShield = m_GluePlateform.GlueStopCylinder.Condition.NoOriginShield,
                NoMoveShield = m_GluePlateform.GlueStopCylinder.Condition.NoMoveShield
            };
            GlueUpOperate = new CylinderOperate(() => { m_GluePlateform.GlueUpCylinder.ManualExecute(); })
            {
                CylinderName = "点胶顶升气缸",
                NoOriginShield = m_GluePlateform.GlueUpCylinder.Condition.NoOriginShield,
                NoMoveShield = m_GluePlateform.GlueUpCylinder.Condition.NoMoveShield
            };
            //载具气缸
            MoveOperate = new CylinderOperate(() => { m_TransBack.CleanGlueBackStopCylinder.ManualExecute(); })
            {
                CylinderName = "载具移动气缸",
                NoOriginShield = m_TransBack.CleanGlueBackStopCylinder.Condition.NoOriginShield,
                NoMoveShield = m_TransBack.CleanGlueBackStopCylinder.Condition.NoMoveShield
            };
      
            flpCylinder.Controls.Add(CleanStopOperate);
            flpCylinder.Controls.Add(CleanUpOperate);
            flpCylinder.Controls.Add(CleanClampOperate);
            flpCylinder.Controls.Add(CleanRotateOperate);
            flpCylinder.Controls.Add(CleanUpDownOperate);
            flpCylinder.Controls.Add(LightUpDownOperate);
            //flpCylinder.Controls.Add(LightingTestOperate);
            flpCylinder.Controls.Add(GlueStopOperate);
            flpCylinder.Controls.Add(GlueUpOperate);
            flpCylinder.Controls.Add(MoveOperate);
            flpCylinder.Controls.Add(CarrierUpOperate);
            flpCylinder.Controls.Add(CarrierClampOperate);
            flpCylinder.Controls.Add(CarrierStopOperate);

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            mopTransBackPlateform.Refreshing();
            mopCleanPlateform.Refreshing();
            mopGluePlateform.Refreshing();
            mopAAstationPlateform.Refreshing();
            mopAAStockpilePlateform.Refreshing();
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
            //LightUpDownOperate.InOrigin = m_CleanPlateform.LightUpDownCylinder.OutOriginStatus;
            //LightUpDownOperate.InMove = m_CleanPlateform.LightUpDownCylinder.OutMoveStatus;
            //LightUpDownOperate.OutMove = m_CleanPlateform.LightUpDownCylinder.IsOutMove;
            //LightingTestOperate.InOrigin = m_LeftPlateform.LightingTestCylinder.OutOriginStatus;
            //LightingTestOperate.InMove = m_LeftPlateform.LightingTestCylinder.OutMoveStatus;
            //LightingTestOperate.OutMove = m_LeftPlateform.LightingTestCylinder.IsOutMove;

            //点胶平台
            GlueStopOperate.InOrigin = m_GluePlateform.GlueStopCylinder.OutOriginStatus;
            GlueStopOperate.InMove = m_GluePlateform.GlueStopCylinder.OutMoveStatus;
            GlueStopOperate.OutMove = m_GluePlateform.GlueStopCylinder.IsOutMove;
            GlueUpOperate.InOrigin = m_GluePlateform.GlueUpCylinder.OutOriginStatus;
            GlueUpOperate.InMove = m_GluePlateform.GlueUpCylinder.OutMoveStatus;
            GlueUpOperate.OutMove = m_GluePlateform.GlueUpCylinder.IsOutMove;

            //载具平台
            MoveOperate.InOrigin = m_TransBack.CleanGlueBackStopCylinder.OutOriginStatus;
            MoveOperate.InMove = m_TransBack.CleanGlueBackStopCylinder.OutMoveStatus;
            MoveOperate.OutMove = m_TransBack.CleanGlueBackStopCylinder.IsOutMove;
            //CarrierUpOperate.InOrigin = m_TransBack.CarrierUpCylinder.OutOriginStatus;
            //CarrierUpOperate.InMove = m_TransBack.CarrierUpCylinder.OutMoveStatus;
            //CarrierUpOperate.OutMove = m_TransBack.CarrierUpCylinder.IsOutMove;
            //CarrierClampOperate.InOrigin = m_TransBack.CarrierClampCylinder.OutOriginStatus;
            //CarrierClampOperate.InMove = m_TransBack.CarrierClampCylinder.OutMoveStatus;
            //CarrierClampOperate.OutMove = m_TransBack.CarrierClampCylinder.IsOutMove;
            //CarrierStopOperate.InOrigin = m_TransBack.CarrierStopCylinder.OutOriginStatus;
            //CarrierStopOperate.InMove = m_TransBack.CarrierStopCylinder.OutMoveStatus;
            //CarrierStopOperate.OutMove = m_TransBack.CarrierStopCylinder.IsOutMove;

            #endregion

            #region 图像显示
            //输送装置信号显示
            txtCarrierCallOut.BackColor = Marking.CarrierCallOut ? Color.LimeGreen : SystemColors.Control;
            txtCarrierCallOutFinish.BackColor = Marking.CarrierCallOutFinish ? Color.LimeGreen : SystemColors.Control;
            txtCarrierCallIn.BackColor = Marking.CarrierCallIn ? Color.LimeGreen : SystemColors.Control; ;
            txtCarrierCallInFinish.BackColor = Marking.CarrierCallInFinish ? Color.LimeGreen : SystemColors.Control;
            //清洗工位信号显示
            //txtCleanCallIn.BackColor = Marking.CleanCallIn ? Color.LimeGreen : SystemColors.Control;
            txtCleanCallInFinish.BackColor = Marking.CleanHaveProduct ? Color.LimeGreen : SystemColors.Control;
            //txtCleanWorking.BackColor = Marking.CleanWorking ? Color.LimeGreen : SystemColors.Control;
            //txtCleanHoming.BackColor = Marking.CleanHoming ? Color.LimeGreen : SystemColors.Control;
            //txtCleanOver.BackColor = Marking.CleanWorkFinish ? Color.LimeGreen : SystemColors.Control;
            //txtCleanCallOut.BackColor = Marking.CleanCallOut ? Color.LimeGreen : SystemColors.Control;
            //txtCleanCallOutFinish.BackColor = Marking.CleanCallOutFinish ? Color.LimeGreen : SystemColors.Control;
            //点胶工位信号显示
            txtGlueCallIn.BackColor = Marking.GlueCallIn ? Color.LimeGreen : SystemColors.Control;
            txtGlueCallInFinish.BackColor = Marking.GlueHaveProduct ? Color.LimeGreen : SystemColors.Control;
            txtGlueWorking.BackColor = Marking.GlueWorking ? Color.LimeGreen : SystemColors.Control;
            txtGlueHoming.BackColor = Marking.GlueHoming ? Color.LimeGreen : SystemColors.Control;
            txtGlueOver.BackColor = Marking.GlueWorkFinish ? Color.LimeGreen : SystemColors.Control;
            txtGlueCallOut.BackColor = Marking.GlueCallOut ? Color.LimeGreen : SystemColors.Control;
            txtGlueCallOutFinish.BackColor = Marking.GlueCallOutFinish ? Color.LimeGreen : SystemColors.Control;

            txtCleanResult.BackColor = Marking.CleanResult_MesLock ? Color.LimeGreen : Color.Red;
            txtGlueResult.BackColor = Marking.GlueResult ? Color.LimeGreen : Color.Red;

            txtFN.Text = Marking.FN;
            txtSN.Text = Marking.SN;
            #endregion

            timer1.Enabled = true;
        }
    }
}
