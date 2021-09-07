using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ViewROI.Config;

namespace ViewWindow.Model
{
	public class HWndCtrl
	{
		private bool mousePressed = false;
		public bool drawModel = false;
		public string exceptionText = "";
		private List<HObjectWithColor> hObjectList = new List<HObjectWithColor>();
		public const int MODE_VIEW_NONE = 10;
		public const int MODE_VIEW_ZOOM = 11;
		public const int MODE_VIEW_MOVE = 12;
		public const int MODE_VIEW_ZOOMWINDOW = 13;
		public const int MODE_INCLUDE_ROI = 1;
		public const int MODE_EXCLUDE_ROI = 2;
		public const int EVENT_UPDATE_IMAGE = 31;
		public const int ERR_READING_IMG = 32;
		public const int ERR_DEFINING_GC = 33;
		private const int MAXNUMOBJLIST = 2;
		private int stateView;
		private double startX;
		private double startY;
		private HWindowControl viewPort;
		private ROIController roiManager;
		private int dispROI;
		private int windowWidth;
		private int windowHeight;
		private int imageWidth;
		private int imageHeight;
		private int[] CompRangeX;
		private int[] CompRangeY;
		private int prevCompX;
		private int prevCompY;
		private double stepSizeX;
		private double stepSizeY;
		private double ImgRow1;
		private double ImgCol1;
		private double ImgRow2;
		private double ImgCol2;
		public FuncDelegate addInfoDelegate;
		public IconicDelegate NotifyIconObserver;
		private HWindow ZoomWindow;
		private double zoomWndFactor;
		private double zoomAddOn;
		private int zoomWndSize;
		private ArrayList HObjImageList;
		private GraphicsContext mGC;

        public  HWndCtrl(HWindowControl view)
		{
			this.viewPort = view;
			this.stateView = 10;
			this.windowWidth = ((Control)this.viewPort).Size.Width;
			this.windowHeight = ((Control)this.viewPort).Size.Height;
			this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
			this.zoomAddOn = Math.Pow(0.9, 5.0);
			this.zoomWndSize = 150;
			this.CompRangeX = new int[2] { 0, 100 };
			this.CompRangeY = new int[2] { 0, 100 };
			this.prevCompX = this.prevCompY = 0;
			this.dispROI = 1;
			viewPort.HMouseUp += new HalconDotNet.HMouseEventHandler(this.mouseUp);
			viewPort.HMouseDown += new HalconDotNet.HMouseEventHandler(this.mouseDown);
			viewPort.HMouseWheel += new HalconDotNet.HMouseEventHandler(this.HMouseWheel);
			viewPort.HMouseMove += new HalconDotNet.HMouseEventHandler(this.mouseMoved);
			this.addInfoDelegate = new FuncDelegate(this.dummyV);
			this.NotifyIconObserver = new IconicDelegate(this.dummy);
			this.HObjImageList = new ArrayList(20);
			this.mGC = new GraphicsContext();
			this.mGC.gcNotification = new GCDelegate(this.exceptionGC);
		}

		private void HMouseWheel(object sender, HMouseEventArgs e)
		{
			if (this.drawModel)
				return;
			double scale = e.Delta <= 0 ? 10.0 / 9.0 : 0.9;
			this.zoomImage(e.X, e.Y, scale);
		}

        public void setImagePart(HImage image)
		{
			string str;
			int c2;
			int r2;
			image.GetImagePointer1(out str, out c2, out r2);
			this.setImagePart(0, 0, r2, c2);
		}

        public void setImagePart(int r1, int c1, int r2, int c2)
		{
			this.ImgRow1 = (double)r1;
			this.ImgCol1 = (double)c1;
			this.ImgRow2 = (double)(this.imageHeight = r2);
			this.ImgCol2 = (double)(this.imageWidth = c2);
			Rectangle imagePart = this.viewPort.ImagePart;
			imagePart.X = (int)this.ImgCol1;
			imagePart.Y = (int)this.ImgRow1;
			imagePart.Height = this.imageHeight;
			imagePart.Width = this.imageWidth;
			this.viewPort.ImagePart = (imagePart);
		}

		public void setViewState(int mode)
		{
			this.stateView = mode;
			if (this.roiManager == null)
				return;
			this.roiManager.resetROI();
		}

		private void dummy(int val)
		{

		}

		private void dummyV()
		{

		}

		private void exceptionGC(string message)
		{
			this.exceptionText = message;
			this.NotifyIconObserver(33);
		}

		public void setDispLevel(int mode)
		{
			this.dispROI = mode;
		}

        public void zoomImage(double x, double y, double scale)
		{
			if (this.drawModel)
				return;
			double num1 = (x - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1);
			double num2 = (y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1);
			double a1 = (this.ImgCol2 - this.ImgCol1) * scale;
			double a2 = (this.ImgRow2 - this.ImgRow1) * scale;
			this.ImgCol1 = x - a1 * num1;
			this.ImgCol2 = x + a1 * (1.0 - num1);
			this.ImgRow1 = y - a2 * num2;
			this.ImgRow2 = y + a2 * (1.0 - num2);
			int num3 = (int)Math.Round(a1);
			int num4 = (int)Math.Round(a2);
			Rectangle imagePart = this.viewPort.ImagePart;
			imagePart.X = (int)Math.Round(this.ImgCol1);
			imagePart.Y = (int)Math.Round(this.ImgRow1);
			imagePart.Width = num3 > 0 ? num3 : 1;
			imagePart.Height = num4 > 0 ? num4 : 1;
			this.viewPort.ImagePart = (imagePart);
			double num5 = scale * this.zoomWndFactor;
			if (this.zoomWndFactor < 0.01 && num5 < this.zoomWndFactor) this.resetWindow();
			else if (this.zoomWndFactor > 100.0 && num5 > this.zoomWndFactor)
			{
				this.resetWindow();
			}
			else
			{
				this.zoomWndFactor = num5;
				this.repaint();
			}
		}

		public void zoomImage(double scaleFactor)
		{
			if (this.ImgRow2 - this.ImgRow1 == scaleFactor * (double)this.imageHeight && this.ImgCol2 - this.ImgCol1 == scaleFactor * (double)this.imageWidth)
			{
				this.repaint();
			}
			else
			{
				this.ImgRow2 = this.ImgRow1 + (double)this.imageHeight;
				this.ImgCol2 = this.ImgCol1 + (double)this.imageWidth;
				double x = this.ImgCol1;
				double y = this.ImgRow1;
				this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
				this.zoomImage(x, y, scaleFactor);
			}
		}

		public void scaleWindow(double scale)
		{
			this.ImgRow1 = 0.0;
			this.ImgCol1 = 0.0;
			this.ImgRow2 = (double)this.imageHeight;
			this.ImgCol2 = (double)this.imageWidth;
			((Control)this.viewPort).Width = (int)(this.ImgCol2 * scale);
			((Control)this.viewPort).Height = (int)(this.ImgRow2 * scale);
			this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
		}

		public void setZoomWndFactor()
		{
			this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
		}

		public void setZoomWndFactor(double zoomF)
		{
			this.zoomWndFactor = zoomF;
		}

		private void moveImage(double motionX, double motionY)
		{
			this.ImgRow1 += -motionY;
			this.ImgRow2 += -motionY;
			this.ImgCol1 += -motionX;
			this.ImgCol2 += -motionX;
			Rectangle imagePart = this.viewPort.ImagePart;
			imagePart.X = (int)Math.Round(this.ImgCol1);
			imagePart.Y = (int)Math.Round(this.ImgRow1);
			this.viewPort.ImagePart = (imagePart);
			this.repaint();
		}

		public void resetAll()
		{
			this.ImgRow1 = 0.0;
			this.ImgCol1 = 0.0;
			this.ImgRow2 = (double)this.imageHeight;
			this.ImgCol2 = (double)this.imageWidth;
			this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
			Rectangle imagePart = this.viewPort.ImagePart;
			imagePart.X = (int)this.ImgCol1;
			imagePart.Y = (int)this.ImgRow1;
			imagePart.Width = this.imageWidth;
			imagePart.Height = this.imageHeight;
			this.viewPort.ImagePart = (imagePart);
			if (this.roiManager == null) return;
			this.roiManager.reset();
		}

		 public  void resetWindow()
		{
			this.ImgRow1 = 0.0;
			this.ImgCol1 = 0.0;
			this.ImgRow2 = (double)this.imageHeight;
			this.ImgCol2 = (double)this.imageWidth;
			this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
			Rectangle imagePart = this.viewPort.ImagePart;
			imagePart.X = (int)this.ImgCol1;
			imagePart.Y = (int)this.ImgRow1;
			imagePart.Width = this.imageWidth;
			imagePart.Height = this.imageHeight;
			this.viewPort.ImagePart = (imagePart);
		}

		private void mouseDown(object sender, HMouseEventArgs e)
		{
			if (this.drawModel)
				return;
			this.stateView = 12;
			this.mousePressed = true;
			int num = -1;
			if (this.roiManager != null && this.dispROI == 1)
				num = this.roiManager.mouseDownAction(e.X, e.Y);
			if (num != -1)
				return;
			switch (this.stateView)
			{
				case 12:
					this.startX = e.X;
					this.startY = e.Y;
					break;
				case 13:
					this.activateZoomWindow((int)e.X, (int)e.Y);
					break;
			}
		}

		private void activateZoomWindow(int X, int Y)
		{
			if (this.ZoomWindow != null)
				((HTool)this.ZoomWindow).Dispose();
			HOperatorSet.SetSystem(("border_width"), (10));
			this.ZoomWindow = new HWindow();
			double num1 = ((double)X - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1) * (double)((Control)this.viewPort).Width;
			double num2 = ((double)Y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1) * (double)((Control)this.viewPort).Height;
			int num3 = (int)((double)(this.zoomWndSize / 2) * this.zoomWndFactor * this.zoomAddOn);
			this.ZoomWindow.OpenWindow((int)num2 - this.zoomWndSize / 2, (int)num1 - this.zoomWndSize / 2, this.zoomWndSize, this.zoomWndSize, this.viewPort.HalconID, "visible", "");
			this.ZoomWindow.SetPart(Y - num3, X - num3, Y + num3, X + num3);
			this.repaint(this.ZoomWindow);
			this.ZoomWindow.SetColor("black");
		}

		public void raiseMouseup()
		{
			this.mousePressed = false;
			if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
			{
				this.roiManager.NotifyRCObserver(50);
			}
			else
			{
				if (this.stateView != 13)
					return;
				((HTool)this.ZoomWindow).Dispose();
			}
		}

		private void mouseUp(object sender, HMouseEventArgs e)
		{
			if (this.drawModel)
				return;
			this.mousePressed = false;
			if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
			{
				this.roiManager.NotifyRCObserver(50);
			}
			else
			{
				if (this.stateView != 13)
					return;
				((HTool)this.ZoomWindow).Dispose();
			}
		}

		private void mouseMoved(object sender, HMouseEventArgs e)
		{
			if (this.drawModel || !this.mousePressed)
				return;
			if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
				this.roiManager.mouseMoveAction(e.X, e.Y);
			else if (this.stateView == 12)
			{
				double motionX = e.X - this.startX;
				double motionY = e.Y - this.startY;
				if ((int)motionX == 0 && (int)motionY == 0)
					return;
				this.moveImage(motionX, motionY);
				this.startX = e.X - motionX;
				this.startY = e.Y - motionY;
			}
			else
			{
				if (this.stateView != 13)
					return;
				HSystem.SetSystem("flush_graphic", "false");
				this.ZoomWindow.ClearWindow();
				double num1 = (e.X - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1) * (double)((Control)this.viewPort).Width;
				double num2 = (e.Y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1) * (double)((Control)this.viewPort).Height;
				double num3 = (double)(this.zoomWndSize / 2) * this.zoomWndFactor * this.zoomAddOn;
				this.ZoomWindow.SetWindowExtents((int)num2 - this.zoomWndSize / 2, (int)num1 - this.zoomWndSize / 2, this.zoomWndSize, this.zoomWndSize);
				this.ZoomWindow.SetPart((int)(e.Y - num3), (int)(e.X - num3), (int)(e.Y + num3), (int)(e.X + num3));
				this.repaint(this.ZoomWindow);
				HSystem.SetSystem("flush_graphic", "true");
				this.ZoomWindow.DispLine(-100.0, -100.0, -100.0, -100.0);
			}
		}

		public void setGUICompRangeX(int[] xRange, int Init)
		{
			this.CompRangeX = xRange;
			int num = xRange[1] - xRange[0];
			this.prevCompX = Init;
			this.stepSizeX = (double)this.imageWidth / (double)num * (double)(this.imageWidth / this.windowWidth);
		}

		public void setGUICompRangeY(int[] yRange, int Init)
		{
			this.CompRangeY = yRange;
			int num = yRange[1] - yRange[0];
			this.prevCompY = Init;
			this.stepSizeY = (double)this.imageHeight / (double)num * (double)(this.imageHeight / this.windowHeight);
		}

		public void resetGUIInitValues(int xVal, int yVal)
		{
			this.prevCompX = xVal;
			this.prevCompY = yVal;
		}

		public void moveXByGUIHandle(int valX)
		{
			double motionX = (double)(valX - this.prevCompX) * this.stepSizeX;
			if (motionX == 0.0)
				return;
			this.moveImage(motionX, 0.0);
			this.prevCompX = valX;
		}

		public void moveYByGUIHandle(int valY)
		{
			double motionY = (double)(valY - this.prevCompY) * this.stepSizeY;
			if (motionY == 0.0)
				return;
			this.moveImage(0.0, motionY);
			this.prevCompY = valY;
		}

		public void zoomByGUIHandle(double valF)
		{
			this.zoomImage(this.ImgCol1 + (this.ImgCol2 - this.ImgCol1) / 2.0, this.ImgRow1 + (this.ImgRow2 - this.ImgRow1) / 2.0, 1.0 / ((this.ImgCol2 - this.ImgCol1) / (double)this.imageWidth) * (100.0 / valF));
		}

		public void repaint()
		{
			this.repaint(this.viewPort.HalconWindow);
		}

		public void repaint(HWindow window)
		{
			try
			{
				int count = this.HObjImageList.Count;
				HSystem.SetSystem("flush_graphic", "false");
				window.ClearWindow();
				this.mGC.stateOfSettings.Clear();
				for (int index = 0; index < count; ++index)
				{
					HObjectEntry hobjectEntry = (HObjectEntry)this.HObjImageList[index];
					this.mGC.applyContext(window, hobjectEntry.gContext);
					window.DispObj(hobjectEntry.HObj);
				}
				this.showHObjectList();
				this.addInfoDelegate();
				if (this.roiManager != null && this.dispROI == 1)
					this.roiManager.paintData(window);
				HSystem.SetSystem("flush_graphic", "true");
				window.SetColor("black");
				window.DispLine(-100.0, -100.0, -101.0, -101.0);
			}
			catch (Exception ex)
			{
			}
		}

		public void addIconicVar(HObject img)
		{
			for (int index = 0; index < this.HObjImageList.Count; ++index)
				((HObjectEntry)this.HObjImageList[index]).clear();
			if (img == null)
				return;
			HTuple htuple = (HTuple)null;
			HOperatorSet.GetObjClass(img, out htuple);
			if (htuple != "image")
				return;
			HImage himage = new HImage(img);
			if (himage != null)
			{
				double num1;
				double num2;
				int num3 = himage.GetDomain().AreaCenter(out num1, out num2);
				string str;
				int c2;
				int r2;
				himage.GetImagePointer1(out str, out c2, out r2);
				if (num3 == c2 * r2)
				{
					this.clearList();
					if (r2 != this.imageHeight || c2 != this.imageWidth)
					{
						this.imageHeight = r2;
						this.imageWidth = c2;
						this.zoomWndFactor = (double)this.imageWidth / (double)((Control)this.viewPort).Width;
						this.setImagePart(0, 0, r2, c2);
					}
				}
			}
			this.HObjImageList.Add(new HObjectEntry(himage, this.mGC.copyContextList()));
			this.clearHObjectList();
			if (this.HObjImageList.Count <= 2)
				return;
			((HObjectEntry)this.HObjImageList[0]).clear();
			this.HObjImageList.RemoveAt(1);
		}

		public void clearList()
		{
			this.HObjImageList.Clear();
		}

		public int getListCount()
		{
			return this.HObjImageList.Count;
		}

		public void changeGraphicSettings(string mode, string val)
		{
			switch (mode)
			{
				case "Color":
					this.mGC.setColorAttribute(val);
					break;
				case "DrawMode":
					this.mGC.setDrawModeAttribute(val);
					break;
				case "Lut":
					this.mGC.setLutAttribute(val);
					break;
				case "Paint":
					this.mGC.setPaintAttribute(val);
					break;
				case "Shape":
					this.mGC.setShapeAttribute(val);
					break;
			}
		}

		public void changeGraphicSettings(string mode, int val)
		{
			switch (mode)
			{
				case "Colored":
					this.mGC.setColoredAttribute(val);
					break;
				case "LineWidth":
					this.mGC.setLineWidthAttribute(val);
					break;
			}
		}

		public void changeGraphicSettings(string mode, HTuple val)
		{
			switch (mode)
			{
				case "LineStyle":
					this.mGC.setLineStyleAttribute(val);
					break;
			}
		}

		public void clearGraphicContext()
		{
			this.mGC.clear();
		}

		public Hashtable getGraphicContext()
		{
			return this.mGC.copyContextList();
		}

		 public  void setROIController(ROIController rC)
		{
			this.roiManager = rC;
			rC.setViewController(this);
			this.setViewState(10);
		}
       public void useROIController(ROIController rC)
		{
			roiManager = rC;
			rC.setViewController(this);
		}
		protected internal void addImageShow(HObject image)
		{
			this.addIconicVar(image);
		}

		public void DispObj(HObject hObj)
		{
			this.DispObj(hObj, (string)null);
		}

		public void DispObj(HObject hObj, string color)
		{
			lock (this)
			{
				if (color != null)
					HOperatorSet.SetColor(((HTool)this.viewPort.HalconWindow), (color));
				else
					HOperatorSet.SetColor(((HTool)this.viewPort.HalconWindow), ("red"));
				if (hObj != null && (hObj).IsInitialized())
				{
					HObject local_0 = new HObject(hObj);
					this.hObjectList.Add(new HObjectWithColor(local_0, color));
					this.viewPort.HalconWindow.DispObj(local_0);
				}
				HOperatorSet.SetColor(((HTool)this.viewPort.HalconWindow), ("red"));
			}
		}

		public void clearHObjectList()
		{
			foreach (HObjectWithColor hObject in this.hObjectList)
				(hObject.HObject).Dispose();
			this.hObjectList.Clear();
		}

		private void showHObjectList()
		{
			try
			{
				foreach (HObjectWithColor hObject in this.hObjectList)
				{
					if (hObject.Color != null)
						HOperatorSet.SetColor(this.viewPort.HalconWindow, (hObject.Color));
					else
						HOperatorSet.SetColor(this.viewPort.HalconWindow, ("red"));
					if (hObject != null && (hObject.HObject).IsInitialized())
					{
						this.viewPort.HalconWindow.DispObj(hObject.HObject);
						HOperatorSet.SetColor(this.viewPort.HalconWindow, ("red"));
					}
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}
