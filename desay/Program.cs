using System;
using System.Windows.Forms;
using log4net;
using System.Toolkit;
using System.Toolkit.Helpers;
using System.Threading;
using desay.ProductData;
using desay.Vision;
namespace desay
{
    static class Program
    {

        static ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitialPalte();
            bool isRunning;
            Mutex mutex = new Mutex(true, "RunOneInstanceOnly", out isRunning);

            if (isRunning)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += new ThreadExceptionEventHandler(UI_ThreadException);//处理UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);//处理非UI线程异常

                //加载配置文件
                //Config.Instance = SerializerManager<Config>.Instance.Load(AppConfig.ConfigFileName);
                //AxisParameter.Instance= SerializerManager<AxisParameter>.Instance.Load(AppConfig.ConfigAxisName);
                //Thread.Sleep(200);
                //Position.Instance = SerializerManager<Position>.Instance.Load(AppConfig.ConfigPositionName);
                //Delay.Instance = SerializerManager<Delay>.Instance.Load(AppConfig.ConfigDelayName);
                //Relationship.Instance = SerializerManager<Relationship>.Instance.Load(AppConfig.ConfigCameraName);
                //AxisParameter.Instance = SerializerManager<AxisParameter>.Instance.Load(AppConfig.ConfigAxisName);
                //DbModelParam.Instance = SerializerManager<DbModelParam>.Instance.Load(VisionMarking.VisionName);
                //VisionProductData.Instance = SerializerManager<VisionProductData>.Instance.Load(VisionMarking.VisionFileName);
                //GlueFindParam.Instance = SerializerManager<GlueFindParam>.Instance.Load(AppConfig.ConfigGlueFindName);
                //加载配置文件
                try
                {
                    Config.Instance = SerializerManager<Config>.Instance.Load(AppConfig.ConfigFileName);
                }
                catch { MessageBox.Show($"配置文件Config出错{AppConfig.ConfigFileName}"); return; }
                try
                {
                    AxisParameter.Instance = SerializerManager<AxisParameter>.Instance.Load(AppConfig.ConfigAxisName);
                }
                catch { MessageBox.Show($"配置文件AxisParameter出错{AppConfig.ConfigAxisName}"); return; }
                Thread.Sleep(200);
                try
                {
                    Position.Instance = SerializerManager<Position>.Instance.Load(AppConfig.ConfigPositionName);
                }
                catch { MessageBox.Show($"配置文件Position出错{AppConfig.ConfigPositionName}"); return; }
                try
                {
                    Delay.Instance = SerializerManager<Delay>.Instance.Load(AppConfig.ConfigDelayName);
                }
                catch { MessageBox.Show($"配置文件Delay出错{AppConfig.ConfigDelayName}"); return; }
                try
                {
                    Relationship.Instance = SerializerManager<Relationship>.Instance.Load(AppConfig.ConfigCameraName);

                }
                catch { MessageBox.Show($"配置文件Relationship出错{AppConfig.ConfigCameraName}"); return; }
                try
                {
                    DbModelParam.Instance = SerializerManager<DbModelParam>.Instance.Load(VisionMarking.VisionName);
                }
                catch { MessageBox.Show($"配置文件DbModelParam出错{VisionMarking.VisionName}"); return; }

                try
                {
                    VisionProductData.Instance = SerializerManager<VisionProductData>.Instance.Load(VisionMarking.VisionFileName);
                }
                catch { MessageBox.Show($"配置文件VisionProductData出错{VisionMarking.VisionFileName}"); return; }

                try
                {
                    GlueFindParam.Instance = SerializerManager<GlueFindParam>.Instance.Load(AppConfig.ConfigGlueFindName);

                }
                catch { MessageBox.Show($"配置文件GlueFindParam出错{AppConfig.ConfigGlueFindName}"); return; }
                Application.Run(new frmMain());
                
            }
            else
            {
                MessageBox.Show("程序已经启动！");
            }
        }
        #region jj
        static string aaPlateRecord;
        public static void InitialPalte()
        {
            aaPlateRecord = "";
            for (int i = 0; i < 100; i++)
            {
                aaPlateRecord += i.ToString() + " ";
            }
            string[] emptys = aaPlateRecord.Split(' ');
            Console.WriteLine();
        }
        #endregion
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        static void UI_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Global.isErrorExit = true;
           // SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName,Config.Instance);

            //Thread.Sleep(200);
            //SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
            //SerializerManager<DbModelParam>.Instance.Save(AppConfig.ConfigPositionName, DbModelParam.Instance);
            log.Fatal(e.Exception.Message);
            MessageBox.Show(e.Exception.Message);
            Application.Exit();
        }
        /// <summary>
        /// 处理非UI线程异常
        /// </summary>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Global.isErrorExit = true;
            //SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);

            //Thread.Sleep(200);
            //SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
            //SerializerManager<DbModelParam>.Instance.Save(AppConfig.ConfigPositionName, DbModelParam.Instance);
            log.Fatal(e.ExceptionObject.ToString());
            MessageBox.Show(e.ExceptionObject.ToString());
            Application.Exit();
        }
    }
}
