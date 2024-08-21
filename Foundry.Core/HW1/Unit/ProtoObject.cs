using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Chef.HW1.Unit
{
    public class ProtoObject
    {
        [YAXAttributeForClass]
        [YAXSerializeAs("name")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Name { get; set; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = ObjectClass.Object)]
        public ObjectClass ObjectClass { get; set; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Visual { get; set; }
    }
}
