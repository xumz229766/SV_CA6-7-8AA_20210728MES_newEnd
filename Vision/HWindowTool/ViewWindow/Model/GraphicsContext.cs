using HalconDotNet;
using System;
using System.Collections;

namespace ViewWindow.Model
{
  public class GraphicsContext
  {
    public const string GC_COLOR = "Color";
    public const string GC_COLORED = "Colored";
    public const string GC_LINEWIDTH = "LineWidth";
    public const string GC_DRAWMODE = "DrawMode";
    public const string GC_SHAPE = "Shape";
    public const string GC_LUT = "Lut";
    public const string GC_PAINT = "Paint";
    public const string GC_LINESTYLE = "LineStyle";
    private Hashtable graphicalSettings;
    public Hashtable stateOfSettings;
    private IEnumerator iterator;
    public GCDelegate gcNotification;

    public GraphicsContext()
    {
      this.graphicalSettings = new Hashtable(10, 0.2f);
      this.gcNotification = new GCDelegate(this.dummy);
      this.stateOfSettings = new Hashtable(10, 0.2f);
    }

    public GraphicsContext(Hashtable settings)
    {
      this.graphicalSettings = settings;
      this.gcNotification = new GCDelegate(this.dummy);
      this.stateOfSettings = new Hashtable(10, 0.2f);
    }

    public void applyContext(HWindow window, Hashtable cContext)
    {
      string str1 = "";
      int num = -1;
      HTuple htuple = (HTuple) null;
      this.iterator = cContext.Keys.GetEnumerator();
      try
      {
        while (this.iterator.MoveNext())
        {
          string str2 = (string) this.iterator.Current;
          if (!this.stateOfSettings.Contains( str2) || this.stateOfSettings[ str2] != cContext[ str2])
          {
            switch (str2)
            {
              case "Color":
                str1 = (string) cContext[ str2];
                window.SetColor(str1);
                if (this.stateOfSettings.Contains( "Colored"))
                {
                  this.stateOfSettings.Remove( "Colored");
                  break;
                }
                break;
              case "Colored":
                num = (int) cContext[ str2];
                window.SetColored(num);
                if (this.stateOfSettings.Contains( "Color"))
                {
                  this.stateOfSettings.Remove( "Color");
                  break;
                }
                break;
              case "DrawMode":
                str1 = (string) cContext[ str2];
                window.SetDraw(str1);
                break;
              case "LineWidth":
                num = (int) cContext[ str2];
                window.SetLineWidth(num);
                break;
              case "Lut":
                str1 = (string) cContext[ str2];
                window.SetLut(str1);
                break;
              case "Paint":
                str1 = (string) cContext[ str2];
                window.SetPaint((str1));
                break;
              case "Shape":
                str1 = (string) cContext[ str2];
                window.SetShape(str1);
                break;
              case "LineStyle":
                htuple = (HTuple) cContext[ str2];
                window.SetLineStyle(htuple);
                break;
            }
            if (num != -1)
            {
              if (this.stateOfSettings.Contains( str2))
                this.stateOfSettings[ str2] =  num;
              else
                this.stateOfSettings.Add( str2,  num);
              num = -1;
            }
            else if (str1 != "")
            {
              if (this.stateOfSettings.Contains( str2))
                this.stateOfSettings[ str2] =  num;
              else
                this.stateOfSettings.Add( str2,  num);
              str1 = "";
            }
            else if (htuple != null)
            {
              if (this.stateOfSettings.Contains( str2))
                this.stateOfSettings[ str2] =  num;
              else
                this.stateOfSettings.Add( str2,  num);
              htuple = (HTuple) null;
            }
          }
        }
      }
      catch (HOperatorException ex)
      {
        this.gcNotification(((Exception) ex).Message);
      }
    }

    public void setColorAttribute(string val)
    {
      if (this.graphicalSettings.ContainsKey( "Colored"))
        this.graphicalSettings.Remove( "Colored");
      this.addValue("Color", val);
    }

    public void setColoredAttribute(int val)
    {
      if (this.graphicalSettings.ContainsKey( "Color"))
        this.graphicalSettings.Remove( "Color");
      this.addValue("Colored", val);
    }

    public void setDrawModeAttribute(string val)
    {
      this.addValue("DrawMode", val);
    }

    public void setLineWidthAttribute(int val)
    {
      this.addValue("LineWidth", val);
    }

    public void setLutAttribute(string val)
    {
      this.addValue("Lut", val);
    }

    public void setPaintAttribute(string val)
    {
      this.addValue("Paint", val);
    }

    public void setShapeAttribute(string val)
    {
      this.addValue("Shape", val);
    }

    public void setLineStyleAttribute(HTuple val)
    {
      this.addValue("LineStyle", val);
    }

    private void addValue(string key, int val)
    {
      if (this.graphicalSettings.ContainsKey( key))
        this.graphicalSettings[ key] =  val;
      else
        this.graphicalSettings.Add( key,  val);
    }

    private void addValue(string key, string val)
    {
      if (this.graphicalSettings.ContainsKey( key))
        this.graphicalSettings[ key] =  val;
      else
        this.graphicalSettings.Add( key,  val);
    }

    private void addValue(string key, HTuple val)
    {
      if (this.graphicalSettings.ContainsKey( key))
        this.graphicalSettings[ key] =  val;
      else
        this.graphicalSettings.Add( key,  val);
    }

    public void clear()
    {
      this.graphicalSettings.Clear();
    }

    public GraphicsContext copy()
    {
      return new GraphicsContext((Hashtable) this.graphicalSettings.Clone());
    }

    public object getGraphicsAttribute(string key)
    {
      if (this.graphicalSettings.ContainsKey( key))
        return this.graphicalSettings[ key];
      return  null;
    }

    public Hashtable copyContextList()
    {
      return (Hashtable) this.graphicalSettings.Clone();
    }

    public void dummy(string val)
    {
    }
  }
}
