using KSoft.Phoenix.Phx;
using System;
using System.Collections.Generic;
using System.Drawing; //cross platform System.Drawing.Primitives is used.
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Chef.HW1.Script.TriggerscriptParams;

namespace Chef.HW1.Script
{
    public struct Selection
    {
        public Selection()
        {
            TriggerId = -1;
            LogicType = TriggerLogicSlot.Condition;
            LogicIndex = -1;
            InsertIndex = -1;
            VarSigId = -1;
            UnitId = -1;
        }

        public int TriggerId { get; set; }
        public TriggerLogicSlot LogicType { get; set; }
        public int LogicIndex { get; set; }
        public int InsertIndex { get; set; }
        public int VarSigId { get; set; }
        public int UnitId { get; set; }


        public static bool operator ==(Selection lhs, Selection rhs)
        {
            return lhs.TriggerId == rhs.TriggerId
                && lhs.LogicType == rhs.LogicType
                && lhs.LogicIndex == rhs.LogicIndex
                && lhs.VarSigId == rhs.VarSigId
                && lhs.UnitId == rhs.UnitId;
        }
        public static bool operator !=(Selection lhs, Selection rhs)
        {
            return !(lhs == rhs);
        }
    }

    public static class TriggerscriptHelpers
    {
        //Bounds
        public static Rectangle ScriptBounds(Triggerscript script)
        {
            if (script.Triggers.Count == 0) return Rectangle.Empty;
            Trigger minX = script.Triggers.Values.First();
            Trigger minY = script.Triggers.Values.First();
            Trigger maxX = script.Triggers.Values.First();
            Trigger maxY = script.Triggers.Values.First();
            foreach (Trigger t in script.Triggers.Values)
            {
                if (t.X < minX.X) minX = t;
                if (t.Y < minY.Y) minY = t;
                if (t.X > maxX.X) maxX = t;
                if (t.Y > maxY.Y) maxY = t;
            }

            Rectangle ret = new Rectangle(
                (int)minX.X,
                (int)minY.Y,
                (int)(maxX.X - minX.X + BoundsTriggerUnit(maxX).Width),
                (int)(maxY.Y - minY.Y + BoundsTriggerUnit(maxY).Height)
                );
            ret.Inflate(100, 100);
            return ret;
        }
        
        public static Rectangle BoundsTriggerUnit(Trigger trigger)
        {
            Rectangle triggerBounds = BoundsTriggerNode(trigger);
            Rectangle ret = triggerBounds;

            foreach (var type in Enum.GetValues<TriggerLogicSlot>())
            {
                for (int i = 0; i < Logics(trigger, type).Count(); i++)
                {
                    Rectangle logicBounds = BoundsLogicNode(trigger, type, i);
                    ret.Width = logicBounds.X - triggerBounds.X + logicBounds.Width;
                    ret.Height = Math.Max(ret.Height, logicBounds.Height);
                }
            }
            ret.Width += 50;

            return ret;
        }
        public static Rectangle BoundsTriggerNode(Trigger trigger)
        {
            return new Rectangle(
                (int)trigger.X,
                (int)trigger.Y,
                DefaultWidth,
                HeaderHeight * 3);
        }
        public static Rectangle BoundsLogicUnit(Trigger trigger, TriggerLogicSlot type)
        {
            IEnumerable<Logic> logics;
            Point loc;
            if (type == TriggerLogicSlot.Condition)
            {
                logics = trigger.Conditions;
                var bounds = BoundsTriggerNode(trigger);
                loc = bounds.Location;
                loc.X += bounds.Width;
                loc.X += LogicSectionSpacing;
                //loc.Y -= HeaderHeight;
            }
            else if (type == TriggerLogicSlot.EffectTrue)
            {
                logics = trigger.TriggerEffectsOnTrue;
                Rectangle blu = BoundsLogicUnit(trigger, TriggerLogicSlot.Condition);
                loc = new Point(blu.X + blu.Width + LogicSectionSpacing, blu.Y);
            }
            else
            {
                logics = trigger.TriggerEffectsOnFalse;
                Rectangle blu = BoundsLogicUnit(trigger, TriggerLogicSlot.EffectTrue);
                loc = new Point(blu.X + blu.Width + LogicSectionSpacing, blu.Y);
            }

            int logicsCount = logics.Count();
            Size size = new Size(0, 25);
            if (logicsCount == 0)
            {
                size.Width = LogicSectionSpacing * 4;
            }
            else
            {
                size.Width = DefaultWidth * logicsCount;
                size.Width += LogicSpacing * (logicsCount - 1);
            }
            foreach (var l in logics)
            {
                size.Height = Math.Max(size.Height, BodySize(l).Height);
            }

            //size.Height += HeaderHeight;
            return new Rectangle(loc, size);
        }
        public static Rectangle BoundsLogicNode(Trigger trigger, TriggerLogicSlot type, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);

            Point loc = BoundsLogicUnit(trigger, type).Location;
            loc.Y += HeaderHeight;
            for (int i = 0; i < index; i++)
            {
                loc.X += BodySize(logics.ElementAt(i)).Width;
                loc.X += LogicSpacing;
            }

            return new Rectangle(loc, BodySize(logics.ElementAt(index)));
        }
        public static Rectangle BoundsLogicDrop(Trigger trigger, TriggerLogicSlot type, int index)
        {
            var logics = Logics(trigger, type);
            Rectangle b;

            if (logics.Count() == 0)
            {
                b = BoundsLogicUnit(trigger, type);
                b.Inflate(LogicSectionSpacing / 2, 0);
                return b; //early out here because were just using the empty logic section bounds wholesale.
            }
            //if index == last+1, make a bounds as if last+1 existed.
            else if (index == logics.Count())
            {
                b = BoundsLogicNode(trigger, type, logics.Count() - 1);
                b.X += b.Width;
                b.X += LogicSpacing;
            }
            //for everything else just use the default bounds.
            else
            {
                b = BoundsLogicNode(trigger, type, index);
            }

            Rectangle slotClamp = BoundsLogicUnit(trigger, type);

            b.X -= LogicSpacing;
            b.X -= b.Width / 2;
            b.Width += LogicSpacing;
            b.Height = slotClamp.Height;

            slotClamp.Inflate(LogicSectionSpacing / 2, 0);
            b.Intersect(slotClamp);

            return b;
        }

        public static Rectangle ParamNameBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicNode(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2,
                logicBounds.Width - Margin * 2,
                VarNameHeight);
            return ret;
        }
        public static Rectangle ParamValBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicNode(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2 + VarNameHeight,
                logicBounds.Width - Margin * 2,
                VarValHeight);
            return ret;
        }
       
        public static Size BodySize(Logic logic)
        {
            int varCount = logic.StaticParamInfo.Count;

            int width = DefaultWidth;

            return new Size(
                width,
                HeaderHeight + varCount * (VarHeight + VarSpacing) + VarHeight
                );
        }

        public static void BodyBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic)
        {
            trigger = -1;
            slot = TriggerLogicSlot.Condition;
            logic = -1;

            foreach (var t in script.Triggers.Values)
            {
                if (BoundsTriggerNode(t).Contains(point))
                {
                    trigger = t.ID;
                    return;
                }

                foreach (var s in Enum.GetValues<TriggerLogicSlot>())
                {
                    var l = Logics(t, s);
                    for (int i = 0; i < l.Count(); i++)
                    {
                        if (BoundsLogicNode(t, s, i).Contains(point))
                        {
                            trigger = t.ID;
                            slot = s;
                            logic = i;
                            return;
                        }
                    }
                }
            }
            return;
        }
        public static void DropBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic)
        {
            trigger = -1;
            slot = TriggerLogicSlot.Condition;
            logic = -1;

            foreach (var t in script.Triggers.Values)
            {
                foreach (var s in Enum.GetValues<TriggerLogicSlot>())
                {
                    var l = Logics(t, s);
                    for (int i = 0; i < l.Count() + 1; i++) //count + 1 because we also want the trailing drop bounds.
                    {
                        if (BoundsLogicDrop(t, s, i).Contains(point))
                        {
                            trigger = t.ID;
                            slot = s;
                            logic = i;
                            return;
                        }
                    }
                }
            }
            return;
        }
        public static void VarBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic, out int param)
        {
            param = -1;
            BodyBoundsAtPoint(script, point, out trigger, out slot, out logic);
            if (trigger == -1 || logic == -1) return;

            Trigger t = script.Triggers[trigger];
            Logic l = Logics(t, slot).ElementAt(logic);
            for (int i = 0; i < l.StaticParamInfo.Count; i++)
            {
                if (//ParamNameBounds(t, slot, logic, i).Contains(point)
                    //||
                    ParamValBounds(t, slot, logic, i).Contains(point))
                {
                    param = l.StaticParamInfo.ElementAt(i).Key;
                    return;
                }
            }
        }


        //Queries
        //public static Selection SelectAt(Triggerscript script, Point point)
        //{
        //    Selection ret = new Selection();

        //    ret.TriggerId = -1;
        //    ret.LogicIndex = -1;
        //    foreach (var trigger in script.Triggers.Values)
        //    {
        //        Rectangle unitBounds = UnitBounds(trigger);
        //        if (unitBounds.Contains(point))
        //        {
        //            ret.UnitId = trigger.ID;
        //            ret.TriggerId = trigger.ID;

        //            foreach (var type in Enum.GetValues<TriggerLogicSlot>())
        //            {
        //                if (SlotBounds(trigger, type).Contains(point))
        //                {
        //                    ret.LogicType = type;

        //                    var logics = Logics(trigger, type);
        //                    for (int i = 0; i < logics.Count(); i++)
        //                    {
        //                        if (LogicDropBounds(trigger, type, i).Contains(point))
        //                        {
        //                            ret.InsertIndex = i;
        //                        }
        //                        if (LogicBodyBounds(trigger, type, i).Contains(point))
        //                        {
        //                            ret.LogicIndex = i;

        //                            for (int v = 0; v < Logics(trigger, type).ElementAt(i).StaticParamInfo.Count(); v++)
        //                            {
        //                                if (ParamValBounds(trigger, type, i, v).Contains(point))
        //                                {
        //                                    ret.VarSigId = Logics(trigger, type).ElementAt(i).StaticParamInfo.ElementAt(v).Key;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (LogicDropBounds(trigger, type, logics.Count()).Contains(point))
        //                    {
        //                        ret.InsertIndex = logics.Count();
        //                    }

        //                    if (Logics(trigger, type).Count() == 0)
        //                    {
        //                        ret.LogicIndex = 0;
        //                        ret.InsertIndex = 0;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return ret;
        //}
        public static int NextVarId(Triggerscript script)
        {
            List<int> ids = script.TriggerVars.Keys.ToList();
            ids.Sort();
            for (int i = 0; i < ids.Count; i++)
            {
                //this is the last one.
                if (i == ids.Count - 1) return ids[i] + 1;
                else
                {
                    if (ids[i] != ids[i + 1] - 1) return ids[i] + 1;
                }
            }
            return -1; //this shouldnt happen.
        }
        public static int NextTriggerId(Triggerscript script)
        {
            List<int> ids = script.Triggers.Keys.ToList();
            ids.Sort();
            for (int i = 0; i < ids.Count; i++)
            {
                //this is the last one.
                if (i == ids.Count - 1) 
                    return ids[i] + 1;
                else
                {
                    if (ids[i] != ids[i + 1] - 1) 
                        return ids[i] + 1;
                }
            }
            return -1; //this shouldnt happen.
        }
        public static IEnumerable<Logic> Logics(Trigger trigger, TriggerLogicSlot slot)
        {
            if (slot == TriggerLogicSlot.Condition) return trigger.Conditions;
            else if (slot == TriggerLogicSlot.EffectTrue) return trigger.TriggerEffectsOnTrue;
            else return trigger.TriggerEffectsOnFalse;
        }
        public static IEnumerable<Var> Variables(Triggerscript script, VarType type)
        {
            return script.TriggerVars.Values.Where(v => v.Type == type);
        }
        /// <summary>
        /// Is var id used in any condition or effect?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger)
        {
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.Condition)) return true;
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.EffectTrue)) return true;
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.EffectFalse)) return true;
            return false;
        }
        /// <summary>
        /// Is var id used in any logic of a particular slot?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger, TriggerLogicSlot slot)
        {
            IEnumerable<Logic> logics = Logics(trigger, slot);
            for (int i = 0; i < logics.Count(); i++)
            {
                if (VarUsedIn(varid, trigger, slot, i)) return true;
            }
            return false;
        }
        /// <summary>
        /// Is var id used in the logic of a particular slot at index?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger, TriggerLogicSlot slot, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, slot);

            foreach (Logic logic in logics)
            {
                foreach (var (sigid, _) in logic.StaticParamInfo)
                {
                    if (logic.GetValueOfParam(sigid) == varid) return true;
                }
            }

            return false;
        }


        //Transformations
        public static bool TransferLogic(Trigger fromTrigger, TriggerLogicSlot fromType, int fromIndex, Trigger toTrigger, TriggerLogicSlot toType, int toIndex)
        {
            if (!CanTransfer(fromType, toType)) return false;
            //if (fromIndex >= TriggerLogicCount(fromTrigger, fromType)) return false;
            //if (toIndex >= TriggerLogicCount(toTrigger, toType)) return false;

            if (fromType == TriggerLogicSlot.Condition && toType == TriggerLogicSlot.Condition)
            {
                Condition move = fromTrigger.Conditions[fromIndex];
                fromTrigger.Conditions.Remove(move);
                toTrigger.Conditions.Insert(toIndex, move);
                return true;
            }


            Effect eff = null;
            if (fromType == TriggerLogicSlot.EffectTrue)
            {
                eff = fromTrigger.TriggerEffectsOnTrue[fromIndex];
                fromTrigger.TriggerEffectsOnTrue.Remove(eff);
            }
            if (fromType == TriggerLogicSlot.EffectFalse)
            {
                eff = fromTrigger.TriggerEffectsOnFalse[fromIndex];
                fromTrigger.TriggerEffectsOnFalse.Remove(eff);
            }
            if (eff == null) return false;

            if (toType == TriggerLogicSlot.EffectTrue)
            {
                toTrigger.TriggerEffectsOnTrue.Insert(toIndex, eff);
                return true;
            }
            if (toType == TriggerLogicSlot.EffectFalse)
            {
                toTrigger.TriggerEffectsOnFalse.Insert(toIndex, eff);
                return true;
            }

            return false;
        }
        public static int GetOrAddNullVar(Triggerscript script, VarType type)
        {
            Var ret = script.TriggerVars.Values.Where(v => v.IsNull && v.Type == type).FirstOrDefault((Var)null);
            if (ret == null)
            {
                ret = new Var()
                {
                    ID = script.NextTriggerVarID++,
                    IsNull = true,
                    Name = string.Format("Null{0}", type),
                    Type = type,
                    Value = "",
                    Refs = new List<int>()
                };
                script.TriggerVars.Add(ret.ID, ret);
            }
            ret.Name = "Null" + type.ToString() + "Var";
            return ret.ID;
        }

        //Validation
        public static bool CanTransfer(TriggerLogicSlot from, TriggerLogicSlot to)
        {
            if (from == to) return true;
            if (from == TriggerLogicSlot.EffectTrue && to == TriggerLogicSlot.EffectFalse) return true;
            if (from == TriggerLogicSlot.EffectFalse && to == TriggerLogicSlot.EffectTrue) return true;
            return false;
        }
        public static void Validate(Triggerscript script)
        {
            FixupVarLocality(script);
        }
        public static void FixupVarLocality(Triggerscript script)
        {
            foreach (Trigger t in script.Triggers.Values)
            {
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.Condition));
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.EffectTrue));
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.EffectFalse));
            }
        }
        private static void FixupVarLocalityFor(Triggerscript script, Trigger trigger, IEnumerable<Logic> logics)
        {
            foreach (Logic logic in logics)
            {
                foreach (int sigid in logic.StaticParamInfo.Keys)
                {
                    int val = logic.GetValueOfParam(sigid);
                    if (!script.TriggerVars.ContainsKey(val)) continue;
                    Var v = script.TriggerVars[val];
                    if (v.Refs == null) v.Refs = new List<int>();
                    if (!v.Refs.Contains(trigger.ID))
                    {
                        v.Refs.Add(trigger.ID);
                    }
                }
            }
        }

        public static void FixupTriggerVars(Triggerscript script)
        {

        }
    }
}
