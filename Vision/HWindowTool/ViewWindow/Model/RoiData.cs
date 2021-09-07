using HalconDotNet;
using System.Xml.Serialization;
using ViewWindow.Config;

namespace ViewWindow.Model
{
    public class RoiData
    {
        private int _id;
        private string _name;
        private Rectangle1 _rectangle1;
        private Rectangle2 _rectangle2;
        private Circle _circle;
        private Circle2 _circle2;
        private Line _line;

        [XmlElement(ElementName = "ID")]
        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        [XmlElement(ElementName = "Rectangle1")]
        public Rectangle1 Rectangle1
        {
            get
            {
                return this._rectangle1;
            }
            set
            {
                this._rectangle1 = value;
            }
        }

        [XmlElement(ElementName = "Rectangle2")]
        public Rectangle2 Rectangle2
        {
            get
            {
                return this._rectangle2;
            }
            set
            {
                this._rectangle2 = value;
            }
        }

        [XmlElement(ElementName = "Circle")]
        public Circle Circle
        {
            get
            {
                return this._circle;
            }
            set
            {
                this._circle = value;
            }
        }
        [XmlElement(ElementName = "Circle2")]
        public Circle2 Circle2
        {
            get
            {
                return this._circle2;
            }
            set
            {
                this._circle2 = value;
            }
        }
        [XmlElement(ElementName = "Line")]
        public Line Line
        {
            get
            {
                return this._line;
            }
            set
            {
                this._line = value;
            }
        }

        protected internal RoiData()
        {
        }

        protected internal RoiData(int id, ROI roi)
        {
            this._id = id;
            HTuple modelData = roi.getModelData();
            switch (roi.Type)
            {
                case "ROIRectangle1":
                    this._name = "Rectangle1";
                    if (modelData == null)
                        break;
                    this._rectangle1 = new Rectangle1(modelData[0], modelData[1], modelData[2], modelData[3]);
                    this._rectangle1.Color = roi.Color;
                    break;
                case "ROIRectangle2":
                    this._name = "Rectangle2";
                    if (modelData == null)
                        break;
                    this._rectangle2 = new Rectangle2(modelData[0], modelData[1], modelData[2], modelData[3], modelData[4]);
                    this._rectangle2.Color = roi.Color;
                    break;
                case "ROICircle":
                    this._name = "Circle";
                    if (modelData == null)
                        break;
                    this._circle = new Circle(modelData[0], modelData[1], modelData[2]);
                    this._circle.Color = roi.Color;
                    break;
                case "ROILine":
                    this._name = "Line";
                    if (modelData == null)
                        break;
                    this._line = new Line(modelData[0], modelData[1], modelData[2], modelData[3]);
                    this._line.Color = roi.Color;
                    break;
            }
        }

        protected internal RoiData(int id, Rectangle1 rectangle1)
        {
            this._id = id;
            this._name = "Rectangle1";
            this._rectangle1 = rectangle1;
        }

        protected internal RoiData(int id, Rectangle2 rectangle2)
        {
            this._id = id;
            this._name = "Rectangle2";
            this._rectangle2 = rectangle2;
        }

        protected internal RoiData(int id, Circle circle)
        {
            this._id = id;
            this._name = "Circle";
            this._circle = circle;
        }

        protected internal RoiData(int id, Line line)
        {
            this._id = id;
            this._name = "Line";
            this._line = line;
        }
    }
}
