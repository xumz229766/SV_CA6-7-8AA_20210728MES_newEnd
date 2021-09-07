using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using HalconDotNet;
using System.Toolkit;
using System.Toolkit.Interfaces;
using log4net;
using ThridLibray;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace ImageAcquition
{
    public class OPTCamera : IAutomatic
    {
        private ILog log = LogManager.GetLogger(typeof(OPTCamera));
        private static readonly object obj = new object();
        private readonly object objimg = new object();
      
       
        private static uint numCameras;
       
        private HWindowControl hWindowsControl = null;

        public HObject ho_Image;
      

        public string Name { get; set; }
        public string Description { get; set; }

        List<IGrabbedRawData> m_frameList = new List<IGrabbedRawData>();        /* 图像缓存列表 */
        Thread renderThread = null;         /* 显示线程  */
        public bool m_bShowLoop = true;            /* 线程控制变量 */       /// <summary>
        /// 控制 实时 显示
        /// </summary>
        Mutex m_mutex = new Mutex();        /* 锁，保证多线程安全 */
        private Graphics _g = null;

        const int DEFAULT_INTERVAL = 40;
        Stopwatch m_stopWatch = new Stopwatch();

  
        /* 设备对象 */
        /* device object */
        private IDevice m_dev;


        public int ExposureTime = 10000;
        public double Gain = 1.0;

        public bool bOpenCam = false;
        public bool bCloseCam = true;
        public OPTCamera( HWindowControl hWindowsControl = null)
        {
            this.hWindowsControl = hWindowsControl;
            FormShown();
        }
        private void FormShown()
        {
            /* 初始化设备关闭按钮 */
            

            if (null == renderThread)
            {
                renderThread = new Thread(new ThreadStart(ShowThread));
                renderThread.Start();
            }
            m_stopWatch.Start();
        }

        /* 转码显示线程 */
        private void ShowThread()
        {
            while (m_bShowLoop)
            {
                if (m_frameList.Count == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                /* 图像队列取最新帧 */
                m_mutex.WaitOne();
                IGrabbedRawData frame = m_frameList.ElementAt(m_frameList.Count - 1);
                m_frameList.Clear();
                m_mutex.ReleaseMutex();

                /* 主动调用回收垃圾 */
                GC.Collect();

                /* 控制显示最高帧率为25FPS */
                if (false == isTimeToDisplay())
                {
                    continue;
                }

                try
                {
                    /* 图像转码成bitmap图像 */
                    var bitmap = frame.ToBitmap(false);
                    HTuple imageW, imageH;

                    HObject Image;
                    Bitmap2HObj(bitmap, out Image);
                    //HOperatorSet.GetImageSize(Image, out imageW, out imageH);
                    //HOperatorSet.SetPart(hWindowsControl.HalconWindow, 0, 0, imageH, imageW);
                    //HOperatorSet.DispObj(Image, hWindowsControl.HalconWindow);

                    HOperatorSet.GenEmptyObj(out ho_Image);
                    ho_Image.Dispose();

                    ho_Image = Image.Clone();
                    if (hWindowsControl != null && hWindowsControl.HalconID != IntPtr.Zero)
                    {
                        Task task;
                        //HTuple imageW, imageH;
                        HOperatorSet.GetImageSize(this.ho_Image, out imageW, out imageH);
                        task = Task.Run(new Action(() =>
                        {
                            try
                            {
                                if (this.ho_Image != null)
                                {
                                    HOperatorSet.SetPart(hWindowsControl.HalconWindow, 0, 0, imageH, imageW);
                                    HOperatorSet.DispObj(this.ho_Image, this.hWindowsControl.HalconWindow);
                                }
                            }
                            catch (HalconException ex) { }
                        }));
                        task.Wait();
                    }

                    //Image.Dispose();
                }
                catch (Exception exception)
                {
                    Catcher.Show(exception);
                }
            }
        }

        public void Bitmap2HObj(Bitmap bmp, out HObject ho_src_image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out ho_src_image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception ex)
            {
                ho_src_image = null;
                //System.Windows.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        /* 判断是否应该做显示操作 */
        private bool isTimeToDisplay()
        {
            m_stopWatch.Stop();
            long m_lDisplayInterval = m_stopWatch.ElapsedMilliseconds;
            if (m_lDisplayInterval <= DEFAULT_INTERVAL)
            {
                m_stopWatch.Start();
                return false;
            }
            else
            {
                m_stopWatch.Reset();
                m_stopWatch.Start();
                return true;
            }
        }

      
        /* 相机打开回调 */
        private void OnCameraOpen(object sender, EventArgs e)
        {
            SoftTriggerOnce();
            bOpenCam = true;
            bCloseCam = false;
        }

        /* 相机关闭回调 */
        private void OnCameraClose(object sender, EventArgs e)
        {
            bOpenCam = false;
            bCloseCam = true;
        }

        /* 相机丢失回调 */
        private void OnConnectLoss(object sender, EventArgs e)
        {
            m_dev.ShutdownGrab();
            m_dev.Dispose();
            m_dev = null;
            bOpenCam = false;
            bCloseCam = true;

        }

      

        public bool Initialize()
        {
            try
            {
                /* 设备搜索 */
                List<IDeviceInfo> li = Enumerator.EnumerateDevices();
                if (li.Count > 0)
                {
                    /* 获取搜索到的第一个设备 */
                    m_dev = Enumerator.GetDeviceByIndex(0);

                    /* 注册链接事件 */
                    m_dev.CameraOpened += OnCameraOpen;
                    m_dev.ConnectionLost += OnConnectLoss;
                    m_dev.CameraClosed += OnCameraClose;

                    /* 打开设备 */
                    if (!m_dev.Open())
                    {
                        MessageBox.Show(@"连接相机失败");
                        return false;
                    }

                    /* 打开Software Trigger */
                    m_dev.TriggerSet.Open(TriggerSourceEnum.Software);

                    /* 设置图像格式 */
                    using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ImagePixelFormat])
                    {
                        p.SetValue("Mono8");
                    }

                    /* 设置曝光 */
                    using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
                    {
                        p.SetValue(ExposureTime);
                    }

                    /* 设置增益 */
                    using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                    {
                        p.SetValue(1.0);
                    }

                    /* 设置缓存个数为8（默认值为16） */
                    m_dev.StreamGrabber.SetBufferCount(8);

                    /* 注册码流回调事件 */
                    m_dev.StreamGrabber.ImageGrabbed += OnImageGrabbed;

                    /* 开启码流 */
                    if (!m_dev.GrabUsingGrabLoopThread())
                    {
                        MessageBox.Show(@"开启码流失败");
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {

                //Catcher.Show(exception);
                MessageBox.Show(exception.ToString());
                return false;
            }
            return true;
        }

        /* 码流数据回调 */
        private void OnImageGrabbed(Object sender, GrabbedEventArgs e)
        {
            m_mutex.WaitOne();
            m_frameList.Add(e.GrabResult.Clone());
            m_mutex.ReleaseMutex();
        }

        /* 停止码流 */
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_dev == null)
                {
                    throw new InvalidOperationException("Device is invalid");
                }

                m_dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed;         /* 反注册回调 */
                m_dev.ShutdownGrab();                                       /* 停止码流 */
                m_dev.Close();                                              /* 关闭相机 */
            }
            catch (Exception exception)
            {
                Catcher.Show(exception);
            }
        }


        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="exposure"></param>
        /// <returns></returns>
        public bool SetExposureTime(double exposure)
        {

            /* 设置曝光 */
            using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
            {
                p.SetValue(exposure);
            }
            return true;
        }


        /// <summary>
        /// 软触发拍照
        /// </summary>
        public void SoftTriggerOnce()
        {
            if (m_dev == null)
            {
                throw new InvalidOperationException("Device is invalid");
            }

            try
            {
                m_dev.ExecuteSoftwareTrigger();
                //GetImgToHalcon();//获取图片
            }
            catch (Exception exception)
            {
                Catcher.Show(exception);
            }
        }

        public void CloseCam()
        {
            try
            {
                if (m_dev == null)
                {
                    throw new InvalidOperationException("Device is invalid");
                }

                m_dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed;         /* 反注册回调 | unregister grab event callback */
                m_dev.ShutdownGrab();                                       /* 停止码流 | stop grabbing */
                m_dev.Close();                                              /* 关闭相机 | close camera */
            }
            catch (Exception exception)
            {
                //Catcher.Show(exception);
                MessageBox.Show(exception.ToString());
            }
        }
        /// <summary>
        /// 相机资源释放
        /// </summary>
        public void Dispose()
        {
            if (m_dev != null)
            {
                m_dev.Dispose();
                m_dev = null;
            }

            m_bShowLoop = false;
            renderThread.Join();
            if (_g != null)
            {
                _g.Dispose();
                _g = null;
            }
            //base.OnClosed(e);
        }
        public void UpHWindowControl(HWindowControl hWindowsControl)
        {
            this.hWindowsControl = hWindowsControl;
        }
        public void CloseHWindowControl()
        {
            if (hWindowsControl != null || hWindowsControl.HalconID != IntPtr.Zero)
            {
                hWindowsControl.Dispose();
                hWindowsControl = null;
            }
        }
    }
}
