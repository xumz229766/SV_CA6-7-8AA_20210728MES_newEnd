using desay.ProductData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Toolkit.Helpers;
namespace desay
{
    public partial class WhiteBoardPower : Form
    {
        private SerialPort wbPort;
        private SerialPort aaPort;
        public WhiteBoardPower(SerialPort Port)
        {
            InitializeComponent();
            wbPort = Port;
            aaPort = Port;
        }
        private void WhiteBoardPower_Load_1(object sender, EventArgs e)
        {
            try
            {
                wbPowerChannel.Text = Config.Instance.PowerChanel_Wb.ToString();
                AAPowerChannel.Text = Config.Instance.PowerChanel_AA.ToString();
                nudWbI.Value = (decimal)Position.Instance.Current_Wb;
                mudWbV.Value = (decimal)Position.Instance.Voltage_Wb;
                nudAAI.Value = (decimal)Position.Instance.Current_AA;
                nudAAV.Value = (decimal)Position.Instance.Voltage_AA;
            }
            catch { }
        }
        public WhiteBoardPower(SerialPort sp, out bool success)
        {
            InitializeComponent();
            wbPort = sp;
            if (wbPort != null)
            {
                success = true;
            }
            else
            {
                success = false;
            }
        }

        private void WhiteBoardPower_Load(object sender, EventArgs e)
        {
            // this.numericUpDown1.Value = Position.Instance.
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("是否立即生效", "是否立即生效", MessageBoxButtons.YesNo))
                {

                    if (this.wbPort.IsOpen)
                    {
                        wbPort.Write("SYST:REM" + Environment.NewLine);
                        Thread.Sleep(50);
                        wbPort.Write($"INST CH{Config.Instance.PowerChanel_Wb}" + Environment.NewLine);
                        wbPort.Write("VOLT " + Position.Instance.Voltage_Wb.ToString() + Environment.NewLine);
                        Thread.Sleep(50);
                        wbPort.Write("CURR " + Position.Instance.Current_Wb.ToString() + Environment.NewLine);
                        Thread.Sleep(50);

                       

                        this.wbPort.Write("OUTP 1" + Environment.NewLine);



                    }
                    else
                    {
                        MessageBox.Show("端口已断开");
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("立即生效异常:" + ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (wbPort.IsOpen)
            {
                wbPort.Write("OUPT 0\r\n");
                MessageBox.Show("完成");
            }
            else
            {
                MessageBox.Show("端口已断开，设置失败！");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("是否保存", "是否保存", MessageBoxButtons.YesNo))
                {
                    string SelectedString = wbPowerChannel.Text;

                    Position.Instance.Current_Wb = (double)nudWbI.Value;
                    Position.Instance.Voltage_Wb = (double)mudWbV.Value;
                    Config.Instance.PowerChanel_Wb = Convert.ToInt32(SelectedString);
                    SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                    SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存异常:" + ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

          

        }

        private void btnAAsave_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("是否保存", "是否保存", MessageBoxButtons.YesNo))
                {
                    string SelectedString = AAPowerChannel.Text;
                   
                    Position.Instance.Current_AA = (double)nudAAI.Value;
                    Position.Instance.Voltage_AA = (double)nudAAV.Value;
                    Config.Instance.PowerChanel_AA = Convert.ToInt32(SelectedString);
                    SerializerManager<Config>.Instance.Save(AppConfig.ConfigFileName, Config.Instance);
                    SerializerManager<Position>.Instance.Save(AppConfig.ConfigPositionName, Position.Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存异常:" + ex.ToString());
            }
        }

        private void btnAADoing_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("是否立即生效", "是否立即生效", MessageBoxButtons.YesNo))
                {

                    if (this.aaPort.IsOpen)
                    {
                        aaPort.Write("SYST:REM" + Environment.NewLine);
                        Thread.Sleep(50);
                        aaPort.Write($"INST CH{Config.Instance.PowerChanel_AA}" + Environment.NewLine);
                        aaPort.Write("VOLT " + Position.Instance.Voltage_AA.ToString() + Environment.NewLine);
                        Thread.Sleep(50);
                        aaPort.Write("CURR " + Position.Instance.Current_AA.ToString() + Environment.NewLine);
                        Thread.Sleep(50);

                      

                        this.aaPort.Write("OUTP 1" + Environment.NewLine);

                    }
                    else
                    {
                        MessageBox.Show("端口已断开");
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("立即生效异常:" + ex.ToString());
            }
        }

        private void AAPowerChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnMj_Click(object sender, EventArgs e)
        {
            if (this.wbPort.IsOpen)
            {
                wbPort.Write("SYST:REM" + Environment.NewLine);
                Thread.Sleep(50);
                wbPort.Write("SYSTem:BEEPer" + Environment.NewLine);
                Thread.Sleep(50);              
            }
        }
    }
}

