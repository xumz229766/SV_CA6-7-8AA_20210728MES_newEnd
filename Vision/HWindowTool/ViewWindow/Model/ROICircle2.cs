using HalconDotNet;
using System;
using System.Xml.Serialization;

namespace ViewWindow.Model
{
    [Serializable]
    public class ROICircle2 : ROI
    {
        private double radius1;
        private double radius2;
        private double row1;
        private double col1;
        private double row2;
        private double col2;
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

        [XmlElement(ElementName = "Radius1")]
        public double Radius1
        {
            get
            {
                return this.radius1;
            }
            set
            {
                this.radius1 = value;
            }
        }
        [XmlElement(ElementName = "Radius2")]
        public double Radius2
        {
            get
            {
                return this.radius2;
            }
            set
            {
                this.radius2 = value;
            }
        }
        public ROICircle2()
        {
            this.NumHandles = 3;
            this.activeHandleIdx = 1;
        }

        public ROICircle2(double row, double col, double radius1, double radius2):this()
        {
            this.createCircle2(row, col, radius1, radius2);
        }

        public override void createCircle2(double row, double col, double radius1, double radius2)
        {
            base.createCircle2(row, col, radius1, radius2);
            this.midR = row;
            this.midC = col;
            this.radius1 = radius1;
            this.radius2 = radius2;
            this.row1 = this.midR;
            this.col1 = this.midC + radius1;
            this.row2 = this.midR;
            this.col2 = this.midC + radius2;
        }

        public override void createROI(double midX, double midY)
        {
            this.midR = midY;
            this.midC = midX;
            this.radius1 = 50;
            this.radius2 = 100;
            this.row1 = this.midR;
            this.col1 = this.midC + radius1;
            this.row2 = this.midR;
            this.col2 = this.midC + radius2;
        }

        public override void draw(HWindow window)
        {
            window.DispCircle(this.midR, this.midC, this.radius1);
            window.DispCircle(this.midR, this.midC, this.radius2);
            window.DispRectangle2(this.row1, this.col1, 0.0, 5.0, 5.0);
            window.DispRectangle2(this.row2, this.col2, 0.0, 5.0, 5.0);
            window.DispRectangle2(this.midR, this.midC, 0.0, 5.0, 5.0);
        }

        public override double distToClosestHandle(double x, double y)
        {
            double num = 10000.0;
            double[] numArray = new double[this.NumHandles];
            numArray[0] = HMisc.DistancePp(y, x, this.row1, this.col1);
            numArray[1] = HMisc.DistancePp(y, x, this.row2, this.col2);
            numArray[2] = HMisc.DistancePp(y, x, this.midR, this.midC);
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
                    window.DispRectangle2(this.row2, this.col2, 0.0, 5.0, 5.0);
                    break;
                case 2:
                    window.DispRectangle2(this.midR, this.midC, 0.0, 5.0, 5.0);
                    break;
            }
        }

        public override HRegion getRegion()
        {
            HRegion hregion1 = new HRegion();
            HRegion hregion2 = new HRegion();
            HObject ho_region;
            HOperatorSet.GenEmptyObj(out ho_region);
            ho_region.Dispose();
            hregion1.GenCircle(this.midR, this.midC, this.radius1);
            hregion2.GenCircle(this.midR, this.midC, this.radius2);
            HOperatorSet.Difference(hregion2, hregion1, out ho_region);
            return new HRegion(ho_region);
        }
        /// <summary>
        /// 从起点获得距离
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public override double getDistanceFromStartPoint(double row, double col)
        {
            double num = HMisc.AngleLl(this.midR, this.midC, this.midR, this.midC + 1.0 * this.radius1, this.midR, this.midC, row, col);
            if (num < 0.0)
                num += 2.0 * Math.PI;
            return this.radius1 * num;
        }

        public override HTuple getModelData()
        {
            return new HTuple(new double[4]
            {
        this.midR,
        this.midC,
        this.radius1,
        this.radius2
            });
        }

        public override void moveByHandle(double newX, double newY)
        {
            switch (this.activeHandleIdx)
            {
                case 0:
                    this.row1 = newY;
                    this.col1 = newX;
                    HTuple htuple1;
                    HOperatorSet.DistancePp(new HTuple(this.row1), new HTuple(this.col1), new HTuple(this.midR), new HTuple(this.midC), out htuple1);
                    this.radius1 = htuple1[0];
                    break;
                case 1:
                    this.row2 = newY;
                    this.col2 = newX;
                    HTuple htuple2;
                    HOperatorSet.DistancePp(new HTuple(this.row2), new HTuple(this.col2), new HTuple(this.midR), new HTuple(this.midC), out htuple2);
                    this.radius2 = htuple2[0];
                    break;
                case 2:
                    double num1 = this.midR - newY;
                    double num2 = this.midC - newX;
                    this.midR = newY;
                    this.midC = newX;
                    this.row1 -= num1;
                    this.col1 -= num2;
                    this.row2 -= num1;
                    this.col2 -= num2;
                    break;
            }
        }
    }
}
