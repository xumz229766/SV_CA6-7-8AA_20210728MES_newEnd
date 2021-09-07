using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HalconDotNet;
using desay.ProductData;
namespace desay.Vision
{
    [Serializable]
    public class VisionProductData
    {
        [NonSerialized]
        public static VisionProductData Instance = new VisionProductData();
        #region 相机参数
        public int nExposure = 6000;                                      //相机曝光
        public int nGain = 0;                                             //相机增益
        public int nLight = 0;                                            //光源亮度
        public int nLight_Glue = 0;                                            //光源亮度
        public int nLightChanel = 1;//光控通道
        public bool bSaveImg = false;                                     //自动运行是否保存图像

        public string strImgPath;                                         //图像路径

        
        #endregion
    }

    public class VisionMarking
    {
        public static string VisionName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{Config.Instance.CurrentProductType}.xml");
            }
        }
        public static string VisionFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\VisionConfig.xml");
            }
        }
        public static string CamParName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vision\\cam.cal");
            }
        }
        public static string PosFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vision\\pos.dat");
            }
        }

        public static bool IsCameraOpen = false;
        public static bool SendDataFlg = false;
        public static bool AutoRunFlg = false;
        public static bool ProcessFlg = false;

        public static string ModelName;
        public static string SN;
        public static string UserID;
        public static int MinExposure;
        public static int MaxExposure;
        public static int MinGain;
        public static int MaxGain;
    }
}
