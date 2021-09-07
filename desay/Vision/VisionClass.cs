using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using log4net;
//using ImageAcquition;
using desay.ProductData;
using Vision.HalconAps;
using ViewWindow.Model;

namespace desay.Vision
{
    public struct MatchResult
    {
        public HTuple hv_RowCheck;
        public HTuple hv_ColumnCheck;
        public HTuple hv_AngleCheck;
        public HTuple hv_Score;
    }
    public struct VisionResult
    {
        public string Result;
        public double Row;
        public double Column;
        public double Angle;
        public double m_Score;
        public double p_Score;
        public double regionGray;
        public bool IsPositive;

        public double Row_Pixel;
        public double Column_Pixel;
    }
    public class VisionClass
    {
        ILog log = LogManager.GetLogger(typeof(VisionClass));
        private HTuple winWidth, winHeight;
        public HTuple hv_Width;
        public HTuple hv_Height;
        private string str_imgSize;

        public bool IsDisplayCross = false;
        public string Name;
        public HObject ho_Image = null;//原图像
        public HObject ho_Image_Noglue = null;//原图像
        public HWindow hv_WindowHandle = new HWindow();//窗口句柄
        public HWndCtrl viewController; //显示
        public ROIController roiController;
        //public int Area;//区域面积
        public double modeltime;
        public double circletime;
        public double graytime;
        public double polartime;
        public double vistime;
        //找圆模板
        public HTuple ModelId = null;
        public VisionResult VisionResult;

        //图像坐标 实际坐标点 
        public double[,] ImagePoints = new double[9, 2];
        public double[,] AxisPoints = new double[9, 2];
        /// <summary>
        /// 标定系数矩阵
        /// </summary>
        private double[] HomMat2D = new double[6];
        /// <summary>
        /// 标定系数矩阵
        /// </summary>
        public HTuple h_HomMat2D = new HTuple();

      

        public VisionClass(HWindowControl hWindowControl)
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image_Noglue);
            hv_WindowHandle = hWindowControl.HalconWindow;
            winWidth = hWindowControl.Width;
            winHeight = hWindowControl.Height;
            viewController = new HWndCtrl(hWindowControl);
            roiController = new ROIController();
            viewController.useROIController(roiController);
            viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
           
            //初始化
            log.Debug($"{Name}初始化控件");
        }
        ~VisionClass()
        {
            log.Debug($"{Name}释放");
            if (ho_Image.Key != IntPtr.Zero) ho_Image.Dispose();
            try
            {
                if (ModelId != null)
                    if (ModelId.Type != HTupleType.EMPTY)
                        HOperatorSet.ClearShapeModel(ModelId);
            }
            catch { }
            log.Debug($"{Name}释放");
        }

        public void ReleaseModelID()
        {
            try
            {
                if (ModelId != null)
                    if (ModelId.Type != HTupleType.EMPTY)
                        HOperatorSet.ClearShapeModel(ModelId);
            }
            catch { }
        }

        /// <summary>
        /// 更新传入并显示图像
        /// </summary>
        /// <param name="hImg"></param>
        /// <returns></returns>
        public bool UpdataImg(HObject hImg)
        {
            HObject h_emtpy;
            HOperatorSet.GenEmptyObj(out h_emtpy);
            try
            {
                HTuple hV;
                HOperatorSet.TestEqualObj(h_emtpy, hImg, out hV);
                h_emtpy.Dispose();
                if (hV == 1 || hImg == null) { log.Error($"{Name}传入图像为空"); return false; }
                else
                {
                    this.ho_Image.Dispose();
                    this.ho_Image = hImg.Clone();
                    FitImage(ho_Image);
                    HOperatorSet.ClearWindow(hv_WindowHandle);
                    HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                    viewController.addIconicVar(ho_Image);
                    viewController.repaint();
                    return true;
                }
            }
            catch (Exception) { return false; }
        }
        /// <summary>
        /// 更新传入并显示图像
        /// </summary>
        /// <param name="hImg"></param>
        /// <returns></returns>
        public bool UpdataImg_Noglue(HObject hImg)
        {
            HObject h_emtpy;
            HOperatorSet.GenEmptyObj(out h_emtpy);
            try
            {
                HTuple hV;
                HOperatorSet.TestEqualObj(h_emtpy, hImg, out hV);
                h_emtpy.Dispose();
                if (hV == 1 || hImg == null) { log.Error($"{Name}传入图像为空"); return false; }
                else
                {
                    this.ho_Image_Noglue.Dispose();
                    this.ho_Image_Noglue = hImg.Clone();
                    FitImage(ho_Image_Noglue);
                    HOperatorSet.ClearWindow(hv_WindowHandle);
                    HOperatorSet.DispObj(ho_Image_Noglue, hv_WindowHandle);
                    viewController.addIconicVar(ho_Image_Noglue);
                    viewController.repaint();

                    return true;
                }
            }
            catch (Exception) { return false; }
        }
        /// <summary>
        /// 适应图像
        /// </summary>
        /// <param name="image"></param>
        public void FitImage(HObject image)
        {
            try
            {
                if (image != null)
                {
                    double ratio_win, ratio_img;
                    HOperatorSet.GetImageSize(image, out hv_Width, out hv_Height);
                    ratio_win = (double)winWidth / (double)winHeight;
                    ratio_img = (double)hv_Width / (double)hv_Height;
                    int _beginRow, _beginCol, _endRow, _endCol;

                    if(ratio_win>=ratio_img)
                    {
                        _beginRow = 0;
                        _endRow = (int)hv_Height.D - 1;
                        _beginCol = (int)(-hv_Width.D * (ratio_win / ratio_img - 1d) / 2d);
                        _endCol = (int)(hv_Width.D + hv_Width.D * (ratio_win / ratio_img - 1d) / 2d);
                    }
                    else
                    {
                        _beginCol = 0;
                        _endCol = (int)hv_Width.D - 1;
                        _beginRow = (int)(-hv_Height.D * (ratio_img / ratio_win - 1d) / 2d);
                        _endRow = (int)(hv_Height.D + hv_Height.D * (ratio_img / ratio_win - 1d) / 2d);
                    }
                    hv_WindowHandle.SetPart(_beginRow, _beginCol, _endRow, _endCol);
                }
            }
            catch(Exception ex) { }
        }
        public void DispObj(HObject hobj)
        {
            try { HOperatorSet.DispObj(hobj, hv_WindowHandle); }
            catch (Exception) { }
        }

        /// <summary>
        /// 显示信息在控件上
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="Rowcoord"></param>
        /// <param name="Colcoord"></param>
        /// <param name="bRed"></param>
        public void DispMsg(string msg, int Rowcoord, int Colcoord, bool bRed)
        {
            if (bRed) disp_message(hv_WindowHandle, (HTuple)msg, "window", Rowcoord, Colcoord, "red", "true");
            else disp_message(hv_WindowHandle, (HTuple)msg, "window", Rowcoord, Colcoord, "green", "true");
        }

        private void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            HTuple hTuple = new HTuple();
            HTuple hTuple2 = new HTuple();
            HTuple hTuple3 = new HTuple();
            HTuple t = new HTuple();
            HTuple t2 = new HTuple();
            HTuple hTuple4 = new HTuple();
            HTuple hTuple5 = new HTuple();
            HTuple hTuple6 = new HTuple();
            HTuple hTuple7 = new HTuple();
            HTuple t3 = new HTuple();
            HTuple hTuple8 = new HTuple();
            HTuple t4 = new HTuple();
            HTuple t5 = new HTuple();
            HTuple hTuple9 = new HTuple();
            HTuple hTuple10 = new HTuple();
            HTuple mode = new HTuple();
            HTuple tuple = new HTuple();
            HTuple hTuple11 = new HTuple();
            HTuple hTuple12 = hv_Color.Clone();
            HTuple hTuple13 = hv_Column.Clone();
            HTuple hTuple14 = hv_Row.Clone();
            HTuple hTuple15 = hv_String.Clone();
            HTuple red;
            HTuple green;
            HTuple blue;
            HOperatorSet.GetRgb(hv_WindowHandle, out red, out green, out blue);
            HTuple hTuple16;
            HTuple hTuple17;
            HTuple hTuple18;
            HTuple hTuple19;
            HOperatorSet.GetPart(hv_WindowHandle, out hTuple16, out hTuple17, out hTuple18, out hTuple19);
            HTuple hTuple20;
            HTuple hTuple21;
            HTuple hTuple22;
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hTuple20, out hTuple21, out hTuple, out hTuple22);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hTuple22 - 1, hTuple - 1);
            if (new HTuple(hTuple14.TupleEqual(-1)) != 0)
            {
                hTuple14 = 12;
            }
            if (new HTuple(hTuple13.TupleEqual(-1)) != 0)
            {
                hTuple13 = 12;
            }
            if (new HTuple(hTuple12.TupleEqual(new HTuple())) != 0)
            {
                hTuple12 = "";
            }
            hTuple15 = ("" + hTuple15 + "").TupleSplit("\n");
            HTuple hTuple23;
            HTuple hTuple24;
            HTuple hTuple25;
            HTuple t6;
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hTuple23, out hTuple24, out hTuple25, out t6);
            if (new HTuple(hv_CoordSystem.TupleEqual("window")) != 0)
            {
                hTuple2 = hTuple14.Clone();
                hTuple3 = hTuple13.Clone();
            }
            else
            {
                t = 1.0 * hTuple22 / (hTuple18 - hTuple16 + 1);
                t2 = 1.0 * hTuple / (hTuple19 - hTuple17 + 1);
                hTuple2 = (hTuple14 - hTuple16 + 0.5) * t;
                hTuple3 = (hTuple13 - hTuple17 + 0.5) * t2;
            }
            if (new HTuple(hv_Box.TupleEqual("true")) != 0)
            {
                hTuple15 = " " + hTuple15 + " ";
                hTuple4 = new HTuple();
                hTuple5 = 0;
                while (hTuple5 <= new HTuple(hTuple15.TupleLength()) - 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hTuple15.TupleSelect(hTuple5), out hTuple6, out hTuple7, out t3, out hTuple8);
                    hTuple4 = hTuple4.TupleConcat(t3);
                    hTuple5++;
                }
                t4 = t6 * new HTuple(hTuple15.TupleLength());
                t5 = new HTuple(0).TupleConcat(hTuple4).TupleMax();
                hTuple9 = hTuple2 + t4;
                hTuple10 = hTuple3 + t5;
                HOperatorSet.GetDraw(hv_WindowHandle, out mode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hTuple2 + 3, hTuple3 + 3, hTuple9 + 3, hTuple10 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hTuple2, hTuple3, hTuple9, hTuple10);
                HOperatorSet.SetDraw(hv_WindowHandle, mode);
            }
            else if (new HTuple(hv_Box.TupleNotEqual("false")) != 0)
            {
                tuple = "Wrong value of control parameter Box";
                throw new HalconException(tuple);
            }
            hTuple5 = 0;
            while (hTuple5 <= new HTuple(hTuple15.TupleLength()) - 1)
            {
                hTuple11 = hTuple12.TupleSelect(hTuple5 % new HTuple(hTuple12.TupleLength()));
                if (new HTuple(hTuple11.TupleNotEqual("")).TupleAnd(new HTuple(hTuple11.TupleNotEqual("auto"))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hTuple11);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, red, green, blue);
                }
                hTuple14 = hTuple2 + t6 * hTuple5;
                HOperatorSet.SetTposition(hv_WindowHandle, hTuple14, hTuple3);
                HOperatorSet.WriteString(hv_WindowHandle, hTuple15.TupleSelect(hTuple5));
                hTuple5++;
            }
            HOperatorSet.SetRgb(hv_WindowHandle, red, green, blue);
            HOperatorSet.SetPart(hv_WindowHandle, hTuple16, hTuple17, hTuple18, hTuple19);
        }
        /// <summary>
        /// 读入图片 本地盘
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public bool ReadImg(string sPath)
        {
            if (sPath == "")
            {
                log.Error($"{Name}未初始化控件或图像路径为空");
                return false;
            }
            try
            {
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, sPath);
                FitImage(ho_Image);
                //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                //str_imgSize = string.Format("{0}X{1}", hv_Width, hv_Height);
                //hv_WindowHandle.SetPart((HTuple)0, (HTuple)0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                viewController.addIconicVar(ho_Image);
                viewController.repaint();
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{Name}{ex}");
                return false;
            }

        }

        /// <summary>
        /// 读入图片 本地盘
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public bool ReadImg_Noglue(string sPath)
        {
            if (sPath == "")
            {
                log.Error($"{Name}未初始化控件或图像路径为空");
                return false;
            }
            try
            {
                ho_Image_Noglue.Dispose();
                HOperatorSet.ReadImage(out ho_Image_Noglue, sPath);
                FitImage(ho_Image_Noglue);
                //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                //str_imgSize = string.Format("{0}X{1}", hv_Width, hv_Height);
                //hv_WindowHandle.SetPart((HTuple)0, (HTuple)0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.DispObj(ho_Image_Noglue, hv_WindowHandle);
                viewController.addIconicVar(ho_Image_Noglue);
                viewController.repaint();
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{Name}{ex}");
                return false;
            }

        }
        /// <summary>
        /// 保存图片 直接保存
        /// </summary>
        /// <param name="sSaveName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool SaveImg(string sSaveName, string filter)
        {
            try
            {
                HOperatorSet.WriteImage((HObject)this.ho_Image, ("jpg"), (0), (sSaveName));
            }
            catch (Exception ex)
            {
                log.Error($"{Name}保存图像时图像为空");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 保存图片 直接保存
        /// </summary>
        /// <param name="sSaveName"></param>
        /// <returns></returns>
        public bool SaveImg(string sSaveName)
        {
            try
            {
                HOperatorSet.WriteImage((HObject)this.ho_Image, ("jpg"), (0), (sSaveName));
            }
            catch (Exception ex)
            {
                log.Error($"{Name}保存图像时图像为空");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 窗口合适大小
        /// </summary>
        public void ZoomToFit()
        {
            try
            {
                viewController.resetAll();
                viewController.repaint();
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// 保存图片 窗口保存
        /// </summary>
        /// <param name="sSaveName"></param>
        /// <returns></returns>
        public bool SaveImgWindow(string sSaveName)
        {
            HObject himg;
            HOperatorSet.GenEmptyObj(out himg);
            try
            {
                HOperatorSet.DumpWindowImage(out himg, hv_WindowHandle);
                HOperatorSet.WriteImage(himg, ("jpg"), (0), (sSaveName));
                himg.Dispose();
            }
            catch (Exception ex)
            {
                himg.Dispose();
                log.Error($"{Name}保存图像时图像为空");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判定图像为空
        /// </summary>
        /// <returns></returns>
        public bool JudgeImg()
        {
            if (ho_Image == null)
                HOperatorSet.GenEmptyObj(out ho_Image);
            HObject h_emtpy;
            HOperatorSet.GenEmptyObj(out h_emtpy);
            HTuple hV;
            HOperatorSet.TestEqualObj(h_emtpy, ho_Image, out hV);
            h_emtpy.Dispose();
            if (hV == 1) { log.Error($"{Name}传入图像为空"); return false; }
            else return true;
        }
        /// <summary>
        /// 判定图像为空
        /// </summary>
        /// <returns></returns>
        public bool JudgeImg(HObject img)
        {
            if (img == null)
                HOperatorSet.GenEmptyObj(out img);
            HObject h_emtpy;
            HOperatorSet.GenEmptyObj(out h_emtpy);
            HTuple hV;
            HOperatorSet.TestEqualObj(h_emtpy, img, out hV);
            h_emtpy.Dispose();
            if (hV == 1) { log.Error($"{Name}传入图像为空"); return false; }
            else return true;
        }
        /// <summary>
        /// 图像增强
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="mult"></param>
        /// <param name="add"></param>
        /// <param name="OutImage"></param>
        /// <returns></returns>
        public bool ScaleImage(HObject ho_InputImage, HTuple mult, HTuple add, out HObject OutImage)
        {
            OutImage = null;
            if (!ho_InputImage.IsInitialized()) return false;
            HOperatorSet.GenEmptyObj(out OutImage);
            try
            {
                HOperatorSet.ScaleImage(ho_InputImage, out OutImage, mult, add);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 阈值分割
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="MinGray">最小阈值</param>
        /// <param name="MaxGray">最大阈值</param>
        /// <param name="Auto">直方图调节(0--true,1--false)</param>
        /// <param name="TarGray">期望值</param>
        /// <param name="OutImage">输出图像</param>
        public bool Threshold(HObject ho_InputImage, HTuple MinGray, HTuple MaxGray, bool Auto, HTuple TarGray, out HObject OutImage)
        {
            OutImage = null;
            if (!ho_InputImage.IsInitialized()) return false;
            if (MaxGray < MinGray)
            {
                log.Error($"{Name}最大阈值、最小阈值输入有误！");
                return false;
            }
            HTuple hAbsoluteHisto, hRelativeHisto;
            HTuple hMin, hMax;
            HTuple hWidth, hHeight;
            HObject hv_Region;
            HOperatorSet.GenEmptyObj(out OutImage);
            HOperatorSet.GenEmptyObj(out hv_Region);
            HOperatorSet.GetImageSize(ho_InputImage, out hWidth, out hHeight);
            hMin = MinGray;
            hMax = MaxGray;
            try
            {
                if (Auto)
                {
                    HOperatorSet.GrayHisto(ho_InputImage, ho_InputImage, out hAbsoluteHisto, out hRelativeHisto);
                    OutImage.Dispose();
                    HOperatorSet.GenRegionHisto(out OutImage, hAbsoluteHisto, 255, 255, 1);
                    HOperatorSet.HistoToThresh(hAbsoluteHisto, 10, out MinGray, out MaxGray);
                    for (int i = 0; i < MinGray.TupleNumber(); i++)
                    {
                        if (MinGray[i] <= TarGray)
                        {
                            hMin = MinGray[i];
                            break;
                        }
                    }
                    for (int j = 0; j < MaxGray.TupleNumber(); j++)
                    {
                        if (MaxGray[j] >= TarGray)
                        {
                            hMax = MaxGray[j];
                            break;
                        }
                    }
                }
                hv_Region.Dispose();
                HOperatorSet.Threshold(ho_InputImage, out hv_Region, hMin, hMax);
                OutImage.Dispose();
                HOperatorSet.RegionToBin(hv_Region, out OutImage, 255, 0, hWidth, hHeight);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                hv_Region.Dispose();
            }
        }

        /// <summary>
        /// 找圆中心
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="dbVisSetting">找圆参数</param>
        /// <param name="IsDisplay">是否显示结果</param>
        /// <param name="ret">返回结果</param>
        /// <returns></returns>
        public bool FindCircleMetrology(HObject ho_InputImage, FitCircleType dbVisSetting, bool IsDisplay, out RetFitCircle ret)
        {
            ret.CenterRow = 0;
            ret.CenterCol = 0;
            ret.Radius = 0;

            if (!ho_InputImage.IsInitialized()) return false;

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
            HTuple t1, t2;
            HOperatorSet.CountSeconds(out t1);
            try
            {
                log.Debug($"找圆参数 ROI中心X{dbVisSetting.CenterCol}Y{dbVisSetting.CenterRow} 测量卡尺半长{dbVisSetting.CalliperLength} 测量卡尺半宽{dbVisSetting.CalliperWidth}");
                log.Debug($"找圆参数 高斯{dbVisSetting.Sigma}Y{dbVisSetting.CenterRow} 边缘灰度{dbVisSetting.Threshold} 边缘极性{dbVisSetting.Transition} 选点方式{dbVisSetting.Select} 最小分数{dbVisSetting.MinScore}");
                HOperatorSet.GetImageSize(ho_InputImage, out hv_Width, out hv_Height);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle, dbVisSetting.CenterRow, dbVisSetting.CenterCol,
                    dbVisSetting.Radius, dbVisSetting.CalliperLength, dbVisSetting.CalliperWidth, dbVisSetting.Sigma, dbVisSetting.Threshold, "distance_threshold", 1,
                    out hv_MetrologyCircleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_transition", dbVisSetting.Transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "num_measures", dbVisSetting.CalliperNum);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "num_instances", dbVisSetting.TargetNum);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_interpolation", "nearest_neighbor");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "measure_select", dbVisSetting.Select);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_MetrologyCircleIndices, "min_score", dbVisSetting.MinScore);
                HOperatorSet.ApplyMetrologyModel(ho_InputImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_MetrologyCircleIndices, "all", "result_type", "all_param", out hv_CircleParameter);
                ho_Contours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contours, hv_MetrologyHandle, "all", "all", 1.5);
                ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", "all", out hv_Row1, out hv_Column1);

                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 6, 0.785398);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                if (IsDisplay)
                {
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
                }
                HOperatorSet.FitCircleContourXld(ho_Contours, "algebraic", -1, 0, 0, 3, 2, out row, out col, out radius, out startPhi, out endPhi, out pointOrder);
                hv_num = row.TupleLength();
                int num = hv_num;
                int i = 0;
                for (i = 0; i < hv_num; i++)
                {
                    if (radius[i] > dbVisSetting.Radius - hv_CircleRadiusTolerance && radius[i] < dbVisSetting.Radius + hv_CircleRadiusTolerance)
                    {
                        ret.CenterRow = row[i];
                        ret.CenterCol = col[i];
                        ret.Radius = radius[i];
                        if (IsDisplay)
                        {
                            HOperatorSet.SetColor(hv_WindowHandle, "blue");
                            HOperatorSet.DispLine(hv_WindowHandle, ret.CenterRow, ret.CenterCol - 30, ret.CenterRow, ret.CenterCol + 30);
                            HOperatorSet.DispLine(hv_WindowHandle, ret.CenterRow - 30, ret.CenterCol, ret.CenterRow + 30, ret.CenterCol);
                        }
                        log.Debug($"找圆成功 X{ret.CenterCol} Y{ret.CenterRow}");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Debug("找圆失败");
                return false;
            }
            finally
            {
                ho_Contours.Dispose();
                ho_Contour.Dispose();
                ho_Cross.Dispose();
                HOperatorSet.CountSeconds(out t2);
                circletime = Math.Round((double)((t2 - t1) * 1000));
            }
        }
        /// <summary>
        ///  图像Blob分析
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="CentryRow">灰度ROI_X</param>
        /// <param name="CentryCol">灰度ROI_Y</param>
        /// <param name="InerR">灰度ROI内径</param>
        /// <param name="OutR">灰度ROI外径</param>
        /// <param name="MinTolerance">最小图像公差</param> 
        /// <param name="MaxTolerance">最大图像公差</param> 
        /// <param name="RegionGray">输出灰度值</param> 
        /// <param name="IsDisplay">是否显示结果</param> 
        /// <returns></returns>
        public bool JudgeDirectorImg(HObject ho_InputImage, double CentryRow, double CentryCol, double InerR, double OutR,
            double MinTolerance, double MaxTolerance, bool IsDisplay, out double RegionGray)
        {
            RegionGray = 0;
            if (!ho_InputImage.IsInitialized()) return false;
            HTuple hv_mean, hv_deviation;
            HTuple Area, Row, Col;
            HObject ho_Circle, ho_Circle1, ho_MaskRegion;
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_MaskRegion);
            HTuple t1, t2;
            HOperatorSet.CountSeconds(out t1);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                if (OutR >= InerR)
                {
                    HOperatorSet.GenCircle(out ho_Circle, CentryRow, CentryCol, OutR);
                    HOperatorSet.GenCircle(out ho_Circle1, CentryRow, CentryCol, InerR);
                }
                else
                {
                    HOperatorSet.GenCircle(out ho_Circle1, CentryRow, CentryCol, OutR);
                    HOperatorSet.GenCircle(out ho_Circle, CentryRow, CentryCol, InerR);
                }
                HOperatorSet.Difference(ho_Circle, ho_Circle1, out ho_MaskRegion);
                HOperatorSet.AreaCenter(ho_MaskRegion, out Area, out Row, out Col);
                if (0 == Area) return false;
                HOperatorSet.Intensity(ho_MaskRegion, ho_InputImage, out hv_mean, out hv_deviation);
                RegionGray = hv_mean;
                if (RegionGray >= MinTolerance && RegionGray < MaxTolerance)
                {
                    if (IsDisplay) HOperatorSet.DispObj(ho_MaskRegion, hv_WindowHandle);
                    return true;
                }
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
                HOperatorSet.CountSeconds(out t2);
                graytime = Math.Round((double)((t2 - t1) * 1000));
            }
        }
        public bool JudgePointImg(HObject ho_InputImage, double CentryRow, double CentryCol, double R,
            double GrayMin, double GrayMax, bool IsDisplay, out double GrayArea)
        {
            GrayArea = 0;
            if (!ho_InputImage.IsInitialized()) return false;
            HTuple Area, Row, Col;
            HObject ho_Circle,ho_MaskRegion,ho_OutRegion;
            HObject connectReg, selectObj;
            HTuple hv_num = 0;
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_MaskRegion);
            HOperatorSet.GenEmptyObj(out ho_OutRegion);
            HOperatorSet.GenEmptyObj(out connectReg);
            HOperatorSet.GenEmptyObj(out selectObj);
            HTuple t1, t2;
            HOperatorSet.CountSeconds(out t1);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandle, "red");
                ho_Circle.Dispose();
                HOperatorSet.GenCircle(out ho_Circle, CentryRow, CentryCol, R);
                HOperatorSet.AreaCenter(ho_Circle, out Area, out Row, out Col);
                if (0 == Area) return false;
                ho_MaskRegion.Dispose();
                HOperatorSet.ReduceDomain(ho_InputImage, ho_Circle, out ho_MaskRegion);
                ho_OutRegion.Dispose();
                HOperatorSet.Threshold(ho_MaskRegion, out ho_OutRegion,new HTuple(GrayMin), new HTuple(GrayMax));
                if (IsDisplay) HOperatorSet.DispObj(ho_OutRegion, hv_WindowHandle);
                connectReg.Dispose();
                HOperatorSet.Connection(ho_OutRegion, out connectReg);
                HOperatorSet.CountObj(connectReg, out hv_num);
                if(hv_num>0)
                {
                    for(var i=1;i<=hv_num;i++)
                    {
                        selectObj.Dispose();
                        HOperatorSet.SelectObj(connectReg, out selectObj, i);
                        HTuple selectArea, selectRow, selectCol;
                        HOperatorSet.AreaCenter(selectObj, out selectArea, out selectRow, out selectCol);
                        if (selectArea > 10) GrayArea += selectArea;
                    }
                }
                return GrayArea <= 0 ? true:false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                ho_Circle.Dispose();
                connectReg.Dispose();
                selectObj.Dispose();
                ho_MaskRegion.Dispose();
                ho_OutRegion.Dispose();
                HOperatorSet.CountSeconds(out t2);
                graytime = Math.Round((double)((t2 - t1) * 1000));
            }
        }
        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="param">模板参数</param>
        /// <param name="IsDisplay">是否显示结果</param>
        /// <param name="ret">返回数据</param>
        /// <returns></returns>
        public bool FindShapeModel(HObject ho_InputImage, ShapeModelType param, bool IsDisplay, out RetShapeModel ret)
        {
            ret.CenterRow = 0;
            ret.CenterCol = 0;
            ret.Angle = 0;
            ret.Score = 0;
            HTuple hv_Row, hv_Column, hv_Angle1, hv_Score;
            HOperatorSet.SetSystem("border_shape_models", "false");
            HTuple t1, t2;
            HOperatorSet.CountSeconds(out t1);
            try
            {
                log.Debug($"模板参数 最小分数{param.MinScore} 匹配个数{param.NumMatches} 重叠度{param.MaxOverlap} 精度控制{ param.SubPixel} 图像金字塔{param.NumLevels} 贪婪度{param.Greediness}");
                HOperatorSet.FindShapeModel(ho_InputImage, ModelId, (new HTuple(param.StartAngle)).TupleRad(), (new HTuple(param.EndAngle)).TupleRad(),
                    param.MinScore, param.NumMatches, param.MaxOverlap, param.SubPixel, param.NumLevels, param.Greediness,
                    out hv_Row, out hv_Column, out hv_Angle1, out hv_Score);
                if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                {
                    ret.CenterRow = hv_Row[0].D;
                    ret.CenterCol = hv_Column[0].D;
                    ret.Angle = hv_Angle1[0].D;
                    ret.Score = hv_Score[0].D;
                    if (IsDisplay)
                    {
                        HOperatorSet.DispCross(hv_WindowHandle, ret.CenterRow, ret.CenterCol,60,0.78);
                        HOperatorSet.SetColor(hv_WindowHandle, "magenta");
                        HDevelopExport.dev_display_shape_matching_results(hv_WindowHandle, ModelId, "green", hv_Row, hv_Column, hv_Angle1, 1, 1, 0);
                    }
                    log.Debug($"模板中心 X{ret.CenterCol} Y{ret.CenterRow}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Debug("模板匹配失败");
                return false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out t2);
                modeltime = Math.Round((double)((t2 - t1) * 1000), 2);
            }
        }
        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="ho_InputImage">输入图像</param>
        /// <param name="ho_Region">输入ROI</param>
        /// <param name="param">创建模板参数</param>
        /// <param name="ModelID">模板ID</param>
        public void CreateShapeModel(HObject ho_InputImage, HObject ho_Region, ShapeModelType param, out HTuple ModelID)
        {
            ModelID = null;
            HOperatorSet.ClearAllShapeModels();
            HObject ho_ImageReduced, ho_ModelContours, ho_ContoursAffinTrans;
            HTuple hv_Area, hv_Row4, hv_Column4, hv_HomMat2DIdentity, hv_HomMat2DTranslate;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_InputImage, ho_Region, out ho_ImageReduced);
                HOperatorSet.AreaCenter(ho_Region, out hv_Area, out hv_Row4, out hv_Column4);
                HOperatorSet.SetSystem("border_shape_models", "false");
               
                HOperatorSet.CreateShapeModel(ho_ImageReduced, param.NumLevels, (new HTuple(param.StartAngle)).TupleRad(), (new HTuple(param.EndAngle)).TupleRad(),
                param.AngleStep, param.Optimization, param.Metric, param.Contrast, param.MinContrast, out ModelID);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, ModelID, 1);
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row4, hv_Column4, out hv_HomMat2DTranslate);
                ho_ContoursAffinTrans.Dispose();
                HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans, hv_HomMat2DTranslate);
                HOperatorSet.ClearWindow(hv_WindowHandle);
                HOperatorSet.DispObj(ho_ImageReduced, hv_WindowHandle);
                HOperatorSet.DispObj(ho_ContoursAffinTrans, hv_WindowHandle);
            }
            catch (Exception ex) { }
            finally
            {
                ho_ImageReduced.Dispose();
                ho_ModelContours.Dispose();
                ho_ContoursAffinTrans.Dispose();
            }
        }
        /// <summary>
        /// 角度寻找
        /// </summary>
        /// <param name="ho_InputImage"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <param name="maxGray"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <param name="IsDisplay">是否显示结果</param>
        /// <param name="ret"></param>
        /// <param name="Area"></param>
        /// <returns></returns>
        public bool FindAngle(HObject ho_InputImage, double row, double col, double R1, double R2,int minGray, int maxGray,
            double minValue, double maxValue, int minArea, int maxArea, bool IsDisplay, out MatchResult ret, out HTuple Area)
        {
            ret.hv_ColumnCheck = 0;
            ret.hv_RowCheck = 0;
            ret.hv_Score = 0;
            ret.hv_AngleCheck = 0;
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
            HTuple t1, t2;
            HOperatorSet.CountSeconds(out t1);
            try
            {
                log.Debug($"找角度参数 ROI中心 X{col}Y{row} R1{R1} R2{R2} 剪口阈值小{minGray} 大{maxGray}");
                log.Debug($"找角度参数 剪口长度 小{minValue}大{maxValue} 面积小{minArea} 大{maxArea} ");
                ho_Circle.Dispose();
                ho_Circle1.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced.Dispose();
                if (R2 >= R1)
                {
                    HOperatorSet.GenCircle(out ho_Circle, row, col, R2);
                    HOperatorSet.GenCircle(out ho_Circle1, row, col, R1);
                }
                else
                {
                    HOperatorSet.GenCircle(out ho_Circle1, row, col, R2);
                    HOperatorSet.GenCircle(out ho_Circle, row, col, R1);
                }
                HOperatorSet.Difference(ho_Circle, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.ReduceDomain(ho_InputImage, ho_RegionDifference, out ho_ImageReduced);
                //需调参数2  60 看现场图像 60可为对比度
                ho_Region.Dispose();
                if(minGray<maxGray)
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, minGray, maxGray);
                else if(minGray > maxGray)
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, maxGray, minGray);
                else
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 0, maxGray);
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
                ret.hv_Score = hv_Value;
                //限定长度 防止找错 也可其它方法
                //需调参数4  Value>50and Value<100
                if ((int)((new HTuple(hv_Value.TupleGreater(minValue))).TupleAnd(new HTuple(hv_Value.TupleLess(maxValue)))) != 0)
                {
                    //区域重心
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out Area, out ret.hv_RowCheck, out ret.hv_ColumnCheck);
                    double Row = ret.hv_RowCheck - row;
                    double Column = ret.hv_ColumnCheck - col;
                    var angle = (Math.Atan2(Row, Column) / Math.PI) * -180;
                    ret.hv_AngleCheck = Math.Round(angle, 2);
                    log.Debug($"找角度成功 {angle}");
                    if (IsDisplay)
                    {
                        ho_RefinedPos.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_RefinedPos, row, col, 25, 0);
                        HOperatorSet.SetColor(hv_WindowHandle, "green");
                        HOperatorSet.DispObj(ho_RefinedPos, hv_WindowHandle);
                        HOperatorSet.DispArrow(hv_WindowHandle, row, col, ret.hv_RowCheck, ret.hv_ColumnCheck, 5);
                        HOperatorSet.DispObj(ho_SelectedRegions, hv_WindowHandle);
                    }
                    if (Area > minArea && Area < maxArea)
                        return true;
                    else return false;
                }
                else return false;
            }
            catch (HalconException Hdex)
            {
                log.Debug("找角度失败");
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
                HOperatorSet.CountSeconds(out t2);
                polartime = Math.Round((double)((t2 - t1) * 1000), 2);
            }
        }

        #region 保存、读取模板
        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="sModelPath"></param>
        /// <returns></returns>
        public bool WriteShapeModel(string sModelPath)
        {
            try
            {
                if (sModelPath == "") return false;
                HOperatorSet.WriteShapeModel(ModelId, sModelPath);
                log.Debug($"保存{Name}找圆模板成功");
                return true;
            }
            catch (Exception ex) { log.Error($"保存{Name}找圆模板失败"); return false; }
        }

        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="sMoelPath"></param>
        /// <returns></returns>
        public bool ReadShapeModel(string sMoelPath)
        {
            if (sMoelPath == "") return false;
            try
            {
                HOperatorSet.ReadShapeModel(sMoelPath, out ModelId);
                log.Debug($"读取{Name}找圆模板成功");
                return true;
            }
            catch (Exception) { log.Error($"读取{Name}找圆模板失败"); return false; }
        }
        #endregion
        public bool GlueVisionAction_old20201019(HObject ho_ImageT, HObject ho_ImageF, double CentrerGlueX , double CentrerGlueY,DbVisSetting dbVisSetting)
        {
            bool is_qualified = false;


            HTuple t1, t2;
         
            try
            {
                HOperatorSet.CountSeconds(out t1);
             

                #region// 声明图像变量
                HObject ho_RegionDifference, ho_RegionOpening2, ho_ConnectedRegions1, ho_SelectedRegions1;
                HObject ho_RegionErosion, ho_Domain, ho_RegionDifference1;
                HObject  ho_TransImage;
                HObject ho_ImageSub, ho_ImageGauss, ho_Regions_R, ho_RegionClosing;
                HObject ho_RegionOpening1, ho_RegionUnion, ho_Rectangle;
                HObject ho_RegionOpening, ho_Regionc;
                HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionFillUp1;
                HObject ho_Contours1, ho_Circle_mask_S = null;
                HObject ho_target_ring = null, ho_target_ring_scale = null;
                HObject ho_Regions = null, ho_RegionFillUp = null, ho_RegionTrans = null;
                HObject ho_Contours = null, ho_glueOutterContour = null, ho_glueInnerContour = null;
                HObject ho_Contour_I_sap = null, ho_Contour_O_sap = null, ho_ContCircle_outter = null;
                HObject ho_ContCircle_Inner = null, Regionoping = null;
                HObject SmoothedGlueOutterContour = null, SmoothedGlueInnerContour = null;
                #endregion

                #region//初始化图像变量
                HOperatorSet.GenEmptyObj(out ho_TransImage);
                HOperatorSet.GenEmptyObj(out ho_ImageSub);
                HOperatorSet.GenEmptyObj(out ho_ImageGauss);
                HOperatorSet.GenEmptyObj(out ho_Regions_R);
                HOperatorSet.GenEmptyObj(out ho_RegionClosing);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
                HOperatorSet.GenEmptyObj(out ho_RegionUnion);
                HOperatorSet.GenEmptyObj(out ho_Rectangle);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening);
                HOperatorSet.GenEmptyObj(out ho_Regionc);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp1);
                HOperatorSet.GenEmptyObj(out ho_RegionErosion);
                HOperatorSet.GenEmptyObj(out ho_Contours1);
                HOperatorSet.GenEmptyObj(out ho_Circle_mask_S);
                HOperatorSet.GenEmptyObj(out ho_target_ring);
                HOperatorSet.GenEmptyObj(out ho_target_ring_scale);
                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_RegionTrans);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_glueOutterContour);
                HOperatorSet.GenEmptyObj(out ho_glueInnerContour);
                HOperatorSet.GenEmptyObj(out ho_Contour_I_sap);
                HOperatorSet.GenEmptyObj(out ho_Contour_O_sap);
                HOperatorSet.GenEmptyObj(out ho_ContCircle_outter);
                HOperatorSet.GenEmptyObj(out ho_ContCircle_Inner);
                HOperatorSet.GenEmptyObj(out Regionoping);
                HOperatorSet.GenEmptyObj(out ho_Domain);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);

                #endregion

                #region// 初始化控制变量 
                // Local control variables 

                HTuple hv_Width = null, hv_Height = null;
                HTuple hv_RowsF = null, hv_ColsF = null, hv_RowsT = null;
                HTuple hv_ColsT = null, hv_HomMat2D = null, hv_Points1 = null;
                HTuple hv_Points2 = null, hv_Number1 = null, hv_Number2 = null;
                HTuple hv_Row = new HTuple();
                HTuple hv_Column = new HTuple(), hv_Radius = new HTuple();
                HTuple hv_StartPhi = new HTuple(), hv_EndPhi = new HTuple();
                HTuple hv_PointOrder = new HTuple();
                //HTuple hv_glueInnerCircle = new HTuple(), hv_glueWidth = new HTuple();
                HTuple hv_glueOutterCircle = new HTuple();
                HTuple hv_glueCenter = new HTuple(), hv_Row2 = new HTuple();
                HTuple hv_Col1 = new HTuple(), hv_Row3 = new HTuple();
                HTuple hv_Col2 = new HTuple(), hv_Row_Inner_sampling = new HTuple();
                HTuple hv_Col_Inner_sampling = new HTuple(), hv_k = new HTuple();
                HTuple hv_i = new HTuple(), hv_Row_Outter_sampling = new HTuple();
                HTuple hv_Col_Outter_sampling = new HTuple(), hv_DistanceMin = new HTuple();
                HTuple hv_DistanceMax = new HTuple(), hv_glueWidthMin = new HTuple();
                HTuple hv_glueWidthMax = new HTuple(), hv_DistanceMin1 = new HTuple();
                HTuple hv_DistanceMax1 = new HTuple(), hv_Distance_inner_min = new HTuple();
                HTuple hv_Distance_inner_max = new HTuple(), hv_DistanceMin2 = new HTuple();
                HTuple hv_DistanceMax2 = new HTuple(), hv_Distance_outter_min = new HTuple();
                HTuple hv_Distance_outter_max = new HTuple(), hv_Row_Outter = new HTuple();
                HTuple hv_Column_Outter = new HTuple(), hv_Radius_Outter = new HTuple();
                HTuple hv_StartPhi1 = new HTuple(), hv_EndPhi1 = new HTuple();
                HTuple hv_PointOrder1 = new HTuple(), hv_DistanceMin_Outter = new HTuple();
                HTuple hv_DistanceMax_Outter = new HTuple(), hv_Ddistance_Outter = new HTuple();
                HTuple hv_Sdistance_Outter = new HTuple(), hv_Row_Inner = new HTuple();
                HTuple hv_Column_Inner = new HTuple(), hv_Radius_Inner = new HTuple();
                HTuple hv_StartPhi2 = new HTuple(), hv_EndPhi2 = new HTuple();
                HTuple hv_PointOrder2 = new HTuple(), hv_DistanceMin_Inner = new HTuple();
                HTuple hv_DistanceMax_Inner = new HTuple(), hv_Ddistance_Inner = new HTuple();
                HTuple hv_Sdistance_Inner = new HTuple(), hv_Row_glue = new HTuple();
                HTuple hv_Col_glue = new HTuple(), hv_Distance_offset = new HTuple();
                HTuple hv_Font = new HTuple();
                #endregion
                is_qualified = true;

                HObject img_old;
                HOperatorSet.GenEmptyObj(out img_old);
                HTuple htuple;
                //彩色变灰色
                HOperatorSet.CountChannels(ho_ImageT, out htuple);
                img_old.Dispose();
                if (htuple == 3)
                {
                    HOperatorSet.Rgb1ToGray(ho_ImageT, out img_old);
                    ho_ImageT.Dispose();
                    ho_ImageT = img_old.Clone();
                }


                //彩色变灰色
                HOperatorSet.CountChannels(ho_ImageF, out htuple);
                img_old.Dispose();
                if (htuple == 3)
                {
                    HOperatorSet.Rgb1ToGray(ho_ImageF, out img_old);
                    ho_ImageF.Dispose();
                    ho_ImageF = img_old.Clone();
                }

                //获取图像的尺寸信息
                HOperatorSet.GetImageSize(ho_ImageF, out hv_Width, out hv_Height);
                //*******************主逻辑
                #region//提取特征点,根据特征点求两张图之间的仿射矩阵：HomMat2D,并对齐图像
                HOperatorSet.PointsHarris(ho_ImageF, 3, 2, 0.08, 8000, out hv_RowsF, out hv_ColsF);
                HOperatorSet.PointsHarris(ho_ImageT, 3, 2, 0.08, 8000, out hv_RowsT, out hv_ColsT);

                //* 根据特征点求两张图之间的仿射矩阵：HomMat2D ssd
                HOperatorSet.ProjMatchPointsRansac(ho_ImageT, ho_ImageF, hv_RowsT, hv_ColsT,
                    hv_RowsF, hv_ColsF, "sad", 10, 0, 0, 100, 100, (new HTuple((new HTuple(-0.2)).TupleRad()
                    )).TupleConcat((new HTuple(0.2)).TupleRad()), 10, "gold_standard", 0.2, 42,
                    out hv_HomMat2D, out hv_Points1, out hv_Points2);
                //*将ImageT进行放射变换以对齐两张图片
                ho_TransImage.Dispose();
                HOperatorSet.ProjectiveTransImage(ho_ImageT, out ho_TransImage, hv_HomMat2D,
                    "bilinear", "false", "false");
                //ho_ImageT.Dispose();
                #endregion

                #region//提取胶圈轮廓
                //将对齐后点胶之后的图片减去点胶之前图片
                ho_ImageSub.Dispose();

                HOperatorSet.SubImage(ho_ImageF, ho_TransImage, out ho_ImageSub, 1.1, 120);

                //  HOperatorSet.WriteImage(ho_ImageSub, "jpg", 0, "D:\\img03.jpg");

                //**对相减得到的图片进行处理，提出胶圈轮廓
                //平滑图像
                ho_ImageGauss.Dispose();
                HOperatorSet.GaussFilter(ho_ImageSub, out ho_ImageGauss, 11);
                ho_ImageSub.Dispose();
                //*阈值分割，采用区域生长法，控制参数tol=3, region阈值500000
                ho_Regions_R.Dispose();
                HOperatorSet.Regiongrowing(ho_ImageGauss, out ho_Regions_R, 4, 4, dbVisSetting.tol, dbVisSetting.area);

                //对region进行开运算和闭运算，并合并region
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_Regions_R, out ho_RegionClosing, 11);
                ho_RegionOpening1.Dispose();
                HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening1, 11);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_RegionOpening1, out ho_RegionUnion);
                //提取胶圈区域region
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Rectangle, ho_RegionUnion, out ho_RegionDifference
                    );
                HOperatorSet.CountObj(ho_RegionDifference, out hv_Number1);
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 7);
                ho_Regionc.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionOpening, out ho_Regionc, 9);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_Regionc, out ho_ConnectedRegions1);

                ho_SelectedRegions1.Dispose();
                           
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1, "max_area",
                    70);
                ho_RegionFillUp1.Dispose();
                HOperatorSet.FillUpShape(ho_SelectedRegions1, out ho_RegionFillUp1, "area", 1,
                    200);
                ho_RegionErosion.Dispose();
                HOperatorSet.ErosionCircle(ho_RegionFillUp1, out ho_RegionErosion, 2.5);

                ho_Contours1.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionErosion, out ho_Contours1, "border_holes");
                HOperatorSet.CountObj(ho_Contours1, out hv_Number2);
                #endregion
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_Font);
                HOperatorSet.SetFont(hv_WindowHandle, (hv_Font.TupleSelect(0)) + "-Bold-13");

                #region//计算点胶区域内外圆
                //***********************
                //理论胶圈外圆
                hv_glueOutterCircle = dbVisSetting.GlueInnerCircle + dbVisSetting.GlueWidth;
                //理论胶圈圆心
                hv_glueCenter = new HTuple(2);
                hv_glueCenter[0] = CentrerGlueY;
                hv_glueCenter[1] = CentrerGlueX;
                HObject ho_Cross, ho_cir1, ho_cir2;
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_cir1);
                HOperatorSet.GenEmptyObj(out ho_cir2);
                //xmz  显示 内外圆区域
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.SetDraw(hv_WindowHandle, "margin"); ;
                ho_Cross.Dispose(); ;

                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_glueCenter[0], hv_glueCenter[1],
                       180, 0.785398);
                HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                HOperatorSet.DispObj(ho_Cross, hv_WindowHandle);
                ho_cir1.Dispose();
                HOperatorSet.GenCircle(out ho_cir1, hv_glueCenter[0], hv_glueCenter[1], dbVisSetting.GlueInnerCircle);
                HOperatorSet.DispObj(ho_cir1, hv_WindowHandle);
                ho_cir2.Dispose();
                HOperatorSet.GenCircle(out ho_cir2, hv_glueCenter[0], hv_glueCenter[1], hv_glueOutterCircle);
                HOperatorSet.DispObj(ho_cir2, hv_WindowHandle);
                //***********************
                #endregion

                #region//检测胶圈是否封闭
                if ((int)((new HTuple((new HTuple(hv_Number2.TupleEqual(2))).TupleAnd(new HTuple(hv_Number1.TupleEqual(
            1))))).TupleNot()) != 0)
                {
                    try
                    {
                        HOperatorSet.WriteImage(ho_ImageT, "jpg", 0, "C:\\img01.jpg");
                        HOperatorSet.WriteImage(ho_ImageF, "jpg", 0, "C:\\img02.jpg");
                    }
                    catch { }

                    is_qualified = false;
                    //HOperatorSet.DispObj(ho_ImageF, Window);
                    HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    HOperatorSet.DispObj(ho_Contours1, hv_WindowHandle);
                    HOperatorSet.DispText(hv_WindowHandle, "无胶或胶圈不完整！", "window",
                        15, 480, "red", "box", "false");
                    #region

                    ho_TransImage.Dispose();
                    ho_ImageSub.Dispose();
                    ho_ImageGauss.Dispose();
                    ho_Regions_R.Dispose();
                    ho_RegionClosing.Dispose();
                    ho_RegionOpening1.Dispose();
                    ho_RegionUnion.Dispose();
                    ho_Rectangle.Dispose();
                    ho_RegionDifference.Dispose();
                    ho_RegionOpening.Dispose();
                    ho_Regionc.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_SelectedRegions.Dispose();
                    ho_RegionFillUp1.Dispose();
                    ho_RegionErosion.Dispose();
                    ho_Contours1.Dispose();
                    ho_Circle_mask_S.Dispose();
                    ho_target_ring.Dispose();
                    ho_target_ring_scale.Dispose();
                    ho_Regions.Dispose();
                    ho_RegionFillUp.Dispose();
                    ho_RegionTrans.Dispose();
                    ho_Contours.Dispose();
                    ho_glueOutterContour.Dispose();
                    ho_glueInnerContour.Dispose();
                    ho_Contour_I_sap.Dispose();
                    ho_Contour_O_sap.Dispose();
                    ho_ContCircle_outter.Dispose();
                    ho_ContCircle_Inner.Dispose();
                    #endregion
                    HOperatorSet.CountSeconds(out t2);
                    circletime = Math.Round((double)((t2 - t1) * 1000));
                    return is_qualified;
                }
                #endregion

                #region//胶圈封闭则进一步检查其它指标是否合格
                else
                {

                    #region
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Regions_R, out ho_RegionClosing, dbVisSetting.kernel);
                    ho_RegionOpening1.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening1, 15);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionOpening1, out ho_RegionUnion);

                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_Rectangle, ho_RegionUnion, out ho_RegionDifference
                        );

                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 7);
                    ho_Regionc.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionOpening, out ho_Regionc, 9);

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regionc, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                        70);
                    ho_RegionFillUp1.Dispose();
                    HOperatorSet.FillUpShape(ho_SelectedRegions, out ho_RegionFillUp1, "area",
                        1, 20000);
                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionCircle(ho_RegionFillUp1, out ho_RegionErosion, 2.5);

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionErosion, out ho_Contours, "border_holes");
                    ho_glueOutterContour.Dispose();
                    HOperatorSet.SelectObj(ho_Contours, out ho_glueOutterContour, 1);
                    ho_glueInnerContour.Dispose();
                    HOperatorSet.SelectObj(ho_Contours, out ho_glueInnerContour, 2);
                    HOperatorSet.GetContourXld(ho_glueInnerContour, out hv_Row2, out hv_Col1);
                    HOperatorSet.GetContourXld(ho_glueOutterContour, out hv_Row3, out hv_Col2);
                    hv_Row_Inner_sampling = new HTuple();
                    hv_Col_Inner_sampling = new HTuple();
                    hv_k = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Row2.TupleLength())) - 1); hv_i = (int)hv_i + 50)
                    {
                        if (hv_Row_Inner_sampling == null)
                            hv_Row_Inner_sampling = new HTuple();
                        hv_Row_Inner_sampling[hv_k] = hv_Row2.TupleSelect(hv_i);
                        if (hv_Col_Inner_sampling == null)
                            hv_Col_Inner_sampling = new HTuple();
                        hv_Col_Inner_sampling[hv_k] = hv_Col1.TupleSelect(hv_i);
                        hv_k = hv_k + 1;
                    }
                    if (hv_Row_Inner_sampling == null)
                        hv_Row_Inner_sampling = new HTuple();
                    hv_Row_Inner_sampling[new HTuple(hv_Row_Inner_sampling.TupleLength())] = hv_Row_Inner_sampling.TupleSelect(
                        0);
                    if (hv_Col_Inner_sampling == null)
                        hv_Col_Inner_sampling = new HTuple();
                    hv_Col_Inner_sampling[new HTuple(hv_Col_Inner_sampling.TupleLength())] = hv_Col_Inner_sampling.TupleSelect(
                        0);
                    hv_Row_Outter_sampling = new HTuple();
                    hv_Col_Outter_sampling = new HTuple();
                    hv_k = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Row3.TupleLength())) - 1); hv_i = (int)hv_i + 50)
                    {
                        if (hv_Row_Outter_sampling == null)
                            hv_Row_Outter_sampling = new HTuple();
                        hv_Row_Outter_sampling[hv_k] = hv_Row3.TupleSelect(hv_i);
                        if (hv_Col_Outter_sampling == null)
                            hv_Col_Outter_sampling = new HTuple();
                        hv_Col_Outter_sampling[hv_k] = hv_Col2.TupleSelect(hv_i);
                        hv_k = hv_k + 1;
                    }
                    if (hv_Row_Outter_sampling == null)
                        hv_Row_Outter_sampling = new HTuple();
                    hv_Row_Outter_sampling[new HTuple(hv_Row_Outter_sampling.TupleLength())] = hv_Row_Outter_sampling.TupleSelect(
                        0);
                    if (hv_Col_Outter_sampling == null)
                        hv_Col_Outter_sampling = new HTuple();
                    hv_Col_Outter_sampling[new HTuple(hv_Col_Outter_sampling.TupleLength())] = hv_Col_Outter_sampling.TupleSelect(
                        0);
                    ho_Contour_I_sap.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour_I_sap, hv_Row_Inner_sampling,
                        hv_Col_Inner_sampling);
                    ho_Contour_O_sap.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour_O_sap, hv_Row_Outter_sampling,
                        hv_Col_Outter_sampling);
                    ho_glueInnerContour.Dispose();
                    ho_glueInnerContour = ho_Contour_I_sap.CopyObj(1, -1);
                    ho_glueOutterContour.Dispose();
                    ho_glueOutterContour = ho_Contour_O_sap.CopyObj(1, -1);
                    //*最大最小胶宽
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_Row2, hv_Col1, out hv_DistanceMin,
                        out hv_DistanceMax);
                    HOperatorSet.TupleMin(hv_DistanceMin, out hv_glueWidthMin);
                    HOperatorSet.TupleMax(hv_DistanceMin, out hv_glueWidthMax);
                    //*胶水内轮廓离边界的距离最大值最小值
                    HOperatorSet.DistancePc(ho_glueInnerContour, hv_glueCenter.TupleSelect(0),
                        hv_glueCenter.TupleSelect(1), out hv_DistanceMin1, out hv_DistanceMax1);
                    hv_Distance_inner_min = hv_DistanceMin1 - dbVisSetting.GlueInnerCircle;
                    hv_Distance_inner_max = hv_DistanceMax1 - dbVisSetting.GlueInnerCircle;
                    //*胶水外轮廓离边界的距离最大最小值
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_glueCenter.TupleSelect(0),
                        hv_glueCenter.TupleSelect(1), out hv_DistanceMin2, out hv_DistanceMax2);
                    hv_Distance_outter_min = hv_DistanceMin2 - hv_glueOutterCircle;
                    hv_Distance_outter_max = hv_DistanceMax2 - hv_glueOutterCircle;
                    //*用胶轮廓与拟合圆间的最大距离判断缺胶溢胶情况
                    HOperatorSet.FitCircleContourXld(ho_glueOutterContour, "algebraic", -1, 0,
                        0, 3, 2, out hv_Row_Outter, out hv_Column_Outter, out hv_Radius_Outter,
                        out hv_StartPhi1, out hv_EndPhi1, out hv_PointOrder1);
                    ho_ContCircle_outter.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle_outter, hv_Row_Outter, hv_Column_Outter,
                        hv_Radius_Outter, 0, 6.28318, "positive", 1);
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_Row_Outter, hv_Column_Outter,
                        out hv_DistanceMin_Outter, out hv_DistanceMax_Outter);
                    hv_Ddistance_Outter = hv_DistanceMax_Outter - hv_Radius_Outter;
                    hv_Sdistance_Outter = hv_Radius_Outter - hv_DistanceMin_Outter;
                    HOperatorSet.FitCircleContourXld(ho_glueInnerContour, "algebraic", -1, 0, 0,
                        3, 2, out hv_Row_Inner, out hv_Column_Inner, out hv_Radius_Inner, out hv_StartPhi2,
                        out hv_EndPhi2, out hv_PointOrder2);
                    ho_ContCircle_Inner.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle_Inner, hv_Row_Inner, hv_Column_Inner,
                        hv_Radius_Inner, 0, 6.28318, "positive", 1);
                    HOperatorSet.DistancePc(ho_glueInnerContour, hv_Row_Inner, hv_Column_Inner,
                        out hv_DistanceMin_Inner, out hv_DistanceMax_Inner);
                    hv_Ddistance_Inner = hv_Radius_Inner - hv_DistanceMin_Inner;
                    hv_Sdistance_Inner = hv_DistanceMax_Inner - hv_Radius_Inner;
                    //*利用拟合内外胶的圆心的平均值与拟合基准圆的圆心坐标判断偏移情况
                    hv_Row_glue = (hv_Row_Outter + hv_Row_Inner) / 2;
                    hv_Col_glue = (hv_Column_Outter + hv_Column_Inner) / 2;
                    HOperatorSet.DistancePp(hv_glueCenter.TupleSelect(0), hv_glueCenter.TupleSelect(
                        1), hv_Row_glue, hv_Col_glue, out hv_Distance_offset);
                    //*算出拟合胶水的宽度
                    hv_Radius = hv_Radius_Outter - hv_Radius_Inner;
                    //平滑轮廓
                    HOperatorSet.SmoothContoursXld(ho_glueOutterContour, out SmoothedGlueOutterContour, 5);
                    HOperatorSet.SmoothContoursXld(ho_glueInnerContour, out SmoothedGlueInnerContour, 5);
                   
                    //HOperatorSet.WriteString() 
                    #endregion
                    if ((int)(new HTuple(hv_Distance_offset.TupleGreater(dbVisSetting.glueOffset))) != 0)
                    {
                        is_qualified = false;
                        HOperatorSet.SetColor(hv_WindowHandle, "red");
                        HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                        HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueOutterContour, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueInnerContour, hv_WindowHandle);
                        //**显示
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离内边界最大 / 最小距离：" + hv_Distance_inner_max) + " / ") + hv_Distance_inner_min,
                            "window", 15, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离外边界最大 / 最小距离：" + hv_Distance_outter_max) + " / ") + hv_Distance_outter_min,
                            "window", 40, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "内外胶拟合圆宽度：" + hv_Radius,
                            "window", 65, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最小胶宽度：" + hv_glueWidthMin,
                            "window", 90, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最大胶宽度：" + hv_glueWidthMax,
                            "window", 115, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶偏移距离：" + hv_Distance_offset,
                            "window", 140, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "标准胶宽：" + dbVisSetting.GlueWidth,
                            "window", 165, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶圈偏移", "window", 40,
                            400, "red", "box", "false");
                        #region//释放图像变量
       
                        ho_TransImage.Dispose();
                        ho_ImageSub.Dispose();
                        ho_ImageGauss.Dispose();
                        ho_Regions_R.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_Rectangle.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_Regionc.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionFillUp1.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_Contours1.Dispose();
                        ho_Circle_mask_S.Dispose();
                        ho_target_ring.Dispose();
                        ho_target_ring_scale.Dispose();
                        ho_Regions.Dispose();
                        ho_RegionFillUp.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Contours.Dispose();
                        ho_glueOutterContour.Dispose();
                        ho_glueInnerContour.Dispose();
                        ho_Contour_I_sap.Dispose();
                        ho_Contour_O_sap.Dispose();
                        ho_ContCircle_outter.Dispose();
                        ho_ContCircle_Inner.Dispose();
                        #endregion
                        HOperatorSet.CountSeconds(out t2);
                        circletime = Math.Round((double)((t2 - t1) * 1000));
                        if (dbVisSetting.OkImageSave)
                        {
                            try
                            {
                               
                                HOperatorSet.WriteImage(ho_ImageT, ("jpg"), (0), $"{AppConfig.VisionPicturePass_GlueFind}{DateTime.Now.ToString("HHmmss")}_NOGlue.jpg");
                                HOperatorSet.WriteImage(ho_ImageF, ("jpg"), (0), $"{AppConfig.VisionPicturePass_GlueFind}{DateTime.Now.ToString("HHmmss")}_Glue.jpg");
                            }
                            catch (Exception ex) { }
                        }
                        return is_qualified;
                    }
                    else
                    {
                       
                        HOperatorSet.SetColor(hv_WindowHandle, "green");
                        HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                        HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueOutterContour, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueInnerContour, hv_WindowHandle);
                        if ((int)(new HTuple(hv_Distance_outter_max.TupleGreater(dbVisSetting.glueOverflowOutter))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "外胶溢胶!", "window", 15, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(((hv_Distance_outter_min.TupleAbs())).TupleGreater(dbVisSetting.glueLackOutter))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "外胶缺胶!", "window", 40, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(((hv_Distance_inner_min.TupleAbs())).TupleGreater(dbVisSetting.glueOverflowInner))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "内胶溢胶!", "window", 65, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(hv_Distance_inner_max.TupleGreater(dbVisSetting.glueLackInner))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "内胶缺胶!", "window", 90, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        //**显示
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离内边界最大 / 最小距离：" + hv_Distance_inner_max) + " / ") + hv_Distance_inner_min,
                            "window", 15, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离外边界最大 / 最小距离：" + hv_Distance_outter_max) + " / ") + hv_Distance_outter_min,
                            "window", 40, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "内外胶拟合圆宽度：" + hv_Radius,
                            "window", 65, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最小胶宽度：" + hv_glueWidthMin,
                            "window", 90, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最大胶宽度：" + hv_glueWidthMax,
                            "window", 115, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶偏移距离：" + hv_Distance_offset,
                            "window", 140, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "标准胶宽：" + dbVisSetting.GlueWidth,
                            "window", 165, 7, "green", "box", "false");

                        #region//释放图像变量
      
                        ho_TransImage.Dispose();
                        ho_ImageSub.Dispose();
                        ho_ImageGauss.Dispose();
                        ho_Regions_R.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_Rectangle.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_Regionc.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionFillUp1.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_Contours1.Dispose();
                        ho_Circle_mask_S.Dispose();
                        ho_target_ring.Dispose();
                        ho_target_ring_scale.Dispose();
                        ho_Regions.Dispose();
                        ho_RegionFillUp.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Contours.Dispose();
                        ho_glueOutterContour.Dispose();
                        ho_glueInnerContour.Dispose();
                        ho_Contour_I_sap.Dispose();
                        ho_Contour_O_sap.Dispose();
                        ho_ContCircle_outter.Dispose();
                        ho_ContCircle_Inner.Dispose();
                        #endregion

                        HOperatorSet.CountSeconds(out t2);
                        circletime = Math.Round((double)((t2 - t1) * 1000));
                        return is_qualified;
                    }


                }
                #endregion

            }
            catch (HalconException ex) { HOperatorSet.DispText(hv_WindowHandle, ex.GetErrorMessage(), "window", 0, 100, "red", new HTuple(), new HTuple()); return false; }
        }
        public bool GlueVisionAction(HObject ho_ImageT, HObject ho_ImageF, double CentrerGlueX, double CentrerGlueY, DbVisSetting dbVisSetting)
        {
            bool is_qualified = false;


            HTuple t1, t2;

            try
            {
                HOperatorSet.CountSeconds(out t1);


                #region// 声明图像变量
                HObject ho_RegionDifference, ho_RegionOpening2, ho_ConnectedRegions1, ho_SelectedRegions1;
                HObject ho_RegionErosion, ho_Domain, ho_RegionDifference1;
                HObject ho_TransImage;
                HObject ho_ImageSub, ho_ImageGauss, ho_Regions_R, ho_RegionClosing;
                HObject ho_RegionOpening1, ho_RegionUnion, ho_Rectangle;
                HObject ho_RegionOpening, ho_Regionc;
                HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionFillUp1;
                HObject ho_Contours1, ho_Circle_mask_S = null;
                HObject ho_target_ring = null, ho_target_ring_scale = null;
                HObject ho_Regions = null, ho_RegionFillUp = null, ho_RegionTrans = null;
                HObject ho_Contours = null, ho_glueOutterContour = null, ho_glueInnerContour = null;
                HObject ho_Contour_I_sap = null, ho_Contour_O_sap = null, ho_ContCircle_outter = null;
                HObject ho_ContCircle_Inner = null, Regionoping = null;
                HObject SmoothedGlueOutterContour = null, SmoothedGlueInnerContour = null;
                #endregion

                #region//初始化图像变量
                HOperatorSet.GenEmptyObj(out ho_TransImage);
                HOperatorSet.GenEmptyObj(out ho_ImageSub);
                HOperatorSet.GenEmptyObj(out ho_ImageGauss);
                HOperatorSet.GenEmptyObj(out ho_Regions_R);
                HOperatorSet.GenEmptyObj(out ho_RegionClosing);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
                HOperatorSet.GenEmptyObj(out ho_RegionUnion);
                HOperatorSet.GenEmptyObj(out ho_Rectangle);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening);
                HOperatorSet.GenEmptyObj(out ho_Regionc);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp1);
                HOperatorSet.GenEmptyObj(out ho_RegionErosion);
                HOperatorSet.GenEmptyObj(out ho_Contours1);
                HOperatorSet.GenEmptyObj(out ho_Circle_mask_S);
                HOperatorSet.GenEmptyObj(out ho_target_ring);
                HOperatorSet.GenEmptyObj(out ho_target_ring_scale);
                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_RegionTrans);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_glueOutterContour);
                HOperatorSet.GenEmptyObj(out ho_glueInnerContour);
                HOperatorSet.GenEmptyObj(out ho_Contour_I_sap);
                HOperatorSet.GenEmptyObj(out ho_Contour_O_sap);
                HOperatorSet.GenEmptyObj(out ho_ContCircle_outter);
                HOperatorSet.GenEmptyObj(out ho_ContCircle_Inner);
                HOperatorSet.GenEmptyObj(out Regionoping);
                HOperatorSet.GenEmptyObj(out ho_Domain);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);

                #endregion

                #region// 初始化控制变量 
                // Local control variables 

                HTuple hv_Width = null, hv_Height = null;
                HTuple hv_RowsF = null, hv_ColsF = null, hv_RowsT = null;
                HTuple hv_ColsT = null, hv_HomMat2D = null, hv_Points1 = null;
                HTuple hv_Points2 = null, hv_Number1 = null, hv_Number2 = null;
                HTuple hv_Row = new HTuple();
                HTuple hv_Column = new HTuple(), hv_Radius = new HTuple();
                HTuple hv_StartPhi = new HTuple(), hv_EndPhi = new HTuple();
                HTuple hv_PointOrder = new HTuple();
                //HTuple hv_glueInnerCircle = new HTuple(), hv_glueWidth = new HTuple();
                HTuple hv_glueOutterCircle = new HTuple();
                HTuple hv_glueCenter = new HTuple(), hv_Row2 = new HTuple();
                HTuple hv_Col1 = new HTuple(), hv_Row3 = new HTuple();
                HTuple hv_Col2 = new HTuple(), hv_Row_Inner_sampling = new HTuple();
                HTuple hv_Col_Inner_sampling = new HTuple(), hv_k = new HTuple();
                HTuple hv_i = new HTuple(), hv_Row_Outter_sampling = new HTuple();
                HTuple hv_Col_Outter_sampling = new HTuple(), hv_DistanceMin = new HTuple();
                HTuple hv_DistanceMax = new HTuple(), hv_glueWidthMin = new HTuple();
                HTuple hv_glueWidthMax = new HTuple(), hv_DistanceMin1 = new HTuple();
                HTuple hv_DistanceMax1 = new HTuple(), hv_Distance_inner_min = new HTuple();
                HTuple hv_Distance_inner_max = new HTuple(), hv_DistanceMin2 = new HTuple();
                HTuple hv_DistanceMax2 = new HTuple(), hv_Distance_outter_min = new HTuple();
                HTuple hv_Distance_outter_max = new HTuple(), hv_Row_Outter = new HTuple();
                HTuple hv_Column_Outter = new HTuple(), hv_Radius_Outter = new HTuple();
                HTuple hv_StartPhi1 = new HTuple(), hv_EndPhi1 = new HTuple();
                HTuple hv_PointOrder1 = new HTuple(), hv_DistanceMin_Outter = new HTuple();
                HTuple hv_DistanceMax_Outter = new HTuple(), hv_Ddistance_Outter = new HTuple();
                HTuple hv_Sdistance_Outter = new HTuple(), hv_Row_Inner = new HTuple();
                HTuple hv_Column_Inner = new HTuple(), hv_Radius_Inner = new HTuple();
                HTuple hv_StartPhi2 = new HTuple(), hv_EndPhi2 = new HTuple();
                HTuple hv_PointOrder2 = new HTuple(), hv_DistanceMin_Inner = new HTuple();
                HTuple hv_DistanceMax_Inner = new HTuple(), hv_Ddistance_Inner = new HTuple();
                HTuple hv_Sdistance_Inner = new HTuple(), hv_Row_glue = new HTuple();
                HTuple hv_Col_glue = new HTuple(), hv_Distance_offset = new HTuple();
                HTuple hv_Font = new HTuple();
                #endregion
                is_qualified = true;

                HObject img_old;
                HOperatorSet.GenEmptyObj(out img_old);
                HTuple htuple;
                //彩色变灰色
                HOperatorSet.CountChannels(ho_ImageT, out htuple);
                img_old.Dispose();
                if (htuple == 3)
                {
                    HOperatorSet.Rgb1ToGray(ho_ImageT, out img_old);
                    ho_ImageT.Dispose();
                    ho_ImageT = img_old.Clone();
                }


                //彩色变灰色
                HOperatorSet.CountChannels(ho_ImageF, out htuple);
                img_old.Dispose();
                if (htuple == 3)
                {
                    HOperatorSet.Rgb1ToGray(ho_ImageF, out img_old);
                    ho_ImageF.Dispose();
                    ho_ImageF = img_old.Clone();
                }

                //获取图像的尺寸信息
                HOperatorSet.GetImageSize(ho_ImageF, out hv_Width, out hv_Height);
                //*******************主逻辑
                #region//提取特征点,根据特征点求两张图之间的仿射矩阵：HomMat2D,并对齐图像
                HOperatorSet.PointsHarris(ho_ImageF, GlueFindParam.Instance.PH_sigama, 2, 0.08, GlueFindParam.Instance.PH_Threshold, out hv_RowsF, out hv_ColsF);
                HOperatorSet.PointsHarris(ho_ImageT, GlueFindParam.Instance.PH_sigama, 2, 0.08, GlueFindParam.Instance.PH_Threshold, out hv_RowsT, out hv_ColsT);

                //* 根据特征点求两张图之间的仿射矩阵：HomMat2D ssd
                HOperatorSet.ProjMatchPointsRansac(ho_ImageT, ho_ImageF, hv_RowsT, hv_ColsT,
                    hv_RowsF, hv_ColsF, "sad", 10, 0, 0, GlueFindParam.Instance.proj_match_points_ransac_Tolenrance, GlueFindParam.Instance.proj_match_points_ransac_Tolenrance, (new HTuple((new HTuple(-0.2)).TupleRad()
                    )).TupleConcat((new HTuple(0.2)).TupleRad()), 10, "gold_standard", 0.2, 0,
                    out hv_HomMat2D, out hv_Points1, out hv_Points2);
                //*将ImageT进行放射变换以对齐两张图片
                ho_TransImage.Dispose();
                HOperatorSet.ProjectiveTransImage(ho_ImageT, out ho_TransImage, hv_HomMat2D,
                    "bilinear", "false", "false");
                //ho_ImageT.Dispose();
                #endregion

                #region//提取胶圈轮廓
                //将对齐后点胶之后的图片减去点胶之前图片
                ho_ImageSub.Dispose();

                //HOperatorSet.AbsDiffImage(ho_ImageF, ho_TransImage, out ho_ImageSub, GlueFindParam.Instance.abs_diff_image_value);
                HOperatorSet.SubImage(ho_ImageF, ho_TransImage, out ho_ImageSub, GlueFindParam.Instance.sub1, GlueFindParam.Instance.subT);

                try
                {
                    HOperatorSet.WriteImage(ho_ImageSub, "jpg", 0, "C:\\Sub.jpg");
                }
                catch { }
                HObject hpowimage,hscareimage;
                HOperatorSet.GenEmptyObj(out hpowimage);
                HOperatorSet.GenEmptyObj(out hscareimage);
                hpowimage.Dispose();
                if (GlueFindParam.Instance.ScaleImg==false)
                    HOperatorSet.PowImage(ho_ImageSub, out hpowimage, GlueFindParam.Instance.pow_image_value);
                else {
                    double Mult = 255 *1.0/ (255 - GlueFindParam.Instance.Gmin);double add=-Mult* GlueFindParam.Instance.Gmin;
                    HOperatorSet.ScaleImage(ho_ImageSub, out hpowimage,Mult,add); }
              
                hscareimage.Dispose();
                HOperatorSet.ScaleImageMax(hpowimage,out hscareimage);
                //**对相减得到的图片进行处理，提出胶圈轮廓
                //平滑图像
                ho_ImageGauss.Dispose();
                //HOperatorSet.GaussFilter(ho_ImageSub, out ho_ImageGauss, 11);
                HOperatorSet.MedianImage(hscareimage, out ho_ImageGauss, "circle",GlueFindParam.Instance.median_image_value, "cyclic");
                ho_ImageSub.Dispose();
                //*阈值分割，采用区域生长法，控制参数tol=3, region阈值500000
                ho_Regions_R.Dispose();
                HOperatorSet.Regiongrowing(ho_ImageGauss, out ho_Regions_R, 4, 4, dbVisSetting.tol, dbVisSetting.area);

                //对region进行开运算和闭运算，并合并region
                //ho_RegionClosing.Dispose();
                //HOperatorSet.ClosingCircle(ho_Regions_R, out ho_RegionClosing, 11);
                //ho_RegionOpening1.Dispose();
                //HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening1, 11);
                //ho_RegionUnion.Dispose();
                //HOperatorSet.Union1(ho_RegionOpening1, out ho_RegionUnion);
                //提取胶圈区域region
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Rectangle, ho_Regions_R, out ho_RegionDifference
                    );
                HOperatorSet.CountObj(ho_RegionDifference, out hv_Number1);
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 30);
                ho_Regionc.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionOpening, out ho_Regionc, 30);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_Regionc, out ho_ConnectedRegions1);

                ho_SelectedRegions1.Dispose();

                HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1, "max_area",
                    70);
                ho_RegionFillUp1.Dispose();
                HOperatorSet.FillUpShape(ho_SelectedRegions1, out ho_RegionFillUp1, "area", 1,
                    GlueFindParam.Instance.fill_up_shape);
                ho_RegionErosion.Dispose();
                HOperatorSet.ErosionCircle(ho_RegionFillUp1, out ho_RegionErosion, 2.5);

                ho_Contours1.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionErosion, out ho_Contours1, "border_holes");
                HOperatorSet.CountObj(ho_Contours1, out hv_Number2);
                #endregion
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_Font);
                HOperatorSet.SetFont(hv_WindowHandle, (hv_Font.TupleSelect(0)) + "-Bold-13");

                #region//计算点胶区域内外圆
                //***********************
                //理论胶圈外圆
                hv_glueOutterCircle = dbVisSetting.GlueInnerCircle + dbVisSetting.GlueWidth;
                //理论胶圈圆心
                hv_glueCenter = new HTuple(2);
                hv_glueCenter[0] = CentrerGlueY;
                hv_glueCenter[1] = CentrerGlueX;
                HObject ho_Cross, ho_cir1, ho_cir2;
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_cir1);
                HOperatorSet.GenEmptyObj(out ho_cir2);
                //xmz  显示 内外圆区域
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.SetDraw(hv_WindowHandle, "margin"); ;
                ho_Cross.Dispose(); ;

                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_glueCenter[0], hv_glueCenter[1],
                       180, 0.785398);
                HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                HOperatorSet.DispObj(ho_Cross, hv_WindowHandle);
                ho_cir1.Dispose();
                HOperatorSet.GenCircle(out ho_cir1, hv_glueCenter[0], hv_glueCenter[1], dbVisSetting.GlueInnerCircle);
                HOperatorSet.DispObj(ho_cir1, hv_WindowHandle);
                ho_cir2.Dispose();
                HOperatorSet.GenCircle(out ho_cir2, hv_glueCenter[0], hv_glueCenter[1], hv_glueOutterCircle);
                HOperatorSet.DispObj(ho_cir2, hv_WindowHandle);
                //***********************
                #endregion

                #region//检测胶圈是否封闭
                if ((int)((new HTuple((new HTuple(hv_Number2.TupleEqual(2))).TupleAnd(new HTuple(hv_Number1.TupleEqual(
            1))))).TupleNot()) != 0)
                {
                    try
                    {
                        HOperatorSet.WriteImage(ho_ImageT, "jpg", 0, "C:\\img01.jpg");
                        HOperatorSet.WriteImage(ho_ImageF, "jpg", 0, "C:\\img02.jpg");
                    }
                    catch { }

                    is_qualified = false;
                    //HOperatorSet.DispObj(ho_ImageF, Window);
                    HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    HOperatorSet.DispObj(ho_Contours1, hv_WindowHandle);
                    HOperatorSet.DispText(hv_WindowHandle, "无胶或胶圈不完整！", "window",
                        15, 480, "red", "box", "false");
                    #region

                    ho_TransImage.Dispose();
                    ho_ImageSub.Dispose();
                    ho_ImageGauss.Dispose();
                    ho_Regions_R.Dispose();
                    ho_RegionClosing.Dispose();
                    ho_RegionOpening1.Dispose();
                    ho_RegionUnion.Dispose();
                    ho_Rectangle.Dispose();
                    ho_RegionDifference.Dispose();
                    ho_RegionOpening.Dispose();
                    ho_Regionc.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_SelectedRegions.Dispose();
                    ho_RegionFillUp1.Dispose();
                    ho_RegionErosion.Dispose();
                    ho_Contours1.Dispose();
                    ho_Circle_mask_S.Dispose();
                    ho_target_ring.Dispose();
                    ho_target_ring_scale.Dispose();
                    ho_Regions.Dispose();
                    ho_RegionFillUp.Dispose();
                    ho_RegionTrans.Dispose();
                    ho_Contours.Dispose();
                    ho_glueOutterContour.Dispose();
                    ho_glueInnerContour.Dispose();
                    ho_Contour_I_sap.Dispose();
                    ho_Contour_O_sap.Dispose();
                    ho_ContCircle_outter.Dispose();
                    ho_ContCircle_Inner.Dispose();
                    #endregion
                    HOperatorSet.CountSeconds(out t2);
                    circletime = Math.Round((double)((t2 - t1) * 1000));
                    return is_qualified;
                }
                #endregion

                #region//胶圈封闭则进一步检查其它指标是否合格
                else
                {

                    #region
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Regions_R, out ho_RegionClosing, dbVisSetting.kernel);
                    ho_RegionOpening1.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening1, 15);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionOpening1, out ho_RegionUnion);

                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_Rectangle, ho_RegionUnion, out ho_RegionDifference
                        );

                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 7);
                    ho_Regionc.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionOpening, out ho_Regionc, 9);

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regionc, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                        70);
                    ho_RegionFillUp1.Dispose();
                    HOperatorSet.FillUpShape(ho_SelectedRegions, out ho_RegionFillUp1, "area",
                        1, 20000);
                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionCircle(ho_RegionFillUp1, out ho_RegionErosion, 2.5);

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionErosion, out ho_Contours, "border_holes");
                    ho_glueOutterContour.Dispose();
                    HOperatorSet.SelectObj(ho_Contours, out ho_glueOutterContour, 1);
                    ho_glueInnerContour.Dispose();
                    HOperatorSet.SelectObj(ho_Contours, out ho_glueInnerContour, 2);
                    HOperatorSet.GetContourXld(ho_glueInnerContour, out hv_Row2, out hv_Col1);
                    HOperatorSet.GetContourXld(ho_glueOutterContour, out hv_Row3, out hv_Col2);
                    hv_Row_Inner_sampling = new HTuple();
                    hv_Col_Inner_sampling = new HTuple();
                    hv_k = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Row2.TupleLength())) - 1); hv_i = (int)hv_i + 50)
                    {
                        if (hv_Row_Inner_sampling == null)
                            hv_Row_Inner_sampling = new HTuple();
                        hv_Row_Inner_sampling[hv_k] = hv_Row2.TupleSelect(hv_i);
                        if (hv_Col_Inner_sampling == null)
                            hv_Col_Inner_sampling = new HTuple();
                        hv_Col_Inner_sampling[hv_k] = hv_Col1.TupleSelect(hv_i);
                        hv_k = hv_k + 1;
                    }
                    if (hv_Row_Inner_sampling == null)
                        hv_Row_Inner_sampling = new HTuple();
                    hv_Row_Inner_sampling[new HTuple(hv_Row_Inner_sampling.TupleLength())] = hv_Row_Inner_sampling.TupleSelect(
                        0);
                    if (hv_Col_Inner_sampling == null)
                        hv_Col_Inner_sampling = new HTuple();
                    hv_Col_Inner_sampling[new HTuple(hv_Col_Inner_sampling.TupleLength())] = hv_Col_Inner_sampling.TupleSelect(
                        0);
                    hv_Row_Outter_sampling = new HTuple();
                    hv_Col_Outter_sampling = new HTuple();
                    hv_k = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Row3.TupleLength())) - 1); hv_i = (int)hv_i + 50)
                    {
                        if (hv_Row_Outter_sampling == null)
                            hv_Row_Outter_sampling = new HTuple();
                        hv_Row_Outter_sampling[hv_k] = hv_Row3.TupleSelect(hv_i);
                        if (hv_Col_Outter_sampling == null)
                            hv_Col_Outter_sampling = new HTuple();
                        hv_Col_Outter_sampling[hv_k] = hv_Col2.TupleSelect(hv_i);
                        hv_k = hv_k + 1;
                    }
                    if (hv_Row_Outter_sampling == null)
                        hv_Row_Outter_sampling = new HTuple();
                    hv_Row_Outter_sampling[new HTuple(hv_Row_Outter_sampling.TupleLength())] = hv_Row_Outter_sampling.TupleSelect(
                        0);
                    if (hv_Col_Outter_sampling == null)
                        hv_Col_Outter_sampling = new HTuple();
                    hv_Col_Outter_sampling[new HTuple(hv_Col_Outter_sampling.TupleLength())] = hv_Col_Outter_sampling.TupleSelect(
                        0);
                    ho_Contour_I_sap.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour_I_sap, hv_Row_Inner_sampling,
                        hv_Col_Inner_sampling);
                    ho_Contour_O_sap.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour_O_sap, hv_Row_Outter_sampling,
                        hv_Col_Outter_sampling);
                    ho_glueInnerContour.Dispose();
                    ho_glueInnerContour = ho_Contour_I_sap.CopyObj(1, -1);
                    ho_glueOutterContour.Dispose();
                    ho_glueOutterContour = ho_Contour_O_sap.CopyObj(1, -1);
                    //*最大最小胶宽
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_Row2, hv_Col1, out hv_DistanceMin,
                        out hv_DistanceMax);
                    HOperatorSet.TupleMin(hv_DistanceMin, out hv_glueWidthMin);
                    HOperatorSet.TupleMax(hv_DistanceMin, out hv_glueWidthMax);
                    //*胶水内轮廓离边界的距离最大值最小值
                    HOperatorSet.DistancePc(ho_glueInnerContour, hv_glueCenter.TupleSelect(0),
                        hv_glueCenter.TupleSelect(1), out hv_DistanceMin1, out hv_DistanceMax1);
                    hv_Distance_inner_min = hv_DistanceMin1 - dbVisSetting.GlueInnerCircle;
                    hv_Distance_inner_max = hv_DistanceMax1 - dbVisSetting.GlueInnerCircle;
                    //*胶水外轮廓离边界的距离最大最小值
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_glueCenter.TupleSelect(0),
                        hv_glueCenter.TupleSelect(1), out hv_DistanceMin2, out hv_DistanceMax2);
                    hv_Distance_outter_min = hv_DistanceMin2 - hv_glueOutterCircle;
                    hv_Distance_outter_max = hv_DistanceMax2 - hv_glueOutterCircle;
                    //*用胶轮廓与拟合圆间的最大距离判断缺胶溢胶情况
                    HOperatorSet.FitCircleContourXld(ho_glueOutterContour, "algebraic", -1, 0,
                        0, 3, 2, out hv_Row_Outter, out hv_Column_Outter, out hv_Radius_Outter,
                        out hv_StartPhi1, out hv_EndPhi1, out hv_PointOrder1);
                    ho_ContCircle_outter.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle_outter, hv_Row_Outter, hv_Column_Outter,
                        hv_Radius_Outter, 0, 6.28318, "positive", 1);
                    HOperatorSet.DistancePc(ho_glueOutterContour, hv_Row_Outter, hv_Column_Outter,
                        out hv_DistanceMin_Outter, out hv_DistanceMax_Outter);
                    hv_Ddistance_Outter = hv_DistanceMax_Outter - hv_Radius_Outter;
                    hv_Sdistance_Outter = hv_Radius_Outter - hv_DistanceMin_Outter;
                    HOperatorSet.FitCircleContourXld(ho_glueInnerContour, "algebraic", -1, 0, 0,
                        3, 2, out hv_Row_Inner, out hv_Column_Inner, out hv_Radius_Inner, out hv_StartPhi2,
                        out hv_EndPhi2, out hv_PointOrder2);
                    ho_ContCircle_Inner.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle_Inner, hv_Row_Inner, hv_Column_Inner,
                        hv_Radius_Inner, 0, 6.28318, "positive", 1);
                    HOperatorSet.DistancePc(ho_glueInnerContour, hv_Row_Inner, hv_Column_Inner,
                        out hv_DistanceMin_Inner, out hv_DistanceMax_Inner);
                    hv_Ddistance_Inner = hv_Radius_Inner - hv_DistanceMin_Inner;
                    hv_Sdistance_Inner = hv_DistanceMax_Inner - hv_Radius_Inner;
                    //*利用拟合内外胶的圆心的平均值与拟合基准圆的圆心坐标判断偏移情况
                    hv_Row_glue = (hv_Row_Outter + hv_Row_Inner) / 2;
                    hv_Col_glue = (hv_Column_Outter + hv_Column_Inner) / 2;
                    HOperatorSet.DistancePp(hv_glueCenter.TupleSelect(0), hv_glueCenter.TupleSelect(
                        1), hv_Row_glue, hv_Col_glue, out hv_Distance_offset);
                    //*算出拟合胶水的宽度
                    hv_Radius = hv_Radius_Outter - hv_Radius_Inner;
                    //平滑轮廓
                    HOperatorSet.SmoothContoursXld(ho_glueOutterContour, out SmoothedGlueOutterContour, 5);
                    HOperatorSet.SmoothContoursXld(ho_glueInnerContour, out SmoothedGlueInnerContour, 5);

                    //HOperatorSet.WriteString() 
                    #endregion
                    if ((int)(new HTuple(hv_Distance_offset.TupleGreater(dbVisSetting.glueOffset))) != 0)
                    {
                       
                        is_qualified = false;
                        HOperatorSet.SetColor(hv_WindowHandle, "red");
                        HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                        HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueOutterContour, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueInnerContour, hv_WindowHandle);
                        //**显示
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离内边界最大 / 最小距离：" + hv_Distance_inner_max) + " / ") + hv_Distance_inner_min,
                            "window", 15, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离外边界最大 / 最小距离：" + hv_Distance_outter_max) + " / ") + hv_Distance_outter_min,
                            "window", 40, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "内外胶拟合圆宽度：" + hv_Radius,
                            "window", 65, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最小胶宽度：" + hv_glueWidthMin,
                            "window", 90, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最大胶宽度：" + hv_glueWidthMax,
                            "window", 115, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶偏移距离：" + hv_Distance_offset,
                            "window", 140, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "标准胶宽：" + dbVisSetting.GlueWidth,
                            "window", 165, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶圈偏移", "window", 40,
                            400, "red", "box", "false");
                        #region//释放图像变量

                        ho_TransImage.Dispose();
                        ho_ImageSub.Dispose();
                        ho_ImageGauss.Dispose();
                        ho_Regions_R.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_Rectangle.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_Regionc.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionFillUp1.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_Contours1.Dispose();
                        ho_Circle_mask_S.Dispose();
                        ho_target_ring.Dispose();
                        ho_target_ring_scale.Dispose();
                        ho_Regions.Dispose();
                        ho_RegionFillUp.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Contours.Dispose();
                        ho_glueOutterContour.Dispose();
                        ho_glueInnerContour.Dispose();
                        ho_Contour_I_sap.Dispose();
                        ho_Contour_O_sap.Dispose();
                        ho_ContCircle_outter.Dispose();
                        ho_ContCircle_Inner.Dispose();
                        #endregion
                        HOperatorSet.CountSeconds(out t2);
                        circletime = Math.Round((double)((t2 - t1) * 1000));
              
                        return is_qualified;
                    }
                    else
                    {

                        HOperatorSet.SetColor(hv_WindowHandle, "green");
                        HOperatorSet.SetLineWidth(hv_WindowHandle, 2);
                        HOperatorSet.DispObj(ho_ImageF, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueOutterContour, hv_WindowHandle);
                        HOperatorSet.DispObj(ho_glueInnerContour, hv_WindowHandle);
                        if ((int)(new HTuple(hv_Distance_outter_max.TupleGreater(dbVisSetting.glueOverflowOutter))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "外胶溢胶!NG", "window", 15, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(((hv_Distance_outter_min.TupleAbs())).TupleGreater(dbVisSetting.glueLackOutter))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "外胶缺胶!NG", "window", 40, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(((hv_Distance_inner_min.TupleAbs())).TupleGreater(dbVisSetting.glueOverflowInner))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "内胶溢胶!NG", "window", 65, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if ((int)(new HTuple(hv_Distance_inner_max.TupleGreater(dbVisSetting.glueLackInner))) != 0)
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "内胶缺胶!NG", "window", 90, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if (hv_glueWidthMax>(dbVisSetting.GlueWidth+(dbVisSetting.glueOverflowOutter+ dbVisSetting.glueOverflowInner)/2.0))
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "最大胶宽度超出！NG", "window", 115, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        if (hv_glueWidthMin < (dbVisSetting.GlueWidth-(dbVisSetting.glueLackOutter + dbVisSetting.glueLackInner) / 2.0))
                        {
                            HOperatorSet.DispText(hv_WindowHandle, "最小胶宽度超出！NG", "window", 140, 400, "red", "box", "false");
                            is_qualified = false;
                        }
                        //**显示
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离内边界最大 / 最小距离：" + hv_Distance_inner_max) + " / ") + hv_Distance_inner_min,
                            "window", 15, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, (("胶离外边界最大 / 最小距离：" + hv_Distance_outter_max) + " / ") + hv_Distance_outter_min,
                            "window", 40, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "内外胶拟合圆宽度：" + hv_Radius,
                            "window", 65, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最小胶宽度：" + hv_glueWidthMin,
                            "window", 90, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "最大胶宽度：" + hv_glueWidthMax,
                            "window", 115, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "胶偏移距离：" + hv_Distance_offset,
                            "window", 140, 7, "green", "box", "false");
                        HOperatorSet.DispText(hv_WindowHandle, "标准胶宽：" + dbVisSetting.GlueWidth,
                            "window", 165, 7, "green", "box", "false");

                        if (dbVisSetting.OkImageSave)
                        {
                            try
                            {

                                HOperatorSet.WriteImage(ho_ImageT, ("jpg"), (0), $"{AppConfig.VisionPicturePass_GlueFind}{DateTime.Now.ToString("HHmmss")}_NOGlue.jpg");
                                HOperatorSet.WriteImage(ho_ImageF, ("jpg"), (0), $"{AppConfig.VisionPicturePass_GlueFind}{DateTime.Now.ToString("HHmmss")}_Glue.jpg");
                            }
                            catch (Exception ex) { }
                        }
                        #region//释放图像变量

                        ho_TransImage.Dispose();
                        ho_ImageSub.Dispose();
                        ho_ImageGauss.Dispose();
                        ho_Regions_R.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_Rectangle.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_Regionc.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionFillUp1.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_Contours1.Dispose();
                        ho_Circle_mask_S.Dispose();
                        ho_target_ring.Dispose();
                        ho_target_ring_scale.Dispose();
                        ho_Regions.Dispose();
                        ho_RegionFillUp.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Contours.Dispose();
                        ho_glueOutterContour.Dispose();
                        ho_glueInnerContour.Dispose();
                        ho_Contour_I_sap.Dispose();
                        ho_Contour_O_sap.Dispose();
                        ho_ContCircle_outter.Dispose();
                        ho_ContCircle_Inner.Dispose();
                        #endregion

                        HOperatorSet.CountSeconds(out t2);
                        circletime = Math.Round((double)((t2 - t1) * 1000));
                        return is_qualified;
                    }


                }
                #endregion

            }
            catch (HalconException ex) { HOperatorSet.DispText(hv_WindowHandle, ex.GetErrorMessage(), "window", 0, 100, "red", new HTuple(), new HTuple()); return false; }
        }
        public bool VisionRun(DbVisSetting dbVisSetting, VisCameraCalib m_CalibParam)
        {
            HTuple t1, t2;
            RetShapeModel retModel = new RetShapeModel();
            RetFitCircle retFit = new RetFitCircle();
            HTuple width = 0, height = 0;
            double parea = 0;
            HTuple area = 0;
            double regionGray = 0;
            MatchResult ret = new MatchResult();
            ret.hv_RowCheck = 0;
            ret.hv_ColumnCheck = 0;
            ret.hv_AngleCheck = 0;
            ret.hv_Score = 0;
            VisionResult visionResult = new VisionResult();
            bool modelIsOK = false, circleIsOK = false, PointIsOK = false, grayIsOK = false, polarIsOK = false;
            HOperatorSet.CountSeconds(out t1);
            //log.Debug($"模板路径{dbVisSetting.strModelPath}");
            //log.Debug($"参数名称{ dbVisSetting.strID}");
            //查找模板
            if (ho_Image != null)
            {
                if (dbVisSetting.IsModelUse)
                {
                    modelIsOK = FindShapeModel(ho_Image, dbVisSetting.ShapeModel, dbVisSetting.IsModelDisplay, out retModel);
                }
                else modelIsOK = true;
            }
            if (modelIsOK)
            {
                //找圆检测
                try
                {
                    if (!dbVisSetting.IsModelUse)
                        circleIsOK = FindCircleMetrology(ho_Image, dbVisSetting.FindCircle, dbVisSetting.IsCircleDisplay, out retFit);
                    else
                    {
                        FitCircleType retC = dbVisSetting.FindCircle;
                        var dx = dbVisSetting.FindCircle.CenterCol - dbVisSetting.ModelCenterCol;
                        var dy = dbVisSetting.FindCircle.CenterRow - dbVisSetting.ModelCenterRow;
                        retC.CenterRow = retModel.CenterRow + dy;
                        retC.CenterCol = retModel.CenterCol + dx;
                        circleIsOK = FindCircleMetrology(ho_Image, retC, dbVisSetting.IsCircleDisplay, out retFit);
                    }
                }
                catch { circleIsOK = false; }
                //寻找角度
                if (dbVisSetting.IsPolarlUse)
                {
                    try
                    {
                        if (circleIsOK)
                            polarIsOK = FindAngle(ho_Image, retFit.CenterRow, retFit.CenterCol, dbVisSetting.PolarInnerR,
                                dbVisSetting.PolarOuterR, dbVisSetting.PolarminGray, dbVisSetting.PolarmaxGray, dbVisSetting.minRegionLenght, 
                                dbVisSetting.maxRegionLenght,dbVisSetting.minPolarArea, dbVisSetting.maxPolarArea, dbVisSetting.IsPolarDisplay, out ret, out area);
                        else polarIsOK = false;
                    }
                    catch (Exception ex)
                    {
                        polarIsOK = false;
                    }
                }
                else polarIsOK = true;
                //打点判断
                if (dbVisSetting.IsPointUse)
                {
                    try
                    {
                        if (circleIsOK)
                            PointIsOK = JudgePointImg(ho_Image, retFit.CenterRow, retFit.CenterCol, dbVisSetting.PointRadius,
                                dbVisSetting.PointGrayMin, dbVisSetting.PointGrayMax, dbVisSetting.IsPointDisplay,out parea);
                        else PointIsOK = false;
                    }
                    catch (Exception ex)
                    {
                        PointIsOK = false;
                    }
                }
                else PointIsOK = true;
                //正反检测
                if (dbVisSetting.IsGraylUse)
                {
                    if (!dbVisSetting.IsModelUse)
                        grayIsOK = JudgeDirectorImg(ho_Image, dbVisSetting.GrayCenterRow, dbVisSetting.GrayCenterCol,
                        dbVisSetting.GrayRadius1, dbVisSetting.GrayRadius2, dbVisSetting.GrayMinTolerance,
                        dbVisSetting.GrayMaxTolerance, dbVisSetting.IsGrayDisplay, out regionGray);
                    else
                    {
                        grayIsOK = JudgeDirectorImg(ho_Image, retFit.CenterRow, retFit.CenterCol,
                        dbVisSetting.GrayRadius1, dbVisSetting.GrayRadius2, dbVisSetting.GrayMinTolerance,
                        dbVisSetting.GrayMaxTolerance, dbVisSetting.IsGrayDisplay, out regionGray);
                    }
                }
                else grayIsOK = true;
                HOperatorSet.GetImageSize(ho_Image, out width, out height);
            }
            if (circleIsOK)
            {
                visionResult.Row_Pixel = retFit.CenterRow;
                visionResult.Column_Pixel = retFit.CenterCol;
                visionResult.Row = retFit.CenterRow == 0 ? 0 : Math.Round((double)(retFit.CenterRow - (height / 2.0)) * m_CalibParam.ResolutionY, 3);
                visionResult.Column = retFit.CenterCol == 0 ? 0 : Math.Round((double)(retFit.CenterCol - (width / 2.0)) * m_CalibParam.ResolutionX, 3);
                visionResult.m_Score = retModel.Score;
            }
            else
            {
                HOperatorSet.GetImageSize(ho_Image, out width, out height);
                visionResult.Row_Pixel = height/2;
                visionResult.Column_Pixel = width/2;
                visionResult.Row = 0.0;
                visionResult.Column = 0.0;
                visionResult.m_Score = 0.0;
            }
            if (polarIsOK)
            {
                visionResult.Angle = ret.hv_AngleCheck;
                visionResult.p_Score = area;
            }
            else
            {
                visionResult.Angle = 0.0;
                visionResult.p_Score = area;
            }
            visionResult.IsPositive = grayIsOK;
            visionResult.regionGray = Math.Round(regionGray, 2);
            if (modelIsOK && circleIsOK && PointIsOK && grayIsOK && polarIsOK)
            {
                visionResult.Result = "OK";
                log.Debug("视觉检测成功");
                DispMsg($"{visionResult.Result} X:{visionResult.Column} Y:{visionResult.Row} A:{visionResult.Angle}", 10, 10, false);
                if (dbVisSetting.OkImageSave)
                {
                    try
                    {
                        SaveImg($"{AppConfig.VisionPicturePass}_OK_Circle{DateTime.Now.ToString("HHmmss")}.jpg");
                    }
                    catch (Exception ex) { }
                }
            }
            else
            {
                visionResult.Result = "NG";
              
                
                if (dbVisSetting.IsPolarlUse&& !polarIsOK) visionResult.Result = "角度NG";
                if (!circleIsOK) visionResult.Result = "找圆NG";
                if (dbVisSetting.IsModelUse && !modelIsOK) visionResult.Result = "模板NG";

                DispMsg($"{visionResult.Result} X:{visionResult.Column} Y:{visionResult.Row} A:{visionResult.Angle}", 10, 10, true);
                if (dbVisSetting.NgImageSave)
                {
                    try
                    {
                        SaveImg($"{AppConfig.VisionPictureFail}_NG_Circle{DateTime.Now.ToString("HHmmss")}.jpg");
                    }
                    catch (Exception ex) { }
                }
            }
            VisionResult = visionResult;
            HOperatorSet.CountSeconds(out t2);
            vistime = Math.Round((double)((t2 - t1) * 1000), 2);
            if (true && ho_Image != null && ho_Image.Key != IntPtr.Zero)
            {
                try
                {
                    HTuple hv_width = 0, hv_height = 0;
                    HOperatorSet.GetImageSize(ho_Image, out hv_width, out hv_height);
                    HOperatorSet.SetColor(hv_WindowHandle, "yellow");
                    HOperatorSet.DispLine(hv_WindowHandle, hv_height / 2, 0, hv_height / 2, hv_width);
                    HOperatorSet.DispLine(hv_WindowHandle, 0, hv_width / 2, hv_height, hv_width / 2);

                    if (dbVisSetting.IsModelUse && modelIsOK)
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, "magenta");
                    }
                }
                catch (Exception ex) { }
            }
            return true;
        }

        /// <summary>
        /// 计算图像坐标与机器坐标的反射变换  9点标定
        /// </summary>
        public bool ImageToAxisMat2D(int PointsNums)
        {
            try
            {
                HTuple px = new HTuple();
                HTuple py = new HTuple();
                HTuple qx = new HTuple();
                HTuple qy = new HTuple();

                for (int i = 0; i < PointsNums /*DgV_TrayDatas1.RowCount*/; i++)
                {
                    px[i] = ImagePoints[i, 0];
                    py[i] = ImagePoints[i, 1];
                    qx[i] = AxisPoints[i, 0];
                    qy[i] = AxisPoints[i, 1];
                }
                HOperatorSet.VectorToHomMat2d(px, py, qx, qy, out h_HomMat2D);
                //Tb_Mat2D1.Text = Convert.ToString(h_HomMat2D[0].D);
                //Tb_Mat2D2.Text = Convert.ToString(h_HomMat2D[1].D);
                //Tb_Mat2D3.Text = Convert.ToString(h_HomMat2D[2].D);
                //Tb_Mat2D4.Text = Convert.ToString(h_HomMat2D[3].D);
                //Tb_Mat2D5.Text = Convert.ToString(h_HomMat2D[4].D);
                //Tb_Mat2D6.Text = Convert.ToString(h_HomMat2D[5].D);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 图像坐标转机械坐标
        /// </summary>
        /// <param name="hImageX"></param>
        /// <param name="hImageY"></param>
        /// <param name="hAxisX"></param>
        /// <param name="hAxisY"></param>
        /// <returns></returns>
        public bool ImageToAxis(HTuple hImageX, HTuple hImageY, out HTuple hAxisX, out HTuple hAxisY)
        {
            hAxisY = 0; hAxisX = 0;
            try
            {

                HOperatorSet.AffineTransPoint2d(h_HomMat2D, hImageX, hImageY, out hAxisX, out hAxisY);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
    public partial class HDevelopExport
    {
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
    }
}
