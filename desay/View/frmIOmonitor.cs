using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Motion.Interfaces;
using desay.ProductData;
namespace desay
{
    public partial class frmIOmonitor : Form
    {
        private IoPoint[] Input;
        private IoPoint[] Output;
        public frmIOmonitor()
        {
            InitializeComponent();
        }
        private void frmIOmonitor_Load(object sender, EventArgs e)
        {
            Input = new IoPoint[]
            {
                IoPoints.TDI0,
                IoPoints.TDI1,IoPoints.TDI2,
                //IoPoints.TDI3,IoPoints.TDI4,
                //IoPoints.TDI5,IoPoints.TDI6,IoPoints.TDI7,IoPoints.TDI8,
                //IoPoints.TDI9,IoPoints.TDI10,IoPoints.TDI11,IoPoints.TDI12,
                //IoPoints.TDI13,IoPoints.TDI14,IoPoints.TDI15,
                IoPoints.IDI0,IoPoints.IDI1,IoPoints.IDI2,IoPoints.IDI3,
                IoPoints.IDI4,IoPoints.IDI5,IoPoints.IDI6,IoPoints.IDI7,
                IoPoints.IDI8,IoPoints.IDI9,IoPoints.IDI10,IoPoints.IDI11,
                IoPoints.IDI12,IoPoints.IDI13,IoPoints.IDI14,IoPoints.IDI15,
                IoPoints.IDI16,IoPoints.IDI17,IoPoints.IDI18,IoPoints.IDI19,
                IoPoints.IDI20,IoPoints.IDI21,IoPoints.IDI22,IoPoints.IDI23,
                IoPoints.IDI24,IoPoints.IDI25,IoPoints.IDI26,IoPoints.IDI27,
                IoPoints.IDI28,IoPoints.IDI29,IoPoints.IDI30,IoPoints.IDI31,
                IoPoints.IDI90,IoPoints.IDI91,IoPoints.IDI92,IoPoints.IDI93,
                IoPoints.IDI94,IoPoints.IDI95,IoPoints.IDI96,IoPoints.IDI97,
                IoPoints.IDI98,IoPoints.IDI99,IoPoints.IDI910,IoPoints.IDI911,
                IoPoints.IDI912,IoPoints.IDI913,IoPoints.IDI914,IoPoints.IDI915,
                IoPoints.IDI916,IoPoints.IDI917,IoPoints.IDI918,IoPoints.IDI919,
                IoPoints.IDI920,IoPoints.IDI921,IoPoints.IDI922,IoPoints.IDI923,
                IoPoints.IDI924,IoPoints.IDI925,IoPoints.IDI926,IoPoints.IDI927,
                IoPoints.IDI928,IoPoints.IDI929,IoPoints.IDI930,IoPoints.IDI931
            };
            Output = new IoPoint[]
            {
                IoPoints.TDO7,IoPoints.IDO0,IoPoints.IDO1,IoPoints.IDO2,IoPoints.IDO3,
                IoPoints.IDO4,IoPoints.IDO5,IoPoints.IDO6,IoPoints.IDO7,
                IoPoints.IDO8,IoPoints.IDO9,IoPoints.IDO10,IoPoints.IDO11,
                IoPoints.IDO12,IoPoints.IDO13,IoPoints.IDO14,IoPoints.IDO15,
                IoPoints.IDO16,IoPoints.IDO17,IoPoints.IDO18,IoPoints.IDO19,
                IoPoints.IDO20,IoPoints.IDO21,IoPoints.IDO22,IoPoints.IDO23,
                IoPoints.IDO24,IoPoints.IDO25,IoPoints.IDO26,IoPoints.IDO27,
                IoPoints.IDO28,IoPoints.IDO29,IoPoints.IDO30,IoPoints.IDO31,
                IoPoints.IDO90,IoPoints.IDO91,IoPoints.IDO92,IoPoints.IDO93,
                IoPoints.IDO94,IoPoints.IDO95,IoPoints.IDO96,IoPoints.IDO97,
                IoPoints.IDO98,IoPoints.IDO99,IoPoints.IDO910,IoPoints.IDO911,
                IoPoints.IDO912,IoPoints.IDO913,IoPoints.IDO914,IoPoints.IDO915,
                IoPoints.IDO916,IoPoints.IDO917,IoPoints.IDO918,IoPoints.IDO919,
                IoPoints.IDO920,IoPoints.IDO921,IoPoints.IDO922,IoPoints.IDO923,
                IoPoints.IDO924,IoPoints.IDO925,IoPoints.IDO926,IoPoints.IDO927,
                IoPoints.IDO928,IoPoints.IDO929,IoPoints.IDO930,IoPoints.IDO931,IoPoints.TDO15
            };
            InitdgvInputViewRows();
            InitdgvOutputViewRows();
            timer1.Enabled = true;
        }
        private void InitdgvInputViewRows()
        {
            this.dgvInputView.Rows.Clear();
            //in a real scenario, you may need to add different rows
            var i = 1;
            foreach (var di in Input)
            {
                dgvInputView.Rows.Add(new object[] {
                    i.ToString(),
                    di.Value?Properties.Resources.LedGreen:Properties.Resources.LedNone,
                    di.Name,
                    di.Description
                });
                i++;
            }
        }
        private void InitdgvOutputViewRows()
        {
            this.dgvOutputView.Rows.Clear();
            //in a real scenario, you may need to add different rows
            var i = 1;
            foreach (var DO in Output)
            {
                dgvOutputView.Rows.Add(new object[] {
                    i.ToString(),
                    DO.Value?Properties.Resources.LedGreen:Properties.Resources.LedNone,
                    DO.Name,
                    DO.Description
                });
                i++;
            }
        }
        private void refreshdgvInputViewRows()
        {
            //in a real scenario, you may need to add different rows
            var i = 1;
            foreach (var DI in Input)
            {
                dgvInputView.Rows[i-1].SetValues(new object[] {
                    i.ToString(),
                    DI.Value?Properties.Resources.LedGreen:Properties.Resources.LedNone,
                    DI.Name,
                    DI.Description
                });
                i++;
            }
        }
        private void refreshdgvOutputViewRows()
        {
            //in a real scenario, you may need to add different rows
            var i = 1;
            foreach (var DO in Output)
            {
                dgvOutputView.Rows[i - 1].SetValues(new object[] {
                    i.ToString(),
                    DO.Value?Properties.Resources.LedGreen:Properties.Resources.LedNone,
                    DO.Name,
                    DO.Description
                });
                i++;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            refreshdgvInputViewRows();
            refreshdgvOutputViewRows();
            timer1.Enabled = true;
        }
    }
}
