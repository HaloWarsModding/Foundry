using Aga.Controls.Tree;
using Chef.HW1;
using Chef.HW1.Script;
using Chef.Win.Render;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static BrightIdeasSoftware.TreeListView;

namespace Chef.Win.UI
{
    public class BrowserWindow : DockContent
    {
        TreeView tree;
        public BrowserWindow()
        {
            tree = new TreeView();
            tree.Dock = DockStyle.Fill;
            Controls.Add(tree);
        }

        public void Update(AssetCache cache, GpuCache gcache, DockPanel target)
        {
            tree.Nodes.Clear();

            tree.NodeMouseDoubleClick += (s, e) =>
            {
                if (e.Node.Tag == "script")
                {
                    TriggerscriptWindow window = new TriggerscriptWindow(cache, gcache);
                    window.ScriptName = e.Node.Text;
                    window.Show(target);
                }
            };

            var scriptsRoot = tree.Nodes.Add("triggerscripts");
            foreach (var s in cache.Triggerscripts)
            {
                var scriptNode = scriptsRoot.Nodes.Add(s.Key);
                scriptNode.Tag = "script";
            }
        }
    }
}
