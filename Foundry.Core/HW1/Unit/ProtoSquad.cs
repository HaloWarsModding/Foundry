using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace Foundry.HW1.Unit
{
    public class ProtoSquad
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
}