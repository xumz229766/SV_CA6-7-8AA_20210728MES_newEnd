namespace desay
{
    partial class frmCylinderDelay
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
            this.flpCylinderPlasma = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flpCylinderGlue = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flpCylinderAA = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpCylinderPlasma
            // 
            this.flpCylinderPlasma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCylinderPlasma.Location = new System.Drawing.Point(2, 16);
            this.flpCylinderPlasma.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpCylinderPlasma.Name = "flpCylinderPlasma";
            this.flpCylinderPlasma.Size = new System.Drawing.Size(834, 123);
            this.flpCylinderPlasma.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flpCylinderPlasma);
            this.groupBox1.Location = new System.Drawing.Point(11, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(838, 141);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 496);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(62, 34);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flpCylinderGlue);
            this.groupBox2.Location = new System.Drawing.Point(9, 156);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(838, 122);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // flpCylinderGlue
            // 
            this.flpCylinderGlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCylinderGlue.Location = new System.Drawing.Point(2, 16);
            this.flpCylinderGlue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpCylinderGlue.Name = "flpCylinderGlue";
            this.flpCylinderGlue.Size = new System.Drawing.Size(834, 104);
            this.flpCylinderGlue.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.flpCylinderAA);
            this.groupBox3.Location = new System.Drawing.Point(6, 282);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(838, 210);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // flpCylinderAA
            // 
            this.flpCylinderAA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCylinderAA.Location = new System.Drawing.Point(2, 16);
            this.flpCylinderAA.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpCylinderAA.Name = "flpCylinderAA";
            this.flpCylinderAA.Size = new System.Drawing.Size(834, 192);
            this.flpCylinderAA.TabIndex = 0;
            // 
            // frmCylinderDelay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 535);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmCylinderDelay";
            this.Text = "气缸延时设置";
            this.Load += new System.EventHandler(this.frmCylinderDelay_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCylinderPlasma;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flpCylinderGlue;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.FlowLayoutPanel flpCylinderAA;
    }
}