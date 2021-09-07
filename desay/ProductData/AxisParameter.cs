using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motion.AdlinkAps;
using Motion.Interfaces;
using System.Toolkit;
namespace desay.ProductData
{
    [Serializable]
    public class AxisParameter
    {
        [NonSerialized]
        public static AxisParameter Instance = new AxisParameter();

        //最大速度
        public double LXvelocityMax { get; set; } = 300.000;
        public double LYvelocityMax { get; set; } = 400.000;
        public double LZvelocityMax { get; set; } = 300.000;
        public double RXvelocityMax { get; set; } = 300.000;
        public double RYvelocityMax { get; set; } = 400.000;
        public double RZvelocityMax { get; set; } = 300.000;
        public double AAYvelocityMax { get; set; } = 400.000;
        public double AAZvelocityMax { get; set; } = 400.000;
        //速度参数
        public VelocityCurve LXspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve LYspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve LZspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve CleanPathSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };

        public VelocityCurve RXspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve RYspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve RZspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve GluePathSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };

        public VelocityCurve AAYspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve AAZspeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };

        //回零速度
        public VelocityCurve LXhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve LYhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve LZhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };

        public VelocityCurve RXhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve RYhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve RZhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve AAYhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        public VelocityCurve AAZhomeSpeed { get; set; } = new VelocityCurve()
        { Strvel = 5.00, Maxvel = 20.0, Tacc = 0.1, Tdec = 0.1, VelocityCurveType = CurveTypes.T };
        //传动参数
        public TransmissionParams LXTransParams { get; set; } = new TransmissionParams() { Lead = 12, SubDivisionNum = 12000 };//电缸
        public TransmissionParams LYTransParams { get; set; } = new TransmissionParams() { Lead = 12, SubDivisionNum = 12000 };
        public TransmissionParams LZTransParams { get; set; } = new TransmissionParams() { Lead = 12, SubDivisionNum = 12000 };
        public TransmissionParams RXTransParams { get; set; } = new TransmissionParams() { Lead = 12, SubDivisionNum = 12000 };
        public TransmissionParams RYTransParams { get; set; } = new TransmissionParams() { Lead = 16, SubDivisionNum = 16000 };//点胶16
        public TransmissionParams RZTransParams { get; set; } = new TransmissionParams() { Lead = 12, SubDivisionNum = 12000 };

        public TransmissionParams AAYTransParams { get; set; } = new TransmissionParams() { Lead = 10, SubDivisionNum = 10000 };//丝杠
        public TransmissionParams AAZTransParams { get; set; } = new TransmissionParams() { Lead = 10, SubDivisionNum = 10000 };

        /// <summary>
        /// AA出料感应 之后延时 在堆料送治具到AA  AA出料至固化保持合理 不至于皮带停止而未接收到
        /// </summary>
        public int AAOutJigsDelay = 200;
        /// <summary>
        /// AA 解串板 上电延时
        /// </summary>
        public int AAPowerOpenDelayTime = 100;
        /// <summary>
        /// 点胶 解串板  上电延时
        /// </summary>
        public int GluePowerOpenDelayTime = 100;
        /// <summary>
        /// 清洗到位感应延时
        /// </summary>
        public int CleanPosDelay = 0;
        /// <summary>
        /// 点胶到位感应延时
        /// </summary>
        public int GluePosDelay = 0;

        /// <summary>
        /// 堆料到位感应延时
        /// </summary>
        public int StopilePosDelay = 0;
        /// <summary>
        /// AA到位感应延时
        /// </summary>
        public int AAPosDelay = 0;

        /// <summary>
        /// 屏蔽上工位时 临时治具码
        /// </summary>
        public string JigsSN = "JIG_01";
        /// <summary>
        /// 屏蔽上工位时 临时产品码
        /// </summary>
        public string HolderSN = "1234567890";
        /// <summary>
        /// 允许提前放行
        /// </summary>
        public bool AAallowStockpilePutJigs=true;
        /// <summary>
        /// AA工位报警 是否自动复位
        /// </summary>
        public bool AAalrmAutoReset = true;
        /// <summary>
        /// AA中时 AA堆料位来料后停止流水线
        /// </summary>
        public bool AAingAAstockpStop = true;
        public string MachineName = "SVCA6";
        /// <summary>
        /// 相机拍照前延时
        /// </summary>
        public int CameraDelayTriger = 500;
        /// <summary>
        /// Plasma工作最短时间 s
        /// </summary>
        public double PlasmaWorkingTime = 1.0;
        #region 标定
        public Point<double> CameraAndNeedleOffset;
        /// <summary>
        /// 点胶定位偏差X
        /// </summary>
        public double GlueOffsetX;
        /// <summary>
        /// 点胶定位偏差Y
        /// </summary>
        public double GlueOffsetY;
        #endregion
        public bool IsUseTanZhenCyl = false;
        /// <summary>
        /// PLC  电脑异常时中断出胶和plasma 时间ms
        /// </summary>
        public int PlcReflashTime = 500;
    }
}
