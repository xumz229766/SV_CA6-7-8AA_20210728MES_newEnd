using desay.ProductData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Device;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Toolkit.Helpers;
using System.Windows.Forms;
using System.Net;

namespace desay
{
    public partial class frmCommSetting : Form
    {
        public frmCommSetting()
        {
            InitializeComponent();

            WbPowerParam.SetConnectionString(Config.Instance.PowerComString);
          
            TB_FormerStation_Ip.Text = Config.Instance.FormerStationIp;
            tbLightControlp.Text= Config.Instance.LightControl_IP ;
            this.TB_FormerStation_Port.Text = Config.Instance.FormerStationPort.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.Instance.PowerComString = WbPowerParam.GetConnectionString();

     
            //待添加保存操作
            try
            {
                IPAddress Ip = IPAddress.Parse(this.TB_FormerStation_Ip.Text);
                Config.Instance.FormerStationIp = this.TB_FormerStation_Ip.Text;
            }
            catch
            {
                MessageBox.Show("上料机IP地址格式不正确");
            }
            int.TryParse(this.TB_FormerStation_Port.Text, out Config.Instance.FormerStationPort);
            SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
            try
            {
                IPAddress Ip = IPAddress.Parse(this.tbLightControlp.Text);
                Config.Instance.LightControl_IP = tbLightControlp.Text;
                SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
            }
            catch
            {
                MessageBox.Show("光控IP地址格式不正确");
            }
            
        }

        public void GetTcpParam(string str, ref string ip, ref int port)
        {
            string[] param = str.Split(',');
            ip = param[0];
            port = int.Parse(param[1]);
            int readTimeout = int.Parse(param[2]);
            int writeTimeout = int.Parse(param[3]);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        Panasonic test = new Panasonic();
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    test.SetConnectionParam(Position.Instance.HeightConnectionString);
        //    if (test.IsOpen)
        //        test.DeviceClose();
        //    test.DeviceOpen();
        //    if (test.IsOpen)
        //    {
        //        MessageBox.Show("open sucess");
        //    }
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            test.WriteDetectHeightCommand();
            Stopwatch time = new Stopwatch();
            time.Start();
            while (true)
            {
                if (time.ElapsedMilliseconds > test.ReadTimeout)
                {
                    time.Stop();
                    break;
                }
                if (test.StringReceived)
                {
                    test.StringReceived = false;
                    MessageBox.Show(test.ReceiveString);
                    break;
                }
            }
            if (test.IsOpen)
                test.DeviceClose();
        }

        private void frmCommSetting_Load(object sender, EventArgs e)
        {

        }
    }
}
