namespace desay
{
    partial class frmCommSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.WbPowerParam = new System.Device.SerialPortParam();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.TB_FormerStation_Ip = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TB_FormerStation_Port = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbLightControlp = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.WbPowerParam);
            this.groupBox1.Location = new System.Drawing.Point(4, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(191, 254);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "白板程控电源";
            // 
            // WbPowerParam
            // 
            this.WbPowerParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WbPowerParam.Location = new System.Drawing.Point(2, 16);
            this.WbPowerParam.Margin = new System.Windows.Forms.Padding(4);
            this.WbPowerParam.Name = "WbPowerParam";
            this.WbPowerParam.Size = new System.Drawing.Size(187, 236);
            this.WbPowerParam.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(358, 178);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(58, 30);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(358, 222);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "退出";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.TB_FormerStation_Ip);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.TB_FormerStation_Port);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.tbLightControlp);
            this.groupBox5.Location = new System.Drawing.Point(199, 26);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(236, 140);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "网口通讯端口";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 25);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "上料机IP地址：";
            // 
            // TB_FormerStation_Ip
            // 
            this.TB_FormerStation_Ip.Location = new System.Drawing.Point(95, 18);
            this.TB_FormerStation_Ip.Margin = new System.Windows.Forms.Padding(2);
            this.TB_FormerStation_Ip.Name = "TB_FormerStation_Ip";
            this.TB_FormerStation_Ip.Size = new System.Drawing.Size(110, 21);
            this.TB_FormerStation_Ip.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 66);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "上料机端口：";
            // 
            // TB_FormerStation_Port
            // 
            this.TB_FormerStation_Port.Location = new System.Drawing.Point(143, 63);
            this.TB_FormerStation_Port.Margin = new System.Windows.Forms.Padding(2);
            this.TB_FormerStation_Port.Name = "TB_FormerStation_Port";
            this.TB_FormerStation_Port.Size = new System.Drawing.Size(62, 21);
            this.TB_FormerStation_Port.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 106);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "光控地址：";
            // 
            // tbLightControlp
            // 
            this.tbLightControlp.Location = new System.Drawing.Point(95, 103);
            this.tbLightControlp.Margin = new System.Windows.Forms.Padding(2);
            this.tbLightControlp.Name = "tbLightControlp";
            this.tbLightControlp.Size = new System.Drawing.Size(110, 21);
            this.tbLightControlp.TabIndex = 1;
            // 
            // frmCommSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 268);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmCommSetting";
            this.Text = "通讯设置";
            this.Load += new System.EventHandler(this.frmCommSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Device.SerialPortParam WbPowerParam;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TB_FormerStation_Port;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TB_FormerStation_Ip;
        private System.Windows.Forms.TextBox tbLightControlp;
        private System.Windows.Forms.Label label12;
    }
}