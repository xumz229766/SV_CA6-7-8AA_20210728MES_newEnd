namespace desay
{
    partial class WhiteBoardPower
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
            this.btnWbDoing = new System.Windows.Forms.Button();
            this.btnSaveWb = new System.Windows.Forms.Button();
            this.mudWbV = new System.Windows.Forms.NumericUpDown();
            this.nudWbI = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.wbPowerChannel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAADoing = new System.Windows.Forms.Button();
            this.AAPowerChannel = new System.Windows.Forms.ComboBox();
            this.btnAAsave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nudAAV = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nudAAI = new System.Windows.Forms.NumericUpDown();
            this.btnMj = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mudWbV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWbI)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAAV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAAI)).BeginInit();
            this.SuspendLayout();
            // 
            // btnWbDoing
            // 
            this.btnWbDoing.Location = new System.Drawing.Point(101, 139);
            this.btnWbDoing.Name = "btnWbDoing";
            this.btnWbDoing.Size = new System.Drawing.Size(76, 43);
            this.btnWbDoing.TabIndex = 19;
            this.btnWbDoing.Text = "立即生效";
            this.btnWbDoing.UseVisualStyleBackColor = true;
            this.btnWbDoing.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnSaveWb
            // 
            this.btnSaveWb.Location = new System.Drawing.Point(18, 139);
            this.btnSaveWb.Name = "btnSaveWb";
            this.btnSaveWb.Size = new System.Drawing.Size(76, 43);
            this.btnSaveWb.TabIndex = 17;
            this.btnSaveWb.Text = "保存参数";
            this.btnSaveWb.UseVisualStyleBackColor = true;
            this.btnSaveWb.Click += new System.EventHandler(this.button1_Click);
            // 
            // mudWbV
            // 
            this.mudWbV.DecimalPlaces = 3;
            this.mudWbV.Location = new System.Drawing.Point(93, 105);
            this.mudWbV.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.mudWbV.Name = "mudWbV";
            this.mudWbV.Size = new System.Drawing.Size(79, 21);
            this.mudWbV.TabIndex = 16;
            // 
            // nudWbI
            // 
            this.nudWbI.DecimalPlaces = 3;
            this.nudWbI.Location = new System.Drawing.Point(94, 59);
            this.nudWbI.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudWbI.Name = "nudWbI";
            this.nudWbI.Size = new System.Drawing.Size(79, 21);
            this.nudWbI.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(15, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "电流";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(15, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "电压";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 16);
            this.label2.TabIndex = 12;
            // 
            // wbPowerChannel
            // 
            this.wbPowerChannel.AutoCompleteCustomSource.AddRange(new string[] {
            "1"});
            this.wbPowerChannel.FormatString = "1";
            this.wbPowerChannel.FormattingEnabled = true;
            this.wbPowerChannel.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.wbPowerChannel.Location = new System.Drawing.Point(93, 23);
            this.wbPowerChannel.Name = "wbPowerChannel";
            this.wbPowerChannel.Size = new System.Drawing.Size(80, 20);
            this.wbPowerChannel.TabIndex = 11;
            this.wbPowerChannel.Text = "1";
            this.wbPowerChannel.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "电源通道";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnWbDoing);
            this.groupBox1.Controls.Add(this.wbPowerChannel);
            this.groupBox1.Controls.Add(this.btnSaveWb);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.mudWbV);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nudWbI);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 191);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "白板程控电源设置";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnAADoing);
            this.groupBox2.Controls.Add(this.AAPowerChannel);
            this.groupBox2.Controls.Add(this.btnAAsave);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.nudAAV);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.nudAAI);
            this.groupBox2.Location = new System.Drawing.Point(237, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(205, 191);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AA程控电源设置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.Location = new System.Drawing.Point(15, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "电源通道";
            // 
            // btnAADoing
            // 
            this.btnAADoing.Location = new System.Drawing.Point(101, 139);
            this.btnAADoing.Name = "btnAADoing";
            this.btnAADoing.Size = new System.Drawing.Size(76, 43);
            this.btnAADoing.TabIndex = 19;
            this.btnAADoing.Text = "立即生效";
            this.btnAADoing.UseVisualStyleBackColor = true;
            this.btnAADoing.Click += new System.EventHandler(this.btnAADoing_Click);
            // 
            // AAPowerChannel
            // 
            this.AAPowerChannel.FormattingEnabled = true;
            this.AAPowerChannel.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.AAPowerChannel.Location = new System.Drawing.Point(93, 23);
            this.AAPowerChannel.Name = "AAPowerChannel";
            this.AAPowerChannel.Size = new System.Drawing.Size(80, 20);
            this.AAPowerChannel.TabIndex = 11;
            this.AAPowerChannel.Text = "2";
            this.AAPowerChannel.SelectedIndexChanged += new System.EventHandler(this.AAPowerChannel_SelectedIndexChanged);
            // 
            // btnAAsave
            // 
            this.btnAAsave.Location = new System.Drawing.Point(18, 139);
            this.btnAAsave.Name = "btnAAsave";
            this.btnAAsave.Size = new System.Drawing.Size(76, 43);
            this.btnAAsave.TabIndex = 17;
            this.btnAAsave.Text = "保存参数";
            this.btnAAsave.UseVisualStyleBackColor = true;
            this.btnAAsave.Click += new System.EventHandler(this.btnAAsave_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.Location = new System.Drawing.Point(15, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "电压";
            // 
            // nudAAV
            // 
            this.nudAAV.DecimalPlaces = 3;
            this.nudAAV.Location = new System.Drawing.Point(93, 105);
            this.nudAAV.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudAAV.Name = "nudAAV";
            this.nudAAV.Size = new System.Drawing.Size(79, 21);
            this.nudAAV.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.Location = new System.Drawing.Point(15, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "电流";
            // 
            // nudAAI
            // 
            this.nudAAI.DecimalPlaces = 3;
            this.nudAAI.Location = new System.Drawing.Point(94, 59);
            this.nudAAI.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAAI.Name = "nudAAI";
            this.nudAAI.Size = new System.Drawing.Size(79, 21);
            this.nudAAI.TabIndex = 15;
            // 
            // btnMj
            // 
            this.btnMj.Location = new System.Drawing.Point(113, 223);
            this.btnMj.Name = "btnMj";
            this.btnMj.Size = new System.Drawing.Size(72, 29);
            this.btnMj.TabIndex = 19;
            this.btnMj.Text = "程控鸣叫";
            this.btnMj.UseVisualStyleBackColor = true;
            this.btnMj.Click += new System.EventHandler(this.btnMj_Click);
            // 
            // WhiteBoardPower
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 263);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnMj);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Name = "WhiteBoardPower";
            this.Text = "WhiteBoardPower";
            this.Load += new System.EventHandler(this.WhiteBoardPower_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.mudWbV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWbI)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAAV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAAI)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnWbDoing;
        private System.Windows.Forms.Button btnSaveWb;
        private System.Windows.Forms.NumericUpDown mudWbV;
        private System.Windows.Forms.NumericUpDown nudWbI;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox wbPowerChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAADoing;
        private System.Windows.Forms.ComboBox AAPowerChannel;
        private System.Windows.Forms.Button btnAAsave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudAAV;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudAAI;
        private System.Windows.Forms.Button btnMj;
    }
}