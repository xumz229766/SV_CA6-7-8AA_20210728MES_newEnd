 using System.Collections.Generic;
using Motion.Enginee;
using Motion.Interfaces;
using System.Threading;
using System.Diagnostics;
using System;
using System.Toolkit;
using System.Collections;
using System.Threading.Tasks;
using desay.ProductData;
using System.IO;
using Motion.AdlinkAps;
using System.Device;
using System.Windows.Forms;
using PylonLiveViewer;
using desay.Vision;
using HalconDotNet;
using System.Text;

using System.Toolkit.Helpers;
namespace desay.Flow
{
    /// <summary>
    /// 点胶平台控制流程
    /// </summary>
    public class GluePlateform : StationPart
    {
        private PlateformAlarm m_Alarm;
        public Thread threadRun = null;
        public int count;//点胶NG计数

        public GluePlateform(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
                        : base(ExternalSign, stationIni, stationOpe, typeof(GluePlateform))
        {
            //threadRun = new Thread(run);
            //threadRun.IsBackground = true;
        }     
        #region 部件
        /// <summary>
        /// X-轴
        /// </summary>
        public ServoAxis Xaxis { get; set; }
        /// <summary>
        /// Y-轴
        /// </summary>
        public ServoAxis Yaxis { get; set; }
        /// <summary>
        /// Z-轴
        /// </summary>
        public ServoAxis Zaxis { get; set; }
        /// <summary>
        /// 点胶阻挡气缸
        /// </summary>
        public SingleCylinder GlueStopCylinder { get; set; }
        /// <summary>
        /// 点胶顶升气缸
        /// </summary>
        public DoubleCylinder GlueUpCylinder { get; set; }
        /// <summary>
        /// 点胶小顶升气缸
        /// </summary>
        public DoubleCylinder GlueUpCylinder_Small { get; set; }
        /// <summary>
        /// 相机
        /// </summary>
        public BaslerCam baslerCamera { get; set; }

        public VisionClass glueVisionClass { get; set; }

        public VisionClass glueVisionClass_GlueText { get; set; }
        public VisionClass CalibglueVisionClass { get; set; }
        public LightControl lightControl { get; set; }

        #endregion

        public event Action<int> SendRequest;
        int failCount = 0;

        int wbCheckCount = 0;
       
        public bool GlueHomeBit;
        public Point3D<int> ccdOffset;
        StringBuilder buf = new StringBuilder(3072);//指定的buf大小必须大于传入的字符长度
        public override void Running(RunningModes runningMode)
        {
            Stopwatch watchGlueCT = new Stopwatch();
            watchGlueCT.Start();
            var _watch = new Stopwatch();
            _watch.Start();
            uint CenterCheckCount = 0;
            uint GlueCheckCount = 0;
            int HomeTime = 0;
            //识别数据
            string res = "";
            Point<double> Center =new Point<double> {};
            HObject ho_ImageT;
            HOperatorSet.GenEmptyObj(out ho_ImageT);
            int GlueLoadTime = 0;
            int GlueFindTime = 0;
            bool WbLightOnNG = false;
            while (true)
            {
                Thread.Sleep(10);
                GlueStopCylinder.Condition.External = AlarmReset;
                GlueUpCylinder.Condition.External = AlarmReset;

                GlueHomeBit = /*IoPoints.IDI17.Value &&*/ IoPoints.IDI10.Value && Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z);

                #region 自动运行流程
                if (stationOperate.Running)
                {

                    Marking.GlueHaveProduct = (IoPoints.IDI10.Value || Marking.GlueHaveProductSheild) && GlueUpCylinder.OutOriginStatus;
                    switch (step)
                    {
                        case 0: //判断平台是否在原点，顶升是否在原位
                          
                            IoPoints.IDO19.Value = false;
                            CenterCheckCount = 0;
                            GlueCheckCount = 0;
                            //清除标志位
                            Marking.WhiteBoardResult_Detect = false;
                            Marking.WhiteBoardResult_Stain = false;
                            Marking.WhiteBoardResult = false;
                            Marking.LoadcationVisonResult = false;
                            Marking.GlueFindVisionResult = false;
                            Marking.GlueResult = false;
                            Center.X = 0;
                            Center.Y = 0;
                            if (GlueHomeBit)
                                 step = 20;
                            else
                                 step = 5;
                            break;
                        case 5:  //复位所有气缸的动作
                            GlueStopCylinder.Reset();
                            GlueUpCylinder_Small.Reset();
                            Thread.Sleep(300);
                            GlueUpCylinder.Reset();
                            step = 10;
                            break;
                        case 10:    //判断所有气缸到位，启动Z轴回安全位
                            if (GlueUpCylinder.OutOriginStatus)
                            {
                                Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, AxisParameter.Instance.RZspeed);
                                step = 15;
                            }
                            break;
                        case 15:  //判断Z轴是否在安全位
                            if (Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                            {
                                step = 20;
                            }
                            break;
                        case 20:
                            if ((Marking.CleanWorkAlreadyRequest /*|| stationOperate.SingleRunning*/) && Marking.GlueShield)
                            {
                                Marking.GlueAlreadyGetJigsFromClean = true;//点胶工位准备好
                                watchGlueCT.Restart();
                                step = 25;//结束流程
                            }
                            else if(Marking.CleanWorkAlreadyRequest /*|| stationOperate.SingleRunning*/)//clean 放行治具
                            {
                                watchGlueCT.Restart();
                                Marking.GlueAlreadyGetJigsFromClean = true;//点胶工位准备好
                                step = 25;
                            }
                            break;
                        case 25:
                            if (Marking.GlueHaveProduct)//治具到位
                            {
                                Thread.Sleep(AxisParameter.Instance.GluePosDelay);//到位感应延时
                                Marking.GlueAlreadyGetJigsFromClean = false;//清除

                                step = 28;
                            }
                            break;
                        case 28:
                            if (!Marking.CleanWorkAlreadyRequest/*||stationOperate.SingleRunning*/)//
                            {
                               
                                //Marking.GlueCycleTime = 0;
                                //Thread.Sleep(100);
                                GlueUpCylinder.Set();//顶升气缸顶起
                                step = 29;
                            }
                            break;
                        case 29:
                            if (GlueUpCylinder.OutMoveStatus) //
                            {
                                Marking.WhiteBoardResult_DetectStain = false;
                                Marking.WhiteBoardResult_Light = false;
                                if (Marking.GlueShield) { step = 380;break; }//结束流程
                                //增加MEs判断
                                if (!MesData.glueData.cleanData.CleanResult_MesLock) { step = 380; break; }
                                if (Marking.WhiteShield)//白板屏蔽
                                {
                                    Marking.WhiteBoardResult = true;
                                    
                                    step = 60;
                                }
                                else step = 30;
                                if (AxisParameter.Instance.IsUseTanZhenCyl) GlueUpCylinder_Small.Set();
                            }
                            break;
                        #region 白板
                        case 30:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if(GlueUpCylinder_Small.OutMoveStatus) step = 32;
                            }
                            else step = 32;
                            break;
                        case 32://XY模组移至白板测试位置
                            if (lightControl != null)//白板检测关闭光源
                            {
                                lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, 0);
                            }
                            Xaxis.MoveTo(Position.Instance.AdjustLightPosition.X, AxisParameter.Instance.RXspeed);
                            Yaxis.MoveTo(Position.Instance.AdjustLightPosition.Y, AxisParameter.Instance.RYspeed);
                            step = 35;
                            break;
                        case 35://Z模组移至白板测试位置
                            if (Yaxis.IsInPosition(Position.Instance.AdjustLightPosition.Y)
                                && Xaxis.IsInPosition(Position.Instance.AdjustLightPosition.X))
                            {
                                Zaxis.MoveTo(Position.Instance.AdjustLightPosition.Z, AxisParameter.Instance.RZspeed);
                                step = 38;
                            }
                            break;
                        case 38://
                            if (Zaxis.IsInPosition(Position.Instance.AdjustLightPosition.Z))
                            {
                                Thread.Sleep(10);
                         
                                step = 40;//白板测试启动
                            }
                            break;
                        case 40://解串板通电
                            IoPoints.IDO14.Value = true;
                            wbCheckCount = 0;
                            AppendText("点胶解串板通电");
                            AppendText($"白板测试启动_状态{CallWb.GetAAImageStatus()}");
                            _watch.Restart();
                            step = 43;
                            break;
                        case 43://白板测试启动
                           
                            try
                            {
                                if (CallWb.GetAAImageStatus() == (int)AAImageSTATUS.AAImage_READY)
                                {
                                    Thread.Sleep(AxisParameter.Instance.GluePowerOpenDelayTime);//到位感应延时
                                    AppendText($"白板开始");
                                    CallWb.StartAAImage(MesData.glueData.cleanData.HolderSN, MesData.glueData.cleanData.JigsSN);
                                    Thread.Sleep(500);
                                    _watch.Restart();
                                    WbLightOnNG = false;
                                    step = 45;
                                }
                                else if (_watch.ElapsedMilliseconds / 1000 > 55)
                                {
                                    m_Alarm = PlateformAlarm.白板工位Ready超时;
                                    AppendText("白板工位Ready超时");
                                    _watch.Restart();

                                }
                            }
                            catch (Exception ex) {
                                AppendText($"白板程序异常！{ex}");
                                Marking.WhiteBoardResult = false;
                                Marking.WbGetResultFlg = true;
                                step = 380;///////////////////////////NG 结束点胶流程
                            }
                           
                            break;
                        case 45:
                            if (CallWb.GetAAImageStatus() == (int)AAImageSTATUS.AAImage_READY)
                            {
                                step = 46;
                            }
                            else if (CallWb.GetAAImageStatus() == (int)AAImageSTATUS.AAImage_TESETING)//测试中
                            {

                            }
                            else if (CallWb.GetAAImageStatus() == (int)AAImageSTATUS.AAImage_WARMING)//报警
                            {
                                //m_Alarm = PlateformAlarm.白板工位报警中;
                                step = 47;//报警   //需要重新复位AA
                                Marking.WhiteBoardResult = false;
                                Marking.WbGetResultFlg = true;
                            }
                            _watch.Stop();
                            if (_watch.ElapsedMilliseconds / 1000 > 30)
                            {
                                //m_Alarm = PlateformAlarm.AA堆料工位复位时气缸不在状态位;
                                AppendText("白板工位超时30S");
                                _watch.Restart();
                                step = 47;
                                Marking.WhiteBoardResult = false;
                                Marking.WbGetResultFlg = true;
                            }
                            _watch.Start();
                            break;
                        case 46:
                            
                            if (CallWb.GetAAImageTestResult(buf) == (int)AAImageResult.AAIamge_PASS)
                            {
                                Marking.WhiteBoardResult = true;
                                Marking.WhiteBoardResult_DetectStain = false;
                                Marking.WhiteBoardResult_Light = false;
                            }
                            else if (CallWb.GetAAImageTestResult(buf) == (int)AAImageResult.AAImage_LightON_NG)
                            {
                                Marking.WhiteBoardResult = false;
                                Marking.WhiteBoardResult_Light = true;
                                WbLightOnNG = true;
                            }
                            else if (CallWb.GetAAImageTestResult(buf) == (int)AAImageResult.AAImage_Particle_NG)
                            {
                                Marking.WhiteBoardResult_DetectStain = true;
                                Marking.WhiteBoardResult = false;
                            }
                            else if (CallWb.GetAAImageTestResult(buf) == (int)AAImageResult.AA_SN_NG)
                            {
                                Marking.WhiteBoardResult = false;
                            }
                            log.Info(buf);
                            step = 47;
                            break;
                        case 47://白板测试结果
                            if (Marking.WhiteBoardResult)
                            {
                                AppendText("点亮成功，识别结果为：OK");
                                Marking.WhiteBoardResult = true;
                                step = 60;
                            }
                            else if (WbLightOnNG && Marking.WhiteBoardResult == false)
                            {
                                if (wbCheckCount < 1)
                                {
                                    wbCheckCount++;
                                    AppendText("点亮失败，重新顶升气缸");
                                    step = 48;
                                }
                                else
                                {
                                    AppendText("再次点亮失败");
                                    Marking.WhiteBoardResult = false;
                                    Marking.WbGetResultFlg = true;
                                    step = 380;///////////////////////////NG 结束点胶流程
                                }
                            }
                            else
                            {
                                Marking.WhiteBoardResult = false;
                                Marking.WbGetResultFlg = true;
                                step = 380;///////////////////////////NG 结束点胶流程
                            }
                            break;
                        case 48://点亮失败重新顶升
                            AppendText("点胶解串板断电");
                            IoPoints.IDO14.Value = false;//断电
                            Thread.Sleep(10);
                            GlueUpCylinder.Reset();                                                      
                            step = 50;
                            break;
                        case 50://
                            if (GlueUpCylinder.OutOriginStatus)
                            {
                                Thread.Sleep(100);
                                GlueUpCylinder.Set();                               
                                step = 55;
                            }
                            break;   
                        case 55://气缸顶升后重新开始白板检测
                            if (GlueUpCylinder.OutMoveStatus)
                            {
                                Thread.Sleep(100);
                                AppendText("点胶解串板通电");
                                IoPoints.IDO14.Value = true;
                                step = 43;//重复上电
                            }
                            break;
                        #endregion
                        case 60:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if (GlueUpCylinder_Small.OutMoveStatus) step = 62;
                            }
                            else step = 62;
                            break;
                        case 62:// Z轴返回安全位
                            AppendText("点胶解串板断电");
                            IoPoints.IDO14.Value = false;//断电
                            Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, AxisParameter.Instance.RZspeed);
                            step = 65;
                            break;
                        #region 定位点胶
                        case 65://XY模组移至相机拍照位置   //定位位置
                            if (Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                            {
                                Xaxis.MoveTo(Position.Instance.GlueCameraPosition.X, AxisParameter.Instance.RXspeed);
                                Yaxis.MoveTo(Position.Instance.GlueCameraPosition.Y, AxisParameter.Instance.RYspeed);
                                step = 70;
                            }
                            break;
                        case 70://Z模组移至相机拍照位置
                            if (Yaxis.IsInPosition(Position.Instance.GlueCameraPosition.Y)
                                && Xaxis.IsInPosition(Position.Instance.GlueCameraPosition.X))
                            {
                                Zaxis.MoveTo(Position.Instance.GlueCameraPosition.Z, AxisParameter.Instance.RZspeed);
                                if (lightControl != null)
                                {
                                    lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueLocationVisionParam.LightControlvalue);
                                }
                                step = 80;
                            }
                            break;
                        case 80://CCD定位抓图
                            if (Zaxis.IsInPosition(Position.Instance.GlueCameraPosition.Z))
                            {
                                Thread.Sleep(AxisParameter.Instance.CameraDelayTriger);
                                Marking.GlueResult = false;//结果复位
                                if (!Marking.CCDShield)
                                {
                                    AppendText("点胶定位识别");
                                    Marking.CenterLocateTestFinish = false;
                                    //Thread.Sleep(Position.Instance.cameratime);
                                    //触发相机拍照
                                    if (baslerCamera.ho_Image != null)
                                    {
                                        if (baslerCamera.ho_Image.Key != IntPtr.Zero)
                                            baslerCamera.ho_Image.Dispose();
                                    }
                                    else
                                    {
                                        HOperatorSet.GenEmptyObj(out baslerCamera.ho_Image);
                                        baslerCamera.ho_Image.Dispose();
                                    }
                                    if (baslerCamera.ho_Image.Key == IntPtr.Zero)
                                    {
                                        Marking.LoadOffset.Result = "";
                                        glueVisionClass.VisionResult.Result = "";
                                        baslerCamera.SendSoftwareExecute();
                                        log.Info($"相机触发拍照！");

                                        _watch.Restart();
                                        step = 90;
                                    }
                                   
                                }
                                else { step = 90; }//Z轴返回安全位
                            }
                            break;
                     
                        case 90://接收数据
                            if (Marking.CCDShield /*|| stationOperate.SingleRunning*/)
                            {
                                Marking.LoadOffset = new Marking.Camera() { Result = "OK", X = 0, Y = 0, A = 0.0, D = 0.0 };
                                step = 100;
                            }
                            else
                            {
                                //判断相机是否读到图像，加载图像，运行处理
                                if (baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        glueVisionClass.UpdataImg(baslerCamera.ho_Image);
                                        glueVisionClass.VisionRun(DbModelParam.Instance.GlueLocationVisionParam,
                                            Relationship.Instance.CameraCalib);
                                        Marking.LoadOffset.Result = glueVisionClass.VisionResult.Result;
                                        Marking.LoadOffset.X = glueVisionClass.VisionResult.Column;
                                        Marking.LoadOffset.Y = glueVisionClass.VisionResult.Row;
                                        Marking.LoadOffset.A = glueVisionClass.VisionResult.Angle;
                                        Center.X = glueVisionClass.VisionResult.Column_Pixel;
                                        Center.Y = glueVisionClass.VisionResult.Row_Pixel;
                                        // });
                                        step = 100;
                                    }
                                    catch
                                    {
                                        Marking.LoadOffset.Result = "NG";
                                        Marking.LoadOffset.X = 0.0;
                                        Marking.LoadOffset.Y = 0.0;
                                        Marking.LoadOffset.A = 0.0;
                                        Center.X = 2048/2;
                                        Center.Y = 1944/2;
                                        log.Info($"镜筒相机处理异常！");
                                        step = 100;
                                    }
                                }
                            }
                            break;
                        case 100://读取相机处理数据
                            if (Marking.LoadOffset.Result != "")
                            {
                                if (Marking.LoadOffset.Result == "OK" /*|| stationOperate.SingleRunning*/)
                                {
                                    step = 200;
                                    Marking.LoadcationVisonResult = true;
                                    AppendText("视觉定位OK");
                                    res = "OK";
                                    GlueLoadTime = 0;
                                }
                                else
                                {
                                    Marking.LoadcationVisonResult = false;
                                    step = 380;/////////////
                                    res = "NG";
                                    log.Debug($"NG");
                                    if (GlueLoadTime < Position.Instance.GlueLoadNGTime) { GlueLoadTime++; }
                                    else { GlueLoadTime = 0;m_Alarm = PlateformAlarm.点胶定位连续NG; }
                                }
                            }
                            break;
                        #endregion
                        #region 空胶图
                        case 200://定位完成后，再拍照   拍无胶水前的 图片 
                                Xaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.X, AxisParameter.Instance.RXspeed);
                                Yaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Y, AxisParameter.Instance.RYspeed);
                                step = 210;
                            break;
                        case 210:
                            if (Yaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Y)
                                                            && Xaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.X))
                            {
                                Zaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Z, AxisParameter.Instance.RZspeed);
                                if (lightControl != null)
                                {
                                    lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, DbModelParam.Instance.GlueFindVisionParam.LightControlvalue);
                                }
                                step = 230;
                            }
                            break;
                        case 230://CCD定位抓图
                            if (Zaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Z))
                            {

                                //稳定后再拍照
                                Thread.Sleep(AxisParameter.Instance.CameraDelayTriger);
                                if (!Marking.CCDShield)
                                {
                                    AppendText("点胶拍摄空胶水照片");                           
                                    //Thread.Sleep(Position.Instance.cameratime);
                                    Marking.GetNoGlueImg = false;
                                    //触发相机拍照
                                    if (baslerCamera.ho_ImageNoglue != null)
                                    {
                                        if (baslerCamera.ho_ImageNoglue.Key != IntPtr.Zero)
                                            baslerCamera.ho_ImageNoglue.Dispose();
                                    }
                                    else
                                    {
                                        HOperatorSet.GenEmptyObj(out baslerCamera.ho_ImageNoglue);
                                        baslerCamera.ho_ImageNoglue.Dispose();
                                    }
                                    if (baslerCamera.ho_ImageNoglue.Key == IntPtr.Zero)
                                    {
                                        Marking.LoadOffset.Result = "";
                                        glueVisionClass.VisionResult.Result = "";
                                        baslerCamera.SendSoftwareExecute();
                                        log.Info($"空胶水相机触发拍照！");
                                        _watch.Restart();
                                        step = 240;
                                    }

                                }
                                else { step = 240; }//Z轴返回安全位
                            }
                            break;

                        case 240://接收数据
                            if (Marking.CCDShield || stationOperate.SingleRunning)
                            {                              
                                step = 380;
                            }
                            else
                            {
                                //判断相机是否读到图像，加载图像，运行处理
                                if (baslerCamera.ho_ImageNoglue.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        /// 
                                        ho_ImageT.Dispose();
                                        HOperatorSet.CopyImage(baslerCamera.ho_ImageNoglue,out ho_ImageT);
                                        Marking.GetNoGlueImg=glueVisionClass_GlueText.UpdataImg_Noglue(baslerCamera.ho_ImageNoglue);
                                        if (Marking.GetNoGlueImg)
                                        { step = 250;
                                            
                                        }
                                        else { step = 380; Marking.GlueFindVisionResult = false; AppendText("获取空胶水图片失败"); }//NG
                                       
           

                                    }
                                    catch
                                    {
                                        Marking.GlueFindVisionResult = false;
                                        AppendText("获取空胶水图片失败");
                                        step = 380;
                                    }
                                }
                            }
                            break;
                        #endregion
                        case 250://计算点胶位置
                            
                            Position.Instance.GlueCenterPosition.X = Position.Instance.GlueCameraPosition.X + AxisParameter.Instance.CameraAndNeedleOffset.X + Marking.LoadOffset.X + AxisParameter.Instance.GlueOffsetX ;
                            Position.Instance.GlueCenterPosition.Y = Position.Instance.GlueCameraPosition.Y + AxisParameter.Instance.CameraAndNeedleOffset.Y + Marking.LoadOffset.Y + AxisParameter.Instance.GlueOffsetY ; 
                            Position.Instance.GlueStartPosition.X = Position.Instance.GlueCenterPosition.X - Position.Instance.GlueRadius;
                            Position.Instance.GlueStartPosition.Y = Position.Instance.GlueCenterPosition.Y;

                            AppendText($"点胶位置中心X:{Position.Instance.GlueCenterPosition.X},Y{ Position.Instance.GlueCenterPosition.Y},半径{Position.Instance.GlueRadius}");
                              
                            step = 260;
                            break;
                        case 260:// Z轴返回安全位
                            Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, AxisParameter.Instance.RZspeed);
                            step = 270;

                            break;
                        #region 点胶
                        case 270:// XY轴点胶圆形轨迹起点
                            if (Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                            {
                                Xaxis.MoveTo(Position.Instance.GlueStartPosition.X, AxisParameter.Instance.RXspeed);
                                Yaxis.MoveTo(Position.Instance.GlueStartPosition.Y, AxisParameter.Instance.RYspeed);

                                step = 280;
                            }

                            break;
                        case 280://Z 点胶圆形轨迹起点
                            if (Xaxis.IsInPosition(Position.Instance.GlueStartPosition.X)
                                && Yaxis.IsInPosition(Position.Instance.GlueStartPosition.Y))
                            {
                                Zaxis.MoveTo(Position.Instance.GlueHeight, AxisParameter.Instance.RZspeed);
                                AppendText($"点胶高度GlueHeight:{Position.Instance.GlueHeight}");
                                step = 290;
                            }
                            break;
                        case 290://XYZ前往点胶圆形轨迹起点
                            if (Zaxis.IsInPosition(Position.Instance.GlueHeight))
                            {
                                int step1 = 0;
                                bool istrue = true;
                                while (istrue)
                                {

                                    switch (step1)
                                    {
                                        case 0://起始空胶
                                            APS168.APS_absolute_arc_move(2, new Int32[2] { Xaxis.NoId, Yaxis.NoId }, new Int32[2]
                                            { (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                                            (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                            (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.StartGlueAngle);
                                            Thread.Sleep(1);
                                            step1 = 10;
                                            break;
                                        case 10://点胶第一圈
                                            if (Xaxis.IsDone && Yaxis.IsDone && Zaxis.IsDone && Xaxis.CurrentSpeed == 0 && Yaxis.CurrentSpeed == 0 && Zaxis.CurrentSpeed == 0)
                                            {
                                                APS168.APS_absolute_arc_move(2, new Int32[2] { Xaxis.NoId, Yaxis.NoId }, new Int32[2]
                                                {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                                                (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                                (int)Position.Instance.GluePathSpeed * 1000, 360);
                                                if (Marking.GlueRun)
                                                    IoPoints.IDO19.Value = false;
                                                else
                                                {
                                                    _watch.Restart();
                                                    IoPoints.IDO19.Value = true;
                                                }
                                                Thread.Sleep(1);
                                                step1 = 20;
                                            }
                                            break;
                                        case 20://点胶第二圈
                                            if (Xaxis.IsDone && Yaxis.IsDone && Zaxis.IsDone && Xaxis.CurrentSpeed == 0 && Yaxis.CurrentSpeed == 0 && Zaxis.CurrentSpeed == 0)
                                            {
                                                APS168.APS_absolute_arc_move(2, new Int32[2] { Xaxis.NoId, Yaxis.NoId }, new Int32[2]
                                                {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                                                (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                                (int)Position.Instance.GluePathSpeed * 1000, Position.Instance.SecondGlueAngle);
                                                Thread.Sleep((int)Position.Instance.StopGlueDelay);
                                                step1 = 30;
                                            }
                                            break;
                                        case 30://点胶拖胶
                                            if (Xaxis.IsDone && Yaxis.IsDone && Zaxis.IsDone && Xaxis.CurrentSpeed == 0 && Yaxis.CurrentSpeed == 0 && Zaxis.CurrentSpeed == 0)
                                            {
                                                IoPoints.IDO19.Value = false;
                                                if (!Marking.GlueRun) { AppendText($"点胶电磁阀输出时间{_watch.ElapsedMilliseconds/1000.0}s"); _watch.Restart(); }
                                                else AppendText($"点胶电磁阀输出被屏蔽");
                                                APS168.APS_absolute_move(Zaxis.NoId, (int)((Zaxis.CurrentPos - Position.Instance.DragGlueHeight) / AxisParameter.Instance.RYTransParams.PulseEquivalent),
                                                    (int)Position.Instance.DragGlueSpeed * 1000);
                                                APS168.APS_absolute_arc_move(2, new Int32[2] { Xaxis.NoId, Yaxis.NoId }, new Int32[2]
                                                {  (int)((Position.Instance.GlueCenterPosition.X) / AxisParameter.Instance.RXTransParams.PulseEquivalent),
                                                (int)((Position.Instance.GlueCenterPosition.Y) / AxisParameter.Instance.RYTransParams.PulseEquivalent) },
                                                (int)Position.Instance.DragGlueSpeed * 1000, Position.Instance.DragGlueAngle);
                                                Thread.Sleep(1);
                                                step1 = 40;
                                            }
                                            break;
                                        case 40://点胶结束
                                            if (Xaxis.IsDone && Yaxis.IsDone && Zaxis.IsDone && Xaxis.CurrentSpeed == 0
                                                && Yaxis.CurrentSpeed == 0 && Zaxis.CurrentSpeed == 0)
                                            {
                                                IoPoints.IDO19.Value = false;
                                                istrue = false;
                                                step1 = 0;
                                            }
                                            break;

                                    }
                                }

                                step = 300;
                            }

                            break;
                        case 300: //点胶次数判断
                            if (Xaxis.IsDone && Yaxis.IsDone && Zaxis.IsDone && Xaxis.CurrentSpeed == 0 && Yaxis.CurrentSpeed == 0)
                            {
                                if (!Marking.GlueRun) Config.Instance.GlueUseNumsIndex++;
                                    Marking.GlueFinish = true;
                                    IoPoints.IDO19.Value = false;
                                    step = 310;
                            }

                            break;
                        case 310:// 判断是否CCD检测
                            if (Marking.GlueFinish && !Marking.CCDShield)
                                step = 320;//进行点胶检测
                            else if (Marking.GlueFinish && Marking.CCDShield)
                            {
                                Marking.GlueResult = true;
                                //Config.Instance.GlueProductOkTotal++;
                                step = 380;//流程结束
                            }

                            break;
                        #endregion
                        case 320:// Z轴返回安全位
                            Marking.GlueFinish = false;
                            Thread.Sleep(10);
                            Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, AxisParameter.Instance.RZspeed);
                            step = 330;
                            break;
                        #region 点胶识别
                        case 330:// XY轴前往拍照位置
                            if (Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                            {
                                Xaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.X, AxisParameter.Instance.RXspeed);
                                Yaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Y, AxisParameter.Instance.RYspeed);
                                step = 340;
                            }
                            break;
                        case 340:// Z轴前往拍照位置
                            if (Xaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.X)
                            && Yaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Y))
                            {
                                Zaxis.MoveTo(Position.Instance.GlueCheckCameraPosition.Z, AxisParameter.Instance.RZspeed);
                                step = 350;
                            }
                            break;
                        case 350://CCD拍照检测
                            if ( Zaxis.IsInPosition(Position.Instance.GlueCheckCameraPosition.Z))
                            {
                                //稳定后触发拍照信号 
                                Thread.Sleep(AxisParameter.Instance.CameraDelayTriger);                             
                                step = Marking.CCDShield ? 380 : 355;
                            }
                            break;
                        case 355://CCD定位抓图
                           
                            {
                                Marking.GlueResult = false;//结果复位
                                if (!Marking.CCDShield)
                                {
                                    AppendText("点胶识别");
                                    Marking.CenterLocateTestFinish = false;
                                    //Thread.Sleep(Position.Instance.cameratime);
                                    //触发相机拍照
                                    if (baslerCamera.ho_Image != null)
                                    {
                                        if (baslerCamera.ho_Image.Key != IntPtr.Zero)
                                            baslerCamera.ho_Image.Dispose();
                                    }
                                    else
                                    {
                                        HOperatorSet.GenEmptyObj(out baslerCamera.ho_Image);
                                        baslerCamera.ho_Image.Dispose();
                                    }
                                    if (baslerCamera.ho_Image.Key == IntPtr.Zero)
                                    {
                                        Marking.GlueOffset.Result = "";
                                        glueVisionClass_GlueText.VisionResult.Result = "";
                                        baslerCamera.SendSoftwareExecute();
                                        log.Info($"相机触发拍照！");

                                        _watch.Restart();
                                        step = 360;
                                    }

                                }
                                else { step = 360; }//Z轴返回安全位
                            }
                            break;

                        case 360://接收数据
                            if (Marking.CCDShield || stationOperate.SingleRunning)
                            {
                                Marking.GlueOffset = new Marking.Camera() { Result = "OK", X = 0, Y = 0, A = 0.0, D = 0.0 };
                                step = 370;
                            }
                            else
                            {
                                //判断相机是否读到图像，加载图像，运行处理
                                if (baslerCamera.ho_Image.Key != IntPtr.Zero)
                                {
                                    try
                                    {
                                        /// Task.Run(() => {
                                        glueVisionClass_GlueText.UpdataImg(baslerCamera.ho_Image);
                                        //glueVisionClass_GlueText.VisionRun(DbModelParam.Instance.GlueFindVisionParam,
                                        //    Relationship.Instance.CameraCalib);
                                        Marking.GlueOffset.Result= glueVisionClass_GlueText.GlueVisionAction(ho_ImageT, //glueVisionClass_GlueText.ho_Image_Noglue
                                            baslerCamera.ho_Image, Center.X,Center.Y,DbModelParam.Instance.GlueFindVisionParam)?"OK":"NG";

                                        if (DbModelParam.Instance.GlueFindVisionParam.NgImageSave&& Marking.GlueOffset.Result=="NG")//是否保存NG图
                                        {
                                            try
                                            {
                                                HOperatorSet.WriteImage(ho_ImageT, ("jpg"), (0), $"{AppConfig.VisionPictureFail_GlueFind}{DateTime.Now.ToString("HHmmss")}_GlueNo.jpg");
                                                HOperatorSet.WriteImage(baslerCamera.ho_Image, ("jpg"), (0), $"{AppConfig.VisionPictureFail_GlueFind}{DateTime.Now.ToString("HHmmss")}_Glue.jpg");

                                            }
                                            catch  { }
                                        }
                                        try { glueVisionClass_GlueText.SaveImgWindow($"{AppConfig.VisionPictureWindow}{DateTime.Now.ToString("HHmmss")}.jpg"); } catch { }
                                        // });
                                        step = 370;
                                    }
                                    catch
                                    {
                                        Marking.GlueOffset.Result = "NG";
                                     
                                        log.Info($"镜筒相机处理异常！");
                                        step = 370;
                                    }
                                }
                            }
                            break;
                        case 370://读取相机处理数据
                            if (Marking.GlueOffset.Result != "")
                            {
                                if (lightControl != null)
                                {
                                    lightControl.SetDigitalValue(VisionProductData.Instance.nLightChanel, 0);
                                }
                                if (Marking.GlueOffset.Result == "OK" || stationOperate.SingleRunning)
                                {
                                    step = 380;
                                    Marking.GlueResult = true;
                                    Marking.GlueFindVisionResult = true;
                                    log.Debug($"OK");
                                    res = "OK";
                                    GlueFindTime = 0;
                                    try { HOperatorSet.WriteImage(ho_ImageT, ("jpg"), (0), $"C:\\OK1.jpg");
                                          HOperatorSet.WriteImage(baslerCamera.ho_Image, ("jpg"), (0), $"C:\\OK2.jpg");
                                    } catch { }
                                }
                                else
                                {
                                    Marking.GlueResult = false;
                                    Marking.GlueFindVisionResult = false;
                                    try
                                    {
                                        HOperatorSet.WriteImage(ho_ImageT, ("jpg"), (0), $"C:\\NG1.jpg");
                                        HOperatorSet.WriteImage(baslerCamera.ho_Image, ("jpg"), (0), $"C:\\NG2.jpg");
                                       
                                    }
                                    catch { }
                                    step = 380;////////////////////////////
                                    res = "NG";
                                    log.Debug($"NG");
                                    if (GlueFindTime < Position.Instance.GlueFindNGTime) { GlueFindTime++; }
                                    else { GlueFindTime = 0; m_Alarm = PlateformAlarm.点胶识别连续NG; }
                                }
                            }
                            break;
                        #endregion
                        case 380://Z轴回原位  
                            Thread.Sleep(10);
                            Zaxis.MoveTo(Position.Instance.GlueSafePosition.Z, AxisParameter.Instance.RZspeed);
                            step = 390;
                            break;
                        case 390://检测Z轴回原位，判断XY轴是否回原位置
                            if (Zaxis.IsInPosition(Position.Instance.GlueSafePosition.Z))
                            {
                                watchGlueCT.Stop();
                                Marking.GlueCycleTime = watchGlueCT.ElapsedMilliseconds / 1000.0;
                                AppendText($"点胶工位CT:{Marking.GlueCycleTime}s");
                                step = 400;
                            }
                            break;
                        case 400://与AA通讯
                            if (!Marking.GlueRecycleRun)//不循环
                            {
                                Marking.AaSendCodeFlg = true;
                                _watch.Restart();
                                Marking.GlueWorkAlreadyRequest = true;//点胶工位完成信号
                                step = 410;
                            }
                            else//循环
                            {
                                GlueStopCylinder.Reset();
                                step = 450;//顶升气缸下降
                            }
                            break;
                        case 410://等待AA准备好
                            if (Marking.AAStockpAlreadyGetJigsFromGlue)
                            {
                                IoPoints.IDO9.Value = true;
                                #region data
                                try
                                {
                                    lock (MesData.GlueDataLock)
                                    {
                                        //MesData.glueData.GlueResult = Marking.GlueResult ? "OK" : "NG";
                                        //清洗

                                        //白板
                                        MesData.glueData.wbData.DefectStainTestResult = Marking.WhiteBoardResult_DetectStain;//脏污
                                        MesData.glueData.wbData.LightResult = Marking.WhiteBoardResult_Light;//白板点亮
                                        MesData.glueData.wbData.AllResult = Marking.WhiteBoardResult;
                                        //胶水
                                        MesData.glueData.glueVisionData.LoadcationResult = Marking.LoadcationVisonResult;
                                        MesData.glueData.glueVisionData.GlueFindResult = Marking.GlueFindVisionResult;
                                        if (!Position.Instance.IsUseGlueFind) Marking.GlueFindVisionResult = true;
                                        MesData.glueData.glueVisionData.AllResult = Marking.LoadcationVisonResult&&Marking.GlueFindVisionResult;/*&Marking.GlueFindVisionResult*/

                                        if (Marking.GlueShield) { MesData.glueData.wbData.AllResult=true; MesData.glueData.glueVisionData.AllResult = true; }
                                        MesData.glueData.AllglueStationResult =  MesData.glueData.wbData.AllResult && MesData.glueData.glueVisionData.AllResult;

                                        MesData.aaStockPileData.glueData = MesData.glueData;//传给下一段

                                        lock (MesData.MesDataLock)
                                        {
                                            if (MesData.MesDataList.ContainsKey(MesData.glueData.cleanData.JigsSN))
                                                MesData.MesDataList.Remove(MesData.glueData.cleanData.JigsSN);
                                            MesData.MesDataList.Add(MesData.glueData.cleanData.JigsSN, MesData.glueData);
                                        }
                                        //MesData.glueData.GlueResult = "";
                                        MesData.glueData.AllglueStationResult = false;
                                        MesData.glueData.cleanData.JigsSN = "";
                                        MesData.glueData.cleanData.HolderSN = "";
                                        MesData.glueData.cleanData.CleanResult_MesLock = false;
                                    }
                                }
                                catch
                                {
                                    m_Alarm = PlateformAlarm.数据放入链表失败;
                                }
                                #endregion
                                step = 415;
                                if (AxisParameter.Instance.IsUseTanZhenCyl) GlueUpCylinder_Small.Reset();
                            }  
                            break;
                        case 415:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                              if( GlueUpCylinder_Small.OutOriginStatus) step = 420;
                            }
                            else
                                step = 420;
                            break;
                        case 420://阻挡气缸下降 顶升气缸下降  
                            //IoPoints.IDO9.Value = false;//流水线停止  
                            GlueStopCylinder.Set();
                            GlueUpCylinder.Reset();
                            step = 425;
                            break;
                        case 425:
                            if (GlueUpCylinder.OutOriginStatus)
                            {
                                IoPoints.IDO9.Value = true;//流水线开启 
                               
                                step = 430;
                            }
                            break;
                        case 430:
                            if (!Marking.AAStockpAlreadyGetJigsFromGlue)//AA已接收到料
                            {
                                Marking.GlueWorkAlreadyRequest = false;//点胶工位完成信号 清除
                                step = 450;
                            }
                            break;
                        case 450:
                            if (!Marking.GlueRecycleRun)
                            {                                
                                GlueStopCylinder.Reset();//阻挡气缸
                                //Thread.Sleep(500);
                            }
                            step = 460;
                            break;
                        case 460://                                                          
                                if (Marking.GlueRecycleRun)
                                {
                                   
                                    step = 0;
                                }
                                else
                                {
                                    //Thread.Sleep(100);                               
                                    IoPoints.IDO9.Value = true;
                                    step = 470;
                                }                      
                            break;
                        case 470://                       
                            step = 0;
                            break;
                        default:
                            stationOperate.RunningSign = false;
                            step = 0;
                            IoPoints.IDO19.Value = false;
                            break;
                    }
                }
                #endregion
                #region 初始化运行流程
                if (stationInitialize.Running)
                {
                    switch (stationInitialize.Flow)
                    {
                        case 0://加载视觉参数
                          
                            var strLensmodelpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{DbModelParam.Instance.GlueLocationVisionParam.strModelPath}");                        
                            glueVisionClass.ReadShapeModel(strLensmodelpath);
                            if (Marking.IsLightControl||Marking.CCDShield) stationInitialize.Flow = 1;
                            else { stationInitialize.Flow = -1; AppendText("光源控制器未打开"); };
                            break;
                        case 1://清除所有标志位
                            stationInitialize.InitializeDone = false;
                            stationOperate.RunningSign = false;
                            //Marking.GlueCallIn = false;
                            //Marking.GlueWorking = false;
                            //Marking.GlueWorkFinish = false;
                            //Marking.GlueHoming = false;
                            //Marking.GlueCallOut = false;
                            //Marking.GlueCallOutFinish = false;
                            //Marking.GlueFinishBit = false;
                            //IoPoints.IDO19.Value = false;
                            //Marking.NeedleLocateTest = false;
                            //Marking.CenterLocateTest = false;
                            //Marking.GlueCheckTest = false;
                            //CenterCheckCount = 0;
                            //GlueCheckCount = 0;
                            Marking.GlueAlreadyGetJigsFromClean = false;//交互清除
                            Marking.GlueWorkAlreadyRequest = false;
                            wbCheckCount = 0;

                            step = 0;
                            Xaxis.Stop();
                            Yaxis.Stop();
                            Zaxis.Stop();
                            HomeTime = 0;
                            Thread.Sleep(200);
                            if (!Xaxis.IsAlarmed && !Yaxis.IsAlarmed && !Zaxis.IsAlarmed)
                            {
                                Xaxis.IsServon = false;
                                Yaxis.IsServon = false;
                                Zaxis.IsServon = false;
                                Xaxis.Clean();
                                Yaxis.Clean();
                                Zaxis.Clean();
                                CleanAlarm();                            
                                stationInitialize.Flow = 10;
                            }
                            break;
                        case 10://气缸复位
                            Zaxis.IsServon = true;
                            Xaxis.IsServon = true;
                            Yaxis.IsServon = true;
                            Thread.Sleep(200);
                            GlueStopCylinder.InitExecute();//阻挡
                            GlueStopCylinder.Reset();
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                GlueUpCylinder_Small.InitExecute();//探针
                                GlueUpCylinder_Small.Reset();
                        
                            }
                           
                            Marking.GlueXHomeFinish = false;
                            Marking.GlueYHomeFinish = false;
                            Marking.GlueZHomeFinish = false;
                            stationInitialize.Flow = 15;
                            break;
                        case 15:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if (GlueUpCylinder_Small.OutOriginStatus)
                                {
                                   
                                    GlueUpCylinder.InitExecute();//顶升
                                    GlueUpCylinder.Reset();
                                    _watch.Restart();
                                    stationInitialize.Flow = 20;
                                }
                            }
                            else
                            {
                                GlueUpCylinder.InitExecute();//顶升
                                GlueUpCylinder.Reset();
                                _watch.Restart();
                                stationInitialize.Flow = 20;
                            }
                            break;
                        case 20://启动IZ轴回原点
                            if (Zaxis.IsServon && GlueUpCylinder.OutOriginStatus)
                            {
                                BackHome(Zaxis, IoPoints.TDO2, IoPoints.TDI10);
                                stationInitialize.Flow = 30;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    stationInitialize.InitializeDone = false;
                                    _watch.Restart();
                                    AppendText("点胶顶升气缸感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 30://轴回零
                            if (Marking.GlueZHomeFinish)
                            {
                                BackHome(Xaxis, IoPoints.TDO0, IoPoints.TDI8);
                                BackHome(Yaxis, IoPoints.TDO1, IoPoints.TDI9);
                                stationInitialize.Flow = 40;
                                _watch.Restart();
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 10)
                                {
                                    _watch.Restart();
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.点胶Z轴回零异常;
                                    AppendText("点胶Z轴回零异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 40://XY轴回零
                            if (Marking.GlueXHomeFinish && Marking.GlueYHomeFinish)
                            {
                               
                                _watch.Restart();
                                stationInitialize.Flow = 50;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 30)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.点胶XY回零异常;
                                    AppendText("点胶XY回零异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                                            
                        case 50://复位完成
                            HomeTime++;
                            Thread.Sleep(100);
                            if (HomeTime < 1) {  stationInitialize.Flow = 20; break; }
                            
                            stationInitialize.Flow =60;                         
                            break;

                        case 60://移动安全位
                            AppendText("点胶移动安全位");
                            Xaxis.MoveTo(Position.Instance.GlueSafePosition.X, AxisParameter.Instance.RXspeed);
                            Yaxis.MoveTo(Position.Instance.GlueSafePosition.Y, AxisParameter.Instance.RYspeed);
                            
                            stationInitialize.Flow = 70;
                            break;
                        case 70:
                            if (Xaxis.IsInPosition(Position.Instance.GlueSafePosition.X)&&Yaxis.IsInPosition(Position.Instance.GlueSafePosition.Y))
                            {
                                stationInitialize.Flow = 80;

                            }
                            break;
                        case 80:
                            AppendText($"{Name}初始化完成！");
                            stationInitialize.Flow = 100;
                            stationInitialize.InitializeDone = true;//初始化完成标志
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                //clean all the enum of alarm information
                if (AlarmReset.AlarmReset)
                {
                    m_Alarm = PlateformAlarm.无消息;
                }
            }
        }
        public void CleanAlarm()
        {
            //报警清除
            IoPoints.AlarmDO0.Value = true;
            IoPoints.AlarmDO1.Value = true;
            IoPoints.AlarmDO2.Value = true;
            IoPoints.AlarmDO3.Value = true;
            IoPoints.AlarmDO4.Value = true;
            IoPoints.AlarmDO5.Value = true;
            IoPoints.AlarmDO6.Value = true;
            IoPoints.AlarmDO7.Value = true;
            Thread.Sleep(100);

            IoPoints.AlarmDO0.Value = false;
            IoPoints.AlarmDO1.Value = false;
            IoPoints.AlarmDO2.Value = false;
            IoPoints.AlarmDO3.Value = false;
            IoPoints.AlarmDO4.Value = false;
            IoPoints.AlarmDO5.Value = false;
            IoPoints.AlarmDO6.Value = false;
            IoPoints.AlarmDO7.Value = false;

        }
        private void run ()
        {
        }
        public bool BackHome_BD(ServoAxis axis, IoPoint Out, IoPoint In)
        {
            Stopwatch _repositoryWatch = new Stopwatch();
            _repositoryWatch.Start();
            bool isHome = false;
            if (!axis.IsServon) return false;
            Task.Factory.StartNew(() =>
            {
                try
                {            //判断Z轴是否在零点
                    Out.Value = true;
                    while (true)
                    {
                        Thread.Sleep(3);
                        if (In.Value == false)
                        {
                            Out.Value = false;
                            isHome = true;
                        }
                        if (isHome && In.Value)
                        {

                            axis.APS_set_command(0.0); AppendText($"{axis.Name}回零");
                            if (axis.NoId == 8) Marking.GlueXHomeFinish = true;
                            if (axis.NoId == 9) Marking.GlueYHomeFinish = true;
                            if (axis.NoId == 10) Marking.GlueZHomeFinish = true;
                          
                            return true;
                            break;
                        }

                        //_repositoryWatch.Stop();
                        if (_repositoryWatch.ElapsedMilliseconds / 1000 > 10)
                        {
                            Out.Value = false;
                          
                            AppendText($"回零NG");
                            return false;
                        }

                    }

                    return true;
                }
                catch (Exception ex)
                {
                   
                    //log.Fatal("设备驱动程序异常", ex);
                    return false;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
          
            return true;

        }
        public bool BackHome(ServoAxis axis, IoPoint Out, IoPoint In)
        {
            Stopwatch _repositoryWatch = new Stopwatch();
            _repositoryWatch.Start();
            bool isHome = false;
            if (!axis.IsServon) return false;
            Global.IsLocating = true;
            Task.Factory.StartNew(() =>
            {
                try
                {            //判断Z轴是否在零点
                    Out.Value = true;
                    while (true)
                    {
                        Thread.Sleep(3);
                        if (In.Value == false)
                        {
                            Out.Value = false;
                            isHome = true;
                            
                        }
                        if (isHome && In.Value)
                        {
                            isHome = false;
                            axis.APS_set_command(0.0); AppendText($"{axis.Name}回零");
                            if (axis.NoId == 8) Marking.GlueXHomeFinish = true;
                            if (axis.NoId == 9) Marking.GlueYHomeFinish = true;
                            if (axis.NoId == 10) Marking.GlueZHomeFinish = true;
                            Global.IsLocating = false;
                            return true;
                            break;
                        }

                        //_repositoryWatch.Stop();
                        if (_repositoryWatch.ElapsedMilliseconds / 1000 > 20)
                        {
                            Out.Value = false;
                            Global.IsLocating = false;
                            AppendText($"回零NG");
                            return false;
                        }

                    }


                    Global.IsLocating = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Global.IsLocating = false;
                    //log.Fatal("设备驱动程序异常", ex);
                    return false;
                }
            }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            Global.IsLocating = false;
            return true;

        }
        /// <summary>
        /// 流程报警集合
        /// </summary>
        protected override IList<Alarm> alarms()
        {
            var list = new List<Alarm>();
            list.AddRange(Xaxis.Alarms);
            list.AddRange(Yaxis.Alarms);
            list.AddRange(Zaxis.Alarms);
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.初始化故障) { AlarmLevel = AlarmLevels.Error, Name = PlateformAlarm.初始化故障.ToString() });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.顶升气缸未复位或平台不在原位启动)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.顶升气缸未复位或平台不在原位启动.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.点胶工位料盘检测有误)
            {
                AlarmLevel = AlarmLevels.Warrning,
                Name = PlateformAlarm.点胶工位料盘检测有误.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.CCD通讯超时)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.CCD通讯超时.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.CCD发送失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.CCD发送失败.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.CCD接收异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.CCD接收异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.结果发送失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.结果发送失败.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.AA通讯超时)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.AA通讯超时.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.接收到错误字符)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.接收到错误字符.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.胶水检测软件重新打开失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.胶水检测软件重新打开失败.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.测高偏差异常过大)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.测高偏差异常过大.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.白板检测超时)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.白板检测超时.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.点胶定位连续NG)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.点胶定位连续NG.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.点胶识别连续NG)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.点胶识别连续NG.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.白板工位Ready超时)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.白板工位Ready超时.ToString()
            });
            
            list.AddRange(GlueStopCylinder.Alarms);
            list.AddRange(GlueUpCylinder.Alarms);
            list.AddRange(GlueUpCylinder_Small.Alarms);
            return list;
        }
        /// <summary>
        /// Collections of cylinder's statuses
        /// </summary>
        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();
            list.Add(GlueStopCylinder);
            list.Add(GlueUpCylinder);
            list.Add(GlueUpCylinder_Small);
            return list;
        }
        public enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            顶升气缸未复位或平台不在原位启动,
            点胶工位料盘检测有误,
            执行条件不满足,
            气缸不在状态位,
            CCD通讯超时,
            CCD发送失败,
            CCD接收异常,
            结果发送失败,
            AA通讯超时,
            测高模块通讯超时,
            测高偏差异常过大,
            接收到错误字符,
            数据放入链表失败,
            胶水检测软件重新打开失败,
            白板检测超时,
            点胶Z轴回零异常,
            点胶XY回零异常,
            白板工位报警中,
            点胶定位连续NG,
            点胶识别连续NG,
            白板工位Ready超时
        }

        public enum AAImageSTATUS:int
        {
            AAImage_READY = 0,
            AAImage_TESETING,
            AAImage_WARMING,
        };

        public enum AAImageResult:int
        {
            AAIamge_PASS = 0,
            AAImage_LightON_NG,
            AAImage_Particle_NG,
            AA_SN_NG,
        };
        string WbIniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"ProjectConfig\\ProjectConfig.ini");
        public bool WbINItrans(string ModelName)
        {

            if (File.Exists(WbIniPath) == false)
            {
                MessageBox.Show($"白板参数文件不存在:{WbIniPath}");
                return false;
            }

            IniHelper.WriteValue("ProjectConfig", "ModelName", ModelName, WbIniPath);
            return true;
        }
        /// <summary>
        /// 打开自己开发的程序
        /// </summary>
        /// <param name="fileName">文件名称（比如C-MES.exe）</param>
        /// <param name="filePath">文件所在路径（比如G:\SoftWare\DMMES）</param>
        private int OpenOtherEXEMethod(string fileName, string filePath)
        {
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
            {
                if (!Directory.Exists(filePath)) return -1;
                //开启一个新process
                System.Diagnostics.ProcessStartInfo p = null;
                System.Diagnostics.Process proc;

                p = new System.Diagnostics.ProcessStartInfo(fileName);
                p.WorkingDirectory = filePath;//设置此外部程序所在windows目录
                proc = System.Diagnostics.Process.Start(p);//调用外部程序

            }

            return 0;

        }

        /// <summary>
        /// 关闭软件
        /// </summary>
        private void CloseSw()
        {
            //这个是判断，关闭
            //获得任务管理器中的所有进程
            Process[] p = Process.GetProcesses();
            foreach (Process p1 in p)
            {
                try
                {
                    string processName = p1.ProcessName.ToLower().Trim();
                    //判断是否包含阻碍更新的进程
                    if (processName == "胶水检测")
                        p1.Kill();
                }
                catch { }
            }
        }

    }

}
