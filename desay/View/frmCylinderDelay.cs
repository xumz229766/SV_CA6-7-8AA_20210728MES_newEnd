using desay.ProductData;
using Motion.Enginee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Toolkit.Helpers;
using System.Windows.Forms;

namespace desay
{
    public partial class frmCylinderDelay : Form
    {
        #region 气缸延时


        private CylinderParameter CleanStopParameter;
        private CylinderParameter CleanUpParameter;
        private CylinderParameter CleanRotateParameter;
        private CylinderParameter CleanClampParameter;
        private CylinderParameter CleanUpDownParameter;


        private CylinderParameter GlueStopParameter;
        private CylinderParameter GlueUpParameter;
        private CylinderParameter GlueUpParameter_Small;
        private CylinderParameter CleanAndGlueStopParameter;

        private CylinderParameter AAJigsStopParameter;
        private CylinderParameter AAJigsUpParameter;
        private CylinderParameter AAJigsUpParameter_Small;
        private CylinderParameter AAStockpileUpParameter;
        private CylinderParameter AAStockpileStopParameter;
        private CylinderParameter AABackFlowStopParameter;
        private CylinderParameter AAUVUpStopParameter;
        private CylinderParameter AAClampClawParameter;
        #endregion
        public frmCylinderDelay()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 气缸参数


            Delay.Instance.cleanStopCylinderDelay = CleanStopParameter.Save;
            Delay.Instance.cleanUpCylinderDelay = CleanUpParameter.Save;
            Delay.Instance.cleanRotateCylinderDelay = CleanRotateParameter.Save;
            Delay.Instance.cleanClampCylinderDelay = CleanClampParameter.Save;
            Delay.Instance.cleanUpDownCylinderDelay = CleanUpDownParameter.Save;
     
            Delay.Instance.glueStopCylinderDelay = GlueStopParameter.Save;
            Delay.Instance.glueUpCylinderDelay = GlueUpParameter.Save;
            Delay.Instance.glueUpCylinderDelay_small = GlueUpParameter_Small.Save;
            Delay.Instance.cleanAndGlueStopCylinderDelay = CleanAndGlueStopParameter.Save;

            Delay.Instance.AAJigsStopCylinderDelay = AAJigsStopParameter.Save;
            Delay.Instance.AAJigsUpCylinderDelay = AAJigsUpParameter.Save;
            Delay.Instance.AAStockpileUpCylinderDelay = AAStockpileUpParameter.Save;
            Delay.Instance.AAStockpileStopCylinderDelay = AAStockpileStopParameter.Save;
            Delay.Instance.AABackFlowStopCylinderDelay = AABackFlowStopParameter.Save;
            Delay.Instance.AAUVUpDownCylinderDelay = AAUVUpStopParameter.Save;
            Delay.Instance.AAClampClawCylinderDelay = AAClampClawParameter.Save;

            Delay.Instance.AAJigsUpCylinderDelay_Small = AAJigsUpParameter_Small.Save;
            #endregion

            SerializerManager<Delay>.Instance.Save(AppConfig.ConfigDelayName, Delay.Instance);
        }

        private void frmCylinderDelay_Load(object sender, EventArgs e)
        {
            CleanStopParameter = new CylinderParameter(Delay.Instance.cleanStopCylinderDelay) { Name = "清洗阻挡气缸" };
            CleanUpParameter = new CylinderParameter(Delay.Instance.cleanUpCylinderDelay) { Name = "清洗顶升气缸" };
            CleanRotateParameter = new CylinderParameter(Delay.Instance.cleanRotateCylinderDelay) { Name = "清洗旋转气缸" };
            CleanClampParameter = new CylinderParameter(Delay.Instance.cleanClampCylinderDelay) { Name = "清洗夹紧气缸" };
            CleanUpDownParameter = new CylinderParameter(Delay.Instance.cleanUpDownCylinderDelay) { Name = "清洗上下气缸" };
           
            GlueStopParameter = new CylinderParameter(Delay.Instance.glueStopCylinderDelay) { Name = "点胶阻挡气缸" };
            GlueUpParameter = new CylinderParameter(Delay.Instance.glueUpCylinderDelay) { Name = "点胶顶升气缸" };
            GlueUpParameter_Small = new CylinderParameter(Delay.Instance.glueUpCylinderDelay_small) { Name = "点胶探针顶升气缸" };
            CleanAndGlueStopParameter = new CylinderParameter(Delay.Instance.cleanAndGlueStopCylinderDelay) { Name = "清洗点胶回流线阻挡气缸" };

            AAJigsStopParameter = new CylinderParameter(Delay.Instance.AAJigsStopCylinderDelay) { Name = "AA夹具阻挡气缸" };
            AAJigsUpParameter = new CylinderParameter(Delay.Instance.AAJigsUpCylinderDelay) { Name = "AA夹具顶升气缸" };
            AAJigsUpParameter_Small = new CylinderParameter(Delay.Instance.AAJigsUpCylinderDelay_Small) { Name = "AA夹具探针顶升气缸" };
            AAStockpileUpParameter = new CylinderParameter(Delay.Instance.AAStockpileUpCylinderDelay) { Name = "AA堆料顶升气缸" };
            AAStockpileStopParameter = new CylinderParameter(Delay.Instance.AAStockpileStopCylinderDelay) { Name = "AA堆料阻挡气缸" };
            AABackFlowStopParameter = new CylinderParameter(Delay.Instance.AABackFlowStopCylinderDelay) { Name = "AA回流阻挡气缸" };
            AAUVUpStopParameter = new CylinderParameter(Delay.Instance.AAUVUpDownCylinderDelay) { Name = "UV灯上下气缸" };
            AAClampClawParameter = new CylinderParameter(Delay.Instance.AAClampClawCylinderDelay) { Name = "AA气夹爪气缸" };
            flpCylinderPlasma.Controls.Add(CleanUpParameter);
            flpCylinderPlasma.Controls.Add(CleanRotateParameter);
            flpCylinderPlasma.Controls.Add(CleanClampParameter);
            flpCylinderPlasma.Controls.Add(CleanUpDownParameter);

            flpCylinderGlue.Controls.Add(GlueStopParameter);
            flpCylinderGlue.Controls.Add(GlueUpParameter);
            flpCylinderGlue.Controls.Add(CleanAndGlueStopParameter);
            flpCylinderGlue.Controls.Add(GlueUpParameter_Small);

            flpCylinderAA.Controls.Add(AAJigsStopParameter);
            flpCylinderAA.Controls.Add(AAJigsUpParameter);
            flpCylinderAA.Controls.Add(AAJigsUpParameter_Small);
            flpCylinderAA.Controls.Add(AAStockpileUpParameter);
            flpCylinderAA.Controls.Add(AAStockpileStopParameter);
            flpCylinderAA.Controls.Add(AABackFlowStopParameter);
            flpCylinderAA.Controls.Add(AAUVUpStopParameter);
            flpCylinderAA.Controls.Add(AAClampClawParameter);
        }
    }
}
