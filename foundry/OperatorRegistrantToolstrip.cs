using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public class OperatorRegistrantToolstrip : OperatorRegistrant
    {
        private void AddMenuItemChildren(ToolStripMenuItem item, Operator op)
        {
            foreach (Operator childOp in op.Children)
            {
                ToolStripMenuItem childItem = new ToolStripMenuItem(childOp.Name, null, 
                    (sender, e) => 
                    { 
                        childOp.Activate(); 
                    });
                item.DropDownItems.Add(childItem);

                ArrayList items = new ArrayList(item.DropDownItems);
                items.Sort(new ToolStripItemComparer());
                item.DropDownItems.Clear();

                foreach(ToolStripMenuItem sorted in items)
                {
                    item.DropDownItems.Add(sorted);
                }

                AddMenuItemChildren(childItem, childOp);
            }
        }

        public List<ToolStripMenuItem> GetRootMenuItems()
        {
            List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();
            foreach (Operator op in Operators)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(op.Name, null,
                    (sender, e) =>
                    {
                        op.Activate();
                    });
                AddMenuItemChildren(item, op);
                ret.Add(item);
            }
            return ret;
        }
    }

    public class ToolStripItemComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            ToolStripItem ix = (ToolStripItem)x;
            ToolStripItem iy = (ToolStripItem)y;
            return string.Compare(ix.Text, iy.Text, true);
        }
    }
}
