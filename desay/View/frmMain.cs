using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using log4net;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

using Motion.Enginee;
using Motion.Interfaces;
using desay.ProductData;
using desay.Flow;
using System.Toolkit;
using System.Toolkit.Helpers;
using System.Text;
using Motion.AdlinkAps;
using PylonLiveViewer;
using desay.Vision;
using HalconDotNet;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
namespace desay
{
    public partial class frmMain : Form
    {
        #region field  
        //public static frmMain main;
        private AlarmType CleanStaionIsAlarm, ClueStationsAlarm, TransBackJigsIsAlarm, AAstaionIsAlarm, AAstockpileIsAlarm, MachineIsAlarm, MesIsAlarm;
        private External m_External = new External();
        private MachineOperate MachineOperation;
        private GluePlateform m_GluePlateform;
        private CleanPlateform m_CleanPlateform;
        private TransBackJigs m_TransBackJigs;
        private AAstation m_AAstation;
        private AAStockpile m_AAstockpile;
        public MES m_Mes;



        private EventButton StartButton1, StartButton2, EstopButton, EstopButton2, StopButton, PauseButton, ResetButton;
        private LayerLight layerLight;
        private bool ManualAutoMode;
        Thread threadMachineRun = null;
        Thread threadAlarmCheck = null;
        Thread threadStatusCheck = null;
        Thread threadLicenseCheck = null;

        static ILog log = LogManager.GetLogger(typeof(frmMain));

        public bool uploadEnable = false;
        event Action<string> LoadingMessage;
        event Action<UserLevel> UserLevelChangeEvent;
        event Action StopEvent;

        AsynTcpServer scannerServer;
        AsynTcpServer aaServer;
        System.ToolKit.AsynTcpClient2 FormerStationClient;

        int faultcount;
        Global.Fault fault = new Global.Fault();

        public bool AutoNeedleStatus;//自动对针状态记忆
        public bool AutoNeedleStatusRun;//自动对针状态忆
        public int NeedleStep;

        public LightControl m_lightControl;
        IntPtr DlgHandle_AA;
        IntPtr DlgHandle_wb;
        #endregion

        #region Constructor

        public frmMain()
        {
            InitializeComponent();
 

            this.WindowState = FormWindowState.Maximized;

            //LogInfor = new frmLog()
        }
        private void ShowWindows(object sender, System.EventArgs e)
        {
            //if (!haspRegistered.Created)
            //{
            //    haspRegistered.ShowDialog();
            //}
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}

        #endregion

        #region 用户权限

        void UserLevelChange(UserLevel level)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<UserLevel>(UserLevelChange), level);
            }
            else
            {
                switch (level)
                {
                    case UserLevel.操作员:
                        btnTeach.Enabled = false;
                        btnSetting.Enabled = false;
                        btnVision.Enabled = false;
                        btnPassword.Enabled = false;
                        btnTest.Enabled = false;
                        
                        break;
                    case UserLevel.工程师:
                        btnTeach.Enabled = true;
                        btnSetting.Enabled = true;
                        btnVision.Enabled = true;
                        btnPassword.Enabled = false;
                        btnTest.Enabled = false;
                        break;
                    case UserLevel.管理员:
                        btnTeach.Enabled = true;
                        btnSetting.Enabled = true;
                        btnVision.Enabled = true;
                        btnPassword.Enabled = true;
                        btnTest.Enabled = true;
                        break;
                    default:
                        btnTeach.Enabled = false;
                        btnSetting.Enabled = false;
                        btnVision.Enabled = false;
                        btnPassword.Enabled = false;
                        btnTest.Enabled = false;
                        break;
                }
            }
        }
        public void OnUserLevelChange(UserLevel level)
        {
            UserLevelChangeEvent?.Invoke(level);
        }

        #endregion

        #region 停止事件
        void StopStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StopStatus));
            }
            else
            {
                ManualAutoMode = false;//cdm加1010
                btnManualAuto.Text = "手动模式";
                btnManualAuto.ForeColor = Color.Red;
            }
        }
        public void OnStop()
        {
            StopEvent?.Invoke();
        }
        #endregion
        CallAAControl callaa = new CallAAControl();
        bool HaspDoorShield = true;
        #region 窗体加载
        public List<vvdatax> ffGetaaData(string msg)
        {
            List<vvdatax> dd = new List<vvdatax>();

            string[] msgx = msg.Split('|');
            for (int i = 0; i < 7; i++)
            {
                vvdatax d1 = new vvdatax();
                d1.vvdata = double.Parse(msgx[4 + i * 7]);
                d1.vvmin = double.Parse(msgx[5 + i * 7]);
                d1.vvmax = double.Parse(msgx[6 + i * 7]);
                dd.Add(d1);

            }

            return dd;
        }
        public void TestMes()
        {
            try
            {
                Common.ReadCommonIniFile();
                StringBuilder buf = new StringBuilder(3072);

                //string AAdata = buf.ToString();
                string AAdata = "$DNN00123$JIG_01|MTF0|Passed|MTF|0.81|0.55|1.00||MTF1|Passed|MTF|0.68|0.40|1.00||MTF2|Passed|MTF|0.69|0.40|1.00||MTF3|Passed|MTF|0.71|0.40|1.00||MTF4|Passed|MTF|0.69|0.40|1.00||CTRX|Passed|mm|0.002|-0.013|0.013||CTRY|Passed|mm|0.004|-0.013|0.013||";
                string[] str1 = AAdata.Split('$');
                //产品码校验
                string HolderSn = str1[1];
                string tool3 = str1[2].Substring(0, 3);
                if (HolderSn != MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN)
                { log.Error("Mes save Error2"); };
                if (AAdata.Length < 40)
                {

                    vvdatax d1 = new vvdatax();
                    d1.vvdata = -2;
                    d1.vvmin = 0;
                    d1.vvmax = 2;

                    Common.Test_WriteMesTxtAndCsvFile(HolderSn, tool3, false, d1, d1, d1, d1, d1, d1, d1);

                }
                else
                {
                    //mes
                    List<vvdatax> dd1 = ffGetaaData(AAdata);

                    if (dd1.Count == 7)
                        Common.Test_WriteMesTxtAndCsvFile(HolderSn, tool3, true, dd1[0], dd1[1], dd1[2], dd1[3], dd1[4], dd1[5], dd1[6]);
                    else
                    {
                        vvdatax d1 = new vvdatax();
                        d1.vvdata = -1;
                        d1.vvmin = 0;
                        d1.vvmax = 1;
                        Common.Test_WriteMesTxtAndCsvFile(HolderSn, tool3, false, d1, d1, d1, d1, d1, d1, d1);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Mes save Error2");

            }
        }
        /// <summary>
        /// 计算两个日期的时间间隔
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        private string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days.ToString() + "天"
                + ts.Hours.ToString() + "小时"
                + ts.Minutes.ToString() + "分钟"
                + ts.Seconds.ToString() + "秒";

            return dateDiff;
        }
        private double DateDiff(DateTime DateTime1, DateTime DateTime2,bool time)
        {
            double dateDiff = 0;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days*24
                + ts.Hours+ts.Minutes*1.0/60.0 ;

            return dateDiff;
        }
        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Position.Instance.MesMode)
                cbMesmode.Text = "MES模式";
            else cbMesmode.Text = "SV模式";

         
            //SV mes
            Common.ReadCommonIniFile();
            //Common.MesMoveIn("1234567890","1234567");
            //Common.MesMoveOut("1234567890", "1234567",true);

            if (Config.Instance.IsIO91) this.Text = "AA设备控制系统（永久版2.0_20210728）";
            else this.Text = "AA设备控制系统（永久版4.0_20210728）";

            //DateTime aa=DateTime.Now;
            //Thread.Sleep(3000);
            //string bb=DateDiff(aa, DateTime.Now);

             UserLevelChangeEvent += UserLevelChange;
            StopEvent += new Action(StopStatus);
            Config.Instance.userLevel = UserLevel.操作员;


            ////调试方便暂时开放权限以及设置屏蔽
            //Config.Instance.userLevel = UserLevel.工程师;
            //Marking.AAShield = true;
            Marking.CurtainShield = true;

            OnUserLevelChange(Config.Instance.userLevel);
            //new Thread(new ThreadStart(() =>
            //{
            //    frmStarting loading = new frmStarting(8);
            //    //利用消息控制loading进度条
            //    LoadingMessage += new Action<string>(loading.ShowMessage);
            //    loading.ShowDialog();
            //})).Start();
            //Thread.Sleep(1000);
            //LoadingMessage("加载板卡信息");
            #region  加载板卡


            try
            {
                //西威Ca6 2张208  1 7442 64进出
                IoPoints.m_ApsController.Initialize();
                if (IoPoints.m_ApsController.LoadParamFromFile(AppConfig.ConfigIniCard0file)) AppendText("加载轴卡0参数成功");
                else { AppendText("加载轴卡0参数失败！"); }
                if (IoPoints.m_ApsController.LoadParamFromFile1(AppConfig.ConfigIniCard1file)) AppendText("加载轴卡1参数成功");
                else { AppendText("加载轴卡1参数失败！"); }

                if (IoPoints.m_ApsController._isInitialized) picMotionCard.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
                else picMotionCard.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;

                IoPoints.m_DaskController.Initialize();
                if (IoPoints.m_DaskController._InitSucess) picIO.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
                else picIO.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;

            }
            catch (Exception ex) { log.Error($"{ex}"); }
            #endregion
            //LoadingMessage("加载轴控制资源");
            #region 伺服轴信息加载

            //西威与其他不同

            var Lxaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 12,
                Name = "清洗X轴",
                Transmission = AxisParameter.Instance.LXTransParams
            };
            var Lyaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 13,
                Name = "清洗Y轴",
                Transmission = AxisParameter.Instance.LYTransParams
            };
            var Lzaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 14,
                Name = "清洗Z轴",
                Transmission = AxisParameter.Instance.LZTransParams
            };

            var Rxaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 8,
                Name = "点胶X轴",
                Transmission = AxisParameter.Instance.RXTransParams
            };
            var Ryaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 9,
                Name = "点胶Y轴",
                Transmission = AxisParameter.Instance.RYTransParams
            };
            var Rzaxis = new ServoAxis(IoPoints.m_ApsController)
            {
                NoId = 10,
                Name = "点胶Z轴",
                Transmission = AxisParameter.Instance.RZTransParams
            };
            //?
            //var AAyaxis = new ServoAxis(IoPoints.m_ApsController)
            //{
            //    NoId = 7,
            //    Name = "AA平台进出轴Y轴",
            //    Transmission = AxisParameter.Instance.AAYTransParams
            //};
            //var AAzaxis = new ServoAxis(IoPoints.m_ApsController)
            //{
            //    NoId = 3,
            //    Name = "AA光源Z轴",
            //    Transmission = AxisParameter.Instance.AAZTransParams
            //};
            #endregion
            #region 设置轴回零参数
            try
            {
                //西威清洗和点胶共6个轴都为电杠 后面再加 
                //Lxaxis.SetAxisHomeParam(new HomeParams(1, 1, 0, 10000, 5000, 1000));
                //Lyaxis.SetAxisHomeParam(new HomeParams(0, 1, 0, 10000, 5000, 0));
                //Lzaxis.SetAxisHomeParam(new HomeParams(1, 1, 0, 10000, 5000, 0));
                //Rxaxis.SetAxisHomeParam(new HomeParams(1, 1, 0, 10000, 5000, 1000));
                //Ryaxis.SetAxisHomeParam(new HomeParams(0, 1, 0, 10000, 5000, 0));
                //Rzaxis.SetAxisHomeParam(new HomeParams(0, 0, 0, 10000, 5000, 0));
                //AAyaxis.SetAxisHomeParam(new HomeParams(0, 1, 0, 
                //    (int)(AxisParameter.Instance.AAYhomeSpeed.Maxvel* AxisParameter.Instance.AAYTransParams.EquivalentPulse), 
                //    (int)(AxisParameter.Instance.AAYhomeSpeed.Strvel * AxisParameter.Instance.AAYTransParams.EquivalentPulse), 0));
                //AAzaxis.SetAxisHomeParam(new HomeParams(0, 1, 0,
                //    (int)(AxisParameter.Instance.AAZhomeSpeed.Maxvel * AxisParameter.Instance.AAZTransParams.EquivalentPulse),
                //    (int)(AxisParameter.Instance.AAZhomeSpeed.Strvel * AxisParameter.Instance.AAZTransParams.EquivalentPulse), 0));
                //AA段 两个轴
            }
            catch (Exception ex) { log.Error($"{ex}"); }
            #endregion
            #region 通讯初始化操作
            OpenPower();//新增20201112
            Thread.Sleep(500);
            OpenPower();//新增20201112
            OpenTcpLightControl();   //创建TCP 服务器，连接 AA，CCD,白板
            OpenTcpClient();
            //OpenDetectHeight(); //测高模块
            #endregion
            //LoadingMessage("加载模组操作资源");
            #region 工站模组操作

            //Loading module operating resources
            var MesInitialize = new StationInitialize(
                () => { return !ManualAutoMode & ( HaspDoorShield); },
                () => { return MesIsAlarm.IsAlarm; });
            var MesOperate = new StationOperate(
                () => { return MesInitialize.InitializeDone & ( HaspDoorShield); },
                () => { return MesIsAlarm.IsAlarm; });
            var TransBackJigsInitialize = new StationInitialize(
                () => { return !ManualAutoMode & ( HaspDoorShield); },
                () => { return TransBackJigsIsAlarm.IsAlarm; });
            var TransBackJigsOperate = new StationOperate(
                () => { return TransBackJigsInitialize.InitializeDone & ( HaspDoorShield); },
                () => { return TransBackJigsIsAlarm.IsAlarm; });
            var AAstationInitialize = new StationInitialize(
               () => { return !ManualAutoMode & ( HaspDoorShield); },
               () => { return AAstaionIsAlarm.IsAlarm; });
            var AAstationOperate = new StationOperate(
                () => { return AAstationInitialize.InitializeDone & ( HaspDoorShield); },
                () => { return AAstaionIsAlarm.IsAlarm; });
            var AAstockpileInitialize = new StationInitialize(
              () => { return !ManualAutoMode & ( HaspDoorShield); },
              () => { return AAstockpileIsAlarm.IsAlarm; });
            var AAstockpileOperate = new StationOperate(
                () => { return AAstockpileInitialize.InitializeDone & ( HaspDoorShield); },
                () => { return AAstockpileIsAlarm.IsAlarm; });
            var CleanStationInitialize = new StationInitialize(
                () => { return !ManualAutoMode & ( HaspDoorShield); },
                () => { return CleanStaionIsAlarm.IsAlarm; });
            var CleanStationOperate = new StationOperate(
                () => { return CleanStationInitialize.InitializeDone & ( HaspDoorShield); },
                () => { return CleanStaionIsAlarm.IsAlarm; });
            var GlueStionInitialize = new StationInitialize(
                () => { return !ManualAutoMode & ( HaspDoorShield); },
                () => { return ClueStationsAlarm.IsAlarm; });
            var GlueStationOperate = new StationOperate(
                () => { return GlueStionInitialize.InitializeDone & (HaspDoorShield); },
                () => { return ClueStationsAlarm.IsAlarm; });

            #endregion
            //LoadingMessage("模组信息加载、启动");

            #region 气缸信息 
            #region  清洗   
            var cleanStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO10)
            {
                Name = "清洗阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoMoveShield = true,
                    NoOriginShield = true
                },
                Delay = Delay.Instance.cleanStopCylinderDelay
            };
            //保压
            var cleanUpCylinder = new DoubleCylinder(IoPoints.IDI2, IoPoints.IDI1, IoPoints.IDO1, IoPoints.IDO0)
            {
                Name = "清洗顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.cleanUpCylinderDelay
            };
            var cleanClampCylinder = new DoubleCylinder(IoPoints.IDI4, IoPoints.IDI5, IoPoints.IDO3, IoPoints.IDO2)
            {
                Name = "清洗夹紧气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.cleanClampCylinderDelay
            };
            //?
            var cleanUpDownCylinder = new DoubleCylinder(IoPoints.IDI8, IoPoints.IDI9, IoPoints.IDO7, IoPoints.IDO6)
            {
                Name = "清洗上下气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.cleanUpDownCylinderDelay
            };
            var cleanRotateCylinder = new DoubleCylinder(IoPoints.IDI6, IoPoints.IDI7, IoPoints.IDO5, IoPoints.IDO4)
            {
                Name = "清洗旋转气缸",//有条件
                Condition = new CylinderCondition(() => { return true; }, () => { return cleanUpDownCylinder.OutOriginStatus && cleanClampCylinder.OutMoveStatus; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.cleanRotateCylinderDelay
            };



            #endregion
            #region 点胶
            var glueStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO12)
            {
                Name = "点胶阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoOriginShield = true,
                    NoMoveShield = true
                },
                Delay = Delay.Instance.glueStopCylinderDelay
            };
            var glueUpCylinder = new DoubleCylinder(IoPoints.IDI17, IoPoints.IDI16, IoPoints.IDO18, IoPoints.IDO17)
            {
                Name = "点胶顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.glueUpCylinderDelay
            };
            var glueUpCylinder_Small = new DoubleCylinder(IoPoints.TDI1, IoPoints.TDI2, IoPoints.TDO6,  IoPoints.TDO7)
            {
                Name = "点胶探针顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.glueUpCylinderDelay_small
            };
            var cleanAndGlueStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO21)
            {
                Name = "清洗点胶回流线阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoOriginShield = true,
                    NoMoveShield = true
                },
                Delay = Delay.Instance.cleanAndGlueStopCylinderDelay
            };
            #endregion
            #region AA
            var AAJigsStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO91)
            {
                Name = "AA夹具阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoOriginShield = true,
                    NoMoveShield = true
                },
                Delay = Delay.Instance.AAJigsStopCylinderDelay
            };
            var AAJigsUpCylinder = new DoubleCylinder(IoPoints.IDI93, IoPoints.IDI92, IoPoints.IDO93, IoPoints.IDO92)
            {
                Name = "AA夹具顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.AAJigsUpCylinderDelay
            };
            var AAJigsUpCylinder_Small = new DoubleCylinder(IoPoints.IDI916, IoPoints.IDI917, IoPoints.IDO922,  IoPoints.IDO923)
            {
                Name = "AA夹具探针顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.AAJigsUpCylinderDelay_Small
            };
            var AAStockpileUpCylinder = new DoubleCylinder(IoPoints.IDI95, IoPoints.IDI94, IoPoints.IDO95, IoPoints.IDO94)
            {
                Name = "AA堆料顶升气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    //2020.9.12被拆了临时屏蔽
                    //NoOriginShield = true,
                    //NoMoveShield = true
                },
                Delay = Delay.Instance.AAStockpileUpCylinderDelay
            };
            var AAStockpileStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO98)
            {
                Name = "AA堆料阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoOriginShield = true,
                    NoMoveShield = true
                },
                Delay = Delay.Instance.AAStockpileStopCylinderDelay
            };
            var AABackFlowStopCylinder = new SingleCylinder(IoPoints.IDI3, IoPoints.IDI3, IoPoints.IDO99)
            {
                Name = "AA回流阻挡气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                    NoOriginShield = true,
                    NoMoveShield = true
                },
                Delay = Delay.Instance.AABackFlowStopCylinderDelay
            };
            //?
            var AAUVUpDownCylinder = new DoubleCylinder(IoPoints.IDI97, IoPoints.IDI98, IoPoints.IDO919, IoPoints.IDO916)
            {
                Name = "UV灯上下气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.AAUVUpDownCylinderDelay
            };
            //?
            var AACalampClawCylinder = new DoubleCylinder(IoPoints.IDI99, IoPoints.IDI910, IoPoints.IDO918, IoPoints.IDO920)
            {
                Name = "AA气夹爪气缸",
                Condition = new CylinderCondition(() => { return true; }, () => { return true; })
                {
                    External = m_External,
                },
                Delay = Delay.Instance.AAClampClawCylinderDelay
            };
            #endregion
            #endregion
            #region 相机
            var GlueVision = new VisionClass(hWindowControl1);
            var GlueVision_Text = new VisionClass(hWindowControl1);
            var CalibGlueVision = new VisionClass(hWindowControl1);
            var BaslerCamera = new BaslerCam();
            if (BaslerCamera.OpenCam())
            {
                AppendText("相机初始化成功！");
                BaslerCamera.SetHeartBeatTime(10000);//10S
                BaslerCamera.SetExposureTime(VisionProductData.Instance.nExposure);
                BaslerCamera.SetGain(VisionProductData.Instance.nGain);
                BaslerCamera.eventComputeGrabTime += computeGrabTime2;    // 注册计算采集图像时间回调函数
                BaslerCamera.eventProcessImage += processHImage1;         // 注册halcon显示回调函数
                BaslerCamera.SetSoftwareTrigger();//第一步设置软触发模式
                BaslerCamera.StartGrabbing();//第二部采集开始
                BaslerCamera.SendSoftwareExecute();//第三部触发
                VisionMarking.IsCameraOpen = true;
                VisionMarking.ModelName = BaslerCamera.strModelName;
                VisionMarking.SN = BaslerCamera.strSerialNumber;
                VisionMarking.UserID = BaslerCamera.strUserID;
                VisionMarking.MinExposure = (int)BaslerCamera.minExposureTime;
                VisionMarking.MaxExposure = (int)BaslerCamera.maxExposureTime;
                VisionMarking.MinGain = (int)BaslerCamera.minGain;
                VisionMarking.MaxGain = (int)BaslerCamera.maxGain;
                Thread.Sleep(100);

                GlueVision.UpdataImg(BaslerCamera.ho_Image);
                picCamera.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
            }
            else picCamera.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
            #endregion
            #region 模组信息加载、启动

            //loading module information
            m_Mes = new MES(m_External, MesInitialize, MesOperate)
            {
                Name = "通讯模块",

                aaServer = aaServer,
                //HeightDectector = heightDetector
            };
            m_Mes.AddPart();
            m_Mes.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_Mes.Run(RunningModes.Online);

            m_TransBackJigs = new TransBackJigs(m_External, TransBackJigsInitialize, TransBackJigsOperate)
            {
                Name = "流水线回流平台",
                CleanGlueBackStopCylinder = cleanAndGlueStopCylinder,
                AAbackHomeStopCylinder = AABackFlowStopCylinder,

            };
            m_TransBackJigs.AddPart();
            m_TransBackJigs.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_TransBackJigs.Run(RunningModes.Online);

            m_CleanPlateform = new CleanPlateform(m_External, CleanStationInitialize, CleanStationOperate)
            {
                Name = "清洗平台",
                Xaxis = Lxaxis,
                Yaxis = Lyaxis,
                Zaxis = Lzaxis,
                CleanStopCylinder = cleanStopCylinder,
                CleanUpCylinder = cleanUpCylinder,
                FormerStationClient = FormerStationClient,
                CleanClampCylinder = cleanClampCylinder,
                CleanRotateCylinder = cleanRotateCylinder,
                CleanUpDownCylinder = cleanUpDownCylinder,
            };
            m_CleanPlateform.AddPart();
            m_CleanPlateform.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_CleanPlateform.Run(RunningModes.Online);

            m_GluePlateform = new GluePlateform(m_External, GlueStionInitialize, GlueStationOperate)
            {
                Name = "点胶平台",
                Xaxis = Rxaxis,
                Yaxis = Ryaxis,
                Zaxis = Rzaxis,
                GlueStopCylinder = glueStopCylinder,
                GlueUpCylinder = glueUpCylinder,
                GlueUpCylinder_Small = glueUpCylinder_Small,
                glueVisionClass = GlueVision,
                glueVisionClass_GlueText = GlueVision_Text,
                CalibglueVisionClass = CalibGlueVision,
                baslerCamera = BaslerCamera,
                lightControl = m_lightControl
            };
            m_GluePlateform.AddPart();
            m_GluePlateform.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_GluePlateform.Run(RunningModes.Online);
            m_GluePlateform.WbINItrans(Config.Instance.CurrentProductType);//加载白板模板 写入20201216
            AppendText($"写入白板参数类型名:{Config.Instance.CurrentProductType}");
            m_AAstation = new AAstation(m_External, AAstationInitialize, AAstationOperate)
            {
                Name = "AA平台",
                //Xaxis = Rxaxis,
                //MoveSixYaxis = AAyaxis,
                //LightZaxis = AAzaxis,
                //AAClampClawCylinder = AACalampClawCylinder,
                AAJigsStopCylinder = AAJigsStopCylinder,
                AAJigsUpCylinder = AAJigsUpCylinder,
                AAJigsCylinder_Small = AAJigsUpCylinder_Small,
                //AAStockpileStopCylinder = AAStockpileStopCylinder,
                //AAStockpileUpCylinder = AAStockpileUpCylinder,
                //AAUVUpDownCylinder = AAUVUpDownCylinder,
            };
            m_AAstation.AddPart();
            m_AAstation.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_AAstation.Run(RunningModes.Online);

            m_AAstockpile = new AAStockpile(m_External, AAstockpileInitialize, AAstockpileOperate)
            {
                Name = "AA堆料",
                AAStockpileStopCylinder = AAStockpileStopCylinder,
                AAStockpileUpCylinder = AAStockpileUpCylinder,
            };
            m_AAstockpile.AddPart();
            m_AAstockpile.StationAppendTextReceive += new System.Toolkit.Interfaces.DataReceiveCompleteEventHandler(DealWithReceiveData);
            m_AAstockpile.Run(RunningModes.Online);
            #endregion
            MachineOperation = new MachineOperate(() =>
            {
                return TransBackJigsInitialize.InitializeDone & CleanStationInitialize.InitializeDone & GlueStionInitialize.InitializeDone
                & MesInitialize.InitializeDone & AAstationInitialize.InitializeDone && AAstockpileInitialize.InitializeDone & !Global.IsLocating & ( HaspDoorShield);
            }, () =>
            {
                return MachineIsAlarm.IsAlarm | TransBackJigsIsAlarm.IsAlarm | AAstaionIsAlarm.IsAlarm | AAstockpileIsAlarm.IsAlarm | CleanStaionIsAlarm.IsAlarm | ClueStationsAlarm.IsAlarm | MesIsAlarm.IsAlarm;
            });

            #region 故障代码建立
            AddAlarms();
            int faultCode = 0;
            foreach (var arm in m_Mes.Alarms)
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in m_TransBackJigs.Alarms)
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in m_CleanPlateform.Alarms)
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in m_GluePlateform.Alarms)
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in m_AAstation.Alarms)//new
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in m_AAstockpile.Alarms)//new
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            foreach (var arm in MachineAlarms)
            {
                ConstructErrorCode(arm, ref faultCode);
            }
            #endregion
            //LoadingMessage("加载信号灯资源");
            #region 加载信号灯资源

            //StartButton1 = new EventButton(IoPoints.TDI6);
            StartButton2 = new LightButton(IoPoints.IDI19, IoPoints.IDO27);
            ResetButton = new LightButton(IoPoints.IDI21, IoPoints.IDO26);
            PauseButton = new LightButton(IoPoints.IDI20, IoPoints.IDO25);
            StopButton = new LightButton(IoPoints.IDI3, IoPoints.IDO25);//beiyong

            EstopButton = new EventButton(IoPoints.IDI22);
            EstopButton2 = new EventButton(IoPoints.IDI22);
            layerLight = new LayerLight(IoPoints.IDO30, IoPoints.IDO29, IoPoints.IDO28, IoPoints.IDO31);

            StartButton2.Pressed += btnStart_MouseDown;
            StartButton2.Released += btnStart_MouseUp;
            PauseButton.Pressed += btnPause_MouseDown;
            PauseButton.Released += btnPause_MouseUp;
            ResetButton.Pressed += btnReset_MouseDown;
            ResetButton.Released += btnReset_MouseUp;
            StopButton.Pressed += btnStop_MouseDown;
            StopButton.Released += btnStop_MouseUp;
            //StopButton.Pressed += btnAlarmClean_MouseDown;
            //StopButton.Released += btnAlarmClean_MouseUp;

            MachineOperation.StartButton = StartButton2;
            MachineOperation.PauseButton = PauseButton;
            MachineOperation.StopButton = StopButton;
            MachineOperation.ResetButton = ResetButton;
            MachineOperation.EstopButton = EstopButton;
            MachineOperation.EstopButton2 = EstopButton2;
            #endregion
            //LoadingMessage("加载子窗体资源");
            ShowAA(this, null);//调用AA
            ShowWb(this, null);//调用白板
            AddSubForm();
            //LoadingMessage("加载线程资源");
            //m_Carrier.AlarmClean += new Action(AlarmClean);

            m_CleanPlateform.SendRequest += new Action<int>(m_Mes.SendRequestMsg);
            m_GluePlateform.SendRequest += new Action<int>(m_Mes.SendRequestMsg);
            m_AAstation.ShowResult += new Action<bool>(ShowAllResult);
            SerialStart();
            IoPoints.IDO19.Value = false;
            IoPoints.IDO15.Value = true;//plasma 断电
            IoPoints.IDO14.Value = false;//断电
            IoPoints.IDO915.Value = false;//断电

            InitdgvResultShows();//结果数据

            timer1.Enabled = true;
            AppendText("数据加载完成");
            timer2.Enabled = true;


            //AppendText("等待员工登录中……");
            //new frmjobnum().ShowDialog();
            //if (Marking.SvJobNumber == "") AppendText($"员工未登录");
            //else AppendText($"员工{Marking.SvJobNumber}已登录");

            ////SV mes
            //Common.ReadCommonIniFile();

            //callaa.CallAAdlg();
        }
        public void LoadagainWb()
        {
            m_GluePlateform.WbINItrans(Config.Instance.CurrentProductType);
           
            CallWb.ExitAAImageDlg();
            CallWb.ExitAAImageDlg();
            Thread.Sleep(100);
            ShowAA(this, null);//调用AA
            ShowWb(this, null);//调用白板
            MessageBox.Show($"已重新加载白板请注意白板界面是否显示正常,若新增型号请配置白板路径，若不正常请关闭软件并重开");
        }
        public void ShowAllResult(bool result)
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                    InitdgvResultShows();
                    if (result) { lblAAResult.Text = "PASS"; lblAAResult.BackColor = Color.Green;/* ReflashdgvResultShows(100); ReflashdgvResultShows(0);*/ }
                    else
                    {
                        //ReflashdgvResultShows(100);
                        if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_LightON_NG)
                        { lblAAResult.Text = "点亮NG";/* ReflashdgvResultShows(1); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_MOVEControl_NG)
                        { lblAAResult.Text = "运控NG"; /*ReflashdgvResultShows(2);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_SEARCH_NG)
                        { lblAAResult.Text = "搜索NG"; /*ReflashdgvResultShows(3); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_OC_TUNE_NG)
                        { lblAAResult.Text = "OCNG"; /*ReflashdgvResultShows(4); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_TILT_TUNE_NG)
                        { lblAAResult.Text = "倾斜NG"; /*ReflashdgvResultShows(5);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_UVBefore_Check_NG)
                        { lblAAResult.Text = "UV前NG"; /*ReflashdgvResultShows(6);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_UVAfter_Check_NG)
                        { lblAAResult.Text = "UV后NG"; /*ReflashdgvResultShows(7);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_SN_NG)
                        { lblAAResult.Text = "条码NG";/* ReflashdgvResultShows(8); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.CleanMesLock_NG)
                        { lblAAResult.Text = "互锁NG"; /*ReflashdgvResultShows(9); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.Glue_Wb_LightNG)
                        { lblAAResult.Text = "白板点亮"; /*ReflashdgvResultShows(10);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.Glue_Vision_LocationNG)
                        { lblAAResult.Text = "定位NG";/* ReflashdgvResultShows(11);*/ }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.Glue_Vision_FindNG)
                        { lblAAResult.Text = "识别NG";/* ReflashdgvResultShows(12); */}
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.Glue_Wb_ParticeNG)
                        { lblAAResult.Text = "白板脏污"; }
                        else if (ResultClass.AllResultShow == (int)AAstation.AAFunctionResult.AA_LightLossFrame_NG)
                        { lblAAResult.Text = "点亮丢帧"; }
                        else
                           lblAAResult.Text = "其余NG";
                        lblAAResult.BackColor = Color.Red;

                    }
                    //保存config
                    SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                }
                catch { }
            }));
            
        }
        // 计算相机2采集图像时间
        private void computeGrabTime2(long time)
        {
            //lblGrabTime2.Text = "[  Time2 : " + time + "  ]";
        }


        // 相机1 halcon窗体显示图像
        private void processHImage1(HObject hImg)
        {
            //hWindowControl1.HalconWindow.ClearWindow();
            //hWindowControl1.HalconWindow.SetPart(0, 0, -1, -1);
            //hWindowControl1.HalconWindow.DispObj(hImg);

            //HOperatorSet.GenEmptyObj(out hImageSave1);
            //hImageSave1.Dispose();
            //HOperatorSet.CopyImage(hImg, out hImageSave1);

            //++count1;
            //lblCount1.Text = "[  Count1 : " + count1 + "  ]";
        }
        public void AlarmClean()
        {
            m_External.AlarmReset = true;
            Thread.Sleep(100);
            m_External.AlarmReset = false;
        }

        private void OpenTcpLightControl()
        {
            //try
            //{
            //    aaServer = new AsynTcpServer(Position.Instance.AAPort, Position.Instance.ServerIP);
            //    Marking.TcpServerOpenSuccess = true;
            //}
            //catch (Exception ex)
            //{
            //    Marking.TcpServerOpenSuccess = false;
            //}
            try
            {
                //光控链接
                m_lightControl = new LightControl();
                m_lightControl.LightControlIP = Config.Instance.LightControl_IP;
                if (m_lightControl.OpenLightControl()) picLightControl.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
                else picLightControl.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
                m_lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, VisionProductData.Instance.nLight);
                AppendText("光控链接成功");
                Marking.IsLightControl = true;
            }
            catch (Exception ex)
            {
                AppendText(ex.Message + ex.StackTrace);
                AppendText("光控链接失败");
                Marking.IsLightControl = false;
            }
        }
        private void OpenTcpClient()
        {
            try
            {
                //   if(FormerStationClient != null)
                {
                    //    FormerStationClient.ReleaseSocket();
                }
               
                FormerStationClient = new System.ToolKit.AsynTcpClient2(Config.Instance.FormerStationIp, Config.Instance.FormerStationPort);
                Thread.Sleep(300);
                FormerStationClient.AsynConnect();
                Thread.Sleep(300);
                if (FormerStationClient.IsConnected)
                {
                    Marking.TcpClientOpenSuccess = true;
                    AppendText("TCP客户端连接成功");
                }
                else
                {
                    Marking.TcpClientOpenSuccess = false;
                    AppendText("TCP客户端连接失败");
                }
            }
            catch (Exception ex)
            {
                Marking.TcpClientOpenSuccess = false;
                AppendText("TCP客户端连接异常");
            }
        }
        private void ReOpenTcpClient()
        {
            try
            {
                if (FormerStationClient != null)
                {
                    if (FormerStationClient.IsConnected)
                    {
                        FormerStationClient.ReleaseSocket();
                        Thread.Sleep(500);
                    }
                }
                FormerStationClient.AsynConnect();
                Thread.Sleep(300);
                if (FormerStationClient.IsConnected)
                {
                    Marking.TcpClientOpenSuccess = true;
                    AppendText("TCP客户端连接成功");
                }
                else
                {
                    Marking.TcpClientOpenSuccess = false;
                    AppendText("TCP客户端连接失败");
                }
            }
            catch (Exception ex)
            {
                Marking.TcpClientOpenSuccess = false;
                AppendText("TCP客户端连接异常");
            }
        }

        private void ConstructErrorCode(Alarm arm, ref int faultCode)
        {
            if (!Global.FaultDictionary.ContainsKey(arm.Name))
            {
                Global.Fault ft = new Global.Fault();
                faultCode++;
                ft.FaultCode = faultCode;
                ft.FaultMessage = arm.Name;
                ft.FaultCount = 0;
                Global.FaultDictionary.Add(arm.Name, ft);
            }
        }

        private List<Alarm> MachineAlarms;
        private void AddAlarms()
        {
            MachineAlarms = new List<Alarm>();

            MachineAlarms.Add(new Alarm(() => (!IoPoints.IDI22.Value/* || !IoPoints.TDI5.Value*/))
            {
                AlarmLevel = AlarmLevels.Error,
                Name = "急停按钮已按下，注意安全！"
            });
            MachineAlarms.Add(new Alarm(() =>((Config.Instance.GlueUseAllNums-Config.Instance.GlueUseNumsIndex)<=Config.Instance.GlueUseAlarmNums))
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "胶水次数已低于报警次数请及时换胶水！"
            });
            MachineAlarms.Add(new Alarm(() => (DateDiff(Config.Instance.GlueDataTime,DateTime.Now,true) >= Config.Instance.GlueTimeAlarm))
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "胶水使用时间已超规定时间请更换"
            });
            //MachineAlarms.Add(new Alarm(() => ((!IoPoints.IDI28.Value|| !IoPoints.IDI29.Value|| !IoPoints.IDI30.Value|| !IoPoints.IDI31.Value)&& !Marking.DoorShield))
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "安全门已打开,请注意安全！"
            //});

            //MachineAlarms.Add(new Alarm(() => !IoPoints.TDI8.Value & !Marking.CurtainShield)
            //{
            //    AlarmLevel = AlarmLevels.Warrning,
            //    Name = "安全光幕已感应！"
            //});

            MachineAlarms.Add(new Alarm(() => Marking.UVAfterAlarm & !Marking.UVAfterRst)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = "UV后检测NG！"
            });

            //MachineAlarms.Add(new Alarm(() => !Marking.TcpServerOpenSuccess)
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "TCP服务器打开失败，请检查IP地址或网线连接！"
            //});
            //MachineAlarms.Add(new Alarm(() => (!Marking.TcpClientOpenSuccess && !Marking.FormerStationShield))
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "TCP客户端打开失败，请检查IP地址或网线连接！"
            //});

            //MachineAlarms.Add(new Alarm(() => !Marking.HeightDetectorOpenSuccess)
            //{
            //    AlarmLevel = AlarmLevels.Warrning,
            //    Name = "测高串口打开失败，请检查通讯设置或线路连接！"
            //});

            MachineAlarms.Add(new Alarm(() => (!IoPoints.IDI11.Value && !Marking.GlueHaveShield))
            {
                AlarmLevel = AlarmLevels.Error,
                Name = "胶水液位报警，请检查！"
            });

            //MachineAlarms.Add(new Alarm(() => !IoPoints.IDI24.Value)
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "未检测到气压信号！"
            //});

            MachineAlarms.Add(new Alarm(() => !m_Mes.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "通讯模块未复位！"
            });

            MachineAlarms.Add(new Alarm(() => !m_TransBackJigs.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "回流线未回原点！"
            });

            MachineAlarms.Add(new Alarm(() => !m_CleanPlateform.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "清洗模组未回原点！"
            });

            MachineAlarms.Add(new Alarm(() => !m_GluePlateform.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "点胶模组未回原点！"
            });
            MachineAlarms.Add(new Alarm(() => !m_AAstockpile.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "AA堆料模组未回原点！"
            });
            MachineAlarms.Add(new Alarm(() => !m_AAstation.stationInitialize.InitializeDone)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "AA工位模组未回原点！"
            });
            MachineAlarms.Add(new Alarm(() => m_CleanPlateform.Xaxis.IsPEL || m_CleanPlateform.Xaxis.IsMEL)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "清洗X轴感应到限位！"
            });

            MachineAlarms.Add(new Alarm(() => m_CleanPlateform.Yaxis.IsPEL || m_CleanPlateform.Yaxis.IsMEL)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "清洗Y轴感应到限位！"
            });

            MachineAlarms.Add(new Alarm(() => m_CleanPlateform.Zaxis.IsPEL /*|| m_CleanPlateform.Zaxis.IsMEL*/)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "清洗Z轴感应到限位！"
            });

            MachineAlarms.Add(new Alarm(() => m_GluePlateform.Xaxis.IsPEL || m_GluePlateform.Xaxis.IsMEL)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "点胶X轴感应到限位！"
            });

            MachineAlarms.Add(new Alarm(() => m_GluePlateform.Yaxis.IsPEL || m_GluePlateform.Yaxis.IsMEL)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "点胶Y轴感应到限位！"
            });

            MachineAlarms.Add(new Alarm(() => m_GluePlateform.Zaxis.IsPEL || m_GluePlateform.Zaxis.IsMEL)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = "点胶Z轴感应到限位！"
            });

            //MachineAlarms.Add(new Alarm(() => !Marking.WbClientOpenFlg && !Marking.WhiteShield && !Marking.CleanShield)
            //{
            //    AlarmLevel = AlarmLevels.Warrning,
            //    Name = "白板检测软件未开启，请检查！"
            //});

            //MachineAlarms.Add(new Alarm(() => !Marking.AaClientOpenFlg && !Marking.CleanRecycleRun && !Marking.GlueRecycleRun && !Marking.AAShield)
            //{
            //    AlarmLevel = AlarmLevels.Warrning,
            //    Name = "AA软件未开启，请检查！"
            //});

            //MachineAlarms.Add(new Alarm(() => !hasp.LicenseIsOK && hasp.Duetime <= 0)
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "该软件为试用软件，现已到期，或加密狗已拔出，请尽快联系厂商！"
            //});

            //MachineAlarms.Add(new Alarm(() => !hasp.LicenseIsOK && hasp.Duetime > 0)
            //{
            //    AlarmLevel = AlarmLevels.Error,
            //    Name = "加密狗无法授权，请检查加密狗或联系厂商！"
            //});
        }
        #endregion
        /// <summary>
        /// receive flow information displayed to the mainview
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="result">return result</param>
        private void DealWithReceiveData(object sender, string result) => AppendText(result);

        #region 窗体关闭
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ManualAutoMode)
            {
                AppendText("软件无法退出，必须在手动模式才能操作！");
                e.Cancel = true;
            }
            else
            {
                IoPoints.IDO31.Value = false;
                DialogResult result = MessageBox.Show("是否保存配置文件再退出？", "退出", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    timer1.Enabled = false;
                    //SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                    //SerializerManager<AxisParameter>.Instance.Save(AppConfig.ConfigAxisName, AxisParameter.Instance);
                    //SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                    //SerializerManager<Delay>.Instance.Save(AppConfig.ConfigDelayName, Delay.Instance);

                    //SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
                    //SerializerManager<Relationship>.Instance.Save(AppConfig.ConfigCameraName, Relationship.Instance);
                    //SerializerManager<VisionProductData>.Instance.Save(VisionMarking.VisionFileName, VisionProductData.Instance);
                    Thread.Sleep(200);
                    log.Debug("配置文件已保存");

                    threadStatusCheck?.Abort();
                    threadMachineRun?.Abort();
                    threadAlarmCheck?.Abort();
                    m_Mes?.threadDealMsg?.Abort();
                    m_GluePlateform.Xaxis.Stop();
                    m_GluePlateform.Yaxis.Stop();
                    m_GluePlateform.Zaxis.Stop();
                    m_CleanPlateform.Xaxis.Stop();
                    m_CleanPlateform.Yaxis.Stop();
                    m_CleanPlateform.Zaxis.Stop();

                    IoPoints.IDO15.Value = false;
                    IoPoints.IDO11.Value = false;
                    IoPoints.IDO16.Value = false;

                    IoPoints.IDO14.Value = true;//断电
                    IoPoints.IDO915.Value = true;//断电
                    //CloseScanner();
                    //CloseDetectHeight();
                    ClosePower();
                    CloseTcpClient();
                   
                    try
                    {
                        m_lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, 0);
                        m_lightControl.CloseLightControl();
                    }
                    catch { }
                    MessageBox.Show("软件即将关闭，请注意清洗点胶装置！");
                    //Environment.Exit(0);
                    try
                    {
                        if ((int)DlgHandle_AA > 0) CallAA.ExitAAFuctionDlg();
                        if ((int)DlgHandle_wb > 0) CallWb.ExitAAImageDlg();
                    }
                    catch { }
                }
                else if (result == DialogResult.No)
                {
                    timer1.Enabled = false;
                    threadStatusCheck?.Abort();
                    threadMachineRun?.Abort();
                    threadAlarmCheck?.Abort();
                    m_Mes?.threadDealMsg?.Abort();
                    //m_GluePlateform.threadRun.Abort();

                    m_GluePlateform.Xaxis.Stop();
                    m_GluePlateform.Yaxis.Stop();
                    m_GluePlateform.Zaxis.Stop();

                    m_CleanPlateform.Xaxis.Stop();
                    m_CleanPlateform.Yaxis.Stop();
                    m_CleanPlateform.Zaxis.Stop();

                    CloseScanner();
                    CloseDetectHeight();
                    IoPoints.IDO15.Value = false;
                    IoPoints.IDO11.Value = false;
                    IoPoints.IDO16.Value = false;
        
                    IoPoints.IDO14.Value = true;//断电
                    IoPoints.IDO915.Value = true;//断电
             
                    ClosePower();
                    CloseTcpClient();
                    //CallAA.ExitAAFuctionDlg();
                    try
                    {
                        m_lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, 0);
                        m_lightControl.CloseLightControl();
                    }
                    catch { }
                    SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                    log.Debug("配置文件不保存");
                    MessageBox.Show("软件即将关闭，请注意清洗点胶装置！");
                    try
                    {
                        if ((int)DlgHandle_AA > 0) CallAA.ExitAAFuctionDlg();
                        if ((int)DlgHandle_wb > 0) CallWb.ExitAAImageDlg();
                    }
                    catch { }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region 窗体切换

        private void btnMain_Click(object sender, EventArgs e)
        {
            tbcMain.SelectedTab = tpgMain;
            CallAA.ShowWindow(DlgHandle_AA, 1);//1为显示，0为隐藏
            CallWb.ShowWindow(DlgHandle_wb, 1);//1为显示，0为隐藏
        }

        private void btnTeach_Click(object sender, EventArgs e)
        {
            new frmTeach(m_GluePlateform, m_CleanPlateform, m_TransBackJigs, m_AAstation, m_AAstockpile, m_Mes).ShowDialog();
        }

        private void btnShowWindows_Click(object sender, EventArgs e)
        {
            //if (!haspRegistered.Created)
            //{
            //    haspRegistered.ShowDialog();
            //}
        }
        private void btnParameter_Click(object sender, EventArgs e)
        {
            new frmParameter().ShowDialog();
            if (Marking.ShowResultHaveThrans) { Marking.ShowResultHaveThrans = false; InitdgvResultShows(); }//结果数据 }
         }
        private void btnCommSetting_Click(object sender, EventArgs e)
        {
            new frmCommSetting().ShowDialog();
        }
        private void btnCylinderDelay_Click(object sender, EventArgs e)
        {
            new frmCylinderDelay().ShowDialog();
        }


        private void btnIOMonitor_Click(object sender, EventArgs e)
        {
            tbcMain.SelectedTab = tpgIOmonitor;
            CallAA.ShowWindow(DlgHandle_AA, 0);//1为显示，0为隐藏
            CallWb.ShowWindow(DlgHandle_wb, 0);//1为显示，0为隐藏

        }
        private void btnProduct_Click(object sender, EventArgs e)
        {
            new frmRecipe(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\"),
                Config.Instance.CurrentProductType,
                () =>
                {
                    try
                    {
                        Config.Instance.CurrentProductType = frmRecipe.CurrentProductType;
                        SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);//1229
                        Position.Instance = SerializerManager<Position>.Instance.Load(AppConfig.ConfigPositionName);
                        //2020.10.5新增
                        DbModelParam.Instance = SerializerManager<DbModelParam>.Instance.Load(VisionMarking.VisionName);
                        VisionProductData.Instance = SerializerManager<VisionProductData>.Instance.Load(VisionMarking.VisionFileName);
                        //模板
                        
                        var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                        m_GluePlateform.glueVisionClass.ReadShapeModel(strLensmodelpath);

                        Marking.IsProductThrans = true;
                       
                    }
                    catch (Exception ex)
                    {
                        AppendText($"加载数据失败！");
                    }
                },
                () =>
                {
                    try
                    {
                        Config.Instance.CurrentProductType = frmRecipe.CurrentProductType;
                        SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);//1229
                        SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                        SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
                        SerializerManager<VisionProductData>.Instance.Save(VisionMarking.VisionFileName, VisionProductData.Instance);

                        //Marking.IsProductThrans = true;
                        var strmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{Config.Instance.CurrentProductType}{ DbModelParam.Instance.GlueLocationVisionParam.strID}Model.sbm");
                        DbModelParam.Instance.GlueLocationVisionParam.strModelPath = $"{Config.Instance.CurrentProductType}{DbModelParam.Instance.GlueLocationVisionParam.strID}Model.sbm";
                        m_GluePlateform.glueVisionClass.WriteShapeModel(strmodelpath);
                    }
                    catch (Exception ex)
                    {
                        AppendText($"保存数据失败！");
                    }

                }).ShowDialog();
            if (Marking.IsProductThrans)
            {
                Marking.IsProductThrans = false;
                LoadagainWb();
                if (Position.Instance.MesMode)
                    cbMesmode.Text = "MES模式";
                else cbMesmode.Text = "SV模式";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            frmLogin fm = new frmLogin();
            fm.ShowDialog();
            OnUserLevelChange(Config.Instance.userLevel);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否退出？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void AddSubForm()
        {
            GenerateForm(new frmIOmonitor(), tpgIOmonitor);


            //frmWhiteBorad wb = new frmWhiteBorad();
            //GenerateForm(wb, tpgAAImage);
            //wb.frmWhiteBorad_Load(this, null);

            //frmAAVision aa = new frmAAVision();
            //GenerateForm(aa, tpgAAVision);
            //aa.frmAAVision_Load(this, null);
        }

        /// <summary>
        /// 在选项卡中生成窗体
        /// </summary>
        private void GenerateForm(Form frm, TabPage sender)
        {
            //设置窗体没有边框 加入到选项卡中  
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Parent = sender;
            frm.ControlBox = false;
            frm.Dock = DockStyle.Fill;
            frm.Show();
        }
        #endregion

        #region 线程处理
        private void SerialStart()
        {
            try
            {
                //xumz
                threadMachineRun = new Thread(MachineRun);
                threadMachineRun.IsBackground = true;
                threadMachineRun.Start();
                if (threadMachineRun.IsAlive) AppendText("设备操作线程运行中...");

                threadAlarmCheck = new Thread(AlarmCheck);
                threadAlarmCheck.IsBackground = true;
                threadAlarmCheck.Start();
                if (threadAlarmCheck.IsAlive) AppendText("故障检查线程运行中...");

                threadStatusCheck = new Thread(StatusCheck);
                threadStatusCheck.IsBackground = true;
                threadStatusCheck.Start();
                if (threadStatusCheck.IsAlive) AppendText("状态检查线程运行中...");

                threadLicenseCheck = new Thread(LicenseCheck);
                threadLicenseCheck.IsBackground = true;
                threadLicenseCheck.Start();

                m_Mes?.threadDealMsg.Start();
                //m_GluePlateform.threadRun.Start();
            }
            catch (Exception ex)
            {
                AppendText("Server start Error: " + ex.Message);
            }
        }
        bool isCleanFireOpen = false;
        private void MachineRun()
        {
            var watchCT = new Stopwatch();
            watchCT.Start();
            while (true)
            {
                Thread.Sleep(50);
                m_External.AirSignal = true;
                m_External.ManualAutoMode = ManualAutoMode;

                layerLight.VoiceClosed = Marking.VoiceClosed;
                layerLight.Status = MachineOperation.Status;
                layerLight.Refreshing();

                m_GluePlateform.stationOperate.ManualAutoMode = ManualAutoMode;
                m_GluePlateform.stationOperate.AutoRun = MachineOperation.Running;
                m_GluePlateform.stationInitialize.Run();
                m_GluePlateform.stationOperate.Run();

                m_CleanPlateform.stationOperate.ManualAutoMode = ManualAutoMode;
                m_CleanPlateform.stationOperate.AutoRun = MachineOperation.Running;
                m_CleanPlateform.stationInitialize.Run();
                m_CleanPlateform.stationOperate.Run();

                m_TransBackJigs.stationOperate.ManualAutoMode = ManualAutoMode;
                m_TransBackJigs.stationOperate.AutoRun = MachineOperation.Running;
                m_TransBackJigs.stationInitialize.Run();
                m_TransBackJigs.stationOperate.Run();


                m_AAstation.stationOperate.ManualAutoMode = ManualAutoMode;
                m_AAstation.stationOperate.AutoRun = MachineOperation.Running;
                m_AAstation.stationInitialize.Run();
                m_AAstation.stationOperate.Run();

                m_AAstockpile.stationOperate.ManualAutoMode = ManualAutoMode;
                m_AAstockpile.stationOperate.AutoRun = MachineOperation.Running;
                m_AAstockpile.stationInitialize.Run();
                m_AAstockpile.stationOperate.Run();

                m_Mes.stationOperate.ManualAutoMode = ManualAutoMode;
                m_Mes.stationOperate.AutoRun = MachineOperation.Running;
                m_Mes.stationInitialize.Run();
                m_Mes.stationOperate.Run();

                MachineOperation.ManualAutoModel = ManualAutoMode;
                MachineOperation.CleanProductDone = Global.CleanProductDone;
                MachineOperation.Run();
                MachineOperation.Stop = !IoPoints.IDI22.Value /*&& !IoPoints.TDI5.Value*/;
                MachineOperation.Pause = IoPoints.IDI20.Value /*&& !Marking.DoorShield*/;//1
                if (!IoPoints.IDI22.Value /*|| !IoPoints.TDI5.Value *//*|| !hasp.LicenseIsOK*/)
                {
                    m_GluePlateform.Xaxis.Stop();
                    m_GluePlateform.Yaxis.Stop();
                    m_GluePlateform.Zaxis.Stop();
                    m_CleanPlateform.Xaxis.Stop();
                    m_CleanPlateform.Yaxis.Stop();
                    m_CleanPlateform.Zaxis.Stop();

                    m_GluePlateform.stationInitialize.InitializeDone = false;
                    m_TransBackJigs.stationInitialize.InitializeDone = false;
                    m_AAstation.stationInitialize.InitializeDone = false;
                    m_AAstockpile.stationInitialize.InitializeDone = false;
                    m_CleanPlateform.stationInitialize.InitializeDone = false;
                    m_Mes.stationInitialize.InitializeDone = false;
                    MachineOperation.IniliazieDone = false;
                }

                #region 设备运行中
                //if (/*IoPoints.TDI6.Value && */IoPoints.TDI7.Value)
                //{
                //    if (!ManualAutoMode)
                //    {
                //        //AppendText("设备无法启动，必须在自动模式才能操作！");
                //    }
                //    else
                //    {
                //        MachineOperation.Stop = false;
                //        MachineOperation.Reset = false;
                //        MachineOperation.Pause = false;
                //        MachineOperation.Start = true;
                //        //cdm 10.26改
                //        IoPoints.IDO0.Value = true;
                //        IoPoints.IDO16.Value = true;
                //    }
                //}
                if (ManualAutoMode && MachineOperation.Running)
                {
                    IoPoints.IDO25.Value = false;
                    IoPoints.IDO26.Value = false;
                    IoPoints.IDO27.Value = true;
                    TimeSpan ts = watchCT.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    Marking.CycleRunTime = elapsedTime;

                    AutoNeedleStatus = false;
                    AutoNeedleStatusRun = false;

                    if (isCleanFireOpen) { isCleanFireOpen = false; IoPoints.IDO16.Value = true; }//火焰
                }
                else
                {
                    //IoPoints.TDO8.Value = false;

                }

                if (MachineOperation.Pausing)
                {
                    //cdm 10.26改
                    IoPoints.IDO9.Value = false;
                    //IoPoints.IDO8.Value = false;
                    IoPoints.IDO90.Value = false;
                    //IoPoints.IDO914.Value = false;
                    IoPoints.IDO27.Value = false;
                    IoPoints.IDO25.Value = true;
                    if (IoPoints.IDO16.Value) { isCleanFireOpen = true; IoPoints.IDO16.Value = false; }
                   
                }

                if (!ManualAutoMode && AutoNeedleStatus
                    && m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.X)
                     && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.Y)
                     && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueAdjustPinPosition.Z))
                {
                    AutoNeedleStatusRun = true;
                }

                //if (!ManualAutoMode && !MachineOperation.Stopping && !MachineOperation.Resetting && AutoNeedleStatus && AutoNeedleStatusRun)
                //{
                //    MoveToNeedlePointP();//执行自动对针                  
                //}

                #endregion

                #region 设备复位中
                if (MachineOperation.Resetting)
                {
                    switch (MachineOperation.Flow)
                    {
                        case 0:
                            MachineOperation.IniliazieDone = false;
                            MachineOperation.Stop = false;
                            MachineOperation.Reset = false;
                            IoPoints.IDO25.Value = false;
                            IoPoints.IDO26.Value = true;
                            IoPoints.IDO27.Value = false;
                            m_External.InitializingDone = false;
                            m_GluePlateform.stationInitialize.InitializeDone = false;
                            m_TransBackJigs.stationInitialize.InitializeDone = false;
                            m_AAstation.stationInitialize.InitializeDone = false;
                            m_AAstockpile.stationInitialize.InitializeDone = false;
                            m_Mes.stationInitialize.InitializeDone = false;
                            m_CleanPlateform.stationInitialize.InitializeDone = false;
                            m_GluePlateform.stationInitialize.Start = false;
                            m_TransBackJigs.stationInitialize.Start = false;
                            m_AAstation.stationInitialize.Start = false;
                            m_AAstockpile.stationInitialize.Start = false;
                            m_Mes.stationInitialize.Start = false;
                            m_CleanPlateform.stationInitialize.Start = false;
                            if (true) MachineOperation.Flow = 10;
                            NeedleStep = 0;
                            AutoNeedleStatus = false;
                            AutoNeedleStatusRun = false;
                            break;
                        case 10:
                            m_TransBackJigs.stationInitialize.Start = true;
                            m_AAstation.stationInitialize.Start = true;
                            m_AAstockpile.stationInitialize.Start = true;
                            m_CleanPlateform.stationInitialize.Start = true;
                            m_GluePlateform.stationInitialize.Start = true;
                            m_Mes.stationInitialize.Start = true;
                            if (m_GluePlateform.stationInitialize.Running &&
                                m_CleanPlateform.stationInitialize.Running &&
                                m_TransBackJigs.stationInitialize.Running &&
                                m_AAstation.stationInitialize.Running &&
                                m_AAstockpile.stationInitialize.Running &&
                                m_Mes.stationInitialize.Running)
                            {
                                MachineOperation.Flow = 20;
                            }
                            break;
                        case 20:
                            if (m_GluePlateform.stationInitialize.Flow == -1 ||
                                m_CleanPlateform.stationInitialize.Flow == -1 ||
                                m_TransBackJigs.stationInitialize.Flow == -1 ||
                                m_AAstation.stationInitialize.Flow == -1 ||
                                m_AAstockpile.stationInitialize.Flow == -1 ||
                                m_Mes.stationInitialize.Flow == -1)
                            {
                                MachineOperation.IniliazieDone = false;
                                IoPoints.IDO26.Value = false;
                                MachineOperation.Flow = -1;
                            }
                            else
                            {
                                if (m_GluePlateform.stationInitialize.InitializeDone &&
                                    m_CleanPlateform.stationInitialize.InitializeDone &&
                                    m_TransBackJigs.stationInitialize.InitializeDone &&
                                    m_AAstation.stationInitialize.InitializeDone &&
                                    m_AAstockpile.stationInitialize.InitializeDone &&
                                    m_Mes.stationInitialize.InitializeDone)
                                {
                                    MachineOperation.IniliazieDone = true;
                                    IoPoints.IDO26.Value = false;
                                    MachineOperation.Flow = 50;
                                }
                            }
                            break;
                        default:
                            m_GluePlateform.stationInitialize.Start = false;
                            m_TransBackJigs.stationInitialize.Start = false;
                            m_AAstation.stationInitialize.Start = false;
                            m_AAstockpile.stationInitialize.Start = false;
                            m_CleanPlateform.stationInitialize.Start = false;
                            m_Mes.stationInitialize.Start = false;
                            break;
                    }
                }
                #endregion

                #region 设备停止中
                if (MachineOperation.Stopping)
                {
                    OnStop();

                    m_GluePlateform.stationInitialize.Estop = true;
                    m_TransBackJigs.stationInitialize.Estop = true;
                    m_AAstation.stationInitialize.Estop = true;
                    m_AAstockpile.stationInitialize.Estop = true;
                    m_CleanPlateform.stationInitialize.Estop = true;
                    m_Mes.stationInitialize.Estop = true;

                    MachineOperation.IniliazieDone = false;
                    m_TransBackJigs.stationInitialize.InitializeDone = false;
                    m_AAstation.stationInitialize.InitializeDone = false;
                    m_AAstockpile.stationInitialize.InitializeDone = false;
                    m_CleanPlateform.stationInitialize.InitializeDone = false;
                    m_GluePlateform.stationInitialize.InitializeDone = false;
                    m_Mes.stationInitialize.InitializeDone = false;
                    IoPoints.IDO16.Value = false;
                    IoPoints.IDO19.Value = false;

                    IoPoints.IDO9.Value = false;
                    IoPoints.IDO8.Value = false;
                    IoPoints.IDO90.Value = false;
                    IoPoints.IDO914.Value = false;
                    //IoPoints.IDO0.Value = false;////待定9.8
                    //IoPoints.IDO1.Value = false;

                    IoPoints.IDO25.Value = true;
                    IoPoints.IDO26.Value = false;
                    IoPoints.IDO27.Value = false;
                    IoPoints.IDO16.Value = false;
                    AutoNeedleStatus = false;
                    AutoNeedleStatusRun = false;

                    if (!m_GluePlateform.stationInitialize.Running &&
                        !m_TransBackJigs.stationInitialize.Running &&
                        !m_AAstation.stationInitialize.Running &&
                        !m_AAstockpile.stationInitialize.Running &&
                        !m_CleanPlateform.stationInitialize.Running &&
                        !m_Mes.stationInitialize.Running)
                    {
                        MachineOperation.IniliazieDone = false;
                        MachineOperation.Stopping = false;
                        m_GluePlateform.stationInitialize.Estop = false;
                        m_TransBackJigs.stationInitialize.Estop = false;
                        m_AAstation.stationInitialize.Estop = false;
                        m_AAstockpile.stationInitialize.Estop = false;
                        m_CleanPlateform.stationInitialize.Estop = false;
                        m_Mes.stationInitialize.Estop = false;
                    }
                }
                #endregion

                #region 设备急停中
                if (!IoPoints.IDI22.Value /*|| !IoPoints.TDI5.Value*/)
                {
                    MachineOperation.IniliazieDone = false;
                    m_TransBackJigs.stationInitialize.InitializeDone = false;
                    m_AAstation.stationInitialize.InitializeDone = false;
                    m_AAstockpile.stationInitialize.InitializeDone = false;
                    m_CleanPlateform.stationInitialize.InitializeDone = false;
                    m_GluePlateform.stationInitialize.InitializeDone = false;
                    m_Mes.stationInitialize.InitializeDone = false;

                    m_TransBackJigs.stationOperate.Stop = true;
                    m_AAstation.stationOperate.Stop = true;
                    m_AAstockpile.stationOperate.Stop = true;
                    m_CleanPlateform.stationOperate.Stop = true;
                    m_GluePlateform.stationOperate.Stop = true;
                    m_Mes.stationOperate.Stop = true;

                    m_CleanPlateform.Xaxis.IsServon = false;
                    m_CleanPlateform.Yaxis.IsServon = false;
                    m_CleanPlateform.Zaxis.IsServon = false;
                    m_GluePlateform.Xaxis.IsServon = false;
                    m_GluePlateform.Yaxis.IsServon = false;
                    m_GluePlateform.Zaxis.IsServon = false;
                    m_CleanPlateform.Xaxis.Stop();
                    m_CleanPlateform.Yaxis.Stop();
                    m_CleanPlateform.Zaxis.Stop();
                    m_GluePlateform.Xaxis.Stop();
                    m_GluePlateform.Yaxis.Stop();
                    m_GluePlateform.Zaxis.Stop();

                    //流水线停止
                    IoPoints.IDO9.Value = false;
                    IoPoints.IDO90.Value = false;
                    IoPoints.IDO8.Value = false;
                    IoPoints.IDO914.Value = false;
                    IoPoints.IDO16.Value = false;//清洗关闭
                    IoPoints.IDO19.Value = false;//点胶关闭
                    IoPoints.IDO26.Value = false;//复位状态清除

                    IoPoints.IDO14.Value = false;//断电
                    IoPoints.IDO915.Value = false;//断电

                    AutoNeedleStatus = false;
                    AutoNeedleStatusRun = false;
                }
                #endregion
            }
        }

        private void AlarmCheck()
        {
            while (true)
            {
                Thread.Sleep(50);
                CleanStaionIsAlarm = AlarmCheck(m_CleanPlateform.Alarms);
                TransBackJigsIsAlarm = AlarmCheck(m_TransBackJigs.Alarms);
                AAstaionIsAlarm = AlarmCheck(m_AAstation.Alarms);
                AAstockpileIsAlarm = AlarmCheck(m_AAstockpile.Alarms);
                ClueStationsAlarm = AlarmCheck(m_GluePlateform.Alarms);
                MesIsAlarm = AlarmCheck(m_Mes.Alarms);
                MachineIsAlarm = AlarmCheck(MachineAlarms);

            }
        }

        private void StatusCheck()
        {
            var list = new List<ICylinderStatusJugger>();
            m_Mes.stationInitialize.Estop = false;
            m_TransBackJigs.stationInitialize.Estop = false;
            m_CleanPlateform.stationInitialize.Estop = false;
            m_GluePlateform.stationInitialize.Estop = false;
            m_AAstation.stationInitialize.Estop = false;
            m_AAstockpile.stationInitialize.Estop = false;
            list.AddRange(m_GluePlateform.CylinderStatus);
            list.AddRange(m_CleanPlateform.CylinderStatus);
            list.AddRange(m_TransBackJigs.CylinderStatus);
            list.AddRange(m_AAstation.CylinderStatus);
            list.AddRange(m_AAstockpile.CylinderStatus);
            while (true)
            {
                Thread.Sleep(50);
                foreach (var lst in list)
                    lst.StatusJugger();
            }
        }

        private void LicenseCheck()
        {
            while (true)
            {
                try
                {
                    //if (!HaspDoorShield)
                        //hasp.UnauthorizedDetection();
                   
                    if (((Config.Instance.GlueUseAllNums - Config.Instance.GlueUseNumsIndex) <= Config.Instance.GlueUseAlarmNums)&&!MachineOperation.Alarming)
                    {
                        IoPoints.IDO28.Value = true;
                        IoPoints.IDO31.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;
                        Thread.Sleep(500);
                        IoPoints.IDO31.Value = true;
                        IoPoints.IDO28.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;
                        Thread.Sleep(500);
                        IoPoints.IDO31.Value = true;
                        IoPoints.IDO28.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;

                    }
                    if (DateDiff(Config.Instance.GlueDataTime, DateTime.Now, true) >= Config.Instance.GlueTimeAlarm && !MachineOperation.Alarming)
                    {
                        IoPoints.IDO28.Value = true;
                        IoPoints.IDO31.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;
                        Thread.Sleep(500);
                        IoPoints.IDO31.Value = true;
                        IoPoints.IDO28.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;
                        Thread.Sleep(500);
                        IoPoints.IDO31.Value = true;
                        IoPoints.IDO28.Value = true;
                        Thread.Sleep(2000);
                        IoPoints.IDO31.Value = false;
                        IoPoints.IDO28.Value = false;
                    }
                   
                    Thread.Sleep(20000);
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }
        }

        #region 数据表格初始化
        public string getdata(int datanum)
        {
            try
            {
                double tol = Config.Instance.ProductTotal == 0 ? 0 : (double)(datanum*1.0 / Config.Instance.ProductTotal);
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
        private void ReflashdgvResultShows(int i)
        {

            switch (i)
            {
                case 100:
                    dgvResultShow.Rows[0].SetValues(new object[] {
                    "总数",
                    Config.Instance.ProductTotal.ToString(),
                    getdata(Config.Instance.ProductTotal)
                     });
                    break;
                case 0:
                    dgvResultShow.Rows[1].SetValues(new object[] {
                     "OK",
                    Config.Instance.AllAAProductOkTotal.ToString(),
                      getdata(Config.Instance.AllAAProductOkTotal)
                     });
                    break;
                case 9:
                    dgvResultShow.Rows[2].SetValues(new object[] {
                      "互锁NG",
                    Config.Instance.CleanMesLockNgTotal.ToString(),
                    getdata(Config.Instance.CleanMesLockNgTotal)
                     });
                    break;
                case 10:
                    dgvResultShow.Rows[3].SetValues(new object[] {
                      "白板NG",
                    Config.Instance.GlueWbNgTotal_LightNG.ToString(),
                    getdata(Config.Instance.GlueWbNgTotal_LightNG)
                     });
                    break;
                case 11:
                    dgvResultShow.Rows[4].SetValues(new object[] {
                     "定位NG",
                    Config.Instance.GlueLocationNgTotal.ToString(),
                      getdata(Config.Instance.GlueLocationNgTotal)
                    });
                    break;
                case 12:
                    dgvResultShow.Rows[5].SetValues(new object[] {
                      "识别NG",
                    Config.Instance.GlueFindNgTotal.ToString(),
                      getdata(Config.Instance.GlueFindNgTotal)
                     });
                    break;
                case 1:
                    dgvResultShow.Rows[6].SetValues(new object[] {
                         "点亮NG",
                    Config.Instance.AALightNgTotal.ToString(),
                     getdata(Config.Instance.AALightNgTotal)
                     });
                    break;
                case 2:
                    dgvResultShow.Rows[7].SetValues(new object[] {
                        "运控NG",
                    Config.Instance.AAMoveNgTotal.ToString(),
                      getdata(Config.Instance.AAMoveNgTotal)
                     });
                    break;
                case 3:
                    dgvResultShow.Rows[8].SetValues(new object[] {
                         "搜索NG",
                    Config.Instance.AASerchNgTotal.ToString(),
                      getdata(Config.Instance.AASerchNgTotal)
                    });
                    break;
                case 4:
                    dgvResultShow.Rows[9].SetValues(new object[] {
                     "OCNG",
                    Config.Instance.AAOCNgTotal.ToString(),
                      getdata(Config.Instance.AAOCNgTotal)
                    });
                    break;
                case 5:
                    dgvResultShow.Rows[10].SetValues(new object[] {
                       "倾斜NG",
                    Config.Instance.AATILT_TUNENgTotal.ToString(),
                      getdata(Config.Instance.AATILT_TUNENgTotal)
                     });
                    break;
                case 6:
                    dgvResultShow.Rows[11].SetValues(new object[] {
                      "UV前NG",
                    Config.Instance.AAUVBeforeNgTotal.ToString(),
                      getdata(Config.Instance.AAUVBeforeNgTotal)
                    });
                    break;
                case 7:
                    dgvResultShow.Rows[12].SetValues(new object[] {
                      "UV后NG",
                    Config.Instance.AAUVAfterNgTotal.ToString(),
                      getdata(Config.Instance.AAUVAfterNgTotal)
                    });
                    break;
                case 8:
                    dgvResultShow.Rows[13].SetValues(new object[] {
                      "条码NG",
                    Config.Instance.AASNNgTotal.ToString(),
                      getdata(Config.Instance.AASNNgTotal)
                    });
                    break;
                default:
                    
                    break;
            }
                       
        }
        #endregion

        private double Okpercent;

        bool isPause = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (FormerStationClient != null)
            {
                Marking.TcpClientOpenSuccess = FormerStationClient.SocketIsConnected;
                picFormaStation.Image = Marking.TcpClientOpenSuccess ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
            }//上料机TCP监控

            //this.Text = hasp.Nature ? "AA点胶设备控制系统（永久版）" : $"AA点胶设备控制系统（试用版）";/*,到期还剩{hasp.Duetime}天*/
            //btnShowWindows.Visible = !hasp.Nature;
            //待检验
            if (MachineOperation.Pausing&&!isPause)
            {
                if (!m_CleanPlateform.Xaxis.IsServon || !m_CleanPlateform.Yaxis.IsServon || !m_CleanPlateform.Zaxis.IsServon
                    || !m_GluePlateform.Xaxis.IsServon || !m_GluePlateform.Yaxis.IsServon || !m_GluePlateform.Zaxis.IsServon)
                {
                    AppendText("轴掉使能请重新复位！");
                    MachineOperation.Stop = true;
                    MachineOperation.Reset = false;
                    MachineOperation.Pause = false;
                    MachineOperation.Start = false;
                    isPause = true;
                }

            }
            else isPause = false;
            #region 文本显示
            lblMachineStatus.Text = MachineOperation.Status.ToString();
            lblMachineStatus.ForeColor = MachineStatusColor(MachineOperation.Status);
            lblSwPath.Text = Config.Instance.ImageSAvePath;

            lblProductType.Text = Config.Instance.CurrentProductType;
            lblshowProductNAme.Text = Config.Instance.CurrentProductType;
            lblName.Text = AxisParameter.Instance.MachineName;
            lblTotalCycleTime.Text = $"{(Marking.CleanCycleTime + Marking.GlueCycleTime+ Marking.AACycleTime).ToString("f1")}s";
            lblTotalNum.Text = $"{Config.Instance.ProductTotal.ToString("f0")}pcs";
            lblOkNum.Text = $"{Config.Instance.AllAAProductOkTotal.ToString("f0")}pcs";
            lblNgNum.Text = $"{Config.Instance.AllAAProductNgTotal.ToString("f0")}pcs";

            lblGlueRunTime.Text = $"{ Marking.GlueCycleTime.ToString("0.0")}s";
            lblCleanRunTime.Text = $"{ Marking.CleanCycleTime.ToString("0.0")}s";
            lblAARunTime.Text = $"{ Marking.AACycleTime.ToString("0.0")}s";
            lblPlc.Text = $"心跳:{ Marking.PlcRefashTimes.ToString("0.0") }s";

            if (Marking.SvJobNumber == "")
            {  lblJobNum.Text = "员工未登录";
            }
            else
            {
                lblJobNum.Text = $"员工号:{Marking.SvJobNumber}" ;
            }
            #endregion

            #region 图像显示

            #endregion

            #region 操作权限变更
            btnStopVoice.BackColor = layerLight.VoiceClosed ? Color.LightPink : Color.Transparent;
            layerLight.Status = MachineOperation.Status;
            layerLight.Refreshing();

            btnManualAuto.Enabled = !MachineOperation.Running;
            btnReset.Enabled = !MachineOperation.Running;

            if (ManualAutoMode || Global.IsLocating)
            {
                btnTeach.Enabled = false;
                btnSetting.Enabled = false;
                btnVision.Enabled = false;
                btnPassword.Enabled = false;
                btnTest.Enabled = false;
            }
            else
            {
                if (Config.Instance.userLevel == UserLevel.管理员 || Config.Instance.userLevel == UserLevel.工程师)
                {
                    if (Config.Instance.userLevel == UserLevel.工程师)
                    {
                        btnTeach.Enabled = true;
                        btnSetting.Enabled = true;
                        btnVision.Enabled = true;
                        btnPassword.Enabled = false;
                        btnTest.Enabled = false;
                    }
                    if (Config.Instance.userLevel == UserLevel.管理员)
                    {
                        btnTeach.Enabled = true;
                        btnSetting.Enabled = true;
                        btnVision.Enabled = true;
                        btnPassword.Enabled = true;
                        btnTest.Enabled = true;
                    }
                }
            }
            //if (lstAlarm.Count > 0) lblAlarmMsg.Text = lstAlarm[0];
            //else
            //    lblAlarmMsg.Text = "";

            //Marking.ScannerEnable = chkScannerEnable.Checked;
            Marking.CleanShield = chkCleanShiled.Checked;
            Marking.CleanRun = chkCleanRun.Checked;
            Marking.GlueShield = chkGlueShiled.Checked;
            Marking.GlueRun = chkGlueRun.Checked;
            Marking.ProductTest = ckbProductTest.Checked;
            //Marking.HaveLensShield = chkLensShield.Checked;
            //Marking.PlasmaShield = chkPlasmaShield.Checked;
            Marking.WhiteShield = chkWhiteShiled.Checked;
            Marking.CCDShield = chkCCDShiled.Checked;
            Marking.CleanRecycleRun = chkCleanRecycleRun.Checked;
            Marking.GlueRecycleRun = chkGlueRecycleRun.Checked;
            Marking.PlasmaOn = cbPasmaOn.Checked;
            Marking.AAShield = cbAAshiled.Checked;
            chkCleanLensShiled.Checked =Position.Instance.CleanLensShield;
            chkCleanHolderShiled.Checked = Position.Instance.CleanHolderShield;

            if (MachineOperation.Running)
            {
                gbxCleanSetting.Enabled = false;
                gbxGlueSetting.Enabled = false;
                //gbxCarrierSetting.Enabled = false;
                //gbxCarrierButton.Enabled = false;
            }
            else
            {
                gbxCleanSetting.Enabled = true;
                gbxGlueSetting.Enabled = true;
                //gbxCarrierSetting.Enabled = true;
                //gbxCarrierButton.Enabled = true;
            }
            #endregion

            #region 通讯信息显示
            //RefreshCommMsg();
            #endregion

            #region AA状态
            try
            {
                //Marking.AAUpClyIsMove = m_AAstation.AAJigsUpCylinder.IsOutMove;
                if (IoPoints.IDO9.Value) { btnCleanMotionPostive.Text = "清洗送料线电机正转"; btnCleanMotionPostive.BaseColor = Color.Green; }
                else { btnCleanMotionPostive.Text = "清洗送料线电机停止"; btnCleanMotionPostive.BaseColor = Color.Red; }
                if (IoPoints.IDO8.Value) { btnGlueBackMotionIN.Text = "点胶回流线电机反转"; btnGlueBackMotionIN.BaseColor = Color.Green; }
                else { btnGlueBackMotionIN.Text = "点胶回流线电机停止"; btnGlueBackMotionIN.BaseColor = Color.Red; }
                if (IoPoints.IDO90.Value) { btnAAPositvie.Text = "AA送料线电机正转"; btnAAPositvie.BaseColor =  Color.Green; }
                else { btnAAPositvie.Text = "AA送料线电机停止"; btnAAPositvie.BaseColor = Color.Red; }
                if (IoPoints.IDO914.Value) { btnAABackIN.Text = "AA回流线电机反转"; btnAABackIN.BaseColor = Color.Green; }
                else { btnAABackIN.Text = "AA回流线电机停止"; btnAABackIN.BaseColor = Color.Red; }
                switch (CallAA.GetAAStatus())
                {
                    case 0:
                        Invoke((Action)(() => { StatusAA.Text = "Ready"; }));
                        break;
                    case 1:
                        Invoke((Action)(() => { StatusAA.Text = "复位中"; }));
                        break;
                    case 2:
                        Invoke((Action)(() => { StatusAA.Text = "测试中"; }));
                        break;
                    case 3://
                        Invoke((Action)(() => { StatusAA.Text = "报警状态"; }));
                        break;
                    case 4: //
                        Invoke((Action)(() => { StatusAA.Text = "暂停状态"; }));
                        break;

                    default:
                        Invoke((Action)(() => { StatusAA.Text = "未定义的报警"; }));
                        break;
                }

            }
            catch { }
            #endregion

         
            if (IoPoints.IDI15.Value) picPlasmaWork.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
            else picPlasmaWork.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;

            if (IoPoints.IDI25.Value) picPlasmaAready.Image = true ? Properties.Resources.LedGreen : Properties.Resources.LedNone;
            else picPlasmaAready.Image = false ? Properties.Resources.LedGreen : Properties.Resources.LedNone;

            if (!IoPoints.IDI24.Value) picPlasmaError.Image = true ? Properties.Resources.LedRed : Properties.Resources.LedNone;
            else picPlasmaError.Image = false ? Properties.Resources.LedRed : Properties.Resources.LedNone;

             
            //if (Marking.CarrierWorking && IoPoints.IDI18.Value||!MachineOperation.Running)
            //{
            //    IoPoints.IDO8.Value = false;
            //}
            //else
            //    IoPoints.IDO8.Value = true;

                lblGlueUSe.Text =$"胶水已使用次数{Config.Instance.GlueUseNumsIndex},胶水总次数{Config.Instance.GlueUseAllNums},报警预警次数{Config.Instance.GlueUseAlarmNums}";

            timer1.Enabled = true;
        }

        /// <summary>
        /// 显示通讯字符
        /// </summary>
        /// <param name="name">通讯模块标识符</param>
        /// <param name="flag">接收和发送标志，true接收，false发送</param>
        /// <param name="show">是否有字符需显示标志</param>
        /// <param name="msg">待显示的字符</param>
        public void ShowCommMsg(string name, bool flag, ref bool show, string msg)
        {
            if (msg != null && show)
            {
                show = false;
                if (flag)
                    AppendText(string.Format("接收到{0}字符：{1}", name, msg));
                else
                    AppendText(string.Format("发送给{0}字符：{1}", name, msg));
            }
        }

        /// <summary>
        /// 刷新所有的通讯信息
        /// </summary>
        public void RefreshCommMsg()
        {
            //if (aaServer != null)
            //{
            //    ShowCommMsg("AA", aaServer.Flag, ref aaServer.ShowMsg, aaServer.MsgShow);
            //    aaServer.MsgShow = null;
            //}
            //if (ccdServer != null)
            //    ShowCommMsg("视觉", ccdServer.Flag, ref ccdServer.ShowMsg, ccdServer.MsgShow);
            //if (wbServer != null)
            //{
            //    ShowCommMsg("白板", wbServer.Flag, ref wbServer.ShowMsg, wbServer.MsgShow);
            //    //if (wbServer.MsgShow != null && (wbServer.MsgShow.Contains("$HWA02") || wbServer.MsgShow.Contains("$HWA99")))
            //    //    wbServer.IsResultTCP = false;
            //}
            //if (heightDetector != null && heightDetector.ShowMsg)
            //{
            //    heightDetector.ShowMsg = false;
            //    if (heightDetector.Flag)
            //        AppendText(string.Format("接收到测高模块字符：{0}", heightDetector.MsgShow));
            //    else
            //        AppendText(string.Format("发送给测高模块字符：{0}", heightDetector.MsgShow));
            //}
        }

        private Color StationStatusColor(StationStatus status)
        {
            switch (status)
            {
                case StationStatus.模组报警:
                    return Color.Red;
                case StationStatus.模组未准备好:
                    return Color.Orange;
                case StationStatus.模组准备好:
                    return Color.Blue;
                case StationStatus.模组运行中:
                    return Color.Green;
                case StationStatus.模组暂停中:
                    return Color.Purple;
                default:
                    return Color.Red;
            }
        }
        #endregion

        #region 消息显示
        /// <summary>
        /// 使用委托方式更新AppendText显示
        /// </summary>
        /// <param name="txt">消息</param>
        public void AppendText(string txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendText), txt);
            }
            else
            {
                if (txt.Equals("ShowFN"))
                {
                    //lblCurrentFN.Text = MesData.NeedShowFN;
                    return;
                }
                if (lstInfo.Items.Count > 1000) lstInfo.Items.Clear();
                lstInfo.Items.Insert(0, string.Format("{0}-{1}" + Environment.NewLine, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), txt));
                log.Debug(txt);
            }
        }

        public AlarmType AlarmCheck(IList<Alarm> Alarms)
        {
            var Alarm = new AlarmType();
            foreach (Alarm alarm in Alarms)
            {
                var btemp = alarm.IsAlarm;
                if (alarm.AlarmLevel == AlarmLevels.Error)
                {
                    Alarm.IsAlarm |= btemp;
                    this.Invoke(new Action(() =>
                    {
                        Msg(string.Format("{0},{1}", alarm.AlarmLevel.ToString(), alarm.Name), btemp);
                        if (btemp)
                        {
                            if (Global.FaultDictionary.ContainsKey(alarm.Name))
                            {
                                var Fau = Global.FaultDictionary[alarm.Name];
                                Fau.FaultCount++;
                            }
                            if (Global.FaultDictionary1.ContainsKey(alarm.Name))
                            {
                                var Fau = Global.FaultDictionary1[alarm.Name];
                                Fau.FaultCount++;
                            }
                            else
                            {
                                fault.FaultCode = Global.FaultDictionary[alarm.Name].FaultCode;
                                fault.FaultCount = Global.FaultDictionary[alarm.Name].FaultCount;
                                fault.FaultMessage = Global.FaultDictionary[alarm.Name].FaultMessage;
                                Global.FaultDictionary1.Add(alarm.Name, fault);
                                faultcount++;
                            }

                        }
                    }));
                }
                else if (alarm.AlarmLevel == AlarmLevels.None)
                {
                    Alarm.IsPrompt |= btemp;
                    this.Invoke(new Action(() =>
                    {
                        Msg(string.Format("{0},{1}", alarm.AlarmLevel.ToString(), alarm.Name), btemp);
                    }));
                }
                else
                {
                    Alarm.IsWarning |= btemp;
                    this.Invoke(new Action(() =>
                    {
                        Msg(string.Format("{0},{1}", alarm.AlarmLevel.ToString(), alarm.Name), btemp);
                    }));
                }
            }
            return Alarm;
        }

        private void Msg(string str, bool value)
        {
            string tempstr = null;
            bool sign = false;
            try
            {
                var arrRight = new List<object>();
                foreach (var tmpist in lstAlarm.Items) arrRight.Add(tmpist);
                if (value)
                {
                    foreach (string tmplist in arrRight)
                    {
                        if (tmplist.IndexOf("-") > -1)
                        {
                            tempstr = tmplist.Substring(tmplist.IndexOf("-") + 1, tmplist.Length - tmplist.IndexOf("-") - 1);
                        }
                        if (tempstr == (str + "\r\n"))
                        {
                            sign = true;
                            break;
                        }
                    }
                    if (!sign)
                    {
                        lstAlarm.Items.Insert(0, (string.Format("{0}-{1}" + Environment.NewLine, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), str)));
                        log.Error(str);
                    }
                }
                else
                {
                    foreach (string tmplist in arrRight)
                    {
                        if (tmplist.IndexOf("-") > -1)
                        {
                            tempstr = tmplist.Substring(tmplist.IndexOf("-") + 1, tmplist.Length - tmplist.IndexOf("-") - 1);
                            if (tempstr == (str + "\r\n")) lstAlarm.Items.Remove(tmplist);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Color MachineStatusColor(MachineStatus status)
        {
            switch (status)
            {
                case MachineStatus.设备未准备好:
                    return Color.Orange;
                case MachineStatus.设备准备好:
                    return Color.Blue;
                case MachineStatus.设备运行中:
                    return Color.Green;
                case MachineStatus.设备停止中:
                    return Color.Red;
                case MachineStatus.设备暂停中:
                    return Color.Purple;
                case MachineStatus.设备复位中:
                    return Color.OrangeRed;
                case MachineStatus.设备报警中:
                    return Color.Red;
                case MachineStatus.设备急停已按下:
                    return Color.Red;
                default:
                    return Color.Red;
            }
        }
        #endregion

        #region 设备操作按钮

        private void btnStart_MouseDown(object sender, EventArgs e)
        {
            if (!ManualAutoMode)
            {
                AppendText("设备无法启动，必须在自动模式才能操作！");
                return;
            }
            if (Marking.SvJobNumber == ""&&Position.Instance.MesMode)
            {
                AppendText("员工号为空，请员工登录……");
                return;
            }
            MachineOperation.Start = true;
            MachineOperation.Stop = false;
            MachineOperation.Reset = false;
            MachineOperation.Pause = false;
            Marking.BaclFlowRuning_Glue = true;//流水线回流运行
            Marking.BaclFlowRuning_AA = true;//流水线回流运行
            //cdm 10.26改
            IoPoints.IDO9.Value = true;
            if(!m_AAstation.AAJigsUpCylinder.IsOutMove) IoPoints.IDO90.Value = true;
            //IoPoints.IDO8.Value = true;
        }

        private void btnStart_MouseUp(object sender, EventArgs e)
        {
            MachineOperation.Start = false;
        }

        private void btnPause_MouseDown(object sender, EventArgs e)
        {
            MachineOperation.Pause = true;
            MachineOperation.Stop = false;
            MachineOperation.Reset = false;
            MachineOperation.Start = false;

            //FormerStationClient.AsynSend("AskForPausing");
            //AppendText("请求上料段暂停动作....");
            //Thread.Sleep(1000);
            //String ReceiveString = "";
            //if (FormerStationClient.RevMessage != "")
            //{
            //    ReceiveString = FormerStationClient.RevMessage.Replace("\0", "");
            //}
            //if ((ReceiveString == "Pausing" || Marking.FormerStationShield))
            //{

            //    AppendText("上料段暂停");

            //}

        }

        private void btnPause_MouseUp(object sender, EventArgs e)
        {
            MachineOperation.Pause = false;
        }

        private void btnStop_MouseDown(object sender, EventArgs e)
        {
            MachineOperation.Stop = true;
            MachineOperation.Reset = false;
            MachineOperation.Pause = false;
            MachineOperation.Start = false;
            Marking.BaclFlowRuning_Glue = false;//流水线回流运行
            Marking.BaclFlowRuning_AA = false;//流水线回流运行
            //FormerStationClient.AsynSend("AskForPausing");
            //AppendText("请求上料段暂停动作....");
            //Thread.Sleep(1000);
            //String ReceiveString = "";
            //if (FormerStationClient.RevMessage != "")
            //{
            //    ReceiveString = FormerStationClient.RevMessage.Replace("\0", "");
            //}
            //if ((ReceiveString == "Pausing" || Marking.FormerStationShield))
            //{

            //    AppendText("上料段暂停");

            //}
        }

        private void btnStop_MouseUp(object sender, EventArgs e)
        {
            MachineOperation.Stop = false;
        }
        private void btnReset_MouseDown(object sender, EventArgs e)
        {
            if (ManualAutoMode)
            {
                if (!MachineIsAlarm.IsAlarm && !CleanStaionIsAlarm.IsAlarm
                     && !TransBackJigsIsAlarm.IsAlarm && !ClueStationsAlarm.IsAlarm)
                    AppendText("设备手动状态时，才能复位。自动状态只能清除报警！");
                m_External.AlarmReset = true;
            }
            else
            {
                DialogResult result = MessageBox.Show("请确认清洗点胶AA工位无遗留治具及流水线治具阻挡隔开并注意安全？", "提示", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if ((int)DlgHandle_AA <= 0|| (int)DlgHandle_wb <= 0)
                    {
                        MessageBox.Show("AA或白板界面未加载成功，请重新加载");
                        return;
                    }
                    if (MachineOperation != null)
                    {
                        MachineOperation.IniliazieDone = false;
                        MachineOperation.Flow = 0;
                        MachineOperation.Reset = true;
                        MachineOperation.Stop = false;
                        MachineOperation.Pause = false;
                        MachineOperation.Start = false;
                    }
                }
            }
        }

        private void btnReset_MouseUp(object sender, EventArgs e)
        {
            MachineOperation.Reset = false;
            m_External.AlarmReset = false;
        }
        private void btnManualAuto_Click(object sender, EventArgs e)
        {
            if (ManualAutoMode && MachineOperation.Running) //自动模式不能直接切换为手动，需要先停止运行再切换模式
            {
                AppendText("设备运行中，不能切换至手动模式!");
                return;
            }
            ManualAutoMode = ManualAutoMode ? false : true;
            btnManualAuto.Text = ManualAutoMode ? "自动模式" : "手动模式";
            btnManualAuto.ForeColor = ManualAutoMode ? Color.Green : Color.Red;
            if (ManualAutoMode) tbcMain.SelectedTab = tpgMain;
        }

        private void btnProductSetting_Click(object sender, EventArgs e)
        {
            if (MachineOperation.Running)
            {
                AppendText("设备停止或暂停时，才能操作！");
                return;
            }
            new frmRunSetting().ShowDialog();
        }

        private void btnTapping_Click(object sender, EventArgs e)
        {
            //if (MachineOperation.Running)
            //{
            //    AppendText("设备停止或暂停时，才能操作！");
            //    return;
            //}
            //else
            //{
            //    bool itrue = true;
            //    int step = 0;
            //    while (itrue)
            //    {
            //        switch (step)
            //        {
            //            case 0:
            //                MoveToPointP(Position.Instance.CutGlueStartPosition);
            //                step = 10;
            //                break;
            //            case 10:
            //                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.CutGlueStartPosition.X)
            //                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Y)
            //                    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Z))
            //                {
            //                    m_GluePlateform.Xaxis.MoveTo(Position.Instance.CutGlueEndPosition.X, Global.RXmanualSpeed);
            //                    m_GluePlateform.Yaxis.MoveTo(Position.Instance.CutGlueEndPosition.Y, Global.RYmanualSpeed);
            //                    step = 20;
            //                }
            //                break;
            //            case 20:
            //                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.CutGlueEndPosition.X)
            //                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.CutGlueEndPosition.Y))
            //                {
            //                    MoveToPointP(Position.Instance.GlueSafePosition);
            //                    step = 30;
            //                }
            //                break;
            //            case 30:
            //                if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.GlueSafePosition.X)
            //                    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.GlueSafePosition.Y)
            //                    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
            //                {
            //                    itrue = false;
            //                    step = 40;
            //                }
            //                break;
            //        }
            //    }
            //    //MoveToPointP(Position.Instance.CutGlueStartPosition);
            //    //if (m_GluePlateform.Xaxis.IsInPosition(Position.Instance.CutGlueStartPosition.X)
            //    //    && m_GluePlateform.Yaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Y)
            //    //    && m_GluePlateform.Zaxis.IsInPosition(Position.Instance.CutGlueStartPosition.Z))
            //    //{
            //    //    m_GluePlateform.Xaxis.MoveTo(Position.Instance.CutGlueEndPosition.X, Global.RXmanualSpeed);
            //    //    m_GluePlateform.Yaxis.MoveTo(Position.Instance.CutGlueEndPosition.Y, Global.RYmanualSpeed);
            //    //}
            //}
        }

        private void btnAlarmClean_MouseUp(object sender, MouseEventArgs e)
        {
            m_External.AlarmReset = false;
        }


        private void btnAlarmClean_MouseDown(object sender, MouseEventArgs e)
        {
            m_External.AlarmReset = true;
            //m_GluePlateform.Xaxis.Clean();
            //m_GluePlateform.Yaxis.Clean();
            //m_GluePlateform.Zaxis.Clean();
            //m_CleanPlateform.Xaxis.Clean();
            //m_CleanPlateform.Yaxis.Clean();
            //m_CleanPlateform.Zaxis.Clean();

            if (m_GluePlateform.Xaxis.IsAlarmed) IoPoints.AlarmDO0.Value = true;
            if (m_GluePlateform.Yaxis.IsAlarmed) IoPoints.AlarmDO1.Value = true;
            if (m_GluePlateform.Zaxis.IsAlarmed) IoPoints.AlarmDO2.Value = true;

            //IoPoints.AlarmDO3.Value = true;
            if (m_CleanPlateform.Xaxis.IsAlarmed) IoPoints.AlarmDO4.Value = true;
            if (m_CleanPlateform.Yaxis.IsAlarmed) IoPoints.AlarmDO5.Value = true;
            if (m_CleanPlateform.Zaxis.IsAlarmed) IoPoints.AlarmDO6.Value = true;

            //IoPoints.AlarmDO7.Value = true;
            if (IoPoints.AlarmDO0.Value|| IoPoints.AlarmDO1.Value|| IoPoints.AlarmDO2.Value||
                IoPoints.AlarmDO4.Value|| IoPoints.AlarmDO5.Value|| IoPoints.AlarmDO6.Value)
                Thread.Sleep(50);

            if (IoPoints.AlarmDO0.Value) IoPoints.AlarmDO0.Value = false;
            if (IoPoints.AlarmDO1.Value) IoPoints.AlarmDO1.Value = false;
            if (IoPoints.AlarmDO2.Value) IoPoints.AlarmDO2.Value = false;
            //IoPoints.AlarmDO3.Value = false;
            if (IoPoints.AlarmDO4.Value) IoPoints.AlarmDO4.Value = false;
            if (IoPoints.AlarmDO5.Value) IoPoints.AlarmDO5.Value = false;
            if (IoPoints.AlarmDO6.Value) IoPoints.AlarmDO6.Value = false;
            //IoPoints.AlarmDO7.Value = false;
           



        }

        //private void chkScannerEnable_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkScannerEnable.Checked)
        //    {
        //        OpenScanner();
        //    }
        //}
        string hc = "\r\n";
        public void Write()
        {
            try
            {
                if (Directory.Exists(Path.Combine("E:\\AAData")) == false)
                {
                    Directory.CreateDirectory(Path.Combine("E:\\AAData"));
                }
                FileStream fs = new FileStream($"E:\\AAData\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")+ ".txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                sw.Write($"线体{AxisParameter.Instance.MachineName}_{DateTime.Now.ToString("yyyy-MM-dd-HH")}产能" + hc);
                sw.Write($"当前产品型号{Config.Instance.CurrentProductType}" + hc);
                double ly = Config.Instance.ProductTotal == 0 ? 100 : Config.Instance.AllAAProductOkTotal*1.0/Config.Instance.ProductTotal*100.0;
                sw.Write($"良率{ly.ToString("0.00")}%" + hc);
                sw.Write($"总数："+Config.Instance.ProductTotal.ToString()+hc);
                sw.Write($"OK：" + Config.Instance.AllAAProductOkTotal.ToString() + hc);
                sw.Write($"互锁NG：" + Config.Instance.CleanMesLockNgTotal.ToString() + hc);
                sw.Write($"白板点亮：" + Config.Instance.GlueWbNgTotal_LightNG.ToString() + hc);
                sw.Write($"白板脏污：" + Config.Instance.GlueWbNgTotal_ParticeNG.ToString() + hc);
                sw.Write($"定位NG：" + Config.Instance.GlueLocationNgTotal.ToString() + hc);
                sw.Write($"识别NG：" + Config.Instance.GlueFindNgTotal.ToString() + hc);
                sw.Write($"点亮NG：" + Config.Instance.AALightNgTotal.ToString() + hc);
                sw.Write($"点亮丢帧：" + Config.Instance.AALightLossFrameNgTotal.ToString() + hc);
                sw.Write($"运控NG：" + Config.Instance.AAMoveNgTotal.ToString() + hc);
                sw.Write($"搜索NG：" + Config.Instance.AASerchNgTotal.ToString() + hc);
                sw.Write($"OCNG：" + Config.Instance.AAOCNgTotal.ToString() + hc);
                sw.Write($"倾斜NG：" + Config.Instance.AATILT_TUNENgTotal.ToString() + hc);
                sw.Write($"UV前NG：" + Config.Instance.AAUVBeforeNgTotal.ToString() + hc);
                sw.Write($"UV后NG：" + Config.Instance.AAUVAfterNgTotal.ToString() + hc);
                sw.Write($"条码NG：" + Config.Instance.AASNNgTotal.ToString() + hc);
                sw.Write($"其余NG：" + Config.Instance.NoneNgTotal.ToString() + hc);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
                AppendText($"保存产能数据成功E:\\AAData");
            }
            catch { }

        }

        private void btnCountClean_Click(object sender, EventArgs e)
        {
            if (MachineOperation.Running)
            {
                AppendText("设备停止或暂停时，才能操作！");
                return;
            }
            DialogResult result = MessageBox.Show("是否清除计数？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Write();
                Config.Instance.CleanMesLockNgTotal = 0;               
                Config.Instance.GlueWbNgTotal_LightNG = 0;
                Config.Instance.GlueWbNgTotal_ParticeNG = 0;
                Config.Instance.GlueLocationNgTotal = 0;
                Config.Instance.GlueFindNgTotal = 0;
                Config.Instance.AALightNgTotal = 0;
                Config.Instance.AALightLossFrameNgTotal = 0;
                Config.Instance.AAMoveNgTotal = 0;
                Config.Instance.AAOCNgTotal = 0;
                Config.Instance.AASerchNgTotal = 0;
                Config.Instance.AASNNgTotal = 0;
                Config.Instance.AATILT_TUNENgTotal = 0;
                Config.Instance.AAUVAfterNgTotal = 0;
                Config.Instance.AAUVBeforeNgTotal = 0;               
                Config.Instance.NoneNgTotal = 0;
                Config.Instance.AllAAProductNgTotal = 0;
                Config.Instance.AllAAProductOkTotal = 0;
                InitdgvResultShows();
                SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            new frmStationDebug(m_CleanPlateform, m_GluePlateform, m_TransBackJigs, m_AAstation, m_AAstockpile).ShowDialog();
           
        }

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

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.M:
                    if (ManualAutoMode && MachineOperation.Running) //自动模式不能直接切换为手动，需要先停止运行再切换模式
                    {
                        AppendText("设备运行中，不能切换至手动模式!");
                        return;
                    }
                    ManualAutoMode = ManualAutoMode ? false : true;
                    btnManualAuto.Text = ManualAutoMode ? "自动模式" : "手动模式";
                    btnManualAuto.ForeColor = ManualAutoMode ? Color.Green : Color.Red;
                    if (ManualAutoMode) tbcMain.SelectedTab = tpgMain;
                    break;
            }
        }

        bool IsAllSetShield;



  

        private void btnAAImage_Click(object sender, EventArgs e)
        {
            //tbcMain.SelectedTab = tpgAAImage;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //if (tbcMain.SelectedTab == tpgMain)
            //{
            //    tbcMain.SelectedTab = tpgAAImage;
            //}

            //if (tbcMain.SelectedTab == tpgAAImage)
            //{
            //    //切回主页面不再切换
            //    tbcMain.SelectedTab = tpgMain;
            //    timer2.Enabled = false;
            //}

            if (!Marking.TcpClientOpenSuccess&&!Marking.FormerStationShield)
                ReOpenTcpClient();

            CallAA.FreeMem();//释放  20201014
            lblGlueTime.Text = DateDiff(Config.Instance.GlueDataTime,DateTime.Now);
            
        }

        private void BTN_RECONNECT_FORMERSTATION_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("是否重新连接上料机服务器", "", MessageBoxButtons.YesNo))
            {
                ReOpenTcpClient();
            }
        }

        private void btnAAVision_Click(object sender, EventArgs e)
        {
            //tbcMain.SelectedTab = tpgAAVision;
            new frmModelBuild(m_GluePlateform.baslerCamera,m_GluePlateform.lightControl, DbModelParam.Instance.GlueLocationVisionParam,
               "GlueVisionParam", typeof(DbModelParam), () =>
               {
                   try
                   {
                       var strUpmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                           $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                       m_GluePlateform.glueVisionClass.ReadShapeModel(strUpmodelpath);
                   }
                   catch (Exception ex)
                   {
                       AppendText($"点胶相机加载模板失败！");
                   }
               }).ShowDialog();

        }



        private void ShowAA(object sender, EventArgs e)
        {
            //new frmShow().Show();
            CallAA.ShowAAFuntionDlg();
             DlgHandle_AA = CallAA.GetAAFuntionDlgHwnd();
            //groupBox1.Controls.Add();
            if ((int)DlgHandle_AA > 0) AppendText($"AA句柄{DlgHandle_AA}");
            else { AppendText($"AA界面加载异常请重新加载"); MessageBox.Show("AA界面加载异常请重新加载"); }
            CallAA.SetWindowPos(DlgHandle_AA, 0, groupBox1.Location.X+5, toolStrip1.Location.Y + groupBox1.Location.Y + panel3.Location.Y + toolStrip3.Location.Y+90, 470, 650, 0);
            CallAA.ShowWindow(DlgHandle_AA, 1);//1为显示，0为隐藏
        }

         private void ShowWb(object sender, EventArgs e)
        {
            CallWb.ShowAAImageDlg();
            DlgHandle_wb = CallWb.GetAAImageDlgHwnd();
            if ((int)DlgHandle_wb > 0) AppendText($"白板句柄{DlgHandle_wb}");
            else { AppendText($"白板界面加载异常请重新加载"); MessageBox.Show("白板界面加载异常请重新加载"); }
            // IntPtr 
            CallWb.SetWindowPos(DlgHandle_wb, 0, groupBox6.Location.X + 5, Wbshow.Location.Y+115, Wbshow.Width+20, Wbshow.Height-20, 0);
            CallWb.ShowWindow(DlgHandle_wb, 1);//1为显示，0为隐藏
        }


        private void btnCamaeraset_Click(object sender, EventArgs e)
        {
            new frmGrab(m_GluePlateform.baslerCamera, m_GluePlateform.lightControl).Show();
        }

        private void btnLoadcation_Click(object sender, EventArgs e)
        {
            new frmModelBuild(m_GluePlateform.baslerCamera, m_GluePlateform.lightControl, DbModelParam.Instance.GlueLocationVisionParam,
              "LoadcationGlueVisionParam", typeof(DbModelParam), () =>
              {
                  try
                  {
                      var strUpmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                          $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");
                      m_GluePlateform.glueVisionClass.ReadShapeModel(strUpmodelpath);
                  }
                  catch (Exception ex)
                  {
                      AppendText($"点胶相机加载模板失败！");
                  }
              }).ShowDialog();
        }

        private void chkGlueRun_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnGlueVision_Click(object sender, EventArgs e)
        {
            new frmModelBuild(m_GluePlateform.baslerCamera, m_GluePlateform.lightControl, DbModelParam.Instance.GlueFindVisionParam,
             "GlueVisionParam", typeof(DbModelParam), () =>
             {
                 try
                 {
                     var strUpmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         $"Vision\\{DbModelParam.Instance.GlueFindVisionParam.strModelPath}");
                     m_GluePlateform.glueVisionClass_GlueText.ReadShapeModel(strUpmodelpath);
                 }
                 catch (Exception ex)
                 {
                     AppendText($"点胶相机加载模板失败！");
                 }
             }).ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CallAA.ShowAAFuntionDlg();
            IntPtr DlgHandle = CallAA.GetAAFuntionDlgHwnd();
            //groupBox1.Controls.Add();
            CallAA.ShowWindow(DlgHandle, 1);//1为显示，0为隐藏
            CallAA.SetWindowPos(DlgHandle, 0, groupBox1.Location.X, groupBox1.Location.Y, 560, 800, 0);
           
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            CallAA.StartAAFunction("DNN00123", "JIG_01");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder(3072);//指定的buf大小必须大于传入的字符长度

            //buf.Append("abc中国人");

            //int outdata = GetErrorMessage(buf, 1);
            int nResult =CallAA.GetTestResult(buf);
            string strout = buf.ToString();
            MessageBox.Show(strout);
            MessageBox.Show(nResult.ToString());
        }

        private void btnCleanMotionPostive_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO9.Value)
            {
                IoPoints.IDO9.Value = true;
               
            }
            else
            {
                IoPoints.IDO9.Value = false;
             
            }
        }

        private void btnGlueBackMotionIN_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO8.Value)
            {
                IoPoints.IDO8.Value = true;
             
            }
            else
            {
                IoPoints.IDO8.Value = false;
                Marking.BaclFlowRuning_Glue = false;
            }
        }

        private void btnAAPositvie_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO90.Value)
            {
                IoPoints.IDO90.Value = true;
                //btnAAPositvie.Text = "AA送料线电机正转";
            }
            else
            {
                IoPoints.IDO90.Value = false;
                //btnAAPositvie.Text = "AA送料线电机停止";
            }
        }

        private void btnAABackIN_Click(object sender, EventArgs e)
        {
            if (!IoPoints.IDO914.Value)
            {
                IoPoints.IDO914.Value = true;
                //btnAABackIN.Text = "AA回流线电机反转";
            }
            else
            {
                IoPoints.IDO914.Value = false;
                //btnAABackIN.Text = "AA回流线电机停止";
                Marking.BaclFlowRuning_AA = false;//流水线回流运行
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CallWb.StartAAImage("DNN00123", "JIG_01");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder(3072);//指定的buf大小必须大于传入的字符长度

            int nResult = CallWb.GetAAImageTestResult(buf);
            string strout = buf.ToString();
            MessageBox.Show(strout);
            MessageBox.Show(nResult.ToString());
        }

        private void btnGlueUseIndexZero_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否胶水当前次数清零！", "", MessageBoxButtons.YesNo)) return;
            Config.Instance.GlueUseNumsIndex = 0;
            if (DialogResult.Yes == MessageBox.Show($"是否同时当前时间设为胶水使用开始记录时间", "", MessageBoxButtons.YesNo)) Config.Instance.GlueDataTime = DateTime.Now;
            SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
        }

        private void btnGlueTimeStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否开始记录当前新换胶水时间！", "", MessageBoxButtons.YesNo)) return;
            Config.Instance.GlueDataTime = DateTime.Now;
            SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
        }

        private void btnPowerOpen_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == (MessageBox.Show("是否重启程控电源", "", MessageBoxButtons.YesNo))) { return; }
            OpenPower();//新增20201112
            log.Debug("电源控制重启");
        }

        private void btnPowShow_Click(object sender, EventArgs e)
        {
            WhiteBoardPower frmPower = new WhiteBoardPower(this.PowerserialPort);
            frmPower.ShowDialog();
        }
        #region 程控电源控制
        public void OpenPower()
        {

            try
            {
                if (PowerserialPort.IsOpen)
                {
                    PowerserialPort.Close();
                    Thread.Sleep(200);
                }
                var ConnectionParam = Config.Instance.PowerComString;
                string[] param = ConnectionParam.Split(',');
                PowerserialPort.PortName = param[0];
                PowerserialPort.BaudRate = int.Parse(param[1]);
                PowerserialPort.Parity = (Parity)Enum.Parse(typeof(Parity), param[2]);
                PowerserialPort.DataBits = int.Parse(param[3]);
                PowerserialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), param[4]);
                PowerserialPort.ReadTimeout = int.Parse(param[5]);
                PowerserialPort.WriteTimeout = int.Parse(param[6]);
                PowerserialPort.RtsEnable = true;
                PowerserialPort.DtrEnable = true;

                if (!PowerserialPort.IsOpen)
                {
                    PowerserialPort.Open();
                    bool success = PowerserialPort.IsOpen;

                    if (success)
                    {

                        PowerserialPort.Write("SYST:REM" + Environment.NewLine);//远程PC操作指令
                        Thread.Sleep(50);
                        PowerserialPort.Write($"INST CH{Config.Instance.PowerChanel_Wb}" + Environment.NewLine);//选择频道1
                        PowerserialPort.Write("VOLT " + Position.Instance.Voltage_Wb.ToString() + Environment.NewLine);
                        Thread.Sleep(50);//电压
                        PowerserialPort.Write("CURR " + Position.Instance.Current_Wb.ToString() + Environment.NewLine);
                        Thread.Sleep(50);//电流

                        PowerserialPort.Write($"INST CH{Config.Instance.PowerChanel_AA}" + Environment.NewLine);
                        Thread.Sleep(50);
                        PowerserialPort.Write("VOLT " + Position.Instance.Voltage_AA.ToString() + Environment.NewLine);
                        Thread.Sleep(50);
                        PowerserialPort.Write("CURR " + Position.Instance.Current_AA.ToString() + Environment.NewLine);
                        Thread.Sleep(50);


                        PowerserialPort.Write($"OUTP {Config.Instance.PowerChanel_Wb}" + Environment.NewLine);//用于开启通道
                        Thread.Sleep(50);
                        PowerserialPort.Write($"OUTP {Config.Instance.PowerChanel_AA}" + Environment.NewLine);//用于开启通道
                        AppendText($"写入参数成功");



                    }
                    else
                    {
                        AppendText($"电源端口打开失败");
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"电源端口打开异常:" + ex.ToString());
                //     LoadingMessage("白板电源端口打开失败");

            }
        }

        private void btnRestAA_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否需要退出AA&白板软件并重新加载", "", MessageBoxButtons.YesNo)) return;

            if (MachineOperation.Running)
            {
                AppendText("设备暂停时，才能操作！");
                return;
            }
            LoadagainWb();
        }

        private void btnPassword_Click(object sender, EventArgs e)
        {
            frmPasswordChange fm = new frmPasswordChange();
            fm.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("是否保存参数？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                SerializerManager<AxisParameter>.Instance.Save(AppConfig.ConfigAxisName, AxisParameter.Instance);
                SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                SerializerManager<Delay>.Instance.Save(AppConfig.ConfigDelayName, Delay.Instance);

                SerializerManager<DbModelParam>.Instance.Save(VisionMarking.VisionName, DbModelParam.Instance);
                SerializerManager<Relationship>.Instance.Save(AppConfig.ConfigCameraName, Relationship.Instance);
                SerializerManager<VisionProductData>.Instance.Save(VisionMarking.VisionFileName, VisionProductData.Instance);
            }

        }

        private void ckbProductTest_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbProductTest.Checked)
            {
                chkCleanShiled.Checked = true;
                chkGlueShiled.Checked = true;
            }
            else
            {
                chkCleanShiled.Checked = false;
                chkGlueShiled.Checked = false;
            }
        }

        private void btnJobnumLogin_Click(object sender, EventArgs e)
        {
            new frmjobnum().ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("是否模式切换并需要重新复位", "", MessageBoxButtons.YesNo)) return;
            if (cbMesmode.Text == "MES模式")
            {
                Position.Instance.MesMode = true;
                SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                MessageBox.Show("切换MES模式成功，请确保已员工登录并重新复位");
            }
            else
            {
                Position.Instance.MesMode = false;
                SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                MessageBox.Show("切换InerLock模式成功，请重新复位"); }
        }

        public void ClosePower()
        {
            try
            {
                if (PowerserialPort.IsOpen)
                    PowerserialPort.Close();
                //MessageBox.Show("电源端口关闭成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("电源端口关闭异常:" + ex.ToString());
            }
        }
        #endregion
        private void btnCalibModel_Click(object sender, EventArgs e)
        {
            new frmModelBuild(m_GluePlateform.baslerCamera, m_GluePlateform.lightControl, DbModelParam.Instance.CalibGlueVisionParam,
                         "CalibGlueVisionParam", typeof(DbModelParam), () =>
                         {
                             try
                             {
                                 var strUpmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     $"Vision\\{DbModelParam.Instance.CalibGlueVisionParam.strModelPath}");
                                 m_GluePlateform.CalibglueVisionClass.ReadShapeModel(strUpmodelpath);
                             }
                             catch (Exception ex)
                             {
                                 AppendText($"点胶相机加载模板失败！");
                             }
                         }).ShowDialog();
        }

        private void btnCameraSet_Click(object sender, EventArgs e)
        {
            
        }

        private void cbDoor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDoor.Checked) IoPoints.IDO22.Value = true;
            else IoPoints.IDO22.Value = false;
        }

        private void btnIsExitWorking_Click(object sender, EventArgs e)
        {
            if (MachineOperation.Running)
            {
                AppendText("设备暂停时，才能操作！");
                return;
            }
            if (MachineOperation.Alarming) return;
            if ((MachineOperation.Status != MachineStatus.设备准备好) 
                && (MachineOperation.Status != MachineStatus.设备暂停中))return;
            if (Global.IsLocating) return;
        }

        private void btnStopVoice_Click(object sender, EventArgs e)
        {
            Marking.VoiceClosed = !Marking.VoiceClosed;
            //layerLight.VoiceClosed = !layerLight.VoiceClosed;
        }

        #endregion

        private bool ServoAxisIsReady(ApsAxis axis)
        {
            return !axis.IsServon || axis.IsAlarmed || axis.IsEmg || axis.IsMEL || axis.IsPEL;
        }

        /// <summary>
        /// 割胶使用函数，用于移动位置
        /// </summary>
        /// <param name="pos">待移动的位置坐标</param>
        /// <returns></returns>
        private int MoveToPointP(Point3D<double> pos)
        {
            if (!m_GluePlateform.stationInitialize.InitializeDone) return -4;
            //判断Z轴是否在零点
            if (!m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                m_GluePlateform.Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, Global.RZmanualSpeed);
            while (true)
            {
                Thread.Sleep(10);
                if (m_GluePlateform.Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z)) break;
                if (m_GluePlateform.Zaxis.IsAlarmed || m_GluePlateform.Zaxis.IsEmg || !m_GluePlateform.Zaxis.IsServon
                    || m_GluePlateform.Zaxis.IsPositiveLimit || m_GluePlateform.Zaxis.IsNegativeLimit)
                {
                    m_GluePlateform.Zaxis.Stop();
                    return -4;
                }
            }
            //将X、Y移动到指定位置
            if (!m_GluePlateform.Xaxis.IsInPosition(pos.X)) m_GluePlateform.Xaxis.MoveTo(pos.X, Global.RXmanualSpeed);
            if (!m_GluePlateform.Yaxis.IsInPosition(pos.Y)) m_GluePlateform.Yaxis.MoveTo(pos.Y, Global.RYmanualSpeed);
            while (true)
            {
                Thread.Sleep(10);
                if (m_GluePlateform.Xaxis.IsInPosition(pos.X) && m_GluePlateform.Yaxis.IsInPosition(pos.Y)) break;
                if (m_GluePlateform.Xaxis.IsAlarmed || m_GluePlateform.Xaxis.IsEmg || !m_GluePlateform.Xaxis.IsServon
                    || m_GluePlateform.Xaxis.IsPositiveLimit || m_GluePlateform.Xaxis.IsNegativeLimit
                    || m_GluePlateform.Yaxis.IsAlarmed || m_GluePlateform.Yaxis.IsEmg || !m_GluePlateform.Yaxis.IsServon
                    || m_GluePlateform.Yaxis.IsPositiveLimit || m_GluePlateform.Yaxis.IsNegativeLimit)
                {
                    m_GluePlateform.Xaxis.Stop();
                    m_GluePlateform.Yaxis.Stop();
                    return -4;
                }
            }
            //将Z轴移动到指定位置
            m_GluePlateform.Zaxis.MoveTo(pos.Z, Global.RZmanualSpeed);
            while (true)
            {
                Thread.Sleep(10);
                if (m_GluePlateform.Zaxis.IsInPosition(pos.Z)) break;
                if (m_GluePlateform.Zaxis.IsAlarmed || m_GluePlateform.Zaxis.IsEmg || !m_GluePlateform.Zaxis.IsServon
                    || m_GluePlateform.Zaxis.IsPositiveLimit || m_GluePlateform.Zaxis.IsNegativeLimit)
                {
                    m_GluePlateform.Zaxis.Stop();
                    return -4;
                }
            }
            return 0;
        }

        #region 串口通讯
  
        public void CloseDetectHeight()
        {
            try
            {
                heightDetector.DeviceClose();
                //snScanner.DeviceClose();
            }
            catch (Exception e)
            {

            }
        }

        private void RecvFromHeightDector(byte[] readbuffer)
        {
            string strMsg = Encoding.Default.GetString(readbuffer);
            try
            {
                heightDetector.DealRecvData(strMsg, ref Marking.DetectHeight);
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
        }

        

        public void CloseScanner()
        {
            
        }
        public void CloseTcpClient()
        {
            try
            {
                if (FormerStationClient != null)
                {
                    FormerStationClient.ReleaseSocket();
                }
            }
            catch (Exception ex)
            {
                log.Debug("关闭TCP客户端吧异常:" + ex.ToString());
            }
        }
        #endregion
    }
}

