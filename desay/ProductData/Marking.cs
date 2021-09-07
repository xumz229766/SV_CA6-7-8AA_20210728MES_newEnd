using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using desay.Flow;
using System.Diagnostics;
namespace desay.ProductData
{
    public class Marking
    {

        public static double CleanCycleTime;
        public static double GlueCycleTime;
        public static double AACycleTime;
        public static double TotalCycleTime;
        public static string CycleRunTime;

        public static Stopwatch CleanSinglewatch = new Stopwatch();
        public static Stopwatch GlueSinglewatch = new Stopwatch();
        public static Stopwatch Totalwatch = new Stopwatch();

        public static bool PlasmaWorking = false;           //上电
        #region 工作信号
        /// <summary>
        /// Feer工位进出料信号
        /// </summary>  

        public static bool CarrierCallOut;       //请求出料
        public static bool CarrierCallOutFinish; //出料完成
        public static bool CarrierCallIn;        //请求入料
        public static bool CarrierCallInFinish;  //入料完成
        public static bool CarrierWorking;       //接驳台工作中
        public static bool CarrierHaveProduct;
        public static bool CarrierStart;

        public static bool ScannerEnable;
        public static bool SnScannerShield = true;
        public static bool CarrierShield;

        public static bool GetAAresult;
        /// <summary>
        /// 清洗工位工作状态信号
        /// </summary>
        //public static bool CleanCallIn;        // 清洗工位入料请求
        //public static bool CleanCallInFinish;  // 清洗工位入料完成
        //public static bool CleanWorking;       // 清洗工位工作中
        //public static bool CleanHoming;        // 清洗工位复位中
        //public static bool CleanWorkFinish;    // 清洗工作结束
        //public static bool CleanCallOut;       // 清洗工位出料请求
        //public static bool CleanCallOutFinish; // 清洗工位出料完成
        public static bool CleanHaveProduct;   // 清洗工位有料标志
        public static bool HaveLensShield;     // 镜头有无判断屏蔽
        public static bool PlasmaShield;       // Plasma功能屏蔽
        public static bool WhiteShield;        // 白板检测屏蔽
        public static bool WhiteBoardResult;   // 白板检测结果
        public static bool CleanShield;        // 清洗工位屏蔽
        public static bool CleanRun;           // 清洗功能是否打开
        public static bool CleanResult_MesLock;        // 清洗工位结果标志  Mes互锁NG
        public static bool CleanRecycleRun;    // 清洗周期运行
        public static bool CleanStart;         // 清洗开始
        public static bool OnceCleanFinish;    // 1次清洗完成
        public static bool TwiceCleanFinish;   // 2次清洗完成
        public static bool CleanFinish;        // 清洗完成
        public static bool CatchPitureResultOK;// 白板检测结果OK
        public static bool CatchPitureResultNG;// 白板检测结果NG
        public static bool ProductTest;
        public static string WbData;

        public static bool PlasmaOn;           //上电

        /// <summary>
        /// 点胶工位工作状态信号
        /// </summary>
        public static bool GlueCallIn;         // 点胶工位入料请求
        public static bool GlueCallInFinish;   // 点胶工位入料完成
        public static bool GlueWorking;        // 点胶工位工作中
        public static bool GlueHoming;         // 点胶工位复位中
        public static bool GlueWorkFinish;     // 点胶工作结束
        public static bool GlueCallOut;        // 点胶工位出料请求
        public static bool GlueCallOutFinish;  // 点胶工位出料完成
        public static bool GlueHaveProduct;    // 点胶工位有料标志
        public static bool GlueShield;         // 点胶工位屏蔽
        public static bool CCDShield;          // CCD功能屏蔽
        public static bool GlueRun;            // 点胶功能是否打开
        public static bool GlueFinish;         // 点胶完成
        public static bool GlueResult;         // 点胶工位结果标志
        public static bool GlueRecycleRun;     // 点胶周期运行

        public static bool AAShield;         // AA工位屏蔽
    
        public static bool LoadcationVisonResult;
        public static bool GlueFindVisionResult;
        /// <summary>
        /// AA工位工作状态信号
        /// </summary>
        public static bool GHShield;           // 固化工位屏蔽
        public static bool AAResult;           // AA工位结果标志
        public static bool AACallIn;           // AA工位入料请求
        public static bool HaveLensRst;        // 有无料检测结果
        public static bool WhiteBoardRst;      // 白板检测结果
        public static bool GlueCheckRst;       // 点胶检测结果
        public static bool LightCameraRst;     // 点亮结果
        public static bool PreAAPosRst;        // 预AA位置结果
        public static bool SearchPosRst;       // 搜索定位结果
        public static bool OCAdjustRst;        // 中心调整结果
        public static bool TiltAdjustRst;      // 倾斜调整结果
        public static bool UVBeforeRst;        // UV前结果
        public static bool UVAfterRst;         // UV后结果
        public static bool UVAfterAlarm;       // UV后结果报警
        public static string AAData;

        //public static bool PreAAIsUse;        // 预AA启用
        #endregion

        #region 产品相关
        /// <summary>
        /// 产品有无检测标志
        /// </summary>
        public static bool HaveLens;
        #endregion

        #region 通讯相关
        public static bool WbRequestResultFlg;     // 请求白板结果
        public static bool WbClientOpenFlg;        // 白板软件打开
        public static bool WbClientCloseFlg;       // 白板软件关闭
        public static bool WbGetResultFlg;         // 获取到白板结果
        public static bool WbCheckAgainFlg;        // 白板首次未点亮标志位

        public static bool BeginTriggerSN;         // 开始触发扫SN
        public static bool GetSNFlg;               // 获取到SN
        public static bool BeginTriggerFN;         // 开始触发扫FN
        public static bool GetFNFlg;               // 获取到FN

        public static bool CcdRequestResultFlg;    // 请求CCD结果
        public static bool CcdRequestLocationFlg;  // 请求CCD定位坐标
        public static bool CcdGetLocationFlg;      // 获取到CCD定位坐标
        public static bool CcdGetResultFlg;        // 获取到CCD结果
        public static bool CcdGetLocationFailFlg;  // 获取CCD定位坐标失败标志位
        public static bool CcdGetResultFailFlg;    // 获取CCD结果失败标志位
        //public static bool CcdClientOpenFlg;       // CCD软件打开
        //public static bool CcdClientCloseFlg;      // CCD软件关闭

        public static bool AaGetDataFlg;           // 获取到AA数据
        public static bool AaSendCodeFlg;          // 发送码和结果给AA
        public static bool AaClientOpenFlg;        // AA软件打开
        public static bool AaClientCloseFlg;       // AA软件关闭
        public static bool AaGetResultFlg;         // 获取到AA结果
        public static bool AaAllowPassFlg;         // 通知AA放行

        public static bool RequestHeightFlg;       // 请求测高数据
        public static bool GetHeightFlg;           // 获取到测得的高度


        public static bool NeedleLocateTest = false;    //启动CCD对针识别
        public static bool CenterLocateTest = false;    //启动CCD点胶圆中心坐标识别
        public static bool GlueCheckTest = false;       //启动CCD胶水识别
        public static bool GlueCheckNoGlue = false;       //无胶水图获取

        public static bool NeedleLocateTestSucceed = false;    //启动CCD对针识别成功
        public static bool CenterLocateTestSucceed = false;    //启动CCD点胶圆中心坐标识别成功
        public static bool GlueCheckTestSucceed = false;       //启动CCD胶水识别成功

        public static bool NeedleLocateTestFinish = false;    //启动CCD对针识别结束
        public static bool CenterLocateTestFinish = false;    //启动CCD点胶圆中心坐标识别结束
        public static bool GlueCheckTestFinish = false;       //启动CCD胶水识别结束

        public static bool RequestHeightError;





        #endregion
        /// <summary>
        /// 胶水识别
        /// </summary>
        public static Camera GlueOffset;
        /// <summary>
        /// 定位
        /// </summary>
        public static Camera LoadOffset;

        #region 交互信号
        /// <summary>
        /// 清洗工位完成清洗
        /// </summary>
        public static bool CleanWorkAlreadyRequest;
        /// <summary>
        /// 点胶工位准备接收清洗工位治具
        /// </summary>
        public static bool GlueAlreadyGetJigsFromClean;      
        /// <summary>
        /// 点胶工位完成
        /// </summary>
        public static bool GlueWorkAlreadyRequest;
        /// <summary>
        /// AA堆料准备接收点胶工位治具
        /// </summary>
        public static bool AAStockpAlreadyGetJigsFromGlue;
        /// <summary>
        /// AA堆料完成
        /// </summary>
        public static bool AAStockpWorkAlreadyRequest;
        /// <summary>
        /// AA准备接收堆料工位治具
        /// </summary>
        public static bool AAAlreadyGetJigsFromStockp;
        /// <summary>
        /// AA完成
        /// </summary>
        public static bool AAWorkAlreadyRequest;
        
        /// <summary>
        /// AA回流完成
        /// </summary>
        public static bool AABackFlowGetEmptyJigsFromNextStation;
        /// <summary>
        /// Glue回流
        /// </summary>
        public static bool GlueBackFlowGetEmptyJigsFromAABackFlow;
        /// <summary>
        /// 上料料机准备好等待回流
        /// </summary>
        public static bool UpMotionReadyGlueBackFlow;
        /// <summary>
        /// Glue回流 到上料机
        /// </summary>
        public static bool GlueBackFlowFinishToUpMotion;
        /// <summary>
        /// 清洗回流到上料机完成
        /// </summary>
        public static bool BackToUpMotionFinish;
        /// <summary>
        /// Plasma火焰报警屏蔽
        /// </summary>
        public static bool PlasmaAlarmShield = false;
        #endregion
        /// <summary>
        /// 安全门屏蔽
        /// </summary>
        public static bool DoorShield;
        /// <summary>
        /// 安全光幕屏蔽
        /// </summary>
        public static bool CurtainShield;
        /// <summary>
        /// 进料光纤感应失败计数
        /// </summary>
        public static int CarrierFailCount;
        [NonSerialized]
        public static double AutoDoorCloseDelay = 10.0;
        /// <summary>
        /// 蜂鸣器关闭
        /// </summary>
        public static bool VoiceClosed;
        /// <summary>
        /// TCP服务器打开标志位
        /// </summary>
        public static bool TcpServerOpenSuccess;
        /// <summary>
        /// 与上一工位TCP通信标志
        /// </summary>
        public static bool TcpClientOpenSuccess;
        /// <summary>
        /// 测高串口打开标志位
        /// </summary>
        public static bool HeightDetectorOpenSuccess;
        /// <summary>
        /// SN扫码枪串口打开标志位
        /// </summary>
        public static bool snScannerOpenSuccess;
        /// <summary>
        /// FN扫码枪串口打开标志位
        /// </summary>
        public static bool fnScannerOpenSuccess;
        /// <summary>
        /// 接驳台有无料屏蔽
        /// </summary>
        public static bool CarrierHaveProductSheild;
        /// <summary>
        /// 清洗有无料屏蔽
        /// </summary>
        public static bool CleanHaveProductSheild;
        /// <summary>
        /// 点胶有无料屏蔽
        /// </summary>
        public static bool GlueHaveProductSheild;
        /// <summary>
        /// 清洗结束状态
        /// </summary>
        //public static bool CleanFinishBit;
        /// <summary>
        /// 点胶结束状态
        /// </summary>
        public static bool GlueFinishBit;
        /// <summary>
        /// 产品码
        /// </summary>
        public static string SN;
        /// <summary>
        /// 治具码
        /// </summary>
        public static string FN;
        /// <summary>
        /// 接收到前段镜头二维码
        /// </summary>
        public static List<string> LensSN;
        /// <summary>
        /// 接收到的物料批次二维码
        /// </summary>
        public static List<string> BatchSN;
        /// <summary>
        /// 点胶高度补偿
        /// </summary>
        public static double GlueHeightOffset;
        /// <summary>
        /// 测到的高度
        /// </summary>
        public static double DetectHeight = 0.0;
        /// <summary>
        /// 屏蔽上一工位
        /// </summary>
        public static bool FormerStationShield = false;
        /// <summary>
        /// 屏蔽镜头传感器
        /// </summary>
        public static bool LensSensorShield = false;
        /// <summary>
        /// 屏蔽外壳传感器
        /// </summary>
        public static bool ShellSensorShield = false;
        /// <summary>
        /// 胶水液位屏蔽
        /// </summary>
        public static bool GlueHaveShield = true;
        /// <summary>
        /// AA回零屏蔽
        /// </summary>
        public static bool AAhomeShield = false;

        public static bool CleanTCPUpMotion = false;
        #region 回原完成
        public static bool CleanXHomeFinish=false;
        public static bool CleanYHomeFinish = false;
        public static bool CleanZHomeFinish = false;
        public static bool GlueXHomeFinish = false;
        public static bool GlueYHomeFinish = false;
        public static bool GlueZHomeFinish = false;


        #endregion

        public static bool WhiteBoardResult_Detect;   // 
        public static bool WhiteBoardResult_Stain;   // 

        /// <summary>
        /// 白板脏污
        /// </summary>
        public static bool WhiteBoardResult_DetectStain;   //
        /// <summary>
        /// 白板点亮
        /// </summary>
                 
        public static bool WhiteBoardResult_Light;   // 
        public static bool GetNoGlueImg = false;

        public static bool AAfinishAtHome = false;

        /// <summary>
        /// 光源初始化成功标志
        /// </summary>
        public static bool IsLightControl = false;

        public static bool AAUpClyIsMove = false;

        public static bool ShowResultHaveThrans = false;

        public static bool BaclFlowRuning_Glue = false;
        public static bool BaclFlowRuning_AA = false;
        /// <summary>
        /// 新增型号或者 切换型号时 重新加载；
        /// </summary>
        public static bool IsProductThrans = false;
        public static bool PlcReflashShield = false;
        public static double PlcRefashTimes = 0.0;
        public static string SvJobNumber = "";
        public struct Camera
        {
            /// <summary>
            /// 结果
            /// </summary>
            public string Result;
            /// <summary>
            /// X轴偏移值
            /// </summary>
            public double X;
            /// <summary>
            /// Y轴偏移值
            /// </summary>
            public double Y;
            /// <summary>
            /// 角度
            /// </summary>
            public double A;
            /// <summary>
            /// 直径
            /// </summary>
            public double D;
        }
    }
}
