namespace desay.Vision
{
    partial class frmRealPosCalib
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.cbManuGetPos = new System.Windows.Forms.CheckBox();
            this.lblVerify = new System.Windows.Forms.Label();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvImgPos = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvRealPos = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.机械手坐标显示 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLXCurrentpos = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.ImageX = new System.Windows.Forms.TextBox();
            this.ImageY = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.RealY = new System.Windows.Forms.TextBox();
            this.RealX = new System.Windows.Forms.TextBox();
            this.lbImageRealParam2 = new System.Windows.Forms.Label();
            this.lbImageRealParam = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGetRobotPos = new CCWin.SkinControl.SkinButton();
            this.btnRestDgvData = new CCWin.SkinControl.SkinButton();
            this.skinButton9 = new CCWin.SkinControl.SkinButton();
            this.btnCaculate = new CCWin.SkinControl.SkinButton();
            this.btnVerify = new CCWin.SkinControl.SkinButton();
            this.btnSave = new CCWin.SkinControl.SkinButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.skinButton2 = new CCWin.SkinControl.SkinButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImgPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRealPos)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            this.机械手坐标显示.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1336, 978);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.cbManuGetPos);
            this.panel1.Controls.Add(this.lblVerify);
            this.panel1.Controls.Add(this.hWindowControl1);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Controls.Add(this.flowLayoutPanel3);
            this.panel1.Controls.Add(this.tableLayoutPanel7);
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1320, 974);
            this.panel1.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(6, 627);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(154, 24);
            this.label12.TabIndex = 12;
            this.label12.Text = "标定四个角点";
            // 
            // cbManuGetPos
            // 
            this.cbManuGetPos.AutoSize = true;
            this.cbManuGetPos.Location = new System.Drawing.Point(768, 769);
            this.cbManuGetPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbManuGetPos.Name = "cbManuGetPos";
            this.cbManuGetPos.Size = new System.Drawing.Size(223, 22);
            this.cbManuGetPos.TabIndex = 12;
            this.cbManuGetPos.Text = "手动输入/自动获取坐标";
            this.cbManuGetPos.UseVisualStyleBackColor = true;
            // 
            // lblVerify
            // 
            this.lblVerify.AutoSize = true;
            this.lblVerify.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblVerify.Location = new System.Drawing.Point(1018, 767);
            this.lblVerify.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVerify.Name = "lblVerify";
            this.lblVerify.Size = new System.Drawing.Size(82, 24);
            this.lblVerify.TabIndex = 12;
            this.lblVerify.Text = "验证：";
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(0, 3);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(765, 608);
            this.hWindowControl1.TabIndex = 8;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(765, 608);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.dgvImgPos);
            this.flowLayoutPanel2.Controls.Add(this.dgvRealPos);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(766, 4);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(510, 606);
            this.flowLayoutPanel2.TabIndex = 10;
            // 
            // dgvImgPos
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvImgPos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvImgPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImgPos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.dgvImgPos.Location = new System.Drawing.Point(3, 3);
            this.dgvImgPos.Name = "dgvImgPos";
            this.dgvImgPos.RowHeadersVisible = false;
            this.dgvImgPos.RowTemplate.Height = 27;
            this.dgvImgPos.Size = new System.Drawing.Size(507, 322);
            this.dgvImgPos.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn1.FillWeight = 70F;
            this.dataGridViewTextBoxColumn1.HeaderText = "序号";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn2.FillWeight = 70F;
            this.dataGridViewTextBoxColumn2.HeaderText = "X像素坐标";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn3.FillWeight = 70F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Y像素坐标";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dgvRealPos
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRealPos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dgvRealPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRealPos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgvRealPos.Location = new System.Drawing.Point(3, 331);
            this.dgvRealPos.Name = "dgvRealPos";
            this.dgvRealPos.RowHeadersVisible = false;
            this.dgvRealPos.RowTemplate.Height = 27;
            this.dgvRealPos.Size = new System.Drawing.Size(507, 255);
            this.dgvRealPos.TabIndex = 5;
            // 
            // Column1
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle14;
            this.Column1.FillWeight = 70F;
            this.Column1.HeaderText = "序号";
            this.Column1.Name = "Column1";
            this.Column1.Width = 40;
            // 
            // Column2
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle15;
            this.Column2.FillWeight = 70F;
            this.Column2.HeaderText = "X机械坐标";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle16;
            this.Column3.FillWeight = 70F;
            this.Column3.HeaderText = "Y机械坐标";
            this.Column3.Name = "Column3";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.机械手坐标显示);
            this.flowLayoutPanel3.Controls.Add(this.groupBox2);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(6, 662);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(758, 308);
            this.flowLayoutPanel3.TabIndex = 11;
            // 
            // 机械手坐标显示
            // 
            this.机械手坐标显示.Controls.Add(this.label5);
            this.机械手坐标显示.Controls.Add(this.label3);
            this.机械手坐标显示.Controls.Add(this.label1);
            this.机械手坐标显示.Controls.Add(this.label4);
            this.机械手坐标显示.Controls.Add(this.label2);
            this.机械手坐标显示.Controls.Add(this.lblLXCurrentpos);
            this.机械手坐标显示.Location = new System.Drawing.Point(4, 4);
            this.机械手坐标显示.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.机械手坐标显示.Name = "机械手坐标显示";
            this.机械手坐标显示.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.机械手坐标显示.Size = new System.Drawing.Size(300, 290);
            this.机械手坐标显示.TabIndex = 0;
            this.机械手坐标显示.TabStop = false;
            this.机械手坐标显示.Text = "机械手坐标显示";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 140);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 18);
            this.label5.TabIndex = 12;
            this.label5.Text = "Z";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 96);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 18);
            this.label3.TabIndex = 12;
            this.label3.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "X ";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Black;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.SpringGreen;
            this.label4.Location = new System.Drawing.Point(112, 124);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 44);
            this.label4.TabIndex = 3;
            this.label4.Text = "0000.000";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.SpringGreen;
            this.label2.Location = new System.Drawing.Point(112, 81);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 44);
            this.label2.TabIndex = 3;
            this.label2.Text = "0000.000";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLXCurrentpos
            // 
            this.lblLXCurrentpos.BackColor = System.Drawing.Color.Black;
            this.lblLXCurrentpos.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLXCurrentpos.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblLXCurrentpos.Location = new System.Drawing.Point(112, 38);
            this.lblLXCurrentpos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLXCurrentpos.Name = "lblLXCurrentpos";
            this.lblLXCurrentpos.Size = new System.Drawing.Size(129, 44);
            this.lblLXCurrentpos.TabIndex = 3;
            this.lblLXCurrentpos.Text = "0000.000";
            this.lblLXCurrentpos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox10);
            this.groupBox2.Controls.Add(this.lbImageRealParam2);
            this.groupBox2.Controls.Add(this.lbImageRealParam);
            this.groupBox2.Location = new System.Drawing.Point(312, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(417, 290);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "矩阵显示";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.ImageX);
            this.groupBox10.Controls.Add(this.ImageY);
            this.groupBox10.Controls.Add(this.label11);
            this.groupBox10.Controls.Add(this.label9);
            this.groupBox10.Controls.Add(this.label10);
            this.groupBox10.Controls.Add(this.label7);
            this.groupBox10.Controls.Add(this.label8);
            this.groupBox10.Controls.Add(this.textBox2);
            this.groupBox10.Controls.Add(this.label6);
            this.groupBox10.Controls.Add(this.textBox1);
            this.groupBox10.Controls.Add(this.RealY);
            this.groupBox10.Controls.Add(this.RealX);
            this.groupBox10.Location = new System.Drawing.Point(9, 24);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Size = new System.Drawing.Size(399, 172);
            this.groupBox10.TabIndex = 13;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "手动输入";
            // 
            // ImageX
            // 
            this.ImageX.Location = new System.Drawing.Point(74, 32);
            this.ImageX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ImageX.Name = "ImageX";
            this.ImageX.Size = new System.Drawing.Size(100, 28);
            this.ImageX.TabIndex = 1;
            // 
            // ImageY
            // 
            this.ImageY.Location = new System.Drawing.Point(255, 32);
            this.ImageY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ImageY.Name = "ImageY";
            this.ImageY.Size = new System.Drawing.Size(100, 28);
            this.ImageY.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(195, 126);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 18);
            this.label11.TabIndex = 12;
            this.label11.Text = "验证Y";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(195, 76);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 18);
            this.label9.TabIndex = 12;
            this.label9.Text = "RealY";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 132);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 18);
            this.label10.TabIndex = 12;
            this.label10.Text = "验证X ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(195, 36);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 18);
            this.label7.TabIndex = 12;
            this.label7.Text = "ImgY";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 82);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 18);
            this.label8.TabIndex = 12;
            this.label8.Text = "RealX ";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(255, 122);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 28);
            this.textBox2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 36);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 18);
            this.label6.TabIndex = 12;
            this.label6.Text = "ImgX ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(74, 122);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 28);
            this.textBox1.TabIndex = 1;
            // 
            // RealY
            // 
            this.RealY.Location = new System.Drawing.Point(255, 72);
            this.RealY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RealY.Name = "RealY";
            this.RealY.Size = new System.Drawing.Size(100, 28);
            this.RealY.TabIndex = 1;
            // 
            // RealX
            // 
            this.RealX.Location = new System.Drawing.Point(74, 72);
            this.RealX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RealX.Name = "RealX";
            this.RealX.Size = new System.Drawing.Size(100, 28);
            this.RealX.TabIndex = 1;
            // 
            // lbImageRealParam2
            // 
            this.lbImageRealParam2.AutoSize = true;
            this.lbImageRealParam2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbImageRealParam2.Location = new System.Drawing.Point(10, 244);
            this.lbImageRealParam2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbImageRealParam2.Name = "lbImageRealParam2";
            this.lbImageRealParam2.Size = new System.Drawing.Size(58, 24);
            this.lbImageRealParam2.TabIndex = 12;
            this.lbImageRealParam2.Text = "矩阵";
            // 
            // lbImageRealParam
            // 
            this.lbImageRealParam.AutoSize = true;
            this.lbImageRealParam.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbImageRealParam.Location = new System.Drawing.Point(10, 202);
            this.lbImageRealParam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbImageRealParam.Name = "lbImageRealParam";
            this.lbImageRealParam.Size = new System.Drawing.Size(58, 24);
            this.lbImageRealParam.TabIndex = 12;
            this.lbImageRealParam.Text = "矩阵";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.btnGetRobotPos, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnRestDgvData, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.skinButton9, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.btnCaculate, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.btnVerify, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.btnSave, 1, 1);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(768, 608);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.73418F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.26582F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(500, 151);
            this.tableLayoutPanel7.TabIndex = 9;
            // 
            // btnGetRobotPos
            // 
            this.btnGetRobotPos.BackColor = System.Drawing.Color.Transparent;
            this.btnGetRobotPos.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnGetRobotPos.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnGetRobotPos.DownBack = null;
            this.btnGetRobotPos.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetRobotPos.ForeColor = System.Drawing.Color.Black;
            this.btnGetRobotPos.Location = new System.Drawing.Point(4, 4);
            this.btnGetRobotPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGetRobotPos.MouseBack = null;
            this.btnGetRobotPos.Name = "btnGetRobotPos";
            this.btnGetRobotPos.NormlBack = null;
            this.btnGetRobotPos.Size = new System.Drawing.Size(196, 40);
            this.btnGetRobotPos.TabIndex = 14;
            this.btnGetRobotPos.Text = "获取机械及图像坐标";
            this.btnGetRobotPos.UseVisualStyleBackColor = false;
            this.btnGetRobotPos.Click += new System.EventHandler(this.btnGetRobotPos_Click);
            // 
            // btnRestDgvData
            // 
            this.btnRestDgvData.BackColor = System.Drawing.Color.Transparent;
            this.btnRestDgvData.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnRestDgvData.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnRestDgvData.DownBack = null;
            this.btnRestDgvData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRestDgvData.ForeColor = System.Drawing.Color.Black;
            this.btnRestDgvData.Location = new System.Drawing.Point(254, 4);
            this.btnRestDgvData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRestDgvData.MouseBack = null;
            this.btnRestDgvData.Name = "btnRestDgvData";
            this.btnRestDgvData.NormlBack = null;
            this.btnRestDgvData.Size = new System.Drawing.Size(138, 40);
            this.btnRestDgvData.TabIndex = 14;
            this.btnRestDgvData.Text = "点位数据重置";
            this.btnRestDgvData.UseVisualStyleBackColor = false;
            this.btnRestDgvData.Click += new System.EventHandler(this.btnRestDgvData_Click);
            // 
            // skinButton9
            // 
            this.skinButton9.BackColor = System.Drawing.Color.Transparent;
            this.skinButton9.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.skinButton9.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton9.DownBack = null;
            this.skinButton9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton9.ForeColor = System.Drawing.Color.Black;
            this.skinButton9.Location = new System.Drawing.Point(4, 52);
            this.skinButton9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.skinButton9.MouseBack = null;
            this.skinButton9.Name = "skinButton9";
            this.skinButton9.NormlBack = null;
            this.skinButton9.Size = new System.Drawing.Size(138, 43);
            this.skinButton9.TabIndex = 14;
            this.skinButton9.Text = "获取图像坐标";
            this.skinButton9.UseVisualStyleBackColor = false;
            // 
            // btnCaculate
            // 
            this.btnCaculate.BackColor = System.Drawing.Color.Transparent;
            this.btnCaculate.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnCaculate.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnCaculate.DownBack = null;
            this.btnCaculate.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCaculate.ForeColor = System.Drawing.Color.Black;
            this.btnCaculate.Location = new System.Drawing.Point(4, 103);
            this.btnCaculate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCaculate.MouseBack = null;
            this.btnCaculate.Name = "btnCaculate";
            this.btnCaculate.NormlBack = null;
            this.btnCaculate.Size = new System.Drawing.Size(138, 44);
            this.btnCaculate.TabIndex = 14;
            this.btnCaculate.Text = "标定计算";
            this.btnCaculate.UseVisualStyleBackColor = false;
            this.btnCaculate.Click += new System.EventHandler(this.btnCaculate_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.BackColor = System.Drawing.Color.Transparent;
            this.btnVerify.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnVerify.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnVerify.DownBack = null;
            this.btnVerify.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnVerify.ForeColor = System.Drawing.Color.Black;
            this.btnVerify.Location = new System.Drawing.Point(254, 103);
            this.btnVerify.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnVerify.MouseBack = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.NormlBack = null;
            this.btnVerify.Size = new System.Drawing.Size(138, 44);
            this.btnVerify.TabIndex = 14;
            this.btnVerify.Text = "标定验证";
            this.btnVerify.UseVisualStyleBackColor = false;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnSave.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnSave.DownBack = null;
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(254, 52);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.MouseBack = null;
            this.btnSave.Name = "btnSave";
            this.btnSave.NormlBack = null;
            this.btnSave.Size = new System.Drawing.Size(138, 43);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.skinButton1);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.skinButton2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Location = new System.Drawing.Point(771, 790);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(532, 180);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机与机械手固定关系";
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DownBack = null;
            this.skinButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton1.ForeColor = System.Drawing.Color.Black;
            this.skinButton1.Location = new System.Drawing.Point(2, 26);
            this.skinButton1.Margin = new System.Windows.Forms.Padding(4);
            this.skinButton1.MouseBack = null;
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.Size = new System.Drawing.Size(166, 44);
            this.skinButton1.TabIndex = 14;
            this.skinButton1.Text = "获取机械扔料坐标";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.btnGetRobotPos_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(241, 36);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 28);
            this.textBox3.TabIndex = 1;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(422, 36);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 28);
            this.textBox4.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(176, 42);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 18);
            this.label13.TabIndex = 12;
            this.label13.Text = "RealX ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(362, 40);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 18);
            this.label14.TabIndex = 12;
            this.label14.Text = "RealY";
            // 
            // skinButton2
            // 
            this.skinButton2.BackColor = System.Drawing.Color.Transparent;
            this.skinButton2.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.skinButton2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton2.DownBack = null;
            this.skinButton2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton2.ForeColor = System.Drawing.Color.Black;
            this.skinButton2.Location = new System.Drawing.Point(2, 88);
            this.skinButton2.Margin = new System.Windows.Forms.Padding(4);
            this.skinButton2.MouseBack = null;
            this.skinButton2.Name = "skinButton2";
            this.skinButton2.NormlBack = null;
            this.skinButton2.Size = new System.Drawing.Size(166, 56);
            this.skinButton2.TabIndex = 14;
            this.skinButton2.Text = "相机拍照目标并计算坐标";
            this.skinButton2.UseVisualStyleBackColor = false;
            // 
            // frmRealPosCalib
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 978);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmRealPosCalib";
            this.Text = "图像坐标转世界坐标";
            this.Load += new System.EventHandler(this.frmRealPosCalib_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImgPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRealPos)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.机械手坐标显示.ResumeLayout(false);
            this.机械手坐标显示.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.DataGridView dgvRealPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridView dgvImgPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.GroupBox 机械手坐标显示;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private CCWin.SkinControl.SkinButton btnGetRobotPos;
        private CCWin.SkinControl.SkinButton btnRestDgvData;
        private CCWin.SkinControl.SkinButton skinButton9;
        private CCWin.SkinControl.SkinButton btnCaculate;
        private CCWin.SkinControl.SkinButton btnVerify;
        private System.Windows.Forms.Label lbImageRealParam;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblLXCurrentpos;
        private System.Windows.Forms.CheckBox cbManuGetPos;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox ImageX;
        private System.Windows.Forms.TextBox ImageY;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox RealY;
        private System.Windows.Forms.TextBox RealX;
        private System.Windows.Forms.Label lbImageRealParam2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblVerify;
        private CCWin.SkinControl.SkinButton btnSave;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private CCWin.SkinControl.SkinButton skinButton1;
        private System.Windows.Forms.Label label14;
        private CCWin.SkinControl.SkinButton skinButton2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label13;
    }
}