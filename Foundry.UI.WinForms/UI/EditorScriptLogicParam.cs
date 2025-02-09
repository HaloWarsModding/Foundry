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
        public EditorScriptLogicParam(Triggerscript script, Logic logic, int sigid, AssetCache cache)
        {
            InitializeComponent();

            varlabel.Visible = false;
            varsel.Visible = false;
            vallabel.Visible = false;
            valsel.Visible = false;

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

            string pCurVal = logic.Params[sigid];

            varlabel.Visible = true;
            varsel.Visible = true;

            varsel.Items.AddRange(script.Constants[pType].Keys.ToArray());
            varsel.SelectedItem = pCurVal;

            var enums = AssetDatabase.TriggerscriptVarValEnums(pType, cache);

            //no values available.
            if (enums.Count() == 0)
                return;

            vallabel.Visible = true;
            valsel.Visible = true;
        }
    }
}
