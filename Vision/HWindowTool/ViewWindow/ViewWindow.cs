using HalconDotNet;
using System.Collections.Generic;
using System.Linq;
using ViewWindow.Config;
using ViewWindow.Model;

namespace ViewWindow
{
    public class ViewWindow : IViewWindow
    {
        private HWndCtrl _hWndControl;
        private ROIController _roiController;

        public ViewWindow(HWindowControl window)
        {
            this._hWndControl = new HWndCtrl(window);
            this._roiController = new ROIController();
            this._hWndControl.setROIController(this._roiController);
            this._hWndControl.setViewState(10);
        }

        public void ClearWindow()
        {
            this._hWndControl.clearList();
            this._hWndControl.clearHObjectList();
        }

        public void displayImage(HObject img)
        {
            this._hWndControl.addImageShow(img);
            this._roiController.reset();
            this._roiController.resetWindowImage();
        }

        public void notDisplayRoi()
        {
            this._roiController.reset();
            this._roiController.resetWindowImage();
        }

        public int getRoiCount()
        {
            return this._roiController.ROIList.Count;
        }

        public void setDrawModel(bool flag)
        {
            this._hWndControl.drawModel = flag;
        }

        public void resetWindowImage()
        {
            this._hWndControl.resetWindow();
            this._roiController.resetWindowImage();
        }

        public void mouseleave()
        {
            this._hWndControl.raiseMouseup();
        }

        public void zoomWindowImage()
        {
            this._roiController.zoomWindowImage();
        }

        public void moveWindowImage()
        {
            this._roiController.moveWindowImage();
        }

        public void noneWindowImage()
        {
            this._roiController.noneWindowImage();
        }

        public void genRect1(double row1, double col1, double row2, double col2, ref List<ROI> rois)
        {
            this._roiController.genRect1(row1, col1, row2, col2, ref rois);
        }

        public void genRect2(double row, double col, double phi, double length1, double length2, ref List<ROI> rois)
        {
            this._roiController.genRect2(row, col, phi, length1, length2, ref rois);
        }

        public void genCircle(double row, double col, double radius, ref List<ROI> rois)
        {
            this._roiController.genCircle1(row, col, radius, ref rois);
        }
        public void genCircle2(double row, double col, double radius1, double radius2, ref List<ROI> rois)
        {
            this._roiController.genCircle2(row, col, radius1, radius2, ref rois);
        }
        public void genLine(double beginRow, double beginCol, double endRow, double endCol, ref List<ROI> rois)
        {
            this._roiController.genLine(beginRow, beginCol, endRow, endCol, ref rois);
        }

        public List<double> smallestActiveROI(out string name, out int index)
        {
            return this._roiController.smallestActiveROI(out name, out index);
        }

        public ROI smallestActiveROI(out List<double> data, out int index)
        {
            return this._roiController.smallestActiveROI(out data, out index);
        }

        public void selectROI(int index)
        {
            this._roiController.selectROI(index);
        }

        public void selectROI(List<ROI> rois, int index)
        {
            if (rois.Count <= index || index < 0)
                return;
            this._hWndControl.resetAll();
            this._hWndControl.repaint();
            HTuple modelData = rois[index].getModelData();
            switch (rois[index].Type)
            {
                case "ROIRectangle1":
                    if (modelData != null)
                    {
                        this._roiController.displayRect1(rois[index], modelData[0], modelData[1], modelData[2], modelData[3]);
                        break;
                    }
                    break;
                case "ROIRectangle2":
                    if (modelData != null)
                    {
                        this._roiController.displayRect2(rois[index], modelData[0], modelData[1], modelData[2], modelData[3], modelData[4]);
                        break;
                    }
                    break;
                case "ROICircle":
                    if (modelData != null)
                    {
                        this._roiController.displayCircle1(rois[index], modelData[0], modelData[1], modelData[2]);
                        break;
                    }
                    break;
                case "ROICircle2":
                    if (modelData != null)
                    {
                        this._roiController.displayCircle2(rois[index], modelData[0], modelData[1], modelData[2], modelData[3]);
                        break;
                    }
                    break;
                case "ROILine":
                    if (modelData != null)
                    {
                        this._roiController.displayLine(rois[index], modelData[0], modelData[1], modelData[2], modelData[3]);
                        break;
                    }
                    break;
            }
        }

        public void displayROI(List<ROI> rois)
        {
            if (rois == null)
                return;
            foreach (ROI roi in rois)
            {
                HTuple modelData = roi.getModelData();
                switch (roi.Type)
                {
                    case "ROIRectangle1":
                        if (modelData != null)
                        {
                            this._roiController.displayRect1(roi, modelData[0], modelData[1], modelData[2], modelData[3]);
                            break;
                        }
                        break;
                    case "ROIRectangle2":
                        if (modelData != null)
                        {
                            this._roiController.displayRect2(roi, modelData[0], modelData[1], modelData[2], modelData[3], modelData[4]);
                            break;
                        }
                        break;
                    case "ROICircle":
                        if (modelData != null)
                        {
                            this._roiController.displayCircle1(roi, modelData[0], modelData[1], modelData[2]);
                            break;
                        }
                        break;
                    case "ROICircle2":
                        if (modelData != null)
                        {
                            this._roiController.displayCircle2(roi, modelData[0], modelData[1], modelData[2], modelData[3]);
                            break;
                        }
                        break;
                    case "ROILine":
                        if (modelData != null)
                        {
                            this._roiController.displayLine(roi, modelData[0], modelData[1], modelData[2], modelData[3]);
                            break;
                        }
                        break;
                }
            }
        }

        public void removeActiveROI(ref List<ROI> rois)
        {
            this._roiController.removeActiveROI(ref rois);
        }

        public void setActiveRoi(int index)
        {
            this._roiController.activeROIidx = index;
        }

        public void saveROI(List<ROI> rois, string fileNmae)
        {
            List<RoiData> roiDataList = new List<RoiData>();
            for (int id = 0; id < rois.Count; ++id)
                roiDataList.Add(new RoiData(id, rois[id]));
            SerializeHelper.Save(roiDataList, fileNmae);
        }

        public void loadROI(string fileName, out List<ROI> rois)
        {
            rois = new List<ROI>();
            List<RoiData> roiDataList = (List<RoiData>)SerializeHelper.Load(new List<RoiData>().GetType(), fileName);
            for (int index = 0; index < roiDataList.Count; ++index)
            {
                switch (roiDataList[index].Name)
                {
                    case "Rectangle1":
                        this._roiController.genRect1(roiDataList[index].Rectangle1.Row1, roiDataList[index].Rectangle1.Column1, roiDataList[index].Rectangle1.Row2, roiDataList[index].Rectangle1.Column2, ref rois);
                        rois.Last<ROI>().Color = roiDataList[index].Rectangle1.Color;
                        break;
                    case "Rectangle2":
                        this._roiController.genRect2(roiDataList[index].Rectangle2.Row, roiDataList[index].Rectangle2.Column, roiDataList[index].Rectangle2.Phi, roiDataList[index].Rectangle2.Lenth1, roiDataList[index].Rectangle2.Lenth2, ref rois);
                        rois.Last<ROI>().Color = roiDataList[index].Rectangle2.Color;
                        break;
                    case "Circle":
                        this._roiController.genCircle1(roiDataList[index].Circle.Row, roiDataList[index].Circle.Column, roiDataList[index].Circle.Radius, ref rois);
                        rois.Last<ROI>().Color = roiDataList[index].Circle.Color;
                        break;
                    case "Circle2":
                        this._roiController.genCircle2(roiDataList[index].Circle.Row, roiDataList[index].Circle.Column, roiDataList[index].Circle2.Radius1, roiDataList[index].Circle2.Radius2, ref rois);
                        rois.Last<ROI>().Color = roiDataList[index].Circle.Color;
                        break;
                    case "Line":
                        this._roiController.genLine(roiDataList[index].Line.RowBegin, roiDataList[index].Line.ColumnBegin, roiDataList[index].Line.RowEnd, roiDataList[index].Line.ColumnEnd, ref rois);
                        rois.Last<ROI>().Color = roiDataList[index].Line.Color;
                        break;
                }
            }
            this._hWndControl.resetAll();
            this._hWndControl.repaint();
        }

        public void displayHobject(HObject obj, string color)
        {
            this._hWndControl.DispObj(obj, color);
        }

        public void displayHobject(HObject obj)
        {
            this._hWndControl.DispObj(obj, (string)null);
        }
    }
}
