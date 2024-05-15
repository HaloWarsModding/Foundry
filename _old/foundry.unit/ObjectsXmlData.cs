using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Foundry.Unit
{
    public class ObjectsXmlData
    {
        public class Object
        {
            public class EditorDataClass
            {
                [YAXCollection(YAXCollectionSerializationTypes.Serially,
                   SeparateBy = "/")]
                [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                public List<string> Group { get; set; }
            }
            [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
            public EditorDataClass EditorData { get; set; }


            [YAXAttributeForClass]
            [YAXSerializeAs("name")]
            [YAXErrorIfMissed(YAXExceptionTypes.Error)]
            public string Name { get; set; }

            public enum ObjectClassEnum
            {
                Object,
                Squad,
                Unit,
                Building,
                Projectile,
            }
            [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = ObjectClassEnum.Object)]
            public ObjectClassEnum ObjectClass { get; set; }


            [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
            public string Visual { get; set; }
        }

        public List<Object> Objects { get; set; }
    }
}
