using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using Vector3 = SharpDX.Vector3;
using SharpDX;
using Color = SharpDX.Color;
using Foundry.Util;
using HelixToolkit.SharpDX.Core.Core;
using Foundry;
using Foundry.util;

namespace Foundry
{
	public struct Vertex
    {
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
	}

	public struct Triangle
    {
		public int A { get; set; }
		public int B { get; set; }
		public int C { get; set; }
	}

    public abstract class SceneView : BaseView
    {
        protected Panel renderControl;
        protected ViewportCore viewport;
        protected PerspectiveCameraCore camera;
        protected AmbientLightNode ambientLight;
        protected SpotLightNode directionalLight;
        protected EffectsManager effectsManager;
        public SceneView(FoundryInstance i) : base(i)
		{
            #region Viewport
            renderControl = new Panel
			{
				Width = 600,
				Height = 400,
				Location = new System.Drawing.Point(0, 0),
				Dock = DockStyle.Fill
			};
			AddElement(renderControl);

			//helix
			viewport = new ViewportCore(renderControl.Handle);
			viewport.EnableRenderFrustum = false; //TODO: culling is broken.

			camera = new PerspectiveCameraCore()
			{
				LookDirection = new Vector3(0, 0, 1),
				Position = new Vector3(0, 0, 0),
                FarPlaneDistance = 7500f,
                NearPlaneDistance = .1f,
                FieldOfView = 90,
                UpDirection = new Vector3(0, 1, 0),
            };
            viewport.CameraCore = camera;

			directionalLight = new SpotLightNode()
			{
				Color = Color.White.ToColor4().ChangeIntensity(.35f),
				ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
				Direction = new Vector3(0, 0, 1),
				Position = new Vector3(0, 600, 0),
				OuterAngle = 4.0f,
				Range = 2500.0f
			};
			viewport.Items.AddChildNode(directionalLight);

			ambientLight = new AmbientLightNode()
			{
				Color = Color.White.ToColor4().ChangeIntensity(.25f),
				ModelMatrix = Matrix.Translation(new Vector3(512, 100, 512)),
			};
			viewport.Items.AddChildNode(ambientLight);

			effectsManager = new DefaultEffectsManager();
			viewport.EffectsManager = effectsManager;

			viewport.StartD3D(Form.Size.Width, Form.Size.Height);
			
			viewport.ShowRenderDetail = true;
			viewport.BackgroundColor = new Color4(0.15f, 0.15f, 0.15f, 1.0f);
			#endregion


			ViewTick += (sender, e) =>
			{
				MouseState mouseState = GetMouseState();

				if (mouseState.middleDown)
				{
					if (GetKeyIsDown(Keys.ShiftKey))
					{
						//Pan
						UpdateCamera(mouseState.deltaX, mouseState.deltaY, 0, 0, 0);
					}
					else
					{
						//Rotate
						UpdateCamera(0, 0, 0, mouseState.deltaY / 100.0f, -mouseState.deltaX / 100.0f);
					}
				}
				else
				{
					float zoomMult = 1.0f;
					if (GetKeyIsDown(Keys.ShiftKey)) zoomMult = 4.0f;

					//Zoom
					UpdateCamera(0, 0, (mouseState.deltaScroll / 40.0f) * zoomMult, 0, 0);
				}
			};

            ViewDraw += (sender, e) =>
            {
                viewport.Render();
            };

            ViewResized += (sender, e) =>
            {
                viewport.Resize(Form.Size.Width, Form.Size.Height);
                viewport.Render();
            };

            ViewClosed += (sender, e) =>
			{
				viewport.EndD3D();
			};
        }


        //Camera
        private bool allowPan = true;
        public void SetAllowPan(bool pan)
        {
            allowPan = pan;
        }
		Vector2 cameraDir = new Vector2();
		Vector3 cameraTarget = new Vector3();
		float cameraZoom = -150.0f;
        public float MaxZoomOut { get; set; } = -2000.0f;
		public float MaxZoomIn { get; set; } = -30.0f;
        public void UpdateCamera(float panX, float panY, float zoom, float rotX, float rotY)
		{
			cameraDir += new Vector2(rotX, rotY);
			cameraDir.X = cameraDir.X.Clamp(-1.56f, 1.56f); //clamp to almost exactly up/down (radians).

			cameraZoom += zoom;
			cameraZoom = cameraZoom.Clamp(MaxZoomOut, MaxZoomIn); //clamp to these arbitrary values.


			var pitch = Quaternion.RotationAxis(Vector3.UnitX, cameraDir.X);
			var yaw = Quaternion.RotationAxis(Vector3.UnitY, cameraDir.Y);
			var rotation = yaw * pitch;

			var zoomOffset = Vector3.Transform(new Vector3(0, 0, cameraZoom), rotation);
			var rotationVec = Vector3.Transform(Vector3.UnitZ, rotation);


			cameraTarget += (Vector3.Transform(new Vector3(panX, panY, 0), rotation) * (-cameraZoom / 200.0f));

			camera.LookDirection = rotationVec;
			camera.Position = cameraTarget + zoomOffset;

			viewport.InvalidateRender();
		}
		public void UpdateCameraSetTarget(Vector3 target)
        {
            var pitch = Quaternion.RotationAxis(Vector3.UnitX, cameraDir.X);
            var yaw = Quaternion.RotationAxis(Vector3.UnitY, cameraDir.Y);
            var rotation = yaw * pitch;

            var zoomOffset = Vector3.Transform(new Vector3(0, 0, cameraZoom), rotation);
            var rotationVec = Vector3.Transform(Vector3.UnitZ, rotation);

            cameraTarget = target;
			camera.LookDirection = rotationVec;
            camera.Position = cameraTarget + zoomOffset;
        }
        public void UpdateCameraSetZoom(float zoom)
        {
            cameraZoom = zoom;
            cameraZoom = cameraZoom.Clamp(MaxZoomOut, MaxZoomIn); //clamp to these arbitrary values.
        }


        //Geometry
        private class FoundryInstancingMeshNode : InstancingMeshNode
		{
			public void UpdateInstances()
			{
				Instances = Instances.ToList();
				InstanceParamArray = InstanceParamArray.ToList();
			}
		}
		private Dictionary<string, FoundryInstancingMeshNode> instancedGeometry = new Dictionary<string, FoundryInstancingMeshNode>();
		public void SetGeometry(string name, IEnumerable<Vertex> vertices, IEnumerable<Triangle> triangles)
		{
			Geometry3D geometry = new MeshGeometry3D();
			geometry.Positions = new Vector3Collection();
			geometry.Indices = new IntCollection();

			foreach(Vertex vertex in vertices)
            {
				geometry.Positions.Add(vertex.Position);
            }
			foreach(Triangle triangle in triangles)
            {
				geometry.Indices.Add(triangle.A);
				geometry.Indices.Add(triangle.B);
				geometry.Indices.Add(triangle.C);
			}

			if (!instancedGeometry.ContainsKey(name))
			{
				instancedGeometry.Add(name,
				new FoundryInstancingMeshNode()
				{
					ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
					Material = new PhongMaterialCore() { DiffuseColor = Color.White.ToColor4() },
					InstanceIdentifiers = new List<Guid>(),
					InstanceParamArray = new List<InstanceParameter>(), // { new InstanceParameter() { DiffuseColor = Color.Red.ToColor4()} },
					Instances = new List<Matrix>(), // { Matrix.Translation(new Vector3(0,0,0)) },
				});

				viewport.Items.AddChildNode(instancedGeometry[name]);
			}

			instancedGeometry[name].Geometry = geometry;
		}
		public void RemoveGeometry(string name)
        {
			ClearInstances(name);
			instancedGeometry.Clear();
		}
		public Guid AddInstance(string name, Matrix matrix)
		{
			if(instancedGeometry.ContainsKey(name))
			{
				Guid guid = Guid.NewGuid();
				instancedGeometry[name].Instances.Add(matrix);
				instancedGeometry[name].InstanceParamArray.Add(new InstanceParameter() { DiffuseColor = Color.Red.ToColor4() });
				instancedGeometry[name].InstanceIdentifiers.Add(guid);
				instancedGeometry[name].UpdateInstances();
				return guid;
			}
			return Guid.Empty;
		}
		public void RemoveInstance(string name, Guid guid)
		{
			if (instancedGeometry.ContainsKey(name))
			{
				int index = instancedGeometry[name].InstanceIdentifiers.IndexOf(guid);
				if (index >= 0)
				{
					instancedGeometry[name].Instances.RemoveAt(index);
					instancedGeometry[name].InstanceParamArray.RemoveAt(index);
					instancedGeometry[name].InstanceIdentifiers.RemoveAt(index);
					instancedGeometry[name].UpdateInstances();
				}
			}
		}
		public void ClearInstances(string name)
		{
			if (instancedGeometry.ContainsKey(name))
			{
				instancedGeometry[name].Instances.Clear();
			}
		}
		public void SetInstanceMatrix(string name, Guid guid, Matrix matrix)
		{
			if(instancedGeometry.ContainsKey(name))
			{
				int index = instancedGeometry[name].InstanceIdentifiers.IndexOf(guid);
				if(index >= 0)
				{
					instancedGeometry[name].Instances[index] = matrix;
					instancedGeometry[name].Instances.RemoveAt(index);
					instancedGeometry[name].Instances.Insert(index, matrix);
					instancedGeometry[name].UpdateInstances();
				}
			}
		}
    }
}
