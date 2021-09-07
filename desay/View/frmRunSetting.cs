using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using desay.ProductData;
namespace desay
{
    public partial class frmRunSetting : Form
    {
        public frmRunSetting()
        {
            InitializeComponent();
        }

        private void frmRunSetting_Load(object sender, EventArgs e)
        {
            chkCarrierHaveProduct.Checked = Marking.CarrierHaveProduct;
            chkCleanHaveProduct.Checked = Marking.CleanHaveProduct;
            chkGlueHaveProduct.Checked = Marking.GlueHaveProduct;
            chkDoorSheild.Checked = Marking.DoorShield;
            chkCurtainShield.Checked = Marking.CurtainShield;
            chkGHShield.Checked = Marking.GHShield;
            chkSnScannerShield.Checked = Marking.SnScannerShield;
            this.CHB_FORMER_STATION_SHIELD.Checked = Marking.FormerStationShield;
            this.CHB_LENS_SENSOR_SHIELD.Checked = Marking.LensSensorShield;
            this.CHB_SHELL_SENSOR_SHIELD.Checked = Marking.ShellSensorShield;
            this.cbGlueHave.Checked = Marking.GlueHaveShield;
            cbAAhomeShield.Checked = Marking.AAhomeShield;
            cbPlasmaAlarmShield.Checked= Marking.PlasmaAlarmShield  ;
            cbPlcReflashShield.Checked = Marking.PlcReflashShield;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Marking.CarrierHaveProduct = chkCarrierHaveProduct.Checked;
            Marking.CleanHaveProduct = chkCleanHaveProduct.Checked;
            Marking.GlueHaveProduct = chkGlueHaveProduct.Checked;
            Marking.DoorShield = chkDoorSheild.Checked;
            Marking.CurtainShield = chkCurtainShield.Checked;
            Marking.GHShield = chkGHShield.Checked;
            Marking.SnScannerShield = chkSnScannerShield.Checked;
            Marking.FormerStationShield = this.CHB_FORMER_STATION_SHIELD.Checked;
            Marking.LensSensorShield = this.CHB_LENS_SENSOR_SHIELD.Checked;
            Marking.ShellSensorShield = this.CHB_SHELL_SENSOR_SHIELD.Checked;
            Marking.GlueHaveShield = this.cbGlueHave.Checked;
            Marking.AAhomeShield= cbAAhomeShield.Checked ;
            Marking.PlasmaAlarmShield = cbPlasmaAlarmShield.Checked;
            Marking.PlcReflashShield = cbPlcReflashShield.Checked;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
