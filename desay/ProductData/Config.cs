using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Toolkit;
using System.Toolkit.Helpers;
namespace desay.ProductData
{
    [Serializable]
    public class Config
    {
        [NonSerialized]
        public static Config Instance = new Config();
        //用户相关信息
        public string userName, SuperUserPassword = SecurityHelper.TextToMd5("desay"), AdminPassword = SecurityHelper.TextToMd5("321"), OperatePassword = SecurityHelper.TextToMd5("123");
        public UserLevel userLevel = UserLevel.None;

        public string SerialParam = "COM2,115200,0,8,1,1500,1500";
        /// <summary>
        /// 当前产品型号
        /// </summary>
        public string CurrentProductType  = "defualt";

        #region 产品计数

        /// <summary>
        /// 清洗产品NG总数
        /// </summary>
        public int CleanMesLockNgTotal=0;
        /// <summary>
        /// 点胶白板NG_点亮NG
        /// </summary>
        public int GlueWbNgTotal_LightNG=0;
        /// <summary>
        /// 点胶白板NG_脏污NG
        /// </summary>
        public int GlueWbNgTotal_ParticeNG = 0;
        /// <summary>
        /// 点胶定位NG总数
        /// </summary>
        public int GlueLocationNgTotal=0;
        /// <summary>
        /// 点胶识别NG总数
        /// </summary>
        public int GlueFindNgTotal=0;
        /// <summary>
        /// AA点亮NG
        /// </summary>
        public int AALightNgTotal = 0;
        /// <summary>
        /// AA点亮丢帧NG
        /// </summary>
        public int AALightLossFrameNgTotal = 0;
        /// <summary>
        /// AA运动NG
        /// </summary>
        public int AAMoveNgTotal = 0;
        /// <summary>
        /// AA搜索NG
        /// </summary>
        public int AASerchNgTotal = 0;
        /// <summary>
        /// AAOCNG
        /// </summary>
        public int AAOCNgTotal = 0;
        /// <summary>
        /// AA倾斜NG
        /// </summary>
        public int AATILT_TUNENgTotal = 0;
        /// <summary>
        /// AAUV前NG
        /// </summary>
        public int AAUVBeforeNgTotal = 0;
        /// <summary>
        /// AAUV后NG
        /// </summary>
        public int AAUVAfterNgTotal = 0;
        /// <summary>
        /// 条码NG
        /// </summary>
        public int AASNNgTotal = 0;
        /// <summary>
        /// 其余NG
        /// </summary>
        public int NoneNgTotal = 0;
        /// <summary>
        /// AA产品OK总数
        /// </summary>
        public int AllAAProductOkTotal=0;
        /// <summary>
        /// AA产品NG总数  互锁NG不算？
        /// </summary>
        public int AllAAProductNgTotal=0;
        /// <summary>
        /// 生产总数
        /// </summary>
        public int ProductTotal { get { return AllAAProductOkTotal + AllAAProductNgTotal; } }
                                     
        public string ImageSAvePath = "E:\\Image";
        #endregion
        /// <summary>
        /// 程控电源通讯设置字符串
        /// </summary>
        public string PowerComString = "COM6,38400,None,8,One,5000,5000";
        /// <summary>
        /// 光源控制器IP
        /// </summary>
        public string LightControl_IP = "192.168.1.164";//暂定

        /// <summary>
        /// 上一工位TCP通信端口号
        /// </summary>
        public int FormerStationPort = 40000;
        /// <summary>
        /// 上一工位TCP通信IP地址
        /// </summary>
        public string FormerStationIp = "192.168.100.13";
        /// <summary>
        /// Ca6 AA到位感应 用9.1   CA7 用9.0 
        /// </summary>
        public bool IsIO91 = true;

        public bool IsMes = true;
        /// <summary>
        /// 互锁螺丝机ID号
        /// </summary>
        public string FormerStationID = "MED-MSCREW-0017";

        public DateTime GlueDataTime=DateTime.Now;
        public double GlueTimeAlarm = 72;//胶水报警时间
        #region 胶水设置
        /// <summary>
        /// 胶水使用总次数
        /// </summary>
        public int GlueUseAllNums = 300;
        /// <summary>
        /// 胶水报警次数
        /// </summary>
        public int GlueUseAlarmNums = 20;
        /// <summary>
        /// 胶水使用当前次数计数
        /// </summary>
        public int GlueUseNumsIndex = 0;
        public double GlueHaveUseTime = 0;

        //public DateTime GlueUseTime;
        #endregion
        /// <summary>
        /// 程控电源通道
        /// </summary>
        public int PowerChanel_Wb = 1;
        public int PowerChanel_AA = 2;

        public bool IsTeachGlueFind = false;
        /// <summary>
        /// A2C 后6位 与SN前6位 是否匹配
        /// </summary>
        public int A2CSNCheckLength = 6;
        /// <summary>
        /// SV员工工号长度限制
        /// </summary>
        public int SVjobNumberLength1 = 8;
        public int SVjobNumberLength2 = 9;
        

    }
}
