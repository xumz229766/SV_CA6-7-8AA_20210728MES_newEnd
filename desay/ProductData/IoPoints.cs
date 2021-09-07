using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motion.Interfaces;
using Motion.AdlinkAps;
using Motion.AdlinkDash;
namespace desay.ProductData
{
    /// <summary>
    ///     设备 IO 项
    /// </summary>
    public class IoPoints
    {
        private const string ApsControllerName = "ApsController";
        private const string daskControllerName = "daskController";
        internal static readonly int[] Card208C = new int[2]{0,1};
        internal static readonly byte PCI7442 = 0;
        public static ApsController m_ApsController = new ApsController(Card208C) { Name = ApsControllerName };
        public static DaskController m_DaskController = new DaskController(PCI7442) { Name = daskControllerName,typeName=DASK.PCI_7442 };
        #region Card208_0 IO list
        //由于IO表改动较多，IO命名规则重新整理：Name 根据IO表地址
        //0-7 

        /// <summary>
        ///  清洗入料感应
        /// </summary>
        public static IoPoint TDI0 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 0, IoModes.Senser)
        {
            Name = "DI5.0",
            Description = "清洗入料感应"
        };
        /// <summary>
        ///  点胶小顶升气缸原位
        /// </summary>
        public static IoPoint TDI1 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 1, IoModes.Senser)
        {
            Name = "DI5.1",
            Description = "点胶探针顶升气缸原位"
        };
        /// <summary>
        ///  点胶小顶升气缸到位
        /// </summary>
        public static IoPoint TDI2 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 2, IoModes.Senser)
        {
            Name = "DI5.2",
            Description = "点胶探针顶升气缸到位"
        };
        /// <summary>
        ///  点胶X回原完成
        /// </summary>
        public static IoPoint TDI8 = new IoPoint(m_ApsController, Card208C[1],0, 8+8, IoModes.Senser)
        {
            Name = "DI5.8",
            Description = "点胶X回原完成"
        };
        /// <summary>
        ///   点胶Y回原完成
        /// </summary>
        public static IoPoint TDI9 = new IoPoint(m_ApsController, Card208C[1],0, 8+9, IoModes.Senser)
        {
            Name = "DI5.9",
            Description = "点胶Y回原完成"
        };
        /// <summary>
        ///   点胶Z回原完成
        /// </summary>
        public static IoPoint TDI10 = new IoPoint(m_ApsController, Card208C[1],0, 8+10, IoModes.Senser)
        {
            Name = "DI5.10",
            Description = "点胶z回原完成"
        };
        /// <summary>
        ///   清洗X回原完成
        /// </summary>
        public static IoPoint TDI11 = new IoPoint(m_ApsController, Card208C[1],0, 8+11, IoModes.Senser)
        {
            Name = "DI5.11",
            Description = "清洗X回原完成"
        };
        /// <summary>
        ///   清洗Y回原完成
        /// </summary>
        public static IoPoint TDI12 = new IoPoint(m_ApsController, Card208C[1],0, 8+12, IoModes.Senser)
        {
            Name = "DI5.12",
            Description = "清洗Y回原完成"
        };
        /// <summary>
        ///   清洗Z回原完成
        /// </summary>
        public static IoPoint TDI13 = new IoPoint(m_ApsController, Card208C[1],0, 8+13, IoModes.Senser)
        {
            Name = "DI5.13",
            Description = "清洗Z回原完成"
        };
        /// <summary>
        ///  点胶X回原
        /// </summary>
        public static IoPoint TDO0 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 0, IoModes.Signal)
        {
            Name = "DO5.0",
            Description = "点胶X回原"
        };
        /// <summary>
        ///   点胶Y回原
        /// </summary>
        public static IoPoint TDO1 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 1, IoModes.Signal)
        {
            Name = "DO5.1",
            Description = "点胶Y回原"
        };
        /// <summary>
        ///   点胶Z回原
        /// </summary>
        public static IoPoint TDO2 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 2, IoModes.Signal)
        {
            Name = "DO5.2",
            Description = "点胶z回原"
        };
        /// <summary>
        ///   清洗X回原
        /// </summary>
        public static IoPoint TDO3 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 3, IoModes.Signal)
        {
            Name = "DO5.3",
            Description = "清洗X回原"
        };
        /// <summary>
        ///   清洗Y回原
        /// </summary>
        public static IoPoint TDO4 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 4, IoModes.Signal)
        {
            Name = "DO5.4",
            Description = "清洗Y回原"
        };

        /// <summary>
        ///   清洗z回原
        /// </summary>
        public static IoPoint TDO5 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 5, IoModes.Signal)
        {
            Name = "DO5.5",
            Description = "清洗z回原"
        };
        /// <summary>
        ///   点胶探针顶升气缸缩回
        /// </summary>
        public static IoPoint TDO6 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 6, IoModes.Signal)
        {
            Name = "DO5.6",
            Description = "点胶探针顶升气缸缩回"
        };
        /// <summary>
        ///   点胶探针顶升气缸伸出
        /// </summary>
        public static IoPoint TDO7 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 7, IoModes.Signal)
        {
            Name = "DO5.7",
            Description = "点胶探针顶升气缸伸出"
        };
        /// <summary>
        ///   PLC心跳信号
        /// </summary>
        public static IoPoint TDO15 = new IoPoint(m_ApsController, Card208C[1], 0, 8 + 15, IoModes.Signal)
        {
            Name = "DO5.15",
            Description = "PLC心跳信号"
        };
        /// <summary>
        ///  清除报警
        /// </summary>
        public static IoPoint AlarmDO0 = new IoPoint(m_ApsController, Card208C[1], 0,  0, IoModes.Signal)
        {
            Name = "AlarmDO0 ",
            Description = "清除报警 "
        };
        /// <summary>
        ///  清除报警 
        /// </summary>
        public static IoPoint AlarmDO1 = new IoPoint(m_ApsController, Card208C[1], 0, 1, IoModes.Signal)
        {
            Name = " AlarmDO1",
            Description = "清除报警 "
        };
        /// <summary>
        ///   清除报警 
        /// </summary>
        public static IoPoint AlarmDO2 = new IoPoint(m_ApsController, Card208C[1], 0, 2, IoModes.Signal)
        {
            Name = "DO5.2",
            Description = "清除报警"
        };
        /// <summary>
        ///   清除报警
        /// </summary>
        public static IoPoint AlarmDO3 = new IoPoint(m_ApsController, Card208C[1], 0, 3, IoModes.Signal)
        {
            Name = "DO5.3",
            Description = "清除报警"
        };
        /// <summary>
        ///   清除报警
        /// </summary>
        public static IoPoint AlarmDO4 = new IoPoint(m_ApsController, Card208C[1], 0, 4, IoModes.Signal)
        {
            Name = "DO5.4",
            Description = "清除报警"
        };
        /// <summary>
        ///   清除报警
        /// </summary>
        public static IoPoint AlarmDO5 = new IoPoint(m_ApsController, Card208C[1], 0, 5, IoModes.Signal)
        {
            Name = "DO5.5",
            Description = "清除报警"
        };
        /// <summary>
        ///   清除报警
        /// </summary>
        public static IoPoint AlarmDO6 = new IoPoint(m_ApsController, Card208C[1], 0, 6, IoModes.Signal)
        {
            Name = "DO5.4",
            Description = "清除报警"
        };
        /// <summary>
        ///   清除报警
        /// </summary>
        public static IoPoint AlarmDO7 = new IoPoint(m_ApsController, Card208C[1], 0, 7, IoModes.Signal)
        {
            Name = "DO5.5",
            Description = "清除报警"
        };
        #endregion
        #region PCI7442 IO list

        /// <summary>
        ///   送料线plasma段到位信号
        /// </summary>
        public static IoPoint IDI0 = new IoPoint(m_DaskController, PCI7442,0, 0, IoModes.Senser)
        {
            Name = "DI6.0",
            Description = "送料线plasma段到位信号"
        };

        /// <summary>
        ///   plasma夹具顶升气缸上感应
        /// </summary>
        public static IoPoint IDI1 = new IoPoint(m_DaskController, PCI7442,0, 1, IoModes.Senser)
        {
            Name = "DI6.1",
            Description = "plasma夹具顶升气缸上感应"
        };

        /// <summary>
        ///   plasma夹具顶升气缸下感应
        /// </summary>
        public static IoPoint IDI2 = new IoPoint(m_DaskController, PCI7442,0, 2, IoModes.Senser)
        {
            Name = "DI6.2",
            Description = "plasma夹具顶升气缸下感应"
        };

        /// <summary>
        ///   备用勿接线
        /// </summary>
        public static IoPoint IDI3 = new IoPoint(m_DaskController, PCI7442,0, 3, IoModes.Senser)
        {
            Name = "DI6.3",
            Description = "备用勿接线"
        };

        /// <summary>
        ///   plasma夹持气缸OFF感应
        /// </summary>
        public static IoPoint IDI4 = new IoPoint(m_DaskController, PCI7442,0, 4, IoModes.Senser)
        {
            Name = "DI6.4",
            Description = "plasma夹持气缸OFF感应"
        };

        /// <summary>
        ///   plasma夹持气缸ON感应
        /// </summary>
        public static IoPoint IDI5 = new IoPoint(m_DaskController, PCI7442,0, 5, IoModes.Senser)
        {
            Name = "DI6.5",
            Description = "plasma夹持气缸ON感应"
        };

        /// <summary>
        ///   plasma翻转气缸OFF感应
        /// </summary>
        public static IoPoint IDI6 = new IoPoint(m_DaskController, PCI7442,0, 6, IoModes.Senser)
        {
            Name = "DI6.6",
            Description = "plasma翻转气缸OFF感应"
        };

        /// <summary>
        ///   plasma翻转气缸ON感应
        /// </summary>
        public static IoPoint IDI7 = new IoPoint(m_DaskController, PCI7442,0, 7, IoModes.Senser)
        {
            Name = "DI6.7",
            Description = "plasma翻转气缸ON感应"
        };

        /// <summary>
        ///   plasma上下气缸上感应
        /// </summary>
        public static IoPoint IDI8 = new IoPoint(m_DaskController, PCI7442,0, 8, IoModes.Senser)
        {
            Name = "DI6.8",
            Description = "plasma上下气缸上感应"
        };

        /// <summary>
        ///   plasma上下气缸下感应
        /// </summary>
        public static IoPoint IDI9 = new IoPoint(m_DaskController, PCI7442,0, 9, IoModes.Senser)
        {
            Name = "DI6.9",
            Description = "plasma上下气缸下感应"
        };

        /// <summary>
        /// 送料线点胶段到位感应
        /// </summary>
        public static IoPoint IDI10 = new IoPoint(m_DaskController, PCI7442,0, 10, IoModes.Senser)
        {
            Name = "DI6.10",
            Description = "送料线点胶段到位感应"
        };

        /// <summary>
        ///   胶水液位感应信号
        /// </summary>
        public static IoPoint IDI11 = new IoPoint(m_DaskController, PCI7442,0, 11, IoModes.Senser)
        {
            Name = "DI6.11",
            Description = "胶水液位感应信号"
        };

        /// <summary>
        ///   点胶完成信号
        /// </summary>
        public static IoPoint IDI12 = new IoPoint(m_DaskController, PCI7442,0, 12, IoModes.Senser)
        {
            Name = "DI6.12",
            Description = "点胶完成信号"
        };

        /// <summary>
        ///   上料联机信号1
        /// </summary>
        public static IoPoint IDI13 = new IoPoint(m_DaskController, PCI7442,0, 13, IoModes.Senser)
        {
            Name = "DI6.13",
            Description = "上料联机信号1"
        };

        /// <summary>
        ///   上料联机信号2
        /// </summary>
        public static IoPoint IDI14 = new IoPoint(m_DaskController, PCI7442,0, 14, IoModes.Senser)
        {
            Name = "DI6.14",
            Description = "上料联机信号2"
        };

        /// <summary>
        ///   Plasma工作中
        /// </summary>
        public static IoPoint IDI15 = new IoPoint(m_DaskController, PCI7442,0, 15, IoModes.Senser)
        {
            Name = "DI6.15",
            Description = "Plasma工作中"
        };

        /// <summary>
        ///   点胶夹具顶升气缸上感应
        /// </summary>
        public static IoPoint IDI16 = new IoPoint(m_DaskController, PCI7442,0, 16, IoModes.Senser)
        {
            Name = "DI6.16",
            Description = "点胶夹具顶升气缸上感应"
        };

        /// <summary>
        ///   点胶夹具顶升气缸下感应
        /// </summary>
        public static IoPoint IDI17 = new IoPoint(m_DaskController, PCI7442,0, 17, IoModes.Senser)
        {
            Name = "DI6.17",
            Description = "点胶夹具顶升气缸下感应"
        };

        /// <summary>
        ///   回流阻挡到位信号
        /// </summary>
        public static IoPoint IDI18 = new IoPoint(m_DaskController, PCI7442,0, 18, IoModes.Senser)
        {
            Name = "DI6.18",
            Description = "回流阻挡到位信号"
        };

        /// <summary>
        ///   启动按钮
        /// </summary>
        public static IoPoint IDI19 = new IoPoint(m_DaskController, PCI7442,0, 19, IoModes.Senser)
        {
            Name = "DI6.19",
            Description = "启动按钮"
        };

        /// <summary>
        ///   停止按钮
        /// </summary>
        public static IoPoint IDI20 = new IoPoint(m_DaskController, PCI7442,0, 20, IoModes.Senser)
        {
            Name = "DI6.20",
            Description = "停止按钮"
        };

        /// <summary>
        ///   复位按钮
        /// </summary>
        public static IoPoint IDI21 = new IoPoint(m_DaskController, PCI7442,0, 21, IoModes.Senser)
        {
            Name = "DI6.21",
            Description = "复位按钮"
        };

        /// <summary>
        ///   急停按钮
        /// </summary>
        public static IoPoint IDI22 = new IoPoint(m_DaskController, PCI7442,0, 22, IoModes.Senser)
        {
            Name = "DI6.22",
            Description = "急停按钮"
        };

        /// <summary>
        ///   总气源压力信号
        /// </summary>
        public static IoPoint IDI23 = new IoPoint(m_DaskController, PCI7442,0, 23, IoModes.Senser)
        {
            Name = "DI6.23",
            Description = "总气源压力信号"
        };

        /// <summary>
        ///   plasma故障
        /// </summary>
        public static IoPoint IDI24 = new IoPoint(m_DaskController, PCI7442,0, 24, IoModes.Senser)
        {
            Name = "DI6.24",
            Description = "plasma故障"
        };

        /// <summary>
        ///   plasma就绪
        /// </summary>
        public static IoPoint IDI25 = new IoPoint(m_DaskController, PCI7442,0, 25, IoModes.Senser)
        {
            Name = "DI6.25",
            Description = " plasma就绪"
        };

        /// <summary>
        ///   对针光纤Y
        /// </summary>
        public static IoPoint IDI26 = new IoPoint(m_DaskController, PCI7442,0, 26, IoModes.Senser)
        {
            Name = "DI6.26",
            Description = "对针光纤Y"
        };

        /// <summary>
        ///   对针光纤X
        /// </summary>
        public static IoPoint IDI27 = new IoPoint(m_DaskController, PCI7442,0, 27, IoModes.Senser)
        {
            Name = "DI6.27",
            Description = "对针光纤X"
        };

        /// <summary>
        ///   点胶段前门右门禁
        /// </summary>
        public static IoPoint IDI28 = new IoPoint(m_DaskController, PCI7442,0, 28, IoModes.Senser)
        {
            Name = "DI6.28",
            Description = "点胶段前门右门禁"
        };

        /// <summary>
        ///   点胶段前门左门禁
        /// </summary>
        public static IoPoint IDI29 = new IoPoint(m_DaskController, PCI7442,0, 29, IoModes.Senser)
        {
            Name = "DI6.29",
            Description = "点胶段前门左门禁"
        };

        /// <summary>
        ///   点胶段后门右门禁
        /// </summary>
        public static IoPoint IDI30 = new IoPoint(m_DaskController, PCI7442,0, 30, IoModes.Senser)
        {
            Name = "DI6.30",
            Description = "点胶段后门右门禁"
        };

        /// <summary>
        ///   点胶段后门左门禁
        /// </summary>
        public static IoPoint IDI31 = new IoPoint(m_DaskController, PCI7442,0, 31, IoModes.Senser)
        {
            Name = "DI6.31",
            Description = "点胶段后门左门禁"
        };

        /// <summary>
        ///   plasma夹具顶升气缸伸出
        /// </summary>
        public static IoPoint IDO0 = new IoPoint(m_DaskController, PCI7442,0, 0, IoModes.Signal)
        {
            Name = "DO6.0",
            Description = "plasma夹具顶升气缸伸出"
        };

        /// <summary>
        ///   plasma夹具顶升气缸退回
        /// </summary>
        public static IoPoint IDO1 = new IoPoint(m_DaskController, PCI7442,0, 1, IoModes.Signal)
        {
            Name = "DO6.1",
            Description = "plasma夹具顶升气缸退回"
        };

        /// <summary>
        ///   plasma夹持气缸夹紧
        /// </summary>
        public static IoPoint IDO2 = new IoPoint(m_DaskController, PCI7442,0, 2, IoModes.Signal)
        {
            Name = "DO6.2",
            Description = "plasma夹持气缸夹紧"
        };

        /// <summary>
        ///   plasma夹持气缸张开
        /// </summary>
        public static IoPoint IDO3 = new IoPoint(m_DaskController, PCI7442,0, 3, IoModes.Signal)
        {
            Name = "DO6.3",
            Description = "plasma夹持气缸张开"
        };
        /// <summary>
        ///   plasma翻转气缸翻转
        /// </summary>
        public static IoPoint IDO4 = new IoPoint(m_DaskController, PCI7442,0, 4, IoModes.Signal)
        {
            Name = "DO6.4",
            Description = "plasma翻转气缸翻转"
        };

        /// <summary>
        ///   plasma翻转气缸退回
        /// </summary>
        public static IoPoint IDO5 = new IoPoint(m_DaskController, PCI7442,0, 5, IoModes.Signal)
        {
            Name = "DO6.5",
            Description = "plasma翻转气缸退回"
        };

        /// <summary>
        ///   plasma上下气缸伸出
        /// </summary>
        public static IoPoint IDO6 = new IoPoint(m_DaskController, PCI7442,0, 6, IoModes.Signal)
        {
            Name = "DO6.6",
            Description = "plasma上下气缸伸出"
        };

        /// <summary>
        ///   plasma上下气缸退回
        /// </summary>
        public static IoPoint IDO7 = new IoPoint(m_DaskController, PCI7442,0, 7, IoModes.Signal)
        {
            Name = "DO6.7",
            Description = "plasma上下气缸退回"
        };

        /// <summary>
        ///   点胶回流电机反转
        /// </summary>
        public static IoPoint IDO8 = new IoPoint(m_DaskController, PCI7442,0, 8, IoModes.Signal)
        {
            Name = "DO6.8",
            Description = "点胶回流电机反转"
        };

        /// <summary>
        ///   plasma送料电机正转
        /// </summary>
        public static IoPoint IDO9 = new IoPoint(m_DaskController, PCI7442,0, 9, IoModes.Signal)
        {
            Name = "DO6.9",
            Description = "plasma送料电机正转"
        };

        /// <summary>
        ///   plasma夹具阻挡气缸
        /// </summary>
        public static IoPoint IDO10 = new IoPoint(m_DaskController, PCI7442,0, 10, IoModes.Signal)
        {
            Name = "DO6.10",
            Description = "plasma夹具阻挡气缸"
        };

        /// <summary>
        ///   Plasma上电准备
        /// </summary>
        public static IoPoint IDO11 = new IoPoint(m_DaskController, PCI7442,0, 11, IoModes.Signal)
        {
            Name = "DO6.11",
            Description = "Plasma上电准备 "
        };

        /// <summary>
        ///   点胶段夹具阻挡气缸
        /// </summary>
        public static IoPoint IDO12 = new IoPoint(m_DaskController, PCI7442,0, 12, IoModes.Signal)
        {
            Name = "DO6.12",
            Description = "点胶段夹具阻挡气缸"
        };

        /// <summary>
        ///   总气阀
        /// </summary>
        public static IoPoint IDO13 = new IoPoint(m_DaskController, PCI7442,0, 13, IoModes.Signal)
        {
            Name = "DO6.13",
            Description = "总气阀"
        };

        /// <summary>
        ///   点胶段解串板电源控制
        /// </summary>
        public static IoPoint IDO14 = new IoPoint(m_DaskController, PCI7442,0, 14, IoModes.Signal)
        {
            Name = "DO6.14",
            Description = "点胶段解串板电源控制"
        };

        /// <summary>
        ///   Plasma断电
        /// </summary>
        public static IoPoint IDO15 = new IoPoint(m_DaskController, PCI7442,0, 15, IoModes.Signal)
        {
            Name = "DO6.15",
            Description = "Plasma断电"
        };

        /// <summary>
        ///   plasma启动继电器
        /// </summary>
        public static IoPoint IDO16 = new IoPoint(m_DaskController, PCI7442,0, 16, IoModes.Signal)
        {
            Name = "DO6.16",
            Description = "plasma启动继电器"
        };

        /// <summary>
        ///   点胶夹具顶升气缸伸出
        /// </summary>
        public static IoPoint IDO17 = new IoPoint(m_DaskController, PCI7442,0, 17, IoModes.Signal)
        {
            Name = "DO6.17",
            Description = "点胶夹具顶升气缸伸出"
        };

        /// <summary>
        ///   点胶夹具顶升气缸退回
        /// </summary>
        public static IoPoint IDO18 = new IoPoint(m_DaskController, PCI7442,0, 18, IoModes.Signal)
        {
            Name = "DO6.18",
            Description = "点胶夹具顶升气缸退回"
        };

        /// <summary>
        ///   点胶电磁阀
        /// </summary>
        public static IoPoint IDO19 = new IoPoint(m_DaskController, PCI7442,0, 19, IoModes.Signal)
        {
            Name = "DO6.19",
            Description = "点胶电磁阀"
        };

        /// <summary>
        ///   出胶控制（程序里面的出胶控制，对应IO表的点胶电磁阀，IO表的出胶控制无需用到）
        /// </summary>
        public static IoPoint IDO20 = new IoPoint(m_DaskController, PCI7442,0, 20, IoModes.Signal)
        {
            Name = "DO6.20",
            Description = "出胶控制"
        };

        /// <summary>
        ///   回流阻挡气缸
        /// </summary>
        public static IoPoint IDO21 = new IoPoint(m_DaskController, PCI7442,0, 21, IoModes.Signal)
        {
            Name = "DO6.21",
            Description = "回流阻挡气缸"
        };

        /// <summary>
        ///   门禁线圈
        /// </summary>
        public static IoPoint IDO22 = new IoPoint(m_DaskController, PCI7442,0, 22, IoModes.Signal)
        {
            Name = "DO6.22",
            Description = "门禁线圈"
        };

        /// <summary>
        ///   照明灯管
        /// </summary>
        public static IoPoint IDO23 = new IoPoint(m_DaskController, PCI7442,0, 23, IoModes.Signal)
        {
            Name = "DO6.23",
            Description = "照明灯管"
        };

        /// <summary>
        ///   Pcl心跳
        /// </summary>
        public static IoPoint IDO24 = new IoPoint(m_DaskController, PCI7442,0, 24, IoModes.Signal)
        {
            Name = "DO6.24",
            Description = "Pcl心跳"
        };

        /// <summary>
        ///   停止按钮指示灯
        /// </summary>
        public static IoPoint IDO25 = new IoPoint(m_DaskController, PCI7442,0, 25, IoModes.Signal)
        {
            Name = "DO6.25",
            Description = "停止按钮指示灯"
        };

        /// <summary>
        /// 复位按钮指示灯
        /// </summary>
        public static IoPoint IDO26 = new IoPoint(m_DaskController, PCI7442,0, 26, IoModes.Signal)
        {
            Name = "DO6.26",
            Description = "复位按钮指示灯"
        };

        /// <summary>
        /// 启动按钮指示灯
        /// </summary>
        public static IoPoint IDO27 = new IoPoint(m_DaskController, PCI7442,0, 27, IoModes.Signal)
        {
            Name = "DO6.27",
            Description = "启动按钮指示灯"
        };

        /// <summary>
        ///   三色-红灯
        /// </summary>
        public static IoPoint IDO28 = new IoPoint(m_DaskController, PCI7442,0, 28, IoModes.Signal)
        {
            Name = "DO6.28",
            Description = "三色-红灯"
        };

        /// <summary>
        ///   三色-黄灯
        /// </summary>
        public static IoPoint IDO29 = new IoPoint(m_DaskController, PCI7442,0, 29, IoModes.Signal)
        {
            Name = "DO6.29",
            Description = "三色-黄灯"
        };

        /// <summary>
        ///   三色-绿灯
        /// </summary>
        public static IoPoint IDO30 = new IoPoint(m_DaskController, PCI7442,0, 30, IoModes.Signal)
        {
            Name = "DO6.30",
            Description = "三色-绿灯"
        };

        /// <summary>
        ///   三色-蜂鸣器
        /// </summary>
        public static IoPoint IDO31 = new IoPoint(m_DaskController, PCI7442,0, 31, IoModes.Signal)
        {
            Name = "DO6.31",
            Description = "三色-蜂鸣器"
        };

        #endregion
        #region PCI7442 IO list 第二段

        /// <summary>
        ///   备用（CA7线使用AA段到位感应信号）
        /// </summary>
        public static IoPoint IDI90 = new IoPoint(m_DaskController, PCI7442,1, 0, IoModes.Senser)
        {
            Name = "DI9.0",
            Description = "备用（CA7线使用AA段到位感应信号）"
        };

        /// <summary>
        ///   AA段到位感应信号
        /// </summary>
        public static IoPoint IDI91 = new IoPoint(m_DaskController, PCI7442,1, 1, IoModes.Senser)
        {
            Name = "DI9.1",
            Description = "AA段到位感应信号"
        };

        /// <summary>
        ///   AA夹具顶升气缸上感应
        /// </summary>
        public static IoPoint IDI92 = new IoPoint(m_DaskController, PCI7442,1, 2, IoModes.Senser)
        {
            Name = "DI9.2",
            Description = "AA夹具顶升气缸上感应"
        };

        /// <summary>
        ///   AA夹具顶升气缸下感应
        /// </summary>
        public static IoPoint IDI93 = new IoPoint(m_DaskController, PCI7442,1, 3, IoModes.Senser)
        {
            Name = "DI9.3",
            Description = "AA夹具顶升气缸下感应"
        };

        /// <summary>
        ///   AA堆料顶升气缸上感应
        /// </summary>
        public static IoPoint IDI94 = new IoPoint(m_DaskController, PCI7442,1, 4, IoModes.Senser)
        {
            Name = "DI9.4",
            Description = "AA堆料顶升气缸上感应"
        };

        /// <summary>
        ///   AA堆料顶升气缸下感应
        /// </summary>
        public static IoPoint IDI95 = new IoPoint(m_DaskController, PCI7442,1, 5, IoModes.Senser)
        {
            Name = "DI9.5",
            Description = "AA堆料顶升气缸下感应"
        };
        /// <summary>
        ///  beiyong
        /// </summary>
        public static IoPoint IDI96 = new IoPoint(m_DaskController, PCI7442,1, 6, IoModes.Senser)
        {
            Name = "DI9.6",
            Description = "beiyong"
        };
        /// <summary>
        ///   UV灯上下气缸上感应
        /// </summary>
        public static IoPoint IDI97 = new IoPoint(m_DaskController, PCI7442,1, 7, IoModes.Senser)
        {
            Name = "DI9.7",
            Description = "UV灯上下气缸上感应"
        };
        /// <summary>
        ///   UV灯上下气缸下感应
        /// </summary>
        public static IoPoint IDI98 = new IoPoint(m_DaskController, PCI7442,1, 8, IoModes.Senser)
        {
            Name = "DI9.8",
            Description = "UV灯上下气缸下感应"
        };
        /// <summary>
        ///   AA气夹爪OFF信号
        /// </summary>
        public static IoPoint IDI99 = new IoPoint(m_DaskController, PCI7442,1, 9, IoModes.Senser)
        {
            Name = "DI9.9",
            Description = "AA气夹爪OFF信号"
        };

        /// <summary>
        ///   AA气夹爪ON信号
        /// </summary>
        public static IoPoint IDI910 = new IoPoint(m_DaskController, PCI7442,1, 10, IoModes.Senser)
        {
            Name = "DI9.10",
            Description = "AA气夹爪ON信号"
        };
        /// <summary>
        ///   AA段堆料位到位感应
        /// </summary>
        public static IoPoint IDI911 = new IoPoint(m_DaskController, PCI7442,1, 11, IoModes.Senser)
        {
            Name = "DI9.11",
            Description = "AA段堆料位到位感应"
        };

        /// <summary>
        ///   AA段前门右门禁
        /// </summary>
        public static IoPoint IDI912 = new IoPoint(m_DaskController, PCI7442,1, 12, IoModes.Senser)
        {
            Name = "DI9.12",
            Description = "AA段前门右门禁"
        };

        /// <summary>
        ///   AA段前门左门禁
        /// </summary>
        public static IoPoint IDI913 = new IoPoint(m_DaskController, PCI7442,1,13, IoModes.Senser)
        {
            Name = "DI9.13",
            Description = "AA段前门左门禁"
        };

        /// <summary>
        ///   AA段后门右门禁
        /// </summary>
        public static IoPoint IDI914 = new IoPoint(m_DaskController, PCI7442,1, 14, IoModes.Senser)
        {
            Name = "DI9.14",
            Description = "AA段后门右门禁"
        };

        /// <summary>
        ///   AA段后门左门禁
        /// </summary>
        public static IoPoint IDI915 = new IoPoint(m_DaskController, PCI7442,1, 15, IoModes.Senser)
        {
            Name = "DI9.15",
            Description = "AA段后门左门禁"
        };

        /// <summary>
        ///   AA探针顶升气缸原位
        /// </summary>
        public static IoPoint IDI916 = new IoPoint(m_DaskController, PCI7442,1, 16, IoModes.Senser)
        {
            Name = "DI9.16",
            Description = "AA探针顶升气缸原位"
        };

        /// <summary>
        ///   AA探针顶升气缸到位
        /// </summary>
        public static IoPoint IDI917 = new IoPoint(m_DaskController, PCI7442,1, 17, IoModes.Senser)
        {
            Name = "DI9.17",
            Description = "AA探针顶升气缸到位"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI918 = new IoPoint(m_DaskController, PCI7442,1, 18, IoModes.Senser)
        {
            Name = "DI9.18",
            Description = "备用"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI919 = new IoPoint(m_DaskController, PCI7442,1, 19, IoModes.Senser)
        {
            Name = "DI9.19",
            Description = "备用"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI920 = new IoPoint(m_DaskController, PCI7442,1, 20, IoModes.Senser)
        {
            Name = "DI9.20",
            Description = "备用"
        };

        /// <summary>
        ///   AA出料感应2
        /// </summary>
        public static IoPoint IDI921 = new IoPoint(m_DaskController, PCI7442,1, 21, IoModes.Senser)
        {
            Name = "DI9.21",
            Description = "AA出料感应2"
        };

        /// <summary>
        ///   AA出料感应1
        /// </summary>
        public static IoPoint IDI922 = new IoPoint(m_DaskController, PCI7442,1, 22, IoModes.Senser)
        {
            Name = "DI9.22",
            Description = "AA出料感应1"
        };

        /// <summary>
        ///   平行光管支架到位信号
        /// </summary>
        public static IoPoint IDI923 = new IoPoint(m_DaskController, PCI7442,1,23, IoModes.Senser)
        {
            Name = "DI9.23",
            Description = "平行光管支架到位信号"
        };

        /// <summary>
        ///   AA回流线治具到位感应
        /// </summary>
        public static IoPoint IDI924 = new IoPoint(m_DaskController, PCI7442,1, 24, IoModes.Senser)
        {
            Name = "DI9.24",
            Description = "AA回流线治具到位感应"
        };

        /// <summary>
        ///   AA回流阻挡到位感应
        /// </summary>
        public static IoPoint IDI925 = new IoPoint(m_DaskController, PCI7442,1, 25, IoModes.Senser)
        {
            Name = "DI9.25",
            Description = "AA回流阻挡到位感应"
        };
        /// <summary>
        ///   接收后机联机信号1
        /// </summary>
        public static IoPoint IDI926 = new IoPoint(m_DaskController, PCI7442,1, 26, IoModes.Senser)
        {
            Name = "DI9.26",
            Description = "接收后机联机信号1"
        };
        /// <summary>
        ///   接收后机联机信号2
        /// </summary>
        public static IoPoint IDI927 = new IoPoint(m_DaskController, PCI7442,1, 27, IoModes.Senser)
        {
            Name = "DI9.27",
            Description = "接收后机联机信号2"
        };
        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI928 = new IoPoint(m_DaskController, PCI7442,1, 28, IoModes.Senser)
        {
            Name = "DI9.28",
            Description = "备用"
        };
        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI929 = new IoPoint(m_DaskController, PCI7442,1, 29, IoModes.Senser)
        {
            Name = "DI9.29",
            Description = "备用"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI930 = new IoPoint(m_DaskController, PCI7442,1, 30, IoModes.Senser)
        {
            Name = "DI9.30",
            Description = "备用"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDI931 = new IoPoint(m_DaskController, PCI7442,1, 31, IoModes.Senser)
        {
            Name = "DI9.31",
            Description = "备用"
        };

        /// <summary>
        ///   AA送料线电机正转
        /// </summary>
        public static IoPoint IDO90 = new IoPoint(m_DaskController, PCI7442,1,0, IoModes.Signal)
        {
            Name = "DO9.0",
            Description = "AA送料线电机正转"
        };
        /// <summary>
        ///   AA夹具阻挡气缸
        /// </summary>
        public static IoPoint IDO91 = new IoPoint(m_DaskController, PCI7442,1,1, IoModes.Signal)
        {
            Name = "DO9.1",
            Description = "AA夹具阻挡气缸"
        };

        /// <summary>
        ///   AA夹具顶升气缸伸出
        /// </summary>
        public static IoPoint IDO92 = new IoPoint(m_DaskController, PCI7442,1,2, IoModes.Signal)
        {
            Name = "DO9.2",
            Description = "AA夹具顶升气缸伸出"
        };

        /// <summary>
        ///   AA夹具顶升气缸退回
        /// </summary>
        public static IoPoint IDO93 = new IoPoint(m_DaskController, PCI7442,1,3, IoModes.Signal)
        {
            Name = "DO9.3",
            Description = "AA夹具顶升气缸退回"
        };
        /// <summary>
        ///   AA堆料顶升气缸伸出
        /// </summary>
        public static IoPoint IDO94 = new IoPoint(m_DaskController, PCI7442,1,4, IoModes.Signal)
        {
            Name = "DO9.4",
            Description = "AA堆料顶升气缸伸出"
        };

        /// <summary>
        ///   AA堆料顶升气缸退回
        /// </summary>
        public static IoPoint IDO95 = new IoPoint(m_DaskController, PCI7442,1,5, IoModes.Signal)
        {
            Name = "DO9.5",
            Description = "AA堆料顶升气缸退回"
        };
        /// <summary>
        ///   给后机联机信号1
        /// </summary>
        public static IoPoint IDO96 = new IoPoint(m_DaskController, PCI7442,1, 6, IoModes.Signal)
        {
            Name = "DO9.6",
            Description = "给后机联机信号1"
        };
        /// <summary>
        ///   给后机联机信号2
        /// </summary>
        public static IoPoint IDO97 = new IoPoint(m_DaskController, PCI7442,1,  7, IoModes.Signal)
        {
            Name = "DO9.7",
            Description = "给后机联机信号2"
        };
        /// <summary>
        ///   AA堆料阻挡气缸
        /// </summary>
        public static IoPoint IDO98 = new IoPoint(m_DaskController, PCI7442,1, 8, IoModes.Signal)
        {
            Name = "DO9.8",
            Description = "AA堆料阻挡气缸"
        };

        /// <summary>
        ///   AA回流阻挡气缸
        /// </summary>
        public static IoPoint IDO99 = new IoPoint(m_DaskController, PCI7442,1,9, IoModes.Signal)
        {
            Name = "DO9.9",
            Description = "AA回流阻挡气缸"
        };

        /// <summary>
        ///   给后机联机信号OK
        /// </summary>
        public static IoPoint IDO910 = new IoPoint(m_DaskController, PCI7442,1, 10, IoModes.Signal)
        {
            Name = "DO9.10",
            Description = "给后机联机信号OK"
        };

        /// <summary>
        ///   给后机联机信号NG
        /// </summary>
        public static IoPoint IDO911 = new IoPoint(m_DaskController, PCI7442,1,11, IoModes.Signal)
        {
            Name = "DO9.11",
            Description = "给后机联机信号NG"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDO912 = new IoPoint(m_DaskController, PCI7442,1,12, IoModes.Signal)
        {
            Name = "DO9.12",
            Description = "备用"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDO913 = new IoPoint(m_DaskController, PCI7442,1,13, IoModes.Signal)
        {
            Name = "DO9.13",
            Description = "备用"
        };

        /// <summary>
        ///   AA回流电机反转
        /// </summary>
        public static IoPoint IDO914 = new IoPoint(m_DaskController, PCI7442,1,14, IoModes.Signal)
        {
            Name = "DO9.14",
            Description = "AA回流电机反转"
        };

        /// <summary>
        ///   AA段解串板电源控制
        /// </summary>
        public static IoPoint IDO915 = new IoPoint(m_DaskController, PCI7442,1,15, IoModes.Signal)
        {
            Name = "DO9.15",
            Description = "AA段解串板电源控制"
        };

        /// <summary>
        ///   UV灯上下气缸伸出
        /// </summary>
        public static IoPoint IDO916 = new IoPoint(m_DaskController, PCI7442,1, 16, IoModes.Signal)
        {
            Name = "DO9.16",
            Description = "UV灯上下气缸伸出"
        };

        /// <summary>
        ///   UV灯启动（1-4）
        /// </summary>
        public static IoPoint IDO917 = new IoPoint(m_DaskController, PCI7442,1,17, IoModes.Signal)
        {
            Name = "DO9.17",
            Description = "UV灯启动（1-4）"
        };

        /// <summary>
        ///   AA气夹爪退回
        /// </summary>
        public static IoPoint IDO918 = new IoPoint(m_DaskController, PCI7442,1,18, IoModes.Signal)
        {
            Name = "DO9.18",
            Description = "AA气夹爪退回"
        };

        /// <summary>
        ///   UV灯上下气缸退回
        /// </summary>
        public static IoPoint IDO919 = new IoPoint(m_DaskController, PCI7442,1,19, IoModes.Signal)
        {
            Name = "DO9.19",
            Description = "UV灯上下气缸退回"
        };

        /// <summary>
        ///   AA气夹爪气缸张开
        /// </summary>
        public static IoPoint IDO920 = new IoPoint(m_DaskController, PCI7442,1,20, IoModes.Signal)
        {
            Name = "DO9.20",
            Description = "AA气夹爪气缸张开"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDO921 = new IoPoint(m_DaskController, PCI7442,1,21, IoModes.Signal)
        {
            Name = "DO9.21",
            Description = "备用"
        };

        /// <summary>
        ///   AA探针顶升气缸缩回
        /// </summary>
        public static IoPoint IDO922 = new IoPoint(m_DaskController, PCI7442,1,22, IoModes.Signal)
        {
            Name = "DO9.22",
            Description = "AA探针顶升气缸缩回"
        };

        /// <summary>
        ///   AA探针顶升气缸伸出
        /// </summary>
        public static IoPoint IDO923 = new IoPoint(m_DaskController, PCI7442,1,23, IoModes.Signal)
        {
            Name = "DO9.23",
            Description = "AA探针顶升气缸伸出"
        };

        /// <summary>
        ///   AA平台X轴使能
        /// </summary>
        public static IoPoint IDO924 = new IoPoint(m_DaskController, PCI7442,1,24, IoModes.Signal)
        {
            Name = "DO9.24",
            Description = "AA平台X轴使能"
        };

        /// <summary>
        ///   AA平台Y轴使能
        /// </summary>
        public static IoPoint IDO925 = new IoPoint(m_DaskController, PCI7442,1,25, IoModes.Signal)
        {
            Name = "DO9.25",
            Description = "AA平台Y轴使能"
        };

        /// <summary>
        /// AA平台Z轴使能
        /// </summary>
        public static IoPoint IDO926 = new IoPoint(m_DaskController, PCI7442,1, 26, IoModes.Signal)
        {
            Name = "DO9.26",
            Description = "AA平台Z轴使能"
        };

        /// <summary>
        /// AA平台U轴使能
        /// </summary>
        public static IoPoint IDO927 = new IoPoint(m_DaskController, PCI7442,1,27, IoModes.Signal)
        {
            Name = "DO9.27",
            Description = "AA平台U轴使能"
        };

        /// <summary>
        ///   AA平台V轴使能
        /// </summary>
        public static IoPoint IDO928 = new IoPoint(m_DaskController, PCI7442,1,28, IoModes.Signal)
        {
            Name = "DO9.28",
            Description = "AA平台V轴使能"
        };

        /// <summary>
        ///   AA平台W轴使能
        /// </summary>
        public static IoPoint IDO929 = new IoPoint(m_DaskController, PCI7442,1,29, IoModes.Signal)
        {
            Name = "DO9.29",
            Description = "AA平台W轴使能"
        };

        /// <summary>
        ///   备用
        /// </summary>
        public static IoPoint IDO930 = new IoPoint(m_DaskController, PCI7442,1,30, IoModes.Signal)
        {
            Name = "DO9.30",
            Description = "三色-绿灯"
        };

        /// <summary>
        ///   三色-蜂鸣器
        /// </summary>
        public static IoPoint IDO931 = new IoPoint(m_DaskController, PCI7442,1,31, IoModes.Signal)
        {
            Name = "DO9.31",
            Description = "备用"
        };

        #endregion
    }
}
