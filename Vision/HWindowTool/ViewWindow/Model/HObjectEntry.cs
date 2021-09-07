using HalconDotNet;
using System.Collections;

namespace ViewWindow.Model
{
  public class HObjectEntry
  {
    public Hashtable gContext;
    public HObject HObj;

    public HObjectEntry(HObject obj, Hashtable gc)
    {
      this.gContext = gc;
      this.HObj = obj;
    }

    public void clear()
    {
      this.gContext.Clear();
      ( this.HObj).Dispose();
    }
  }
}
