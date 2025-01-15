﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace Chef.HW1.Script
{
    public static class TriggerscriptIO
    {
        /// <summary>
        /// Reads a triggerscript from an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Triggerscript ReadXml(Stream stream)
        {
            YAXSerializer<Triggerscript> ser = new YAXSerializer<Triggerscript>();

            using (TextReader reader = new StreamReader(stream))
            {
                var script = ser.Deserialize(reader);
                TriggerscriptHelpers.Validate(script);

#if DEBUG
                ser.SerializeToFile(script, "C:\\users\\jaken\\desktop\\out.triggerscript");
#endif

                return script;
            }
        }
    }
}
