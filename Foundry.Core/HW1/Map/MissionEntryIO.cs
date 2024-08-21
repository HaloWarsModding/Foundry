using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YAXLib;

namespace Chef.HW1.Map
{
    internal class MissionEntryIO
    {
        /// <summary>
        /// Reads a triggerscript from an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MissionEntry[] ReadXml(Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            List<MissionEntry> descs = new List<MissionEntry>();
            foreach(XElement element in doc.Elements("ScenarioDescriptions"))
            {
                try
                {
                    MissionEntry desc = new MissionEntry()
                    {
                        File = element.Attribute("File").Value,
                        LoadingImageName = element.Attribute("LoadingScreen").Value,
                        MapName = element.Attribute("MapName").Value,
                        Players = Enum.Parse<ScenarioPlayers>(element.Attribute("MaxPlayers").Value),
                        Type = Enum.Parse<ScenarioType>(element.Attribute("Type").Value),
                    };
                    descs.Add(desc);
                }
                catch(Exception e) { }
            }
            return descs.ToArray();
        }
    }
}