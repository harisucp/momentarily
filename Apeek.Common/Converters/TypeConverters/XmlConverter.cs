using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Apeek.Common.IO;
namespace Apeek.Common.Converters.TypeConverters
{
    public class XmlConverter<T>
    {
        public static T XmlToObject(string str)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(str))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
        public static List<T> XmlToObject(List<string> str)
        {
            List<T> list = new List<T>();
            foreach (var s in str)
            {
                list.Add(XmlToObject(s));
            }
            return list;
        }
        public static string ObjectToXml(T obj)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }
        public static List<string> ObjectToXml(List<T> obj)
        {
            List<string> list = new List<string>();
            foreach (var o in obj)
            {
                list.Add(ObjectToXml(o));
            }
            return list;
        }
    }
}
