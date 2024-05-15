using Foundry.Triggerscript;
using static Foundry.Triggerscript.ScriptModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundry;

//namespace Foundry.Triggerscript
//{
//    public class ScriptData : BaseNodeEditorData
public class ScriptData : BaseNodeEditorData
{
    public ScriptData(FoundryInstance i) : base(i)
    {
    }
}
//    {
//        private class TriggerNode : BaseNode
//        {
//            public static Color TypeColor { get; private set; } = Color.FromArgb(55, 100, 190);

//            public ScriptData EditorData { get; private set; }
//            public ScriptXml.TriggerClass TriggerData { get; private set; }
//            public ScriptXml.TriggerVarClass TriggerVariableData { get; private set; }
//            public TriggerNode(ScriptData data)
//            {
//                TriggerData = new ScriptXml.TriggerClass();
//                TriggerData.ID = data.NextTriggerID++;

//                TriggerVariableData = new ScriptXml.TriggerVarClass();
//                TriggerVariableData.ID = data.NextVariableID++;
//                TriggerVariableData.Name = "TriggerVar" + TriggerData.ID;
//                TriggerVariableData.Type = "Trigger";
//                TriggerVariableData.Value = TriggerData.ID.ToString();

//                EditorData = data;


//                HeaderColor = TypeColor;
//                Subtext = "Trigger";

//                Conditions = new BaseNodeSocket(this)
//                {
//                    Text = "Conditions",
//                    Direction = BaseNodeSocketDirection.Out,
//                    Flags = BaseNodeSocketFlags.Restrict | BaseNodeSocketFlags.MultiConnect,
//                    Color = ConditionNode.TypeColor,
//                    Restriction = "__CONDITION__",
//                };
//                OnTrue = new BaseNodeSocket(this)
//                {
//                    Text = "On True",
//                    Direction = BaseNodeSocketDirection.Out,
//                    Flags = BaseNodeSocketFlags.Restrict,
//                    Color = EffectNode.TypeColor,
//                    Restriction = "__EFFECT__",
//                };
//                OnFalse = new BaseNodeSocket(this)
//                {
//                    Text = "On False",
//                    Direction = BaseNodeSocketDirection.Out,
//                    Flags = BaseNodeSocketFlags.Restrict,
//                    Color = EffectNode.TypeColor,
//                    Restriction = "__EFFECT__",
//                };
//                Caller = new BaseNodeSocket(this)
//                {
//                    Text = "Caller",
//                    Direction = BaseNodeSocketDirection.In,
//                    Flags = BaseNodeSocketFlags.Restrict | BaseNodeSocketFlags.MultiConnect,
//                    Color = TypeColor,
//                    Restriction = "__TRIGGER__",
//                };

//                List<BaseNodeSocket> sockets = new List<BaseNodeSocket>()
//                {
//                    Caller,
//                    Conditions,
//                    OnTrue,
//                    OnFalse,
//                };
//                Sockets = sockets;


//                BaseNodeControl ActiveControl = new BaseNodeControl(this);
//                ActiveControl.OnClicked += (sender, e) =>
//                {
//                    TriggerData.Active = !TriggerData.Active;
//                };
//                ActiveControl.OnDraw += (sender, e) =>
//                {
//                    bool border = (e.DrawFlags & DrawFlags.Border) > 0;
//                    bool fast = (e.DrawFlags & DrawFlags.Fast) > 0;

//                    e.Graphics.FillRectangle(ActiveControl.BackgroundBrush, ActiveControl.Bounds);

//                    Font font = new Font("Consolas", BaseNodeControl.Padding * 2, FontStyle.Regular);

//                    if (!fast)
//                    {
//                        e.Graphics.DrawString(
//                            "Active:",
//                            font,
//                            ActiveControl.TextBrush,
//                            ActiveControl.Location);

//                        e.Graphics.DrawString(
//                            TriggerData.Active.ToString(),
//                            font,
//                            ActiveControl.TextBrush,
//                            ActiveControl.Location.X + ActiveControl.Bounds.Width,
//                            ActiveControl.Location.Y,
//                            new StringFormat()
//                            {
//                                FormatFlags = StringFormatFlags.DirectionRightToLeft
//                            });
//                    }
//                    if (border)
//                    {
//                        e.Graphics.DrawRectangle(ActiveControl.ForegroundPen, Rectangle.Round(ActiveControl.Bounds));
//                    }
//                };

//                BaseNodeControl ConditionalControl = new BaseNodeControl(this);
//                ConditionalControl.OnClicked += (sender, e) =>
//                {
//                    TriggerData.ConditionalTrigger = !TriggerData.ConditionalTrigger;
//                };
//                ConditionalControl.OnDraw += (sender, e) =>
//                {
//                    bool border = (e.DrawFlags & DrawFlags.Border) > 0;
//                    bool fast = (e.DrawFlags & DrawFlags.Fast) > 0;

//                    e.Graphics.FillRectangle(ConditionalControl.BackgroundBrush, ConditionalControl.Bounds);

//                    Font font = new Font("Consolas", BaseNodeControl.Padding * 2, FontStyle.Regular);

//                    if (!fast)
//                    {
//                        e.Graphics.DrawString(
//                        "Conditional:",
//                        font,
//                        ConditionalControl.TextBrush,
//                        ConditionalControl.Location);

//                        e.Graphics.DrawString(
//                            TriggerData.ConditionalTrigger.ToString(),
//                            font,
//                            ConditionalControl.TextBrush,
//                            ConditionalControl.Location.X + ConditionalControl.Bounds.Width,
//                            ConditionalControl.Location.Y,
//                            new StringFormat()
//                            {
//                                FormatFlags = StringFormatFlags.DirectionRightToLeft
//                            });
//                    }
//                    if (border)
//                    {
//                        e.Graphics.DrawRectangle(ConditionalControl.ForegroundPen, Rectangle.Round(ConditionalControl.Bounds));
//                    }
//                };

//                List<BaseNodeControl> controls = new List<BaseNodeControl>()
//                {
//                    ActiveControl,
//                    ConditionalControl
//                };
//                Controls = controls;


//                NodeMoved += (sender, e) =>
//                {
//                    if (!EditorData.TriggerscriptData.Metadata.TriggerMetadata.ContainsKey(TriggerData.ID))
//                    {
//                        EditorData.TriggerscriptData.Metadata.TriggerMetadata.Add(TriggerData.ID, new TriggerscriptNodeMetadataXml());
//                    }

//                    EditorData.TriggerscriptData.Metadata.TriggerMetadata[TriggerData.ID].X = e.CurrentLocation.X;
//                    EditorData.TriggerscriptData.Metadata.TriggerMetadata[TriggerData.ID].Y = e.CurrentLocation.Y;
//                };
//            }


//            //sockets
//            public BaseNodeSocket OnTrue { get; private set; }
//            public BaseNodeSocket OnFalse { get; private set; }
//            public BaseNodeSocket Conditions { get; private set; }
//            public BaseNodeSocket Caller { get; private set; }


//            //public override void DrawBase(Graphics g, DrawFlags flags)
//            //{
//            //    //if (TriggerData.Active)
//            //    //{
//            //    //    HeaderColor = ActiveTypeColor;
//            //    //}
//            //    //else
//            //    //{
//            //    //    HeaderColor = InactiveTypeColor;
//            //    //}
//            //    base.DrawBase(g, flags);
//            //}
//            //public override void DrawText(Graphics g, DrawFlags flags)
//            //{
//            //    Subtext = "";
//            //    if (TriggerData.Active) Subtext += "Active ";
//            //    if (TriggerData.ConditionalTrigger) Subtext += "Conditional ";
//            //    Subtext += "Trigger";

//            //    Text = TriggerData.Name;

//            //    base.DrawText(g, flags);
//            //}
//        }
//        //private class VariableNode : BaseNode
//        //{
//        //    public static Color TypeColorRequired { get; private set; } = Color.FromArgb(55, 150, 55);
//        //    public static Color TypeColorOptional { get; private set; } = Color.FromArgb(115, 150, 65);

//        //    public ScriptData EditorData { get; private set; }
//        //    public ScriptXml.TriggerVarClass VarData { get; private set; }
//        //    public VariableNode(ScriptData data)
//        //    {
//        //        VarData = new ScriptXml.TriggerVarClass();
//        //        VarData.ID = data.NextVariableID++;

//        //        EditorData = data;

//        //        HeaderColor = TypeColorRequired;
//        //        Subtext = "UnknownType";


//        //        Set = new BaseNodeSocket(this)
//        //        {
//        //            Text = "Set",
//        //            Color = TypeColorRequired,
//        //            Restriction = "__?????set__",
//        //            Direction = BaseNodeSocketDirection.In,
//        //            Flags = BaseNodeSocketFlags.Restrict | BaseNodeSocketFlags.MultiConnect,
//        //        };
//        //        Use = new BaseNodeSocket(this)
//        //        {
//        //            Text = "Use",
//        //            Color = TypeColorRequired,
//        //            Restriction = "__?????use__",
//        //            Direction = BaseNodeSocketDirection.Out,
//        //            Flags = BaseNodeSocketFlags.Restrict | BaseNodeSocketFlags.MultiConnect,
//        //        };
//        //        List<BaseNodeSocket> sockets = new List<BaseNodeSocket>()
//        //        {
//        //            Set,
//        //            Use
//        //        };
//        //        Sockets = sockets;

//        //        BaseNodeControl ValueControl = new BaseNodeControl(this);
//        //        ValueControl.OnKeyboard += (sender, e) =>
//        //        {
//        //            string temp = VarData.Value;
//        //            if (e == '\b')
//        //            {
//        //                temp = temp.Substring(0, temp.Length - 1);
//        //            }
//        //            else
//        //            {
//        //                temp += e;
//        //            }

//        //            Match match = Regex.Match(temp, @"[[0-9a-zA-Z,;:_\.\-]*]*");
//        //            VarData.Value = match.Value;
//        //        };
//        //        ValueControl.OnDraw += (sender, e) =>
//        //        {
//        //            bool border = (e.DrawFlags & DrawFlags.Border) > 0;
//        //            bool fast = (e.DrawFlags & DrawFlags.Fast) > 0;

//        //            e.Graphics.FillRectangle(ValueControl.BackgroundBrush, ValueControl.Bounds);

//        //            Font font = new Font("Consolas", BaseNodeControl.Padding * 2, FontStyle.Regular);

//        //            if (!fast)
//        //            {
//        //                e.Graphics.DrawString(
//        //                    VarData.Value,
//        //                    font,
//        //                    ValueControl.TextBrush,
//        //                    ValueControl.Location,
//        //                    new StringFormat()
//        //                    {
//        //                    });
//        //            }
//        //            if (border)
//        //            {
//        //                e.Graphics.DrawRectangle(ValueControl.ForegroundPen, Rectangle.Round(ValueControl.Bounds));
//        //            }
//        //        };

//        //        List<BaseNodeControl> controls = new List<BaseNodeControl>()
//        //        {
//        //            ValueControl,
//        //        };
//        //        Controls = controls;


//        //        NodeMoved += (sender, e) =>
//        //        {
//        //            if (!EditorData.TriggerscriptData.Metadata.VariableMetadata.ContainsKey(VarData.ID))
//        //            {
//        //                EditorData.TriggerscriptData.Metadata.VariableMetadata.Add(VarData.ID, new TriggerscriptNodeMetadataXml());
//        //            }

//        //            EditorData.TriggerscriptData.Metadata.VariableMetadata[VarData.ID].X = e.CurrentLocation.X;
//        //            EditorData.TriggerscriptData.Metadata.VariableMetadata[VarData.ID].Y = e.CurrentLocation.Y;
//        //        };

//        //        Type = ScriptVarType.Invalid;
//        //    }

//        //    public ScriptVarType Type
//        //    {
//        //        get
//        //        {
//        //            return GetTypeFromString(VarData.Type);
//        //        }
//        //        set
//        //        {
//        //            if (value != Type)
//        //            {
//        //                ClearConnections();
//        //            }

//        //            VarData.Type = value.ToString();
//        //            Subtext = value + " Variable";

//        //            Set.Restriction = value.ToString();
//        //            Use.Restriction = value.ToString();
//        //        }
//        //    }

//        //    public BaseNodeSocket Set { get; private set; }
//        //    public BaseNodeSocket Use { get; private set; }
//        //    private BaseNodeControl ValueControl { get; set; }

//        //    public override void DrawText(Graphics g, DrawFlags flags)
//        //    {
//        //        base.DrawText(g, flags);

//        //        Text = VarData.Name;
//        //    }
//        //}


//        private abstract class LogicNode : BaseNode
//        {
//            public ScriptXml.TriggerClass.LogicClass LogicData { get; protected set; }
//            public BaseNodeSocket ParentTriggerSocket { get; private set; }
//            public ScriptData EditorData { get; private set; }

//            public class ParentTriggerChangedArgs
//            {
//                public BaseNodeSocket CurrentSocket { get; set; }
//                public BaseNodeSocket PreviousSocket { get; set; }
//            }
//            public event EventHandler<ParentTriggerChangedArgs> ParentTriggerChanged;

//            public LogicNode(ScriptData data)
//            {
//                EditorData = data;
//                VariableSockets = new Dictionary<int, BaseNodeSocket>();

//                Previous = new BaseNodeSocket(this)
//                {
//                    Text = "Caller",
//                    Direction = BaseNodeSocketDirection.In,
//                    Flags = BaseNodeSocketFlags.Restrict,
//                    Color = EffectNode.TypeColor,
//                    Restriction = "__?????prev__",
//                };
//                Previous.SocketDisconnected += (sender, e) =>
//                {
//                    ParentTriggerChangedArgs args = new ParentTriggerChangedArgs()
//                    {
//                        PreviousSocket = ParentTriggerSocket,
//                        CurrentSocket = null
//                    };
//                    BaseNodeSocket cur = Next;
//                    while (cur != null)
//                    {
//                        LogicNode node = (LogicNode)cur.Node;
//                        node.ParentTriggerSocket = null;
//                        node.ParentTriggerChanged?.Invoke(this, args);
//                        cur = node.Next.Connections.FirstOrDefault((BaseNodeSocket)null);
//                    }
//                };
//                Previous.SocketConnected += (sender, e) =>
//                {
//                    ParentTriggerChangedArgs args = new ParentTriggerChangedArgs()
//                    {
//                        PreviousSocket = ParentTriggerSocket
//                    };

//                    if (e.ToSocket.Node is TriggerNode)
//                    {
//                        ParentTriggerSocket = e.ToSocket;
//                    }
//                    if (e.ToSocket.Node is LogicNode)
//                    {
//                        ParentTriggerSocket = ((LogicNode)e.ToSocket.Node).ParentTriggerSocket;
//                    }

//                    args.CurrentSocket = ParentTriggerSocket;

//                    BaseNodeSocket cur = Next;
//                    while (cur != null)
//                    {
//                        LogicNode node = (LogicNode)cur.Node;
//                        node.ParentTriggerSocket = this.ParentTriggerSocket;
//                        node.ParentTriggerChanged?.Invoke(this, args);
//                        cur = node.Next.Connections.FirstOrDefault((BaseNodeSocket)null);
//                    }
//                };

//                Next = new BaseNodeSocket(this)
//                {
//                    Text = "Next",
//                    Direction = BaseNodeSocketDirection.Out,
//                    Flags = BaseNodeSocketFlags.Restrict,
//                    Color = EffectNode.TypeColor,
//                    Restriction = "__?????next__",
//                };
//            }

//            public BaseNodeSocket GetVariableSocket(int sigid)
//            {
//                if (VariableSockets.ContainsKey(sigid))
//                {
//                    return VariableSockets[sigid];
//                }
//                return null;
//            }
//            public bool SetFromDBID(int dbid, int version)
//            {
//                //ScriptLogicPrototype item = GetItemFromDBID(dbid, version);
//                //LogicData.DBID = dbid;
//                //LogicData.Version = version;
//                //LogicData.Inputs.Clear();
//                //LogicData.Outputs.Clear();
//                //if (item == null)
//                //{
//                //    Text = string.Format("INVALID [{0}] [v{1}]", dbid, version);
//                //    LogicData.Type = Text;
//                //    return false;
//                //}
//                //LogicData.Type = item.Name;

//                ////reset sockets
//                //VariableSockets.Clear();
//                //foreach (BaseNodeSocket socket in Sockets)
//                //{
//                //    if (socket != Previous && socket != Next)
//                //    {
//                //        socket.DisconnectAll();
//                //    }
//                //}
//                //List<BaseNodeSocket> sockets = new List<BaseNodeSocket>() { Previous, Next };

//                //foreach (var slot in item.Params)
//                //{
//                //    BaseNodeControl control = new BaseNodeControl(this);
//                    //control.cli

//                    //BaseNodeSocketDirection dir = slot.Output ? BaseNodeSocketDirection.Out : BaseNodeSocketDirection.In;
//                    //BaseNodeSocket socket = new BaseNodeSocket(this)
//                    //{
//                    //    Direction = dir,
//                    //    Flags = BaseNodeSocketFlags.Restrict,
//                    //    Text = slot.Name + "\n[" + slot.Type + "]",
//                    //    Restriction = slot.Type.ToString(),
//                    //    Color = slot.Optional ? VariableNode.TypeColorOptional : VariableNode.TypeColorRequired
//                    //};

//                    //if (slot.Type == ScriptVarType.Trigger)
//                    //{
//                    //    socket.Direction = BaseNodeSocketDirection.Out;
//                    //    socket.Text = "Trigger";
//                    //    socket.Restriction = "__TRIGGER__";
//                    //    socket.Color = TriggerNode.TypeColor;
//                    //}

//                    //int defaultValue = EditorData.GetNullVariable(slot.Type).ID;

//                    //socket.SocketConnected += (sender, e) =>
//                    //{
//                    //    if (e.ToSocket.Node is VariableNode)
//                    //    {
//                    //        LogicData.SetValueOfParam(slot.ID, ((VariableNode)e.ToSocket.Node).VarData.ID);
//                    //    }
//                    //    if (e.ToSocket.Node is TriggerNode)
//                    //    {
//                    //        LogicData.SetValueOfParam(slot.ID, ((TriggerNode)e.ToSocket.Node).TriggerVariableData.ID);
//                    //    }
//                    //};
//                    //socket.SocketDisconnected += (sender, e) =>
//                    //{
//                    //    LogicData.SetValueOfParam(slot.ID, defaultValue);
//                    //};
//                    //sockets.Add(socket);

//                    //if (!VariableSockets.ContainsKey(slot.ID))
//                    //{
//                    //    VariableSockets.Add(slot.ID, socket);
//                    //}

//                    //ScriptXml.TriggerClass.ParameterClass parameter = new ScriptXml.TriggerClass.ParameterClass()
//                    //{
//                    //    Name = slot.Name,
//                    //    Optional = slot.Optional,
//                    //    SigID = slot.ID,
//                    //    Value = defaultValue
//                    //};
//                    //if (slot.Output)
//                    //    LogicData.Outputs.Add(parameter);
//                    //else
//                    //    LogicData.Inputs.Add(parameter);
//                //}

//                //Sockets = sockets;
//                //Text = item.Name;

//                return true;
//            }
//            protected virtual ScriptLogicPrototype GetItemFromDBID(int dbid, int version)
//            {
//                return null;
//            }

//            protected Dictionary<int, BaseNodeSocket> VariableSockets { get; set; }
//            public BaseNodeSocket Previous { get; private set; }
//            public BaseNodeSocket Next { get; private set; }

//            public override void DrawBase(Graphics g, DrawFlags flags)
//            {
//                List<ScriptXml.TriggerClass.ParameterClass> fields = new List<ScriptXml.TriggerClass.ParameterClass>();
//                if (LogicData.Inputs != null) fields.AddRange(LogicData.Inputs);
//                if (LogicData.Outputs != null) fields.AddRange(LogicData.Outputs);
//                BodyHeight = fields.Count * 40 + 80;
//                for (int i = 0; i < fields.Count; i++)
//                {
//                    g.FillRectangle(new SolidBrush(Color.Black), 20, i * 40, Width - 40, 20);
//                }

//                base.DrawBase(g, flags);
//            }
//            public override void DrawText(Graphics g, DrawFlags flags)
//            {
//                List<ScriptXml.TriggerClass.ParameterClass> fields = new List<ScriptXml.TriggerClass.ParameterClass>();
//                if (LogicData.Inputs != null) fields.AddRange(LogicData.Inputs);
//                if (LogicData.Outputs != null) fields.AddRange(LogicData.Outputs);
//                for (int i = 0; i < fields.Count; i++)
//                {
//                    g.DrawString(
//                        EditorData.TriggerscriptData.TriggerVars.Where(e => e.ID == fields[i].Value).First().Name,
//                        new Font("Consolas", HeaderHeight / 3.5f, FontStyle.Regular),
//                        new SolidBrush(Color.White),
//                        20, i * 40);
//                }

//                if (ParentTriggerSocket == null)
//                    TextColor = Color.Red;
//                else
//                    TextColor = Color.White;

//                base.DrawText(g, flags);
//            }
//        }
//        private class EffectNode : LogicNode
//        {
//            public static Color TypeColor { get; private set; } = Color.FromArgb(130, 50, 135);

//            public ScriptXml.TriggerClass.EffectClass EffectData { get { return (ScriptXml.TriggerClass.EffectClass)LogicData; } }
//            public EffectNode(ScriptData data) : base(data)
//            {
//                LogicData = new ScriptXml.TriggerClass.EffectClass();
//                LogicData.ID = EditorData.NextLogicID++;

//                HeaderColor = TypeColor;
//                TextColor = Color.White;
//                Subtext = "Effect";

//                Next.Restriction = "__EFFECT__";
//                Next.Color = TypeColor;

//                Previous.Restriction = "__EFFECT__";
//                Previous.Color = TypeColor;

//                List<BaseNodeSocket> sockets = new List<BaseNodeSocket>()
//                {
//                    Previous,
//                    Next,
//                };
//                Sockets = sockets;

//                ParentTriggerChanged += (sender, e) =>
//                {
//                    if (e.PreviousSocket != null)
//                    {
//                        TriggerNode prev = ((TriggerNode)e.PreviousSocket.Node);
//                        if (e.PreviousSocket == prev.OnTrue)
//                        {
//                            prev.TriggerData.TriggerEffectsOnTrue.Remove(EffectData);
//                        }
//                        if (e.PreviousSocket == prev.OnFalse)
//                        {
//                            prev.TriggerData.TriggerEffectsOnFalse.Remove(EffectData);
//                        }
//                    }

//                    if (e.CurrentSocket != null)
//                    {
//                        TriggerNode cur = ((TriggerNode)e.CurrentSocket.Node);
//                        if (e.CurrentSocket == cur.OnTrue)
//                        {
//                            cur.TriggerData.TriggerEffectsOnTrue.Add(EffectData);
//                        }
//                        if (e.CurrentSocket == cur.OnFalse)
//                        {
//                            cur.TriggerData.TriggerEffectsOnFalse.Add(EffectData);
//                        }
//                    }
//                };
//                NodeMoved += (sender, e) =>
//                {
//                    if (!EditorData.TriggerscriptData.Metadata.EffectMetadata.ContainsKey(EffectData.ID))
//                    {
//                        EditorData.TriggerscriptData.Metadata.EffectMetadata.Add(EffectData.ID, new TriggerscriptNodeMetadataXml());
//                    }

//                    EditorData.TriggerscriptData.Metadata.EffectMetadata[EffectData.ID].X = e.CurrentLocation.X;
//                    EditorData.TriggerscriptData.Metadata.EffectMetadata[EffectData.ID].Y = e.CurrentLocation.Y;
//                };
//            }

//            protected override ScriptLogicPrototype GetItemFromDBID(int dbid, int version)
//            {
//                if (!EffectItems.ContainsKey(dbid)) return null;
//                if (EffectItems[dbid].ContainsKey(version))
//                {
//                    ScriptLogicPrototype item = EffectItems[dbid][version];
//                    return item;
//                }
//                else
//                {
//                    if (EffectItems[dbid].ContainsKey(-1))
//                    {
//                        return EffectItems[dbid][-1];
//                    }
//                }
//                return null;
//            }
//        }
//        private class ConditionNode : LogicNode
//        {
//            public static Color TypeColor { get; private set; } = Color.FromArgb(155, 65, 65);

//            public ScriptXml.TriggerClass.ConditionClass ConditionData { get { return (ScriptXml.TriggerClass.ConditionClass)LogicData; } }
//            public ConditionNode(ScriptData data) : base(data)
//            {
//                LogicData = new ScriptXml.TriggerClass.ConditionClass();
//                LogicData.ID = EditorData.NextLogicID++;

//                HeaderColor = TypeColor;
//                Subtext = "Condition";

//                Next.Restriction = "__CONDITION__";
//                Next.Color = TypeColor;
//                Previous.Restriction = "__CONDITION__";
//                Previous.Color = TypeColor;

//                List<BaseNodeSocket> sockets = new List<BaseNodeSocket>() { Previous, Next };
//                Sockets = sockets;

//                VariableSockets = new Dictionary<int, BaseNodeSocket>();


//                ParentTriggerChanged += (sender, e) =>
//                {
//                    if (e.PreviousSocket != null)
//                    {
//                        TriggerNode prev = ((TriggerNode)e.PreviousSocket.Node);
//                        if (e.PreviousSocket == prev.Conditions)
//                        {
//                            prev.TriggerData.Conditions.Remove(ConditionData);
//                        }
//                    }

//                    if (e.CurrentSocket != null)
//                    {
//                        TriggerNode cur = ((TriggerNode)e.CurrentSocket.Node);
//                        if (e.CurrentSocket == cur.Conditions)
//                        {
//                            cur.TriggerData.Conditions.Add(ConditionData);
//                        }
//                    }
//                };
//                NodeMoved += (sender, e) =>
//                {
//                    if (!EditorData.TriggerscriptData.Metadata.ConditionMetadata.ContainsKey(ConditionData.ID))
//                    {
//                        EditorData.TriggerscriptData.Metadata.ConditionMetadata.Add(ConditionData.ID, new TriggerscriptNodeMetadataXml());
//                    }

//                    EditorData.TriggerscriptData.Metadata.ConditionMetadata[ConditionData.ID].X = e.CurrentLocation.X;
//                    EditorData.TriggerscriptData.Metadata.ConditionMetadata[ConditionData.ID].Y = e.CurrentLocation.Y;
//                };
//            }

//            protected override ScriptLogicPrototype GetItemFromDBID(int dbid, int version)
//            {
//                if (!ConditionItems.ContainsKey(dbid)) return null;
//                if (ConditionItems[dbid].ContainsKey(version))
//                {
//                    ScriptLogicPrototype item = ConditionItems[dbid][version];
//                    return item;
//                }
//                else
//                {
//                    if (ConditionItems[dbid].ContainsKey(-1))
//                    {
//                        return ConditionItems[dbid][-1];
//                    }
//                }
//                return null;
//            }
//        }


//        public ScriptXml TriggerscriptData { get; private set; }
//        public ScriptData(FoundryInstance i) : base(i)
//        {
//            TriggerscriptData = new ScriptXml();

//            NodeAdded += (sender, e) =>
//            {
//                if (e is TriggerNode)
//                {
//                    TriggerscriptData.Triggers.Add((e as TriggerNode).TriggerData);
//                    TriggerscriptData.TriggerVars.Add((e as TriggerNode).TriggerVariableData);
//                }
//                //if (e is VariableNode)
//                //{
//                //    TriggerscriptData.TriggerVars.Add((e as VariableNode).VarData);
//                //}
//            };
//            NodeRemoved += (sender, e) =>
//            {
//                if (e is TriggerNode)
//                {
//                    TriggerscriptData.Triggers.Remove((e as TriggerNode).TriggerData);
//                    TriggerscriptData.TriggerVars.Remove((e as TriggerNode).TriggerVariableData);
//                }
//                //if (e is VariableNode)
//                //{
//                //    TriggerscriptData.TriggerVars.Remove((e as VariableNode).VarData);
//                //}
//            };

//            //OnDataSave += (sender, e) =>
//            //{
//            //    YAXSerializer<TriggerscriptXml> ser = new YAXSerializer<TriggerscriptXml>(new SerializerOptions()
//            //    {
//            //        SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects,
//            //        ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
//            //    });
//            //    ser.SerializeToFile(TriggerscriptData, FileName);
//            //};
//            //OnDataLoad += (sender, e) =>
//            //{
//            //    YAXSerializer<TriggerscriptXml> ser = new YAXSerializer<TriggerscriptXml>(new SerializerOptions()
//            //    {
//            //        SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects,
//            //        ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
//            //    });
//            //    TriggerscriptXml tsdata = ser.DeserializeFromFile(FileName);

//            //    foreach (var node in CreateFromData(tsdata))
//            //    {
//            //        AddNode(node);
//            //    }
//            //};
//        }
//        public ScriptData(FoundryInstance i, ScriptXml xml) : this(i)
//        {
//            foreach (var node in CreateFromData(xml))
//            {
//                AddNode(node);
//            }
//        }

//        //TODO: Clean this shit up!!! This should all be static. Nodes should not need references to their owner.
//        #region Importing
//        private List<BaseNode> CreateFromData(ScriptXml tsdata)
//        {
//            List<BaseNode> Nodes = new List<BaseNode>();
//            Dictionary<int, TriggerNode> triggerNodes = new Dictionary<int, TriggerNode>();
//            Dictionary<int, ScriptXml.TriggerClass> triggerDatas = new Dictionary<int, ScriptXml.TriggerClass>();
//            //Dictionary<int, VariableNode> varNodes = new Dictionary<int, VariableNode>();
//            Dictionary<int, EffectNode> effectNodes = new Dictionary<int, EffectNode>();
//            Dictionary<int, ConditionNode> conditionNodes = new Dictionary<int, ConditionNode>();

//            //spawn var nodes.
//            //foreach (ScriptXml.TriggerVarClass var in tsdata.TriggerVars)
//            //{
//            //    if (var.IsNull)
//            //    {
//            //        continue;
//            //    }

//            //    PointF location = new PointF(0, 0);

//            //    VariableNode node = new VariableNode(this)
//            //    {
//            //        Location = location,
//            //        Type = GetTypeFromString(var.Type),
//            //    };
//            //    node.VarData.Value = var.Value;
//            //    node.VarData.Name = var.Name;

//            //    if (node.Type != ScriptVarType.Trigger) Nodes.Add(node); //dont actually add the trigger node type.

//            //    if (!varNodes.ContainsKey(var.ID))
//            //    {
//            //        varNodes.Add(var.ID, node);
//            //    }
//            //}

//            //spawn trigger nodes first.
//            foreach (ScriptXml.TriggerClass trigger in tsdata.Triggers)
//            {

//                PointF triggerLocation = new PointF(
//                        trigger.X * 100,
//                        trigger.Y * 100
//                        );

//                TriggerNode triggerNode = new TriggerNode(this)
//                {
//                    Location = triggerLocation,
//                };
//                triggerNode.TriggerData.Name = trigger.Name;
//                triggerNode.TriggerData.Active = trigger.Active;
//                triggerNode.TriggerData.ConditionalTrigger = trigger.ConditionalTrigger;
//                triggerNode.TriggerData.EvalLimit = trigger.EvalLimit;
//                triggerNode.TriggerData.EvaluateFrequency = trigger.EvaluateFrequency;

//                Nodes.Add(triggerNode);
//                triggerNodes.Add(trigger.ID, triggerNode);
//                triggerDatas.Add(trigger.ID, trigger);
//            }

//            //then spawn all trigger logic. this comes second so that we can connect TriggerActivate/TriggerDeactivate to the correct node.
//            foreach (int id in triggerNodes.Keys)
//            {
//                TriggerNode triggerNode = triggerNodes[id];
//                ScriptXml.TriggerClass trigger = triggerDatas[id];

//                //effects
//                BaseNodeSocket callerSocket;

//                callerSocket = triggerNode.OnTrue;
//                foreach (var effect in trigger.TriggerEffectsOnTrue)
//                {
//                    EffectNode effectNode = ImportEffect(effect, callerSocket);
//                    callerSocket = effectNode.Next;

//                    Nodes.Add(effectNode);
//                    if (!effectNodes.ContainsKey(effect.ID))
//                    {
//                        effectNodes.Add(effect.ID, effectNode);
//                    }

//                    //ConnectVars(effectNode, effect, varNodes, triggerNodes);
//                }

//                callerSocket = triggerNode.OnFalse;
//                foreach (var effect in trigger.TriggerEffectsOnFalse)
//                {
//                    EffectNode effectNode = ImportEffect(effect, callerSocket);
//                    callerSocket = effectNode.Next;

//                    Nodes.Add(effectNode);
//                    if (!effectNodes.ContainsKey(effect.ID))
//                    {
//                        effectNodes.Add(effect.ID, effectNode);
//                    }

//                    //ConnectVars(effectNode, effect, varNodes, triggerNodes);
//                }


//                //conditions
//                List<ScriptXml.TriggerClass.ConditionClass> conditions;
//                if
//                    (trigger.ConditionsAnd != null) conditions = trigger.ConditionsAnd;
//                else if
//                    (trigger.ConditionsOr != null) conditions = trigger.ConditionsOr;
//                else
//                    conditions = new List<ScriptXml.TriggerClass.ConditionClass>();

//                callerSocket = triggerNode.Conditions;
//                foreach (var condition in conditions)
//                {
//                    ConditionNode conditionNode = ImportCondition(condition, callerSocket);
//                    callerSocket = conditionNode.Next;

//                    Nodes.Add(conditionNode);
//                    if (!conditionNodes.ContainsKey(condition.ID))
//                    {
//                        conditionNodes.Add(condition.ID, conditionNode);
//                    }

//                    //ConnectVars(conditionNode, condition, varNodes, triggerNodes);
//                }

//                SizeF lastSize;
//                ApplyLayout(
//                    triggerNode,
//                    //varNodes,
//                    tsdata.Metadata,
//                    out lastSize);
//            }


//            //apply metadata
//            if (tsdata.Metadata != null)
//            {
//                foreach (var metadata in tsdata.Metadata.TriggerMetadata)
//                {
//                    if (triggerNodes.ContainsKey(metadata.Key))
//                    {
//                        triggerNodes[metadata.Key].Location = new PointF(
//                            metadata.Value.X,
//                            metadata.Value.Y
//                            );
//                    }
//                }
//                foreach (var metadata in tsdata.Metadata.VariableMetadata)
//                {
//                    //if (varNodes.ContainsKey(metadata.Key))
//                    //{
//                    //    varNodes[metadata.Key].Location = new PointF(
//                    //        metadata.Value.X,
//                    //        metadata.Value.Y
//                    //        );
//                    //}
//                }
//                foreach (var metadata in tsdata.Metadata.EffectMetadata)
//                {
//                    if (effectNodes.ContainsKey(metadata.Key))
//                    {
//                        effectNodes[metadata.Key].Location = new PointF(
//                            metadata.Value.X,
//                            metadata.Value.Y
//                            );
//                    }
//                }
//                foreach (var metadata in tsdata.Metadata.ConditionMetadata)
//                {
//                    if (conditionNodes.ContainsKey(metadata.Key))
//                    {
//                        conditionNodes[metadata.Key].Location = new PointF(
//                            metadata.Value.X,
//                            metadata.Value.Y
//                            );
//                    }
//                }
//            }
//            return Nodes;
//        }

//        private EffectNode ImportEffect(ScriptXml.TriggerClass.EffectClass effect, BaseNodeSocket caller)
//        {
//            PointF effectLocation = new PointF(0, 0);

//            EffectNode effectNode = new EffectNode(this)
//            {
//                Location = effectLocation,
//            };
//            effectNode.SetFromDBID(effect.DBID, effect.Version);

//            effectNode.Previous.Connect(caller);

//            return effectNode;
//        }
//        private ConditionNode ImportCondition(ScriptXml.TriggerClass.ConditionClass condition, BaseNodeSocket caller)
//        {
//            PointF conditionLocation = new PointF(0, 0);

//            ConditionNode conditionNode = new ConditionNode(this)
//            {
//                Location = conditionLocation,
//            };
//            conditionNode.SetFromDBID(condition.DBID, condition.Version);

//            conditionNode.Previous.Connect(caller);

//            return conditionNode;
//        }
//        //private void ConnectVars(LogicNode node, ScriptXml.TriggerClass.LogicClass logic, Dictionary<int, VariableNode> varNodes, Dictionary<int, TriggerNode> triggerNodes)
//        //{
//        //    float varTracker = node.Location.Y + node.SocketBodyHeight + 350;
//        //    List<ScriptXml.TriggerClass.ParameterClass> parameters = new List<ScriptXml.TriggerClass.ParameterClass>();
//        //    if (logic.Inputs != null) parameters.AddRange(logic.Inputs);
//        //    if (logic.Outputs != null) parameters.AddRange(logic.Outputs);

//        //    //connect and position variables
//        //    foreach (var param in parameters)
//        //    {
//        //        int value = logic.GetValueOfParam(param.SigID);
//        //        if (varNodes.ContainsKey(value))
//        //        {
//        //            var socket = node.GetVariableSocket(param.SigID);
//        //            if (socket != null)
//        //            {
//        //                if (varNodes[value].Type == ScriptVarType.Trigger)
//        //                {
//        //                    BaseNodeSocket triggerSocket = triggerNodes[int.Parse(varNodes[value].VarData.Value)].Caller;
//        //                    socket.Connect(triggerSocket);
//        //                }
//        //                else
//        //                {
//        //                    BaseNodeSocket varSocket = socket.Direction == BaseNodeSocketDirection.In ? varNodes[value].Use : varNodes[value].Set;
//        //                    socket.Connect(varSocket);
//        //                }
//        //            }
//        //        }
//        //    }
//        //}

//        private static float Spacing = 250;
//        private void ApplyLayout(
//            TriggerNode node,
//            //Dictionary<int, VariableNode> varNodes,
//            TriggerscriptMetadatasXml metadata,
//            out SizeF size
//            )
//        {
//            //file who owns each variable node
//            //Dictionary<VariableNode, BaseNodeSocket> varOwners = new Dictionary<VariableNode, BaseNodeSocket>();
//            //foreach (VariableNode varNode in varNodes.Values)
//            //{
//            //    if (!varOwners.ContainsKey(varNode))
//            //        varOwners.Add(varNode, null);

//            //    foreach (BaseNodeSocket s in varNode.Use.Connections)
//            //    {
//            //        varOwners[varNode] = s;
//            //    }
//            //    foreach (BaseNodeSocket s in varNode.Set.Connections)
//            //    {
//            //        varOwners[varNode] = s;
//            //    }
//            //}


//            SizeF currentSize;
//            SizeF totalSize = new SizeF(0, 0);


//            currentSize = new SizeF(0, 0);
//            BaseNodeSocket condition = node.Conditions.Connections.FirstOrDefault((BaseNodeSocket)null);
//            if (condition != null)
//                ApplyLogicNodeLayout(
//                    node,
//                    (ConditionNode)condition.Node,
//                    //varOwners,
//                    new PointF(node.Width + Spacing * 3, totalSize.Height),
//                    metadata,
//                    ref currentSize
//                    );
//            totalSize += currentSize;


//            currentSize = new SizeF(0, 0);
//            BaseNodeSocket trueEff = node.OnTrue.Connections.FirstOrDefault((BaseNodeSocket)null);
//            if (trueEff != null)
//                ApplyLogicNodeLayout(
//                    node,
//                    (EffectNode)trueEff.Node,
//                    //varOwners,
//                    new PointF(node.Width + Spacing * 3, totalSize.Height),
//                    metadata,
//                    ref currentSize
//                    );
//            totalSize += currentSize;


//            currentSize = new SizeF(0, 0);
//            BaseNodeSocket falseEff = node.OnFalse.Connections.FirstOrDefault((BaseNodeSocket)null);
//            if (falseEff != null)
//                ApplyLogicNodeLayout(
//                    node,
//                    (EffectNode)falseEff.Node,
//                    //varOwners,
//                    new PointF(node.Width + Spacing * 3, totalSize.Height),
//                    metadata,
//                    ref currentSize
//                    );
//            totalSize += currentSize;


//            size = totalSize;
//        }

//        private void ApplyLogicNodeLayout(
//            BaseNode previous,
//            LogicNode node,
//            //Dictionary<VariableNode, BaseNodeSocket> varOwners,
//            PointF offset,
//            TriggerscriptMetadatasXml metadata,
//            ref SizeF size
//            )
//        {
//            if (node == null)
//            {
//                return;
//            }

//            //apply location
//            node.Location = new PointF(
//                previous.Bounds.X + offset.X,
//                previous.Bounds.Y + offset.Y
//                );

//            float thisHeight = node.Bounds.Height;
//            float thisWidth = node.Bounds.Width;

//            //position relevant variables
//            //List<VariableNode> ins = new List<VariableNode>();
//            //List<VariableNode> outs = new List<VariableNode>();
//            //foreach (var pair in varOwners)
//            //{
//            //    if (pair.Value != null && pair.Value.Node == node)
//            //    {
//            //        if (pair.Value.Direction == BaseNodeSocketDirection.In)
//            //            ins.Add(pair.Key);
//            //        else
//            //            outs.Add(pair.Key);
//            //    }
//            //}

//            ////if there are ins, shift the effect over to fit them, and account for their width.
//            //if (ins.Count > 0)
//            //{
//            //    node.Location = new PointF(
//            //        node.Location.X + ins[0].Bounds.Width + 100,
//            //        node.Location.Y
//            //        );

//            //    thisWidth += ins[0].Bounds.Width + 100;
//            //}
//            ////if there are outs, account for their width.
//            //if (outs.Count > 0)
//            //{
//            //    thisWidth += outs[0].Bounds.Width + 100;
//            //}

//            //float varHeightIn = 100;
//            //foreach (var inn in ins)
//            //{
//            //    inn.Location = new PointF(
//            //        node.Location.X - inn.Bounds.Width - 100,
//            //        node.Location.Y + node.Bounds.Height + varHeightIn
//            //        );

//            //    varHeightIn += inn.Bounds.Height + 100;
//            //}

//            //float varHeightOut = 100;
//            //foreach (var outn in outs)
//            //{
//            //    outn.Location = new PointF(
//            //    node.Location.X + node.Bounds.Width + 100,
//            //    node.Location.Y + node.Bounds.Height + varHeightOut
//            //    );

//            //    varHeightOut += outn.Bounds.Height + 100;
//            //}

//            //thisHeight += new float[] { varHeightIn, varHeightOut }.Max();


//            //set total height
//            thisHeight += +Spacing;
//            if (thisHeight > size.Height)
//                size.Height = thisHeight;
//            size.Width += thisWidth + Spacing;


//            BaseNodeSocket next = node.Next.Connections.FirstOrDefault((BaseNodeSocket)null);
//            if (next != null)
//            {
//                //do next
//                ApplyLogicNodeLayout(
//                    node,
//                    (LogicNode)next.Node,
//                    //varOwners,
//                    new PointF(thisWidth + Spacing, 0),
//                    metadata,
//                    ref size);
//            }
//        }

//        #endregion


//        private int NextTriggerID;
//        public void AddTrigger(string name, PointF location)
//        {
//            TriggerNode node = new TriggerNode(this)
//            {
//                Location = location,
//                Text = name,
//            };

//            AddNode(node);
//        }

//        private int NextVariableID;
//        public void AddVariable(ScriptVarType type, PointF location)
//        {
//            //VariableNode node = new VariableNode(this)
//            //{
//            //    Location = location,
//            //};
//            //node.Type = type;
//            //AddNode(node);
//        }

//        private int NextLogicID;
//        public void AddEffect(int dbid, int version, PointF location)
//        {
//            EffectNode node = new EffectNode(this)
//            {
//                Location = location
//            };
//            node.SetFromDBID(dbid, version);
//            AddNode(node);
//        }
//        public void AddCondition(int dbid, int version, PointF location)
//        {
//            ConditionNode node = new ConditionNode(this)
//            {
//                Location = location
//            };
//            node.SetFromDBID(dbid, version);
//            AddNode(node);
//        }

//        public static Dictionary<ScriptVarType, List<object>> VarValues = new Dictionary<ScriptVarType, List<object>>()
//        {
//            {ScriptVarType.TechStatus, new List<object>() { "Unobtainable", "Obtainable", "Available", "Researching", "Active", "Disabled", "CoopResearching" } },
//            {ScriptVarType.Operator, new List<object>() { "NotEqualTo", "LessThan", "LessThanOrEqualTo", "EqualTo", "GreaterThanOrEqualTo", "GreaterThan" } },
//            {ScriptVarType.MathOperator, new List<object>() {"Add","Subtract","Multiply","Divide","Modulus"} },
//            {ScriptVarType.Bool, new List<object>() {"True", "False"} },
//            {ScriptVarType.LOSType, new List<object>() {"LOSNormal", "LOSFullVisible", "LOSDontCare"} },
//            {ScriptVarType.ListPosition, new List<object>() {"First", "Last", "Random"} },
//            {ScriptVarType.DataScalar, new List<object>() { "Accuracy", "WorkRate", "Damage", "DamageTaken", "LOS", "Velocity", "WeaponRange" } },
//            {ScriptVarType.UIButton, new List<object>() { "ResultSelected", "ResultWaiting" } },
//            {ScriptVarType.BuildingCommandState, new List<object>() {"ResultWaiting", "ResultDone"} },
//            //{ScriptVarType. }
//        };
//        public ScriptXml.TriggerVarClass GetNullVariable(ScriptVarType type)
//        {
//            if (!NullVarDatas.ContainsKey(type))
//            {
//                ScriptXml.TriggerVarClass data = new ScriptXml.TriggerVarClass()
//                {
//                    ID = NextVariableID++,
//                    Name = "Null" + type.ToString() + "Var",
//                    IsNull = true,
//                    Type = type.ToString(),
//                };
//                TriggerscriptData.TriggerVars.Add(data);
//                NullVarDatas.Add(type, data);
//            }

//            return NullVarDatas[type];
//        }
//        private Dictionary<ScriptVarType, ScriptXml.TriggerVarClass> NullVarDatas = new Dictionary<ScriptVarType, ScriptXml.TriggerVarClass>();
//    }
//}