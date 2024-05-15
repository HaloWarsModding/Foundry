using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;
using YAXLib.Attributes;

namespace Foundry.Triggerscript
{
    public class UserClassEntryField
    {
        [YAXAttributeForClass]
        public string Name { get; set; }
        [YAXAttributeForClass]
        public VarType Type { get; set; }
    }

    public class UserClassEntry
    {
        [YAXAttributeForClass]
        public int DBID { get; set; }
        [YAXAttributeForClass]
        public string Name { get; set; }

        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Fields")]
        public List<UserClassEntryField> Fields { get; set; }
    }
    
    public class UserClassesXml
    {
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "UserClass")]
        public List<UserClassEntry> UserClasses { get; set; }
    }
}
