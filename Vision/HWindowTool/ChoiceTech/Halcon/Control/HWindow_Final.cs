using HalconDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ChoiceTech.Halcon.Control
{
	public class HWindowFinal : UserControl
	{
		private bool drawModel = false;
		private IContainer components = (IContainer)null;
		private HWindow hv_window;
		private ContextMenuStrip hv_MenuStrip;
		private ToolStripMenuItem fit_strip;
		private ToolStripMenuItem saveImg_strip;
		private ToolStripMenuItem saveWindow_strip;
		private ToolStripMenuItem barVisible_strip;
		private ToolStripMenuItem histogram_strip;
		private HImage hv_image;
		private int hv_imageWidth;
		private int hv_imageHeight;
		private string str_imgSize;
		public ViewWindow.ViewWindow viewWindow;
		public HWindowControl hWindowControl;
		private Label m_CtrlHStatusLabelCtrl;
		private ImageList m_CtrlImageList;
		private HWindowControl mCtrl_HWindow;

		public bool DrawModel
		{
			get
			{
				return this.drawModel;
			}
			set
			{
				this.viewWindow.setDrawModel(value);
				if (value)
					((System.Windows.Forms.Control)this.mCtrl_HWindow).ContextMenuStrip = (ContextMenuStrip)null;
				else
					((System.Windows.Forms.Control)this.mCtrl_HWindow).ContextMenuStrip = this.hv_MenuStrip;
				this.drawModel = value;
			}
		}

		public HImage Image
		{
			get
			{
				return this.hv_image;
			}
			set
			{
				if (value == null)
					return;
				if (this.hv_image != null)
					((HObjectBase)this.hv_image).Dispose();
				this.hv_image = value;
				this.hv_image.GetImageSize(out this.hv_imageWidth, out this.hv_imageHeight);
				this.str_imgSize = string.Format("{0}X{1}", this.hv_imageWidth, this.hv_imageHeight);
				try
				{
					this.barVisible_strip.Enabled = true;
					this.fit_strip.Enabled = true;
					this.histogram_strip.Enabled = true;
					this.saveImg_strip.Enabled = true;
					this.saveWindow_strip.Enabled = true;
				}
				catch (Exception ex)
				{
				}
				this.viewWindow.displayImage((HObject)this.hv_image);
			}
		}

		public IntPtr HWindowHalconID
		{
			get
			{
				return this.mCtrl_HWindow.HalconID;
			}
		}

		public HWindowFinal()
		{
			this.InitializeComponent();
			this.viewWindow = new ViewWindow.ViewWindow(this.mCtrl_HWindow);
			this.hWindowControl = this.mCtrl_HWindow;
			this.hv_window = this.mCtrl_HWindow.HalconWindow;
			this.fit_strip = new ToolStripMenuItem("适应窗口");
			this.fit_strip.Click += (EventHandler)((s, e) => this.DispImageFit(this.mCtrl_HWindow));
			this.barVisible_strip = new ToolStripMenuItem("显示StatusBar");
			this.barVisible_strip.CheckOnClick = true;
			this.barVisible_strip.CheckedChanged += new EventHandler(this.barVisible_strip_CheckedChanged);
			this.m_CtrlHStatusLabelCtrl.Visible = false;
			((System.Windows.Forms.Control)this.mCtrl_HWindow).Height = this.Height;
			this.saveImg_strip = new ToolStripMenuItem("保存原始图像");
			this.saveImg_strip.Click += (EventHandler)((s, e) => this.SaveImage());
			this.saveWindow_strip = new ToolStripMenuItem("保存窗口缩略图");
			this.saveWindow_strip.Click += (EventHandler)((s, e) => this.SaveWindowDump());
			this.histogram_strip = new ToolStripMenuItem("显示直方图(H)");
			this.histogram_strip.CheckOnClick = true;
			this.histogram_strip.Checked = false;
			this.hv_MenuStrip = new ContextMenuStrip();
			this.hv_MenuStrip.Items.Add((ToolStripItem)this.fit_strip);
			this.hv_MenuStrip.Items.Add((ToolStripItem)this.barVisible_strip);
			this.hv_MenuStrip.Items.Add((ToolStripItem)new ToolStripSeparator());
			this.hv_MenuStrip.Items.Add((ToolStripItem)this.saveImg_strip);
			this.hv_MenuStrip.Items.Add((ToolStripItem)this.saveWindow_strip);
			this.barVisible_strip.Enabled = true;
			this.fit_strip.Enabled = false;
			this.histogram_strip.Enabled = false;
			this.saveImg_strip.Enabled = false;
			this.saveWindow_strip.Enabled = false;
			((System.Windows.Forms.Control)this.mCtrl_HWindow).ContextMenuStrip = this.hv_MenuStrip;
			((System.Windows.Forms.Control)this.mCtrl_HWindow).SizeChanged += (EventHandler)((s, e) => this.DispImageFit(this.mCtrl_HWindow));
		}

		public HWindowControl getHWindowControl()
		{
			return this.mCtrl_HWindow;
		}

		private void barVisible_strip_CheckedChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			this.SuspendLayout();
			if (toolStripMenuItem.Checked)
			{
				this.m_CtrlHStatusLabelCtrl.Visible = true;
				// ISSUE: method pointer
				this.mCtrl_HWindow.HMouseMove += new HMouseEventHandler((this.HWindowControl_HMouseMove));
			}
			else
			{
				this.m_CtrlHStatusLabelCtrl.Visible = false;
				// ISSUE: method pointer
				this.mCtrl_HWindow.HMouseMove -= new HMouseEventHandler((this.HWindowControl_HMouseMove));
			}
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public void showStatusBar()
		{
			this.barVisible_strip.Checked = true;
		}

		private void SaveWindowDump()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "PNG图像|*.png|所有文件|*.*";
			if (saveFileDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(saveFileDialog.FileName))
				return;
			HOperatorSet.DumpWindow((this.HWindowHalconID), ("png best"), (saveFileDialog.FileName));
		}

		private void SaveImage()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "BMP图像|*.bmp|所有文件|*.*";
			if (saveFileDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(saveFileDialog.FileName))
				return;
			HOperatorSet.WriteImage((HObject)this.hv_image, ("bmp"), (0), (saveFileDialog.FileName));
		}

		private void DispImageFit(HWindowControl hw_Ctrl)
		{
			try
			{
				this.viewWindow.resetWindowImage();
			}
			catch (Exception ex)
			{
			}
		}

		private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
		{
			if (this.hv_image == null)
				return;
			try
			{
				HTuple htuple;
				HOperatorSet.CountChannels((HObject)this.hv_image, out htuple);
				double num1;
				double num2;
				int num3;
				this.hv_window.GetMpositionSubPix(out num1, out num2, out num3);
				string str1 = string.Format("ROW: {0:0000.0}, COLUMN: {1:0000.0}", num1, num2);
				if (num2 >= 0.0 && num2 < (double)this.hv_imageWidth && (num1 >= 0.0 && num1 < (double)this.hv_imageHeight))
				{
					string str2;
					if ((htuple) == 1)
						str2 = string.Format("Val: {0:000.0}", this.hv_image.GetGrayval((int)num1, (int)num2));
					else if ((htuple) == 3)
					{
						HImage himage1 = this.hv_image.AccessChannel(1);
						HImage himage2 = this.hv_image.AccessChannel(2);
						HImage himage3 = this.hv_image.AccessChannel(3);
						double grayval1 = himage1.GetGrayval((int)num1, (int)num2);
						double grayval2 = himage2.GetGrayval((int)num1, (int)num2);
						double grayval3 = himage3.GetGrayval((int)num1, (int)num2);
						((HObjectBase)himage1).Dispose();
						((HObjectBase)himage2).Dispose();
						((HObjectBase)himage3).Dispose();
						str2 = string.Format("Val: ({0:000.0}, {1:000.0}, {2:000.0})", grayval1, grayval2, grayval3);
					}
					else
						str2 = "";
					this.m_CtrlHStatusLabelCtrl.Text = this.str_imgSize + "    " + str1 + "    " + str2;
				}
			}
			catch (Exception ex)
			{
			}
		}

		public void ClearWindow()
		{
			try
			{
				base.Invoke(new Action(delegate
				{
					this.m_CtrlHStatusLabelCtrl.Visible = false;
					this.barVisible_strip.Enabled = false;
					this.fit_strip.Enabled = false;
					this.histogram_strip.Enabled = false;
					this.saveImg_strip.Enabled = false;
					this.saveWindow_strip.Enabled = false;
					this.mCtrl_HWindow.HalconWindow.ClearWindow();
					this.viewWindow.ClearWindow();
				}));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public void HobjectToHimage(HObject hobject)
		{
			if (hobject == null || !hobject.IsInitialized())
				this.ClearWindow();
			else
				this.Image = new HImage(hobject);
		}

		public void DispObj(HObject hObj)
		{
			lock (this)
			  this.viewWindow.displayHobject(hObj, null);
		}

		public void DispObj(HObject hObj, string color)
		{
			lock (this)
			  this.viewWindow.displayHobject(hObj, color);
		}

		private void mCtrl_HWindow_MouseLeave(object sender, EventArgs e)
		{
			this.viewWindow.mouseleave();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
				this.hv_MenuStrip.Dispose();
				// ISSUE: method pointer
				this.mCtrl_HWindow.HMouseMove -= new HMouseEventHandler((this.HWindowControl_HMouseMove));
			}
			if (disposing && this.hv_image != null)
				((HObjectBase)this.hv_image).Dispose();
			if (disposing && this.hv_window != null)
				((HTool)this.hv_window).Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HWindowFinal));
            this.m_CtrlHStatusLabelCtrl = new System.Windows.Forms.Label();
            this.m_CtrlImageList = new System.Windows.Forms.ImageList(this.components);
            this.mCtrl_HWindow = new HalconDotNet.HWindowControl();
            this.SuspendLayout();
            // 
            // m_CtrlHStatusLabelCtrl
            // 
            this.m_CtrlHStatusLabelCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_CtrlHStatusLabelCtrl.AutoSize = true;
            this.m_CtrlHStatusLabelCtrl.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_CtrlHStatusLabelCtrl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.m_CtrlHStatusLabelCtrl.Location = new System.Drawing.Point(3, 326);
            this.m_CtrlHStatusLabelCtrl.Margin = new System.Windows.Forms.Padding(3);
            this.m_CtrlHStatusLabelCtrl.Name = "m_CtrlHStatusLabelCtrl";
            this.m_CtrlHStatusLabelCtrl.Size = new System.Drawing.Size(0, 17);
            this.m_CtrlHStatusLabelCtrl.TabIndex = 1;
            // 
            // m_CtrlImageList
            // 
            this.m_CtrlImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_CtrlImageList.ImageStream")));
            this.m_CtrlImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.m_CtrlImageList.Images.SetKeyName(0, "PicturesIcon.png");
            this.m_CtrlImageList.Images.SetKeyName(1, "TableIcon.png");
            // 
            // mCtrl_HWindow
            // 
            this.mCtrl_HWindow.BackColor = System.Drawing.Color.Black;
            this.mCtrl_HWindow.BorderColor = System.Drawing.Color.Black;
            this.mCtrl_HWindow.Cursor = System.Windows.Forms.Cursors.Default;
            this.mCtrl_HWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mCtrl_HWindow.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.mCtrl_HWindow.Location = new System.Drawing.Point(0, 0);
            this.mCtrl_HWindow.Margin = new System.Windows.Forms.Padding(0);
            this.mCtrl_HWindow.Name = "mCtrl_HWindow";
            this.mCtrl_HWindow.Size = new System.Drawing.Size(419, 346);
            this.mCtrl_HWindow.TabIndex = 0;
            this.mCtrl_HWindow.WindowSize = new System.Drawing.Size(419, 346);
            this.mCtrl_HWindow.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWindowControl_HMouseMove);
            this.mCtrl_HWindow.MouseLeave += new System.EventHandler(this.mCtrl_HWindow_MouseLeave);
            // 
            // HWindow_Final
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.m_CtrlHStatusLabelCtrl);
            this.Controls.Add(this.mCtrl_HWindow);
            this.Name = "HWindow_Final";
            this.Size = new System.Drawing.Size(419, 346);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
