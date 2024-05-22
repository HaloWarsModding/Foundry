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
using WeifenLuo.WinFormsUI.Docking;
using static BrightIdeasSoftware.TreeListView;

namespace Foundry.HW1
{
    public class BrowserViewableClickedArgs
    {
        public MouseButtons Button { get; set; }
        public BrowserUIWinforms View { get; set; }
    }
    public interface IBrowserViewable
    {
        public IEnumerable<IBrowserViewable> BrowserChildren { get; }
    }

    internal class InternalModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public BrowserUIWinforms Owner;

        public InternalModel(BrowserUIWinforms owner)
        {
            Owner = owner;
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (Owner == null)
            {
                return new List<IBrowserViewable>();
            }

            if (treePath == null || treePath.LastNode == null)
            {
                return Owner.RootItems;
            }

            if (treePath.LastNode is IBrowserViewable)
            {
                return (treePath.LastNode as IBrowserViewable).BrowserChildren;
            }

            return new List<IBrowserViewable>();
        }
        public bool IsLeaf(TreePath treePath)
        {
            if (Owner == null)
            {
                return true;
            }

            if (treePath == null || treePath.LastNode == null)
            {
                return true;
            }

            if (treePath.LastNode is IBrowserViewable)
            {
                return (treePath.LastNode as IBrowserViewable).BrowserChildren.Count() == 0;
            }

            return true;
        }
        public void Update()
        {
            //UpdateRecursive(UserView.Root);
            StructureChanged?.Invoke(this, new TreePathEventArgs());
        }
    }

    public class BrowserNodesChangedArgs
    {
        public BrowserNodesChangedArgs(IBrowserViewable item)
        {
            Item = item;
        }

        public IBrowserViewable Item { get; private set; }
    }
    public class BrowserNodeClickedArgs
    {
        public BrowserNodeClickedArgs(MouseButtons button, IBrowserViewable nodeData, Point location)
        {
            Button = button;
            Item = nodeData;
            Location = location;
        }

        public MouseButtons Button { get; private set; }
        public IBrowserViewable Item { get; private set; }
        public Point Location { get; private set; }
    }

    public class BrowserUIWinforms : DockContent
    {
        public event EventHandler BrowserViewUpdated;
        public event EventHandler<BrowserNodesChangedArgs> BrowserRootAdded;
        public event EventHandler<BrowserNodesChangedArgs> BrowserRootRemoved;
        public event EventHandler<BrowserNodeClickedArgs> BrowserNodeClicked;
        public event EventHandler<BrowserNodeClickedArgs> BrowserNodeDoubleClicked;

        public ObservableCollection<IBrowserViewable> RootItems { get; private set; }

        private TreeViewAdv Tree { get; set; }

        public BrowserUIWinforms()
        {
            Text = "Browser";

            RootItems = new ObservableCollection<IBrowserViewable>();
            RootItems.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (object item in e.NewItems)
                    {
                        BrowserRootAdded?.Invoke(this, new BrowserNodesChangedArgs(item as IBrowserViewable));
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object item in e.OldItems)
                    {
                        BrowserRootRemoved?.Invoke(this, new BrowserNodesChangedArgs(item as IBrowserViewable));
                    }
                }
            };

            Tree = new TreeViewAdv();
            Tree.Dock = DockStyle.Fill;
            Tree.Model = new InternalModel(this);
            Tree.NodeMouseClick += (s, e) =>
            {
                BrowserNodeClicked?.Invoke(this, new BrowserNodeClickedArgs(
                    e.Button,
                    e.Node.Tag as IBrowserViewable,
                    e.Location)
                   );
            };
            Tree.NodeMouseDoubleClick += (s, e) =>
            {
                BrowserNodeDoubleClicked?.Invoke(this, new BrowserNodeClickedArgs(
                    e.Button,
                    e.Node.Tag as IBrowserViewable,
                    e.Location)
                   );
            };

            var c2 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            c2.DataPropertyName = "Icon";
            c2.ScaleMode = ImageScaleMode.ScaleDown;
            c2.VerticalAlign = VerticalAlignment.Top;
            Tree.NodeControls.Add(c2);

            var c = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            c.DataPropertyName = "Name";
            Tree.NodeControls.Add(c);

            Controls.Add(Tree);
        }

        public void UpdateView()
        {
            ((InternalModel)Tree.Model).Update();
            Tree.FullUpdate();
            BrowserViewUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
