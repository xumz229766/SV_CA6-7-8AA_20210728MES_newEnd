using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace desay.Vision
{
    public class LightControl
    {

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_EthernetConnectIP(string IP, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_EthernetConnectStop(ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetGetDigitalValue(int CH, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetLightState(int CH, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetStrobeValue(int CH, int Light, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetDigitalValue(int CH, int Light, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_CreateSerialPort(int ComNum, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_ReleaseSerialPort(ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_SerialPortSetDigitalValue(int CH, int Light, ref Int64 Handle);

        public struct MulDigitalValue
        {
            public int channelIndex;
            public int DigitalValue;
        }

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_CST_GetAdapter(IntPtr mAdapterPrmPtr);
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Adapter_prm
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
            public char[] cSn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] cIp;
        }


        Int64 mHandle = 0;

        public string LightControlIP = "192.168.1.118";
        /// <summary>
        /// 网口链接
        /// </summary>
        /// <returns></returns>
        public bool OpenLightControl()
        {
            if( CST_EthernetConnectIP(LightControlIP, ref mHandle)== 10000) return true;
            else return false; //IP连接 
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public bool CloseLightControl()
        {
            if (CST_EthernetConnectStop(ref mHandle) == 10000) return true;
            else return false; //关闭 
        }

        public bool SetDigitalValue(int ChanelNum,int Value)
        {
            if (CST_EthernetSetDigitalValue(ChanelNum, Value, ref mHandle) == 10000) return true;
            else return false; //设置1通道数字亮度值
        }

        public string[] SnBuffer;
        public string[] IpBuffer;
        public int AdapterNum;
        /// <summary>
        /// 遍历网卡
        /// </summary>
        public void SerchAllIP()
        {

            int workStationCount = 10;
            int size = Marshal.SizeOf(typeof(Adapter_prm));
            IntPtr mAdapterPrmPtr = Marshal.AllocHGlobal(size * workStationCount);
            Adapter_prm[] mAdapter_prm = new Adapter_prm[workStationCount];
            int AdapterNum = CST_CST_GetAdapter(mAdapterPrmPtr);
            for (int inkIndex = 0; inkIndex < workStationCount; inkIndex++)
            {
                IntPtr ptr = (IntPtr)(mAdapterPrmPtr + inkIndex * size);
                mAdapter_prm[inkIndex] = (Adapter_prm)Marshal.PtrToStructure(ptr, typeof(Adapter_prm));
            }

            SnBuffer = new string[AdapterNum];
            for (int i = 0; i < AdapterNum; i++)
            {
                SnBuffer[i] = new string(mAdapter_prm[i].cSn);
                
            }
            IpBuffer = new string[AdapterNum];
            for (int i = 0; i < AdapterNum; i++)
            {
                IpBuffer[i] = new string(mAdapter_prm[i].cIp);
               
            }
           
        }

    }
}
