using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;                        //INI引用
using System.Runtime.InteropServices;   //INI引用
using System.Windows.Forms;
using HalconDotNet;
using System.Xml;
using System.Management;//需要手动添加引用
using System.Xml.Serialization;
namespace Vision.HalconAps
{
    public class HalconController
    {
        public bool bLog = true;
        /// <summary>
        /// 绘制Rake区域
        /// </summary>
        /// <param name="ho_Regions">rake区域</param>
        /// <param name="hv_WindowHandle">窗口句柄</param>
        /// <param name="hv_Elements">卡尺工具数量</param>
        /// <param name="hv_DetectHeight">单个卡尺工具的高度</param>
        /// <param name="hv_DetectWidth">单个卡尺工具的宽度</param>
        /// <param name="hv_Row1">直线起点Y</param>
        /// <param name="hv_Column1">直线起点X</param>
        /// <param name="hv_Row2">直线终点Y</param>
        /// <param name="hv_Column2">直线终点X</param>
        public void draw_rake(out HObject ho_Regions, HTuple hv_WindowHandle, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_Row1, out HTuple hv_Column1,
            out HTuple hv_Row2, out HTuple hv_Column2)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_ATan = null, hv_Deg1 = null, hv_Deg = null;
            HTuple hv_i = null, hv_RowC = new HTuple(), hv_ColC = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            try
            {
                disp_message(hv_WindowHandle, "点击鼠标左键画一条直线,点击右键确认", "window",
                    12, 12, "red", "false");
                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    out hv_Column2);
                //disp_line (WindowHandle, Row1, Column1, Row2, Column2)
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_Row1, hv_Column1, hv_Row2,
                    hv_Column2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_RegionLines, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                HOperatorSet.TupleAtan2((-hv_Row2) + hv_Row1, hv_Column2 - hv_Column1, out hv_ATan);
                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);

                hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);


                HTuple end_val14 = hv_Elements;
                HTuple step_val14 = 1;
                for (hv_i = 1; hv_i.Continue(end_val14, step_val14); hv_i = hv_i.TupleAdd(step_val14))
                {
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * hv_i) / (hv_Elements + 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * hv_i) / (hv_Elements + 1));

                    if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                    {
                        HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_Distance / 2);
                    }
                    else
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_DetectWidth / 2);
                    }

                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                    {
                        hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                            ho_Regions.Dispose();
                            ho_Regions = ExpTmpOutVar_0;
                        }
                    }
                }

                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// 绘制spoke（找圆）区域
        /// </summary>
        /// <param name="ho_Image">图像对象</param>
        /// <param name="ho_Regions">找圆区域</param>
        /// <param name="hv_WindowHandle">窗口句柄</param>
        /// <param name="hv_Elements">找圆卡尺工具个数</param>
        /// <param name="hv_DetectHeight">单个卡尺工具的高度</param>
        /// <param name="hv_DetectWidth">单个卡尺工具的宽度</param>
        /// <param name="hv_ROIRows">卡尺工具Y坐标集合</param>
        /// <param name="hv_ROICols">卡尺工具X坐标集合</param>
        /// <param name="hv_Direct">指示卡尺工具查找方向</param>
        public void draw_spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_WindowHandle,
            HTuple hv_Elements, HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_ROIRows,
            out HTuple hv_ROICols, out HTuple hv_Direct)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ContOut1, ho_Contour, ho_ContCircle;
            HObject ho_Cross, ho_Rectangle1 = null, ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Rows = null, hv_Cols = null, hv_Weights = null;
            HTuple hv_Length1 = null, hv_RowC = null, hv_ColumnC = null;
            HTuple hv_Radius = null, hv_StartPhi = null, hv_EndPhi = null;
            HTuple hv_PointOrder = null, hv_RowXLD = null, hv_ColXLD = null;
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_DistanceStart = null, hv_DistanceEnd = null;
            HTuple hv_Length = null, hv_Length2 = null, hv_i = null;
            HTuple hv_j = new HTuple(), hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ContOut1);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            hv_ROIRows = new HTuple();
            hv_ROICols = new HTuple();
            hv_Direct = new HTuple();
            try
            {
                disp_message(hv_WindowHandle, "1、画4个以上点确定一个圆弧,点击右键确认", "window",
                    12, 12, "red", "false");
                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                ho_ContOut1.Dispose();
                HOperatorSet.DrawNurbs(out ho_ContOut1, hv_WindowHandle, "true", "true", "true",
                    "true", 3, out hv_Rows, out hv_Cols, out hv_Weights);
                HOperatorSet.TupleLength(hv_Weights, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(4))) != 0)
                {
                    disp_message(hv_WindowHandle, "提示：点数太少，请重画", "window", 32, 12,
                        "red", "false");
                    hv_ROIRows = new HTuple();
                    hv_ROICols = new HTuple();
                    ho_ContOut1.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }


                hv_ROIRows = hv_Rows.Clone();
                hv_ROICols = hv_Cols.Clone();

                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);

                HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3, 2, out hv_RowC,
                    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                ho_ContCircle.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_ContCircle, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ContCircle, HDevWindowStack.GetActive());
                }
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowC, hv_ColumnC, 60, 0.785398);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Cross, HDevWindowStack.GetActive());
                }
                disp_message(hv_WindowHandle, "2、远离圆心，画箭头确定边缘检测方向，点击右键确认",
                    "window", 12, 12, "red", "false");
                if (HDevWindowStack.IsOpen())
                {
                    //dev_set_color ('red')
                }
                HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    out hv_Column2);
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row1, hv_Column1, out hv_DistanceStart);
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row2, hv_Column2, out hv_DistanceEnd);
                HOperatorSet.LengthXld(ho_ContCircle, out hv_Length);
                HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
                if ((int)(new HTuple(hv_Elements.TupleLess(1))) != 0)
                {
                    hv_ROIRows = new HTuple();
                    hv_ROICols = new HTuple();
                    ho_ContOut1.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }
                HTuple end_val37 = hv_Elements - 1;
                HTuple step_val37 = 1;
                for (hv_i = 0; hv_i.Continue(end_val37, step_val37); hv_i = hv_i.TupleAdd(step_val37))
                {
                    if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                        hv_Length2 - 1)))) != 0)
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    }
                    else
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);

                    }
                    if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                    {
                        hv_j = hv_Length2 - 1;
                        //continue
                    }

                    hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                    hv_ColE = hv_ColXLD.TupleSelect(hv_j);
                    if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                        hv_Direct = "inner";
                    }
                    else
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_Direct = "outer";
                    }


                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                        hv_DetectHeight / 2, hv_DetectWidth / 2);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                    {
                        hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                            ho_Regions.Dispose();
                            ho_Regions = ExpTmpOutVar_0;
                        }
                    }
                }

                ho_ContOut1.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ContOut1.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }
        /// <summary>
        /// 拟合最合适的圆
        /// </summary>
        /// <param name="ho_Circle">拟合结果（圆或圆弧）</param>
        /// <param name="hv_Rows">拟合圆的输入y坐标</param>
        /// <param name="hv_Cols">拟合圆的输入x坐标</param>
        /// <param name="hv_ActiveNum">最小有效点数</param>
        /// <param name="hv_ArcType">拟合圆弧类型：'arc'圆弧；'circle'圆</param>
        /// <param name="hv_RowCenter">拟合结果：圆心y坐标</param>
        /// <param name="hv_ColCenter">拟合结果：圆心x坐标</param>
        /// <param name="hv_Radius">拟合结果：圆半径</param>
        public void pts_to_best_circle(out HObject ho_Circle, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ActiveNum, HTuple hv_ArcType, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
            out HTuple hv_Radius)
        {



            // Local iconic variables 

            HObject ho_Contour = null;

            // Local control variables 

            HTuple hv_Length = null, hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_Length1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            try
            {
                hv_RowCenter = 0;
                hv_ColCenter = 0;
                hv_Radius = 0;

                ho_Circle.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Circle);
                HOperatorSet.TupleLength(hv_Cols, out hv_Length);

                if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(
                    new HTuple(hv_ActiveNum.TupleGreater(2)))) != 0)
                {

                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                    HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2,
                        out hv_RowCenter, out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi,
                        out hv_PointOrder);

                    HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
                    if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                    {
                        ho_Contour.Dispose();

                        return;
                    }
                    if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
                    {
                        ho_Circle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                            hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                    }
                    else
                    {
                        ho_Circle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                            hv_Radius, 0, (new HTuple(360)).TupleRad(), hv_PointOrder, 1);
                    }
                }

                ho_Contour.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();

                throw HDevExpDefaultException;
            }
        }
        /// <summary>
        /// 拟合最合适的直线
        /// </summary>
        /// <param name="ho_Line">输出拟合直线的xld</param>
        /// <param name="hv_Rows">拟合直线的输入y数组</param>
        /// <param name="hv_Cols">拟合直线的输入x数组</param>
        /// <param name="hv_ActiveNum">最小有效点数</param>
        /// <param name="hv_Row1">拟合结果：直线起点y</param>
        /// <param name="hv_Col1">拟合结果：直线起点x</param>
        /// <param name="hv_Row2">拟合结果：直线终点y</param>
        /// <param name="hv_Col2">拟合结果：直线终点x</param>
        public void pts_to_best_line(out HObject ho_Line, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ActiveNum, out HTuple hv_Row1, out HTuple hv_Col1, out HTuple hv_Row2,
            out HTuple hv_Col2)
        {



            // Local iconic variables 

            HObject ho_Contour = null;

            // Local control variables 

            HTuple hv_Length = null, hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Length1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            try
            {
                hv_Row1 = 0;
                hv_Col1 = 0;
                hv_Row2 = 0;
                hv_Col2 = 0;
                ho_Line.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Line);
                HOperatorSet.TupleLength(hv_Cols, out hv_Length);

                if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(
                    new HTuple(hv_ActiveNum.TupleGreater(1)))) != 0)
                {

                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                    HOperatorSet.FitLineContourXld(ho_Contour, "tukey", hv_ActiveNum, 0, 5, 2,
                        out hv_Row1, out hv_Col1, out hv_Row2, out hv_Col2, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    HOperatorSet.TupleLength(hv_Dist, out hv_Length1);
                    if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                    {
                        ho_Contour.Dispose();

                        return;
                    }
                    ho_Line.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Line, hv_Row1.TupleConcat(hv_Row2),
                        hv_Col1.TupleConcat(hv_Col2));

                }

                ho_Contour.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();

                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// rake检测工具
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_Regions">输出边缘点检测区域及方向</param>
        /// <param name="hv_Elements">检测边缘点数</param>
        /// <param name="hv_DetectHeight">卡尺工具的高度</param>
        /// <param name="hv_DetectWidth">卡尺工具的宽度</param>
        /// <param name="hv_Sigma">高斯滤波因子</param>
        /// <param name="hv_Threshold">边缘检测阈值，又叫边缘强度</param>
        /// <param name="hv_Transition">极性：positive表示由黑到白，negative表示由白到黑，all表示以上两种方向</param>
        /// <param name="hv_Select">first表示选择第一点，last表示选择最后一点，max表示选择边缘强度最强点</param>
        /// <param name="hv_Row1">检测区域起点的y值</param>
        /// <param name="hv_Column1">检测区域起点的x值</param>
        /// <param name="hv_Row2">检测区域终点的y值</param>
        /// <param name="hv_Column2">检测区域终点的y值</param>
        /// <param name="hv_ResultRow">检测到的边缘点的y坐标数组</param>
        /// <param name="hv_ResultColumn">检测到的边缘点的x坐标数组</param>
        public void rake(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2,
            HTuple hv_Column2, out HTuple hv_ResultRow, out HTuple hv_ResultColumn)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Rectangle = null, ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_Deg1 = null, hv_Deg = null, hv_i = null, hv_RowC = new HTuple();
            HTuple hv_ColC = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple();
            HTuple hv_ColL2 = new HTuple(), hv_ColL1 = new HTuple();
            HTuple hv_MsrHandle_Measure = new HTuple(), hv_RowEdge = new HTuple();
            HTuple hv_ColEdge = new HTuple(), hv_Amplitude = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
            HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_j = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                hv_ResultRow = new HTuple();
                hv_ResultColumn = new HTuple();
                HOperatorSet.TupleAtan2((-hv_Row2) + hv_Row1, hv_Column2 - hv_Column1, out hv_ATan);
                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);

                hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);


                HTuple end_val13 = hv_Elements;
                HTuple step_val13 = 1;
                for (hv_i = 1; hv_i.Continue(end_val13, step_val13); hv_i = hv_i.TupleAdd(step_val13))
                {
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * hv_i) / (hv_Elements + 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * hv_i) / (hv_Elements + 1));
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                    {
                        HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_Distance / 2);
                    }
                    else
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_DetectWidth / 2);
                    }

                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                    {
                        hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                            ho_Regions.Dispose();
                            ho_Regions = ExpTmpOutVar_0;
                        }
                    }
                    HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_Deg.TupleRad(), hv_DetectHeight / 2,
                        hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);


                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                    {
                        hv_Transition_COPY_INP_TMP = "negative";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                        {

                            hv_Transition_COPY_INP_TMP = "positive";
                        }
                        else
                        {
                            hv_Transition_COPY_INP_TMP = "all";
                        }
                    }

                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                    {
                        hv_Select_COPY_INP_TMP = "first";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                        {

                            hv_Select_COPY_INP_TMP = "last";
                        }
                        else
                        {
                            hv_Select_COPY_INP_TMP = "all";
                        }
                    }

                    HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                        hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                        out hv_Amplitude, out hv_Distance);
                    HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                    hv_tRow = 0;
                    hv_tCol = 0;
                    hv_t = 0;
                    HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                    {
                        continue;
                    }
                    HTuple end_val69 = hv_Number - 1;
                    HTuple step_val69 = 1;
                    for (hv_j = 0; hv_j.Continue(end_val69, step_val69); hv_j = hv_j.TupleAdd(step_val69))
                    {
                        if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                            hv_t))) != 0)
                        {

                            hv_tRow = hv_RowEdge.TupleSelect(hv_j);
                            hv_tCol = hv_ColEdge.TupleSelect(hv_j);
                            hv_t = ((hv_Amplitude.TupleSelect(hv_j))).TupleAbs();
                        }
                    }
                    if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                    {

                        hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                        hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                    }
                }
                HOperatorSet.TupleLength(hv_ResultRow, out hv_Number);


                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }
        /// <summary>
        /// 找圆spoke
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_Regions">输出边缘点检测区域及方向</param>
        /// <param name="hv_Elements">检测边缘点数</param>
        /// <param name="hv_DetectHeight">卡尺工具的高度</param>
        /// <param name="hv_DetectWidth">卡尺工具的宽度</param>
        /// <param name="hv_Sigma">高斯滤波因子</param>
        /// <param name="hv_Threshold">边缘检测阈值，又叫边缘强度</param>
        /// <param name="hv_Transition">极性：positive表示由黑到白，negative表示由白到黑，all表示以上两种方向</param>
        /// <param name="hv_Select">first表示选择第一点，last表示选择最后一点，max表示选择边缘强度最强点</param>
        /// <param name="hv_ROIRows">检测区域起点的y值</param>
        /// <param name="hv_ROICols">检测区域起点的x值</param>
        /// <param name="hv_Direct">'inner'表示检测方向由边缘点指向圆心;'outer'表示检测方向由圆心指向边缘点</param>
        /// <param name="hv_ResultRow">检测到的边缘点的y坐标数组</param>
        /// <param name="hv_ResultColumn">检测到的边缘点的x坐标数组</param>
        /// <param name="hv_ArcType">拟合圆弧类型：'arc'圆弧；'circle'圆</param>
        public void spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_ROIRows, HTuple hv_ROICols,
            HTuple hv_Direct, out HTuple hv_ResultRow, out HTuple hv_ResultColumn, out HTuple hv_ArcType)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_RowC = null;
            HTuple hv_ColumnC = null, hv_Radius = null, hv_StartPhi = null;
            HTuple hv_EndPhi = null, hv_PointOrder = null, hv_RowXLD = null;
            HTuple hv_ColXLD = null, hv_Length = null, hv_Length2 = null;
            HTuple hv_i = null, hv_j = new HTuple(), hv_RowE = new HTuple();
            HTuple hv_ColE = new HTuple(), hv_ATan = new HTuple();
            HTuple hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple();
            HTuple hv_ColL2 = new HTuple(), hv_ColL1 = new HTuple();
            HTuple hv_MsrHandle_Measure = new HTuple(), hv_RowEdge = new HTuple();
            HTuple hv_ColEdge = new HTuple(), hv_Amplitude = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_tRow = new HTuple();
            HTuple hv_tCol = new HTuple(), hv_t = new HTuple(), hv_Number = new HTuple();
            HTuple hv_k = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            hv_ArcType = new HTuple();
            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                hv_ResultRow = new HTuple();
                hv_ResultColumn = new HTuple();


                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);

                HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3, 2, out hv_RowC,
                    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                ho_ContCircle.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
                HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

                HOperatorSet.LengthXld(ho_ContCircle, out hv_Length);
                HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
                if ((int)(new HTuple(hv_Elements.TupleLess(1))) != 0)
                {

                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }
                HTuple end_val19 = hv_Elements - 1;
                HTuple step_val19 = 1;
                for (hv_i = 0; hv_i.Continue(end_val19, step_val19); hv_i = hv_i.TupleAdd(step_val19))
                {
                    if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                        hv_Length2 - 1)))) != 0)
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                        hv_ArcType = "circle";
                    }
                    else
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                        hv_ArcType = "arc";
                    }
                    if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                    {
                        hv_j = hv_Length2 - 1;
                        //continue
                    }
                    hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                    hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                    //超出图像区域，不检测，否则容易报异常
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;

                    }
                    else
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);

                    }


                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                        hv_DetectHeight / 2, hv_DetectWidth / 2);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                    {
                        hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                            ho_Regions.Dispose();
                            ho_Regions = ExpTmpOutVar_0;
                        }
                    }

                    HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, hv_DetectHeight / 2,
                        hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);


                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                    {
                        hv_Transition_COPY_INP_TMP = "negative";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                        {

                            hv_Transition_COPY_INP_TMP = "positive";
                        }
                        else
                        {
                            hv_Transition_COPY_INP_TMP = "all";
                        }
                    }

                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                    {
                        hv_Select_COPY_INP_TMP = "first";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                        {

                            hv_Select_COPY_INP_TMP = "last";
                        }
                        else
                        {
                            hv_Select_COPY_INP_TMP = "all";
                        }
                    }

                    HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                        hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                        out hv_Amplitude, out hv_Distance);
                    HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                    hv_tRow = 0;
                    hv_tCol = 0;
                    hv_t = 0;
                    HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                    {
                        continue;
                    }
                    HTuple end_val93 = hv_Number - 1;
                    HTuple step_val93 = 1;
                    for (hv_k = 0; hv_k.Continue(end_val93, step_val93); hv_k = hv_k.TupleAdd(step_val93))
                    {
                        if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                            hv_t))) != 0)
                        {

                            hv_tRow = hv_RowEdge.TupleSelect(hv_k);
                            hv_tCol = hv_ColEdge.TupleSelect(hv_k);
                            hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
                        }
                    }
                    if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                    {

                        hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                        hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                    }
                }


                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }
        // Chapter: Graphics / Text
        // Short Description: This procedure writes a text message. 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
            HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
            HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow (same as if no second value is given)
            //       otherwise -> use given string as color string for the shadow color
            //
            //Prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //Transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //Display text box depending on text size
            hv_UseShadow = 1;
            hv_ShadowColor = "gray";
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
            {
                if (hv_Box_COPY_INP_TMP == null)
                    hv_Box_COPY_INP_TMP = new HTuple();
                hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                hv_ShadowColor = "#f28d26";
            }
            if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                {
                    //Use default ShadowColor set above
                }
                else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                    "false"))) != 0)
                {
                    hv_UseShadow = 0;
                }
                else
                {
                    hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                    //Valid color?
                    try
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                            1));
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                        throw new HalconException(hv_Exception);
                    }
                }
            }
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
            {
                //Valid color?
                try
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                //Calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //Display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                //Set shadow color
                HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
                if ((int)(hv_UseShadow) != 0)
                {
                    HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
                }
                //Set box color
                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //Reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }

        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = null, hv_ZeroLengthIndices = null;
            HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
            HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
            HTuple hv_ColP2 = null, hv_Index = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            try
            {
                //This procedure generates arrow shaped XLD contours,
                //pointing from (Row1, Column1) to (Row2, Column2).
                //If starting and end point are identical, a contour consisting
                //of a single point is returned.
                //
                //input parameteres:
                //Row1, Column1: Coordinates of the arrows' starting points
                //Row2, Column2: Coordinates of the arrows' end points
                //HeadLength, HeadWidth: Size of the arrow heads in pixels
                //
                //output parameter:
                //Arrow: The resulting XLD contour
                //
                //The input tuples Row1, Column1, Row2, and Column2 have to be of
                //the same length.
                //HeadLength and HeadWidth either have to be of the same length as
                //Row1, Column1, Row2, and Column2 or have to be a single element.
                //If one of the above restrictions is violated, an error will occur.
                //
                //
                //Init
                ho_Arrow.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Arrow);
                //
                //Calculate the arrow length
                HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
                //
                //Mark arrows with identical start and end point
                //(set Length to -1 to avoid division-by-zero exception)
                hv_ZeroLengthIndices = hv_Length.TupleFind(0);
                if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
                {
                    if (hv_Length == null)
                        hv_Length = new HTuple();
                    hv_Length[hv_ZeroLengthIndices] = -1;
                }
                //
                //Calculate auxiliary variables.
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
                //
                //Calculate end points of the arrow head.
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
                //
                //Finally create output XLD contour for each input point pair
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                    {
                        //Create_ single points for arrows with identical start and end point
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                            hv_Index), hv_Column1.TupleSelect(hv_Index));
                    }
                    else
                    {
                        //Create arrow contour
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                        ho_Arrow.Dispose();
                        ho_Arrow = ExpTmpOutVar_0;
                    }
                }
                ho_TempArrow.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TempArrow.Dispose();

                throw HDevExpDefaultException;
            }
        }
        /// <summary>
        /// 几何圆
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hv_WindowHandle">窗体句柄</param>
        /// <param name="hv_Row">工具行坐标</param>
        /// <param name="hv_Column">工具列坐标</param>
        /// <param name="hv_Radius">工具半径</param>
        /// <param name="hv_ToolLength">工具长度</param>
        /// <param name="hv_ToolWidth">工具宽度</param>
        /// <param name="hv_Threshold">找圆阀值</param>
        /// <param name="hv_Select">找圆边缘点</param>
        /// <param name="hv_Director">找圆方向</param>
        /// <param name="ho_RowCheck">输出找圆行坐标</param>
        /// <param name="ho_ColumnCheck">输出找圆列坐标</param>
        /// <param name="ho_Radius">输出找圆半径</param>
        /// <returns></returns>
        public int FindMetrologyCircle(HObject ho_Image, HTuple hv_WindowHandle,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Radius,HTuple hv_ToolLength, 
            HTuple hv_ToolWidth, HTuple hv_Threshold, HTuple hv_Select, HTuple hv_Director,
            out double ho_RowCheck, out double ho_ColumnCheck, out double ho_Radius)
        {
            ho_RowCheck = 0;
            ho_ColumnCheck = 0;
            ho_Radius = 0;
            if (hv_Row == 0 || hv_Column == 0 || hv_Radius == 0 || hv_ToolLength == 0 || hv_ToolWidth == 0)
                return -1;
            HObject ho_Contours, ho_Contour, ho_Cross;
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HTuple hv_Width, hv_Height, hv_Row1, hv_Column1;
            HTuple hv_MetrologyHandle, hv_MetrologyCircleIndices, hv_CircleRadiusTolerance, hv_CircleParameter;
            HTuple row, col, radius, startPhi, endPhi, pointOrder;
            HTuple hv_num;
            //限定值  限定 圆形ROI 与所找圆半径差值
            hv_CircleRadiusTolerance = 20;
            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle, hv_Row, hv_Column,
                    hv_Radius, hv_ToolLength, hv_ToolWidth, 1.5, hv_Threshold, "distance_threshold", 1,
                    out hv_MetrologyCircleIndices);
                if (hv_Select == 0)
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_transition", "negative");
                else if (hv_Select == 1)
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_transition", "positive");
                else if (hv_Select == 2)
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_transition", "uniform");
                if (hv_Director == 0)
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "instances_outside_measure_regions", "false");
                else if (hv_Director == 1)
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "instances_outside_measure_regions", "true");

                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "num_instances", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "min_score", 0.7);
                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_MetrologyCircleIndices, "all", "result_type", "all_param", out hv_CircleParameter);
                ho_Contours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contours, hv_MetrologyHandle, "all", "all", 1.5);
                ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", "all", out hv_Row1, out hv_Column1);


                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 6, 0.785398);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.ClearWindow(hv_WindowHandle);

                HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                HOperatorSet.SetLineWidth(hv_WindowHandle, 1);
                HOperatorSet.SetColor(hv_WindowHandle, "blue");
                HOperatorSet.DispObj(ho_Contour, hv_WindowHandle);
                HOperatorSet.SetColor(hv_WindowHandle, "red");
                HOperatorSet.DispObj(ho_Cross, hv_WindowHandle);
                HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.DispObj(ho_Contours, hv_WindowHandle);
                HOperatorSet.FitCircleContourXld(ho_Contours, "algebraic", -1, 0, 0, 3, 2, out row, out col, out radius, out startPhi, out endPhi, out pointOrder);
                hv_num = row.TupleLength();
                int num = hv_num;
                int i = 0;
                //?
                for (i = 0; i < hv_num; i++)
                {
                    ho_RowCheck = row[i];
                    ho_ColumnCheck = col[i];
                    ho_Radius = radius[i];
                    ho_Contours.Dispose();
                    ho_Contour.Dispose();
                    ho_Cross.Dispose();
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ho_Contours.Dispose();
                ho_Contour.Dispose();
                ho_Cross.Dispose();
                return -2;
            }
        }

        /// <summary>
        /// 产生NC 模板  以某个半径为基础 及保存
        /// </summary>
        /// <param name="hv_Image">输入图像</param>
        /// <param name="hv_WindowHandle">窗体句柄</param>
        /// <param name="InnerR">内圆半径</param>
        /// <param name="OuterR">外圆半径</param>
        /// <param name="hv_ModelID">模板ID</param>
        /// <returns></returns>
        public int CreateRCModel(HObject hv_Image, HTuple hv_WindowHandle, double InnerR, double OuterR, out HTuple hv_ModelID)
        {

            HObject m_hImage, ho_ModelRegion, ho_Circle, ho_Circle1, ho_ImageResult, ho_ModelContours;
            HOperatorSet.GenEmptyObj(out m_hImage);
            HOperatorSet.GenEmptyObj(out ho_ModelRegion);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HObject ho_ModelAtNewPosition;
            HOperatorSet.GenEmptyObj(out ho_ModelAtNewPosition);
            hv_ModelID = 0;
            try
            {
                HTuple m_hWidth, m_hHeight;
                HOperatorSet.GetImageSize(hv_Image, out m_hWidth, out m_hHeight);
                if (OuterR == 0)
                {

                    return -1;
                }

                m_hImage.Dispose();
                HOperatorSet.GenImage1(out m_hImage, "byte", m_hWidth, m_hHeight, (0));
                //HOperatorSet.ClearWindow(hv_WindowHandle);
                ho_Circle.Dispose();
                HOperatorSet.GenCircle(out ho_Circle, m_hHeight / 2, m_hWidth / 2, InnerR);
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, m_hHeight / 2, m_hWidth / 2, OuterR);
                ho_ModelRegion.Dispose();
                HOperatorSet.Difference(ho_Circle1, ho_Circle, out ho_ModelRegion);
                ho_ImageResult.Dispose();
                HOperatorSet.PaintRegion(ho_ModelRegion, m_hImage, out ho_ImageResult, 255, "fill");
                HOperatorSet.DispObj(ho_ImageResult, hv_WindowHandle);

                HOperatorSet.CreateShapeModel(ho_ImageResult, "auto", 0, 0, 0.0175, "pregeneration", "ignore_local_polarity",
                    "auto", "auto", out hv_ModelID);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                HTuple hv_MovementOfObject;


                HOperatorSet.VectorAngleToRigid(0, 0, 0, (m_hHeight / 2), (m_hWidth / 2), 0, out hv_MovementOfObject);
                ho_ModelAtNewPosition.Dispose();
                HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ModelAtNewPosition, hv_MovementOfObject);
                HOperatorSet.DispObj(ho_ModelAtNewPosition, hv_WindowHandle);
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                m_hImage.Dispose();
                ho_ModelRegion.Dispose();
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_ImageResult.Dispose();
                ho_ModelContours.Dispose();
                ho_ModelAtNewPosition.Dispose();
            }
        }
        /// <summary>
        /// 寻找RC模板
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ModelID"></param>
        /// <param name="WindowHandle"></param>
        /// <param name="ho_Row"></param>
        /// <param name="ho_Column"></param>
        /// <param name="ho_Angle"></param>
        /// <param name="hv_Score"></param>
        /// <returns></returns>
        public int FindRCModel(HObject Image, HTuple ModelID, HTuple WindowHandle, out HTuple ho_Row, 
            out HTuple ho_Column, out HTuple ho_Angle, out HTuple ho_Score)
        {
            ho_Column = 0;
            ho_Row = 0;
            ho_Score = 0;
            ho_Angle = 0;
            HTuple hv_Mean1, hv_Deviation1, hv_width, hv_heigh;
            HObject ho_ShapeModel;
            HOperatorSet.GenEmptyObj(out ho_ShapeModel);
            HObject ho_ModelAtNewPosition;
            HOperatorSet.GenEmptyObj(out ho_ModelAtNewPosition);

            try
            {
                HOperatorSet.GetImageSize(Image, out hv_width, out hv_heigh);
                HOperatorSet.Intensity(Image, Image, out hv_Mean1, out hv_Deviation1);
                double dgray = hv_Mean1;
                if (dgray < 30 || dgray >= 240)
                    return -1;

                // 		FindShapeModel(ho_SearchImg, ModelID, -(HTuple(RCData.nSearchAngle).TupleRad()), HTuple(RCData.nSearchAngle).TupleRad(),
                // 			0.7, 1, 0, "least_squares", 1, 0, &Result.hv_RowCheck, &Result.hv_ColumnCheck, &Result.hv_AngleCheck, &Result.hv_Score);
                HOperatorSet.FindShapeModel(Image, ModelID, 0, 0, 0.7, 1, 0, "least_squares", 3, 1, 
                    out ho_Row, out ho_Column, out ho_Angle, out ho_Score);
                HOperatorSet.DispObj(Image, WindowHandle);
                if (ho_Score.Length == 0)
                {
                    ho_Row = 0;
                    ho_Column = 0;
                    ho_Angle = 0;
                    ho_Score = 0;
                    return -1;
                }
                ho_ShapeModel.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ShapeModel, ModelID, 1);
                double dA = ho_Angle;
                HTuple hv_MovementOfObject;

                HOperatorSet.SetColor(WindowHandle, "red");
                HOperatorSet.VectorAngleToRigid(0, 0, 0, ho_Row, ho_Column, ho_Angle, out hv_MovementOfObject);
                ho_ModelAtNewPosition.Dispose();
                HOperatorSet.AffineTransContourXld(ho_ShapeModel, out ho_ModelAtNewPosition, hv_MovementOfObject);

                HOperatorSet.SetDraw(WindowHandle, "margin");
                HOperatorSet.SetLineWidth(WindowHandle, 1);
                HOperatorSet.DispObj(ho_ModelAtNewPosition, WindowHandle);

                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                ho_ShapeModel.Dispose();
                ho_ModelAtNewPosition.Dispose();
            }

        }

        /// <summary>
        /// 灰度值
        /// </summary>
        /// <param name="WindowHandle">窗体句柄</param>
        /// <param name="hv_Image">输入图像</param>
        /// <param name="row">row</param>
        /// <param name="col">column</param>
        /// <param name="R1">内圆半径</param>
        /// <param name="R2">外圆半径</param>
        /// <param name="RegionGray">输出灰度数据</param>
        /// <param name="Deviation">灰度公差</param>
        /// <returns></returns>
        public int VisRegionGray(HObject hv_Image, HTuple WindowHandle,  double row, double col, double R1, double R2, 
            out double RegionGray, out double Deviation)
        {
            RegionGray = 0;
            Deviation = 0;
            HTuple hv_mean, hv_deviation;
            // 	HObject ho_MaskRegion, ho_rect, ho_rect1;
            HObject ho_Circle, ho_Circle1, ho_MaskRegion, ho_ProcessImg;
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_MaskRegion);
            HOperatorSet.GenEmptyObj(out ho_ProcessImg);
            try
            {
                HOperatorSet.SetColor(WindowHandle, "red");
                HOperatorSet.GenCircle(out ho_Circle, row, col, R1);
                HOperatorSet.GenCircle(out ho_Circle1, row, col, R2);
                HOperatorSet.Difference(ho_Circle1, ho_Circle, out ho_MaskRegion);

                HOperatorSet.Intensity(ho_MaskRegion, hv_Image, out hv_mean, out hv_deviation);
                RegionGray = hv_mean;
                Deviation = hv_deviation;
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_MaskRegion.Dispose();
                ho_ProcessImg.Dispose();
            }
        }
        /// <summary>
        /// 极性图像
        /// </summary>
        /// <param name="WindowHandle">窗体句柄</param>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="row">中心x</param>
        /// <param name="col">中心y</param>
        /// <param name="R1">内圆半径</param>
        /// <param name="R2">外圆半径</param>
        /// <param name="ho_OutputImage">输出极性图像</param>
        /// <returns></returns>
        public int PolarImage(HObject ho_Image, HTuple WindowHandle, double row, double col, 
            double R1, double R2, out HObject ho_OutputImage)
        {
            HTuple hv_RowFittedCircle, hv_ColumnFittedCircle;
            hv_RowFittedCircle = row;
            hv_ColumnFittedCircle = col;
            HObject ho_polarImg;
            HOperatorSet.GenEmptyObj(out ho_polarImg);

            ho_OutputImage = null;
            if (R2 == 0 || R2 <= R1)
            {
                //if (bLog) log.Error("极性转换半径错误");
                return -1;
            }
            try
            {
                HTuple hWindth, hHeight;
                HOperatorSet.GetImageSize(ho_Image, out hWindth, out hHeight);
                HOperatorSet.PolarTransImageExt(ho_Image, out ho_polarImg, hv_RowFittedCircle, hv_ColumnFittedCircle, 0, 6.2831853, R1, R2,
                    hWindth, R2 - R1, "bilinear");

                HOperatorSet.CopyImage(ho_polarImg, out ho_OutputImage);
                HOperatorSet.DispObj(ho_polarImg, WindowHandle);

                ho_polarImg.Dispose();
            }
            catch (Exception ex)
            {

                ho_polarImg.Dispose();
                //if (bLog) log.Error("极性转换错误");
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 创建极性模板
        /// </summary>
        /// <param name="hv_WindowHandle">窗体句柄</param>
        /// <param name="ho_Input">输入图像</param>
        /// <param name="row1">row1</param>
        /// <param name="col1">column1</param>
        /// <param name="row2">row2</param>
        /// <param name="col2">column2</param>
        /// <param name="hv_PolarModelId">极性模板ID</param>
        /// <returns></returns>
        public int CreatePolarModel(HTuple hv_WindowHandle, HObject ho_Input, double row1, double col1,
            double row2, double col2, out HTuple hv_PolarModelId)
        {
            hv_PolarModelId = 0;
            if (row1 == 0 || row2 == 0 || col1 == 0 || col2 == 0)
                return -1;
            HObject ho_AsymEdgesRegion, ho_PolarModelImage, ho_PolarModelContours;
            HOperatorSet.GenEmptyObj(out ho_AsymEdgesRegion);
            HOperatorSet.GenEmptyObj(out ho_PolarModelImage);
            HOperatorSet.GenEmptyObj(out ho_PolarModelContours);
            ho_AsymEdgesRegion.Dispose();
            ho_PolarModelContours.Dispose();
            ho_PolarModelImage.Dispose();

            HTuple hv_ContrastPolar, hv_MinContrast;
            hv_ContrastPolar = "auto";
            hv_MinContrast = 20;

            try
            {
                HOperatorSet.GenRectangle1(out ho_AsymEdgesRegion, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(ho_Input, ho_AsymEdgesRegion, out ho_PolarModelImage);
                HOperatorSet.CreateShapeModel(ho_PolarModelImage, 3, 0, 0, "auto", "auto", "use_polarity", hv_ContrastPolar,
                        hv_MinContrast, out hv_PolarModelId);
                HOperatorSet.GetShapeModelContours(out ho_PolarModelContours, hv_PolarModelId, 1);
                HOperatorSet.DispObj(ho_PolarModelContours, hv_WindowHandle);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                ho_AsymEdgesRegion.Dispose();
                ho_PolarModelContours.Dispose();
                ho_PolarModelImage.Dispose();
            }
        }
        /// <summary>
        /// 寻找极性角度
        /// </summary>
        /// <param name="WindowHandle">窗体句柄</param>
        /// <param name="ho_PolarImg">极性图像</param>
        /// <param name="PolarModelID">极性模板Id</param>
        /// <param name="row">中心x</param>
        /// <param name="col">中心y</param>
        /// <param name="R1">内圆半径</param>
        /// <param name="R2">外圆半径</param>
        /// <param name="ho_RowCheck">返回行数据</param>
        /// <param name="ho_ColumnCheck">返回列数据</param>
        /// <param name="ho_AngleCheck">返回角度数据</param>
        /// <param name="ho_Score">返回得分数据</param>
        /// <returns></returns>
        public int FindPolarAngle(HObject ho_PolarImg, HTuple WindowHandle,  HTuple PolarModelID, 
            double row, double col, double R1, double R2, out HTuple ho_RowCheck
            , out HTuple ho_ColumnCheck, out HTuple ho_AngleCheck, out HTuple ho_Score)
        {
            //Match_Result PolarResult;
            HTuple hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, hv_Score;
            HObject ho_PolarModelContours;
            HOperatorSet.GenEmptyObj(out ho_PolarModelContours);

            HTuple hWidth, hHeight;
            ho_ColumnCheck = 0;
            ho_RowCheck = 0;
            ho_Score = 0;
            ho_AngleCheck = 0;

            try
            {
                HOperatorSet.GetImageSize(ho_PolarImg, out hWidth, out hHeight);
                ho_PolarModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_PolarModelContours, PolarModelID, 1);
                //Determine reference position
                HOperatorSet.FindShapeModel(ho_PolarImg, PolarModelID, 0, 0, 0.4, 1,
                0.0, "least_squares", 2, 1, out hv_RowCheck, out hv_ColumnCheck,
                out hv_AngleCheck, out hv_Score);
                int n = (hv_Score.TupleLength());
                if (n <= 0)
                {
                    ho_PolarModelContours.Dispose();
                    return -1;
                }
                HObject ho_RefinedPos;
                HOperatorSet.GenEmptyObj(out ho_RefinedPos);
                ho_RefinedPos.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_RefinedPos, row, col, 50, 0);

                HTuple hv_MovementOfObject;
                HObject ho_ModelAtNewPosition;
                HOperatorSet.GenEmptyObj(out ho_ModelAtNewPosition);

                HOperatorSet.SetColor(WindowHandle, "red");
                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject);
                ho_ModelAtNewPosition.Dispose();
                HOperatorSet.AffineTransContourXld(ho_PolarModelContours, out ho_ModelAtNewPosition, hv_MovementOfObject);

                ho_Score = hv_Score;
                hv_AngleCheck = (hv_ColumnCheck / hWidth) * Math.PI * 2;
                double dA = hv_AngleCheck;
                hv_ColumnCheck = col + (hv_RowCheck + R1) * Math.Cos(dA);
                hv_RowCheck = row - (hv_RowCheck + R1) * Math.Sin(dA);
                //	DispObj(ho_SearchImage, WindowHandle);
                HOperatorSet.DispObj(ho_RefinedPos, WindowHandle);
                HOperatorSet.DispArrow(WindowHandle, row, col, hv_RowCheck, hv_ColumnCheck, 5);

                ho_RefinedPos.Dispose();
                ho_ModelAtNewPosition.Dispose();
            }
            catch (Exception)
            {
                ho_PolarModelContours.Dispose();
            }
            return 1;
        }
        /// <summary>
        /// 读入图片 本地盘
        /// </summary>
        /// <param name="WindowHandle">窗体句柄</param>
        /// <param name="filePath">图像路径</param>
        /// <param name="ho_Image">输出图像</param>
        /// <returns></returns>
        public bool ReadImg(HTuple WindowHandle, string filePath,out HObject ho_Image)
        {
            ho_Image = null;
            if (filePath == "")
            {
                //if (bLog) log.Error("未初始化控件或图像路径为空");
                return false;
            }
            try
            {
                HTuple hv_Width, hv_Height;
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, filePath);
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                HOperatorSet.SetPart(WindowHandle,(HTuple)0, (HTuple)0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.DispObj(ho_Image, WindowHandle);
                return true;
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return false;
            }

        }
        /// <summary>
        /// 保存图片 直接保存
        /// </summary>
        /// <param name="hv_Image">输入图像</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool SaveImg(HObject hv_Image,string filePath)
        {
            try
            {
                HOperatorSet.WriteImage(hv_Image, ("bmp"), (0), (filePath));
            }
            catch (Exception ex)
            {
                //if (bLog) log.Error("保存图像时图像为空");
                return false;
            }
            return true;
        }
        /// <summary>
        ///  判定图像 正反 灰度值
        /// </summary>
        /// <param name="dCentryRow">灰度ROI_X</param>
        /// <param name="dCentryCol">灰度ROI_Y</param>
        /// <param name="dInerR">灰度ROI内径</param>
        /// <param name="dOutR">灰度ROI外径</param>
        /// <param name="dGray">灰度设定值</param> 
        /// <param name="dTolerance">误差</param> 
        /// <param name="dRegionGray">输出灰度值</param> 
        /// <returns></returns>
        public bool JudgeDirectorImg(HObject ho_Image, HTuple hv_WindowHandle, double dCentryRow, double dCentryCol,
            double dInerR, double dOutR, double dGray, double dTolerance, out double dRegionGray)
        {
            dRegionGray = 0;
            dRegionGray = 0;
            HTuple hv_mean, hv_deviation;
            HObject ho_Circle, ho_Circle1, ho_MaskRegion;
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_MaskRegion);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.GenCircle(out ho_Circle, dCentryRow, dCentryCol, dOutR);
                HOperatorSet.GenCircle(out ho_Circle1, dCentryRow, dCentryCol, dInerR);
                HOperatorSet.Difference(ho_Circle, ho_Circle1, out ho_MaskRegion);
                HOperatorSet.Intensity(ho_MaskRegion, ho_Image, out hv_mean, out hv_deviation);
                dRegionGray = hv_mean;
                //if (dRegionGray >= dGray - dTolerance && dRegionGray < dGray + dTolerance) return true;
                //else return false;
                if (dRegionGray >= dGray - dTolerance && dRegionGray < 255) return true;
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_MaskRegion.Dispose();
            }
        }
        /// <summary>
        /// 极性图像
        /// </summary>
        /// <param name="WindowHandle">窗体句柄</param>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="row">中心x</param>
        /// <param name="col">中心y</param>
        /// <param name="R1">内圆半径</param>
        /// <param name="R2">外圆半径</param>
        /// <param name="ho_OutputImage">输出极性图像</param>
        /// <returns></returns>
        public int PolarImage(HObject ho_InputImage, double row, double col, double R1, double R2, 
            out HObject ho_OutputImage0, out HObject ho_OutputImage1)
        {
            HTuple hv_RowFittedCircle, hv_ColumnFittedCircle;
            hv_RowFittedCircle = row;
            hv_ColumnFittedCircle = col;
            HObject ho_polarImg0, ho_polarImg1;
            HOperatorSet.GenEmptyObj(out ho_polarImg0);
            HOperatorSet.GenEmptyObj(out ho_polarImg1);
            ho_OutputImage0 = null;
            ho_OutputImage1 = null;
            if (R2 == 0 || R2 <= R1)
            {
                //if (bLog) log.Error("极性转换半径错误");
                return -1;
            }
            try
            {
                HTuple hWindth, hHeight;
                HOperatorSet.GetImageSize(ho_InputImage, out hWindth, out hHeight);
                HOperatorSet.PolarTransImageExt(ho_InputImage, out ho_polarImg0, hv_RowFittedCircle, hv_ColumnFittedCircle, 0.0, 2 * Math.PI, R1, R2,
                    hWindth, R2 - R1, "bilinear");
                HOperatorSet.CopyImage(ho_polarImg0, out ho_OutputImage0);

                HOperatorSet.PolarTransImageExt(ho_InputImage, out ho_polarImg1, hv_RowFittedCircle, hv_ColumnFittedCircle, -Math.PI, Math.PI, R1, R2,
                    hWindth, R2 - R1, "bilinear");
                HOperatorSet.CopyImage(ho_polarImg1, out ho_OutputImage1);
                //6.2831853
                ho_polarImg0.Dispose();
                ho_polarImg1.Dispose();
            }
            catch (Exception ex)
            {

                ho_polarImg0.Dispose();
                ho_polarImg1.Dispose();
               // if (bLog) log.Error("极性转换错误");
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 寻找极性角度
        /// </summary>
        /// <param name="ho_PolarImg0">极性图像0</param>
        /// <param name="ho_PolarImg1">极性图像1</param>
        /// <param name="PolarModelID">极性模板Id</param>
        /// <param name="row">中心x</param>
        /// <param name="col">中心y</param>
        /// <param name="R1">内圆半径</param>
        /// <param name="R2">外圆半径</param>
        /// <param name="ho_RowCheck">返回行数据</param>
        /// <param name="ho_ColumnCheck">返回列数据</param>
        /// <param name="ho_AngleCheck">返回角度数据</param>
        /// <param name="ho_Score">返回得分数据</param>
        /// <returns></returns>
        public int FindPolarAngle(HTuple hv_WindowHandle, HObject ho_PolarImg0, HObject ho_PolarImg1, HTuple PolarModelID,
            double row, double col, double R1, double R2, out HTuple ho_RowCheck, out HTuple ho_ColumnCheck, 
            out HTuple ho_AngleCheck, out HTuple ho_Score)
        {
            HTuple hv_ColumnCheck, hv_RowCheck, hv_AngleCheck, hv_Score;
            HObject ho_PolarModelContours;
            HOperatorSet.GenEmptyObj(out ho_PolarModelContours);

            HTuple hWidth, hHeight;
            ho_ColumnCheck = 0;
            ho_RowCheck = 0;
            ho_Score = 0;
            ho_AngleCheck = 0;
            HObject ho_RefinedPos;
            HOperatorSet.GenEmptyObj(out ho_RefinedPos);
            HObject ho_ModelAtNewPosition;
            HOperatorSet.GenEmptyObj(out ho_ModelAtNewPosition);
            bool IsPi = false;
            try
            {
                ho_PolarModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_PolarModelContours, PolarModelID, 1);
                HOperatorSet.GetImageSize(ho_PolarImg0, out hWidth, out hHeight);
                HOperatorSet.FindShapeModel(ho_PolarImg0, PolarModelID, 0, 0, 0.5, 1,
                0.0, "least_squares", 3, 0.8, out hv_RowCheck, out hv_ColumnCheck,out hv_AngleCheck, out hv_Score);
                int n = (hv_Score.TupleLength());
                if (n <= 0) IsPi = true;
                if (IsPi)
                {
                    HOperatorSet.GetImageSize(ho_PolarImg1, out hWidth, out hHeight);
                    HOperatorSet.FindShapeModel(ho_PolarImg1, PolarModelID, 0, 0, 0.5, 1,
                    0.0, "least_squares", 3, 0.8, out hv_RowCheck, out hv_ColumnCheck, out hv_AngleCheck, out hv_Score);
                    int m = (hv_Score.TupleLength());
                    if (m <= 0)
                    {
                        ho_PolarModelContours.Dispose();
                        return -1;
                    }
                }
                ho_RefinedPos.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_RefinedPos, row, col, 50, 0);


                HTuple hv_MovementOfObject;


                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject);
                ho_ModelAtNewPosition.Dispose();
                HOperatorSet.AffineTransContourXld(ho_PolarModelContours, out ho_ModelAtNewPosition, hv_MovementOfObject);

                ho_Score = hv_Score;
                if (!IsPi)
                    ho_AngleCheck = (hv_ColumnCheck / hWidth) * Math.PI * 2;
                else
                    ho_AngleCheck = (hv_ColumnCheck / hWidth) * Math.PI * 2 - Math.PI;
                double dA = ho_AngleCheck;
                ho_ColumnCheck = col + (hv_RowCheck + R1) * Math.Cos(dA);
                ho_RowCheck = row - (hv_RowCheck + R1) * Math.Sin(dA);
                //	DispObj(ho_SearchImage, WindowHandle);
                HOperatorSet.DispObj(ho_RefinedPos, hv_WindowHandle);
                HOperatorSet.DispArrow(hv_WindowHandle, row, col, ho_RowCheck, ho_ColumnCheck, 5);
                HTuple Angle = ((HTuple)dA).TupleDeg();
                ho_AngleCheck = (double)Angle;//+(IsPi?180:0);
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                ho_RefinedPos.Dispose();
                ho_ModelAtNewPosition.Dispose();
            }
        }
        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="AngleRange">角度范围</param>
        /// <param name="minScore">最小得分</param>
        /// <param name="numMatches">搜索个数</param>
        /// <param name="b_Disp">是否显示</param>
        /// <param name="Row">X</param>
        /// <param name="Col">Y</param>
        /// <param name="Angle">角度</param>
        /// <returns></returns>
        public bool FindShapeModel(HObject ho_Image, HTuple hv_WindowHandle, HTuple ModelId,int AngleRange, double minScore, int numMatches, bool b_Disp,
            out double Row, out double Col, out double Angle)
        {
            HTuple hv_Row, hv_Column, hv_Angle1, hv_Score;
            HOperatorSet.SetSystem("border_shape_models", "false");
            try
            {
                HOperatorSet.FindShapeModel(ho_Image, ModelId, (new HTuple(0)).TupleRad(), (new HTuple(AngleRange)).TupleRad(),
                    minScore, numMatches, 0.4, "least_squares_high", 4, 0.9, out hv_Row, out hv_Column, out hv_Angle1, out hv_Score);
                if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, "green");
                    if (b_Disp)
                        HDevelopExport.dev_display_shape_matching_results(hv_WindowHandle, ModelId, "green", hv_Row, hv_Column, hv_Angle1, 1, 1, 0);
                    Row = hv_Row[0].D;
                    Col = hv_Column[0].D;
                    Angle = hv_Angle1[0].D;
                    return true;
                }
                else
                {
                    Row = 0;
                    Col = 0;
                    Angle = 0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Row = 0;
                Col = 0;
                Angle = 0;
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hv_Image"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <param name="grayValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <param name="ho_RowCheck">返回行数据</param>
        /// <param name="ho_ColumnCheck">返回列数据</param>
        /// <param name="ho_AngleCheck">返回角度数据</param>
        /// <param name="ho_Score">返回得分数据</param>
        /// <param name="Area"></param>
        /// <returns></returns>
        public bool FindAngle(HObject hv_Image, HTuple hv_WindowHandle, double row, double col, double R1, double R2, int grayValue,
            double minValue, double maxValue, int minArea, int maxArea, out HTuple ho_RowCheck, out HTuple ho_ColumnCheck,
            out HTuple ho_AngleCheck, out HTuple ho_Score, out HTuple Area)
        {
            ho_ColumnCheck = 0;
            ho_RowCheck = 0;
            ho_Score = 0;
            ho_AngleCheck = 0;
            Area = 0;
            HObject ho_Region = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null;
            HObject ho_Circle = null, ho_Circle1 = null;
            HObject ho_RegionDifference = null, ho_ImageReduced = null;
            HObject ho_RegionFillUp = null, ho_RegionClosing = null;
            HObject ho_RefinedPos = null;

            // Local control variables 
            HTuple hv_Value = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RefinedPos);
            try
            {
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced.Dispose();
                HOperatorSet.GenCircle(out ho_Circle, row, col, R2);
                HOperatorSet.GenCircle(out ho_Circle1, row, col, R1);
                HOperatorSet.Difference(ho_Circle, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.ReduceDomain(hv_Image, ho_RegionDifference, out ho_ImageReduced);
                //需调参数2  60 看现场图像 60可为对比度
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 0, grayValue);
                //填充
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
                //去除杂点 需调参数3  15  此大小图像一般无需再调
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, 10);

                //求出目标缺口 不同
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                //最大面积
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.RegionFeatures(ho_SelectedRegions, "ra", out hv_Value);
                ho_Score = hv_Value;
                //限定长度 防止找错 也可其它方法
                //需调参数4  Value>50and Value<100
                if ((int)((new HTuple(hv_Value.TupleGreater(minValue))).TupleAnd(new HTuple(hv_Value.TupleLess(maxValue)))) != 0)
                {
                    //区域重心
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out Area, out ho_RowCheck, out ho_ColumnCheck);
                    double Row = ho_RowCheck - row;
                    double Column = ho_ColumnCheck - col;
                    var angle = (Math.Atan2(Row, Column) / Math.PI) * -180;
                    ho_AngleCheck = Math.Round(angle, 2);
                    ho_RefinedPos.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_RefinedPos, row, col, 25, 0);
                    HOperatorSet.SetColor(hv_WindowHandle, "green");
                    HOperatorSet.DispObj(ho_RefinedPos, hv_WindowHandle);
                    HOperatorSet.DispArrow(hv_WindowHandle, row, col, ho_RowCheck, ho_ColumnCheck, 5);
                    HOperatorSet.DispObj(ho_SelectedRegions, hv_WindowHandle);
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    if (Area > minArea && Area < maxArea)
                        return true;
                    else return false;
                }
                else return false;
            }
            catch (HalconException Hdex)
            {
                return false;
                throw Hdex;
            }
            finally
            {
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_RegionDifference.Dispose();
                ho_RefinedPos.Dispose();
                ho_ImageReduced.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionClosing.Dispose();
            }
        }
        #region 保存、读取模板
        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="filePath">模板路径</param>
        /// <param name="ModelId">模板Id</param>
        /// <returns></returns>
        public bool WriteShapeModel(string filePath,HTuple ModelId)
        {
            try
            {
                if (filePath == "") return false;
                HOperatorSet.WriteShapeModel(ModelId, filePath);
                //if (bLog) log.Debug($"保存{Name}找圆模板成功");
                return true;
            }
            catch (Exception ex) { //if (bLog) log.Error($"保存{Name}找圆模板失败"); 
                return false; }
        }

        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="filePath">模板路径</param>
        /// <param name="ModelId">模板Id</param>
        /// <returns></returns>
        public bool ReadShapeModel(string filePath,out HTuple ModelId)
        {
            ModelId = null;
            if (filePath == "") return false;
            try
            {
                HOperatorSet.ReadShapeModel(filePath, out ModelId);
                //if (bLog) log.Debug($"读取{Name}找圆模板成功");
                return true;
            }
            catch (Exception) { //if (bLog) log.Error($"读取{Name}找圆模板失败"); 
                return false; }
        }
        #endregion
        //创建模板       
        public void CreateShapeModel(HTuple hv_WindowHandle, HObject ho_Image, HObject ho_Region, ref HTuple ModelID, int i_gauss, int AngleRange)
        {
            HOperatorSet.ClearAllShapeModels();
            HObject ho_ImageReduced, ho_ModelContours, ho_ContoursAffinTrans, ho_GaussImage;
            HTuple hv_Area, hv_Row4, hv_Column4, hv_HomMat2DIdentity, hv_HomMat2DTranslate;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
            HOperatorSet.GenEmptyObj(out ho_GaussImage);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                if (2 < i_gauss && i_gauss < 12)
                    HOperatorSet.GaussImage(ho_Image, out ho_GaussImage, i_gauss);
                else
                    ho_GaussImage = ho_Image;
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_GaussImage, ho_Region, out ho_ImageReduced);
                HOperatorSet.AreaCenter(ho_Region, out hv_Area, out hv_Row4, out hv_Column4);
                HOperatorSet.SetSystem("border_shape_models", "false");
                HOperatorSet.CreateShapeModel(ho_ImageReduced, 4, (new HTuple(0)).TupleRad(), 
                    (new HTuple(AngleRange)).TupleRad(), "auto", "point_reduction_high", 
                    "ignore_global_polarity", "auto", "auto", out ModelID);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, ModelID, 1);
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row4, hv_Column4, out hv_HomMat2DTranslate);
                ho_ContoursAffinTrans.Dispose();
                HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans, hv_HomMat2DTranslate);
                HOperatorSet.ClearWindow(hv_WindowHandle);
                HOperatorSet.DispObj(ho_ImageReduced, hv_WindowHandle);
                HOperatorSet.DispObj(ho_GaussImage, hv_WindowHandle);
                HOperatorSet.DispObj(ho_ContoursAffinTrans, hv_WindowHandle);
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// 创建极性模板
        /// </summary>
        /// <param name="ho_Input">输入图像</param>
        /// <param name="row1">row1</param>
        /// <param name="col1">column1</param>
        /// <param name="row2">row2</param>
        /// <param name="col2">column2</param>
        /// <param name="hv_PolarModelId">极性模板ID</param>
        /// <returns></returns>
        public void CreatePolarModel(HTuple hv_WindowHandle, HObject ho_Input, HObject ho_Region, out HTuple hv_PolarModelId)
        {
            hv_PolarModelId = 0;
            HObject ho_PolarModelImage, ho_PolarModelContours;
            HOperatorSet.GenEmptyObj(out ho_PolarModelImage);
            HOperatorSet.GenEmptyObj(out ho_PolarModelContours);
            ho_PolarModelContours.Dispose();
            ho_PolarModelImage.Dispose();

            HTuple hv_ContrastPolar, hv_MinContrast;
            hv_ContrastPolar = "auto";
            hv_MinContrast = 20;
            try
            {
                //HOperatorSet.GenRectangle1(out ho_AsymEdgesRegion, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(ho_Input, ho_Region, out ho_PolarModelImage);
                HOperatorSet.CreateShapeModel(ho_PolarModelImage, 3, 0, 0, "auto", "auto", "use_polarity", hv_ContrastPolar,
                        hv_MinContrast, out hv_PolarModelId);
                HOperatorSet.GetShapeModelContours(out ho_PolarModelContours, hv_PolarModelId, 1);
                HOperatorSet.DispObj(ho_PolarModelContours, hv_WindowHandle);
            }
            catch (Exception) { }
            finally
            {
                ho_PolarModelContours.Dispose();
                ho_PolarModelImage.Dispose();
            }
        }
        //寻找模板
        public bool FindShapeModel(HTuple hv_WindowHandle, HObject ho_Image, HObject ho_Region, HTuple ModelID, int AngleRange, double minScore, int numMatches, out double Row, out double Col, out double Angle)
        {
            HTuple hv_Row, hv_Column, hv_Angle1, hv_Score;
            HObject ho_ImageReduced;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.SetSystem("border_shape_models", "false");
            try
            {
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Region, out ho_ImageReduced);
                HTuple t1, t2;
                HOperatorSet.CountSeconds(out t1);
                HOperatorSet.FindShapeModel(ho_Region, ModelID, (new HTuple(0)).TupleRad(), (new HTuple(AngleRange)).TupleRad(), minScore, numMatches, 0.5, "least_squares_high", 4, 0.9, out hv_Row, out hv_Column, out hv_Angle1, out hv_Score);
                HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                HOperatorSet.CountSeconds(out t2);
                Angle = (t2 - t1) * 1000;
                if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    HDevelopExport.dev_display_shape_matching_results(hv_WindowHandle, ModelID, "green", hv_Row, hv_Column, hv_Angle1, 1, 1, 0);
                    Row = hv_Row[0].D;
                    Col = hv_Column[0].D;
                    Angle = hv_Angle1[0].D;
                    return true;
                }
                else
                {
                    Row = 0;
                    Col = 0;
                    Angle = 0;
                    return false;
                }

            }
            catch
            {
                Row = 0;
                Col = 0;
                Angle = 0;
                return false;
            }
        }
        public bool FindShapeModel(HTuple hv_WindowHandle, HObject ho_Image, HTuple ModelID, int AngleRange, double minScore, int numMatches, bool b_Disp, out double Row, out double Col, out double Angle)
        {
            HTuple hv_Row, hv_Column, hv_Angle1, hv_Score;
            HOperatorSet.SetSystem("border_shape_models", "false");
            try
            {
                HOperatorSet.FindShapeModel(ho_Image, ModelID, (new HTuple(0)).TupleRad(), (new HTuple(AngleRange)).TupleRad(),
                    minScore, numMatches, 0.5, "least_squares_high", 4, 0.9, out hv_Row, out hv_Column, out hv_Angle1, out hv_Score);
                if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    if (b_Disp)
                        HDevelopExport.dev_display_shape_matching_results(hv_WindowHandle, ModelID, "green",
                            hv_Row, hv_Column, hv_Angle1, 1, 1, 0);
                    Row = hv_Row[0].D;
                    Col = hv_Column[0].D;
                    Angle = hv_Angle1[0].D;
                    return true;
                }
                else
                {
                    Row = 0;
                    Col = 0;
                    Angle = 0;
                    return false;
                }
            }
            catch
            {
                Row = 0;
                Col = 0;
                Angle = 0;
                return false;
            }
        }

        private bool FindCircle(HObject ho_Image, HTuple hv_Row, HTuple hv_Col, HTuple hv_Radius, 
            string Transition, string Select, string Direct, out HTuple CircleRow, out HTuple CircleCol, out HTuple CircleRadius)
        {
            try
            {
                HTuple hv_RRow1, hv_CCol1;
                HTuple hv_ResultRow, hv_ResultColumn, hv_ArcType;
                HTuple hv_RowCenter2, hv_ColCenter2, hv_Radius2;
                HObject ho_Regions, ho_Circle;
                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.GenEmptyObj(out ho_Circle);
                HDevelopExport.circle_corner_point(hv_Row, hv_Col, hv_Radius, out hv_RRow1, out hv_CCol1);
                ho_Regions.Dispose();
                HDevelopExport.spoke(ho_Image, out ho_Regions, 200, 60, 20, 1, 10, Transition, Select, hv_RRow1, hv_CCol1, Direct, out hv_ResultRow, out hv_ResultColumn, out hv_ArcType);
                ho_Circle.Dispose();
                HDevelopExport.pts_to_best_circle(out ho_Circle, hv_ResultRow, hv_ResultColumn, ((new HTuple(hv_ResultColumn.TupleLength())) / 3) * 2, "circle", out hv_RowCenter2, out hv_ColCenter2, out hv_Radius2);
                if (ho_Circle != null)
                    ho_Circle.Dispose();
                if (ho_Regions != null)
                    ho_Regions.Dispose();
                if ((int)(new HTuple((new HTuple(hv_RowCenter2.TupleLength())).TupleGreater(0))) != 0 && hv_Radius2 > 1)
                {
                    CircleRow = hv_RowCenter2;
                    CircleCol = hv_ColCenter2;
                    CircleRadius = hv_Radius2;
                    return true;
                }
                else
                {
                    CircleRow = 0;
                    CircleCol = 0;
                    CircleRadius = 0;
                    return false;
                }

            }
            catch
            {
                CircleRow = 0;
                CircleCol = 0;
                CircleRadius = 0;
                return false;
            }
        }
        public void GetRegionFeature(HObject ho_Region, out HTuple Area, out HTuple Lenght, out HTuple Width)
        {
            HTuple hv_Area, hv_Row, hv_Col, hv_Lenght, hv_Width;
            HTuple hv_row1, hv_col1, hv_phi;
            HObject hv_ConnectRegion;
            HOperatorSet.GenEmptyObj(out hv_ConnectRegion);
            hv_ConnectRegion.Dispose();
            HOperatorSet.Connection(ho_Region, out hv_ConnectRegion);
            HOperatorSet.AreaCenter(hv_ConnectRegion, out hv_Area, out hv_Row, out hv_Col);
            HOperatorSet.SmallestRectangle2(hv_ConnectRegion, out hv_row1, out hv_col1, out hv_phi, out hv_Lenght, out hv_Width);
            Area = hv_Area;
            Lenght = hv_Lenght;
            Width = hv_Width;
            hv_ConnectRegion.Dispose();
        }
        public bool TwoRegionIntersection(HObject Region1, HObject Region2, HWindowControl hWindowControl, int i_open, int i_DispRadius)
        {
            HObject hv_IntersectionRegion, hv_OpenRegion;
            HTuple hv_area, hv_row, hv_col;
            HOperatorSet.GenEmptyObj(out hv_IntersectionRegion);
            HOperatorSet.GenEmptyObj(out hv_OpenRegion);
            hv_IntersectionRegion.Dispose();
            HOperatorSet.Intersection(Region1, Region2, out hv_IntersectionRegion);
            hv_OpenRegion.Dispose();
            HOperatorSet.OpeningCircle(hv_IntersectionRegion, out hv_OpenRegion, i_open);
            HOperatorSet.AreaCenter(hv_OpenRegion, out hv_area, out hv_row, out hv_col);
            hv_IntersectionRegion.Dispose();
            hv_OpenRegion.Dispose();
            if (hv_area > 0)
            {
                HOperatorSet.SetDraw(hWindowControl.HalconWindow, "margin");
                HOperatorSet.SetColor(hWindowControl.HalconWindow, "red");
                HOperatorSet.DispCircle(hWindowControl.HalconWindow, hv_row, hv_col, i_DispRadius);
                return false;
            }
            else
                return true;

        }
        public bool RegionFeatureInspect(HObject Region, HWindowControl hWindowControl, HTuple hv_compareArea, HTuple hv_compareLenght, HTuple hv_compareWidth, double d_AreaOkRange, double d_LenghtOkRange, double d_WidthOkRange)
        {
            HTuple hv_area, hv_row, hv_col;
            HTuple row1, col1, phi1, hv_lenght, hv_width;
            HOperatorSet.AreaCenter(Region, out hv_area, out hv_row, out hv_col);
            HOperatorSet.SmallestRectangle2(Region, out row1, out col1, out phi1, out hv_lenght, out hv_width);
            if ((1 - d_AreaOkRange) * hv_compareArea < hv_area && hv_area < (1 + d_AreaOkRange) * hv_compareArea && (1 - d_LenghtOkRange) * hv_compareLenght < hv_lenght && hv_lenght < (1 + d_LenghtOkRange) * hv_compareLenght && (1 - d_WidthOkRange) * hv_compareWidth < hv_width && hv_width < (1 + d_WidthOkRange) * hv_compareWidth)
                return true;
            else
            {
                HOperatorSet.SetDraw(hWindowControl.HalconWindow, "margin");
                HOperatorSet.SetColor(hWindowControl.HalconWindow, "red");
                Region.DispObj(hWindowControl.HalconWindow);
                return false;
            }
        }
        #region  " SoftDog "
        //SoftDog///////////////
        private bool b_key = false;
        public void Initial()
        {
            if (GetPassword())
                b_key = true;
            else
            {
                //StreamWriter writetext = File.AppendText("license.txt");
                StreamWriter writetext = new StreamWriter("license.dat", false);
                writetext.Write(GetCpuID() + "\r\n" + GetHardDiskID());
                writetext.Close();
                MessageBox.Show("License Error");
            }
        }
        //获取CPU编号
        public string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码 
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        //获取第一块硬盘编号
        public String GetHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                String strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }
        //解码
        public bool GetPassword()
        {
            try
            {
                //读取dynamickey和key
                FileStream fs = new FileStream("license.dat", FileMode.Open);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string s_dynamickey = sr.ReadLine();
                string s_readkey = sr.ReadLine();
                sr.Close();
                string s_cpu = GetCpuID();//获取Cpu ID
                string s_harddisk = GetHardDiskID();//获取硬盘ID
                string s_key = "";
                char[] c_cpu = s_cpu.Trim().ToCharArray();
                char[] c_harddisk = s_harddisk.Trim().ToCharArray();
                int i_Length = c_cpu.Length < c_harddisk.Length ? c_cpu.Length : c_harddisk.Length;
                for (int i = 0; i < i_Length; i++)
                {
                    if (i % s_dynamickey.Length == 0)
                        s_key = s_key + c_cpu[i].ToString() + c_harddisk[i].ToString() + s_dynamickey;
                    else if (i % s_dynamickey.Length == 1)
                        s_key = s_key + c_harddisk[i].ToString() + c_cpu[i].ToString() + s_dynamickey;
                    else if (i % s_dynamickey.Length == 2)
                        s_key = s_key + s_dynamickey + c_cpu[i].ToString() + c_harddisk[i].ToString();
                    else
                        s_key = s_key + c_cpu[i].ToString() + s_dynamickey + c_harddisk[i].ToString();
                }
                char[] c_key = s_key.ToCharArray();
                string key = "";
                for (int i = 0; i < 5; i++)
                {
                    key = key + c_key[i * (c_key.Length / 5)];
                }
                if (s_readkey.Trim() == key.Trim())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
    //定位跟随类
    [Serializable]
    public class MatchFollowClass
    {
        [XmlElement("路径")]
        public string ShapleLocation;
        [XmlElement("基点Y")]
        public double hv_BaseRow { get; set; }
        [XmlElement("基点X")]
        public double hv_BaseCol { get; set; }
        [XmlElement("查找数量")]
        public double hv_Match_Num { get; set; }
        [XmlElement("起始角度")]
        public double hv_Start_Angle { get; set; }
        [XmlElement("终止角度")]
        public double hv_End_Angle { get; set; }
        [XmlElement("最小分数")]
        public double hv_Match_Score { get; set; }
        [XmlElement("最大重叠系数")]
        public double hv_Discoverty { get; set; }
        [XmlElement("金字塔等级")]
        public double hv_Pyramid { get; set; }

        public bool GetHmat2D(HObject ho_Image, string ShapleLocation, HTuple hv_BaseRow, HTuple hv_BaseCol,
            HTuple hv_Match_Num, HTuple hv_Start_Angle, HTuple hv_End_Angle, HTuple hv_Match_Score, HTuple hv_Discoverty, HTuple hv_Pyramid, out HTuple hv_Hmat2d)
        {
            HTuple hv_ModelID, hv_dxfStatus, hv_Find_Row, hv_Find_Col, hv_Find_Phi, hv_Find_Score;
            HObject ho_ModelContours;

            try
            {
                HOperatorSet.ReadShapeModel(Path.Combine(ShapleLocation, "Model.shm"), out hv_ModelID);
                HOperatorSet.ReadContourXldDxf(out ho_ModelContours, Path.Combine(ShapleLocation, "Model.dxf"), "read_attributes", "true", out hv_dxfStatus);
                //查找模板
                HOperatorSet.FindShapeModel(ho_Image, hv_ModelID, hv_Start_Angle, hv_End_Angle, hv_Match_Score, hv_Match_Num, hv_Discoverty, "least_squares", hv_Pyramid,
                    0.7, out hv_Find_Row, out hv_Find_Col, out hv_Find_Phi, out hv_Find_Score);
                //产生仿射变换矩阵-》用于变换基准点
                HOperatorSet.VectorAngleToRigid(hv_BaseRow, hv_BaseCol, 0, hv_Find_Row, hv_Find_Col, hv_Find_Phi, out hv_Hmat2d);
                return true;
            }
            catch
            {
                hv_Hmat2d = null;
                return false;
            }
        }
    }
    class ReapImage
    {
        public void OpenImage(ref HObject m_Image, HWindowControl hWindowControl)
        {
            OpenFileDialog OpenImage = new OpenFileDialog();
            OpenImage.Title = "打开图片";
            OpenImage.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            if (OpenImage.ShowDialog() == DialogResult.OK)
            {
                HTuple hv_Width, hv_Height;
                if (m_Image != null)
                    m_Image.Dispose();
                HOperatorSet.GenEmptyObj(out m_Image);
                m_Image.Dispose();
                HOperatorSet.ReadImage(out m_Image, OpenImage.FileName);
                HOperatorSet.GetImageSize(m_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.DispObj(m_Image, hWindowControl.HalconWindow);
            }
        }
        public void OpenImage(ref HObject m_Image, HWindowControl hWindowControl, bool b_Disp)
        {
            OpenFileDialog OpenImage = new OpenFileDialog();
            OpenImage.Title = "打开图片";
            OpenImage.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            if (OpenImage.ShowDialog() == DialogResult.OK)
            {
                HTuple hv_Width, hv_Height;
                if (m_Image != null)
                    m_Image.Dispose();
                HOperatorSet.GenEmptyObj(out m_Image);
                m_Image.Dispose();
                HOperatorSet.ReadImage(out m_Image, OpenImage.FileName);
                HOperatorSet.GetImageSize(m_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);
                if (b_Disp)
                    HOperatorSet.DispObj(m_Image, hWindowControl.HalconWindow);
            }
        }
        public void OpenImage(ref HObject m_Image, HWindowControl hWindowControl, string Path)
        {
            HTuple hv_Width, hv_Height;
            if (m_Image != null)
                m_Image.Dispose();
            HOperatorSet.GenEmptyObj(out m_Image);
            m_Image.Dispose();
            HOperatorSet.ReadImage(out m_Image, Path);
            HOperatorSet.GetImageSize(m_Image, out hv_Width, out hv_Height);
            HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);
            HOperatorSet.DispObj(m_Image, hWindowControl.HalconWindow);
        }
        public void SaveImage(HObject m_Image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "保存图像";
            saveFileDialog.Filter = "保存图像|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (m_Image != null ? m_Image.IsInitialized() : false)
                    HOperatorSet.WriteImage(m_Image, "bmp", 0, saveFileDialog.FileName);
                else
                    throw new Exception("图片为空,无法保存...... ");
            }
        }
        public void SaveImage(HObject m_Image, string Path)
        {
            if (m_Image != null ? m_Image.IsInitialized() : false)
                HOperatorSet.WriteImage(m_Image, "bmp", 0, Path);
            else
                throw new Exception("图片为空,无法保存...... ");
        }
        public void DumpWindow(HWindowControl hWindowControl)
        {
            HObject ho_Image;
            HOperatorSet.GenEmptyObj(out ho_Image);
            ho_Image.Dispose();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "保存图像";
            saveFileDialog.Filter = "保存图像|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                HOperatorSet.DumpWindowImage(out ho_Image, hWindowControl.HalconWindow);
                HOperatorSet.WriteImage(ho_Image, "bmp", 0, saveFileDialog.FileName);
            }
            if (ho_Image != null)
                ho_Image.Dispose();
        }
        public void DumpWindow(HWindowControl hWindowControl, string Path)
        {
            HObject ho_Image;
            HOperatorSet.GenEmptyObj(out ho_Image);
            ho_Image.Dispose();
            HOperatorSet.DumpWindowImage(out ho_Image, hWindowControl.HalconWindow);
            HOperatorSet.WriteImage(ho_Image, "bmp", 0, Path);
            ho_Image.Dispose();
        }
    }
    class OperateIniFile
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        #endregion

        #region 读Ini文件

        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region 写Ini文件

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        public static void WriteXmlInnerText(string xmlFileRootName, string subrootName, string ElementName, string Value)
        {
            string[] xmlFileNameAndRootName = xmlFileRootName.Split(':');
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileNameAndRootName[0]); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(xmlFileNameAndRootName[1]);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);
            if (subroot != null)
            {
                XmlElement node = (XmlElement)subroot.SelectSingleNode(ElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
                if (node != null && Value != "")
                    node.InnerText = Value;
                else if (node != null && Value == "")
                    subroot.RemoveChild(subroot.SelectSingleNode(ElementName));
                else if (node == null && Value != "")
                {
                    XmlElement child = xmlDoc.CreateElement(ElementName);//创建一个子节点 
                    child.InnerText = Value;
                    subroot.AppendChild(child);//添加到根节点中 
                }
                xmlDoc.Save(xmlFileNameAndRootName[0]); //保存其更改
            }
            else
            {
                subroot = xmlDoc.CreateElement(subrootName);//创建一个子节点 
                if (Value != "")
                {
                    XmlElement child = xmlDoc.CreateElement(ElementName);//创建一个子节点 
                    child.InnerText = Value;
                    subroot.AppendChild(child);//添加到根节点中 
                }
                root.AppendChild(subroot);//添加到根节点中 
                xmlDoc.Save(xmlFileNameAndRootName[0]); //保存其更改
            }
        }
        public static void WriteXmlInnerText(string xmlFileRootName, string subrootName, string subsubrootName, string ElementName, string Value)
        {
            string[] xmlFileNameAndRootName = xmlFileRootName.Split(':');
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileNameAndRootName[0]); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(xmlFileNameAndRootName[1]);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);
            if (subroot == null)
            {
                subroot = xmlDoc.CreateElement(subrootName);//创建一个子节点 
                XmlNode subsubroot = xmlDoc.CreateElement(subsubrootName);//创建一个子子节点 
                if (Value != "")
                {
                    XmlElement child = xmlDoc.CreateElement(ElementName);//创建一个子节点 
                    child.InnerText = Value;
                    subsubroot.AppendChild(child);//添加到根节点中 
                }
                subroot.AppendChild(subsubroot);
                root.AppendChild(subroot);//添加到根节点中 
                xmlDoc.Save(xmlFileNameAndRootName[0]); //保存其更改
            }
            else
            {
                XmlNode subsubroot = subroot.SelectSingleNode(subsubrootName);
                if (subsubroot == null)
                {
                    subsubroot = xmlDoc.CreateElement(subsubrootName);//创建一个子子节点 
                    if (Value != "")
                    {
                        XmlElement child = xmlDoc.CreateElement(ElementName);//创建一个子节点 
                        child.InnerText = Value;
                        subsubroot.AppendChild(child);//添加到根节点中 
                    }
                    subroot.AppendChild(subsubroot);
                    root.AppendChild(subroot);
                    xmlDoc.Save(xmlFileNameAndRootName[0]); //保存其更改
                }
                else
                {
                    XmlNode child = subsubroot.SelectSingleNode(ElementName);
                    if (child == null)
                        child = xmlDoc.CreateElement(ElementName);//创建一个子节点 
                    child.InnerText = Value;
                    subsubroot.AppendChild(child);//添加到根节点中 
                    subroot.AppendChild(subsubroot);
                    root.AppendChild(subroot);
                    xmlDoc.Save(xmlFileNameAndRootName[0]); //保存其更改
                }
            }
        }
        public static string ReadXmlInnerText(string xmlFileRootName, string subrootName, string ElementName)
        {
            string[] xmlFileNameAndRootName = xmlFileRootName.Split(':');
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileNameAndRootName[0]); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(xmlFileNameAndRootName[1]);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);
            XmlElement node = (XmlElement)subroot.SelectSingleNode(ElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            if (node != null)
                return node.InnerText;
            else
                return "";
        }
        public static string ReadXmlInnerText(XmlNode root, string subrootName, string ElementName)
        {
            XmlNode subroot = root.SelectSingleNode(subrootName);
            XmlElement node = (XmlElement)subroot.SelectSingleNode(ElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            if (node != null)
                return node.InnerText;
            else
                return "";
        }
        public static string ReadXmlInnerText(string xmlFileRootName, string subrootName, string subsubrootName, string ElementName)
        {
            string[] xmlFileNameAndRootName = xmlFileRootName.Split(':');
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileNameAndRootName[0]); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(xmlFileNameAndRootName[1]);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);
            XmlNode subsubroot = subroot.SelectSingleNode(subsubrootName);
            XmlElement node = (XmlElement)subsubroot.SelectSingleNode(ElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            if (node != null)
                return node.InnerText;
            else
                return "";
        }
        public static string ReadXmlInnerText(XmlNode root, string subrootName, string subsubrootName, string ElementName)
        {
            XmlNode subroot = root.SelectSingleNode(subrootName);
            XmlNode subsubroot = subroot.SelectSingleNode(subsubrootName);
            XmlElement node = (XmlElement)subsubroot.SelectSingleNode(ElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            if (node != null)
                return node.InnerText;
            else
                return "";
        }
        public static void WriteXML(string xmlFileName, string rootName, string ChildElementName, string[] ChildAttributeName, string[] ChildAttributeValue, string[] GrandchildAttributeName, string[] GrandchildInnerText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlElement child = xmlDoc.CreateElement(ChildElementName);//创建一个子节点 
            if (ChildAttributeName.Length > 0 && ChildAttributeName.Length == ChildAttributeValue.Length)//设置子节点属性
            {
                for (int i = 0; i < ChildAttributeName.Length; i++)
                {
                    child.SetAttribute(ChildAttributeName[i], ChildAttributeValue[i]);//设置该节点ISBN属性 
                }
            }
            if (GrandchildAttributeName.Length > 0 && GrandchildAttributeName.Length == GrandchildInnerText.Length)//添加孙节点、设置其节点文本值
            {
                for (int j = 0; j < GrandchildAttributeName.Length; j++)
                {
                    XmlElement grandchild = xmlDoc.CreateElement(GrandchildAttributeName[j]);
                    grandchild.InnerText = GrandchildInnerText[j];//设置孙节点的文本值 
                    child.AppendChild(grandchild);//添加到子节点中 
                }
            }
            root.AppendChild(child);//添加到﹤bookshop﹥节点中 
            xmlDoc.Save(xmlFileName); //保存其更改
        }
        public static void WriteXML(string xmlFileName, string rootName, string subrootName, string ChildElementName, string[] ChildAttributeName, string[] ChildAttributeValue, string[] GrandchildAttributeName, string[] GrandchildInnerText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);//查找根节点下一级节点
            XmlElement child = xmlDoc.CreateElement(ChildElementName);//创建一个子节点 
            if (ChildAttributeName.Length > 0 && ChildAttributeName.Length == ChildAttributeValue.Length)//设置子节点属性
            {
                for (int i = 0; i < ChildAttributeName.Length; i++)
                {
                    child.SetAttribute(ChildAttributeName[i], ChildAttributeValue[i]);//设置该节点ISBN属性 
                }
            }
            if (GrandchildAttributeName.Length > 0 && GrandchildAttributeName.Length == GrandchildInnerText.Length)//添加孙节点、设置其节点文本值
            {
                for (int j = 0; j < GrandchildAttributeName.Length; j++)
                {
                    XmlElement grandchild = xmlDoc.CreateElement(GrandchildAttributeName[j]);
                    grandchild.InnerText = GrandchildInnerText[j];//设置孙节点的文本值 
                    child.AppendChild(grandchild);//添加到子节点中 
                }
            }
            subroot.AppendChild(child);//添加到﹤bookshop﹥节点中 
            xmlDoc.Save(xmlFileName); //保存其更改
        }
        public static void WriteXML(string xmlFileName, string rootName, string subrootName, string subsubrootName, string ChildElementName, string[] ChildAttributeName, string[] ChildAttributeValue, string[] GrandchildAttributeName, string[] GrandchildInnerText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);//查找根节点下一级节点
            XmlElement subsubroot = xmlDoc.CreateElement(subsubrootName);//创建一个子子节点 
            XmlElement child = xmlDoc.CreateElement(ChildElementName);//创建一个子节点 
            if (ChildAttributeName.Length > 0 && ChildAttributeName.Length == ChildAttributeValue.Length)//设置子节点属性
            {
                for (int i = 0; i < ChildAttributeName.Length; i++)
                {
                    child.SetAttribute(ChildAttributeName[i], ChildAttributeValue[i]);//设置该节点ISBN属性 
                }
            }
            if (GrandchildAttributeName.Length > 0 && GrandchildAttributeName.Length == GrandchildInnerText.Length)//添加孙节点、设置其节点文本值
            {
                for (int j = 0; j < GrandchildAttributeName.Length; j++)
                {
                    XmlElement grandchild = xmlDoc.CreateElement(GrandchildAttributeName[j]);
                    grandchild.InnerText = GrandchildInnerText[j];//设置孙节点的文本值 
                    child.AppendChild(grandchild);//添加到子节点中 
                }
            }
            subsubroot.AppendChild(child);
            subroot.AppendChild(subsubroot);
            xmlDoc.Save(xmlFileName); //保存其更改
        }
        public static void DeleteXML(string xmlFileName, string rootName, string DeleteElementName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            if (root.SelectSingleNode(DeleteElementName) != null)//找到的第一个匹配节点，如果没有匹配的节点就返回 null
                root.RemoveChild(root.SelectSingleNode(DeleteElementName));
            xmlDoc.Save(xmlFileName); //保存其更改
        }
        public static void DeleteXML(string xmlFileName, string rootName, string subrootName, string DeleteElementName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);//查找根节点下一级节点
            if (subroot.SelectSingleNode(DeleteElementName) != null)//找到的第一个匹配节点，如果没有匹配的节点就返回 null
                subroot.RemoveChild(subroot.SelectSingleNode(DeleteElementName));
            xmlDoc.Save(xmlFileName); //保存其更改
        }
        public static void ReadXML(string xmlFileName, string rootName, string ChildElementName, string[] ChildAttributeName, out string[] ChildAttributeValue, out string[] GrandchildInnerText)
        {
            string[] array_tempChildAttributeValue = new string[ChildAttributeName.Length];
            List<string> tempGrandchildInnerText = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlElement node = (XmlElement)root.SelectSingleNode(ChildElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            for (int i = 0; i < ChildAttributeName.Length; i++)
            {
                array_tempChildAttributeValue[i] = node.GetAttribute(ChildAttributeName[i]);
            }
            XmlNodeList grandchildNode = node.ChildNodes;
            foreach (XmlNode TempGrandchild in grandchildNode)
            {
                tempGrandchildInnerText.Add(TempGrandchild.InnerText);
            }
            string[] array_tempGrandchildInnerText = new string[tempGrandchildInnerText.Count()];
            for (int j = 0; j < tempGrandchildInnerText.Count(); j++)
            {
                array_tempGrandchildInnerText[j] = tempGrandchildInnerText[j];
            }
            ChildAttributeValue = array_tempChildAttributeValue;
            GrandchildInnerText = array_tempGrandchildInnerText;
        }
        public static void ReadXML(string xmlFileName, string rootName, string subrootName, string ChildElementName, string[] ChildAttributeName, out string[] ChildAttributeValue, out string[] GrandchildInnerText)
        {
            string[] array_tempChildAttributeValue = new string[ChildAttributeName.Length];
            List<string> tempGrandchildInnerText = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);//查找根节点下一级节点
            XmlElement node = (XmlElement)subroot.SelectSingleNode(ChildElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            for (int i = 0; i < ChildAttributeName.Length; i++)
            {
                array_tempChildAttributeValue[i] = node.GetAttribute(ChildAttributeName[i]);
            }
            XmlNodeList grandchildNode = node.ChildNodes;
            foreach (XmlNode TempGrandchild in grandchildNode)
            {
                tempGrandchildInnerText.Add(TempGrandchild.InnerText);
            }
            string[] array_tempGrandchildInnerText = new string[tempGrandchildInnerText.Count()];
            for (int j = 0; j < tempGrandchildInnerText.Count(); j++)
            {
                array_tempGrandchildInnerText[j] = tempGrandchildInnerText[j];
            }
            ChildAttributeValue = array_tempChildAttributeValue;
            GrandchildInnerText = array_tempGrandchildInnerText;
        }
        public static void ReadXML(string xmlFileName, string rootName, string subrootName, string subsubrootName, string ChildElementName, string[] ChildAttributeName, out string[] ChildAttributeValue, out string[] GrandchildInnerText)
        {
            string[] array_tempChildAttributeValue = new string[ChildAttributeName.Length];
            List<string> tempGrandchildInnerText = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName); //加载xml文件
            XmlNode root = xmlDoc.SelectSingleNode(rootName);//查找根节点
            XmlNode subroot = root.SelectSingleNode(subrootName);//查找根节点下一级节点
            XmlNode subsubroot = subroot.SelectSingleNode(subsubrootName);
            XmlElement node = (XmlElement)subsubroot.SelectSingleNode(ChildElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            for (int i = 0; i < ChildAttributeName.Length; i++)
            {
                array_tempChildAttributeValue[i] = node.GetAttribute(ChildAttributeName[i]);
            }
            XmlNodeList grandchildNode = node.ChildNodes;
            foreach (XmlNode TempGrandchild in grandchildNode)
            {
                tempGrandchildInnerText.Add(TempGrandchild.InnerText);
            }
            string[] array_tempGrandchildInnerText = new string[tempGrandchildInnerText.Count()];
            for (int j = 0; j < tempGrandchildInnerText.Count(); j++)
            {
                array_tempGrandchildInnerText[j] = tempGrandchildInnerText[j];
            }
            ChildAttributeValue = array_tempChildAttributeValue;
            GrandchildInnerText = array_tempGrandchildInnerText;
        }
        public static void ReadXML(XmlNode subroot, string rootName, string ChildElementName, string[] ChildAttributeName, out string[] ChildAttributeValue, out string[] GrandchildInnerText)
        {
            string[] array_tempChildAttributeValue = new string[ChildAttributeName.Length];
            List<string> tempGrandchildInnerText = new List<string>();
            XmlNode root = subroot.SelectSingleNode(rootName);//查找根节点
            XmlElement node = (XmlElement)root.SelectSingleNode(ChildElementName);//找到的第一个匹配节点，如果没有匹配的节点就返回 null
            for (int i = 0; i < ChildAttributeName.Length; i++)
            {
                array_tempChildAttributeValue[i] = node.GetAttribute(ChildAttributeName[i]);
            }
            XmlNodeList grandchildNode = node.ChildNodes;
            foreach (XmlNode TempGrandchild in grandchildNode)
            {
                tempGrandchildInnerText.Add(TempGrandchild.InnerText);
            }
            string[] array_tempGrandchildInnerText = new string[tempGrandchildInnerText.Count()];
            for (int j = 0; j < tempGrandchildInnerText.Count(); j++)
            {
                array_tempGrandchildInnerText[j] = tempGrandchildInnerText[j];
            }
            ChildAttributeValue = array_tempChildAttributeValue;
            GrandchildInnerText = array_tempGrandchildInnerText;
        }

        public static void InsertNote(string note, ListBox listBox1, bool WriteText)
        {
            string time = DateTime.Now.ToString();
            listBox1.Items.Insert(0, time + ": " + note);
            if (WriteText)
            {
                StreamWriter writetext = File.AppendText("Tool\\Note.txt");
                writetext.Write(time + ": " + note + "\r\n");
                writetext.Close();
            }
        }
        public static void InsertAlarm(string note, ListBox listBox1, bool WriteText)
        {
            string time = DateTime.Now.ToString();
            listBox1.Items.Insert(0, time + ": " + note);
            if (WriteText)
            {
                StreamWriter writetext = File.AppendText("Tool\\Alarm.txt");
                writetext.Write(time + ": " + note + "\r\n");
                writetext.Close();
            }
        }
        public static void ImageZoomUp(HObject m_Image, HWindowControl hWindowControl1, ContextMenuStrip contextMenuStrip1)
        {
            if (m_Image != null ? m_Image.IsInitialized() : false)
            {
                HTuple hv_Row1, hv_Column1, hv_Row2, hv_Column2;
                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                hWindowControl1.Focus();
                hWindowControl1.ContextMenuStrip = null;
                HOperatorSet.DrawRectangle1(hWindowControl1.HalconWindow, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                hWindowControl1.ContextMenuStrip = contextMenuStrip1;
                double aa = (hv_Row2 - hv_Row1) / 3.0 * 4.0;
                HOperatorSet.SetPart(hWindowControl1.HalconWindow, hv_Row1, hv_Column1, hv_Row2, hv_Column1 + aa);
                HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
                HOperatorSet.DispObj(m_Image, hWindowControl1.HalconWindow);
            }
        }
        public static void ImageZoomDown(HObject m_Image, HWindowControl hWindowControl1)
        {
            if (m_Image != null ? m_Image.IsInitialized() : false)
            {
                HTuple hv_Width, hv_Height;
                HOperatorSet.GetImageSize(m_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
                HOperatorSet.DispObj(m_Image, hWindowControl1.HalconWindow);
            }
        }
        public static void DispImagePixel(HWindowControl hWindowControl1)
        {
            HTuple hv_Row, hv_Column, hv_Button;
            HObject ho_Cross;
            HOperatorSet.GenEmptyObj(out ho_Cross);
            hWindowControl1.Focus();
            HOperatorSet.GetMbutton(hWindowControl1.HalconWindow, out hv_Row, out hv_Column, out hv_Button);
            ho_Cross.Dispose();
            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
            HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
            HDevelopExport.disp_message(hWindowControl1.HalconWindow, "(" + hv_Row + "," + hv_Column + ")", "image", hv_Row, hv_Column, "red", "true");
            HOperatorSet.DispObj(ho_Cross, hWindowControl1.HalconWindow);
            ho_Cross.Dispose();
        }
        public static void DispImagePixel(HWindowControl hWindowControl1, out double Row, out double Col)
        {
            HTuple hv_Row, hv_Column, hv_Button;
            HObject ho_Cross;
            HOperatorSet.GenEmptyObj(out ho_Cross);
            hWindowControl1.Focus();
            HOperatorSet.GetMbutton(hWindowControl1.HalconWindow, out hv_Row, out hv_Column, out hv_Button);
            Row = hv_Row[0].D;
            Col = hv_Column[0].D;
            ho_Cross.Dispose();
            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
            HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
            HDevelopExport.disp_message(hWindowControl1.HalconWindow, "(" + hv_Row + "," + hv_Column + ")", "image", hv_Row, hv_Column, "red", "true");
            HOperatorSet.DispObj(ho_Cross, hWindowControl1.HalconWindow);
            ho_Cross.Dispose();
        }

    }
    public partial class HDevelopExport
    {
        public static void rectangle_corner_point(HTuple hv_CenterR, HTuple hv_CenterC, HTuple hv_Phi,
    HTuple hv_Len1, HTuple hv_Len2, out HTuple hv_CornerR, out HTuple hv_CornerC)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Cos = null, hv_Sin = null, hv_TempCornerR = null;
            HTuple hv_TempCornerC = null, hv_SortedCornerR = null;
            HTuple hv_IndicesCornerR = null;
            // Initialize local and output iconic variables 
            HOperatorSet.TupleCos(hv_Phi, out hv_Cos);
            HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
            hv_CornerR = new HTuple();
            hv_CornerC = new HTuple();
            hv_TempCornerR = new HTuple();
            hv_TempCornerR = hv_TempCornerR.TupleConcat((hv_CenterR + (hv_Len1 * hv_Sin)) - (hv_Len2 * hv_Cos));
            hv_TempCornerR = hv_TempCornerR.TupleConcat((hv_CenterR - (hv_Len1 * hv_Sin)) - (hv_Len2 * hv_Cos));
            hv_TempCornerR = hv_TempCornerR.TupleConcat((hv_CenterR - (hv_Len1 * hv_Sin)) + (hv_Len2 * hv_Cos));
            hv_TempCornerR = hv_TempCornerR.TupleConcat((hv_CenterR + (hv_Len1 * hv_Sin)) + (hv_Len2 * hv_Cos));
            hv_TempCornerC = new HTuple();
            hv_TempCornerC = hv_TempCornerC.TupleConcat((hv_CenterC - (hv_Len1 * hv_Cos)) - (hv_Len2 * hv_Sin));
            hv_TempCornerC = hv_TempCornerC.TupleConcat((hv_CenterC + (hv_Len1 * hv_Cos)) - (hv_Len2 * hv_Sin));
            hv_TempCornerC = hv_TempCornerC.TupleConcat((hv_CenterC + (hv_Len1 * hv_Cos)) + (hv_Len2 * hv_Sin));
            hv_TempCornerC = hv_TempCornerC.TupleConcat((hv_CenterC - (hv_Len1 * hv_Cos)) + (hv_Len2 * hv_Sin));
            HOperatorSet.TupleSort(hv_TempCornerR, out hv_SortedCornerR);
            HOperatorSet.TupleSortIndex(hv_TempCornerR, out hv_IndicesCornerR);
            if ((int)(new HTuple(((hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                0)))).TupleLess(hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                1))))) != 0)
            {
                if ((int)(new HTuple(((hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                    2)))).TupleGreater(hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                    3))))) != 0)
                {
                    hv_CornerR = new HTuple();
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        0));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        1));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        2));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        3));
                    hv_CornerC = new HTuple();
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(0)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(1)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(2)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(3)));
                }
                else
                {
                    hv_CornerR = new HTuple();
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        0));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        1));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        3));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        2));
                    hv_CornerC = new HTuple();
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(0)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(1)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(3)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(2)));
                }
            }
            else
            {
                if ((int)(new HTuple(((hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                    2)))).TupleGreater(hv_TempCornerC.TupleSelect(hv_IndicesCornerR.TupleSelect(
                    3))))) != 0)
                {
                    hv_CornerR = new HTuple();
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        1));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        0));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        2));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        3));
                    hv_CornerC = new HTuple();
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(1)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(0)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(2)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(3)));
                }
                else
                {
                    hv_CornerR = new HTuple();
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        1));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        0));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        3));
                    hv_CornerR = hv_CornerR.TupleConcat(hv_SortedCornerR.TupleSelect(
                        2));
                    hv_CornerC = new HTuple();
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(1)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(0)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(3)));
                    hv_CornerC = hv_CornerC.TupleConcat(hv_TempCornerC.TupleSelect(
                        hv_IndicesCornerR.TupleSelect(2)));
                }
            }

            return;
        }

        public static void circle_corner_point(HTuple hv_Row, HTuple hv_Col, HTuple hv_Radius,
      out HTuple hv_RRow, out HTuple hv_CCol)
        {
            hv_RRow = new HTuple();
            hv_RRow = hv_RRow.TupleConcat(hv_Row);
            hv_RRow = hv_RRow.TupleConcat(hv_Row + hv_Radius);
            hv_RRow = hv_RRow.TupleConcat(hv_Row);
            hv_RRow = hv_RRow.TupleConcat(hv_Row - hv_Radius);
            hv_RRow = hv_RRow.TupleConcat(hv_Row);
            hv_CCol = new HTuple();
            hv_CCol = hv_CCol.TupleConcat(hv_Col - hv_Radius);
            hv_CCol = hv_CCol.TupleConcat(hv_Col);
            hv_CCol = hv_CCol.TupleConcat(hv_Col + hv_Radius);
            hv_CCol = hv_CCol.TupleConcat(hv_Col);
            hv_CCol = hv_CCol.TupleConcat(hv_Col - hv_Radius);
            return;
        }

        public static void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
        HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
            HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple();
            HTuple hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_CurrentColor = new HTuple();

            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 

            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If set to 'true', the text is written within a white box.
            //
            //prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //display text box depending on text size
            if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
            {
                //calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3, hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }

        public static void dev_display_shape_matching_results(HTuple hv_WindowHandle, HTuple hv_ModelID, HTuple hv_Color,
     HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_ScaleR, HTuple hv_ScaleC, HTuple hv_Model)
        {
            HObject ho_ModelContours = null, ho_ContoursAffinTrans = null;


            // Local control variables 

            HTuple hv_NumMatches, hv_Index = new HTuple();
            HTuple hv_Match = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DScale = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_HomMat2DTranslate = new HTuple();

            HTuple hv_Model_COPY_INP_TMP = hv_Model.Clone();
            HTuple hv_ScaleC_COPY_INP_TMP = hv_ScaleC.Clone();
            HTuple hv_ScaleR_COPY_INP_TMP = hv_ScaleR.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);

            //This procedure displays the results of Shape-Based Matching.
            //
            hv_NumMatches = new HTuple(hv_Row.TupleLength());
            if ((int)(new HTuple(hv_NumMatches.TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_ScaleR_COPY_INP_TMP.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleR_COPY_INP_TMP, out hv_ScaleR_COPY_INP_TMP);
                }
                if ((int)(new HTuple((new HTuple(hv_ScaleC_COPY_INP_TMP.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleC_COPY_INP_TMP, out hv_ScaleC_COPY_INP_TMP);
                }
                if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    HOperatorSet.TupleGenConst(hv_NumMatches, 0, out hv_Model_COPY_INP_TMP);
                }
                else if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength()
                    )).TupleEqual(1))) != 0)
                {
                    HOperatorSet.TupleGenConst(hv_NumMatches, hv_Model_COPY_INP_TMP, out hv_Model_COPY_INP_TMP);
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ModelID.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    ho_ModelContours.Dispose();
                    HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID.TupleSelect(
                        hv_Index), 1);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_Color.TupleSelect(
                            hv_Index % (new HTuple(hv_Color.TupleLength()))));
                    }
                    for (hv_Match = 0; hv_Match.Continue(hv_NumMatches - 1, 1); hv_Match = hv_Match.TupleAdd(1))
                    {
                        if ((int)(new HTuple(hv_Index.TupleEqual(hv_Model_COPY_INP_TMP.TupleSelect(
                            hv_Match)))) != 0)
                        {
                            HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                            HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_ScaleR_COPY_INP_TMP.TupleSelect(
                                hv_Match), hv_ScaleC_COPY_INP_TMP.TupleSelect(hv_Match), 0, 0, out hv_HomMat2DScale);
                            HOperatorSet.HomMat2dRotate(hv_HomMat2DScale, hv_Angle.TupleSelect(hv_Match),
                                0, 0, out hv_HomMat2DRotate);
                            HOperatorSet.HomMat2dTranslate(hv_HomMat2DRotate, hv_Row.TupleSelect(
                                hv_Match), hv_Column.TupleSelect(hv_Match), out hv_HomMat2DTranslate);
                            ho_ContoursAffinTrans.Dispose();
                            HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans,
                                hv_HomMat2DTranslate);
                            HOperatorSet.DispObj(ho_ContoursAffinTrans, hv_WindowHandle);
                        }
                    }
                }
            }
            ho_ModelContours.Dispose();
            ho_ContoursAffinTrans.Dispose();

            return;
        }

        public static void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
      HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_TempArrow = null;


            // Local control variables 

            HTuple hv_Length, hv_ZeroLengthIndices, hv_DR;
            HTuple hv_DC, hv_HalfHeadWidth, hv_RowP1, hv_ColP1, hv_RowP2;
            HTuple hv_ColP2, hv_Index;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);

            try
            {
                //This procedure generates arrow shaped XLD contours,
                //pointing from (Row1, Column1) to (Row2, Column2).
                //If starting and end point are identical, a contour consisting
                //of a single point is returned.
                //
                //input parameteres:
                //Row1, Column1: Coordinates of the arrows' starting points
                //Row2, Column2: Coordinates of the arrows' end points
                //HeadLength, HeadWidth: Size of the arrow heads in pixels
                //
                //output parameter:
                //Arrow: The resulting XLD contour
                //
                //The input tuples Row1, Column1, Row2, and Column2 have to be of
                //the same length.
                //HeadLength and HeadWidth either have to be of the same length as
                //Row1, Column1, Row2, and Column2 or have to be a single element.
                //If one of the above restrictions is violated, an error will occur.
                //
                //
                //Init
                ho_Arrow.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Arrow);
                //
                //Calculate the arrow length
                HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
                //
                //Mark arrows with identical start and end point
                //(set Length to -1 to avoid division-by-zero exception)
                hv_ZeroLengthIndices = hv_Length.TupleFind(0);
                if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
                {
                    hv_Length[hv_ZeroLengthIndices] = -1;
                }
                //
                //Calculate auxiliary variables.
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
                //
                //Calculate end points of the arrow head.
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
                //
                //Finally create output XLD contour for each input point pair
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                    {
                        //Create_ single points for arrows with identical start and end point
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                            hv_Index), hv_Column1.TupleSelect(hv_Index));
                    }
                    else
                    {
                        //Create arrow contour
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                    OTemp[SP_O] = ho_Arrow.CopyObj(1, -1);
                    SP_O++;
                    ho_Arrow.Dispose();
                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_TempArrow, out ho_Arrow);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                }
                ho_TempArrow.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TempArrow.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void draw_rake(out HObject ho_Regions, HTuple hv_WindowHandle, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_Row1, out HTuple hv_Column1,
            out HTuple hv_Row2, out HTuple hv_Column2)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle = null;
            HObject ho_Arrow1 = null;


            // Local control variables 

            HTuple hv_ATan, hv_Deg1, hv_Deg, hv_i, hv_RowC = new HTuple();
            HTuple hv_ColC = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);

            try
            {
                disp_message(hv_WindowHandle, "点击鼠标左键画一条直线,点击右键确认", "window",
                    12, 12, "red", "false");
                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    out hv_Column2);
                //disp_line (WindowHandle, Row1, Column1, Row2, Column2)
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_Row1, hv_Column1, hv_Row2,
                    hv_Column2);
                OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                SP_O++;
                ho_Regions.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_RegionLines, out ho_Regions);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                HOperatorSet.TupleAtan2((-hv_Row2) + hv_Row1, hv_Column2 - hv_Column1, out hv_ATan);
                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);

                hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);


                for (hv_i = 1; hv_i.Continue(hv_Elements, 1); hv_i = hv_i.TupleAdd(1))
                {
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * hv_i) / (hv_Elements + 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * hv_i) / (hv_Elements + 1));

                    if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                    {
                        HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_Distance / 2);
                    }
                    else
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_DetectWidth / 2);
                    }

                    OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                    SP_O++;
                    ho_Regions.Dispose();
                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Rectangle, out ho_Regions);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                    if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                    {
                        hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                        SP_O++;
                        ho_Regions.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_Regions);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                    }
                }

                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void draw_spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_WindowHandle,
            HTuple hv_Elements, HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_ROIRows,
            out HTuple hv_ROICols, out HTuple hv_Direct)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_ContOut1, ho_Contour, ho_ContCircle;
            HObject ho_Cross, ho_Rectangle1 = null, ho_Arrow1 = null;


            // Local control variables 

            HTuple hv_Rows, hv_Cols, hv_Weights, hv_Length1;
            HTuple hv_RowC, hv_ColumnC, hv_Radius, hv_StartPhi, hv_EndPhi;
            HTuple hv_PointOrder, hv_RowXLD, hv_ColXLD, hv_Row1, hv_Column1;
            HTuple hv_Row2, hv_Column2, hv_DistanceStart, hv_DistanceEnd;
            HTuple hv_Length, hv_Length2, hv_i, hv_j = new HTuple();
            HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple(), hv_ATan = new HTuple();
            HTuple hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ContOut1);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);

            hv_ROIRows = new HTuple();
            hv_ROICols = new HTuple();
            hv_Direct = new HTuple();
            try
            {
                disp_message(hv_WindowHandle, "1、画4个以上点确定一个圆弧,点击右键确认", "window",
                    12, 12, "red", "false");
                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                ho_ContOut1.Dispose();
                HOperatorSet.DrawNurbs(out ho_ContOut1, hv_WindowHandle, "true", "true", "true",
                    "true", 3, out hv_Rows, out hv_Cols, out hv_Weights);
                HOperatorSet.TupleLength(hv_Weights, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(4))) != 0)
                {
                    disp_message(hv_WindowHandle, "提示：点数太少，请重画", "window", 32, 12,
                        "red", "false");
                    hv_ROIRows = new HTuple();
                    hv_ROICols = new HTuple();
                    ho_ContOut1.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }


                hv_ROIRows = hv_Rows.Clone();
                hv_ROICols = hv_Cols.Clone();

                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);

                HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3, 2, out hv_RowC,
                    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                ho_ContCircle.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
                OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                SP_O++;
                ho_Regions.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ContCircle, out ho_Regions);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ContCircle, HDevWindowStack.GetActive());
                }
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowC, hv_ColumnC, 60, 0.785398);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Cross, HDevWindowStack.GetActive());
                }
                disp_message(hv_WindowHandle, "2、远离圆心，画箭头确定边缘检测方向，点击右键确认",
                    "window", 12, 12, "red", "false");
                if (HDevWindowStack.IsOpen())
                {
                    //dev_set_color ('red')
                }
                HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    out hv_Column2);
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row1, hv_Column1, out hv_DistanceStart);
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row2, hv_Column2, out hv_DistanceEnd);
                HOperatorSet.LengthXld(ho_ContCircle, out hv_Length);
                HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
                if ((int)(new HTuple(hv_Elements.TupleLess(1))) != 0)
                {
                    hv_ROIRows = new HTuple();
                    hv_ROICols = new HTuple();
                    ho_ContOut1.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }
                for (hv_i = 0; hv_i.Continue(hv_Elements - 1, 1); hv_i = hv_i.TupleAdd(1))
                {
                    if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                        hv_Length2 - 1)))) != 0)
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    }
                    else
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);

                    }
                    if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                    {
                        hv_j = hv_Length2 - 1;
                        //continue
                    }

                    hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                    hv_ColE = hv_ColXLD.TupleSelect(hv_j);
                    if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                        hv_Direct = "inner";
                    }
                    else
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_Direct = "outer";
                    }


                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                        hv_DetectHeight / 2, hv_DetectWidth / 2);
                    OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                    SP_O++;
                    ho_Regions.Dispose();
                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Rectangle1, out ho_Regions);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                    if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                    {
                        hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                        SP_O++;
                        ho_Regions.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_Regions);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                    }
                }

                ho_ContOut1.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ContOut1.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void pts_to_best_circle(out HObject ho_Circle, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ActiveNum, HTuple hv_ArcType, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
            out HTuple hv_Radius)
        {


            // Local iconic variables 

            HObject ho_Contour = null;


            // Local control variables 

            HTuple hv_Length, hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_Length1 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Contour);

            try
            {
                hv_RowCenter = 0;
                hv_ColCenter = 0;
                hv_Radius = 0;

                ho_Circle.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Circle);
                HOperatorSet.TupleLength(hv_Cols, out hv_Length);

                if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(
                    new HTuple(hv_ActiveNum.TupleGreater(2)))) != 0)
                {

                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                    HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2,
                        out hv_RowCenter, out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi,
                        out hv_PointOrder);

                    HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
                    if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                    {
                        ho_Contour.Dispose();

                        return;
                    }
                    if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
                    {
                        ho_Circle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                            hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                    }
                    else
                    {
                        ho_Circle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                            hv_Radius, 0, (new HTuple(360)).TupleRad(), hv_PointOrder, 1);
                    }
                }

                ho_Contour.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void pts_to_best_line(out HObject ho_Line, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ActiveNum, out HTuple hv_Row1, out HTuple hv_Col1, out HTuple hv_Row2,
            out HTuple hv_Col2)
        {


            // Local iconic variables 

            HObject ho_Contour = null;


            // Local control variables 

            HTuple hv_Length, hv_Nr = new HTuple(), hv_Nc = new HTuple();
            HTuple hv_Dist = new HTuple(), hv_Length1 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Contour);

            try
            {
                hv_Row1 = 0;
                hv_Col1 = 0;
                hv_Row2 = 0;
                hv_Col2 = 0;
                ho_Line.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Line);
                HOperatorSet.TupleLength(hv_Cols, out hv_Length);

                if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(
                    new HTuple(hv_ActiveNum.TupleGreater(1)))) != 0)
                {

                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                    HOperatorSet.FitLineContourXld(ho_Contour, "tukey", hv_ActiveNum, 0, 5, 2,
                        out hv_Row1, out hv_Col1, out hv_Row2, out hv_Col2, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    HOperatorSet.TupleLength(hv_Dist, out hv_Length1);
                    if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                    {
                        ho_Contour.Dispose();

                        return;
                    }
                    ho_Line.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Line, hv_Row1.TupleConcat(hv_Row2),
                        hv_Col1.TupleConcat(hv_Col2));

                }

                ho_Contour.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void rake(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2,
            HTuple hv_Column2, out HTuple hv_ResultRow, out HTuple hv_ResultColumn)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_Rectangle = null, ho_Arrow1 = null;


            // Local control variables 

            HTuple hv_Width, hv_Height, hv_ATan, hv_Deg1;
            HTuple hv_Deg, hv_i, hv_RowC = new HTuple(), hv_ColC = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple(), hv_ColL1 = new HTuple();
            HTuple hv_MsrHandle_Measure = new HTuple(), hv_RowEdge = new HTuple();
            HTuple hv_ColEdge = new HTuple(), hv_Amplitude = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple(), hv_t = new HTuple();
            HTuple hv_Number = new HTuple(), hv_j = new HTuple();

            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);

            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                hv_ResultRow = new HTuple();
                hv_ResultColumn = new HTuple();
                HOperatorSet.TupleAtan2((-hv_Row2) + hv_Row1, hv_Column2 - hv_Column1, out hv_ATan);
                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);

                hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

                HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);


                for (hv_i = 1; hv_i.Continue(hv_Elements, 1); hv_i = hv_i.TupleAdd(1))
                {
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * hv_i) / (hv_Elements + 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * hv_i) / (hv_Elements + 1));
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                    {
                        HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_Distance / 2);
                    }
                    else
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                            hv_Deg.TupleRad(), hv_DetectHeight / 2, hv_DetectWidth / 2);
                    }

                    OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                    SP_O++;
                    ho_Regions.Dispose();
                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Rectangle, out ho_Regions);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                    if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                    {
                        hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                        SP_O++;
                        ho_Regions.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_Regions);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                    }
                    HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_Deg.TupleRad(), hv_DetectHeight / 2,
                        hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);


                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                    {
                        hv_Transition_COPY_INP_TMP = "negative";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                        {

                            hv_Transition_COPY_INP_TMP = "positive";
                        }
                        else
                        {
                            hv_Transition_COPY_INP_TMP = "all";
                        }
                    }

                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                    {
                        hv_Select_COPY_INP_TMP = "first";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                        {

                            hv_Select_COPY_INP_TMP = "last";
                        }
                        else
                        {
                            hv_Select_COPY_INP_TMP = "all";
                        }
                    }

                    HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                        hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                        out hv_Amplitude, out hv_Distance);
                    HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                    hv_tRow = 0;
                    hv_tCol = 0;
                    hv_t = 0;
                    HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                    {
                        continue;
                    }
                    for (hv_j = 0; hv_j.Continue(hv_Number - 1, 1); hv_j = hv_j.TupleAdd(1))
                    {
                        if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                            hv_t))) != 0)
                        {

                            hv_tRow = hv_RowEdge.TupleSelect(hv_j);
                            hv_tCol = hv_ColEdge.TupleSelect(hv_j);
                            hv_t = ((hv_Amplitude.TupleSelect(hv_j))).TupleAbs();
                        }
                    }
                    if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                    {

                        hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                        hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                    }
                }
                HOperatorSet.TupleLength(hv_ResultRow, out hv_Number);


                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_ROIRows, HTuple hv_ROICols,
            HTuple hv_Direct, out HTuple hv_ResultRow, out HTuple hv_ResultColumn, out HTuple hv_ArcType)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
            HObject ho_Arrow1 = null;


            // Local control variables 

            HTuple hv_Width, hv_Height, hv_RowC, hv_ColumnC;
            HTuple hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder;
            HTuple hv_RowXLD, hv_ColXLD, hv_Length, hv_Length2, hv_i;
            HTuple hv_j = new HTuple(), hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple();
            HTuple hv_ColL2 = new HTuple(), hv_ColL1 = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple(), hv_t = new HTuple();
            HTuple hv_Number = new HTuple(), hv_k = new HTuple();

            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);

            hv_ArcType = new HTuple();
            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                hv_ResultRow = new HTuple();
                hv_ResultColumn = new HTuple();


                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);

                HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3, 2, out hv_RowC,
                    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                ho_ContCircle.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
                HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

                HOperatorSet.LengthXld(ho_ContCircle, out hv_Length);
                HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
                if ((int)(new HTuple(hv_Elements.TupleLess(1))) != 0)
                {

                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    return;
                }
                for (hv_i = 0; hv_i.Continue(hv_Elements - 1, 1); hv_i = hv_i.TupleAdd(1))
                {
                    if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                        hv_Length2 - 1)))) != 0)
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                        hv_ArcType = "circle";
                    }
                    else
                    {
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                        hv_ArcType = "arc";
                    }
                    if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                    {
                        hv_j = hv_Length2 - 1;
                        //continue
                    }
                    hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                    hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                    //超出图像区域，不检测，否则容易报异常
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;

                    }
                    else
                    {
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);

                    }


                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                        hv_DetectHeight / 2, hv_DetectWidth / 2);
                    OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                    SP_O++;
                    ho_Regions.Dispose();
                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Rectangle1, out ho_Regions);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                    if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                    {
                        hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                        hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        OTemp[SP_O] = ho_Regions.CopyObj(1, -1);
                        SP_O++;
                        ho_Regions.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_Regions);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                    }

                    HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, hv_DetectHeight / 2,
                        hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);


                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                    {
                        hv_Transition_COPY_INP_TMP = "negative";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                        {

                            hv_Transition_COPY_INP_TMP = "positive";
                        }
                        else
                        {
                            hv_Transition_COPY_INP_TMP = "all";
                        }
                    }

                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                    {
                        hv_Select_COPY_INP_TMP = "first";
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                        {

                            hv_Select_COPY_INP_TMP = "last";
                        }
                        else
                        {
                            hv_Select_COPY_INP_TMP = "all";
                        }
                    }

                    HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                        hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                        out hv_Amplitude, out hv_Distance);
                    HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                    hv_tRow = 0;
                    hv_tCol = 0;
                    hv_t = 0;
                    HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                    {
                        continue;
                    }
                    for (hv_k = 0; hv_k.Continue(hv_Number - 1, 1); hv_k = hv_k.TupleAdd(1))
                    {
                        if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                            hv_t))) != 0)
                        {

                            hv_tRow = hv_RowEdge.TupleSelect(hv_k);
                            hv_tCol = hv_ColEdge.TupleSelect(hv_k);
                            hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
                        }
                    }
                    if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                    {

                        hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                        hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                    }
                }


                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                throw HDevExpDefaultException;
            }
        }
    }
}
