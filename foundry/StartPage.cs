using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry
{
    public partial class StartPage : DockContent
    {
        Panel panel;
        public StartPage()
        {
            panel = new Panel();
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            Controls.Add(panel);

            QuickStartOptions.OperatorAdded += (sender, e) => { RefreshOptions(); };
            QuickStartOptions.OperatorRemoved += (sender, e) => { RefreshOptions(); };

            Text = "Quick Start";
        }

        public OperatorRegistrant QuickStartOptions { get; } = new OperatorRegistrant();

        private void RefreshOptions()
        {
            panel.Controls.Clear();

            int y = 12;
            foreach(Operator op in QuickStartOptions.Operators)
            {
                LinkLabel label = new LinkLabel();
                label.Text = op.Name;
                label.Location = new Point(0, y);
                label.Size = new Size(200, 23);
                y += 25;
                panel.Controls.Add(label);
            }
        }

    }
}
