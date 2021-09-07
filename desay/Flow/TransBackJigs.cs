using System.Collections.Generic;
using Motion.Enginee;
using Motion.Interfaces;
using System.Crosscutting.Logging;
using System.Threading;
using System.Diagnostics;
using System;
using desay.ProductData;
using System.Collections;
using System.Threading.Tasks;
using System.Drawing;
using System.Toolkit;
using System.IO;
using log4net;
using System.Device;

namespace desay.Flow
{
    /// <summary>
    /// Control flow of carrier transportation  回流线
    /// </summary>
    public class TransBackJigs : StationPart
    {
        private PlateformAlarm m_Alarm;
        public TransBackJigs(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
            : base(ExternalSign, stationIni, stationOpe, typeof(TransBackJigs)) { }     
        #region 部件
        /// <summary>
        /// 清洗点胶回流水线阻挡气缸
        /// </summary>
        public SingleCylinder CleanGlueBackStopCylinder { get; set; }
        /// <summary>
        /// AA段回流阻挡气缸
        /// </summary>
        public SingleCylinder AAbackHomeStopCylinder { get; set; }

        #endregion


        public bool CarrierHomeBit;
        bool needAlarm;
        public int step_Glue = 0;
        #region abstract class ThreadPart
        public override void Running(RunningModes runningMode)
        {
            var _watch = new Stopwatch();
            _watch.Start();
            var _watch_AAback = new Stopwatch();
            _watch_AAback.Start();
            var _watchGlueback = new Stopwatch();
            _watchGlueback.Start();
            var _watchGluebackup= new Stopwatch();
            _watchGluebackup.Start();
            var _watch_AAbackgLUE = new Stopwatch();
            _watch_AAbackgLUE.Start();
            bool isAABackHaveJigs = false;
            bool isGlueBackHaveJigs = false;
            bool isAABackFinishGlue = false;
            bool isAAHaveNull = false;
            bool isGlueHaveNull = false;
            while (true)
            {
                Thread.Sleep(10);
                if (!IoPoints.IDI18.Value&& Marking.BaclFlowRuning_Glue) IoPoints.IDO8.Value = true;//点胶回流反转
                if (!IoPoints.IDI925.Value&&Marking.BaclFlowRuning_AA) IoPoints.IDO914.Value = true;//回流电机反转AA

                #region  转动AA线有信号就停止
                //转动AA线有信号就停止
                if (IoPoints.IDI924.Value)//
                {
                    //AppendText("AA段回流线有治具");

                    IoPoints.IDO97.Value = false;//联机信号

                }
                if (IoPoints.IDI925.Value && !AAbackHomeStopCylinder.IsOutMove)
                {
                    //AppendText("AA段回流线有治具停止");
                    isAABackHaveJigs = true;
                    IoPoints.IDO914.Value = false;//回流电机反转AA stop
                    IoPoints.IDO97.Value = false;//联机信号
                    Thread.Sleep(50);//保证固话可以收到
                }
                //AA无料时 告诉固化可以随时回流
                else if (!IoPoints.IDI924.Value && !IoPoints.IDI925.Value && !AAbackHomeStopCylinder.IsOutMove)
                {
                    if (!isAAHaveNull) { isAAHaveNull = true; _watch_AAback.Restart(); }
                    if (_watch_AAback.ElapsedMilliseconds > 1000)
                    {
                        _watch_AAback.Restart();
                        isAAHaveNull = false;
                        isAABackHaveJigs = false;
                        //双重保险 送料ready  或者固化回流后
                        //if (IoPoints.IDI926.Value || !IoPoints.IDI927.Value)
                            IoPoints.IDO97.Value = true;//联机信号2 可以回流
                    }

                }

                #endregion

                #region 点胶回流线有治具停止
                if (IoPoints.IDI18.Value && !CleanGlueBackStopCylinder.IsOutMove)
                {
                    isGlueBackHaveJigs = true;
                    isAABackFinishGlue = true;
                    //AppendText("点胶回流线有治具停止");
                    IoPoints.IDO8.Value = false;//点胶回流反转
                    AAbackHomeStopCylinder.Reset();//新增
                    isAABackHaveJigs = false;//新增
                    Thread.Sleep(100);
                }
                else if (!IoPoints.IDI18.Value && !CleanGlueBackStopCylinder.IsOutMove && isGlueBackHaveJigs)//新增
                {
                    if (!isGlueHaveNull) { isGlueHaveNull = true; _watchGlueback.Restart(); }
                    if (_watchGlueback.ElapsedMilliseconds > 1000)
                    {
                        _watchGlueback.Restart();
                        isGlueHaveNull = false;
                        //待验证 料被拿走
                        isGlueBackHaveJigs = false;
                        Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false; ;
                        step_Glue = 0;
                    }

                }

                if (Marking.GlueBackFlowFinishToUpMotion)//回流完成
                {
                    Marking.UpMotionReadyGlueBackFlow = false;//新增20201021
                    Marking.GlueBackFlowFinishToUpMotion = false;
                    CleanGlueBackStopCylinder.Reset();
                    isGlueBackHaveJigs = false;
                    step_Glue = 0;
                    Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;//只发一次
                    AppendText("回流完成到上位机");
                }
                #endregion
                #region 自动流程
                if (stationOperate.Running)
                {

                
                    #region Glue回流有料处理
                    if (isGlueBackHaveJigs)//Glue有料处理
                    {
                        //其余继续等待 上料段取走                                               
                        #region  Glue回流
                        switch (step_Glue)
                        {
                            case 0://判断接驳台是否在原点，顶升是否在原位
                                Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = true;//等待取走                              
                                if (Marking.FormerStationShield)
                                {
                                    isGlueBackHaveJigs = false;
                                    //m_Alarm = PlateformAlarm.上料段已屏蔽请人工拿走;
                                    //AppendText("上料段已屏蔽请人工拿走");
                                    step_Glue = 0;
                                    break;
                                }
                                if (Marking.TcpClientOpenSuccess) { step_Glue = 10; }
                                break;
                            case 10://复位气缸
                                if (Marking.UpMotionReadyGlueBackFlow)//上料段等待回流中
                                {
                                    //Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;//只发一次
                                   
                                    Marking.UpMotionReadyGlueBackFlow= false;
                                    CleanGlueBackStopCylinder.Set();
                                    IoPoints.IDO8.Value = true; //点胶回流反转
                                    Marking.GlueBackFlowFinishToUpMotion = false;
                                    Thread.Sleep(1500);
                                    step_Glue = 20;

                                    _watchGluebackup.Reset();
                                    AppendText("上料段等待回流中");
                                }

                                break;
                            case 20:
                                if (Marking.GlueBackFlowFinishToUpMotion)//回流完成
                                {
                                    Marking.GlueBackFlowFinishToUpMotion = false;
                                    CleanGlueBackStopCylinder.Reset();
                                    isGlueBackHaveJigs = false;
                                    step_Glue = 0;
                                    Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;//只发一次
                                    AppendText("回流完成到上位机");
                                }
                                else if (_watchGluebackup.ElapsedMilliseconds/1000>8)//增加超时
                                {
                                    Marking.GlueBackFlowFinishToUpMotion = false;
                                    CleanGlueBackStopCylinder.Reset();
                                    isGlueBackHaveJigs = false;
                                    step_Glue = 0;
                                    Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;//只发一次
                                    AppendText("回流完成到上位机超时");
                                }
                                break;

                            default:
                                //stationOperate.RunningSign = false;
                                step_Glue = 0;
                                break;

                        }
                        #endregion
                    }
                    #endregion

                    #region  AA回流
                    if (isAABackHaveJigs)
                    {
                        switch (step)
                        {
                            case 0://判断接驳台是否在原点，顶升是否在原位
                                //Thread.Sleep(1000);
                                step = 10;
                                AppendText(" AA回流中");
                                break;
                            case 10://复位气缸
                                if (!isGlueBackHaveJigs) //Glue无料状态
                                {
                                    isAABackFinishGlue = false;
                                    AAbackHomeStopCylinder.Set();
                                    //Thread.Sleep(100);
                                    step = 20;
                                    
                                    AppendText(" AA回流阻挡气缸放下");
                                }
                                break;
                            case 20:
                                IoPoints.IDO914.Value = true;//反装
                                Thread.Sleep(500);
                                step =40;//更改
                                AppendText(" AA回流电机反转");
                                _watch_AAbackgLUE.Restart();
                                break;
                            case 30://更改
                                if (isAABackFinishGlue)
                                {
                                    isAABackFinishGlue = false;
                                    AAbackHomeStopCylinder.Reset();
                                    Thread.Sleep(500);
                                    isAABackHaveJigs = false;
                                    step = 0;
                                    AppendText(" AA回流到点胶段完成");
                                }
                                //else if (_watch_AAbackgLUE.ElapsedMilliseconds/1000>10)
                                //{
                                //    isAABackFinishGlue = false;
                                //    AAbackHomeStopCylinder.Reset();
                                //    Thread.Sleep(500);
                                //    isAABackHaveJigs = false;
                                //    step = 0;
                                //    AppendText(" AA回流到点胶段完成超时");
                                //}
                                break;
                            default:
                                //stationOperate.RunningSign = false;
                                step = 0;
                                break;

                        }
                    }
                    #endregion



                }
                #endregion
                #region 初始化流程
                if (stationInitialize.Running)
                {
                    switch (stationInitialize.Flow)
                    {
                        case 0://清除所有标志位
                            stationInitialize.InitializeDone = false;
                            stationOperate.RunningSign = false;
                            step = 0;
                            step_Glue = 0;
                            Thread.Sleep(200);
                            //流水线停止
                            IoPoints.IDO8.Value = false;
                            IoPoints.IDO914.Value = false;
                            stationInitialize.Flow = 10;
                            Marking.AABackFlowGetEmptyJigsFromNextStation = false;//交互清除
                            Marking.GlueBackFlowGetEmptyJigsFromAABackFlow = false;
                            Marking.GlueBackFlowFinishToUpMotion = false;
                            Marking.UpMotionReadyGlueBackFlow = false;
                            isAABackHaveJigs = false;
                            isGlueBackHaveJigs = false;
                            isAABackFinishGlue = false;
                            IoPoints.IDO97.Value = false;

                            break;
                        case 10://复位气缸
                            Thread.Sleep(200);

                            CleanGlueBackStopCylinder.InitExecute();
                            CleanGlueBackStopCylinder.Reset();

                            AAbackHomeStopCylinder.InitExecute();
                            AAbackHomeStopCylinder.Reset();
                            stationInitialize.Flow = 20;
                            Thread.Sleep(500);
                            _watch.Restart();
                            break;
                 
                        case 20://判断所有气缸到位,伸缩气缸收回
                            if (CleanGlueBackStopCylinder.OutOriginStatus&& AAbackHomeStopCylinder.OutOriginStatus)
                            {
                                stationInitialize.Flow = 30;
                            }
                            else
                            {
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    //m_Alarm = PlateformAlarm.AA堆料工位复位时气缸不在状态位;
                                    AppendText("AA堆料工位复位时气缸不在状态位");
                                    stationInitialize.InitializeDone = false;
                                    stationOperate.RunningSign = false;
                                    step = 60;
                                }
                            }
                            break;
                        case 30://流水线
                                //流水线
                            IoPoints.IDO8.Value = true;
                            IoPoints.IDO914.Value = true;
                            _watch.Restart();
                            stationInitialize.Flow = 40;
                            break;
                        case 40:
                            if (IoPoints.IDI18.Value)//回流阻挡信号
                            {
                                IoPoints.IDO8.Value = false;
                                AppendText("请拿走点胶回流线治具");
                            }
                            else if (IoPoints.IDI925.Value)
                            {
                                IoPoints.IDO914.Value = false;
                                AppendText("请拿走AA回流线治具");
                            }
                            if (_watch.ElapsedMilliseconds > 500 )
                            {
                                stationInitialize.Flow = 50;
                            }
                            break;
                        case 50://复位完成，置位初始化标志
                            Thread.Sleep(200);
                            IoPoints.IDO8.Value = false;
                            IoPoints.IDO914.Value = false;
                            stationInitialize.InitializeDone = true;
                            AppendText($"{Name}初始化完成！");
                            stationInitialize.Flow = 60;
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                //clean all the enum of alarm information
                #region 报警清除
                if (AlarmReset.AlarmReset)
                {
                    m_Alarm = PlateformAlarm.无消息;
                }
                #endregion
            }
        }
        /// <summary>
        /// 流程报警集合
        /// </summary>
        protected override IList<Alarm> alarms()
        {
            var list = new List<Alarm>();
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.初始化故障)
            {
                AlarmLevel = AlarmLevels.None,
                Name = PlateformAlarm.初始化故障.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.上料机未发送二维码或为空)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.上料机未发送二维码或为空.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.上料段已屏蔽请人工拿走)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.上料段已屏蔽请人工拿走.ToString()
            });
            list.AddRange(CleanGlueBackStopCylinder.Alarms);
            list.AddRange(AAbackHomeStopCylinder.Alarms);
            return list;
        }
        /// <summary>
        /// 气缸状态集合
        /// </summary>
        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();

            list.Add(CleanGlueBackStopCylinder);
            list.Add(AAbackHomeStopCylinder);

            return list;
        }
        #endregion

        private enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            气缸不在状态位,         
            上料机未发送二维码或为空,
            上料段已屏蔽请人工拿走,
        }
    }
}
