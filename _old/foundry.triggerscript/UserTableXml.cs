using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;
using YAXLib.Attributes;

namespace Foundry.Triggerscript
{
    public class UserTableRow
    {
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "c")]
        public List<string> Columns { get; set; }
    }

    public class UserTable
    {
        [YAXAttributeForClass()]
        public string Name { get; set; }
        [YAXAttributeForClass()]
        public string Type { get; set; }

        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Row")]
        public List<UserTableRow> Rows { get; set; }
    }

    public class UserTablesXml
    {
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Table")]
        public List<UserTable> Tables;
    }
}
