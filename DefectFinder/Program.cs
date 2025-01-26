
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
        foreach (var l in logics)
        {
            var curL = new FindingsL();
            foreach (var p in l.Elements())
            {
                curL.SigVals.Add(int.Parse(p.Attribute("SigID").Value), int.Parse(p.Value));
            }
            curT.Logics.Add(curL);
        }
        ret.Triggers.Add(curT);
    }

    return ret;
}

Console.WriteLine("Start");

var og = scrape("E:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\data\\triggerscripts\\skirmishplayer.triggerscript");
var bad = scrape("E:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\test\\data\\triggerscripts\\skirmishplayer.triggerscript");

if (og.Triggers.Count != bad.Triggers.Count)
    Console.WriteLine("A");

for (int i = 0; i < og.Triggers.Count; i++)
{
    if (og.Triggers.ElementAt(i).Logics.Count != bad.Triggers.ElementAt(i).Logics.Count)
        Console.WriteLine("B");

    for (int j = 0; j < og.Triggers.ElementAt(i).Logics.Count; j++)
    {
        if (og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.Count != bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.Count)
            Console.WriteLine("C");

        foreach (var (sig, var) in og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals)
        {
            if (!bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
                Console.WriteLine("D");

            if (bad.varNulls[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != og.varNulls[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
                Console.WriteLine("E");

            if (bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                Console.WriteLine("E2");
                Console.WriteLine(var);
                Console.WriteLine(og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine(bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine();
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

        foreach (var (sig, var) in bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals)
        {
            if (!og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals.ContainsKey(sig))
                Console.WriteLine("F");

            if (og.varNulls[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != bad.varNulls[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
                Console.WriteLine("G");

            if (og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                Console.WriteLine("G2");
                Console.WriteLine(var);
                Console.WriteLine(og.varValues[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine(bad.varValues[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine();
            }

            if (og.varTypes[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]
                != bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]])
            {
                Console.WriteLine("M");
                Console.WriteLine(var);
                Console.WriteLine(og.varTypes[og.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine(bad.varTypes[bad.Triggers.ElementAt(i).Logics.ElementAt(j).SigVals[sig]]);
                Console.WriteLine();
            }
        }
    }
}

Console.WriteLine("End");

class FindingsL
{
    public Dictionary<int, int> SigVals = new Dictionary<int, int>();
}
class FindingsT
{
    public List<FindingsL> Logics = new List<FindingsL>();
}
class Findings
{
    public Dictionary<int, string> varValues = new Dictionary<int, string>();
    public Dictionary<int, bool> varNulls = new Dictionary<int, bool>();
    public Dictionary<int, string> varTypes = new Dictionary<int, string>();
    public List<FindingsT> Triggers = new List<FindingsT>();
};

