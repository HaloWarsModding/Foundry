using System.ComponentModel;
using System.Runtime.Remoting;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

Findings scrape(string file)
{
    XDocument doc = XDocument.Load(file);
    Findings ret = new Findings();
    
    foreach (var v in doc.Root.Element("TriggerVars").Elements())
    {
        ret.varValues[int.Parse(v.Attribute("ID").Value)] = v.Value;
        ret.varNulls[int.Parse(v.Attribute("ID").Value)] = bool.Parse(v.Attribute("IsNull").Value);
        ret.varTypes[int.Parse(v.Attribute("ID").Value)] = v.Attribute("Type").Value;
    }

    foreach (var t in doc.Root.Element("Triggers").Elements())
    {
        List<XElement> logics = new List<XElement>();

        var cnd = t.Element("TriggerConditions");
        if (cnd.Element("And") != null)
        {
            logics.AddRange(cnd.Element("And").Elements());
        }
        if (cnd.Element("Or") != null)
        {
            logics.AddRange(cnd.Element("Or").Elements());
        }

        logics.AddRange(t.Element("TriggerEffectsOnTrue").Elements());
        logics.AddRange(t.Element("TriggerEffectsOnFalse").Elements());

        var curT = new FindingsT();
        curT.name = t.Attribute("Name").Value;
        curT.id = int.Parse(t.Attribute("ID").Value);
        curT.active = bool.Parse(t.Attribute("Active").Value);
        foreach (var l in logics)
        {
            var curL = new FindingsL();
            curL.dbid = int.Parse(l.Attribute("DBID").Value);
            curL.ver = int.Parse(l.Attribute("Version").Value);
            foreach (var p in l.Elements())
            {
                curL.SigVals.Add(int.Parse(p.Attribute("SigID").Value), int.Parse(p.Value));
                bool opt;
                if (p.Attribute("Optional") != null 
                    && bool.TryParse(p.Attribute("Optional").Value, out opt))
                {
                    curL.sigOptionals.Add(int.Parse(p.Attribute("SigID").Value), opt);
                }
            }
            curT.Logics.Add(curL);
        }
        ret.Triggers.Add(curT);
    }

    return ret;
}

var og = scrape("E:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\data\\triggerscripts\\skirmishplayer.triggerscript");
var bad = scrape("E:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\test\\data\\triggerscripts\\skirmishplayer.triggerscript");


void CallStack(Findings script, FindingsT cur, Dictionary<int, List<int>> found)
{
    if (!found.ContainsKey(cur.id))
        found[cur.id] = new List<int>();

    Console.WriteLine(cur.name);
    foreach (var l in cur.Logics)
    {
        if (l.dbid == 31 && l.ver == 1)
        {
            int nextVarId = l.SigVals[1];
            int nextTriggerId = int.Parse(script.varValues[nextVarId]);
            if (!found[cur.id].Contains(nextTriggerId))
            {
                found[cur.id].Add(nextTriggerId);
                FindingsT next = script.Triggers.Where(t => t.id == nextTriggerId).First();
                CallStack(script, next, found);
            }
        }
    }
}

Dictionary<int, List<int>> found;

Console.WriteLine("Og");
found = new Dictionary<int, List<int>>();
foreach (var a in og.Triggers.Where(t => t.active))
{
    CallStack(og, a, found);
}
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Bad");
found = new Dictionary<int, List<int>>();
foreach (var a in bad.Triggers.Where(t => t.active))
{
    CallStack(bad, a, found);
}
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();


if (og.Triggers.Count != bad.Triggers.Count)
{
    Console.WriteLine("A");
    Console.WriteLine("A");
    Console.WriteLine("A");
    Console.WriteLine("A");
    Console.WriteLine("A");
}
for (int i = 0; i < og.Triggers.Count; i++)
{
    if (og.Triggers.ElementAt(i).Logics.Count != bad.Triggers.ElementAt(i).Logics.Count)
    {
        Console.WriteLine("B");
        Console.WriteLine("B");
        Console.WriteLine("B");
        Console.WriteLine("B");
        Console.WriteLine("B");
    }

    for (int j = 0; j < og.Triggers.ElementAt(i).Logics.Count; j++)
    {
        if (og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.Count != bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.Count)
        {
            Console.WriteLine("C");
            Console.WriteLine("C");
            Console.WriteLine("C");
            Console.WriteLine("C");
            Console.WriteLine("C");
        }

        foreach (var (sig, var) in og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals)
        {
            if (!bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
            {
                Console.WriteLine("Z");
                Console.WriteLine(sig);
                Console.WriteLine(og.Triggers.ElementAt(i).Logics.ElementAt(i).dbid);
                Console.WriteLine(og.Triggers.ElementAt(i).Logics.ElementAt(i).ver);
                Console.WriteLine();
            }
            if (bad.Triggers.ElementAt(i).Logics.ElementAt(j).dbid != og.Triggers.ElementAt(i).Logics.ElementAt(j).dbid)
            {
                Console.WriteLine("Q");
                Console.WriteLine("Q");
                Console.WriteLine("Q");
                Console.WriteLine("Q");
                Console.WriteLine("Q");
            }
            if (bad.Triggers.ElementAt(i).Logics.ElementAt(j).ver != og.Triggers.ElementAt(i).Logics.ElementAt(j).ver)
            {
                Console.WriteLine("R");
                Console.WriteLine("R");
                Console.WriteLine("R");
                Console.WriteLine("R");
                Console.WriteLine("R");
            }

            if (!bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
            {
                Console.WriteLine("D");
                Console.WriteLine("D");
                Console.WriteLine("D");
                Console.WriteLine("D");
                Console.WriteLine("D");
            }

            if (bad.varNulls[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != og.varNulls[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                Console.WriteLine("E");
                Console.WriteLine("E");
                Console.WriteLine("E");
                Console.WriteLine("E");
                Console.WriteLine("E");
            }

            if (bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                if (bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]] != "Trigger")
                {
                    Console.WriteLine("=========");
                    Console.WriteLine(bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                    Console.WriteLine(bad.varNulls[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                    Console.WriteLine(og.varNulls[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                    Console.WriteLine(bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]);
                    Console.WriteLine(og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]);
                    Console.WriteLine(bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                    Console.WriteLine(og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                    Console.WriteLine();
                }
            }

            if (bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != og.varTypes[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                Console.WriteLine("L");
                Console.WriteLine(var);
                Console.WriteLine(og.varTypes[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine(bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine();
            }
        }

        foreach (var (sig, opt) in og.Triggers.ElementAt(i).Logics.ElementAt(j).sigOptionals)
        {
            if (bad.varNulls[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                && !opt)
            {
                Console.WriteLine("AAAAAAAAAAAAAAAAAA");
                Console.WriteLine("AAAAAAAAAAAAAAAAAA");
                Console.WriteLine("AAAAAAAAAAAAAAAAAA");
                Console.WriteLine("AAAAAAAAAAAAAAAAAA");
                Console.WriteLine();
            }
        }

        foreach (var (sig, val) in og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals)
        {
            if (!bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
            {
                Console.WriteLine("Y");
                Console.WriteLine("Y");
                Console.WriteLine("Y");
                Console.WriteLine("Y");
                Console.WriteLine();
            }
        }
        foreach (var (sig, val) in bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals)
        {
            if (!og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
            {
                Console.WriteLine("N");
                Console.WriteLine("N");
                Console.WriteLine("N");
                Console.WriteLine("N");
                Console.WriteLine();
            }
        }
    }
}

Console.WriteLine("End");

class FindingsL
{
    public Dictionary<int, int> SigVals = new Dictionary<int, int>();
    public Dictionary<int, bool> sigOptionals = new Dictionary<int, bool>();
    public int dbid, ver;
}
class FindingsT
{
    public int id;
    public string name;
    public bool active;
    public List<FindingsL> Logics = new List<FindingsL>();
}
class Findings
{
    public Dictionary<int, string> varValues = new Dictionary<int, string>();
    public Dictionary<int, bool> varNulls = new Dictionary<int, bool>();
    public Dictionary<int, string> varTypes = new Dictionary<int, string>();
    public List<FindingsT> Triggers = new List<FindingsT>();
};

