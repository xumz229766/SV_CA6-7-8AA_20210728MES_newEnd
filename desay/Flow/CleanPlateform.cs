        using System.Collections.Generic;
using Motion.Enginee;
using Motion.Interfaces;
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using System.Toolkit;
using desay.ProductData;
using Motion.AdlinkAps;
using System.Text.RegularExpressions;
namespace desay.Flow
{
    /// <summary>
    /// 清洗平台控制流程
    /// </summary>
    public class CleanPlateform : StationPart
    {
        /// <summary>
        /// 报警信息枚举变量
        /// </summary>
        private PlateformAlarm m_Alarm;
        public CleanPlateform(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
                        : base(ExternalSign, stationIni, stationOpe, typeof(CleanPlateform)) { }
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
        /// 清洗阻挡气缸
        /// </summary>
        public SingleCylinder CleanStopCylinder { get; set; }
        /// <summary>
        /// 清洗顶升气缸
        /// </summary>
        public DoubleCylinder CleanUpCylinder { get; set; }
        /// <summary>
        /// 清洗旋转气缸
        /// </summary>
        public DoubleCylinder CleanRotateCylinder { get; set; }
        /// <summary>
        /// 清洗夹紧气缸
        /// </summary>
        public DoubleCylinder CleanClampCylinder { get; set; }
        /// <summary>
        /// 清洗上下气缸
        /// </summary>
        public DoubleCylinder CleanUpDownCylinder { get; set; }


     
        #endregion

        public event Action<int> SendRequest;

        public int CleanNum;           // 清洗次数
        public int CleanNum2;          // 清洗次数
        public bool CleanHomeBit;

        int wbCheckCount = 0;
        /// <summary>
        /// 与前机台通讯客户端
        /// </summary>
        public System.ToolKit.AsynTcpClient2 FormerStationClient { get; set; }
        public override void Running(RunningModes runningMode)
        {
            Stopwatch _repositoryWatch = new Stopwatch();
            _repositoryWatch.Start();
            var _watch = new Stopwatch();
            _watch.Start();
            var _watchBack = new Stopwatch();
            _watchBack.Start();
            var _PlasmaW = new Stopwatch();
            _PlasmaW.Start();
            bool AskBack = false;
            string ReceiveString = "";
            bool CleanTCPuse = false;
            bool PlasemaTrue = false;//
            bool PlasmaAlarm = false;
            bool PlasmaAlarmCheck = false;
            while (true)
            {
                Thread.Sleep(5);
                CleanStopCylinder.Condition.External = AlarmReset;
                CleanUpCylinder.Condition.External = AlarmReset;
                CleanClampCylinder.Condition.External = AlarmReset;
                CleanRotateCylinder.Condition.External = AlarmReset;
                CleanUpDownCylinder.Condition.External = AlarmReset;
              

                CleanHomeBit = IoPoints.IDI2.Value && IoPoints.IDI6.Value && IoPoints.IDI8.Value && IoPoints.IDI4.Value 
                            && IoPoints.IDI10.Value && Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z);
               
                #region 自动运行控制流程
                if (stationOperate.Running)
                {
                    Marking.CleanHaveProduct = (IoPoints.IDI0.Value || Marking.CleanHaveProductSheild) 
                        && CleanUpCylinder.OutOriginStatus;

                    if (Marking.GlueBackFlowGetEmptyJigsFromAABackFlow && CleanTCPuse == false) //回流有空治具 请求回到上料段
                    {
                        //Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;//只发一次？
                        if (AskBack)
                        {
                            if (_watchBack.ElapsedMilliseconds>3000)
                            {
                                _watchBack.Restart();
                                try
                                {
                                    FormerStationClient.AsynSend("AskBackJigs");
                                }
                                catch { }
                                AppendText("请求回流空治具");
                            }
                        }
                        else
                        {
                            _watchBack.Restart();
                            try { FormerStationClient.AsynSend("AskBackJigs"); } catch { }
                            AppendText("请求回流空治具");
                        }
                        AskBack = true;
                     

                    }
                    else AskBack = false;
                    try
                    {
                        if(Marking.CleanTCPUpMotion)//接收清空
                        {
                            Marking.CleanTCPUpMotion = false;
                            FormerStationClient.RevMessage = "";
                        }
                        if (FormerStationClient.RevMessage.Contains("HaveGetBackJigs")) //上料段完成 收回空治具 
                        {
                            FormerStationClient.RevMessage = "";
                            Marking.GlueBackFlowFinishToUpMotion = true;
                            Marking.UpMotionReadyGlueBackFlow = false;//10.24新增
                            AppendText("上料段完成 收回空治具");
                        }
                        if (FormerStationClient.RevMessage.Contains("AlreayBackJigs")) //上料段等待回流 
                        {
                            FormerStationClient.RevMessage = "";
                            Marking.UpMotionReadyGlueBackFlow = true;
                            AppendText("上料段等待回流");
                        }
                    }
                    catch { }
                    #region 判断Plasma 是否处于工作中
                    if (Marking.PlasmaWorking)
                    {

                        if (PlasemaTrue && PlasmaAlarmCheck == false)
                        {
                            if (_PlasmaW.ElapsedMilliseconds / 1000.0 > AxisParameter.Instance.PlasmaWorkingTime)
                            {
                                _PlasmaW.Restart();
                                PlasemaTrue = false;
                                if (IoPoints.IDI15.Value == false)
                                {
                                    PlasmaAlarm = true;
                                }
                                else PlasmaAlarm = false;
                                PlasmaAlarmCheck = true;
                            }
                        }
                        else _PlasmaW.Restart();
                        PlasemaTrue = true;
                    }
                    else PlasemaTrue = false;

                    #endregion
                    switch (step)
                    {
                        case 0: //判断平台是否在原点，顶升是否在原位
                            Marking.OnceCleanFinish = false;
                            Marking.TwiceCleanFinish = false;
                            CleanTCPuse = false;
                            IoPoints.IDO16.Value = false;//plasma 
                            if (Marking.PlasmaOn&&!IoPoints.IDO11.Value) IoPoints.IDO11.Value = true;
                            else IoPoints.IDO11.Value = false;//上电
                            step = 10;
                            break;;
                        case 10:  //复位所有气缸的动作                          
                            CleanStopCylinder.Reset();
                            CleanClampCylinder.Reset();

                            Thread.Sleep(10);
                            if (CleanClampCylinder.OutOriginStatus)
                            {
                                CleanUpDownCylinder.Reset();
                                Thread.Sleep(10);
                            }
                            if (CleanUpDownCylinder.OutOriginStatus)
                            {
                                CleanRotateCylinder.Reset();
                                Thread.Sleep(10);
                                CleanUpCylinder.Reset();
                                Thread.Sleep(10);
                                step = 20;
                            }
                            break;
                        case 20:    //判断所有气缸到位，启动Z轴回安全位
                            if (Zaxis.IsServon && CleanUpCylinder.OutOriginStatus && CleanUpDownCylinder.OutOriginStatus)
                            {
                                Zaxis.MoveTo(Position.Instance.CleanSafePosition.Z, AxisParameter.Instance.LZspeed);
                                step = 30;
                            }
                            break;
                        case 30://判断Z轴是否在安全位置
                            if (Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z))
                            {
                                step = 35;//已准备好
                                Marking.CleanResult_MesLock = true;
                            }
                            break;
                        case 35:
                            if (Marking.FormerStationShield)//屏蔽上一个工位
                            {
                                IoPoints.IDO9.Value = true;//送料线开启
                                log.Debug("屏蔽上一个工位送料线开启");
                                step = 65;///////////
                                MesData.cleanData.JigsSN = AxisParameter.Instance.JigsSN;
                                MesData.cleanData.HolderSN = AxisParameter.Instance.HolderSN;
                                _repositoryWatch.Restart();
                            }
                            else
                            {
                                if (!FormerStationClient.IsConnected) { m_Alarm = PlateformAlarm.与上料机未连接成功; step = 0; }
                                else { step = 40; AppendText("请求上料段上治具料"); }//已准备好
                            }
                            break;
                        case 40://  与上一个工位 
                            FormerStationClient.AsynSend("AskJigs");                         
                            IoPoints.IDO9.Value = true;//送料线开启
                            step = 50;
                            _watch.Restart();
                            break;
                        case 50:
                            ReceiveString = "";
                            if (FormerStationClient.RevMessage != "")
                            {
                                ReceiveString = FormerStationClient.RevMessage.Replace("\0", "");
                            }
                            if ((ReceiveString == "AlreadyPutJigs" || Marking.FormerStationShield))
                            {
                                _repositoryWatch.Restart();
                                AppendText("上料段响应上料中:" + ReceiveString);
                                CleanTCPuse = true;//TCP
                                step = 55;
                                break;
                            }
                            if (_watch.ElapsedMilliseconds/1000>15)
                            {
                                _watch.Restart();
                                step = 40;
                            }
                            break;
                        case 55://请求发二维码
                            Thread.Sleep(10);
                            FormerStationClient.AsynSend("AskSN");
                            AppendText("请求发送二维码");
                            Thread.Sleep(20);
                            _watch.Restart();
                            step = 60;
                            break;
                        case 60:
                            ReceiveString = "";
                            if (FormerStationClient.RevMessage != "")
                            {
                                ReceiveString = FormerStationClient.RevMessage.Replace("\0", "");
                            }
                            if ((ReceiveString.Contains ("SN") || Marking.FormerStationShield))
                            {
                                try
                                {
                                    string[] temp = ReceiveString.Split(':');
                                    log.Debug($"收到二维码信息{ReceiveString}");
                                    if (temp[1] != "" && temp[2] != "")
                                    {
                                        MesData.cleanData.JigsSN = temp[1];//治具码
                                        MesData.cleanData.HolderSN = temp[2];//产品码
                                        AppendText("收到二维码信息:" + ReceiveString);
                                        AppendText($"治具码:{MesData.cleanData.JigsSN} 产品码：{MesData.cleanData.HolderSN}");
                                        step = 65;
                                    }
                                    else { m_Alarm = PlateformAlarm.上料机未发送二维码或为空; step = 310; Marking.CleanResult_MesLock = false; AppendText("二维码NG！"); }
                                }
                                catch { m_Alarm = PlateformAlarm.上料机未发送二维码或为空; step = 310; Marking.CleanResult_MesLock = false; AppendText("二维码NG！"); }
                               
                            }
                           
                            break;
                        case 65://等待治具到位
                            if (IoPoints.IDI0.Value)
                            {
                                if (Marking.FormerStationShield) Thread.Sleep(AxisParameter.Instance.CleanPosDelay);//到位感应延时
                                else
                                {
                                  
                                    Thread.Sleep(AxisParameter.Instance.CleanPosDelay);//到位感应延时

                                }
                                step = 70;                                
                            }
                            break;
                        case 70://判断Plasma是否启用
                            if (Marking.CleanShield)
                            { step = 310;
                                if (!Marking.FormerStationShield)
                                {
                                    AppendText($"告诉上料段已经取到物料");
                                    //告诉上料段已经取到物料
                                    FormerStationClient.AsynSend("HaveGetJigs");//
                                    Thread.Sleep(100);
                                }
                            }//结束清洗
                            else
                                step = 75;//执行Plasma流程
                            break;
                        case 75://判断Mes互锁
                            try
                            {
                                if (Position.Instance.IsUseMesLock==false)
                                {
                                    AppendText($"Mes互锁已屏蔽跳过");
                                    step = 80;//执行Plasma流程
                                    break;
                                }
                                if (IsNumAndEnCh(MesData.cleanData.HolderSN)==false)
                                {
                                    MesData.cleanData.HolderSN = Regex.Replace(MesData.cleanData.HolderSN, @"[^a-zA-Z0-9]", "");//20210616 6线扫码有特殊符号特殊处理
                                    AppendText($"SN不是只包含数字字母而有特殊符号，校正为{ MesData.cleanData.HolderSN}");
                                }
                                                             
                                string SN = $"{Position.Instance.A2C.Trim()}{0}{MesData.cleanData.HolderSN.Trim().Substring(MesData.cleanData.HolderSN.Length - 6)}";
                                AppendText($"SN：{SN}");
                                //新增判断A2C后六位与SN前6位匹配
                                if (Position.Instance.A2C.Trim().Substring(Position.Instance.A2C.Length - Config.Instance.A2CSNCheckLength) == MesData.cleanData.HolderSN.Trim().Substring(0, Config.Instance.A2CSNCheckLength))
                                {
                                    AppendText($"A2C后6位与SN前6位匹配");
                                    if (Position.Instance.MesMode)
                                    {
                                        AppendText($"Mes模式入站");
                                        if ( Common.MesMoveIn(SN, Marking.SvJobNumber))
                                            step = 80;//执行Plasma流程
                                         else
                                        {
                                            step = 310; Marking.CleanResult_MesLock = false; AppendText("Mes互锁NG！"); CleanUpCylinder.Set();
                                            if (!Marking.FormerStationShield) { FormerStationClient.AsynSend("HaveGetJigs"); Thread.Sleep(50); }
                                        }//
                                    }
                                    else//InterLock
                                    {
                                        if (Common.Test_ScanSN(Position.Instance.IsUseMesLock, SN))
                                            step = 80;//执行Plasma流程
                                        else
                                        {
                                            step = 310; Marking.CleanResult_MesLock = false; AppendText("Mes互锁NG！"); CleanUpCylinder.Set();
                                            if (!Marking.FormerStationShield) { FormerStationClient.AsynSend("HaveGetJigs"); Thread.Sleep(50); }
                                        }//
                                    }
                                   
                                }
                                else
                                {
                                    AppendText($"A2C后六位{Position.Instance.A2C.Trim().Substring(Position.Instance.A2C.Length - Config.Instance.A2CSNCheckLength)}与SN前6位{MesData.cleanData.HolderSN.Trim().Substring(0, Config.Instance.A2CSNCheckLength)}不匹配");
                                    step = 310; Marking.CleanResult_MesLock = false; AppendText($"Mes互锁NG！"); CleanUpCylinder.Set();
                                    if (!Marking.FormerStationShield) { FormerStationClient.AsynSend("HaveGetJigs"); Thread.Sleep(50); }
                                }
                               
                            }
                            catch (Exception ex){
                                step = 310; Marking.CleanResult_MesLock = false; AppendText($"报错{ex.Message}_Mes互锁NG！"); CleanUpCylinder.Set();
                                if (!Marking.FormerStationShield) { FormerStationClient.AsynSend("HaveGetJigs"); Thread.Sleep(50); }
                            }                            
                            break;
                        case 80: //顶升气缸顶起                        
                            
                             //Marking.CleanCycleTime = 0;
                            //Thread.Sleep(100);
                             CleanUpCylinder.Set(); //顶升 
                            if (!Marking.FormerStationShield)
                            {
                                AppendText($"告诉上料段已经取到物料");
                                //告诉上料段已经取到物料
                                FormerStationClient.AsynSend("HaveGetJigs");//
                                Thread.Sleep(100);
                            }                            
                            step = 90;   
                            break;
                        case 90:
                            //CleanUpCylinder.Set(); //顶升
                            if (CleanUpCylinder.OutMoveStatus)
                            {
                                if (!Marking.FormerStationShield) FormerStationClient.AsynSend("HaveGetJigs");//确保收到                                
                                step = 100;
                            }                           
                            break;                                         
                        case 100://Z轴移至清洗安全位
                            Zaxis.MoveTo(Position.Instance.CleanSafePosition.Z, AxisParameter.Instance.LZspeed);
                            step = 110;//执行Plasma流程
                            //增加可屏蔽镜头镜座
                            if(Position.Instance.CleanHolderShield) step = 148;//跳过清洗镜座
                            AppendText("执行Plasma流程");
                            break;

                        case 110://XYZ前往镜筒清洗圆形轨迹起点
                            if (Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z))
                            {
                                CleanTCPuse = false;
                                Xaxis.MoveTo(Position.Instance.CleanConeFirstPosition.X, AxisParameter.Instance.LXspeed);
                                Yaxis.MoveTo(Position.Instance.CleanConeFirstPosition.Y, AxisParameter.Instance.LYspeed);
                                CleanNum = 0;
                               
                                step = 120;
                            }
                            break;
                        case 120://Z轴移动到镜筒清洗位
                            if (Xaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.X)
                                && Yaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Y))
                            {
                                Zaxis.MoveTo(Position.Instance.CleanConeFirstPosition.Z, AxisParameter.Instance.LZspeed);
                                step = 130;
                            }
                            break;
                        case 130://XYZ 1次清洗镜筒  西威改成角度
                            if (Zaxis.IsInPosition(Position.Instance.CleanConeFirstPosition.Z))
                            {
                                Int32 Dimension = 2;
                                Int32[] Axis_ID_Array_For_2Axes_ArcMove = new Int32[2] { Xaxis.NoId, Yaxis.NoId };
                                Int32[] Center_Pos_Array = new Int32[2] { Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.X / AxisParameter.Instance.LXTransParams.PulseEquivalent),
                                    Convert.ToInt32(Position.Instance.CleanConeCenterPositionReal.Y/ AxisParameter.Instance.LYTransParams.PulseEquivalent) };
                                //Int32 Max_Arc_Speed = 20000;
                                //Int32 Angle = 360;
                                if (Marking.CleanRun)
                                    IoPoints.IDO16.Value = false;
                                else
                                { IoPoints.IDO16.Value = true; Thread.Sleep(Position.Instance.CleanFireTime); PlasmaAlarmCheck = false; Marking.PlasmaWorking = true; }
                                APS168.APS_absolute_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, (int)Position.Instance.CleanPathSpeed * (int)AxisParameter.Instance.LYTransParams.EquivalentPulse, Position.Instance.HoldersCleanAngle);
                                step = 140;
                                //Thread.Sleep(10);
                            }
                            break;
                        case 140://清洗次数判断
                            if (Xaxis.IsDone && Yaxis.IsDone)   //&& Xaxis.IsAcrStop && Yaxis.IsAcrStop
                            {
                                Marking.PlasmaWorking = false;
                                Marking.OnceCleanFinish = true;
                                IoPoints.IDO16.Value = false;
                                step = 145;
                                Thread.Sleep(10);
                            }
                            break;
                        ///报警
                        case 145:
                            if (PlasmaAlarm == true && Marking.PlasmaAlarmShield == false)
                            {
                                PlasmaAlarm = false;
                                m_Alarm = PlateformAlarm.Plasma工作火焰可见光异常请确认是否工作出火焰或屏蔽;
                            }
                            else
                            {
                                step = 148;
                            }
                            break;
                        case 148://屏蔽清洗镜头
                            if(Position.Instance.CleanLensShield) step = 320;
                            else step = 150;
                            break;
                        case 150://判断Z轴是否在安全位置  
                            CleanNum = 0;
                            Zaxis.MoveTo(Position.Instance.CleanSafePosition.Z, AxisParameter.Instance.LZspeed);
                            CleanUpDownCylinder.Set();//清洗上下气缸下降
                            step = 160;
                            break;
                        case 160:
                            if (CleanUpDownCylinder.OutMoveStatus)
                            {
                                Thread.Sleep(10);
                                CleanClampCylinder.Set();//清洗夹紧气缸夹紧
                                step = 180;
                            }
                            break;
                        case 180:
                            if ( CleanClampCylinder.OutMoveStatus)
                            {
                                Thread.Sleep(10);
                                CleanUpDownCylinder.Reset();//清洗上下气缸上升
                                step = 190;
                            }
                            break;
                        case 190:
                            if (CleanUpDownCylinder.OutOriginStatus)
                            {
                                Thread.Sleep(10);
                                CleanRotateCylinder.Set();//清洗翻转气缸翻转180
                                step = 210;
                            }
                            break;
                        case 210://XY 2次前往清洗圆形轨迹起点
                            if (Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z) && CleanRotateCylinder.OutMoveStatus)//此处才判断Z轴安全位 优化时间
                            {
                               
                                Xaxis.MoveTo(Position.Instance.CleanLensFirstPosition.X, AxisParameter.Instance.LXspeed);
                                Yaxis.MoveTo(Position.Instance.CleanLensFirstPosition.Y, AxisParameter.Instance.LYspeed);
                                CleanNum = 0;
                                step = 220;
                            }

                            break;
                        case 220://XY 2次前往清洗圆形轨迹起点
                            if (Xaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.X)
                                && Yaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Y))
                            {
                                Zaxis.MoveTo(Position.Instance.CleanLensFirstPosition.Z, AxisParameter.Instance.LZspeed);
                                CleanNum = 0;
                                step = 240;
                            }

                            break;
                        case 240://XYZ 2次清洗镜头
                            if (Zaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Z))
                            {
                                Int32 Dimension2 = 2;
                                Int32[] Axis_ID_Array_For_2Axes_ArcMove2 = new Int32[2] { Xaxis.NoId, Yaxis.NoId };
                                Int32[] Center_Pos_Array2 = new Int32[2] { Convert.ToInt32(Position.Instance.CleanLensCenterPositionReal.X/ AxisParameter.Instance.LXTransParams.PulseEquivalent),
                                    Convert.ToInt32(Position.Instance.CleanLensCenterPositionReal.Y/ AxisParameter.Instance.LYTransParams.PulseEquivalent) };
                                if (Marking.CleanRun)
                                    IoPoints.IDO16.Value = false;
                                else
                                { IoPoints.IDO16.Value = true;Thread.Sleep(Position.Instance.CleanLensFireTime); PlasmaAlarmCheck = false; Marking.PlasmaWorking = true; }
                                if (Xaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.X) && Yaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Y)
                                    && Zaxis.IsInPosition(Position.Instance.CleanLensFirstPosition.Z))
                                {
                                    // APS168.APS_relative_arc_move(Dimension, Axis_ID_Array_For_2Axes_ArcMove, Center_Pos_Array, Max_Arc_Speed, Angle);
                                    APS168.APS_absolute_arc_move(Dimension2, Axis_ID_Array_For_2Axes_ArcMove2, Center_Pos_Array2, (int)Position.Instance.CleanPathSpeed * (int)AxisParameter.Instance.LYTransParams.EquivalentPulse, Position.Instance.LensCleanAngle);
                                    step = 250;
                                    //Thread.Sleep(10);
                                }
                            }
                            break;
                        case 250://清洗次数判断   
                            if (Xaxis.IsDone && Yaxis.IsDone)   //&& Xaxis.IsAcrStop && Yaxis.IsAcrStop
                            {
                                Marking.PlasmaWorking = false;
                                Marking.TwiceCleanFinish = true;
                                IoPoints.IDO16.Value = false;
                                step = 255;
                            }
                            break;
                        ///报警
                        case 255:
                            if (PlasmaAlarm == true && Marking.PlasmaAlarmShield == false)
                            {
                                PlasmaAlarm = false;
                                m_Alarm = PlateformAlarm.Plasma工作火焰可见光异常请确认是否工作出火焰或屏蔽;
                            }
                            else
                            {
                                step = 260;
                            }
                            break;
                        case 260://判断Z轴是否在回原位  
                            CleanNum2 = 0;
                            Zaxis.MoveTo(Position.Instance.CleanSafePosition.Z, AxisParameter.Instance.LZspeed);
                            step = 270;
                            break;
                        case 270:
                            CleanNum = 0;
                            if (Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z) )
                            {
                                CleanRotateCylinder.Reset();//清洗翻转气缸返回原位
                                step = 280;
                            }
                            break;
                        case 280:
                            if (CleanRotateCylinder.OutOriginStatus)
                            {
                                Thread.Sleep(10);
                                CleanUpDownCylinder.Set();//清洗上下气缸下降
                                step = 290;
                            }
                            break;
                        case 290:
                            if (CleanUpDownCylinder.OutMoveStatus )
                            {
                                Thread.Sleep(10);
                                CleanClampCylinder.Reset();//清洗夹紧气缸松开                               
                                step = 300;
                            }
                            break;
                        case 300:
                            if (CleanClampCylinder.OutOriginStatus )
                            {
                                Thread.Sleep(10);
                                CleanUpDownCylinder.Reset();//清洗上下气缸上升
                                Thread.Sleep(50);
                                step = 310;
                            }
                            break;
                        case 310:
                            if (Marking.CleanResult_MesLock == false) { }
                             step = 320;//清洗结束
                            break;                     
                        case 320:// Z轴返回安全位                            
                            Zaxis.MoveTo(Position.Instance.CleanSafePosition.Z, AxisParameter.Instance.RZspeed);                            
                            step = 330;//
                            break;
                        case 330:
                            if (Zaxis.IsInPosition(Position.Instance.CleanSafePosition.Z))
                            {
                                step = 390;//
                            }
                            break; 
                        case 390:
                            if (!Marking.CleanRecycleRun)
                            {
                                Marking.CleanWorkAlreadyRequest = true;//完成
                                step = 400;
                            }
                            else 
                            {
                                Thread.Sleep(2000);
                                step = 420;//循环
                            }
                            break;
                        case 400:
                            if (Marking.GlueAlreadyGetJigsFromClean/*||stationOperate.SingleRunning*/) //点胶工位准备好
                            {
                                //IoPoints.IDO9.Value = false;//流水线停止
                                CleanStopCylinder.Set();// 阻挡气缸下降
                                CleanUpCylinder.Reset();//顶升气缸下降
                                lock (MesData.CleanDataLock)//提前1008
                                {

                                    MesData.cleanData.CleanResult_MesLock = Marking.CleanResult_MesLock;
                                    lock (MesData.GlueDataLock)
                                    {
                                        MesData.glueData.cleanData.HolderSN = MesData.cleanData.HolderSN;
                                        MesData.glueData.cleanData.JigsSN = MesData.cleanData.JigsSN;
                                        MesData.glueData.cleanData.CleanResult_MesLock = MesData.cleanData.CleanResult_MesLock;
                                    }
                                    MesData.cleanData.HolderSN = "";
                                    MesData.cleanData.JigsSN = "";
                                    MesData.cleanData.CleanResult_MesLock = false;
                                }
                                //流水线开启
                                step = 405;
                            }
                            break;
                        case 405:
                            if (CleanUpCylinder.OutOriginStatus)
                            {
                                IoPoints.IDO9.Value = true;//流水线开启
                               
                                step = 410;
                            }
                            break;                         
                        case 410:
                            if (!Marking.GlueAlreadyGetJigsFromClean /*|| stationOperate.SingleRunning*/)//点胶接收到料
                            {
                                if(stationOperate.SingleRunning) IoPoints.IDO9.Value = false;
                                CleanStopCylinder.Reset();//
                                Marking.CleanWorkAlreadyRequest = false;//结束
                                step = 420;
                            }
                            break;

                        case 420://顶升气缸下降(清洗结束)                        
                            {                                                             
                                _repositoryWatch.Stop();
                                Marking.CleanCycleTime = _repositoryWatch.ElapsedMilliseconds / 1000.0;
                                AppendText($"Plasma流程结束，时间{Marking.CleanCycleTime}s");
                                if (Marking.CleanRecycleRun)
                                {
                                    CleanStopCylinder.Reset();                              
                                    Thread.Sleep(10);
                                    step = 0;
                                }
                                else
                                    step = 430;

                            }
                            break;
                        case 430://阻挡气缸复位                           
                            step = 0;
                            break;
                        default:
                            stationOperate.RunningSign = false;
                            step = 0;
                            break;
                    }
                }
                #endregion

                #region 初始化控制流程
                if (stationInitialize.Running)
                {
                    switch (stationInitialize.Flow)
                    {

                        case 0://清除所有标志位
                            stationInitialize.InitializeDone = false;
                            stationOperate.RunningSign = false;
                     
                            IoPoints.IDO16.Value = false;//Plasma启动继电器
                            Marking.WbCheckAgainFlg = false;
                            Marking.WbRequestResultFlg = false;
                            Marking.WhiteBoardResult = false;
                            Marking.CleanWorkAlreadyRequest = false;//交互清除
                            Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;
                            Marking.UpMotionReadyGlueBackFlow = false ;
                            Marking.GlueBackFlowFinishToUpMotion = false;
                            step = 0;
                            Xaxis.Stop();
                            Yaxis.Stop();
                            Zaxis.Stop();
                            //流水线停止？
                            IoPoints.IDO9.Value = false;
                            Thread.Sleep(50);
                            if (!Xaxis.IsAlarmed && !Yaxis.IsAlarmed && !Zaxis.IsAlarmed)
                            {
                                Xaxis.IsServon = false;
                                Yaxis.IsServon = false;
                                Zaxis.IsServon = false;
                                Xaxis.Clean();
                                Yaxis.Clean();
                                Zaxis.Clean();
                                CleanAlarm();//报警清除 IO控制
                                Thread.Sleep(200);
                                stationInitialize.Flow = 10;
                                Thread.Sleep(200);
                            }
                            break;
                        case 10://气缸复位
                            Xaxis.IsServon = true;
                            Yaxis.IsServon = true;
                            Zaxis.IsServon = true;
                            Thread.Sleep(200);
                            Marking.CleanXHomeFinish = false;//清除回零标志
                            Marking.CleanYHomeFinish = false;
                            Marking.CleanZHomeFinish = false;
                            BackHome(Zaxis, IoPoints.TDO5, IoPoints.TDI13);
                            stationInitialize.Flow =20;
                            _watch.Restart();
                            break;
                        case 20://轴回零
                            if (Marking.CleanZHomeFinish)
                            {
                               BackHome(Xaxis, IoPoints.TDO3, IoPoints.TDI11);
                               BackHome(Yaxis, IoPoints.TDO4, IoPoints.TDI12);
                               stationInitialize.Flow  = 40;
                                _watch.Restart();
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds/1000>10)
                                {
                                    _watch.Restart();
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗Z轴回零异常;
                                    AppendText("清洗Z轴回零异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 40://XY轴回零
                            if (Marking.CleanXHomeFinish&&Marking.CleanYHomeFinish)
                            {
                                CleanStopCylinder.InitExecute();//初始化气缸
                                CleanStopCylinder.Reset();//阻挡
                                CleanClampCylinder.InitExecute();
                                CleanClampCylinder.Reset();//夹紧
                                _watch.Restart();
                                stationInitialize.Flow = 50;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 10)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗XY回零异常;
                                    AppendText("清洗XY回零异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 50:
                            if (CleanClampCylinder.OutOriginStatus)
                            {
                                CleanRotateCylinder.InitExecute();
                                CleanRotateCylinder.Reset();//旋转
                                _watch.Restart();
                                stationInitialize.Flow = 60;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 10)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗夹紧气缸动作感应异常;
                                    AppendText("清洗夹紧气缸动作感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 60:
                            if (CleanRotateCylinder.OutOriginStatus)
                            {
                                CleanUpDownCylinder.InitExecute();
                                CleanUpDownCylinder.Reset();//上下气缸
                                _watch.Restart();
                                stationInitialize.Flow = 70;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗旋转气缸动作感应异常;
                                    AppendText("清洗旋转气缸动作感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 70:
                            if (CleanUpDownCylinder.OutOriginStatus)
                            {
                                CleanUpCylinder.InitExecute();
                                CleanUpCylinder.Reset();
                                _watch.Restart();
                                stationInitialize.Flow = 80;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗上下气缸动作感应异常;
                                    AppendText("清洗上下气缸动作感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 80:
                            if (CleanUpCylinder.OutOriginStatus)
                            {                           
                                _watch.Restart();
                                stationInitialize.Flow = 90;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    stationInitialize.InitializeDone = false;
                                    //m_Alarm = PlateformAlarm.清洗顶升气缸动作感应异常;
                                    AppendText("清洗顶升气缸动作感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;                
                    
                        case 90://复位完成
                           
                            Thread.Sleep(100);
                           
                            stationInitialize.Flow = 100;
                            break;
                        case 100://移至安全位
                            Xaxis.MoveTo(Position.Instance.CleanSafePosition.X, AxisParameter.Instance.LXspeed);
                            Yaxis.MoveTo(Position.Instance.CleanSafePosition.Y, AxisParameter.Instance.LYspeed);
                            Thread.Sleep(100);
                            stationInitialize.Flow = 110;
                            break;
                        case 110:
                            if (Xaxis.IsInPosition(Position.Instance.CleanSafePosition.X) && Yaxis.IsInPosition(Position.Instance.CleanSafePosition.Y))
                            {
                                stationInitialize.InitializeDone = true;
                                AppendText($"{Name}初始化完成！");
                                stationInitialize.Flow = 200;
                            }
                            break;
                        case 120:
                            IoPoints.IDO9.Value = true;//送料线开启
                            Thread.Sleep(500);
                            IoPoints.IDO9.Value = false;//送料线开启
                            break;
                        //case 130:
                        //    if ()
                        //    { }
                        //    break;
                        default:
                            break;
                    }
                }
                #endregion

                //清除所有报警信息
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
      
        public bool BackHome(ServoAxis axis,IoPoint Out,IoPoint In)
        {
            Stopwatch _repositoryWatch = new Stopwatch();
            _repositoryWatch.Start();

            bool isHome = false;
            if (!axis.IsServon ) return false;
            Global.IsLocating = true;
            Task.Factory.StartNew(() =>
            {
                try
                {           
                    
                    Out.Value = true;
                   
                    while (true)
                    {
                        Thread.Sleep(3);

                        if (In.Value == false)
                        {
                            isHome = true;
                            Out.Value = false;
                        }
                        if (isHome && In.Value)
                        {
                            isHome = false;
                            axis.APS_set_command(0.0);
                            AppendText($"回零");
                            if (axis.NoId == 12) Marking.CleanXHomeFinish = true;
                            if (axis.NoId == 13) Marking.CleanYHomeFinish = true;
                            if (axis.NoId == 14) Marking.CleanZHomeFinish = true;
                            Global.IsLocating = false;
                            return true;
                            break;
                        } 

                        //_repositoryWatch.Stop();
                        if (_repositoryWatch.ElapsedMilliseconds  /1000> 5)
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
        /// <summary>
        /// 报警信息集合
        /// </summary>
        protected override IList<Alarm> alarms()
        {
            var list = new List<Alarm>();
            list.AddRange(Xaxis.Alarms);
            list.AddRange(Yaxis.Alarms);
            list.AddRange(Zaxis.Alarms);
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.初始化故障)
            {
                AlarmLevel = AlarmLevels.None,
                Name = PlateformAlarm.初始化故障.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.顶升气缸未复位或平台不在原位)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.顶升气缸未复位或平台不在原位.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗工位料盘检测有误)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗工位料盘检测有误.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗Z轴回零异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗Z轴回零异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗XY回零异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗XY回零异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗顶升气缸动作感应异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗顶升气缸动作感应异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗上下气缸动作感应异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗上下气缸动作感应异常.ToString()
            });

            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗旋转气缸动作感应异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗旋转气缸动作感应异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗夹紧气缸动作感应异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗夹紧气缸动作感应异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.上料机未发送二维码或为空)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.上料机未发送二维码或为空.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.与上料机未连接成功)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.与上料机未连接成功.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.清洗到位感应超时请确认感应器正常工作)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.清洗到位感应超时请确认感应器正常工作.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.Plasma工作火焰可见光异常请确认是否工作出火焰或屏蔽)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.Plasma工作火焰可见光异常请确认是否工作出火焰或屏蔽.ToString()
            });
            list.AddRange(CleanStopCylinder.Alarms);
            list.AddRange(CleanUpCylinder.Alarms);
            list.AddRange(CleanClampCylinder.Alarms);
            list.AddRange(CleanRotateCylinder.Alarms);
            list.AddRange(CleanUpDownCylinder.Alarms);
  
            return list;
        }

        /// <summary>
        /// 气缸状态集合
        /// </summary>
        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();
            //要添加气缸
            list.Add(CleanStopCylinder);
            list.Add(CleanUpCylinder);
            list.Add(CleanRotateCylinder);
            list.Add(CleanClampCylinder);
            list.Add(CleanUpDownCylinder);


            return list;
        }

        public enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            清洗Z轴回零异常,
            清洗XY回零异常,
            清洗顶升气缸动作感应异常,
            清洗上下气缸动作感应异常,
            清洗旋转气缸动作感应异常,
            清洗夹紧气缸动作感应异常,
            顶升气缸未复位或平台不在原位,
            清洗工位料盘检测有误,
            等待扫码结果,
            执行条件不满足,
            气缸不在状态位,
            上料机未发送二维码或为空,
            与上料机未连接成功,
            清洗到位感应超时请确认感应器正常工作,
            Plasma工作火焰可见光异常请确认是否工作出火焰或屏蔽
        }
    }
}
