//namespace Chef.Workspace2
//{
//    //public delegate float[] FuncLoadProperty(string item);
//    //public delegate string[] FuncLoadReference(string item);
//    //public delegate bool FuncSaveProperty(string item);
//    //public delegate bool FuncSaveReference(string item);

//    //public delegate float[] FuncConstrainProperty(float[] source, float[] reference);

//    //public struct PropertyInfoz
//    //{
//    //    public FuncLoadProperty Load;
//    //    public FuncSaveProperty Save;
//    //}
//    //public struct ReferenceInfo
//    //{
//    //    public string Type;
//    //    public FuncLoadReference Load;
//    //    public FuncSaveReference Save;
//    //}
//    //public struct ConstraintInfoz
//    //{
//    //    public string SrcProperty;
//    //    public string RefProperty;
//    //    public FuncConstrainProperty Constrain;
//    //}
//    //public class Item
//    //{
//    //    public Dictionary<string, float[]> Properties = [];
//    //    public Dictionary<string, string[]> References = [];
//    //}
//    //public class Type
//    //{
//    //    public Dictionary<string, Item> Items = [];
//    //    public Dictionary<string, PropertyInfo> PropertyInfo = [];
//    //    public Dictionary<string, ReferenceInfo> ReferenceInfo = [];
//    //    public List<PropertyConstraint> Constraints = [];
//    //}    
//    //public class WorkspaceCache
//    //{
//    //    public Dictionary<string, Type> Types = [];
//    //}   
//    //public class Workspace
//    //{
//    //    public static IEnumerable<string> Types(WorkspaceCache w)
//    //    {
//    //        return w.Types.Keys;
//    //    }
//    //    public static IEnumerable<string> TypeItems(WorkspaceCache w, string type)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return [];

//    //        return w.Types[type].Items.Keys;
//    //    }
//    //    public static IEnumerable<string> TypeProperties(WorkspaceCache w, string type)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return [];

//    //        return w.Types[type].PropertyInfo.Keys;
//    //    }
//    //    public static IEnumerable<string> TypeReferences(WorkspaceCache w, string type)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return [];

//    //        return w.Types[type].ReferenceInfo.Keys;
//    //    }

//    //    public static float[] PropertyValue(WorkspaceCache w, string type, string item, string property)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return null;

//    //        if (!w.Types[type].Items.ContainsKey(item))
//    //            return null;

//    //        if (!w.Types[type].PropertyInfo.ContainsKey(property))
//    //            return null;


//    //        if (!w.Types[type].Items[item].Properties.ContainsKey(property))
//    //        {
//    //            //obtain initial value here.
//    //            float[] value = w.Types[type].PropertyInfo[property].Load?.Invoke(item);
//    //            w.Types[type].Items[item].Properties[property] = value;
//    //        }

//    //        return w.Types[type].Items[item].Properties[property];
//    //    }

//    //    public static string ReferenceType(WorkspaceCache w, string type, string item, string reference)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return null;

//    //        if (!w.Types[type].ReferenceInfo.ContainsKey(reference))
//    //            return null;

//    //        string reftype = w.Types[type].ReferenceInfo[reference].Type;

//    //        if (!w.Types.ContainsKey(reftype))
//    //            return null;

//    //        return reftype;
//    //    }
//    //    public static string[] ReferenceValue(WorkspaceCache w, string type, string item, string reference)
//    //    {
//    //        if (!w.Types.ContainsKey(type))
//    //            return null;

//    //        if (!w.Types[type].Items.ContainsKey(item))
//    //            return null;

//    //        if (!w.Types[type].ReferenceInfo.ContainsKey(reference))
//    //            return null;


//    //        if (!w.Types[type].Items[item].References.ContainsKey(reference))
//    //        {
//    //            //obtain initial value here.
//    //            string[] value = w.Types[type].ReferenceInfo[reference].Load?.Invoke(item);
//    //            w.Types[type].Items[item].References[reference] = value;
//    //        }

//    //        return w.Types[type].Items[item].References[reference];
//    //    }
//    //}




//    public struct PropertyConstraint
//    {
//        public PropertyConstraint(PropertyConstrain constrain)
//        {
//            ReffedSubdir = [];
//            ReffedProperty = null;
//            DefaultValue = [];
//            Constrain = constrain;
//        }
//        public PropertyConstraint(PropertyConstrain constrain, float[] defaultvalue)
//        {
//            ReffedSubdir = [];
//            ReffedProperty = null;
//            DefaultValue = defaultvalue;
//            Constrain = constrain;
//        }
//        public PropertyConstraint(PropertyConstrain constrain, float[] defaultvalue, string[] reffedsubdir, string reffedproperty)
//        {
//            ReffedSubdir = reffedsubdir;
//            ReffedProperty = reffedproperty;
//            DefaultValue = defaultvalue;
//            Constrain = constrain;
//        }
//        public string[] ReffedSubdir;
//        public string ReffedProperty;
//        public float[] DefaultValue;
//        public PropertyConstrain Constrain;
//    }
//    public delegate float[] PropertyConstrain(float[] original, float[] reffed);
//    public class PropertyInfoa
//    {
//        public float[] InitialValue { get; set; }
//        public PropertyConstraint[] Constraints { get; set; }
//    }


//    public struct RefConstraint
//    {
//        public RefConstraint(RefConstrain constrain)
//        {
//            ReffedSubdir = [];
//            ReffedProperty = null;
//            DefaultValue = [];
//            Constrain = constrain;
//        }
//        public RefConstraint(RefConstrain constrain, string[][] defaultvalue)
//        {
//            ReffedSubdir = [];
//            ReffedProperty = null;
//            DefaultValue = defaultvalue;
//            Constrain = constrain;
//        }
//        public RefConstraint(RefConstrain constrain, string[][] defaultvalue, string[] reffedsubdir, string reffedproperty)
//        {
//            ReffedSubdir = reffedsubdir;
//            ReffedProperty = reffedproperty;
//            DefaultValue = defaultvalue;
//            Constrain = constrain;
//        }
//        public string[] ReffedSubdir;
//        public string ReffedProperty;
//        public string[][] DefaultValue;
//        public RefConstrain Constrain;
//    }
//    public delegate string[][] RefConstrain(string[][] original, float[] reffed);
//    public class RefInfo
//    {
//        public string[][] InitialValue { get; set; }
//        public RefConstraint[] Constraints { get; set; }
//    }


//    public struct PropertyInfo
//    {
//        public float[] InitialValue;
//    }
//    public struct ChildInfo
//    {
//        public string Type;
//        public bool IsRef;
//        public Path[] InitialValue;
//    }

//    public struct Path
//    {
//        public string[] elements;
//    }

//    public class TypeInfo
//    {
//        public TypeInfo() { }
//        public IDictionary<string, PropertyInfo> Properties = new Dictionary<string, PropertyInfo>();
//        public IDictionary<string, ChildInfo> Children = new Dictionary<string, ChildInfo>();
//    }

//    public class Workspacez
//    {
//        private TypeInfo root;
//        private Dictionary<string, TypeInfo> types;

//        public void SetTypeInfo(string name, TypeInfo ti)
//        {
//            types[name] = ti;
//        }

//        public string PathType(Path path)
//        {
//            var e = root.Children;
//            string ret = null;

//            for (int i = 0; i < path.elements.Length; i++)
//            {
//                if (!e.ContainsKey(path.elements[i]))
//                    return null;

//                var childType = e[path.elements[i]].Type;

//                if (!types.ContainsKey(childType))
//                    return null;

//                e = types[childType].Children;
//                ret = childType;
//            }

//            return ret;
//        }
//        public float[] PathProperty(Path path, string name)
//        {
//            string type = PathType(path);
//            if (type == null)
//                return null;

//            if (!types.ContainsKey(type))
//                return null;

//            var props = types[type].Properties;

//            if (!props.ContainsKey(name))
//                return null;

//            var prop = props[name];
//            var val = prop.InitialValue.ToArray();
//            return val;
//        }
//        public Path[] PathChild(Path path, string name)
//        {
//            string type = PathType(path);
//            if (type == null)
//                return null;

//            if (!types.ContainsKey(type))
//                return null;

//            var children = types[type].Children;

//            if (!children.ContainsKey(name))
//                return null;

//            var child = children[name];
//            var val = child.InitialValue.ToArray();
//            return val;
//        }

//    }



//    //public class Workspacex
//    //{
//    //    public Workspace()
//    //    {
//    //    }

//    //    public string RootType;
//    //    public Dictionary<string, FuncSubdirs> Enumerators;
//    //    public Dictionary<string, List<Constraint>> Constraints;

//    //    private class Element
//    //    {
//    //        public List<Constraint> constraints = [];
//    //        public float[] value = [];
//    //        public string type;
//    //    }
//    //    private Dictionary<string[], Element> elementInfo;

//    //    public IEnumerable<string[]> PathRefs(string[] path)
//    //    {

//    //    }
//    //}



//#if false
//    public class Element
//    {
//        public Element(string type, float[] value, string reftype)
//        {
//            Type = type;
//            Value = value;
//            RefType = reftype;
//        }

//        public string Type { get; private set; }
//        public float[] Value { get; private set; }
//        public string RefType { get; private set; }
//    }



//    public delegate Dictionary<string, string>  FuncTypeChildren();
//    public delegate void                        FuncTypeChildAdd(string name);
//    public delegate int                         FuncTypeValueCount();

//    public struct Type
//    {
//        public int ValueCount { get; set; }
//    }

//    public class Workspace
//    {
//        public string RootType { get; set; }

//        public Dictionary<string, FuncTypeChildren> TypeEnumerators;

//        public Dictionary<string, Dictionary<string, FuncTypeChildAdd>> PathAdders;
//        public Dictionary<string, Dictionary<string, FuncTypeValueCount>> PathValueLengths;
//        private Dictionary<string, Dictionary<string, Element>> PathChildren;

//        public Element Load(IEnumerable<string> path)
//        {
//            string curType = RootType;
//            string curPath = "/";
//            foreach(string p in path)
//            {
//                if (!PathChildren.ContainsKey(curPath))
//                {
//                    //bad type.
//                    if (!TypeEnumerators.ContainsKey(curType))
//                        return null;


//                    PathChildren[curPath] = new Dictionary<string, Element>();
//                    foreach (var (name, type) in TypeEnumerators[curType]())
//                    {
//                        PathChildren[curPath][name] = new Element(type, );
//                    }
//                }

//                //bad path element name.
//                if (!PathChildren[curPath].ContainsKey(p))
//                    return;

//                curType = PathChildren[curPath][p].Type;
//                curPath += p + "/";
//            }
//        }
//    }



//    public class WorkspaceRef
//    {
//        public WorkspaceRef()
//        {
//            Path = null;
//        }
//        public string[] Path { get; set; }
//    }
//    public class WorkspaceValue
//    {
//        public WorkspaceValue(int length, int decimalDigits, MidpointRounding roundingMode)
//        {
//            vals = new float[length];
//            DecimalDigits = decimalDigits;
//            RoundingMode = roundingMode;
//        }

//        public int DecimalDigits { get; private set; }
//        public MidpointRounding RoundingMode { get; private set; }
//        public int Length { get => vals.Length; }

//        private float[] vals;
//        public float this[int i]
//        {
//            get
//            {
//                return vals[i];
//            }
//            set
//            {
//                vals[i] = float.Round(value, DecimalDigits, RoundingMode);
//            }
//        }
//    }
//    public class WorkspaceDB
//    {
//        public delegate WorkspaceValue FuncTypePathValue(string[] path);
//        public delegate WorkspaceRef FuncTypePathRef(string[] path);

//        public FuncTypeEnum RootEnumerator;
//        public Dictionary<string, FuncTypeEnum> TypeEnumerators;
//        public Dictionary<string, FuncTypePathValue> TypePathValues;
//        public Dictionary<string, FuncTypePathRef> TypePathRefs;

//        private Dictionary<string[], string> CachedTypes;
//        private Dictionary<string[], WorkspaceValue> CachedValues;
//        private Dictionary<string[], WorkspaceRef> CachedRefs;

//        private string[] Resolve(string[] path)
//        {
//            string[] rest = path[1..^1];
//            string[] cur = path[0..1];

//            while (rest.Length > 0)
//            {
//                var curref = PathRef(cur);
//                if (curref != null)
//                {
//                    if (curref.Path == null)
//                        return null;
//                    cur = curref.Path;
//                }
//                else
//                {
//                    cur = [.. cur, .. rest.Take(1)];
//                }
//                rest = [.. rest.Skip(1)];
//            }

//            return cur;
//        }

//        public string PathType(string[] path)
//        {
//            path = Resolve(path);

//            if (!CachedTypes.ContainsKey(path))
//            {
//                var enums = RootEnumerator();
//                string curType = null;

//                for (int i = 0; i < path.Length; i++)
//                {
//                    var found = enums.Where(e => e.Name == path[i]);
//                    if (!found.Any())
//                        return null;

//                    curType = found.First().Type;

//                    if (!TypeEnumerators.ContainsKey(curType))
//                        return null;

//                    enums = TypeEnumerators[curType]();
//                }

//                CachedTypes[path] = curType;
//            }

//            return CachedTypes[path];
//        }

//        public IEnumerable<ElementInfo> PathSubpaths(string[] path)
//        {
//            path = Resolve(path);

//            string type = PathType(path);
//            if (type == null)
//                return [];

//            if (!TypeEnumerators.ContainsKey(type))
//                return [];

//            return TypeEnumerators[type]();
//        }

//        public WorkspaceValue PathValue(string[] path)
//        {
//            path = Resolve(path);

//            if (!CachedValues.ContainsKey(path))
//            {
//                string type = PathType(path);
//                if (type == null)
//                    return null;

//                if (!TypePathValues.ContainsKey(type))
//                    return null;

//                CachedValues[path] = TypePathValues[type](path);
//            }

//            return CachedValues[path];
//        }

//        public WorkspaceRef PathRef(string[] path)
//        {
//            path = Resolve(path);

//            if (!CachedRefs.ContainsKey(path))
//            {
//                string type = PathType(path);
//                if (type == null)
//                    return null;

//                if (!TypePathRefs.ContainsKey(type))
//                    return null;

//                CachedRefs[path] = TypePathRefs[type](path);
//            }

//            return CachedRefs[path];
//        }
//    }
//#endif
//}
//namespace Chef.Workspace
//{
//    //public delegate float Constrain(int index, float current, float inputs);
//    //public delegate float[] Constrain(float[] current, float[] inputs);
//    //public delegate void Constrain(Workspace w, ref float[] current);

//    //public struct ConstraintInfo
//    //{
//    //    public PathElement[] DestSubdir;
//    //    public string DestChild;
//    //    public PathElement[] InputSubdir;
//    //    public string InputChild;
//    //    public Constrain Constrain;
//    //}

//    public struct ChildInfo
//    {
//        public string Type;
//        public bool IsRef;
//    }

//    public class TypeInfo
//    {
//        public TypeInfo() { }
//        public IDictionary<string, ChildInfo> Children = new Dictionary<string, ChildInfo>();
//        public float DefaultValue;
//        //public Constrain Constrain;
//    }


//    public class PathElement
//    {
//        public string name;
//        public int index;
//    }


//    //public class Workspace
//    //{
//    //    private string Root;
//    //    private Dictionary<string, TypeInfo> Types;

//    //    private class PathData
//    //    {
//    //        public Dictionary<string, float[]> ChildValues = [];
//    //    }
//    //    private Dictionary<PathElement[], PathData> Values;
//    //    private class RefData
//    //    {
//    //        public Dictionary<string, PathElement[]> ChildRefs = [];
//    //        public List<PathElement[]> ReffedBy = [];
//    //    }
//    //    private Dictionary<PathElement[], RefData> Refs;


//    //    public string Type(PathElement[] path)
//    //    {
//    //        if (!Types.ContainsKey(Root))
//    //            return null;

//    //        string last = Root;

//    //        for (int i = 0; i < path.Elements.Length; i++)
//    //        {
//    //            if (!Types.ContainsKey(last))
//    //                return null;

//    //            string child = path.Elements[i].name;

//    //            if (!Types[last].Children.ContainsKey(child))
//    //                return null;

//    //            last = Types[last].Children[child].Type;
//    //        }

//    //        return last;
//    //    }


//    //    public float[] Value(PathElement[] path, string child)
//    //    {
//    //        string type = Type(path);

//    //        if (type == null)
//    //            return null;

//    //        if (!Types[type].Children.ContainsKey(child))
//    //            return null;

//    //        float[] data = [];

//    //        var cinfo = Types[type].Children[child];
//    //        if (!Refs.ContainsKey(path)) Refs[path] = new RefData();
//    //        if (Refs[path].ChildRefs.ContainsKey(child))
//    //        {
//    //            var refpath = Refs[path].ChildRefs[child];
//    //            data = Value(refpath, child);
//    //        }
//    //        else
//    //        {
//    //            if (!Values.ContainsKey(path)) 
//    //                Values[path] = new PathData();
//    //            if (!Values[path].ChildValues.ContainsKey(child))
//    //                Values[path].ChildValues[child] = [];
//    //            data = Values[path].ChildValues[child].ToArray();
//    //        }

//    //        Types[type].Constrain(this, ref data);

//    //        return data;
//    //    }
//    //}
//}

using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Chef.Workspace
{
    public delegate bool Add(string name);
    public delegate bool Remove(string name);

    public class Type
    {
        public string Name;
        public Add Add;
        public Remove Remove;
    }

    public class Node
    {
        public static char SEPARATOR = '/';

        private Node _Parent = null;
        private Type _Type;
        private string _Path = "";

        public Node Parent => _Parent;
        public string[] Name
        {
            get
            {
                if (Parent == null)
                    return [_PathName];
                return [.. Parent.Name, _PathName];
            }
        }


        private Dictionary<string, Node> children;
        public Node Add(string name, string type)
        {
            if (!children.ContainsKey(name))
            {
                if (!_TypeInfo.Subtypes.ContainsKey(type))
                    return null;

                if (!_TypeInfo.Subtypes[type].Add(name))
                    return null;

                //all good past here.
                Node n = new Node();
                n._Parent = this;
                n._TypeInfo = _TypeInfo.Subtypes[type];
                n._TypeName = type;
                n._PathName = name;

                children[name] = n;
            }

            return children[name];
        }
        public bool Remove(string name, string type)
        {
            if (!_TypeInfo.Subtypes.ContainsKey(type))
                return false;

            if (!_TypeInfo.Subtypes[type].Remove(name))
                return false;

            return children.Remove(name);
        }
        public IEnumerable<string> Enumerate()
        {
            return children.Keys;
        }


        public static bool ChildOf(Node node, Node child)
        {
            //TODO:
            return false;
        }
        public static bool CompareType(Node a, Node b)
        {
            if (a.Type.Length != b.Type.Length)
                return false;

            for (int i = 0; i < a.Type.Length; i++)
            {
                if (a.Type[i] != b.Type[i])
                    return false;
            }

            return true;
        }
    }

}

namespace Chef.Workspace.DD
{
    public delegate bool Add(string name);
    public delegate bool Remove(string name);



    public class Workspace
    {
        class PathData
        {
            public string Type;
            public Dictionary<string, PathData> Subdata = new Dictionary<string, PathData>();
        }
        class TypeData
        {
            public Add Add;
            public Remove Remove;
            public Dictionary<string, TypeData> Subtypes;
        }

        private PathData RootData;
        private TypeData RootType;

        public string Type(string path)
        {
            string type = "";
            PathData cur = RootData;

            foreach (string e in path.Split("/"))
            {
                if (!cur.Subdata.ContainsKey(e))
                    return null;

                PathData next = cur.Subdata[e];
                type += "/" + next.Type;
                cur = next;
            }

            return type;
        }

        public IEnumerable<string> Subtypes(string type)
        {
            TypeData cur = RootType;

            foreach(string e in type.Split("/"))
            {
                if (!cur.Subtypes.ContainsKey(e))
                    return null;

                cur = cur.Subtypes[e];
            }

            return cur.Subtypes.Keys;
        }
    }
}