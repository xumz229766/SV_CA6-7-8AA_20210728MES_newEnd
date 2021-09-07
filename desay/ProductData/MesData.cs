using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desay.ProductData
{
    public class MesData
    {
        //public static MesData Instance = new MesData();
   
        public struct CarrierData
        {
            public string JigSN;

            public string HoldersSN;

            public string StartTime;
        }
        public static object CleanDataLock = new object();

        public static object CarrierDataLock = new object();
        public struct PreAAData  //new preAA
        {
            //public CarrierData carrierData;
            public string HolderSN;
            public string LensSN;
            public string BatchSN;
            public bool  PreAAFinish;

            public bool  PreAAResult;

        }
        public static object PreAADataLock = new object();

        public struct CleanData
        {
            //public CarrierData carrierData;

            public string HolderSN;

            public string JigsSN;
            //public string WbData;

            //public string WbResult;

            //public string HaveLens;
            public string StartTime;
            public bool CleanResult_MesLock;
        }
        public static object GlueDataLock = new object();
        public struct GlueData
        {
            public CleanData cleanData;

            //public string GlueParam;

            public WbData wbData;
            public GlueVisionData glueVisionData;

            public bool AllglueStationResult;
        }
        public static object AADataLock = new object();
        public struct AAStockplieData
        {
            public GlueData glueData;
        }
        public static object AAStockplieDataLock = new object();
        public struct WbData
        {
            public bool AllResult;
            public bool DefectStainTestResult;
            public bool LightResult;
        }
        public struct GlueVisionData
        {
            public bool AllResult;
            public bool LoadcationResult;//定位
            public bool GlueFindResult;//识别
        }
        public struct AAData
        {
            public AAStockplieData aaStockpileData;
            public bool AllResult;           // 总结果
            public bool aaResult;           // AA工位结果标志
            public bool lightCameraRst;     // 点亮结果
            public bool preAAPosRst;        // 预AA位置结果
            public bool searchPosRst;       // 搜索定位结果
            public bool ocAdjustRst;        // 中心调整结果
            public bool tiltAdjustRst;      // 倾斜调整结果
            public bool uvBeforeRst;        // UV前结果
            public bool uvAfterRst;         // UV后结果
            public bool haveLensRst;        // 有无料检测结果
            public bool whiteBoardRst;      // 白板检测结果
            public bool glueCheckRst;       // 点胶检测结果
            public static List<string> HodlerSN = new List<string>();     //收到的前段镜座的二维码
            public static List<string> LensSN = new List<string>();      //收到的前段镜头的二维码
            public static List<string> BatchSN = new List<string>();    //收到的前段批次码
        }
        
        //public static CarrierData carrierData = new CarrierData();
        public static CleanData cleanData = new CleanData();
        public static GlueData glueData = new GlueData();
        public static AAStockplieData aaStockPileData = new AAStockplieData();
        public static AAData aaData = new AAData();
        public static PreAAData preAAData = new PreAAData();
        public static object MesDataLock = new object();
        public static Dictionary<string, GlueData> MesDataList = new Dictionary<string, GlueData>();
        public static Dictionary<string, AAData> ResultList = new Dictionary<string, AAData>();
        public static List<AAData> AADataList = new List<AAData>();
        public static string NeedShowFN = "123";
        //public static List<string> SNReturnRobot = new List<string>();
        public static object AALock = new object();
        /// <summary>
        /// Mes数据词典
        /// </summary>
        //public static Dictionary<string, Data> MesDataDictionary = new Dictionary<string, Data>();
    }
}
