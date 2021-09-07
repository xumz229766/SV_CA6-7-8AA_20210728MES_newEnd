using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ViewWindow.Model
{
    public class ROIController
    {
        private string activeCol = "green";
        private string activeHdlCol = "red";
        private string inactiveCol = "yellow";
        public const int MODE_ROI_POS = 21;
        public const int MODE_ROI_NEG = 22;
        public const int MODE_ROI_NONE = 23;
        public const int EVENT_UPDATE_ROI = 50;
        public const int EVENT_CHANGED_ROI_SIGN = 51;
        public const int EVENT_MOVING_ROI = 52;
        public const int EVENT_DELETED_ACTROI = 53;
        public const int EVENT_DELETED_ALL_ROIS = 54;
        public const int EVENT_ACTIVATED_ROI = 55;
        public const int EVENT_CREATED_ROI = 56;
        private ROI roiMode;
        private int stateROI;
        private double currX;
        private double currY;
        public int activeROIidx;
        public int deletedIdx;
        public ArrayList ROIList;
        public HRegion ModelROI;
        public HWndCtrl viewController;
        public IconicDelegate NotifyRCObserver;

        // protected internal ROIController()
        public ROIController()
        {
            this.stateROI = 23;
            this.ROIList = new ArrayList();
            this.activeROIidx = -1;
            this.ModelROI = new HRegion();
            this.NotifyRCObserver = new IconicDelegate(this.dummyI);
            this.deletedIdx = -1;
            this.currX = this.currY = -1.0;
        }

        public void setViewController(HWndCtrl view)
        {
            this.viewController = view;
        }

        public HRegion getModelRegion()
        {
            return this.ModelROI;
        }

        public ArrayList getROIList()
        {
            return this.ROIList;
        }

        public ROI getActiveROI()
        {
            try
            {
                if (this.activeROIidx != -1)
                    return (ROI)this.ROIList[this.activeROIidx];
                return (ROI)null;
            }
            catch (Exception ex)
            {
                return (ROI)null;
            }
        }

        public int getActiveROIIdx()
        {
            return this.activeROIidx;
        }

        public void setActiveROIIdx(int active)
        {
            this.activeROIidx = active;
        }

        public int getDelROIIdx()
        {
            return this.deletedIdx;
        }

        public void setROIShape(ROI r)
        {
            this.roiMode = r;
            this.roiMode.setOperatorFlag(this.stateROI);
        }

        public void setROISign(int mode)
        {
            this.stateROI = mode;
            if (this.activeROIidx == -1)
                return;
            ((ROI)this.ROIList[this.activeROIidx]).setOperatorFlag(this.stateROI);
            this.viewController.repaint();
            this.NotifyRCObserver(51);
        }

        public void removeActive()
        {
            if (this.activeROIidx == -1)
                return;
            this.ROIList.RemoveAt(this.activeROIidx);
            this.deletedIdx = this.activeROIidx;
            this.activeROIidx = -1;
            this.viewController.repaint();
            this.NotifyRCObserver(53);
        }

        public bool defineModelROI()
        {
            if (this.stateROI == 23)
                return true;
            HRegion hregion1 = new HRegion();
            HRegion hregion2 = new HRegion();
            hregion1.GenEmptyRegion();
            hregion2.GenEmptyRegion();
            for (int index = 0; index < this.ROIList.Count; ++index)
            {
                switch (((ROI)this.ROIList[index]).getOperatorFlag())
                {
                    case 21:
                        hregion1 = ((ROI)this.ROIList[index]).getRegion().Union2(hregion1);
                        break;
                    case 22:
                        hregion2 = ((ROI)this.ROIList[index]).getRegion().Union2(hregion2);
                        break;
                }
            }
            this.ModelROI = (HRegion)null;
            double num1;
            double num2;
            if (hregion1.AreaCenter(out num1, out num2) > 0)
            {
                HRegion hregion3 = hregion1.Difference(hregion2);
                if (hregion3.AreaCenter(out num1, out num2) > 0)
                    this.ModelROI = hregion3;
            }
            return this.ModelROI != null && this.ROIList.Count != 0;
        }

        public void reset()
        {
            this.ROIList.Clear();
            this.activeROIidx = -1;
            this.ModelROI = (HRegion)null;
            this.roiMode = (ROI)null;
            this.NotifyRCObserver(54);
        }

        public void resetROI()
        {
            this.activeROIidx = -1;
            this.roiMode = (ROI)null;
        }

        public void setDrawColor(string aColor, string aHdlColor, string inaColor)
        {
            if (aColor != "")
                this.activeCol = aColor;
            if (aHdlColor != "")
                this.activeHdlCol = aHdlColor;
            if (!(inaColor != ""))
                return;
            this.inactiveCol = inaColor;
        }

        public void paintData(HWindow window)
        {
            window.SetDraw("margin");
            window.SetLineWidth(1);
            if (this.ROIList.Count <= 0)
                return;
            window.SetDraw("margin");
            for (int index = 0; index < this.ROIList.Count; ++index)
            {
                window.SetColor(((ROI)this.ROIList[index]).Color);
                window.SetLineStyle(((ROI)this.ROIList[index]).flagLineStyle);
                ((ROI)this.ROIList[index]).draw(window);
            }
            if (this.activeROIidx != -1)
            {
                window.SetColor(this.activeCol);
                window.SetLineStyle(((ROI)this.ROIList[this.activeROIidx]).flagLineStyle);
                ((ROI)this.ROIList[this.activeROIidx]).draw(window);
                window.SetColor(this.activeHdlCol);
                ((ROI)this.ROIList[this.activeROIidx]).displayActive(window);
            }
        }

        public int mouseDownAction(double imgX, double imgY)
        {
            int num1 = -1;
            double num2 = 10000.0;
            double num3 = 35.0;
            if (this.roiMode != null)
            {
                this.roiMode.createROI(imgX, imgY);
                this.ROIList.Add(this.roiMode);
                this.roiMode = (ROI)null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
            else if (this.ROIList.Count > 0)
            {
                this.activeROIidx = -1;
                for (int index = 0; index < this.ROIList.Count; ++index)
                {
                    double closestHandle = ((ROI)this.ROIList[index]).distToClosestHandle(imgX, imgY);
                    if (closestHandle < num2 && closestHandle < num3)
                    {
                        num2 = closestHandle;
                        num1 = index;
                    }
                }
                if (num1 >= 0)
                {
                    this.activeROIidx = num1;
                    this.NotifyRCObserver(55);
                }
                this.viewController.repaint();
            }
            return this.activeROIidx;
        }

        public void mouseMoveAction(double newX, double newY)
        {
            try
            {
                if (newX == this.currX && newY == this.currY)
                    return;
                ((ROI)this.ROIList[this.activeROIidx]).moveByHandle(newX, newY);
                this.viewController.repaint();
                this.currX = newX;
                this.currY = newY;
                this.NotifyRCObserver(52);
            }
            catch (Exception ex)
            {
            }
        }

        public void dummyI(int v)
        {

        }

        public void displayRect1(ROI roi, double row1, double col1, double row2, double col2)
        {
            this.setROIShape(roi);
            if (this.roiMode == null) return;
            this.roiMode.createRectangle1(row1, col1, row2, col2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            this.roiMode.Color = roi.Color;
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        public void displayRect2(ROI roi, double row, double col, double phi, double length1, double length2)
        {
            this.setROIShape(roi);
            if (this.roiMode == null) return;
            this.roiMode.createRectangle2(row, col, phi, length1, length2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            this.roiMode.Color = roi.Color;
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        public void displayCircle1(ROI roi, double row, double col, double radius)
        {
            this.setROIShape(roi);
            if (this.roiMode == null) return;
            this.roiMode.createCircle1(row, col, radius);
            this.roiMode.Type = this.roiMode.GetType().Name;
            this.roiMode.Color = roi.Color;
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }
        public void displayCircle2(ROI roi, double row, double col, double radius1, double radius2)
        {
            this.setROIShape(roi);
            if (this.roiMode == null) return;
            this.roiMode.createCircle2(row, col, radius1,radius2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            this.roiMode.Color = roi.Color;
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }
        public void displayLine(ROI roi, double beginRow, double beginCol, double endRow, double endCol)
        {
            this.setROIShape(roi);
            if (this.roiMode == null) return;
            this.roiMode.createLine(beginRow, beginCol, endRow, endCol);
            this.roiMode.Type = this.roiMode.GetType().Name;
            this.roiMode.Color = roi.Color;
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        protected internal void genRect1(double row1, double col1, double row2, double col2, ref List<ROI> rois)
        {
            this.setROIShape((ROI)new ROIRectangle1());
            if (rois == null) rois = new List<ROI>();
            if (this.roiMode == null) return;
            this.roiMode.createRectangle1(row1, col1, row2, col2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            rois.Add(this.roiMode);
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        protected internal void genRect2(double row, double col, double phi, double length1, double length2, ref List<ROI> rois)
        {
            this.setROIShape((ROI)new ROIRectangle2());
            if (rois == null) rois = new List<ROI>();
            if (this.roiMode == null) return;
            this.roiMode.createRectangle2(row, col, phi, length1, length2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            rois.Add(this.roiMode);
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        protected internal void genCircle1(double row, double col, double radius, ref List<ROI> rois)
        {
            this.setROIShape((ROI)new ROICircle());
            if (rois == null) rois = new List<ROI>();
            if (this.roiMode == null) return;
            this.roiMode.createCircle1(row, col, radius);
            this.roiMode.Type = this.roiMode.GetType().Name;
            rois.Add(this.roiMode);
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }
        protected internal void genCircle2(double row, double col, double radius1, double radius2, ref List<ROI> rois)
        {
            this.setROIShape((ROI)new ROICircle());
            if (rois == null) rois = new List<ROI>();
            if (this.roiMode == null) return;
            this.roiMode.createCircle2(row, col, radius1, radius2);
            this.roiMode.Type = this.roiMode.GetType().Name;
            rois.Add(this.roiMode);
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }
        protected internal void genLine(double beginRow, double beginCol, double endRow, double endCol, ref List<ROI> rois)
        {
            this.setROIShape((ROI)new ROILine());
            if (rois == null) rois = new List<ROI>();
            if (this.roiMode == null) return;
            this.roiMode.createLine(beginRow, beginCol, endRow, endCol);
            this.roiMode.Type = this.roiMode.GetType().Name;
            rois.Add(this.roiMode);
            this.ROIList.Add(this.roiMode);
            this.roiMode = (ROI)null;
            this.activeROIidx = this.ROIList.Count - 1;
            this.viewController.repaint();
            this.NotifyRCObserver(56);
        }

        protected internal List<double> smallestActiveROI(out string name, out int index)
        {
            name = "";
            int activeRoiIdx = this.getActiveROIIdx();
            index = activeRoiIdx;
            if (activeRoiIdx <= -1)
                return (List<double>)null;
            ROI activeRoi = this.getActiveROI();
            Type type = activeRoi.GetType();
            name = type.Name;
            HTuple modelData = activeRoi.getModelData();
            List<double> doubleList = new List<double>();
            for (int index1 = 0; index1 < modelData.Length; ++index1)
                doubleList.Add(modelData[index1]);
            return doubleList;
        }

        protected internal ROI smallestActiveROI(out List<double> data, out int index)
        {
            try
            {
                int activeRoiIdx = this.getActiveROIIdx();
                index = activeRoiIdx;
                data = new List<double>();
                if (activeRoiIdx <= -1) return (ROI)null;
                ROI activeRoi = this.getActiveROI();
                activeRoi.GetType();
                HTuple modelData = activeRoi.getModelData();
                for (int index1 = 0; index1 < modelData.Length; ++index1) data.Add(modelData[index1]);
                return activeRoi;
            }
            catch (Exception ex)
            {
                data = (List<double>)null;
                index = 0;
                return (ROI)null;
            }
        }

        protected internal void removeActiveROI(ref List<ROI> roi)
        {
            int activeRoiIdx = this.getActiveROIIdx();
            if (activeRoiIdx <= -1)
                return;
            this.removeActive();
            roi.RemoveAt(activeRoiIdx);
        }

        protected internal void selectROI(int index)
        {
            this.activeROIidx = index;
            this.NotifyRCObserver(55);
            this.viewController.repaint();
        }

        protected internal void resetWindowImage()
        {
            this.viewController.repaint();
        }

        protected internal void zoomWindowImage()
        {
            this.viewController.setViewState(11);
        }

        protected internal void moveWindowImage()
        {
            this.viewController.setViewState(12);
        }

        protected internal void noneWindowImage()
        {
            this.viewController.setViewState(10);
        }
    }
}
