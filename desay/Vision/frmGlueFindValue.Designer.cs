namespace desay.Vision
{
    partial class frmGlueFindValue
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
            this.cbPowImg = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudpowimage = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudPH_Sigama = new System.Windows.Forms.NumericUpDown();
            this.nudmedianimage = new System.Windows.Forms.NumericUpDown();
            this.nudabsdisimage = new System.Windows.Forms.NumericUpDown();
            this.nudGmin = new System.Windows.Forms.NumericUpDown();
            this.nudPH_Threshold = new System.Windows.Forms.NumericUpDown();
            this.nudfillupvalue = new System.Windows.Forms.NumericUpDown();
            this.nudTolenrance = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.nudSub1 = new System.Windows.Forms.NumericUpDown();
            this.nudSubT = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudpowimage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPH_Sigama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudmedianimage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudabsdisimage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPH_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudfillupvalue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolenrance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSub1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSubT)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPowImg);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudpowimage);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudPH_Sigama);
            this.groupBox1.Controls.Add(this.nudmedianimage);
            this.groupBox1.Controls.Add(this.nudSubT);
            this.groupBox1.Controls.Add(this.nudSub1);
            this.groupBox1.Controls.Add(this.nudabsdisimage);
            this.groupBox1.Controls.Add(this.nudGmin);
            this.groupBox1.Controls.Add(this.nudPH_Threshold);
            this.groupBox1.Controls.Add(this.nudfillupvalue);
            this.groupBox1.Controls.Add(this.nudTolenrance);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 329);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "胶水识别高级参数";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // cbPowImg
            // 
            this.cbPowImg.AutoSize = true;
            this.cbPowImg.Location = new System.Drawing.Point(248, 173);
            this.cbPowImg.Name = "cbPowImg";
            this.cbPowImg.Size = new System.Drawing.Size(174, 16);
            this.cbPowImg.TabIndex = 66;
            this.cbPowImg.Text = "启用Scale增强默认PowerImg";
            this.cbPowImg.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(181, 314);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 12);
            this.label4.TabIndex = 65;
            this.label4.Text = "此处为软件工程师调参界面，请勿随意更改参数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 65;
            this.label3.Text = "增强值";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-2, 255);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(137, 12);
            this.label8.TabIndex = 65;
            this.label8.Text = "PointsHarris_Threshold";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-2, 223);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 12);
            this.label7.TabIndex = 65;
            this.label7.Text = "PointsHarris_sigama";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 65;
            this.label2.Text = "减法值";
            // 
            // nudpowimage
            // 
            this.nudpowimage.DecimalPlaces = 1;
            this.nudpowimage.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudpowimage.Location = new System.Drawing.Point(142, 95);
            this.nudpowimage.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudpowimage.Name = "nudpowimage";
            this.nudpowimage.Size = new System.Drawing.Size(75, 21);
            this.nudpowimage.TabIndex = 64;
            this.nudpowimage.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 65;
            this.label5.Text = "中值滤波";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(246, 219);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 65;
            this.label9.Text = "Scale_Gmin";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 65;
            this.label6.Text = "填充参数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 65;
            this.label1.Text = "Tolenrance ";
            // 
            // nudPH_Sigama
            // 
            this.nudPH_Sigama.DecimalPlaces = 1;
            this.nudPH_Sigama.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudPH_Sigama.Location = new System.Drawing.Point(143, 214);
            this.nudPH_Sigama.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPH_Sigama.Name = "nudPH_Sigama";
            this.nudPH_Sigama.Size = new System.Drawing.Size(75, 21);
            this.nudPH_Sigama.TabIndex = 64;
            this.nudPH_Sigama.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            // 
            // nudmedianimage
            // 
            this.nudmedianimage.Location = new System.Drawing.Point(143, 134);
            this.nudmedianimage.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudmedianimage.Name = "nudmedianimage";
            this.nudmedianimage.Size = new System.Drawing.Size(75, 21);
            this.nudmedianimage.TabIndex = 64;
            this.nudmedianimage.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // nudabsdisimage
            // 
            this.nudabsdisimage.DecimalPlaces = 1;
            this.nudabsdisimage.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudabsdisimage.Location = new System.Drawing.Point(143, 60);
            this.nudabsdisimage.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudabsdisimage.Name = "nudabsdisimage";
            this.nudabsdisimage.Size = new System.Drawing.Size(75, 21);
            this.nudabsdisimage.TabIndex = 64;
            this.nudabsdisimage.Value = new decimal(new int[] {
            21,
            0,
            0,
            65536});
            // 
            // nudGmin
            // 
            this.nudGmin.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudGmin.Location = new System.Drawing.Point(321, 211);
            this.nudGmin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGmin.Name = "nudGmin";
            this.nudGmin.Size = new System.Drawing.Size(75, 21);
            this.nudGmin.TabIndex = 64;
            this.nudGmin.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudPH_Threshold
            // 
            this.nudPH_Threshold.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPH_Threshold.Location = new System.Drawing.Point(143, 253);
            this.nudPH_Threshold.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudPH_Threshold.Name = "nudPH_Threshold";
            this.nudPH_Threshold.Size = new System.Drawing.Size(75, 21);
            this.nudPH_Threshold.TabIndex = 64;
            this.nudPH_Threshold.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // nudfillupvalue
            // 
            this.nudfillupvalue.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudfillupvalue.Location = new System.Drawing.Point(142, 173);
            this.nudfillupvalue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudfillupvalue.Name = "nudfillupvalue";
            this.nudfillupvalue.Size = new System.Drawing.Size(75, 21);
            this.nudfillupvalue.TabIndex = 64;
            this.nudfillupvalue.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // nudTolenrance
            // 
            this.nudTolenrance.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTolenrance.Location = new System.Drawing.Point(143, 24);
            this.nudTolenrance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTolenrance.Name = "nudTolenrance";
            this.nudTolenrance.Size = new System.Drawing.Size(75, 21);
            this.nudTolenrance.TabIndex = 64;
            this.nudTolenrance.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(357, 20);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // nudSub1
            // 
            this.nudSub1.DecimalPlaces = 1;
            this.nudSub1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudSub1.Location = new System.Drawing.Point(273, 60);
            this.nudSub1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSub1.Name = "nudSub1";
            this.nudSub1.Size = new System.Drawing.Size(53, 21);
            this.nudSub1.TabIndex = 64;
            this.nudSub1.Value = new decimal(new int[] {
            12,
            0,
            0,
            65536});
            // 
            // nudSubT
            // 
            this.nudSubT.Location = new System.Drawing.Point(379, 60);
            this.nudSubT.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSubT.Name = "nudSubT";
            this.nudSubT.Size = new System.Drawing.Size(53, 21);
            this.nudSubT.TabIndex = 64;
            this.nudSubT.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(224, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 65;
            this.label10.Text = "sub值";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(335, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 65;
            this.label11.Text = "SubT值";
            // 
            // frmGlueFindValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 353);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmGlueFindValue";
            this.Text = "frmGlueFindValue";
            this.Load += new System.EventHandler(this.frmGlueFindValue_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudpowimage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPH_Sigama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudmedianimage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudabsdisimage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPH_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudfillupvalue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolenrance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSub1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSubT)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.NumericUpDown nudTolenrance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudabsdisimage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudpowimage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudmedianimage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudfillupvalue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudPH_Sigama;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudPH_Threshold;
        private System.Windows.Forms.CheckBox cbPowImg;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudGmin;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudSubT;
        private System.Windows.Forms.NumericUpDown nudSub1;
    }
}