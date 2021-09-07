using System;
using System.Runtime.InteropServices;

namespace Vision.HalconAps
{
    /******************************圆ROI****************************************/
    public struct tagCircleRoi
    {
        public float fCenterX;             //圆心X
        public float fCenterY;             //圆心Y
        public float fRadiusR;             //圆R
    };

    /******************************矩形ROI(正)****************************************/
    public struct tagRectRoi
    {
        public float fRectStartX;          //起点X
        public float fRectStartY;          //起点Y
        public float fRectEndX;            //终点X
        public float fRectEndY;            //终点Y
    };

    /******************************矩形ROI(带角度)****************************************/
    public struct tagRectAngleRoi
    {
        public float fRectCenterX;          //矩形中心X
        public float fRectCenterY;          //矩形中心Y
        public float fRectAngle;            //角度
        public float fLength1;              //长轴半径
        public float fLength2;              //短轴半径(fLength2 <= fLength1)
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

    /******************************形状模板匹配************************************/
    public struct tagShape
    {
        public int nNumLevels;           // 图像金字塔，层数越高，像素越少
        public float fAngleStart;          // 起始角度  
        public float fAngleExtent;         // 角度范围  
        public float fAngleStep;           // 角度步长  
        public string szOptimization;       // 优化算法，none不减少像素，point_reduction_low大约一半点，point_reduction_medium 大约1/3，point_reduction_high大约1/4
        public string szMetric;             // 极性，如果图像有光线的变化，需要调整这个参数，但由于匹配的
                                           // 极性模式: use_polarity 生成的模板就一个目标
                                           // 极性模式 : ignore_global_polarity 生成的模板有两个目标，一个是原图另一个是灰度值取反，黑的变白，白的变黑
                                           // 极性模式 : ignore_local_polarity 生成的模板有三个目标，除上面两个还有一个是灰度值渐变
        public float fContrast;            // 对比度  (越大边缘越少，越小边缘越多)
        public float fMinContrast;         // 最小对比度  
        public float fMinScore;            // 最小匹配分值
        public int nNumMatches;          // 匹配个数
        public float fMaxOverlap;          // 重叠度
        public float fSubPixel;            // 精度控制
        public float fGreediness;          // 贪婪度
    };

    /******************************返回形状模板匹配**************************************/
    public struct tagRetShape
    {
        public float fOutRow;              // 输出 X
        public float fOutColumn;           // 输出 Y
        public float fOutAngle;            // 输出角度
        public float fOutScore;            // 输出分数
    };

    /******************************自相关模板匹配****************************************/
    public struct tagNcc
    {
        public int nNumLevels;            // 图像金字塔，层数越高，像素越少
        public float fAngleStart;           // 起始角度  
        public float fAngleExtent;          // 角度范围  
        public float fAngleStep;            // 角度步长  
        public string szMetric;              // 极性，如果图像有光线的变化，需要调整这个参数，但由于匹配的
                                            // 极性模式: use_polarity 生成的模板就一个目标
                                            // 极性模式 : ignore_global_polarity 生成的模板有两个目标，一个是原图另一个是灰度值取反，黑的变白，白的变黑
                                            // 极性模式 : ignore_local_polarity 生成的模板有三个目标，除上面两个还有一个是灰度值渐变 
        public float fMinScore;             // 最小匹配分值
        public int nNumMatches;           // 匹配个数
        public float fMaxOverlap;           // 重叠度
        public float fSubPixel;             // 精度控制
    };

    /******************************返回自相关模板匹配****************************************/
    public struct tagRetNcc
    {
        public float fOutRow;               // 输出 X
        public float fOutColumn;            // 输出 Y
        public float fOutAngle;             // 输出角度
        public float fOutScore;             // 输出分数
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
        public float fOutRow;               // 输出 X
        public float fOutColumn;            // 输出 Y
    };

    /******************************似合线****************************************/
    public struct tagFitLine
    {
        public float fLineStartX;           //线起点X坐标
        public float fLineStartY;           //线起点Y坐标
        public float fLineEndX;             //线终点X坐标
        public float fLineEndY;             //线终点Y坐标
        public float fCalliperLength;       //测量卡尺半长
        public float fCalliperWidth;        //测量卡尺半款
        public int nCalliperNum;            //测量卡尺数
        public int nTargetNum;            //目标个数
        public float fMinScore;             //最小分数
        public float fSigma;                //高斯Sigma,0.4-100
        public int nThreshold;          //最小边缘强度,1-255
        public string szTransition;          //边缘极性,0:'all', 1:'positive', 2:'negative' 
        public string szSelect;              //选点方式,0:'all', 1:'first', 2:'last' 
    };

    /******************************返回似合线****************************************/
    public struct tagRetFitLine
    {
        public float fOutRowBegin;         // 输出线起点 X
        public float fOutColBegin;         // 输出线起点 Y
        public float fOutRowEnd;           // 输出线起点 X
        public float fOutColEnd;           // 输出线起点 Y
    };

    /******************************似合圆****************************************/
    public struct tagFitCircle
    {
        public float fCircleCenterX;        //圆心X坐标
        public float fCircleCenterY;        //圆心Y坐标
        public float fCircleRadiusR;        //圆半径
        public float fCircleStartAngle;     //圆起始角度
        public float fCircleEndAngle;       //圆终止角度
        public float fCalliperLength;       //测量卡尺半长
        public float fCalliperWidth;        //测量卡尺半款
        public int nCalliperNum;            //测量卡尺数
        public int nTargetNum;              //目标个数
        public float fMinScore;             //最小分数
        public float fSigma;                //高斯Sigma,0.4-100
        public int nThreshold;              //最小边缘强度,1-255
        public string szTransition;         //边缘极性,0:'all', 1:'positive', 2:'negative' 
        public string szSelect;             //选点方式,0:'all', 1:'first', 2:'last' 
    };
    /******************************返回似合圆****************************************/
    public struct tagRetFitCircle
    {
        public float fOutRow;               // 输出圆心 X
        public float fOutCol;               // 输出圆心 Y
        public float fOutR;                 // 输出圆半径 R
    };
    public class HalconDefine
    {
        /// <summary>
        /// New对象
        /// </summary>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_NewObiect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_NewObiect();                                                                                       
        /// <summary>
        /// 设置圆形ROI
        /// </summary>
        /// <param name="tCircleRoi"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "HV_SetCircleRegions", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool HV_SetCircleRegions(ref tagCircleRoi tCircleRoi);                                                         
        /// <summary>
        /// 设置环形ROI
        /// </summary>
        /// <param name="tMaxCircle"></param>
        /// <param name="tMinCircle"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "HV_SetAnnularRegions", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool HV_SetAnnularRegions(ref tagCircleRoi tMaxCircle, ref tagCircleRoi tMinCircle);                              
        /// <summary>
        /// 设置矩形(正)ROI
        /// </summary>
        /// <param name="tRectRoi"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "HV_SetRectRegions", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool HV_SetRectRegions(ref tagRectRoi tRectRoi);                                                          
        /// <summary>
        /// 设置矩形(带角度)ROI
        /// </summary>
        /// <param name="tRectAngleRoi"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "HV_SetRectAngleRegions", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool HV_SetRectAngleRegions(ref tagRectAngleRoi tRectAngleRoi);                                              
        /// <summary>
        /// 图像增强
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="fMult"></param>
        /// <param name="nAdd"></param>
        /// <param name="pOutImage"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_ScaleImage", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_ScaleImage(ref tagByteImage tByteImage, float fMult, int nAdd, out byte pOutImage);                   
        /// <summary>
        /// 阈值分割
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tThreshold"></param>
        /// <param name="pOutImage"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_Threshold", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_Threshold(ref tagByteImage tByteImage, ref tagThreshold tThreshold, out byte pOutImage);                
        /// <summary>
        /// 创建形状模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tShape"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_CreateShapeTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_CreateShapeTemplate(ref tagByteImage tByteImage, ref tagShape tShape);                           
        /// <summary>
        /// 查找形状模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tShape"></param>
        /// <param name="tRetShape"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_FindShapeTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_FindShapeTemplate(ref tagByteImage tByteImage, ref tagShape tShape, ref tagRetShape tRetShape);          
        /// <summary>
        /// 读形状模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_ReadShapeModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_ReadShapeModelID(string szPath);                                                       
        /// <summary>
        /// 写形状模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_WriteShapeModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_WriteShapeModelID(string szPath);                                                           
        /// <summary>
        /// 创建自相关模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tNcc"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_CreateNccTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_CreateNccTemplate(ref tagByteImage tByteImage, ref tagNcc tNcc);                            
        /// <summary>
        /// 查找自相关模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tNcc"></param>
        /// <param name="tRetNcc"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_FindNccTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_FindNccTemplate(ref tagByteImage tByteImage, ref tagNcc tNcc, ref tagRetNcc tRetNcc);              
        /// <summary>
        /// 读自相关模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_ReadNccModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_ReadNccModelID(string szPath);                                                    
        /// <summary>
        /// 写自相关模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_WriteNccModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_WriteNccModelID(string szPath);                                                    
        /// <summary>
        /// 创建灰度模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tGray"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_CreateGrayTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_CreateGrayTemplate(ref tagByteImage tByteImage, ref tagGray tGray);                           
        /// <summary>
        /// 查找灰度模板
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tGray"></param>
        /// <param name="tRetGray"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_FindGrayTemplate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_FindGrayTemplate(ref tagByteImage tByteImage, ref tagGray tGray, ref tagRetGray tRetGray);      
        /// <summary>
        /// 读灰度模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_ReadGrayModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_ReadGrayModelID(string szPath);                                                               
        /// <summary>
        /// 写灰度模板ID
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_WriteGrayModelID", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_WriteGrayModelID(string szPath);                                                             
        /// <summary>
        /// Blob分析(分析ROI平均灰度)
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="nMinGray"></param>
        /// <param name="nMaxGray"></param>
        /// <param name="pMeanGray"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_Blob", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_Blob(ref tagByteImage tByteImage, int nMinGray, int nMaxGray, ref int pMeanGray);          
        /// <summary>
        /// 拟合线
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tFitLineParam"></param>
        /// <param name="tRetFitLine"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_FitLine", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_FitLine(ref tagByteImage tByteImage, ref tagFitLine tFitLineParam, ref tagRetFitLine tRetFitLine);    
        /// <summary>
        /// 拟合圆
        /// </summary>
        /// <param name="tByteImage"></param>
        /// <param name="tFitCircleParam"></param>
        /// <param name="tRetFitCircle"></param>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_FitCircle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_FitCircle(ref tagByteImage tByteImage, ref tagFitCircle tFitCircleParam, ref tagRetFitCircle tRetFitCircle);
        /// <summary>
        /// Delete对象
        /// </summary>
        /// <returns></returns>
        [DllImport("ImageOperatorDll.dll", EntryPoint = "VO_DeleteObiect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool VO_DeleteObiect();                                                                                     
    }
}
