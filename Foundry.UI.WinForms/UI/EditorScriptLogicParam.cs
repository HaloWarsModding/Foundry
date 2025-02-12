using Chef.HW1;
using Chef.HW1.Script;
using static Chef.HW1.Script.TriggerscriptHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chef.Win.UI
{
    public partial class EditorScriptLogicParam : UserControl
    {
        private static IEnumerable<string> VarTypeHeaders(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return ["x", "y", "z"];
                default:
                    return [type.ToString()];
            }
        }
        private static char VarTypeColumnSeparator(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return ',';
                default:
                    return '\0';
            }
        }
        private static char VarTypeListSeparator(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return '|';
                default:
                    return ',';
            }
        }
        private static string VarTypeDefault(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return "0.0,0.0,0.0";
                default:
                    return "";
            }
        }

        public EditorScriptLogicParam(Triggerscript script, Logic logic, int sigid, AssetCache cache)
        {
            InitializeComponent();

            varlabel.Visible = false;
            varname.Visible = false;
            vallabel.Visible = false;

            //sigid is invalid.
            var pInfo = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            if (!pInfo.ContainsKey(sigid))
                return;

            var pType = pInfo[sigid].Type;

            //no vars available.
            if (!script.Constants.ContainsKey(pType))
                return;

            //param is null.
            if (!logic.Params.ContainsKey(sigid) || logic.Params[sigid] == null)
                return; //TODO: add option to denull it.

            string pCurName = logic.Params[sigid];
            if (!script.Constants[pType].ContainsKey(pCurName))
                return; //TODO: add option to set const val.

            string pCurVal = script.Constants[pType][pCurName];

            varlabel.Visible = true;
            varname.Visible = true;

            varname.Text = pCurName;

            //var enums = AssetDatabase.TriggerscriptVarValEnums(pType, cache);
            //no values available.
            //if (enums.Count() == 0)
            //    return;

            vallabel.Visible = true;

            var pColSep = VarTypeColumnSeparator(pType);
            var pRowSep = VarTypeListSeparator(pType);
            var pIsList = VarTypeIsList(pType);

            var pHeaders = VarTypeHeaders(pType);
            var pFormat = VarTypeFormat(pType);

            string[] rows = pIsList ? pCurVal.Split(pRowSep) : [pCurVal];
            int curIndex = 0;
            string[] cols = new string[pHeaders.Count()];
            for (int i = 0; i < rows[curIndex].Split(pColSep).Count(); i++)
            {
                cols[i] = rows[curIndex].Split(pColSep)[i];
            }

            Action Rebuild = () =>
            {
                rows[curIndex] = string.Join(pColSep, cols);
                script.Constants[pType][pCurName] = string.Join(pRowSep, rows);
            };

            for (int i = 0; i < pHeaders.Count(); i++)
            {
                int captureI = i;

                Label label = new Label();
                label.Text = pHeaders.ElementAt(i);
                label.Location = new Point(0, Bottom - 1);
                label.Width = Width / 3;
                label.Margin = new Padding(3, 3, 3, 3);
                label.TextAlign = ContentAlignment.MiddleRight;
                Controls.Add(label);

                Control cur;
                switch (pFormat)
                {
                    case VarFormat.Float:
                        cur = new NumericUpDown()
                        {
                            DecimalPlaces = 3
                        };
                        float val = 0.0f;
                        float.TryParse(cols[i], out val);
                        ((NumericUpDown)cur).Value = (decimal)val;
                        ((NumericUpDown)cur).ValueChanged += (_, _) =>
                            {
                                cols[captureI] = ((NumericUpDown)cur).Value.ToString();
                                Rebuild();
                            };
                        break;
                    default:
                        continue;
                }
                cur.Location = new Point(Width / 3, Bottom);
                cur.Width = Width / 3 * 2;
                cur.Margin = new Padding(3, 3, 3, 3);
                Controls.Add(cur);

                Size = new Size(Size.Width, Size.Height + Math.Max(label.Height, cur.Height) + 3);
            }
        }
    }
}
