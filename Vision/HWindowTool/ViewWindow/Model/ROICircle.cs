using HalconDotNet;
using System;
using System.Xml.Serialization;

namespace ViewWindow.Model
{
    [Serializable]
    public class ROICircle : ROI
    {
        private double radius;
        private double row1;
        private double col1;
        private double midR;
        private double midC;

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

        [XmlElement(ElementName = "Radius")]
        public double Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = value;
            }
        }

        public ROICircle()
        {
            this.NumHandles = 2;
            this.activeHandleIdx = 1;
        }

        public ROICircle(double row, double col, double radius):this()
        {
            this.createCircle1(row, col, radius);
        }

        public override void createCircle1(double row, double col, double radius)
        {
            base.createCircle1(row, col, radius);
            this.midR = row;
            this.midC = col;
            this.radius = radius;
            this.row1 = this.midR;
            this.col1 = this.midC + radius;
        }

        public override void createROI(double midX, double midY)
        {
            this.midR = midY;
            this.midC = midX;
            this.radius = 100.0;
            this.row1 = this.midR;
            this.col1 = this.midC + this.radius;
        }

        public override void draw(HWindow window)
        {
            window.DispCircle(this.midR, this.midC, this.radius);
            window.DispRectangle2(this.row1, this.col1, 0.0, 5.0, 5.0);
            window.DispRectangle2(this.midR, this.midC, 0.0, 5.0, 5.0);
        }

        public override double distToClosestHandle(double x, double y)
        {
            double num = 10000.0;
            double[] numArray = new double[this.NumHandles];
            numArray[0] = HMisc.DistancePp(y, x, this.row1, this.col1);
            numArray[1] = HMisc.DistancePp(y, x, this.midR, this.midC);
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
            switch (this.activeHandleIdx)
            {
                case 0:
                    window.DispRectangle2(this.row1, this.col1, 0.0, 5.0, 5.0);
                    break;
                case 1:
                    window.DispRectangle2(this.midR, this.midC, 0.0, 5.0, 5.0);
                    break;
            }
        }

        public override HRegion getRegion()
        {
            HRegion hregion = new HRegion();
            hregion.GenCircle(this.midR, this.midC, this.radius);
            return hregion;
        }

        public override double getDistanceFromStartPoint(double row, double col)
        {
            double num = HMisc.AngleLl(this.midR, this.midC, this.midR, this.midC + 1.0 * this.radius, this.midR, this.midC, row, col);
            if (num < 0.0)
                num += 2.0 * Math.PI;
            return this.radius * num;
        }

        public override HTuple getModelData()
        {
            return new HTuple(new double[3]
            {
        this.midR,
        this.midC,
        this.radius
            });
        }

        public override void moveByHandle(double newX, double newY)
        {
            switch (this.activeHandleIdx)
            {
                case 0:
                    this.row1 = newY;
                    this.col1 = newX;
                    HTuple htuple;
                    HOperatorSet.DistancePp(new HTuple(this.row1), new HTuple(this.col1), new HTuple(this.midR), new HTuple(this.midC), out htuple);
                    this.radius = htuple[0];
                    break;
                case 1:
                    double num1 = this.midR - newY;
                    double num2 = this.midC - newX;
                    this.midR = newY;
                    this.midC = newX;
                    this.row1 -= num1;
                    this.col1 -= num2;
                    break;
            }
        }
    }
}
