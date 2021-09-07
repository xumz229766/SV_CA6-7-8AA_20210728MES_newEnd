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
using System.Text;
namespace desay.Flow
{
    /// <summary>
    /// Control flow of carrier transportation
    /// </summary>
    public class AAstation : StationPart
    {
        private PlateformAlarm m_Alarm;
        public AAstation(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
            : base(ExternalSign, stationIni, stationOpe, typeof(TransBackJigs)) { }
        #region 部件
        /// <summary>
        /// 六轴平台进出轴Y-轴
        /// </summary>
        //public ServoAxis MoveSixYaxis { get; set; }
        /// <summary>
        /// 光源Z-轴
        /// </summary>
        //public ServoAxis LightZaxis { get; set; }
        /// <summary>
        /// AA夹具阻挡气缸
        /// </summary>
        public SingleCylinder AAJigsStopCylinder { get; set; }
        /// <summary>
        /// AA夹具顶升气缸
        /// </summary>
        public DoubleCylinder AAJigsUpCylinder { get; set; }
        /// <summary>
        /// 点胶小顶升气缸
        /// </summary>
        public DoubleCylinder AAJigsCylinder_Small { get; set; }
        ///// <summary>
        ///// AA堆料顶升气缸
        ///// </summary>
        //public DoubleCylinder AAStockpileUpCylinder{ get; set; }
        ///// <summary>
        ///// AA堆料阻挡气缸
        ///// </summary>
        //public SingleCylinder AAStockpileStopCylinder { get; set; }

        /// <summary>
        /// UV灯上下气缸
        /// </summary>
        //public DoubleCylinder AAUVUpDownCylinder { get; set; }
        /// <summary>
        /// AA气夹爪气缸
        /// </summary>
        //public DoubleCylinder AAClampClawCylinder { get; set; }

        #endregion

        public event Action<int> SendRequest;
        public event Action AlarmClean;
        public event Action<bool> ShowResult;
        private int FailCount; 
        public bool CarrierHomeBit;
        bool needAlarm;
        StringBuilder buf = new StringBuilder(3072);//指定的buf大小必须大于传入的字符长度
        public bool Allresult = false;
        public  List<vvdatax> ffGetaaData(string msg)
        {
            List<vvdatax> dd = new List<vvdatax>();

            string[] msgx = msg.Split('|');
            for (int i = 0; i < 7; i++)
            {
                vvdatax d1 = new vvdatax();
                d1.vvdata = double.Parse(msgx[4 + i * 7]);
                d1.vvmin = double.Parse(msgx[5 + i * 7]);
                d1.vvmax = double.Parse(msgx[6 + i * 7]);
                dd.Add(d1);

            }
            return dd;
        }
        vvdatax d1 = new vvdatax();
        #region abstract class ThreadPart
        public override void Running(RunningModes runningMode)
        {
            var _watch = new Stopwatch();
            _watch.Start();
            var _watchAACT = new Stopwatch();
            _watchAACT.Start();
            bool bSensor = false;
            bool bGhFinish = false;
            int AAlightTimes = 0;
            bool GHhaveGetJigs = true;
            bool AALightNG = false;
            bool AALightLossFrameNG = false;
            while (true)
            {
                Thread.Sleep(10);
                if (!IoPoints.IDI926.Value)//固化接收到料
                {
                    GHhaveGetJigs = true;
                    IoPoints.IDO96.Value = false;//联机信号
                }
                #region 自动流程
                if (stationOperate.Running)
                {

                   
                    switch (step)
                    {
                        case 0://
                            step = 10;
                            AALightNG = false;
                            AALightLossFrameNG = false;
                            break;

                        case 10://
                            step = 20;
                            break;
                        case 20:
                            AAJigsStopCylinder.Reset();
                            AAJigsUpCylinder.Reset();
                            step = 30;
                            break;
                        case 30:
                            if (AAJigsStopCylinder.OutOriginStatus&&AAJigsUpCylinder.OutOriginStatus)
                            {
                                step = 40;
                            }
                            break;
                        case 40://////堆料取料
                            if (Marking.AAStockpWorkAlreadyRequest /*|| stationOperate.SingleRunning*/)
                            {
                              
                                IoPoints.IDO90.Value = true;
                                Marking.AAAlreadyGetJigsFromStockp = true;
                                MesData.aaData.aaResult = false;//结果清除
                                AppendText("AA工位等待上治具");
                                if(Config.Instance.IsIO91) step = 50;//CA6线用9.1
                                else step = 55;//CA7线用9.0
                            }
                            break;
                        case 50:
                            if (IoPoints.IDI91.Value)
                            {
                                _watchAACT.Restart();
                                if (!bSensor) { bSensor = true; _watch.Restart(); }
                                if (_watch.ElapsedMilliseconds > 0)//信号长达100ms
                                {
                                    Thread.Sleep(AxisParameter.Instance.AAPosDelay);//到位感应延时
                                    bSensor = false;

                                    //if(!Marking.AAStockpAlreadyGetJigsFromGlue)//堆料没有来料时
                                    //  IoPoints.IDO90.Value = false; //停止

                                    Marking.AAAlreadyGetJigsFromStockp = false;//取完料
                                    AppendText("AA工位取完料");
                                    step = 60;
                                    MesData.aaData.aaResult = false;
                                }
                            }
                            else { bSensor = false; _watch.Restart(); }
                            break;
                        case 55://CA7线
                            if (IoPoints.IDI90.Value )
                            {
                                _watchAACT.Restart();
                                if (!bSensor) { bSensor = true; _watch.Restart(); }
                                if (_watch.ElapsedMilliseconds > 0)//信号长达100ms
                                {
                                    Thread.Sleep(AxisParameter.Instance.AAPosDelay);//到位感应延时
                                    bSensor = false;

                                    //if (!Marking.AAStockpAlreadyGetJigsFromGlue)//堆料没有来料时
                                    //    IoPoints.IDO90.Value = false; //停止

                                    Marking.AAAlreadyGetJigsFromStockp = false;//取完料
                                    AppendText("AA工位取完料");
                                    step = 60;
                                    MesData.aaData.aaResult = false;
                                }
                            }
                            else { bSensor = false; _watch.Restart(); }
                            break;
                        case 60:
                            if (!Marking.AAStockpWorkAlreadyRequest)
                            {
                                //判断
                                if (MesData.aaData.aaStockpileData.glueData.AllglueStationResult && MesData.aaData.aaStockpileData.glueData.cleanData.CleanResult_MesLock)
                                {
                                    step = 65;//结果OK
                                    AAlightTimes = 0;
                                }
                                else {
                                    AppendText("AA工位前NG");
                                    step = 220;
                                    AAJigsUpCylinder.Set();
                                    //Config.Instance.AllAAProductNgTotal++;
                                    #region
                                    d1.vvdata = -2;
                                    d1.vvmin = 0;
                                    d1.vvmax = 2;
                                    string tool3 = MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN;//治具码
                                    string SN = "123456";
                                    try
                                    {
                                         SN = $"{Position.Instance.A2C}{0}{MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Substring(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Length - 6)}";
                                    }
                                    catch {  }
                                   #endregion
                                    if (!MesData.aaData.aaStockpileData.glueData.cleanData.CleanResult_MesLock) { ResultClass.AllResultShow = 9; Config.Instance.CleanMesLockNgTotal++;

                                        try
                                        {
                                            Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1,false);
                                            AppendText($"Mes上传互锁NG{SN}");
                                        }
                                        catch { AppendText($"Mes互锁写入Mes失败！"); }
                                    }//清洗mes互锁NG
                                    else if (!MesData.aaData.aaStockpileData.glueData.wbData.AllResult)//白板NG
                                    {
                                        if (MesData.aaData.aaStockpileData.glueData.wbData.LightResult)
                                        {
                                            ResultClass.AllResultShow = 10;
                                            Config.Instance.GlueWbNgTotal_LightNG++;
                                            try
                                            {
                                                Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, false);
                                                AppendText($"白板点亮NG");
                                            }
                                            catch { AppendText($"白板点亮NG写入Mes失败！"); }
                                        }
                                        else
                                        {
                                            ResultClass.AllResultShow = 14;
                                            Config.Instance.GlueWbNgTotal_ParticeNG++;
                                            try
                                            {
                                                Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, true,false);
                                                AppendText($"白板脏污NG");
                                            }
                                            catch { AppendText($"白板脏污写入Mes失败！"); }
                                        }
                                     
                                    }
                                    else if (!MesData.aaData.aaStockpileData.glueData.glueVisionData.LoadcationResult)//定位NG
                                    {
                                        ResultClass.AllResultShow = 11; Config.Instance.GlueLocationNgTotal++;
                                        try
                                        {
                                            Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, true, true,false);
                                            AppendText($"定位NG");
                                        }
                                        catch { AppendText($"定位NG写入Mes失败！"); }
                                    }
                                    else if (!MesData.aaData.aaStockpileData.glueData.glueVisionData.GlueFindResult)//识别NG
                                    {
                                        ResultClass.AllResultShow = 12; Config.Instance.GlueFindNgTotal++;
                                        try
                                        {
                                            Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, true, true, true,false);
                                            AppendText($"识别NG");
                                        }
                                        catch { AppendText($"识别NG写入Mes失败！"); }
                                    }
                                    Thread.Sleep(500);
                                }//结束
                                if (Marking.AAShield) { step = 220; AAJigsUpCylinder.Set(); Thread.Sleep(500); }//结束
                            }
                            break;
                        case 65:                           
                            AAJigsUpCylinder.Set();
                            step = 70;
                            break;
                        case 70:
                            if (AAJigsUpCylinder.OutMoveStatus)
                            {

                                if (AxisParameter.Instance.IsUseTanZhenCyl) AAJigsCylinder_Small.Set();
                                IoPoints.IDO915.Value = true;//通电
                                Thread.Sleep(AxisParameter.Instance.AAPowerOpenDelayTime);//
                                _watch.Reset();
                                step = 75;                                                               
                            }
                            break;
                        case 75:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if(AAJigsCylinder_Small.OutMoveStatus) step = 80;
                            }
                            else
                                step = 80;
                            break;
                        case 80://AA流程 
                            if (CallAA.GetAAStatus() == (int)AAstatus.AA_Ready)
                            {
                                AppendText("AA开始");
                                try
                                {
                                    if (Marking.ProductTest)
                                    {
                                        CallAA.StartTestFunction(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN, MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN);
                                    }//AA跑产品
                                    else
                                    {
                                        CallAA.StartAAFunction(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN, MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN);//调用AA流程
                                    }//调用AA流程
                                    Thread.Sleep(500);
                                    Marking.AAUpClyIsMove = true;//AAing
                                    if (!Marking.AAStockpAlreadyGetJigsFromGlue)//堆料没有来料时
                                        IoPoints.IDO90.Value = false; //停止
                                    step = 100;
                                    _watch.Reset();

                                }
                                catch (Exception ex) { AppendText($"调用AA程序异常！{ex}"); Thread.Sleep(500); step = 220; }                                                                                                                                       //Common.Test_WriteMesTxtAndCsvFile(sn1, tool3, false, d1, d1, d1, d1, d1, d1, d1);
                                
                            }
                            else if (_watch.ElapsedMilliseconds / 1000 > 55)
                            {
                                m_Alarm = PlateformAlarm.AA工位Ready超时;
                                AppendText("AA工位Ready超时");
                                _watch.Restart();
                                
                            }
                            break;
                        case 100:
                            if (CallAA.GetAAStatus() == (int)AAstatus.AA_Finish|| CallAA.GetAAStatus() == (int)AAstatus.AA_Ready)
                            {
                                Marking.AAUpClyIsMove = false;
                                AppendText("AA完成");
                                step = 110;                                
                            }
                            else if (CallAA.GetAAStatus() == (int)AAstatus.AA_Pause)//暂停
                            {

                            }
                            else if (CallAA.GetAAStatus() == (int)AAstatus.AA_Reseting)//复位中
                            {

                            }
                            else if (CallAA.GetAAStatus() == (int)AAstatus.AA_Testing)//测试中
                            {

                            }
                            else if (CallAA.GetAAStatus() == (int)AAstatus.AA_Warming)//报警
                            {
                                m_Alarm = PlateformAlarm.AA工位报警中;
                                Marking.AAUpClyIsMove = false;
                                step = 140;//报警   //需要重新复位AA
                                MesData.aaData.aaResult = false;//NG
                            }
                            _watch.Stop();
                            if (_watch.ElapsedMilliseconds / 1000 > 105)
                            {
                                //m_Alarm = PlateformAlarm.AA堆料工位复位时气缸不在状态位;
                                Marking.AAUpClyIsMove = false;
                                AppendText("AA工位超时");
                                _watch.Restart();
                                step = 120;
                            }
                            _watch.Start();
                            break;
                        case 110:
                            int iAAresult = CallAA.GetTestResult(buf);
                            if (iAAresult == (int)AAFunctionResult.AA_PASS)
                            {
                                MesData.aaData.aaResult = true;//OK
                                AppendText("AAOK");
                                ResultClass.AllResultShow = 0;
                                Config.Instance.AllAAProductOkTotal++;
                            }
                            else
                            {   MesData.aaData.aaResult = false;//NG
                                AppendText("AANG");
                                if (iAAresult == (int)AAFunctionResult.AA_LightON_NG)//
                                {
                                    //点不亮继续
                                    if (AAlightTimes < Position.Instance.AALightOnTime - 1) { AAlightTimes++; step = 115; break; }
                                    else
                                    {
                                        AppendText("AA重复点亮NG");
                                        ResultClass.AllResultShow = 1;
                                        Config.Instance.AALightNgTotal++;
                                        AALightNG = true;
                                    }
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_MOVEControl_NG)
                                {
                                    ResultClass.AllResultShow = 2;
                                    Config.Instance.AAMoveNgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_SEARCH_NG)
                                {
                                    ResultClass.AllResultShow = 3;
                                    Config.Instance.AASerchNgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_OC_TUNE_NG)
                                {
                                    ResultClass.AllResultShow = 4;
                                    Config.Instance.AAOCNgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_TILT_TUNE_NG)
                                {
                                    ResultClass.AllResultShow = 5;
                                    Config.Instance.AATILT_TUNENgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_UVBefore_Check_NG)
                                {
                                    ResultClass.AllResultShow = 6;
                                    Config.Instance.AAUVBeforeNgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_UVAfter_Check_NG)
                                {
                                    ResultClass.AllResultShow = 7;
                                    Config.Instance.AAUVAfterNgTotal++;
                                }
                                else if (iAAresult == (int)AAFunctionResult.AA_SN_NG)
                                {
                                    ResultClass.AllResultShow = 8;
                                    Config.Instance.AASNNgTotal++;
                                }
                                else if (iAAresult == 9)//搞错了序号
                                {
                                    ResultClass.AllResultShow = 15;
                                    Config.Instance.AALightLossFrameNgTotal++;
                                    AALightLossFrameNG = true;
                                }
                                else { ResultClass.AllResultShow = 13; Config.Instance.NoneNgTotal++; }

                                //Config.Instance.AllAAProductNgTotal++;//NG数
                            }
                            log.Info(iAAresult);
                            step = 200;//MEs
                            break;
                        case 115://点不亮时 继续点亮
                            IoPoints.IDO915.Value = false;//断电
                            Thread.Sleep(10);
                            AAJigsUpCylinder.Reset();
                            step = 118;
                            break;
                        case 118:
                            if (AAJigsUpCylinder.OutOriginStatus)
                            {
                                step = 65;
                                AppendText("点亮NG，AA继续");
                            }
                            break;
                        case 120:
                            IoPoints.IDO915.Value = false;//断电
                            Thread.Sleep(10);
                            step = 290;
                            break;
                        case 140://重新复位AA  NG处理
                            IoPoints.IDO915.Value = false;//断电
                            Thread.Sleep(10);
                            if (AxisParameter.Instance.AAalrmAutoReset) Marking.AAfinishAtHome = CallAA.bResetHome();
                            else Marking.AAfinishAtHome = true;
                            step = 160;
                            break;
                        case 160:
                            if (Marking.AAfinishAtHome)
                                step = 180;
                            else { AppendText($"AA工位报警后重新复位失败！！请查找原因"); m_Alarm = PlateformAlarm.AA报警重新复位失败请停止设备; }
                            break;
                        case 180://NG  处理
                            step = 220;
                            break;
                        case 200://Mes数据开始处理结果
                            Allresult = MesData.aaData.aaResult;
                            if (Allresult == false && AALightNG)
                            {
                                AALightNG = false;
                                #region
                                d1.vvdata = -2;
                                d1.vvmin = 0;
                                d1.vvmax = 2;
                                string tool3 = MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN;//治具码
                                string SN = "123456";
                                try
                                {
                                    SN = $"{Position.Instance.A2C}{0}{MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Substring(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Length - 6)}";
                                }
                                catch { }
                                #endregion
                                try
                                {
                                    Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, true, true, true, true,false);
                                    AppendText($"点亮NG");
                                }
                                catch { AppendText($"点亮NG写入Mes失败！"); }
                                step = 220;
                                break;

                            }
                            else if (Allresult == false && AALightLossFrameNG)
                            {
                                AALightLossFrameNG = false;
                                #region
                                d1.vvdata = -2;
                                d1.vvmin = 0;
                                d1.vvmax = 2;
                                string tool3 = MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN;//治具码
                                string SN = "123456";
                                try
                                {
                                    SN = $"{Position.Instance.A2C}{0}{MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Substring(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Length - 6)}";
                                }
                                catch { }
                                #endregion
                                try
                                {
                                    Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1, true, true, true, true, true, true,false);
                                    AppendText($"点亮丢帧NG");
                                }
                                catch { AppendText($"点亮丢帧NG写入Mes失败！"); }
                                step = 220;
                                break;
                            }
                            try
                            {
                                string AAdata = buf.ToString();
                                AppendText($"AA反馈数据：{AAdata}");
                                string[] str1 = AAdata.Split('$');
                                //产品码校验
                                string HolderSn = str1[1];
                                string tool3 = MesData.aaData.aaStockpileData.glueData.cleanData.JigsSN;//治具码

                                //if (HolderSn.Contains( MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN))
                                // { log.Error("Mes save Error2");if(Config.Instance.IsMes) m_Alarm = PlateformAlarm.二维码校验失败; step = 220; break; } ;
                                string SN = $"{Position.Instance.A2C}{0}{MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Substring(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Length-6)}";
                                if (AAdata.Length < 30)
                                {

                                    //vvdatax d1 = new vvdatax();
                                    d1.vvdata = -2;
                                    d1.vvmin = 0;
                                    d1.vvmax = 2;

                                    Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1);
                                    AppendText($"AA反馈数据小于30写入Mes_NG");
                                    step = 220;
                                }
                                else
                                {
                                    //mes
                                    List<vvdatax> dd1 = ffGetaaData(AAdata);

                                    if (dd1.Count == 7)
                                    { Common.Test_WriteMesTxtAndCsvFile(SN, tool3, Allresult, dd1[0], dd1[1], dd1[2], dd1[3], dd1[4], dd1[5], dd1[6]);
                                        AppendText($"AA写入Mes_{Allresult}_{SN}");
                                    }
                                    else
                                    {
                                       
                                        d1.vvdata = -1;
                                        d1.vvmin = 0;
                                        d1.vvmax = 1;
                                        Common.Test_WriteMesTxtAndCsvFile(SN, tool3, false, d1, d1, d1, d1, d1, d1, d1);
                                        AppendText($"AA反馈数据不满足写入Mes_NG");
                                    }
                                }
                            }
                            catch
                            {
                               log.Error("Mes save Error2");
                                if (Config.Instance.IsMes) m_Alarm = PlateformAlarm.保存Mes数据失败;
                            }
                            step = 220;
                            break;
                        case 220://结果显示
                            if(Marking.CleanShield&&Marking.GlueShield&&Marking.AAShield) Allresult = false;
                            else Allresult = MesData.aaData.aaResult;

                            if (Position.Instance.MesMode)
                            {
                                AppendText("Mes模式出站");
                                string SN="";
                                try
                                {
                                   SN = $"{Position.Instance.A2C}{0}{MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Substring(MesData.aaData.aaStockpileData.glueData.cleanData.HolderSN.Length - 6)}";
                                }
                                catch { AppendText("Mes模式出站SN出错"); }
                                string meg = Common.MesMoveOut(SN,Marking.SvJobNumber,Allresult);
                                AppendText($"{meg}");
                            }

                            ShowResult(Allresult);//刷新参数
                            AppendText("刷新结果");
                            step = 290;
                            //给固化1结果清除
                            //IoPoints.IDO910.Value = false;
                            //IoPoints.IDO911.Value = false;
                            break;
                        case 290:
                            if (Marking.GHShield)
                            {
                                step = 320;//临时
                            }
                            else
                            {
                                step = 300;
                            }
                            break;
                        case 300://AA 接收
                                 //OK NG 
                            Marking.AAWorkAlreadyRequest = true;//完成
                            //MesData.aaData.AllResult = MesData.aaData.aaResult && MesData.aaData.aaStockpileData.glueData.AllglueStationResult;
                          
                            if (GHhaveGetJigs) AppendText("固化已完成上一次取料");
                            else AppendText("固化未完成上一次取料！！！");
                            step = 310;
                            break;
                        case 310:
                            if (IoPoints.IDI926.Value&& GHhaveGetJigs)//固化已准备好
                            {
                                if (Allresult)
                                {
                                    IoPoints.IDO910.Value = true;//联机信号 OK
                                    IoPoints.IDO911.Value = false;
                                }
                                else
                                {
                                    IoPoints.IDO910.Value = false;
                                    IoPoints.IDO911.Value = true;//联机信号 NG
                                }
                                GHhaveGetJigs = false;//未接
                                AppendText("固化已准备好");
                                IoPoints.IDO96.Value = true;//联机信号
                                step = 320;
                            }                          
                            break;
                        case 320:
                            IoPoints.IDO90.Value = false;//停止不需要
                            IoPoints.IDO915.Value = false;//断电                                                          
                            if (AxisParameter.Instance.IsUseTanZhenCyl) AAJigsCylinder_Small.Reset();
                            step = 325;
                            break;
                        case 325:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if (AAJigsCylinder_Small.OutOriginStatus)
                                {
                                    step = 330;
                                    AAJigsUpCylinder.Reset();
                                }
                            }
                            else
                            { step = 330; AAJigsUpCylinder.Reset(); }
                            break;
                        case 330:
                            if (AAJigsUpCylinder.OutOriginStatus)
                            {
                               
                                AAJigsStopCylinder.Set();
                                Thread.Sleep(20);
                                if(AxisParameter.Instance.AAallowStockpilePutJigs) Marking.AAAlreadyGetJigsFromStockp = true;//10.24 提前允许放行
                                IoPoints.IDO90.Value = true;//开启
                                step = 335;
                            }
                            break;
                        case 335:
                            if (Marking.GHShield)
                            {
                                IoPoints.IDO90.Value = true;
                                //Thread.Sleep(5000);
                                _watch.Restart();
                                //AAJigsStopCylinder.Reset();
                                step = 338;
                            }
                            else
                            {
                                if (Config.Instance.IsIO91) step = 340;
                                else step = 345;
                                Thread.Sleep(500);
                            }
                            break;
                        case 338://增加固化屏蔽时 出料感应有料就结束
                            if (IoPoints.IDI922.Value)
                            {
                                Marking.AACycleTime = _watchAACT.ElapsedMilliseconds / 1000.0;
                                AAJigsStopCylinder.Reset();
                                step = 350;
                            }
                            else if(_watch.ElapsedMilliseconds>5000)
                            {
                                Marking.AACycleTime = _watchAACT.ElapsedMilliseconds / 1000.0;
                                AAJigsStopCylinder.Reset();
                                step = 350;
                            }
                            break;

                        case 340://优化 增加 出料感应 
                            if ((!IoPoints.IDI926.Value||IoPoints.IDI922.Value || IoPoints.IDI921.Value) && !IoPoints.IDI91.Value/*||stationOperate.Running*/)//已取完料
                            {
                                if (!bSensor) { bSensor = true; _watch.Restart(); }
                                if (_watch.ElapsedMilliseconds > 5)
                                {
                                    Marking.AACycleTime = _watchAACT.ElapsedMilliseconds / 1000.0;
                                    bSensor = false;
                                    AppendText("固化已取完料");
                                    AAJigsStopCylinder.Reset();
                                    step = 350;
                                }                                
                            }
                            break;
                        case 345://优化 增加 出料感应 2个感应器来保障
                            if ((!IoPoints.IDI926.Value || IoPoints.IDI922.Value || IoPoints.IDI921.Value) && !IoPoints.IDI90.Value/*||stationOperate.Running*/)//已取完料
                            {
                                if (!bSensor) { bSensor = true; _watch.Restart(); }
                                if (_watch.ElapsedMilliseconds > 5)
                                {
                                    Marking.AACycleTime = _watchAACT.ElapsedMilliseconds / 1000;
                                    bSensor = false;
                                    AppendText("固化已取完料");
                                    AAJigsStopCylinder.Reset();
                                    step = 350;
                                }
                            }
                            break;
                        case 350:
                            if (AAJigsStopCylinder.OutOriginStatus)
                            {
                                Thread.Sleep(AxisParameter.Instance.AAOutJigsDelay);
                                if(!IoPoints.IDI926.Value) IoPoints.IDO96.Value = false;//联机信号
                               
                                step = 360;
                            }
                            break;
                        case 360:
                            step = 0;
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
                            IoPoints.IDO915.Value = false;//断电
                            //流水线停止？
                            IoPoints.IDO90.Value = false;
                            stationInitialize.Flow = 10;
                            //给固化1结果清除
                            IoPoints.IDO910.Value = false;
                            IoPoints.IDO911.Value = false;
                            IoPoints.IDO96.Value = false;//联机信号
                            IoPoints.IDO97.Value = false;//联机信号
                            Marking.AAAlreadyGetJigsFromStockp = false;
                            Marking.AAWorkAlreadyRequest = false;
                            Marking.AAUpClyIsMove = false;
                            GHhaveGetJigs = true;
                            break;
                        case 10:
                            //AA复位
                            try
                            {
                                if (Marking.AAhomeShield) Marking.AAfinishAtHome = true;
                                else Marking.AAfinishAtHome = CallAA.bResetHome();
                            }
                            catch { }

                            stationInitialize.Flow = 20;
                            break;
                        case 20://复位气缸
                            
                            AAJigsStopCylinder.InitExecute();//夹具阻挡
                            AAJigsStopCylinder.Reset();
                            Thread.Sleep(100);
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                AAJigsCylinder_Small.InitExecute();//探针
                                AAJigsCylinder_Small.Reset();
                            }
                           
                            stationInitialize.Flow = 25;
                            break;
                        case 25:
                            if (AxisParameter.Instance.IsUseTanZhenCyl)
                            {
                                if (AAJigsCylinder_Small.OutOriginStatus)
                                {
                                   
                                    AAJigsUpCylinder.InitExecute();//顶升
                                    AAJigsUpCylinder.Reset();
                                    _watch.Restart();
                                    stationInitialize.Flow = 30;
                                }
                            }
                            else
                            {
                                AAJigsUpCylinder.InitExecute();//顶升
                                AAJigsUpCylinder.Reset();
                                _watch.Restart();
                                stationInitialize.Flow = 30;
                            }
                            break;
                       case 30:
                            if (AAJigsUpCylinder.OutOriginStatus)
                            {
                                stationInitialize.Flow = 40;
                            }
                            else
                            {
                                _watch.Stop();
                                if (_watch.ElapsedMilliseconds / 1000 > 5)
                                {
                                    _watch.Restart();
                                    stationInitialize.InitializeDone = false;
                                  
                                    AppendText("AA段顶升气缸感应异常");
                                    stationInitialize.Flow = -1;
                                }
                                _watch.Start();
                            }
                            break;
                        case 40://流水线停止转动
                            if (Marking.AAfinishAtHome)
                                stationInitialize.Flow = 50;
                            else { stationInitialize.Flow = -1; AppendText($"AA工位复位失败！！"); }
                            break;
                        case 50://复位完成，置位初始化标志
                            //Thread.Sleep(200);
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
            //list.AddRange(LightZaxis.Alarms);
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.初始化故障)
            {
                AlarmLevel = AlarmLevels.None,
                Name = PlateformAlarm.初始化故障.ToString()
            });

            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.预AA结果完成异常)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.预AA结果完成异常.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.预AA结果连续NG)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.预AA结果连续NG.ToString()
            });

            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.AA工位报警中)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.AA工位报警中.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.AA报警重新复位失败请停止设备)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.AA报警重新复位失败请停止设备.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.保存Mes数据失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.保存Mes数据失败.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.二维码校验失败)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.二维码校验失败.ToString()
            });
            list.Add(new Alarm(() => m_Alarm == PlateformAlarm.AA工位Ready超时)
            {
                AlarmLevel = AlarmLevels.Error,
                Name = PlateformAlarm.AA工位Ready超时.ToString()
            });
            list.AddRange(AAJigsStopCylinder.Alarms);
            list.AddRange(AAJigsUpCylinder.Alarms);
            list.AddRange(AAJigsCylinder_Small.Alarms);
            //list.AddRange(AAClampClawCylinder.Alarms);
            //list.AddRange(AAStockpileStopCylinder.Alarms);
            //list.AddRange(AAStockpileUpCylinder.Alarms);
            //list.AddRange(AAUVUpDownCylinder.Alarms);
            return list;
        }
        /// <summary>
        /// 气缸状态集合
        /// </summary>
        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();

            list.Add(AAJigsStopCylinder);
            list.Add(AAJigsUpCylinder);
            list.Add(AAJigsCylinder_Small);
            //list.Add(AAClampClawCylinder);
            //list.Add(AAStockpileStopCylinder);
            //list.Add(AAStockpileUpCylinder);
            //list.Add(AAUVUpDownCylinder);
            return list;
        }
        #endregion

        private enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            气缸不在状态位,       
            预AA结果完成异常,
            预AA结果连续NG,
            AA工位报警中,
            AA报警重新复位失败请停止设备,
            保存Mes数据失败,
            二维码校验失败,
            AA工位Ready超时
        }

        /// <summary>
        /// AA状态
        /// </summary>
        public enum AAstatus : int
        {
            AA_Ready = 0,
            AA_Reseting,
            AA_Testing,
            AA_Warming,
            AA_Pause,
            AA_Finish
        }

        public enum AAFunctionResult:int 
        {
            AA_PASS = 0,
            AA_LightON_NG,
            AA_MOVEControl_NG,
            AA_SEARCH_NG,
            AA_OC_TUNE_NG,
            AA_TILT_TUNE_NG,
            AA_UVBefore_Check_NG,
            AA_UVAfter_Check_NG,
            AA_SN_NG,
            CleanMesLock_NG,
            Glue_Wb_LightNG,
            Glue_Vision_LocationNG,
            Glue_Vision_FindNG,
            null_NG,
            Glue_Wb_ParticeNG,
            AA_LightLossFrame_NG
        };
   
    }
}
