using Aga.Controls.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry
{
    public class PropertyView : BaseView
    {
        public object Object { get; set; }
        public PropertyView(FoundryInstance i) : base(i)
        {
            
        }
    }
}
