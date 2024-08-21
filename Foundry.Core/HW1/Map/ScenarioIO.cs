using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace Chef.HW1.Map
{
    public static class ScenarioIO
    {
        /// <summary>
        /// Reads a triggerscript from an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Scenario ReadXml(Stream stream)
        {
            YAXSerializer<Scenario> ser = new YAXSerializer<Scenario>();

            using (TextReader reader = new StreamReader(stream))
            {
                return ser.Deserialize(reader);
            }
        }
    }
}
