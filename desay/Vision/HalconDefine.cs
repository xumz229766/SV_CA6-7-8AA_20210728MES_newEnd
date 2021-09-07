using System;
using System.Runtime.InteropServices;
using HalconDotNet;
using log4net;
namespace desay.Vision
{
    /******************************圆ROI****************************************/
    public struct tagCircleRoi
    {
        public double fCenterRow;             //圆心Row
        public double fCenterCol;             //圆心Col
        public double fRadiusR;             //圆R
    };

    /******************************矩形ROI(正)****************************************/
    public struct tagRectRoi
    {
        public double fRectStartRow;          //起点Row
        public double fRectStartCol;          //起点Col
        public double fRectEndRow;            //终点Row
        public double fRectEndCol;            //终点Col
    };

    /******************************矩形ROI(带角度)****************************************/
    public struct tagRectAngleRoi
    {
        public double fRectCenterRow;          //矩形中心Row
        public double fRectCenterCol;          //矩形中心Col
        public double fRectAngle;            //角度
        public double fLength1;              //长轴半径
        public double fLength2;              //短轴半径(fLength2 <= fLength1)
    };

    /******************************输入图片****************************************/
    public struct tagByteImage
    {
        public byte[] pInImage;             //输入图片
        public int nWidth;                 //宽     
        public int nHeight;                //高
        public int nChannels;               //通道数
    };

    /******************************阈值分割****************************************/
    public struct tagThreshold
    {
        public int nMinGray;              //最小阈值
        public int nMaxGray;              //最大阈值
        public bool bAuto;                 //直方图调节(0--true,1--false)
        public int nTarGray;              //期望值
    };

    /// <summary>
    /// 形状模板匹配
    /// </summary>
    public struct ShapeModelType
    {
        public int NumLevels;               // 图像金字塔，层数越高，像素越少
        public double StartAngle;            // 起始角度  
        public double EndAngle;           // 角度范围  
        public string AngleStep;             // 角度步长  
        public string Optimization;         // 优化算法，none不减少像素，point_reduction_low大约一半点，point_reduction_medium 大约1/3，point_reduction_high大约1/4
        public string Metric;               // 极性，如果图像有光线的变化，需要调整这个参数，但由于匹配的
                                            // 极性模式: use_polarity 生成的模板就一个目标
                                            // 极性模式 : ignore_global_polarity 生成的模板有两个目标，一个是原图另一个是灰度值取反，黑的变白，白的变黑
                                            // 极性模式 : ignore_local_polarity 生成的模板有三个目标，除上面两个还有一个是灰度值渐变
        public string Contrast;              // 对比度  (越大边缘越少，越小边缘越多)
        public string MinContrast;           // 最小对比度  
        public double MinScore;              // 最小匹配分值
        public int NumMatches;              // 匹配个数
        public double MaxOverlap;            // 重叠度
        public string SubPixel;              // 精度控制
        public double Greediness;            // 贪婪度
    };

    /// <summary>
    /// 返回形状模板匹配
    /// </summary>
    public struct RetShapeModel
    {
        public double CenterRow;              // 输出Row
        public double CenterCol;           // 输出Col
        public double Angle;            // 输出角度
        public double Score;            // 输出分数
    };

    /******************************自相关模板匹配****************************************/
    public struct tagNcc
    {
        public int nNumLevels;            // 图像金字塔，层数越高，像素越少
        public double fAngleStart;           // 起始角度  
        public double fAngleExtent;          // 角度范围  
        public double fAngleStep;            // 角度步长  
        public string szMetric;              // 极性，如果图像有光线的变化，需要调整这个参数，但由于匹配的
                                            // 极性模式: use_polarity 生成的模板就一个目标
                                            // 极性模式 : ignore_global_polarity 生成的模板有两个目标，一个是原图另一个是灰度值取反，黑的变白，白的变黑
                                            // 极性模式 : ignore_local_polarity 生成的模板有三个目标，除上面两个还有一个是灰度值渐变 
        public double fMinScore;             // 最小匹配分值
        public int nNumMatches;           // 匹配个数
        public double fMaxOverlap;           // 重叠度
        public double fSubPixel;             // 精度控制
    };

    /******************************返回自相关模板匹配****************************************/
    public struct tagRetNcc
    {
        public double fOutRow;               // 输出Row
        public double fOutColumn;            // 输出Col
        public double fOutAngle;             // 输出角度
        public double fOutScore;             // 输出分数
    };

    /******************************灰度模板匹配****************************************/
    public struct tagGray
    {
        public int nNumLevels;             // 图像金字塔，层数越高，像素越少
        public string szOptimize;             // 优化
        public string szGrayValues;           // 灰度值
        public int nMaxError;
        public string szSubPixel;             // 亚像素
    };

    /******************************返回灰度模板匹配****************************************/
    public struct tagRetGray
    {
        public double fOutRow;               // 输出Row
        public double fOutColumn;            // 输出Col
    };

    /// <summary>
    /// 似合线
    /// </summary>
    public struct FitLineType
    {
        public double StartRow;                //线起点Row坐标
        public double StartCol;                //线起点Col坐标
        public double EndRow;                  //线终点Row坐标
        public double EndCol;                  //线终点Col坐标
        public double CalliperLength;        //测量卡尺半长
        public double CalliperWidth;         //测量卡尺半款
        public int CalliperNum;             //测量卡尺数
        public int TargetNum;               //目标个数
        public double MinScore;              //最小分数
        public double Sigma;                 //高斯Sigma,0.4-100
        public int Threshold;               //最小边缘强度,1-255
        public string Transition;           //边缘极性,0:'all', 1:'positive', 2:'negative' 
        public string Select;               //选点方式,0:'all', 1:'first', 2:'last' 
                
    }

    /******************************返回似合线****************************************/
    public struct tagRetFitLine
    {
        public double fOutRowBegin;         // 输出线起点Row
        public double fOutColBegin;         // 输出线起点Col
        public double fOutRowEnd;           // 输出线终点Row
        public double fOutColEnd;           // 输出线终点Col
    };

    /******************************似合圆****************************************/
    /// <summary>
    /// 似合圆数据结构
    /// </summary>
    public struct FitCircleType
    {
        public double CenterRow;               //圆心Row坐标
        public double CenterCol;               //圆心Col坐标
        public double Radius;                //圆半径
        public double StartAngle;            //圆起始角度
        public double EndAngle;              //圆终止角度
        public double CalliperLength;        //测量卡尺半长
        public double CalliperWidth;         //测量卡尺半宽
        public int CalliperNum;             //测量卡尺数
        public int TargetNum;               //目标个数
        public double MinScore;              //最小分数
        public double Sigma;                 //高斯Sigma,0.4-100
        public int Threshold;               //最小边缘强度,1-255
        public string Transition;           //边缘极性,0:'all', 1:'positive', 2:'negative' 
        public string Select;               //选点方式,0:'all', 1:'first', 2:'last' 
    };
    /******************************返回似合圆****************************************/
    public struct RetFitCircle
    {
        public double CenterRow;               //圆心X坐标
        public double CenterCol;               //圆心Y坐标
        public double Radius;                //圆半径
    };
    public class HalconControl
    {
       static ILog log = LogManager.GetLogger(typeof(HalconControl));
        static readonly object obj = new object();
        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="sModelPath"></param>
        /// <returns></returns>
        public static bool ReadShapeModel(string Name,string sModelPath,out HTuple ModelId)
        {
            lock(obj)
            {
                ModelId = null;
                if (sModelPath == "") return false;
                try
                {
                    HOperatorSet.ReadShapeModel(sModelPath, out ModelId);
                    log.Debug($"读取{Name}找圆模板成功");
                    return true;
                }
                catch (Exception) { log.Error($"读取{Name}找圆模板失败"); return false; }
            }
        }
    }
}
