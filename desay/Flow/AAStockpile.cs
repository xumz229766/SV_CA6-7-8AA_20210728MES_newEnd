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
    /// Control flow of carrier transportation  AA堆料位
    /// </summary>
    public class AAStockpile : StationPart
    {
        private PlateformAlarm m_Alarm;
        public AAStockpile(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
            : base(ExternalSign, stationIni, stationOpe, typeof(TransBackJigs)) { }
        #region 部件
      
     
        /// <summary>
        /// AA堆料顶升气缸
        /// </summary>
        public DoubleCylinder AAStockpileUpCylinder{ get; set; }
        /// <summary>
        /// AA堆料阻挡气缸
        /// </summary>
        public SingleCylinder AAStockpileStopCylinder { get; set; }

       

        #endregion

        public event Action<int> SendRequest;
        public event Action AlarmClean;

        private int FailCount;
        public bool CarrierHomeBit;
        bool needAlarm;

        #region abstract class ThreadPart
        public override void Running(RunningModes runningMode)
        {
            var _watch = new Stopwatch();
            _watch.Start();
            bool bSensor = false;
            while (true)
            {
                Thread.Sleep(10);

                #region 自动流程
                if (stationOperate.Running)
                {
                    
                    switch (step)
                    {
                        case 0://
                            step = 10;
                            break;
                        case 10:
                            if (Marking.GlueWorkAlreadyRequest/*||stationOperate.SingleRunning*/)//收到Glue 完成信号
                            {
                                AppendText("点胶工位完成信号，准备进入堆料工位");
                                AAStockpileStopCylinder.Reset();
                                AAStockpileUpCylinder.Reset();
                                step = 20;
                            }
                            break;
                        case 20:
                            if (AAStockpileStopCylinder.OutOriginStatus&& AAStockpileUpCylinder.OutOriginStatus)
                            {
                                //皮带转到
                                IoPoints.IDO90.Value = true;
                                Marking.AAStockpAlreadyGetJigsFromGlue=true;//堆料段已准备好
                                step = 30;
                            }

                            break;

                        case 30://接近开关
                            if (IoPoints.IDI911.Value)
                            {
                                if (!bSensor) { bSensor = true;_watch.Restart(); }
                                if (_watch.ElapsedMilliseconds > 0)
                                {
                                    Thread.Sleep(AxisParameter.Instance.StopilePosDelay);//到位感应延时

                                    bSensor = false;
                                    if(Marking.AAUpClyIsMove&&AxisParameter.Instance.AAingAAstockpStop)//AA时停止
                                              IoPoints.IDO90.Value = false;
                                    
                                    AAStockpileUpCylinder.Set();//临时
                                  
                                    Marking.AAStockpAlreadyGetJigsFromGlue = false;//堆料段已接到料
                                    //step = 40; 更改 当AA为空料时直接过去
                                    Marking.AAStockpWorkAlreadyRequest = true;//告诉AA准备好
                                    step = 50;
                                }
                            }
                            break;
                        case 40:
                            //AAStockpileUpCylinder.Set();//临时
                            if (AAStockpileUpCylinder.OutMoveStatus)
                            {
                                Marking.AAStockpWorkAlreadyRequest = true;//告诉AA准备好
                                step = 50;
                            }
                            break;            
                        case 50:
                            if (Marking.AAAlreadyGetJigsFromStockp)//AA段准备好
                            {
                                //IoPoints.IDO90.Value = false;//停止不需要 1013
                                AAStockpileUpCylinder.Reset();
                                AAStockpileStopCylinder.Set();
                                //shuju
                                try
                                {
                                    lock (MesData.AAStockplieDataLock)
                                    {                                        
                                        //传给下一段
                                        MesData.aaData.aaStockpileData = MesData.aaStockPileData;                                        
                                        
                                    }
                                }
                                catch
                                {
                                    m_Alarm = PlateformAlarm.数据放入链表失败;
                                }
                                step =60;

                            }
                            break;
                        case 60:
                            if (AAStockpileUpCylinder.OutOriginStatus)
                            {
                                //AAStockpileStopCylinder.Set();
                                //IoPoints.IDO90.Value = true;  1013
                                step = 70;
                            }
                            break;
                        case 70:
                            if (!Marking.AAAlreadyGetJigsFromStockp)//完成送料
                            {
                                Marking.AAStockpWorkAlreadyRequest = false;//清除
                                //if (Marking.AAUpClyIsMove) IoPoints.IDO90.Value = false;
                                AAStockpileStopCylinder.Reset();


                                step = 80;
                            }
                            break;
                        case 80:
                            if (AAStockpileStopCylinder.OutOriginStatus)
                            {
                                step = 0;
                            }
                            break;
                        default:
                            stationOperate.RunningSign = false;
                            step = 0;
                            break;

                    }
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
                            Marking.AAStockpAlreadyGetJigsFromGlue = false;//交互清除
                            Marking.AAStockpWorkAlreadyRequest = false;
                            stationInitialize.Flow = 10;
                            break;
                        case 10://复位气缸
                            AAStockpileStopCylinder.InitExecute();
                            AAStockpileStopCylinder.Reset();
                            Thread.Sleep(100);
                            AAStockpileUpCylinder.InitExecute();
                            AAStockpileUpCylinder.Reset();
                            stationInitialize.Flow = 20;
                            AppendText("AA堆料工位气缸复位");
                            _watch.Restart();
                            break;
                    
                        case 20:
                            if (AAStockpileStopCylinder.OutOriginStatus && AAStockpileUpCylinder.OutOriginStatus)
                                stationInitialize.Flow = 50;
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
                        case 50://复位完成，置位初始化标志
                            Thread.Sleep(50);
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
                if (AlarmReset.AlarmReset)
                {
                    m_Alarm = PlateformAlarm.无消息;
                }
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
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.初始化故障.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.AA堆料工位复位时气缸不在状态位)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.AA堆料工位复位时气缸不在状态位.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.数据放入链表失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.数据放入链表失败.ToString()
            });
            list.AddRange(AAStockpileStopCylinder.Alarms);
            list.AddRange(AAStockpileUpCylinder.Alarms);

            return list;
        }
        /// <summary>
        /// 气缸状态集合
        /// </summary>
        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();

            list.Add(AAStockpileStopCylinder);
            list.Add(AAStockpileUpCylinder);
            return list;
        }
        #endregion

        private enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            AA堆料工位复位时气缸不在状态位,
            数据放入链表失败,
        }
    }
}
