﻿using HalconDotNet;
using System;
using System.Xml.Serialization;

namespace ViewWindow.Model
{
    [Serializable]
    public class ROIRectangle2 : ROI
    {
        private double length1;
        private double length2;
        private double midR;
        private double midC;
        private double phi;
        private HTuple rowsInit;
        private HTuple colsInit;
        private HTuple rows;
        private HTuple cols;
        private HHomMat2D hom2D;
        private HHomMat2D tmp;

        [XmlElement(ElementName = "Row")]
        public double Row
        {
            get
            {
                return this.midR;
            }
            set
            {
                this.midR = value;
            }
        }

        [XmlElement(ElementName = "Column")]
        public double Column
        {
            get
            {
                return this.midC;
            }
            set
            {
                this.midC = value;
            }
        }

        [XmlElement(ElementName = "Phi")]
        public double Phi
        {
            get
            {
                return this.phi;
            }
            set
            {
                this.phi = value;
            }
        }

        [XmlElement(ElementName = "Length1")]
        public double Lenth1
        {
            get
            {
                return this.length1;
            }
            set
            {
                this.length1 = value;
            }
        }

        [XmlElement(ElementName = "Length2")]
        public double Lenth2
        {
            get
            {
                return this.length2;
            }
            set
            {
                this.length2 = value;
            }
        }

        public ROIRectangle2()
        {
            this.NumHandles = 6;
            this.activeHandleIdx = 4;
        }

        public ROIRectangle2(double row, double col, double phi, double length1, double length2)
        {
            this.createRectangle2(row, col, phi, length1, length2);
        }

        public override void createRectangle2(double row, double col, double phi, double length1, double length2)
        {
            base.createRectangle2(row, col, phi, length1, length2);
            this.midR = row;
            this.midC = col;
            this.length1 = length1;
            this.length2 = length2;
            this.phi = phi;
            this.rowsInit = new HTuple(new double[6]
            {
        -1.0,
        -1.0,
        1.0,
        1.0,
        0.0,
        0.0
            });
            this.colsInit = new HTuple(new double[6]
            {
        -1.0,
        1.0,
        1.0,
        -1.0,
        0.0,
        0.6
            });
            this.hom2D = new HHomMat2D();
            this.tmp = new HHomMat2D();
            this.updateHandlePos();
        }

        public override void createROI(double midX, double midY)
        {
            this.midR = midY;
            this.midC = midX;
            this.length1 = 100.0;
            this.length2 = 50.0;
            this.phi = 0.0;
            this.rowsInit = new HTuple(new double[6]
            {
        -1.0,
        -1.0,
        1.0,
        1.0,
        0.0,
        0.0
            });
            this.colsInit = new HTuple(new double[6]
            {
        -1.0,
        1.0,
        1.0,
        -1.0,
        0.0,
        0.6
            });
            this.hom2D = new HHomMat2D();
            this.tmp = new HHomMat2D();
            this.updateHandlePos();
        }

        public override void draw(HWindow window)
        {
            window.DispRectangle2(this.midR, this.midC, -this.phi, this.length1, this.length2);
            for (int index = 0; index < this.NumHandles; ++index)
                window.DispRectangle2(this.rows[index], this.cols[index], -this.phi, 5.0, 5.0);
            window.DispArrow(this.midR, this.midC, this.midR + Math.Sin(this.phi) * this.length1 * 1.2, this.midC + Math.Cos(this.phi) * this.length1 * 1.2, 5.0);
        }

        public override double distToClosestHandle(double x, double y)
        {
            double num = 10000.0;
            double[] numArray = new double[this.NumHandles];
            for (int index = 0; index < this.NumHandles; ++index)
                numArray[index] = HMisc.DistancePp(y, x, this.rows[index], this.cols[index]);
            for (int index = 0; index < this.NumHandles; ++index)
            {
                if (numArray[index] < num)
                {
                    num = numArray[index];
                    this.activeHandleIdx = index;
                }
            }
            return numArray[this.activeHandleIdx];
        }

        public override void displayActive(HWindow window)
        {
            window.DispRectangle2(this.rows[activeHandleIdx], this.cols[activeHandleIdx], -this.phi, 5.0, 5.0);
            if (this.activeHandleIdx != 5)
                return;
            window.DispArrow(this.midR, this.midC, this.midR + Math.Sin(this.phi) * this.length1 * 1.2, this.midC + Math.Cos(this.phi) * this.length1 * 1.2, 5.0);
        }

        public override HRegion getRegion()
        {
            HRegion hregion = new HRegion();
            hregion.GenRectangle2(this.midR, this.midC, -this.phi, this.length1, this.length2);
            return hregion;
        }

        public override HTuple getModelData()
        {
            return new HTuple(new double[5]
            {
        this.midR,
        this.midC,
        this.phi,
        this.length1,
        this.length2
            });
        }

        public override void moveByHandle(double newX, double newY)
        {
            double y = 0.0;
            switch (this.activeHandleIdx)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    this.tmp = this.hom2D.HomMat2dInvert();
                    double x = this.tmp.AffineTransPoint2d(newX, newY, out y);
                    this.length2 = Math.Abs(y);
                    this.length1 = Math.Abs(x);
                    this.checkForRange(x, y);
                    break;
                case 4:
                    this.midC = newX;
                    this.midR = newY;
                    break;
                case 5:
                    this.phi = Math.Atan2(newY - this.rows[4], newX - this.cols[4]);
                    break;
            }
            this.updateHandlePos();
        }

        private void updateHandlePos()
        {
            this.hom2D.HomMat2dIdentity();
            this.hom2D = this.hom2D.HomMat2dTranslate(this.midC, this.midR);
            this.hom2D = this.hom2D.HomMat2dRotateLocal(this.phi);
            this.tmp = this.hom2D.HomMat2dScaleLocal(this.length1, this.length2);
            this.cols = this.tmp.AffineTransPoint2d(this.colsInit, this.rowsInit, out this.rows);
        }

        private void checkForRange(double x, double y)
        {
            switch (this.activeHandleIdx)
            {
                case 0:
                    if (x < 0.0 && y < 0.0)
                        break;
                    if (x >= 0.0)
                        this.length1 = 0.01;
                    if (y < 0.0)
                        break;
                    this.length2 = 0.01;
                    break;
                case 1:
                    if (x > 0.0 && y < 0.0)
                        break;
                    if (x <= 0.0)
                        this.length1 = 0.01;
                    if (y < 0.0)
                        break;
                    this.length2 = 0.01;
                    break;
                case 2:
                    if (x > 0.0 && y > 0.0)
                        break;
                    if (x <= 0.0)
                        this.length1 = 0.01;
                    if (y > 0.0)
                        break;
                    this.length2 = 0.01;
                    break;
                case 3:
                    if (x < 0.0 && y > 0.0)
                        break;
                    if (x >= 0.0)
                        this.length1 = 0.01;
                    if (y > 0.0)
                        break;
                    this.length2 = 0.01;
                    break;
            }
        }
    }
}
