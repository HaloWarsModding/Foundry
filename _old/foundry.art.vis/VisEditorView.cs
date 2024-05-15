using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Asset
{
    public class VisEditorView : SceneView
    {
        public AssetModule Owner { get; private set; }
        private RefWrapper<VisXmlData> VisRef { get; set; }
        private List<RefWrapper<UgxBinData>> GeometryRefs { get; set; }

        public VisEditorView(AssetModule owner) : base(owner.Instance)
        {
            Owner = owner;
            VisRef = null;
            GeometryRefs = new List<RefWrapper<UgxBinData>>();

            MaxZoomIn = -5.0f;
            UpdateCameraSetZoom(-75);

            Redraw();
        }

        public void SetData(RefWrapper<VisXmlData> vis)
        {
            if (VisRef != null)
            {
                foreach (var model in vis.Value.Models)
                {
                    RemoveGeometry(model.Name);
                }
            }

            VisRef = vis;
            GeometryRefs.Clear();

            foreach (var model in vis.Value.Models)
            {
                RefWrapper<UgxBinData> ugx = Owner.GetUgxData(new WorkspaceItem("art/" + model.Component.Asset.File + ".ugx"));
                GeometryRefs.Add(ugx);
                SetGeometry(model.Name, ugx.Value.Vertices, ugx.Value.Triangles);
                AddInstance(model.Name, Matrix.Identity);
                
                //temp - just do the default (first) model.
                break;
            }

            Redraw();
        }
    }
}