using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ViewWindow.Config
{
  public class SerializeHelper
  {
    public static void Save(object obj, string filename)
    {
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        new XmlSerializer(obj.GetType()).Serialize((Stream) fileStream, obj);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (fileStream != null)
          fileStream.Close();
      }
    }

    public static object Load(Type type, string filename)
    {
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return new XmlSerializer(type).Deserialize((Stream) fileStream);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (fileStream != null)
          fileStream.Close();
      }
    }

    public string ToXml<T>(T item)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
      StringBuilder output = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(output))
      {
        xmlSerializer.Serialize(xmlWriter,  item);
        return output.ToString();
      }
    }

    public T FromXml<T>(string str)
    {
      using (XmlReader xmlReader = (XmlReader) new XmlTextReader((TextReader) new StringReader(str)))
        return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
    }

    public string ToSoap<T>(T item)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new SoapFormatter().Serialize((Stream) memoryStream,  item);
        memoryStream.Position = 0L;
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load((Stream) memoryStream);
        return xmlDocument.InnerXml;
      }
    }

    public T FromSoap<T>(string str)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(str);
      SoapFormatter soapFormatter = new SoapFormatter();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        xmlDocument.Save((Stream) memoryStream);
        memoryStream.Position = 0L;
        return (T) soapFormatter.Deserialize((Stream) memoryStream);
      }
    }

    public string ToBinary<T>(T item)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new BinaryFormatter().Serialize((Stream) memoryStream,  item);
        memoryStream.Position = 0L;
        byte[] array = memoryStream.ToArray();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in array)
          stringBuilder.Append(string.Format("{0:X2}",  num));
        return stringBuilder.ToString();
      }
    }

    public T FromBinary<T>(string str)
    {
      int length = str.Length / 2;
      byte[] buffer = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        int int32 = Convert.ToInt32(str.Substring(index * 2, 2), 16);
        buffer[index] = (byte) int32;
      }
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        return (T) new BinaryFormatter().Deserialize((Stream) memoryStream);
    }

    public static byte[] GetBytes(object pObj)
    {
      if (pObj == null)
        return (byte[]) null;
      MemoryStream memoryStream = new MemoryStream();
      new BinaryFormatter().Serialize((Stream) memoryStream, pObj);
      memoryStream.Position = 0L;
      byte[] buffer = new byte[memoryStream.Length];
      memoryStream.Read(buffer, 0, buffer.Length);
      memoryStream.Close();
      return buffer;
    }

    public static XmlDocument GetXmlDoc(object pObj)
    {
      if (pObj == null)
        return (XmlDocument) null;
      XmlSerializer xmlSerializer = new XmlSerializer(pObj.GetType());
      StringBuilder sb = new StringBuilder();
      StringWriter stringWriter = new StringWriter(sb);
      xmlSerializer.Serialize((TextWriter) stringWriter, pObj);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(sb.ToString());
      stringWriter.Close();
      return xmlDocument;
    }

    public static T GetObject<T>(byte[] binData)
    {
      if (binData == null)
        return default (T);
      return (T) new BinaryFormatter().Deserialize((Stream) new MemoryStream(binData));
    }

    public static T GetObject<T>(XmlDocument xmlDoc)
    {
      if (xmlDoc == null)
        return default (T);
      return (T) new XmlSerializer(typeof (T)).Deserialize((XmlReader) new XmlNodeReader((XmlNode) xmlDoc.DocumentElement));
    }
  }
}
