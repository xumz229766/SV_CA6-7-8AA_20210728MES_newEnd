using System;
using SV.Core.Common;
using sv_Interlocking_Main;
using System.Data.SqlClient;
using System.Data;
using desay.ProductData;
using MES_Interlock_DLL_EDCMESASS_64;
//using log4net;
namespace desay
{
    class Common
    {

        #region 参数    
        //protected static ILog log;
        public static SvMesHelper mes;

        //路径参数
        public static string OiIniPath = FileHelper.AppPath + @"Config\OI_Production_Info_Config.ini";
        public static string CommonIniPath;
        public static string ModelIniPath;
        public static string ProductIniPath = FileHelper.AppPath + @"Config\Product.ini";
        /// <summary>
        /// MesTxt用于数据采集上传的TXT所在的文件夹路径
        /// </summary>
        public static string MesTxtSaveDirPath;
        public static string MesCsvSaveDirPath;
        public static string ImageSaveDirPath;
        /// <summary>
        /// MesTxt中用于上传的测试项所在的文件路径
        /// </summary>
        public static string MesTestItemsFilePath = FileHelper.AppPath + @"Config\MesData.txt";

        //设备参数
        public static string DMMTestAddress;
        public static bool IsAdmin;
        public static string Line;
        public static Object obj { get; set; } = new object();

        //机型参数
        public static string ModelSerial;
        public static string SN_Prefix;
        public static int SN_Length;
        public static string TestResult;
        public static string SersorFilePath;
        public static string[] AllA2Cs;
        public static int PassNum;
        public static int FailNum;
        //软件参数
        public static bool IsLogShowClassAndFunctionName { get; set; } = true;
        public static INIFile INIFile = new INIFile();
        public static bool IsToPutImage { get; set; } = false;



        #endregion

        #region 常规与机型配置读写
        public static void ReadCommonIniFile()
        {
            try
            {
                string computerName = "lcw-rd";
                Line = Common.INIFile.ReadValue(OiIniPath, computerName, "Line", "");
                CommonIniPath = FileHelper.AppPath + @"Config\" + Common.INIFile.ReadValue(OiIniPath, computerName, "CommonIniPath", "");
                ModelIniPath = FileHelper.AppPath + @"Config\" + Common.INIFile.ReadValue(OiIniPath, computerName, "ModelIniPath", "");
                mes = new SvMesHelper(CommonIniPath, MesTestItemsFilePath);

                DMMTestAddress = Common.INIFile.ReadValue(CommonIniPath, "DMMTest", "Address");
                MesTxtSaveDirPath = Common.INIFile.ReadValue(CommonIniPath, "MesData", "MesTxtSaveDirPath");
                MesCsvSaveDirPath = Common.INIFile.ReadValue(CommonIniPath, "MesData", "MesCsvSaveDirPath");
                ImageSaveDirPath = Common.INIFile.ReadValue(CommonIniPath, "Common", "ImageSaveDirPath");

           //     mes.EvData.BatchSerialNumber = "SV";//用于区分供应商软件与自主软件
                                                    //mes.EvData.StationID = Common.INIFile.ReadValue(CommonIniPath, "MesData", "StationID");


            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region 生成报告
        /// <summary>
        /// 生产报告
        /// </summary>
        /// <param name="ProductDate">当前产品测试数据</param>
        /// <param name="cameraTest">相机设定参数数据</param>
        public static void SendTestDataToME_MSTR(string sn, vvdatax d1, vvdatax d2, vvdatax d3, vvdatax d4, vvdatax d5, vvdatax d6, vvdatax d7,bool MesLock=true,bool WbLight=true,bool WbParitice=true,bool GlueLoacation=true,bool GlueFind=true,bool AALight=true,bool AALightLossFrame=true)
        {
            int order = 0;
            #region 通用数据
            mes.add_ME_MSTR_Record(ref order, mes.EvData.TestStatus.ToString());
            mes.add_ME_MSTR_Record(ref order, sn);
            mes.add_ME_MSTR_Record(ref order, mes.EvData.LoginName);
            mes.add_ME_MSTR_Record(ref order, DateTime.Now.ToString());
            mes.add_ME_MSTR_Record(ref order, mes.EvData.IsQueryInterlocking);
            mes.add_ME_MSTR_Record(ref order, $"Sensor:{FileHelper.GetFileName(SersorFilePath)}");
            #endregion

            if (MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (WbLight&& MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (WbParitice&& WbLight && MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (GlueLoacation&& WbLight&& WbParitice&& MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (GlueFind&& GlueLoacation && WbLight && WbParitice && MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (AALight&& GlueFind && GlueLoacation && WbLight && WbParitice && MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            if (AALightLossFrame&& AALight&& GlueFind && GlueLoacation && WbLight && WbParitice && MesLock) mes.add_ME_MSTR_NumericTest(ref order, 0, 2, 1);
            else mes.add_ME_MSTR_NumericTest(ref order, 0, 2, -2);
            mes.add_ME_MSTR_NumericTest(ref order, d1.vvmin, d1.vvmax, d1.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d2.vvmin, d2.vvmax, d2.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d3.vvmin, d3.vvmax, d3.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d4.vvmin, d4.vvmax, d4.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d5.vvmin, d5.vvmax, d5.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d6.vvmin, d6.vvmax, d6.vvdata);
            mes.add_ME_MSTR_NumericTest(ref order, d7.vvmin, d7.vvmax, d7.vvdata);
            

        }
        public static bool Test_WriteMesTxtAndCsvFile(string sn,string tool3,   bool ifok, vvdatax d1, vvdatax d2, vvdatax d3, vvdatax d4, vvdatax d5, vvdatax d6, vvdatax d7, bool MesLock = true, bool WbLight = true, bool WbParitice = true, bool GlueLoacation = true, bool GlueFind = true, bool AALight = true, bool AALightLossFrame = true)
        {

            lock (obj)
            {

                bool result = false;
                try
                {
                    mes.EvData.BatchSerialNumber = tool3;//用于区分供应商软件与自主软件
                    //mes.EvData.DeviceA2C = sn.Substring(0,12);
                    mes.EvData.DeviceA2C = sn.Substring(0, 12);//9.22临时修改
                    mes.EvData.SerialNumber = sn;
                    if(ifok)
                    mes.EvData.TestStatus = "Passed";
                    else
                        mes.EvData.TestStatus = "Failed";

                   

                   SendTestDataToME_MSTR(sn,d1,d2,d3,d4,d5,d6,d7,MesLock, WbLight, WbParitice,GlueLoacation, GlueFind, AALight,AALightLossFrame);
                    bool isWriteMesTxtSucceed = mes.WriteMesTxtToFile($"{MesTxtSaveDirPath}{mes.EvData.DeviceA2C}_{mes.EvData.SerialNumber}_{DateTime.Now.ToString("yyMMddHHmmss")}_{mes.EvData.TestStatus}.txt");
                    //bool isWriteCsvSucceed = mes.AppendTestValuesRoCsvFile($"{MesCsvSaveDirPath}{mes.EvData.ModelName.Replace("/", "-")}_Camera Test_{DateTime.Now.ToString("yyMMdd")}.csv");


                }
                catch (Exception ex)
                {
                    LogHelper.Info("写入失败:" + ex.ToString());

                }
                return result;
            }

        }
      
        public static bool Test_FailFile(int num,string meg)
        {

            lock (obj)
            {

               
                try
                {


                    mes.add_ME_MSTR_PassFailTest(0,0,meg,"",0,"");
                    
                }
                catch (Exception ex)
                {
                    LogHelper.Info("写入失败:" + ex.ToString());

                }
                return true;
            }

        }
        public static bool Test_ScanSN(bool IsLock, string sn)
        {
            bool Result = true;
            string mesInfo = $"{Common.mes.EvData.DB_Password},{Common.mes.EvData.DB_User},{Common.mes.EvData.DatabaseName},{Common.mes.EvData.ServerName},{sn},{Common.mes.EvData.StationID},{Common.mes.EvData.LineGroup},{Common.mes.EvData.LoginName},True,False,False,7";
            
            if (IsLock)
            {
                try
                {

                    Result = SNStatus(sn, Common.mes.EvData.StationID).ToUpper() == "PASSED";
                    //Result = sv_Interlocking_Main_Class.SV_Interlocking_Main(mesInfo) == 0 ? true : false;
                }
                catch(Exception ex) { Result = false; }
            }
              
            return Result;

        }

        public static string SNStatus(string SN, string frontStationID)
        {
            string strResult = "Error";
            using (SqlConnection conn_HZHE015A = new SqlConnection())
            {
                string constr = "Server=" + Common.mes.EvData.ServerName + ";" + "user=" + Common.mes.EvData.DB_User + ";" + "pwd=" + Common.mes.EvData.DB_Password + ";" + "database=" + Common.mes.EvData.DatabaseName;
                conn_HZHE015A.ConnectionString = constr;
                //string snOrder = "SELECT TOP 1  UUT_STATUS  from UUT_RESULT where UUT_SERIAL_NUMBER = '" + SN + "' AND STATION_ID = '" + "MED-MSCREW-0017" + "' order by START_DATE_TIME desc";//查询数据库数据结果 //frontStationID MED-MSCREW-0017
                string snOrder = "SELECT TOP 1  UUT_STATUS  from UUT_RESULT where UUT_SERIAL_NUMBER = '" + SN + "' AND STATION_ID = '" + Config.Instance.FormerStationID + "' order by START_DATE_TIME desc";//查询数据库数据结果 //frontStationID MED-MSCREW-0017
                DataTable status = ReadOrderData(constr, snOrder);
                try
                {
                    strResult = status.Rows[0][0].ToString();
                    //LogHelper.Info(SN + ",SN查询结果：" + strResult);
                }
                catch (Exception ex)
                {

                    //LogHelper.Error(SN + ",查询异常" + ex);
                }

            }
            return strResult;
        }
        /// <summary>
        /// 数据库查询Conm
        /// </summary>
        /// 
        private static DataTable ReadOrderData(string connectionString, string queryString)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    data.Load(reader);
                    reader.Close();

                }
                catch (Exception ex)
                {
                    //LogHelper.Error("查询异常" + ex);
                }
                return data;

            }
        }
        //public static bool TestMSA_WriteMesTxtAndCsvFile(Product Product, WirteResultData WirteResultData, BlackResultData BlackResultData, string productSn, CameraHDTest cameraTest, double current, string Version, string PATH, string SenserID)
        //{

        //    lock (obj)
        //    {

        //        bool result = false;
        //        try
        //        {

        //            mes.EvData.SerialNumber = productSn;
        //            bool CurrentTestResult = current >= cameraTest.CurrentTest_Limit.Split('|')[0].ToDouble() &&
        //                    current <= cameraTest.CurrentTest_Limit.Split('|')[1].ToDouble(); //电流结果
        //            bool ix = Product.Colorresulr && Product.DFOVresulr && Product.OCresulr
        //                && Product.Grayresulr && Product.SFRresulr;  //图卡结果
        //            bool WirteTestResult = WirteResultData.WBresulr && WirteResultData.Darkresulr
        //                && WirteResultData.GrayAreaIResult && WirteResultData.GrayCountIResult;  //白板结果
        //            bool BlackTestResult = BlackResultData.Lightresulr &&
        //               Version == cameraTest.VersionLimit;  //黑板结果
        //            mes.EvData.TestStatus = CurrentTestResult && ix && WirteTestResult && BlackTestResult ? "Pass" : "Fail";
        //            SendTestDataToME_MSTR(Product, WirteResultData, BlackResultData, cameraTest, current, Version, SenserID);
        //            bool isWriteMesTxtSucceed = mes.WriteMesTxtToFile($"{PATH}{mes.EvData.DeviceA2C}_{mes.EvData.SerialNumber}_{DateTime.Now.ToString("yyMMddHHmmss")}_{mes.EvData.TestStatus}.txt");
        //            bool isWriteCsvSucceed = mes.AppendTestValuesRoCsvFile($"{PATH}{mes.EvData.ModelName.Replace("/", "-")}_Camera Test_{DateTime.Now.ToString("yyMMdd")}.csv");
        //            System.ToolKit.LogHelper.Info("写入ok:");
        //            result = isWriteCsvSucceed && isWriteMesTxtSucceed;
        //        }
        //        catch (Exception ex)
        //        {
        //            System.ToolKit.LogHelper.Info("写入失败:" + ex.ToString());

        //        }
        //        return result;
        //    }


        //}


        #endregion

        #region 20210722SV增加Mes出入站 直接调用dll
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SN"></param>
            /// <param name="StationID"></param>
            /// <param name="jobNumber"></param>
            /// <returns></returns>
        public static bool MesMoveIn(string SN,string jobNumber)
        {
            string messtring = $"{SN},{mes.EvData.StationID},,{jobNumber},True";
            short bresult=MES_Interlock_DLL_EDCMESASS_64_Class.MoveIn(messtring);

            //log.Debug($"Mes模式MoveIn返回结果{MES_Interlock_DLL_EDCMESASS_64_Class.ResultStr}");
            return bresult==0?true:false;
        }
        public static string MesMoveOut(string SN,  string jobNumber,bool bresult)
        {
            string sresult = bresult ? "1" : "0";
            string messtring = $"{SN},{mes.EvData.StationID},{DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss") },{sresult},{jobNumber},True";
            short result = MES_Interlock_DLL_EDCMESASS_64_Class.MoveStd(messtring);
            return MES_Interlock_DLL_EDCMESASS_64_Class.ResultStr;
            //log.Debug(($"Mes模式MoveOut返回结果{MES_Interlock_DLL_EDCMESASS_64_Class.ResultStr}");
        }
        #endregion
    }




    public class vvdatax
    {

        public double vvmin;
        public double vvmax;
        public double vvdata;

    }
}
