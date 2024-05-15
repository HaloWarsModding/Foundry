using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Foundry;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry.Unit
{
    public class UnitModule : BaseModule 
    {
        protected override void OnWorkspaceOpened()
        {
        }
        protected override void OnWorkspaceClosed()
        {
        }


        //public List<UnitPickerPage> UnitPickers { get; } = new List<UnitPickerPage>();
        public void UpdateUnitPickers()
        {
            UnitPickersUpdatedArgs args = new UnitPickersUpdatedArgs()
            {

            };
            UnitPickersUpdated(this, args);
        }
        public class UnitPickersUpdatedArgs
        {

        }
        public event EventHandler<UnitPickersUpdatedArgs> UnitPickersUpdated;


        public ObjectsXmlData Objects { get; private set; }
        public SquadsXmlData Squads { get; private set; }
    }
}
