using System;
using System.Toolkit;
using desay.Vision;
using System.Collections.Generic;
using HalconDotNet;
namespace desay.ProductData
{
    [Serializable]
    public class Relationship
    {
        public static Relationship Instance = new Relationship();

        #region 相机参数
        /// <summary>
        /// 相机1mm = pix
        /// </summary>
        public VisCameraCalib CameraCalib;

 
        #endregion
        /// <summary>
        /// 曝光时间
        /// </summary>
        public VisCCDParam LensCameraCCDParam;
        public VisCCDParam ShellCameraCCDParam;

        public VisLightParam LeftDownInhaleLight = new VisLightParam()
        {
            Name = "左下光源",
            IsOpen = true,
            Value = 100
        };
        public VisLightParam RightDownInhaleLight = new VisLightParam()
        {
            Name = "右下光源",
            IsOpen = true,
            Value = 100
        };

        /// <summary>
        /// 相机补偿取反 启用
        /// </summary>
        public bool CameraOffsetXN = false;

        /// <summary>
        /// 相机补偿取反 启用
        /// </summary>
        public bool CameraOffsetYN = false;

        /// <summary>
        /// 流水线复位完成后 电机上升到 顶升位置
        /// </summary>
        public bool TrayMoveGoodUse = false;
    }
    public struct VisCameraCalib
    {
        public uint SerialNum;
        public string Name;
        public ImageMirror Mirror;
        public double RotateAngle;
        public double ResolutionX;
        public double ResolutionY;
    }
    public struct CoaxialityType
    {
        public double X;
        public double Y;
        public double R;
    }
    public struct VisCCDParam
    {
        public double Exposure;
        public double Gain;
    }
    public struct VisLightParam
    {
        public string Name;
        public bool IsOpen;
        public byte Value;
    }
    public struct InhaleParam
    {
        public double Angle;
        public Point<double> Position;
    }
    public struct HeightParam
    {
        public double Z;
        public double Value1;
        public double Value2;
        public double Height;
    }
    public struct ConCenParam
    {
        public double X;
        public double Y;
    }
    [Serializable]
    public class DbModelParam
    {
        public static DbModelParam Instance = new DbModelParam();

        public DbVisSetting GlueLocationVisionParam = new DbVisSetting();//定位参数
        public DbVisSetting GlueFindVisionParam = new DbVisSetting();//胶水识别参数
        public DbVisSetting CalibGlueVisionParam = new DbVisSetting();
    }

    public class DbVisSetting
    {
        public string strID;
        /// <summary>
        /// OK图像是否保存
        /// </summary>
        public bool OkImageSave;
        /// <summary>
        /// NG图像是否保存
        /// </summary>
        public bool NgImageSave;

        /// <summary>
        /// 光源亮度
        /// </summary>
        public int LightControlvalue = 43;

        #region 模板参数
        /// <summary>
        /// 模板是否启用
        /// </summary>
        public bool IsModelUse;
        public bool IsModelDisplay;
        /// <summary>
        /// 模板路径
        /// </summary>
        public string strModelPath;
        /// <summary>
        /// 模板ROI的中心row
        /// </summary>
        public double ModelCenterRow;
        /// <summary>
        /// 模板ROI的中心col
        /// </summary>
        public double ModelCenterCol;
        /// <summary>
        /// 模板ROI的半径1
        /// </summary>
        public double ModelRadius1;
        /// <summary>
        /// 模板ROI的半径2
        /// </summary>
        public double ModelRadius2;
        /// <summary>
        /// 模板ROI最小得分
        /// </summary>
        public ShapeModelType ShapeModel=new ShapeModelType()
        {
            StartAngle = 0,
		    EndAngle = 360,
		    AngleStep = "auto",
		    Optimization = "auto",
		    Metric = "use_polarity",
		    Contrast = "auto",
		    MinContrast = "auto",
		    MinScore = 0.3,
		    NumMatches = 1,
		    MaxOverlap = 0.5,
		    SubPixel = "least_squares_high",
            NumLevels = 4,
            Greediness = 0.9
        };
        #endregion

        #region 找圆参数
        public bool IsCircleDisplay;
        public FitCircleType FindCircle=new FitCircleType()
        {
            CenterRow = 300,
            CenterCol = 300,
            Radius = 100,
            StartAngle = 0,
            EndAngle = 360,
            CalliperLength = 50,
            CalliperWidth = 20,
            CalliperNum = 50,
            TargetNum = 1,
            MinScore = 0.4,
            Sigma = 1,
            Threshold = 15,
            Transition = "all",
            Select = "all"
        };
        #endregion

        #region 测量打点参数
        /// <summary>
        /// 测量灰度是否启用
        /// </summary>
        public bool IsPointUse;
        public bool IsPointDisplay;
        /// <summary>
        /// 测量灰度ROI的中心X
        /// </summary>
        public double PointCenterRow;
        /// <summary>
        /// 测量灰度ROI的中心Y
        /// </summary>
        public double PointCenterCol;
        /// <summary>
        /// 测量灰度ROI的半径
        /// </summary>
        public double PointRadius;
        /// <summary>
        /// 最小灰度公差
        /// </summary>
        public double PointGrayMin;
        public double PointGrayMax;
        #endregion

        #region 测量灰度参数
        /// <summary>
        /// 测量灰度是否启用
        /// </summary>
        public bool IsGraylUse;
        public bool IsGrayDisplay;
        /// <summary>
        /// 测量灰度ROI的中心X
        /// </summary>
        public double GrayCenterRow;
        /// <summary>
        /// 测量灰度ROI的中心Y
        /// </summary>
        public double GrayCenterCol;
        /// <summary>
        /// 测量灰度ROI的半径1
        /// </summary>
        public double GrayRadius1;
        /// <summary>
        /// 测量灰度ROI的半径2
        /// </summary>
        public double GrayRadius2;
        /// <summary>
        /// 最小灰度公差
        /// </summary>
        public double GrayMinTolerance;
        /// <summary>
        /// 最大灰度公差
        /// </summary>
        public double GrayMaxTolerance;
        #endregion

        #region 角度检测
        /// <summary>
        /// 测量极性是否启用
        /// </summary>
        public bool IsPolarlUse;
        public bool IsPolarDisplay;
        /// <summary>
        /// 极性模板ROI_X
        /// </summary>
        public double PolarRow;
        /// <summary>
        /// 极性模板ROI_Y
        /// </summary>
        public double PolarCol;
        /// <summary>
        /// 极性模板ROI内径
        /// </summary>
        public double PolarInnerR;
        /// <summary>
        /// 极性模板ROI外径
        /// </summary>
        public double PolarOuterR;
        public int PolarminGray;
        public int PolarmaxGray;
        public int minRegionLenght;
        public int maxRegionLenght;
        public int minPolarArea;
        public int maxPolarArea;
        #endregion

        #region 胶水识别
        public bool isUseGlue = false;
        /// <summary>
        /// 胶水识别ROI的中心X
        /// </summary>
        public double GlueCenterRow;
        /// <summary>
        /// 胶水识别ROI的中心Y
        /// </summary>
        public double GlueCenterCol;
        /// <summary>
        /// 胶水识别ROI的半径1
        /// </summary>
        public double GlueRadius1;
        /// <summary>
        /// vROI的半径2
        /// </summary>
        public double GlueRadius2;
        #region//胶水判断参数
        //胶宽
        public int GlueWidth = 140;
        //胶圈内圆
        public int GlueInnerCircle = 470;
        //flag胶圈是否合格
        public bool is_qualified = true;
        //胶水外溢判断阈值
        public int glueOverflowOutter = 60;
        //胶水内溢判断阈值
        public  int glueOverflowInner = 60;
        //胶水外圈缺胶判断阈值
        public int glueLackOutter = 60;
        //胶水内圈缺胶判断阈值
        public int glueLackInner = 60;
        //胶圈偏移判断阈值
        public int glueOffset = 80;
        #endregion

        #region//阈值分割参数
        //区域之间的tolerance
        public  double tol = 2.0;
        //region的最小面积阈值
        public  long  area =200000;
        //开运算核大小
        public  double kernel = 150;
        #endregion
        #endregion
    }
    public struct PresVoltage
    {
        public double InVoltage;
        public double OutVoltage;
        public double Pressure;
    }
    /// <summary>
    /// 胶水识别其余高级参数
    /// </summary>
    ///  
    [Serializable]
    public class GlueFindParam
    {
        [NonSerialized]
        public static GlueFindParam Instance = new GlueFindParam();
        public int proj_match_points_ransac_Tolenrance = 80;//
        public double abs_diff_image_value = 2.1;
        public double pow_image_value = 1.0;
        public int median_image_value = 2;
        public int fill_up_shape = 500;
        public double PH_sigama = 2.0;
        public int PH_Threshold = 2500;
        public bool ScaleImg = false;
        public int Gmin = 100;
        public double sub1 = 1.5;
        public int subT = 130;
    }
}
