using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using YAXLib;

namespace Chef.HW1.Unit
{
    public static class ProtoObjectIO
    {
        public static ProtoObject[] ReadXml(Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            XElement objects = doc.Element("Objects");

            YAXSerializer<ProtoObject[]> ser = new YAXSerializer<ProtoObject[]>();
            return ser.Deserialize(objects);
        }
    }
}