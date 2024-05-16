using YAXLib;
using Foundry.HW1.Triggerscript;

void PrintEnum(Dictionary<int, Dictionary<int, ProtoLogic>> effects)
{
    List<string> enumLines = new List<string>();

    foreach (var e in effects)
    {
        foreach (var v in e.Value)
        {
            uint value = 0;
            value |= (uint)e.Key << 8;

            if (e.Value.Count > 1)
                value |= (uint)v.Key;

            string val = string.Format("{0} = 0x{1:X},\n", v.Value.Name, e.Key);
            if (!enumLines.Contains(val))
            {
                enumLines.Add(val);
            }
            //if (v.Key != -1)
            //    enumLines.Add(string.Format("{0}{1} = 0x{2:X},\n", v.Value.Name, v.Key, value));
            //else
            //    enumLines.Add(string.Format("{0} = 0x{1:X},\n", v.Value.Name, value));
        }
    }

    enumLines.Sort();

    Console.Write("public enum EffectType\n{\n");
    foreach (var e in enumLines)
    {
        Console.Write(e);
    }
    Console.Write("};");
}
void PrintVersionGetter(Dictionary<int, Dictionary<int, ProtoLogic>> logics)
{
    Console.Write("public static IEnumerable<int> ValidVersions(EffectType type)\n{\n");
    foreach(var e in logics)
    {
        if (e.Value.Values.Count == 1) continue;
        Console.Write(string.Format("if (type == EffectType.{0}) return new List<int>() ", e.Value.Values.First().Name));
        Console.Write("{ ");
        foreach (var v in e.Value.Keys)
        {
            if (v == e.Value.Keys.Last())
            {
                Console.Write(v + " };\n");
            }
            else
            {
                Console.Write(v + ", ");
            }
        }
    }
    Console.Write("return new List<int> { -1 }\n");
    Console.Write("}");
}

Dictionary<EffectType, List<int>> RepackEffVersions(Dictionary<int, Dictionary<int, ProtoLogic>> effects)
{
    Dictionary<EffectType, List<int>> ret = new Dictionary<EffectType, List<int>>();
    foreach( var v in effects)
    {
        ret.Add(Enum.Parse<EffectType>(v.Value.Values.First().Name), v.Value.Keys.ToList());
    }
    return ret;
}

YAXLib.YAXSerializer<Dictionary<int, Dictionary<int, ProtoLogic>>> ser = new YAXSerializer<Dictionary<int, Dictionary<int, ProtoLogic>>>();
Dictionary<int, Dictionary<int, ProtoLogic>> effects = ser.DeserializeFromFile("effects.tsdef");
Dictionary<int, Dictionary<int, ProtoLogic>> conditions = ser.DeserializeFromFile("conditions.tsdef");
//PrintEnum(effects);
//PrintEnum(conditions);
PrintVersionGetter(effects);

while (true) { }