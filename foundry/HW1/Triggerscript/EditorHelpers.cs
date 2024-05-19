using KSoft.Phoenix.Phx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Foundry.HW1.Triggerscript.EditorParams;

namespace Foundry.HW1.Triggerscript
{
    public struct Selection
    {
        public Selection()
        {
            TriggerId = -1;
            LogicType = TriggerLogicSlot.Condition;
            LogicIndex = -1;
            VarSigId = -1;
        }

        public int TriggerId { get; set; }
        public TriggerLogicSlot LogicType { get; set; }
        public int LogicIndex { get; set; }
        public int VarSigId { get; set; }

        public static bool operator ==(Selection lhs, Selection rhs)
        {
            return (lhs.TriggerId == rhs.TriggerId
                && lhs.LogicType == rhs.LogicType
                && lhs.LogicIndex == rhs.LogicIndex
                && lhs.VarSigId == rhs.VarSigId);
        }
        public static bool operator !=(Selection lhs, Selection rhs)
        {
            return !(lhs == rhs);
        }
    }

    public static class EditorHelpers
    {
        //Bounds
        public static Rectangle ScriptBounds(Triggerscript script)
        {
            if (script.Triggers.Count == 0) return Rectangle.Empty;
            Trigger minX = script.Triggers.Values.First();
            Trigger minY = script.Triggers.Values.First();
            Trigger maxX = script.Triggers.Values.First();
            Trigger maxY = script.Triggers.Values.First();
            foreach(Trigger t in script.Triggers.Values)
            {
                if (t.X < minX.X) minX = t;
                if (t.Y < minY.Y) minY = t;
                if (t.X > maxX.X) maxX = t;
                if (t.Y > maxY.Y) maxY = t;
            }

            return new Rectangle(
                (int)minX.X,
                (int)minY.Y,
                (int)(maxX.X - minX.X + UnitBounds(maxX).Width),
                (int)(maxY.Y - minY.Y + UnitBounds(maxY).Height)
                );
        }
        public static Rectangle UnitBounds(Trigger trigger)
        {
            Rectangle triggerBounds = TriggerBounds(trigger);
            Rectangle ret = triggerBounds;

            foreach (var type in Enum.GetValues<TriggerLogicSlot>())
            {
                for (int i = 0; i < Logics(trigger, type).Count(); i++)
                {
                    Rectangle logicBounds = LogicBounds(trigger, type, i);
                    ret.Width = (logicBounds.X - triggerBounds.X) + logicBounds.Width;
                    ret.Height = Math.Max(ret.Height, logicBounds.Height);
                }
            }
            ret.Width += 50;

            return ret;
        }
        public static Rectangle TriggerBounds(Trigger trigger)
        {
            return new Rectangle(
                (int)trigger.X,
                (int)trigger.Y,
                DefaultWidth,
                HeaderHeight * 3);
        }
        public static Size LogicSize(Logic logic)
        {
            int varCount = logic.StaticParamInfo.Count;

            int width = DefaultWidth;

            return new Size(
                width, 
                HeaderHeight + (varCount * (VarHeight + VarSpacing)) + VarHeight
                );
        }
        public static Rectangle LogicBounds(Trigger trigger, TriggerLogicSlot type)
        {
            IEnumerable<Logic> logics;
            Point loc;
            if (type == TriggerLogicSlot.Condition)
            {
                logics = trigger.Conditions;
                loc = TriggerBounds(trigger).Location;
                loc.X += TriggerBounds(trigger).Width;
                loc.X += LogicSectionSpacing;
            }
            else if (type == TriggerLogicSlot.EffectTrue)
            {
                logics = trigger.TriggerEffectsOnTrue;
                loc = LogicBounds(trigger, TriggerLogicSlot.Condition).Location;
                foreach (var cnd in trigger.Conditions)
                {
                    loc.X += LogicSize(cnd).Width;
                    loc.X += LogicSpacing;
                }
                loc.X += LogicSectionSpacing;
            }
            else
            {
                logics = trigger.TriggerEffectsOnFalse;
                loc = LogicBounds(trigger, TriggerLogicSlot.EffectTrue).Location;
                foreach (var eff in trigger.TriggerEffectsOnTrue)
                {
                    loc.X += LogicSize(eff).Width;
                    loc.X += LogicSpacing;
                }
                loc.X += LogicSectionSpacing;
            }

            Size size = new Size(LogicSectionSpacing, 25);
            foreach (var l in logics)
            {
                size.Height = Math.Max(size.Height, LogicSize(l).Height);
                size.Width += LogicSize(l).Width;
                size.Width += LogicSpacing;
            }

            return new Rectangle(loc, size);
        }
        public static Rectangle LogicBounds(Trigger trigger, TriggerLogicSlot type, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);

            Point loc = LogicBounds(trigger, type).Location;
            for (int i = 0; i < index; i++)
            {
                loc.X += LogicSize(logics.ElementAt(i)).Width;
                loc.X += LogicSpacing;
            }

            return new Rectangle(loc, LogicSize(logics.ElementAt(index)));
        }
        public static Rectangle ParamNameBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = LogicBounds(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + (paramIndex * VarSpacing) + (paramIndex * VarHeight) + (VarHeight / 2),
                logicBounds.Width - (Margin * 2),
                VarNameHeight);
            return ret;
        }
        public static Rectangle ParamValBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = LogicBounds(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + (paramIndex * VarSpacing) + (paramIndex * VarHeight) + (VarHeight / 2) + VarNameHeight,
                logicBounds.Width -(Margin * 2),
                VarValHeight);
            return ret;
        }

        //Queries
        public static Selection SelectAt(Triggerscript script, Point point)
        {
            Selection ret = new Selection();

            ret.TriggerId = -1;
            ret.LogicIndex = -1;
            foreach (var trigger in script.Triggers.Values)
            {
                if (UnitBounds(trigger).Contains(point))
                {
                    ret.TriggerId = trigger.ID;

                    foreach (var type in Enum.GetValues<TriggerLogicSlot>())
                    {
                        if (LogicBounds(trigger, type).Contains(point))
                        {
                            ret.LogicType = type;

                            var logics = Logics(trigger, type);
                            int logicIndex = -1;
                            for(int i = 0; i < logics.Count(); i++)
                            {
                                if (LogicBounds(trigger, type, i).Contains(point))
                                {
                                    ret.LogicIndex = i;

                                    for(int v = 0; v < Logics(trigger, type).ElementAt(i).StaticParamInfo.Count(); v++)
                                    {
                                        if (ParamValBounds(trigger, type, i, v).Contains(point))
                                        {
                                            ret.VarSigId = Logics(trigger, type).ElementAt(i).StaticParamInfo.ElementAt(v).Key;
                                            return ret;
                                        }
                                    }
                                    return ret;
                                }
                            }
                            if (Logics(trigger, type).Count() == 0)
                            {
                                ret.LogicIndex = 0;
                                return ret;
                            }
                        }
                    }

                    if (TriggerBounds(trigger).Contains(point))
                    {
                        return ret;
                    }
                }
            }
            ret.TriggerId = -1;
            return ret;
        }
        public static Logic SelectedLogic(Triggerscript script, Selection selection)
        {
            if (selection.LogicType == TriggerLogicSlot.Condition)
            {
                return script.Triggers[selection.TriggerId].Conditions.ElementAt(selection.LogicIndex);
            }
            else if (selection.LogicType == TriggerLogicSlot.EffectTrue)
            {
                return script.Triggers[selection.TriggerId].TriggerEffectsOnTrue.ElementAt(selection.LogicIndex);
            }
            else if (selection.LogicType == TriggerLogicSlot.EffectFalse)
            {
                return script.Triggers[selection.TriggerId].TriggerEffectsOnFalse.ElementAt(selection.LogicIndex);
            }
            return null;
        }
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
                if (i == ids.Count - 1) return ids[i] + 1;
                else
                {
                    if (ids[i] != ids[i + 1] - 1) return ids[i] + 1;
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

            foreach(Logic logic in logics)
            {
                foreach(var (sigid, _) in logic.StaticParamInfo)
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
                var move = fromTrigger.Conditions[fromIndex];
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
                    Value = ""
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
            foreach(Var v in script.TriggerVars.Values)
            {
                int count = 0;
                int triggerId = -1;
                foreach (Trigger t in script.Triggers.Values)
                {
                    if (VarUsedIn(v.ID, t))
                    {
                        count++;
                        triggerId = t.ID;
                    }
                }
                if (count == 1)
                {
                    v.LocalTrigger = triggerId;
                }
                if (count > 1)
                {
                    v.LocalTrigger = -1;
                }
            }
        }
        public static void FixupTriggerVars(Triggerscript script)
        {

        }
    }
}
