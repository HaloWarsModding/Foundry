using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace Foundry.HW1.Serialization
{
    public static class Triggerscript
    {
        /// <summary>
        /// Reads a triggerscript from an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static HW1.Triggerscript.Triggerscript ReadXml(Stream stream)
        {
            YAXSerializer<HW1.Triggerscript.Triggerscript> ser = new YAXSerializer<HW1.Triggerscript.Triggerscript>();

            using (TextReader reader = new StreamReader(stream))
            {
                return ser.Deserialize(reader);
            }
        }
    }
}
