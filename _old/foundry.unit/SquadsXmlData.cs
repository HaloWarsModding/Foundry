using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace Foundry.Unit
{
    public class SquadsXmlData
    {
        public class Squad
        {
            [YAXAttributeForClass]
            [YAXSerializeAs("name")]
            public string Name { get; set; }

            public class UnitRef
            {

                [YAXAttributeForClass]
                [YAXSerializeAs("count")]
                public int Count { get; set; }

                public enum RoleType
                {
                    Normal
                }
                [YAXAttributeForClass]
                [YAXSerializeAs("role")]
                public RoleType Role { get; set; }

                public string Name { get; set; }
            }
            public List<UnitRef> Units { get; set; }
        }

        public List<Squad> Squads { get; set; }
    }
}
