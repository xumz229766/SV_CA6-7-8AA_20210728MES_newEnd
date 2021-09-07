using System;
using System.Xml.Serialization;

namespace ViewWindow.Config
{
    [Serializable]
    public class Circle2
    {
        private string color = "yellow";
        private double _row;
        private double _column;
        private double _radius1;
        private double _radius2;
        [XmlElement(ElementName = "Row")]
        public double Row
        {
            get
            {
                return this._row;
            }
            set
            {
                this._row = value;
            }
        }

        [XmlElement(ElementName = "Column")]
        public double Column
        {
            get
            {
                return this._column;
            }
            set
            {
                this._column = value;
            }
        }

        [XmlElement(ElementName = "Radius1")]
        public double Radius1
        {
            get
            {
                return this._radius1;
            }
            set
            {
                this._radius1 = value;
            }
        }
        [XmlElement(ElementName = "Radius2")]
        public double Radius2
        {
            get
            {
                return this._radius2;
            }
            set
            {
                this._radius2 = value;
            }
        }
        [XmlElement(ElementName = "Color")]
        public string Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

        public Circle2()
        {
        }

        public Circle2(double row, double column, double radius1, double radius2)
        {
            this._row = row;
            this._column = column;
            this._radius1 = radius1;
            this._radius2 = radius2;
        }
    }
}
