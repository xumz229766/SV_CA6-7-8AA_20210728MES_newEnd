using Motion.Enginee;
using System;
using System.Collections.Generic;
using Motion.Interfaces;
using System.Toolkit;
using System.Device;
using System.Threading;
using desay.ProductData;
using System.IO;
using System.Diagnostics;
namespace desay.Flow
{
    public class MES : StationPart
    {
        private PlateformAlarm m_Alarm;
        /// <summary>AA通讯服务</summary>
        public AsynTcpServer aaServer { get; set; }


        /// <summary>测高模块</summary>
        //public Panasonic HeightDectector { get; set; }
        /// <summary>Mes的AA数据</summary>
        public MesData.AAData MesAAData = new MesData.AAData();


        public Thread threadDealMsg = null;
        IAsyncResult SNResult;
        IAsyncResult FNResult;
        public string StartTime = null;
        public string EndTime = null;
        bool RequestLocation = false;
        bool RequestResult = false;

        private void DealMsg()
        {
            var _watch = new Stopwatch();
            _watch.Start();
            while (true)
            {
                //增加plasma和点胶电脑卡死控制

                if (!Marking.PlcReflashShield)
                {
                    if (IoPoints.IDO24.Value)
                    {
                        Marking.PlcRefashTimes = _watch.ElapsedMilliseconds / 1000.0;
                        _watch.Restart();
                        IoPoints.IDO24.Value = false;
                        Thread.Sleep(10);
                        IoPoints.TDO15.Value = false;
                        Thread.Sleep(AxisParameter.Instance.PlcReflashTime);
                    }
                    else
                    {
                        Marking.PlcRefashTimes = _watch.ElapsedMilliseconds / 1000.0;
                        _watch.Restart();
                        IoPoints.IDO24.Value = true;
                        Thread.Sleep(10);
                        IoPoints.TDO15.Value = true;
                        Thread.Sleep(AxisParameter.Instance.PlcReflashTime);
                       
                    }

                }
            }
         }

        public MES(External ExternalSign, StationInitialize stationIni, StationOperate stationOpe)
                        : base(ExternalSign, stationIni, stationOpe, typeof(MES))
        {
            threadDealMsg = new Thread(DealMsg);
            threadDealMsg.IsBackground = true;
        }
        public override void Running(RunningModes runningMode)
        {
            int count = 0;
            while (true)
            {
                Thread.Sleep(10);
                #region 自动流程
                if (stationOperate.Running)
                {
                    switch (step)
                    {
                        case 0://控制流程结束，接收控制流程数据信息
                            if (Marking.AaGetDataFlg)
                            {
                                Marking.AaGetDataFlg = false;
                                WriteFile();
                                step = 100;
                            }
                            break;
                        case 10:
                            break;
                        case 20://
                            break;
                        case 30://
                            break;
                        case 100://复位各标志位
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
                            stationInitialize.Flow = 10;
                            break;
                        case 10://检查MES连接
                            Thread.Sleep(200);
                            stationInitialize.Flow =90;
                            break;
                        case 20://检查CCD连接
                            //if (Marking.CCDShield)
                            {
                                Marking.GlueResult = false;
                                stationInitialize.Flow = 30;
                            }
                            break;
                        case 30://检查白板连接
                            //if (Marking.WhiteShield || Marking.CleanShield)
                            {
                                Marking.WbData = null;
                                Marking.WhiteBoardResult = false;
                                Marking.WbRequestResultFlg = false;
                                Marking.WbGetResultFlg = false;
                                //AppendText("白板复位完成！");
                                stationInitialize.Flow = 40;
                            }
                            break;
                        case 40://检查AA连接 不需要了西威
                            //if (aaServer._status || Marking.AAShield)
                            {
                                Marking.AaAllowPassFlg = false;
                                Marking.AACallIn = false;
                                Marking.AAData = null;
                                Marking.AaGetDataFlg = false;
                                Marking.AaGetResultFlg = false;
                                Marking.AAResult = false;
                                Marking.AaSendCodeFlg = false;
                                //AppendText("AA复位完成！");
                                stationInitialize.Flow =90;
                            }
                            break;
                        //case 70:
                        //    if (HeightDectector.IsOpen)
                        //    {
                        //        Marking.RequestHeightFlg = false;
                        //        Marking.GetHeightFlg = false;
                        //        AppendText("测高模块复位完成！");
                        //        stationInitialize.Flow = 90;
                        //    }
                        //    break;
                        case 90://复位完成，置位初始化标志
                            lock (MesData.MesDataLock)
                            {
                                MesData.MesDataList.Clear();
                            }
                            lock (MesData.AADataLock)
                            {
                                if (MesData.ResultList.Count > 0)
                                    MesData.ResultList.Clear();
                                if (MesData.AADataList.Count > 0)
                                    MesData.AADataList.Clear();
                                
                            }
                            Thread.Sleep(200);
                            stationInitialize.InitializeDone = true;
                            AppendText($"{Name}初始化完成！");
                            stationInitialize.Flow = 100;
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                if (AlarmReset.AlarmReset)
                {
                    m_Alarm = PlateformAlarm.无消息;
                }
            }
        }

        protected override IList<Alarm> alarms()
        {
            var list = new List<Alarm>();
            return list;
        }

        protected override IList<ICylinderStatusJugger> cylinderStatus()
        {
            var list = new List<ICylinderStatusJugger>();
            return list;
        }

        public enum PlateformAlarm : int
        {
            无消息,
            初始化故障,
            MES通讯超时,
            MES通讯失败次数超限度,
            MES通讯连接失败,
            入料请求发送失败,
            白板发送失败,
            接收到错误字符,
            CCD发送失败,
            结果发送失败,
            接收到AA字符为空,
            AA返回的结果数据缺失,
            AA返回结果码数据缺失,
            未找到当前治具码数据,
            写入MES数据文件失败
        }

        public void SendRequestMsg(int type)
        {
            //#region 测高
            //if(type==1)
            //{
            //    if (Marking.RequestHeightFlg)
            //    {
            //        AppendText("请求测高数据！");
            //        Marking.RequestHeightFlg = false;
            //        Marking.GetHeightFlg = false;
            //        HeightDectector.WriteDetectHeightCommand();
            //    }
            //}
            //#endregion


            #region AA
            if(type==5)
            {
                if (Marking.AaAllowPassFlg && Marking.AaClientOpenFlg)
                {
                    AppendText("允许AA放行治具！");
                    Marking.AaAllowPassFlg = false;
                    try
                    {
                        lock (MesData.AALock)
                        {
                            aaServer.AsynSend(string.Format("$AHB01\r\n"));
                            Thread.Sleep(300);
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Alarm = PlateformAlarm.入料请求发送失败;
                    }
                }
                if (Marking.AaSendCodeFlg && Marking.AaClientOpenFlg)
                {
                    Marking.AaSendCodeFlg = false;
                    try
                    {
                        string strMsg = null;
                        //Marking.FN = "1111";
                        //aaServer.AsynSend(string.Format("$AHVSN{0}#\r\n", Marking.FN));
                        lock (MesData.GlueDataLock)
                        {
                            if (Marking.GlueShield) { Marking.GlueResult = true; }
                            if (Marking.CleanShield) { MesData.glueData.cleanData.CleanResult_MesLock = true; }

                         
                            //Config.Instance.GlueProductNgTotal++;
                            strMsg = (!MesData.glueData.cleanData.CleanResult_MesLock || !Marking.GlueResult) ? "10" : "01";  
                            if ( !Marking.HaveLensShield && !Marking.CleanShield)
                                strMsg += "NG1";
                            else if ( !Marking.WhiteShield && !Marking.CleanShield)
                                strMsg += "NG2";
                            else if (!Marking.GlueResult && !Marking.GlueShield && !Marking.CCDShield)
                                strMsg += "NG3";

                          

                            lock (MesData.AALock)
                            {
                                aaServer.AsynSend(string.Format("$AHVSN{0},{1}#\r\n", MesData.glueData.cleanData.JigsSN, MesData.glueData.cleanData.HolderSN));
                                Thread.Sleep(300);
                                aaServer.AsynSend(string.Format("$AHR{0}\r\n", strMsg));
                                Thread.Sleep(300);
                                strMsg += "*" + MesData.glueData.cleanData.JigsSN;
                            }
                            AppendText("发送AA治具码及结果数据！" + strMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Alarm = PlateformAlarm.结果发送失败;
                    }
                }
            }
            #endregion

            #region 传料给AA
            if (type == 6)
            {
                if (Marking.AaSendCodeFlg && Marking.AaClientOpenFlg)
                {
                    Marking.AaSendCodeFlg = false;
                    try
                    {
                        string strMsg = null;
                        //Marking.FN = "1111";
                        //aaServer.AsynSend(string.Format("$AHVSN{0}#\r\n", Marking.FN));
                        lock (MesData.GlueDataLock)
                        {
                            if (Marking.GlueShield) { Marking.GlueResult = true; }
                            if (Marking.CleanShield) { MesData.glueData.cleanData.CleanResult_MesLock = true; }

                           
                            //Config.Instance.GlueProductNgTotal++;
                            strMsg = (!MesData.glueData.cleanData.CleanResult_MesLock || !Marking.GlueResult) ? "10" : "01";
                            if ( !Marking.HaveLensShield && !Marking.CleanShield)
                                strMsg += "NG1";
                            else if ( !Marking.WhiteShield && !Marking.CleanShield)
                                strMsg += "NG2";
                            else if (!Marking.GlueResult && !Marking.GlueShield && !Marking.CCDShield)
                                strMsg += "NG3";

                        
                          
                            lock (MesData.AALock)
                            {
                                string HolderSN = "";
                                string LensSN ="";
                                string BatchSN = "";
                                if (MesData.AAData.HodlerSN.Count > 0)
                                {
                                    log.Debug($"SN赋值");
                                    HolderSN = MesData.AAData.HodlerSN[0];
                                     LensSN = MesData.AAData.LensSN[0];
                                     BatchSN = MesData.AAData.BatchSN[0];
                                }
                               
                                log.Debug($"发送给AA相关二维码FN{MesData.glueData.cleanData.JigsSN}，HSN:{HolderSN}，Lens:{LensSN},B:{BatchSN}");
                                aaServer.AsynSend(string.Format("$AHVSN{0},{1},{2},{3}#\r\n", MesData.glueData.cleanData.JigsSN, HolderSN, LensSN, BatchSN));

                                if (MesData.AAData.HodlerSN.Count > 0)
                                {
                                    MesData.AAData.BatchSN.RemoveAt(0);
                                    MesData.AAData.HodlerSN.RemoveAt(0);
                                    MesData.AAData.LensSN.RemoveAt(0);
                                }


                                Thread.Sleep(300);
                                aaServer.AsynSend(string.Format("$AHR{0}\r\n", strMsg));
                                Thread.Sleep(300);
                                strMsg += "*" + MesData.glueData.cleanData.JigsSN + HolderSN + LensSN + BatchSN;
                                log.Debug($"发送给AA相关二维码数据及结果数据{strMsg}");
                            }
                           
                            AppendText("发送AA相关二维码数据及结果数据！" + strMsg+"；");
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Alarm = PlateformAlarm.结果发送失败;
                    }
                }
            }
            #endregion
        }

        public void WriteFile()
        {
            if (Marking.SnScannerShield)
                return;
            try
            {
                log.Debug("开始解析AA数据");
                string[] temp1 = Marking.AAData.Split(':');
                if (temp1.Length < 2)
                {
                    m_Alarm = PlateformAlarm.AA返回的结果数据缺失;
                    return;
                }
                string[] code = temp1[0].Split(',', ';');
                if (code.Length < 2)
                {
                    m_Alarm = PlateformAlarm.AA返回结果码数据缺失;
                    return;
                }
                string fn = code[0];
                log.Debug("从AA数据中获取治具码");
                MesData.GlueData data = new MesData.GlueData();
                lock (MesData.MesDataLock)
                {
                    if (!MesData.MesDataList.ContainsKey(fn))
                    {
                        m_Alarm = PlateformAlarm.未找到当前治具码数据;
                        return;
                    }
                    data = MesData.MesDataList[fn];
                }
                log.Debug("根据治具码获取对应的数据信息");
                string FileName = AppConfig.MesShareFileFolderName + data.cleanData.JigsSN.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                string Header = "\"" + data.cleanData.JigsSN.Trim() + "\";\"" + data.cleanData.StartTime.Trim() + "\";\"" + EndTime.Trim() + "\";\""
                    + (MesAAData.aaResult ? "OK" : "NG") + "\""/* + "\r\n"*/;
                //string Body = "\"1\";\"HaveLens\";;;\"" + data.cleanData.HaveLens.Trim() + "\";;\"T\";\"" + data.cleanData.HaveLens.Trim() + "\";\r\n"
                //    + data.cleanData.WbData.Trim() + "\r\n";
                //if (!data.cleanData.HaveLens.Trim().Contains("NG") && !data.cleanData.WbData.Trim().Contains("NG"))
                //{
                //    Body += "\"1\";\"GlueCheck\";;;\"" + data.GlueResult.Trim() + "\";;\"T\";\"" + data.GlueResult.Trim() + "\";\r\n"
                //            + "\"1\";\"LightCamera\";;;\"" + (MesAAData.lightCameraRst ? "OK" : "NG") + "\";;\"T\";\"" + (MesAAData.lightCameraRst ? "OK" : "NG") + "\";\r\n"
                //            + "\"1\";\"PreAAPos\";;;\"" + (MesAAData.preAAPosRst ? "OK" : "NG") + "\";;\"T\";\"" + (MesAAData.preAAPosRst ? "OK" : "NG") + "\";\r\n"
                //            + "\"1\";\"SearchPos\";;;\"" + (MesAAData.searchPosRst ? "OK" : "NG") + "\";;\"T\";\"" + (MesAAData.searchPosRst ? "OK" : "NG") + "\";\r\n"
                //            + "\"1\";\"OCAdjust\";;;\"" + (MesAAData.ocAdjustRst ? "OK" : "NG") + "\";;\"T\";\"" + (MesAAData.ocAdjustRst ? "OK" : "NG") + "\";\r\n"
                //            + "\"1\";\"Tiltadjust\";;;\"" + (MesAAData.tiltAdjustRst ? "OK" : "NG") + "\";;\"T\";\"" + (MesAAData.tiltAdjustRst ? "OK" : "NG") + "\";\r\n";
                //}
                //for (int i = 1; i < temp1.Length - 1; i++)
                //{
                //    Body += temp1[i];
                //}
                //string Footer = "##\r\n";
                log.Debug("MES数据组合完成");
                lock (MesData.MesDataLock)
                {
                    MesData.MesDataList.Remove(fn);
                }

                //if (!Marking.SnScannerShield)
                //{
                //    FileStream fs = new FileStream(FileName.Trim(), FileMode.Create);
                //    StreamWriter sw = new StreamWriter(fs);
                //    写入数据
                //    sw.WriteLine(Header);
                //    sw.WriteLine(Body + Footer);
                //    sw.WriteLine(Footer);
                //    清空缓冲区
                //    sw.Flush();
                //    关闭流
                //    sw.Close();
                //    fs.Close();
                //    log.Debug("MES数据写入完成！");
                //}

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                log.Error(e.StackTrace);
                m_Alarm = PlateformAlarm.写入MES数据文件失败;
            }
        }
    }
}
