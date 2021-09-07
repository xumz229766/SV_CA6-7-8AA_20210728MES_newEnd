using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Toolkit;
namespace desay.ProductData
{
    [Serializable]
    public class Position
    {
        public static Position Instance = new Position();

        #region mes
        //机型 505941
        //a2c P01001505941
        //Sn  P01001505941 0999999
        //扫描出来的 sn 50594102 999999
        public string A2C = "900168503941";
        /// <summary>
        /// 是否启用Mes互锁
        /// </summary>
        public bool IsUseMesLock = true;
        public bool IsUseGlueFind = true;
        #endregion


        #region 参数设置
        /// <summary>
        /// 清洗Z轴是否启用缓冲
        /// </summary>
        public bool IsCleanZBuffer;
        /// <summary>
        /// 清洗Z轴缓冲速度
        /// </summary>
        public double CleanZBufferSpeed;
        /// <summary>
        /// 清洗Z轴缓冲距离
        /// </summary>
        public double CleanZBufferDistance;
        /// <summary>
        /// 点胶Z轴是否启用缓冲
        /// </summary>
        public bool IsGlueZBuffer;
        /// <summary>
        /// 点胶Z轴缓冲速度
        /// </summary>
        public double GlueZBufferSpeed;
        /// <summary>
        /// 点胶Z轴缓冲距离
        /// </summary>
        public double GlueZBufferDistance;
        /// <summary>
        /// 开始清洗延时
        /// </summary>
        public double StartCleanDelay;
        /// <summary>
        /// 结束清洗延时
        /// </summary>
        public double StopCleanDelay;
        /// <summary>
        /// 开始点胶延时,即聚胶时间
        /// </summary>
        public double StartGlueDelay;
        /// <summary>
        /// 结束点胶延时,即断胶时间
        /// </summary>
        public double StopGlueDelay;
        /// <summary>
        /// 清洗轨迹速度
        /// </summary>
        public double CleanPathSpeed;
        /// <summary>
        /// 点胶轨迹速度
        /// </summary>
        public double GluePathSpeed;
        /// <summary>
        /// 镜筒清洗次数
        /// </summary>
        public int ConeCleanNum;
        /// <summary>
        /// 镜片清洗次数
        /// </summary>
        public int LensCleanNum;
        /// <summary>
        /// 预AA报警次数
        /// </summary>
        public int PreAlarmNum=5;
        /// <summary>
        /// UV后NG报警时间
        /// </summary>
        public int UVAfterAlarmTime = 2000;
        /// <summary>
        /// 相机拍照延时
        /// </summary>
        public int TriggerCameraDelay = 500;
      
        /// <summary>
        /// 启动空胶角度
        /// </summary>
        public int StartGlueAngle;
        /// <summary>
        /// 拖胶角度
        /// </summary>
        public int DragGlueAngle;
        /// <summary>
        /// 拖胶高度
        /// </summary>
        public double DragGlueHeight;
        /// <summary>
        /// 拖胶延时
        /// </summary>
        public double DragGlueDelay;
        /// <summary>
        /// 拖胶速度
        /// </summary>
        public double DragGlueSpeed;
        /// <summary>
        /// 二次点胶角度
        /// </summary>
        public int SecondGlueAngle;
  
        /// <summary>
        /// 点胶高度
        /// </summary>
        public double GlueHeight;
        /// <summary>
        /// 点胶半径
        /// </summary>
        public double GlueRadius;
        /// <summary>
        /// 点胶基准高度
        /// </summary>
        public double GlueBaseHeight;

        /// <summary>
        /// 胶针从安全距离到点胶基准高度的偏差值
        /// </summary>
        public double DetectHeight2BaseHeight;
        #endregion

        #region plasma清洗平台位置数据
        /// <summary>
        /// 清洗镜筒角度
        /// </summary>
        public int HoldersCleanAngle = 360;
        /// <summary>
        /// 清洗镜头角度
        /// </summary>
        public int LensCleanAngle = 360;
        /// <summary>
        /// 清洗开火焰延时
        /// </summary>
        public int CleanFireTime = 500;
        /// <summary>
        /// 清洗镜头开火焰延时
        /// </summary>
        public int CleanLensFireTime = 500;
        /// <summary>
        /// 清洗安全位置
        /// </summary>
        public Point3D<double> CleanSafePosition;
        /// <summary>
        /// 有无料判断位置
        /// </summary>
        public Point3D<double> LensDetectPosition;
        /// <summary>
        /// 白板测试位置
        /// </summary>
        public Point3D<double> AdjustLightPosition;
        /// <summary>
        /// 视觉标定点胶位置
        /// </summary>
        public Point3D<double> VisionCalibGluePosition;
        /// <summary>
        /// 清洗镜筒轨迹第一点位置
        /// </summary>
        public Point3D<double> CleanConeFirstPosition;
        /// <summary>
        /// 清洗镜筒轨迹第二点位置
        /// </summary>
        public Point3D<double> CleanConeSecondPosition;
        /// <summary>
        /// 清洗镜筒轨迹第三点位置
        /// </summary>
        public Point3D<double> CleanConeThirdPositon;
        /// <summary>
        /// 清洗镜筒轨迹第一点机械位置
        /// </summary>
        public Point<float> CleanConeFirstPositionReal;
        /// <summary>
        /// 清洗镜筒轨迹第二点机械位置
        /// </summary>
        public Point<float> CleanConeSecondPositionReal;
        /// <summary>
        /// 清洗镜筒轨迹第三点机械位置
        /// </summary>
        public Point<float> CleanConeThirdPositonReal;
        /// <summary>
        /// 清洗镜筒轨迹圆心机械位置
        /// </summary>
        public Point<float> CleanConeCenterPositionReal;
        /// <summary>
        /// 清洗镜片轨迹第一点位置
        /// </summary>
        public Point3D<double> CleanLensFirstPosition;
        /// <summary>
        /// 清洗镜片轨迹第二点位置
        /// </summary>
        public Point3D<double> CleanLensSecondPosition;
        /// <summary>
        /// 清洗镜片轨迹第三点位置
        /// </summary>
        public Point3D<double> CleanLensThirdPositon;
        /// <summary>
        /// 清洗镜片轨迹第一点机械位置
        /// </summary>
        public Point<float> CleanLensFirstPositionReal;
        /// <summary>
        /// 清洗镜片轨迹第二点机械位置
        /// </summary>
        public Point<float> CleanLensSecondPositionReal;
        /// <summary>
        /// 清洗镜片轨迹第三点机械位置
        /// </summary>
        public Point<float> CleanLensThirdPositonReal;
        /// <summary>
        /// 清洗镜片轨迹圆心机械位置
        /// </summary>
        public Point<float> CleanLensCenterPositionReal;
        #endregion

        #region 点胶平台位置数据
        /// <summary>
        /// 点胶安全位置
        /// </summary>
        public Point3D<double> GlueSafePosition;
        /// <summary>
        /// 点胶相机标定位置
        /// </summary>
        public Point3D<double> GlueCameraCalibPosition;
        /// <summary>
        /// 点胶定位相机拍照位置
        /// </summary>
        public Point3D<double> GlueCameraPosition;
        /// <summary>
        /// 点胶中心位置
        /// </summary>
        public Point3D<double> GlueCenterPosition;
        /// <summary>
        /// 点胶第一个位置
        /// </summary>
        public Point3D<double> GlueStartPosition;
        /// <summary>
        /// 点胶轨迹第一点机械位置
        /// </summary>
        public Point<float> GlueFirstPositionReal;
        /// <summary>
        /// 点胶轨迹第二点机械位置
        /// </summary>
        public Point<float> GlueSecondPositionReal;
        /// <summary>
        /// 点胶轨迹第三点机械位置
        /// </summary>
        public Point<float> GlueThirdPositonReal;
        /// <summary>
        /// 点胶轨迹圆心机械位置
        /// </summary>
        public Point<float> GlueCenterPositionReal;
        /// <summary>
        /// 点胶对针位置
        /// </summary>
        public Point3D<double> GlueAdjustPinPosition;
        /// <summary>
        /// 胶重点检位置
        /// </summary>
        public Point3D<double> WeightGluePosition;
        /// <summary>
        /// 测胶高位置
        /// </summary>
        public Point3D<double> GlueHeightPosition;
        /// <summary>
        /// 点胶割胶起始位置
        /// </summary>
        public Point3D<double> CutGlueStartPosition;
        /// <summary>
        /// 点胶割胶结束位置
        /// </summary>
        public Point3D<double> CutGlueEndPosition;
        /// <summary>
        /// 点胶识别相机拍照位置
        /// </summary>
        public Point3D<double> GlueCheckCameraPosition;
        #endregion
        /// <summary>
        /// AA 点不良时  点亮次数
        /// </summary>
        public int AALightOnTime = 2;
        /// <summary>
        /// 点胶定位NG次数
        /// </summary>
        public int GlueLoadNGTime = 3;
        /// <summary>
        /// 点胶识别NG次数
        /// </summary>
        public int GlueFindNGTime = 3;
        public double AAZWorkPos;
        /// <summary>
        /// 胶针偏移
        /// </summary>
        public Point3D<double> NeedleOffset;


        /// <summary>
        /// 记录CCD到胶针的偏差值，Z轴暂不用
        /// </summary>
        public Point3D<double> CCD2NeedleOffset;
        /// <summary>
        /// 记录产品圆圈中心到CCD的偏差值，Z轴暂不用
        /// </summary>
        public Point3D<double> PCB2CCDOffset;



        public double MaxNeedleOffsetX=41.0;
        public double MaxNeedleOffsetY=-105.0;
        public double MinNeedleOffsetX=38.0;
        public double MinNeedleOffsetY=-108.0;
        /// <summary>
        /// 自动对针设定偏差
        /// </summary>
        public Point3D<double> NeedleCalibOffset;
        /// <summary>
        /// 自动对针中心位置
        /// </summary>
        public Point3D<double> NeedleCalibCenter;
        #region 通讯相关
     
 

     
    
        #endregion
 
        /// <summary>
        /// 白板电源-电流
        /// </summary>
        public double Current_Wb = 0.3;
        /// <summary>
        /// 白板电源-电压
        /// </summary>
        public double Voltage_Wb = 12;

        /// <summary>
        /// AA电源-电流
        /// </summary>
        public double Current_AA = 0.3;
        /// <summary>
        /// AA电源-电压
        /// </summary>
        public double Voltage_AA = 12;
        //public int cameratime=500;

        public  bool CleanLensShield=false;         // 清洗镜头屏蔽
        public  bool CleanHolderShield = false;         // 清洗镜座屏蔽 
        /// <summary>
        /// SV Mes模式 false 为Interlock  true Mes出入站
        /// </summary>
        public bool MesMode = false;
    }
}
