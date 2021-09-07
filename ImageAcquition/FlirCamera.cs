using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlyCapture2Managed;
using System.Drawing;
using System.Drawing.Imaging;
using HalconDotNet;
using System.Toolkit;
using System.Toolkit.Interfaces;
using log4net;
namespace ImageAcquition
{
    public class FlirCamera : IAutomatic
    {
        private ILog log = LogManager.GetLogger(typeof(FlirCamera));
        private static readonly object obj = new object();
        private readonly object objimg = new object();
        public static Dictionary<uint, CameraInfo> listCamera = new Dictionary<uint, CameraInfo>();
        private static ManagedBusManager busMgr;//总线控制类
        private static uint numCameras;
        private ManagedPGRGuid m_Guid;//相机标识类
        private ManagedCameraBase m_Device;//相机类
        private HWindowControl hWindowsControl = null;

        public HObject ho_Image;
        public CameraInfo CameraInfo { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double ImageRotateAngle { get; set; }
        public ImageMirror ImageMirror { get; set; }
        public FlirCamera(double imageRotateAngle = 0.0, ImageMirror imageMirror = ImageMirror.none, HWindowControl hWindowsControl = null)
        {
            this.hWindowsControl = hWindowsControl;
            ImageRotateAngle = imageRotateAngle;
            ImageMirror = imageMirror;
        }
        public static bool SearchAllCamera()
        {
            lock (obj)
            {
                listCamera.Clear();
                try
                {
                    busMgr = new ManagedBusManager();
                    numCameras = busMgr.GetNumOfCameras();
                    if (numCameras == 0)
                    {
                        throw new Exception("Not enough cameras!");
                    }
                    for (uint index = 0; index < numCameras; index++)
                    {
                        ManagedPGRGuid guid = busMgr.GetCameraFromIndex(index);
                        InterfaceType ifType = busMgr.GetInterfaceTypeFromGuid(guid);
                        ManagedCameraBase cam = null;
                        if (ifType == InterfaceType.GigE) cam = new ManagedGigECamera();
                        else cam = new ManagedCamera();
                        cam.Connect(guid);
                        // Get the camera information
                        CameraInfo camInfo = cam.GetCameraInfo();
                        listCamera.Add(camInfo.serialNumber, camInfo);
                        cam.Disconnect();
                    }
                }
                catch (FC2Exception ex)
                {
                    throw new Exception($"ExceptionType:{ex.GetType()}! ErrorDescription:{ex.CauseType} in function:{ex.NativeErrorTrace}");
                }
                return true;
            }
        }
        public bool Initialize(uint CameraSerialNumber)
        {
            if (!listCamera.ContainsKey(CameraSerialNumber))
            {
                log.Error("相机系列号不在字典中！");
                return false;
            }
            if (numCameras > 0)
            {
                try
                {
                    m_Guid = busMgr.GetCameraFromSerialNumber(CameraSerialNumber);//根据序列号获取相机的唯一标识
                    InterfaceType ifType = busMgr.GetInterfaceTypeFromGuid(m_Guid);
                    if (ifType == InterfaceType.GigE) m_Device = new ManagedGigECamera();
                    else m_Device = new ManagedCamera();
                    // Connect to the first selected GUID
                    m_Device.Connect(m_Guid);
                    const uint CameraPower = 0x610;
                    const uint CameraPowerValue = 0x80000000;
                    m_Device.WriteRegister(CameraPower, CameraPowerValue);
                    uint cameraPowerValueRead = 0;
                    var i = 0;
                    // Wait for camera to complete power-up
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                        cameraPowerValueRead = m_Device.ReadRegister(CameraPower);
                        i++;
                        if (i > 100)
                        {
                            log.Error("Wait for camera to complete power-up is fail！");
                            return false;
                        }
                    }
                    while ((cameraPowerValueRead & CameraPowerValue) == 0);
                    CameraInfo = listCamera[CameraSerialNumber];//获取相机相关信息
                    m_Device.StartCapture(OnImageGrabbed);//开启相机采集
                    // Set embedded timestamp to on
                    EmbeddedImageInfo embeddedInfo = m_Device.GetEmbeddedImageInfo();
                    embeddedInfo.timestamp.onOff = true;
                    m_Device.SetEmbeddedImageInfo(embeddedInfo);
                    return true;
                }
                catch (FC2Exception ex)
                {
                    log.Error($"相机初始化中异常：{ex}！");
                    return false;
                }
            }
            else
            {
                log.Error($"总线中相机数量小于等于{numCameras}！");
                return false;
            }
        }
        /// <summary>
        /// 图像获取的回调函数
        /// </summary>
        /// <param name="Image"></param>
        private void OnImageGrabbed(ManagedImage Image)
        {
            ManagedImage m_Image = new ManagedImage();
            HObject image,timage;
            HOperatorSet.GenEmptyObj(out image);
            HOperatorSet.GenEmptyObj(out timage);
            try
            {
                lock (objimg)
                {
                    Image.Convert(FlyCapture2Managed.PixelFormat.PixelFormatMono8, m_Image);
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    ho_Image.Dispose();
                    image.Dispose();
                    timage.Dispose();
                    Bitmap2HObject8bpp(m_Image.bitmap, out image);
                    if (ImageRotateAngle != 0.0)
                        HOperatorSet.RotateImage(image, out timage, ImageRotateAngle, "constant");
                    else timage = image.Clone();
                    if (ImageMirror != ImageMirror.none)
                        HOperatorSet.MirrorImage(timage, out ho_Image, ImageMirror.ToString());
                    else ho_Image = timage.Clone();
                    if (hWindowsControl != null && hWindowsControl.HalconID != IntPtr.Zero)
                    {
                        Task task;
                        HTuple imageW, imageH;
                        HOperatorSet.GetImageSize(this.ho_Image, out imageW, out imageH);
                        task = Task.Run(new Action(() =>
                        {
                            try {
                                if (this.ho_Image != null)
                                {
                                    HOperatorSet.SetPart(hWindowsControl.HalconWindow, 0, 0, imageH, imageW);
                                    HOperatorSet.DispObj(this.ho_Image, this.hWindowsControl.HalconWindow);
                                }
                            }
                            catch(HalconException ex) { }
                        }));
                        task.Wait();
                    }
                }
            }
            catch (FC2Exception ex)
            {
                log.Error($"相机回调获取图像时异常：{ex}！");
                return;
            }
            finally
            {
                image.Dispose();
                timage.Dispose();
                m_Image.ReleaseBuffer();
            }
        }
        /// <summary>
        /// bitmap图像转换成HObject图像
        /// </summary>
        /// <param name="srcImage">输入图像</param>
        /// <param name="image">输出图像</param>
        private void Bitmap2HObject8bpp(Bitmap srcImage, out HObject image)
        {
            image = null;
            try
            {
                Point po = new Point(0, 0);
                Size so = new Size(srcImage.Width, srcImage.Height);
                Rectangle ro = new Rectangle(po, so);

                Bitmap dstImg = new Bitmap(srcImage.Width, srcImage.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                dstImg = srcImage.Clone(ro, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                Rectangle rect = new Rectangle(0, 0, dstImg.Width, dstImg.Height);
                BitmapData srcBmpData = dstImg.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                HOperatorSet.GenImage1(out image, "byte", dstImg.Width, dstImg.Height, srcBmpData.Scan0);
                dstImg.UnlockBits(srcBmpData);
            }
            catch (Exception ex)
            {
                log.Error($"Bitmap图像转换成HObject图像时异常：{ex}！");
                image = null;
            }
        }
        /// <summary>
        /// 获取相机曝光时间
        /// </summary>
        /// <param name="exposure"></param>
        /// <returns></returns>
        public bool GetExposureTime(ref double exposure)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                CameraProperty PropertyShutter;
                PropertyShutter = m_Device.GetProperty(PropertyType.Shutter);
                exposure = PropertyShutter.absValue;
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"获取{Name}相机曝光时间时异常：{ex}！");
                return false;
            }
        }
        public bool GetExposureTimeRange(ref double min, ref double max)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                CameraProperty PropertyShutter;
                PropertyShutter = m_Device.GetProperty(PropertyType.Shutter);
                min = PropertyShutter.valueA;
                max = PropertyShutter.valueB;
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"获取{Name}相机曝光范围时异常：{ex}！");
                return false;
            }
        }
        /// <summary>
        /// 获取增益值
        /// </summary>
        /// <param name="gain">增益</param>
        /// <returns></returns>
        public bool GetGain(ref double gain)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                CameraProperty PropertyGain;
                PropertyGain = m_Device.GetProperty(PropertyType.Gain);
                gain = PropertyGain.absValue;
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"获取{Name}相机增益时异常：{ex}！");
                return false;
            }
        }
        /// <summary>
        /// 获取增益范围
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public bool GetGainRange(ref double min, ref double max)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                CameraProperty PropertyGain;
                PropertyGain = m_Device.GetProperty(PropertyType.Gain);
                min = PropertyGain.valueA;
                max = PropertyGain.valueB;
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"获取{Name}相机增益范围时异常：{ex}！");
                return false;
            }
        }
        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="exposure"></param>
        /// <returns></returns>
        public bool SetExposureTime(double exposure)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                if (exposure <= 0.0)
                {
                    return false;
                }
                CameraProperty PropertyShutter;
                PropertyShutter = m_Device.GetProperty(PropertyType.Shutter);
                PropertyShutter.absControl = true;
                PropertyShutter.onOff = true;
                PropertyShutter.autoManualMode = false;//关闭自动曝光
                PropertyShutter.absValue = (float)exposure;
                m_Device.SetProperty(PropertyShutter);
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"设置{Name}相机曝光时间异常：{ex}！");
                return false;
            }
        }
        /// <summary>
        /// 设置相机增益
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetGain(double value)
        {
            if (this.m_Device == null)
            {
                return false;
            }
            try
            {
                if (value <= 0.0) return false;
                CameraProperty PropertyGain;
                PropertyGain = m_Device.GetProperty(PropertyType.Gain);
                PropertyGain.absControl = true;
                PropertyGain.onOff = true;
                PropertyGain.autoManualMode = false;//关闭自动增益
                PropertyGain.absValue = (float)value;
                m_Device.SetProperty(PropertyGain);
                return true;
            }
            catch (FC2Exception ex)
            {
                log.Error($"设置{Name}相机增益异常：{ex}！");
                return false;
            }
        }

        public void SaveToMemoryChannel()
        {
            if (this.m_Device == null) return;
            try
            {               
                m_Device.SaveToMemoryChannel(1);
            }
            catch (FC2Exception ex)
            {
                log.Error($"保存{Name}相机Memory：{ex}！");
                return;
            }
        }
        /// <summary>
        /// 实时触发
        /// </summary>
        /// <param name="value"></param>
        public void SetTriggerModeOn(bool value)
        {
            if (this.m_Device == null) return;
            try
            {
                TriggerMode triggerMode = m_Device.GetTriggerMode();
                triggerMode.onOff = value;//打开触发模式
                m_Device.SetTriggerMode(triggerMode);
            }
            catch (FC2Exception ex)
            {
                log.Error($"开启/关闭{Name}相机触发模式异常：{ex}！");
                return;
            }
        }
        /// <summary>
        /// 停止实时拍照
        /// </summary>
        public void SetTriggerStop()
        {
            if (this.m_Device == null) return;
            try
            {
                TriggerMode triggerMode = m_Device.GetTriggerMode();
                triggerMode.onOff = true;//打开触发模式
                m_Device.SetTriggerMode(triggerMode);
            }
            catch (FC2Exception ex)
            {
                log.Error($"开启{Name}相机触发模式异常：{ex}！");
                return;
            }
        }
        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="value">false为软触发，true为硬触发</param>
        public void SetTriggerSource(bool value = false)
        {
            if (this.m_Device == null) return;
            try
            {
                TriggerMode triggerMode = m_Device.GetTriggerMode();
                triggerMode.onOff = true;//打开触发模式
                triggerMode.mode = 0;//设置触发模式为0模式
                triggerMode.parameter = 0;
                triggerMode.source = (uint)(value ? 0 : 7); //7为软触发，0为pin脚0作为触发端口的外触发
                m_Device.SetTriggerMode(triggerMode);
            }
            catch (FC2Exception ex)
            {
                log.Error($"设置{Name}相机触发模式异常：{ex}！");
                return;
            }
        }
        /// <summary>
        /// 软触发拍照
        /// </summary>
        public void SoftTriggerOnce()
        {
            if (this.m_Device == null) return;
            try
            {
                TriggerMode triggerMode = m_Device.GetTriggerMode();
                triggerMode.onOff = true;//打开触发模式
                m_Device.SetTriggerMode(triggerMode);
                const uint SoftwareTrigger = 0x62C;
                const uint SoftwareTriggerFireValue = 0x80000000;

                m_Device.WriteRegister(SoftwareTrigger, SoftwareTriggerFireValue);
            }
            catch (FC2Exception ex)
            {
                log.Error($"{Name}相机软触发异常：{ex}！");
                return;
            }
        }
        /// <summary>
        /// 相机资源释放
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (m_Device != null)
                {
                    // Stop capturing images
                    m_Device?.StopCapture();

                    // Turn off trigger mode
                    TriggerMode triggerMode = m_Device?.GetTriggerMode();
                    triggerMode.onOff = false;//打开触发模式
                    m_Device?.SetTriggerMode(triggerMode);

                    // Disconnect the camera
                    m_Device?.Disconnect();
                }
            }
            catch (FC2Exception ex)
            {
                log.Error($"{Name}相机资源释放异常：{ex}！");
                return;
            }
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
