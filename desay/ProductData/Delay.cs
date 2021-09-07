using Motion.Enginee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desay
{
    [Serializable]
    public class Delay
    {
        [NonSerialized]
        public static Delay Instance = new Delay();

        #region 清洗位
        /// <summary>
        /// 清洗阻挡气缸
        /// </summary>
        public CylinderDelay cleanStopCylinderDelay = new CylinderDelay() { OriginTime = 50, MoveTime = 50, AlarmTime = 100 };
        /// <summary>
        /// 清洗顶升气缸
        /// </summary>
        public CylinderDelay cleanUpCylinderDelay = new CylinderDelay() { OriginTime = 100, MoveTime = 100, AlarmTime = 500 };
        /// <summary>
        /// 清洗夹紧气缸
        /// </summary>
        public CylinderDelay cleanClampCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// 清洗上下气缸
        /// </summary>
        public CylinderDelay cleanUpDownCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// 清洗旋转气缸
        /// </summary>
        public CylinderDelay cleanRotateCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        #endregion

        #region 点胶位
        /// <summary>
        /// 点胶阻挡气缸
        /// </summary>
        public CylinderDelay glueStopCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// 点胶顶升气缸
        /// </summary>
        public CylinderDelay glueUpCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// 点胶小顶升气缸
        /// </summary>
        public CylinderDelay glueUpCylinderDelay_small = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 1500 };
        /// <summary>
        /// 清洗点胶回流线阻挡气缸
        /// </summary>
        public CylinderDelay cleanAndGlueStopCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        #endregion

        #region AA位
        /// <summary>
        /// AA夹具阻挡气缸
        /// </summary>
        public CylinderDelay AAJigsStopCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// AA夹具顶升气缸
        /// </summary>
        public CylinderDelay AAJigsUpCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// AA夹具小顶升气缸
        /// </summary>
        public CylinderDelay AAJigsUpCylinderDelay_Small = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 1500 };
        /// <summary>
        /// AA堆料顶升气缸
        /// </summary>
        public CylinderDelay AAStockpileUpCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// AA堆料阻挡气缸
        /// </summary>
        public CylinderDelay AAStockpileStopCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// AA回流阻挡气缸
        /// </summary>
        public CylinderDelay AABackFlowStopCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        // <summary>
        /// UV灯上下气缸
        /// </summary>
        public CylinderDelay AAUVUpDownCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        /// <summary>
        /// AA气夹爪气缸
        /// </summary>
        public CylinderDelay AAClampClawCylinderDelay = new CylinderDelay() { OriginTime = 0, MoveTime = 0, AlarmTime = 500 };
        #endregion

    }
}
