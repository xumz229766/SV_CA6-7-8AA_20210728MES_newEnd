using System;
using System.IO;

namespace desay.ProductData
{
    public class AppConfig
    {
        static string path = Path.Combine(Config.Instance.ImageSAvePath +"\\"+ DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd"));
        public static string VisionName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\{Config.Instance.CurrentProductType}.xml");
            }
        }
        public static string ModelName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Vision\\ModelParam.xml");
            }
        }
        public static string VisionPicturePass
        {

            get
            {
              
                try {
                    if (Directory.Exists(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Pass\\")) ==false)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Pass\\"));
                    }
                    return Path.Combine(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Pass\\"));
                }
                catch { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Pass\\"); }
                
                //return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Pass\\");
            }
        }
        public static string VisionPictureFail
        {
            get
            {
                try
                {
                    if (Directory.Exists(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Fail\\")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Fail\\"));
                    }
                    return Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Location\\Fail\\");
                }
                catch { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Fail\\"); }
               
            }
        }
        public static string VisionPicturePass_GlueFind
        {

            get
            {

                try
                {
                    if (Directory.Exists(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Pass\\")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Pass\\"));
                    }
                    return Path.Combine(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Pass\\"));
                }
                catch { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Pass\\"); }

                //return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Pass\\");
            }
        }
        public static string VisionPictureFail_GlueFind
        {
            get
            {
                try
                {
                    if (Directory.Exists(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Fail\\")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Fail\\"));
                    }
                    return Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Glue\\Fail\\");
                }
                catch { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Glue\\Fail\\"); }

            }
        }
        public static string VisionPictureWindow
        {
            get
            {
                try
                {
                    if (Directory.Exists(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Window\\")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Window\\"));
                    }
                    return Path.Combine(Path.Combine(Config.Instance.ImageSAvePath + "\\" + DateTime.Now.ToString("yyyy_MM") + "\\" + DateTime.Now.ToString("MM_dd")) + "\\Window\\");
                }
                catch { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VisionPicture\\Fail\\"); }

            }
        }
        public static string ConfigFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\Config.xml");
            }
        }
        public static string ConfigCameraName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\Camera.xml");
            }
        }
        public static string ConfigGlueFindName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlueFindParam.xml");
            }
        }
        public static string ConfigIniCard0file
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Param\\paramCard0.xml");
            }
        }
        public static string ConfigIniCard1file
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Param\\paramCard1.xml");
            }
        }
        public static string ConfigEniCardfile
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Param\\param.eni");
            }
        }
        public static string ConfigTrayName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\Tray.ini");
            }
        }
        public static string ConfigAxisName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\AxisParam.xml");
            }
        }
        public static string ConfigDelayName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\CylinderDelay.xml");
            }
        }
        public static string ConfigPositionName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data\\{Config.Instance.CurrentProductType}.xml");
            }
        }
        public static string LogFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            }
        }
        /// <summary>
        /// 生产信息文件路径
        /// </summary>
        public static string ProdutionInfoFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProdutionInfo.ini");
            }
        }
        /// <summary>
        /// MES配置参数文件路径
        /// </summary>
        public static string MESConfigFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MESConfig.ini");
            }
        }
        public static string MesShareFileFolderName
        {
            get
            {
                return "D:\\files\\";
            }
        }
        public static string MesDataPassFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MesData\\Pass\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            }
        }
        public static string MesDataFailFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MesData\\Fail\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            }
        }
        /// <summary>
        /// 本地数据库文件夹路径
        /// </summary>
        public static string dataBaseDirectoryPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase");
            }
        }
        /// <summary>
        /// 本地数据库文件路径
        /// </summary>
        public static string dataBaseFileName
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.db3", DateTime.Now.ToString("yyyyMMdd")));
            }
        }
    }
}
