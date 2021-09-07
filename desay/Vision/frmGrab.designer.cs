namespace desay
{
    partial class frmGrab
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
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSingleFrame = new System.Windows.Forms.Button();
            this.btnLoadImg = new System.Windows.Forms.Button();
            this.btnSaveImg = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tkbExposure = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.tkbGain = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.tkbLight = new System.Windows.Forms.TrackBar();
            this.txtExposure = new System.Windows.Forms.TextBox();
            this.txtGain = new System.Windows.Forms.TextBox();
            this.txtLight = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSaveImg = new System.Windows.Forms.CheckBox();
            this.btnSaveParam = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkbExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkbGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkbLight)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(10, 60);
            this.hWindowControl1.Margin = new System.Windows.Forms.Padding(2);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(562, 449);
            this.hWindowControl1.TabIndex = 0;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(562, 449);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(585, 262);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(72, 30);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "连续采集";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(706, 262);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(70, 30);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "停止采集";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSingleFrame
            // 
            this.btnSingleFrame.Location = new System.Drawing.Point(828, 262);
            this.btnSingleFrame.Margin = new System.Windows.Forms.Padding(2);
            this.btnSingleFrame.Name = "btnSingleFrame";
            this.btnSingleFrame.Size = new System.Drawing.Size(64, 30);
            this.btnSingleFrame.TabIndex = 1;
            this.btnSingleFrame.Text = "单帧采集";
            this.btnSingleFrame.UseVisualStyleBackColor = true;
            this.btnSingleFrame.Click += new System.EventHandler(this.btnSingleFrame_Click);
            // 
            // btnLoadImg
            // 
            this.btnLoadImg.Location = new System.Drawing.Point(585, 318);
            this.btnLoadImg.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadImg.Name = "btnLoadImg";
            this.btnLoadImg.Size = new System.Drawing.Size(72, 30);
            this.btnLoadImg.TabIndex = 1;
            this.btnLoadImg.Text = "加载图像";
            this.btnLoadImg.UseVisualStyleBackColor = true;
            this.btnLoadImg.Click += new System.EventHandler(this.btnLoadImg_Click);
            // 
            // btnSaveImg
            // 
            this.btnSaveImg.Location = new System.Drawing.Point(706, 318);
            this.btnSaveImg.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveImg.Name = "btnSaveImg";
            this.btnSaveImg.Size = new System.Drawing.Size(70, 30);
            this.btnSaveImg.TabIndex = 1;
            this.btnSaveImg.Text = "保存图像";
            this.btnSaveImg.UseVisualStyleBackColor = true;
            this.btnSaveImg.Click += new System.EventHandler(this.btnSaveImg_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "型号：";
            // 
            // txtModelName
            // 
            this.txtModelName.Location = new System.Drawing.Point(58, 23);
            this.txtModelName.Margin = new System.Windows.Forms.Padding(2);
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.ReadOnly = true;
            this.txtModelName.Size = new System.Drawing.Size(77, 21);
            this.txtModelName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "序列号：";
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(202, 23);
            this.txtSN.Margin = new System.Windows.Forms.Padding(2);
            this.txtSN.Name = "txtSN";
            this.txtSN.ReadOnly = true;
            this.txtSN.Size = new System.Drawing.Size(77, 21);
            this.txtSN.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 27);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "用户名：";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(352, 23);
            this.txtUserID.Margin = new System.Windows.Forms.Padding(2);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(77, 21);
            this.txtUserID.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtUserID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSN);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtModelName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(438, 53);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机信息";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "曝光";
            // 
            // tkbExposure
            // 
            this.tkbExposure.Location = new System.Drawing.Point(63, 20);
            this.tkbExposure.Margin = new System.Windows.Forms.Padding(2);
            this.tkbExposure.Maximum = 100000;
            this.tkbExposure.Name = "tkbExposure";
            this.tkbExposure.Size = new System.Drawing.Size(176, 45);
            this.tkbExposure.TabIndex = 1;
            this.tkbExposure.Scroll += new System.EventHandler(this.tkbExposure_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 100);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "增益";
            // 
            // tkbGain
            // 
            this.tkbGain.Location = new System.Drawing.Point(62, 100);
            this.tkbGain.Margin = new System.Windows.Forms.Padding(2);
            this.tkbGain.Name = "tkbGain";
            this.tkbGain.Size = new System.Drawing.Size(177, 45);
            this.tkbGain.TabIndex = 1;
            this.tkbGain.Scroll += new System.EventHandler(this.tkbGain_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 171);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "光源亮度";
            // 
            // tkbLight
            // 
            this.tkbLight.Location = new System.Drawing.Point(62, 171);
            this.tkbLight.Margin = new System.Windows.Forms.Padding(2);
            this.tkbLight.Maximum = 255;
            this.tkbLight.Name = "tkbLight";
            this.tkbLight.Size = new System.Drawing.Size(177, 45);
            this.tkbLight.TabIndex = 1;
            this.tkbLight.Scroll += new System.EventHandler(this.tkbLight_Scroll);
            // 
            // txtExposure
            // 
            this.txtExposure.Location = new System.Drawing.Point(243, 16);
            this.txtExposure.Margin = new System.Windows.Forms.Padding(2);
            this.txtExposure.Name = "txtExposure";
            this.txtExposure.Size = new System.Drawing.Size(57, 21);
            this.txtExposure.TabIndex = 2;
            this.txtExposure.TextChanged += new System.EventHandler(this.txtExposure_TextChanged);
            // 
            // txtGain
            // 
            this.txtGain.Location = new System.Drawing.Point(243, 98);
            this.txtGain.Margin = new System.Windows.Forms.Padding(2);
            this.txtGain.Name = "txtGain";
            this.txtGain.Size = new System.Drawing.Size(57, 21);
            this.txtGain.TabIndex = 2;
            this.txtGain.TextChanged += new System.EventHandler(this.txtGain_TextChanged);
            // 
            // txtLight
            // 
            this.txtLight.Location = new System.Drawing.Point(243, 169);
            this.txtLight.Margin = new System.Windows.Forms.Padding(2);
            this.txtLight.Name = "txtLight";
            this.txtLight.Size = new System.Drawing.Size(57, 21);
            this.txtLight.TabIndex = 2;
            this.txtLight.TextChanged += new System.EventHandler(this.txtLight_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtLight);
            this.groupBox2.Controls.Add(this.txtGain);
            this.groupBox2.Controls.Add(this.txtExposure);
            this.groupBox2.Controls.Add(this.tkbLight);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tkbGain);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tkbExposure);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(585, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(307, 228);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "用户操作";
            // 
            // chkSaveImg
            // 
            this.chkSaveImg.AutoSize = true;
            this.chkSaveImg.Location = new System.Drawing.Point(585, 377);
            this.chkSaveImg.Margin = new System.Windows.Forms.Padding(2);
            this.chkSaveImg.Name = "chkSaveImg";
            this.chkSaveImg.Size = new System.Drawing.Size(96, 16);
            this.chkSaveImg.TabIndex = 5;
            this.chkSaveImg.Text = "是否保存图像";
            this.chkSaveImg.UseVisualStyleBackColor = true;
            // 
            // btnSaveParam
            // 
            this.btnSaveParam.Location = new System.Drawing.Point(828, 318);
            this.btnSaveParam.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveParam.Name = "btnSaveParam";
            this.btnSaveParam.Size = new System.Drawing.Size(64, 30);
            this.btnSaveParam.TabIndex = 1;
            this.btnSaveParam.Text = "保存参数";
            this.btnSaveParam.UseVisualStyleBackColor = true;
            this.btnSaveParam.Click += new System.EventHandler(this.btnSaveParam_Click);
            // 
            // frmGrab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 520);
            this.Controls.Add(this.chkSaveImg);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSaveParam);
            this.Controls.Add(this.btnSaveImg);
            this.Controls.Add(this.btnLoadImg);
            this.Controls.Add(this.btnSingleFrame);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.hWindowControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGrab";
            this.Text = "图像采集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGrab_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGrab_FormClosed);
            this.Load += new System.EventHandler(this.frmGrab_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkbExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkbGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkbLight)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSingleFrame;
        private System.Windows.Forms.Button btnLoadImg;
        private System.Windows.Forms.Button btnSaveImg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar tkbExposure;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar tkbGain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar tkbLight;
        private System.Windows.Forms.TextBox txtExposure;
        private System.Windows.Forms.TextBox txtGain;
        private System.Windows.Forms.TextBox txtLight;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkSaveImg;
        private System.Windows.Forms.Button btnSaveParam;
    }
}