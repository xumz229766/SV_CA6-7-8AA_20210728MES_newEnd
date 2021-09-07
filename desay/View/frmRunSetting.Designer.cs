namespace desay
{
    partial class frmRunSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunSetting));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CHB_SHELL_SENSOR_SHIELD = new System.Windows.Forms.CheckBox();
            this.CHB_LENS_SENSOR_SHIELD = new System.Windows.Forms.CheckBox();
            this.CHB_FORMER_STATION_SHIELD = new System.Windows.Forms.CheckBox();
            this.chkCarrierHaveProduct = new System.Windows.Forms.CheckBox();
            this.chkCleanHaveProduct = new System.Windows.Forms.CheckBox();
            this.chkSnScannerShield = new System.Windows.Forms.CheckBox();
            this.chkGHShield = new System.Windows.Forms.CheckBox();
            this.cbGlueHave = new System.Windows.Forms.CheckBox();
            this.chkCurtainShield = new System.Windows.Forms.CheckBox();
            this.cbPlasmaAlarmShield = new System.Windows.Forms.CheckBox();
            this.cbAAhomeShield = new System.Windows.Forms.CheckBox();
            this.chkDoorSheild = new System.Windows.Forms.CheckBox();
            this.chkGlueHaveProduct = new System.Windows.Forms.CheckBox();
            this.cbPlcReflashShield = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(118, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(258, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbPlcReflashShield);
            this.groupBox3.Controls.Add(this.CHB_SHELL_SENSOR_SHIELD);
            this.groupBox3.Controls.Add(this.CHB_LENS_SENSOR_SHIELD);
            this.groupBox3.Controls.Add(this.CHB_FORMER_STATION_SHIELD);
            this.groupBox3.Controls.Add(this.chkCarrierHaveProduct);
            this.groupBox3.Controls.Add(this.chkCleanHaveProduct);
            this.groupBox3.Controls.Add(this.chkSnScannerShield);
            this.groupBox3.Controls.Add(this.chkGHShield);
            this.groupBox3.Controls.Add(this.cbGlueHave);
            this.groupBox3.Controls.Add(this.chkCurtainShield);
            this.groupBox3.Controls.Add(this.cbPlasmaAlarmShield);
            this.groupBox3.Controls.Add(this.cbAAhomeShield);
            this.groupBox3.Controls.Add(this.chkDoorSheild);
            this.groupBox3.Controls.Add(this.chkGlueHaveProduct);
            this.groupBox3.Location = new System.Drawing.Point(6, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(336, 318);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "屏蔽设置";
            // 
            // CHB_SHELL_SENSOR_SHIELD
            // 
            this.CHB_SHELL_SENSOR_SHIELD.AutoSize = true;
            this.CHB_SHELL_SENSOR_SHIELD.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CHB_SHELL_SENSOR_SHIELD.Location = new System.Drawing.Point(197, 129);
            this.CHB_SHELL_SENSOR_SHIELD.Name = "CHB_SHELL_SENSOR_SHIELD";
            this.CHB_SHELL_SENSOR_SHIELD.Size = new System.Drawing.Size(123, 20);
            this.CHB_SHELL_SENSOR_SHIELD.TabIndex = 13;
            this.CHB_SHELL_SENSOR_SHIELD.Text = "屏蔽镜座感应";
            this.CHB_SHELL_SENSOR_SHIELD.UseVisualStyleBackColor = true;
            this.CHB_SHELL_SENSOR_SHIELD.Visible = false;
            // 
            // CHB_LENS_SENSOR_SHIELD
            // 
            this.CHB_LENS_SENSOR_SHIELD.AutoSize = true;
            this.CHB_LENS_SENSOR_SHIELD.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CHB_LENS_SENSOR_SHIELD.Location = new System.Drawing.Point(197, 94);
            this.CHB_LENS_SENSOR_SHIELD.Name = "CHB_LENS_SENSOR_SHIELD";
            this.CHB_LENS_SENSOR_SHIELD.Size = new System.Drawing.Size(123, 20);
            this.CHB_LENS_SENSOR_SHIELD.TabIndex = 12;
            this.CHB_LENS_SENSOR_SHIELD.Text = "屏蔽镜头感应";
            this.CHB_LENS_SENSOR_SHIELD.UseVisualStyleBackColor = true;
            this.CHB_LENS_SENSOR_SHIELD.Visible = false;
            // 
            // CHB_FORMER_STATION_SHIELD
            // 
            this.CHB_FORMER_STATION_SHIELD.AutoSize = true;
            this.CHB_FORMER_STATION_SHIELD.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CHB_FORMER_STATION_SHIELD.Location = new System.Drawing.Point(6, 56);
            this.CHB_FORMER_STATION_SHIELD.Name = "CHB_FORMER_STATION_SHIELD";
            this.CHB_FORMER_STATION_SHIELD.Size = new System.Drawing.Size(123, 20);
            this.CHB_FORMER_STATION_SHIELD.TabIndex = 11;
            this.CHB_FORMER_STATION_SHIELD.Text = "屏蔽上一工位";
            this.CHB_FORMER_STATION_SHIELD.UseVisualStyleBackColor = true;
            // 
            // chkCarrierHaveProduct
            // 
            this.chkCarrierHaveProduct.AutoSize = true;
            this.chkCarrierHaveProduct.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkCarrierHaveProduct.Location = new System.Drawing.Point(197, 168);
            this.chkCarrierHaveProduct.Name = "chkCarrierHaveProduct";
            this.chkCarrierHaveProduct.Size = new System.Drawing.Size(107, 20);
            this.chkCarrierHaveProduct.TabIndex = 10;
            this.chkCarrierHaveProduct.Text = "接驳台有料";
            this.chkCarrierHaveProduct.UseVisualStyleBackColor = true;
            this.chkCarrierHaveProduct.Visible = false;
            // 
            // chkCleanHaveProduct
            // 
            this.chkCleanHaveProduct.AutoSize = true;
            this.chkCleanHaveProduct.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkCleanHaveProduct.Location = new System.Drawing.Point(197, 20);
            this.chkCleanHaveProduct.Name = "chkCleanHaveProduct";
            this.chkCleanHaveProduct.Size = new System.Drawing.Size(123, 20);
            this.chkCleanHaveProduct.TabIndex = 9;
            this.chkCleanHaveProduct.Text = "清洗平台有料";
            this.chkCleanHaveProduct.UseVisualStyleBackColor = true;
            this.chkCleanHaveProduct.Visible = false;
            // 
            // chkSnScannerShield
            // 
            this.chkSnScannerShield.AutoSize = true;
            this.chkSnScannerShield.Checked = true;
            this.chkSnScannerShield.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSnScannerShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkSnScannerShield.Location = new System.Drawing.Point(197, 284);
            this.chkSnScannerShield.Name = "chkSnScannerShield";
            this.chkSnScannerShield.Size = new System.Drawing.Size(139, 20);
            this.chkSnScannerShield.TabIndex = 10;
            this.chkSnScannerShield.Text = "产品扫码枪屏蔽";
            this.chkSnScannerShield.UseVisualStyleBackColor = true;
            this.chkSnScannerShield.Visible = false;
            // 
            // chkGHShield
            // 
            this.chkGHShield.AutoSize = true;
            this.chkGHShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkGHShield.Location = new System.Drawing.Point(6, 20);
            this.chkGHShield.Name = "chkGHShield";
            this.chkGHShield.Size = new System.Drawing.Size(139, 20);
            this.chkGHShield.TabIndex = 10;
            this.chkGHShield.Text = "屏蔽下一个工位";
            this.chkGHShield.UseVisualStyleBackColor = true;
            // 
            // cbGlueHave
            // 
            this.cbGlueHave.AutoSize = true;
            this.cbGlueHave.Checked = true;
            this.cbGlueHave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGlueHave.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbGlueHave.Location = new System.Drawing.Point(6, 94);
            this.cbGlueHave.Name = "cbGlueHave";
            this.cbGlueHave.Size = new System.Drawing.Size(123, 20);
            this.cbGlueHave.TabIndex = 10;
            this.cbGlueHave.Text = "胶水液位屏蔽";
            this.cbGlueHave.UseVisualStyleBackColor = true;
            // 
            // chkCurtainShield
            // 
            this.chkCurtainShield.AutoSize = true;
            this.chkCurtainShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkCurtainShield.Location = new System.Drawing.Point(197, 248);
            this.chkCurtainShield.Name = "chkCurtainShield";
            this.chkCurtainShield.Size = new System.Drawing.Size(123, 20);
            this.chkCurtainShield.TabIndex = 10;
            this.chkCurtainShield.Text = "安全光幕屏蔽";
            this.chkCurtainShield.UseVisualStyleBackColor = true;
            this.chkCurtainShield.Visible = false;
            // 
            // cbPlasmaAlarmShield
            // 
            this.cbPlasmaAlarmShield.AutoSize = true;
            this.cbPlasmaAlarmShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPlasmaAlarmShield.Location = new System.Drawing.Point(6, 168);
            this.cbPlasmaAlarmShield.Name = "cbPlasmaAlarmShield";
            this.cbPlasmaAlarmShield.Size = new System.Drawing.Size(179, 20);
            this.cbPlasmaAlarmShield.TabIndex = 10;
            this.cbPlasmaAlarmShield.Text = "Plasama火焰报警屏蔽";
            this.cbPlasmaAlarmShield.UseVisualStyleBackColor = true;
            // 
            // cbAAhomeShield
            // 
            this.cbAAhomeShield.AutoSize = true;
            this.cbAAhomeShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbAAhomeShield.Location = new System.Drawing.Point(6, 129);
            this.cbAAhomeShield.Name = "cbAAhomeShield";
            this.cbAAhomeShield.Size = new System.Drawing.Size(107, 20);
            this.cbAAhomeShield.TabIndex = 10;
            this.cbAAhomeShield.Text = "AA回零屏蔽";
            this.cbAAhomeShield.UseVisualStyleBackColor = true;
            // 
            // chkDoorSheild
            // 
            this.chkDoorSheild.AutoSize = true;
            this.chkDoorSheild.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkDoorSheild.Location = new System.Drawing.Point(197, 60);
            this.chkDoorSheild.Name = "chkDoorSheild";
            this.chkDoorSheild.Size = new System.Drawing.Size(91, 20);
            this.chkDoorSheild.TabIndex = 10;
            this.chkDoorSheild.Text = "门禁屏蔽";
            this.chkDoorSheild.UseVisualStyleBackColor = true;
            this.chkDoorSheild.Visible = false;
            // 
            // chkGlueHaveProduct
            // 
            this.chkGlueHaveProduct.AutoSize = true;
            this.chkGlueHaveProduct.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkGlueHaveProduct.Location = new System.Drawing.Point(197, 208);
            this.chkGlueHaveProduct.Name = "chkGlueHaveProduct";
            this.chkGlueHaveProduct.Size = new System.Drawing.Size(123, 20);
            this.chkGlueHaveProduct.TabIndex = 10;
            this.chkGlueHaveProduct.Text = "点胶平台有料";
            this.chkGlueHaveProduct.UseVisualStyleBackColor = true;
            this.chkGlueHaveProduct.Visible = false;
            // 
            // cbPlcReflashShield
            // 
            this.cbPlcReflashShield.AutoSize = true;
            this.cbPlcReflashShield.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPlcReflashShield.Location = new System.Drawing.Point(6, 208);
            this.cbPlcReflashShield.Name = "cbPlcReflashShield";
            this.cbPlcReflashShield.Size = new System.Drawing.Size(147, 20);
            this.cbPlcReflashShield.TabIndex = 14;
            this.cbPlcReflashShield.Text = "PLC输出刷新屏蔽";
            this.cbPlcReflashShield.UseVisualStyleBackColor = true;
            // 
            // frmRunSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 359);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRunSetting";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "运行设置";
            this.Load += new System.EventHandler(this.frmRunSetting_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkCleanHaveProduct;
        private System.Windows.Forms.CheckBox chkGlueHaveProduct;
        private System.Windows.Forms.CheckBox chkCarrierHaveProduct;
        private System.Windows.Forms.CheckBox chkDoorSheild;
        private System.Windows.Forms.CheckBox chkCurtainShield;
        private System.Windows.Forms.CheckBox chkGHShield;
        private System.Windows.Forms.CheckBox chkSnScannerShield;
        private System.Windows.Forms.CheckBox CHB_FORMER_STATION_SHIELD;
        private System.Windows.Forms.CheckBox CHB_SHELL_SENSOR_SHIELD;
        private System.Windows.Forms.CheckBox CHB_LENS_SENSOR_SHIELD;
        private System.Windows.Forms.CheckBox cbGlueHave;
        private System.Windows.Forms.CheckBox cbAAhomeShield;
        private System.Windows.Forms.CheckBox cbPlasmaAlarmShield;
        private System.Windows.Forms.CheckBox cbPlcReflashShield;
    }
}