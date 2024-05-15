using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Art.Granny
{
    public class UgxViewerPage : SceneView
    {
        public GrannyModule GrannyModule { get; private set; }
        private WorkspaceItem LoadedItem { get; set; }

        public UgxViewerPage(GrannyModule owner, WorkspaceItem ugx) : base(owner.Instance)
        {
            MaxZoomIn = -5.0f;
            UpdateCameraSetZoom(-75);
            
            LoadedItem = ugx;

            GrannyModule module;
            Instance.GetModuleByType(out module);
            GrannyModule = module;

            Geometry3D geometry = UGXImporter.ImportUGXGeometry(LoadedItem.FullPath);
            MeshGeometryHelper.CalculateNormals((MeshGeometry3D)geometry);

            SetGeometry("View", geometry);
            AddInstance("View", Matrix.Identity);

            MeshBuilder planeBuilder = new MeshBuilder();
            planeBuilder.AddQuad(new Vector3(-15, 0, 15), new Vector3(-15, 0, -15), new Vector3(15, 0, -15), new Vector3(15, 0, 15));
            MeshNode plane = new MeshNode()
            {
                ModelMatrix = Matrix.Identity,
                Material = new DiffuseMaterialCore() { DiffuseColor = new Color4(.5f, .5f, .5f, 1.0f) },
                Geometry = planeBuilder.ToMeshGeometry3D(),
            };
            viewport.Items.AddChildNode(plane);

            Redraw();
        }
    }
}
