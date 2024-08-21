using Chef.HW1.Map;
using Chef.HW1.Unit;
using Chef.HW1;
using Chef.Util;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Image = DirectXTexNet.Image;

namespace Chef.Win.Render
{
    public static class ScenarioRenderer
    {
        public static void DrawObjects(RenderTargetView target, DepthStencilView depth, Camera camera, Scenario scenario, AssetCache assets, GpuCache gpuassets)
        {
            Dictionary<ModelSectionMesh, List<Matrix4x4>> instances = new Dictionary<ModelSectionMesh, List<Matrix4x4>>();
            Dictionary<ModelSectionMesh, ShaderResourceView> diffuses = new Dictionary<ModelSectionMesh, ShaderResourceView>();

            foreach (var o in scenario.Objects)
            {
                ProtoObject proto = AssetDatabase.GetOrLoadProtoObject(o.Unit, assets);
                if (proto == null) continue;

                string visName = AssetDatabase.ObjectVisualName(o.Unit, assets);
                if (visName == null) continue;

                foreach (var modelName in AssetDatabase.VisualModelNames(visName, assets))
                {
                    Model model = AssetDatabase.GetOrLoadModel(modelName, assets);
                    if (model == null) continue;

                    int seci = 0;
                    Vector3 position = Misc.FromString(o.Position);
                    Vector3 forward = Misc.FromString(o.Forward);
                    Vector3 right = Misc.FromString(o.Right);
                    Vector3 up = Vector3.Cross(forward, right);
                    //we need a left handed matrix, but c# only offers this function in right-handedness, so we invert the forward vector.
                    Matrix4x4 transform = Matrix4x4.CreateWorld(position, -forward, up);

                    foreach (var section in model.Sections)
                    {
                        ModelSectionMesh mesh = GpuDatabase.GetOrUploadModel(section, gpuassets);

                        if (!instances.ContainsKey(mesh))
                        {
                            string diffuseName = AssetDatabase.ModelTextures(modelName, seci, assets).First();
                            Image diffuseImage = AssetDatabase.GetOrLoadTexture(diffuseName, assets);
                            Texture2D diffuse;
                            if (diffuseImage != null)
                            {
                                diffuse = GpuDatabase.GetOrUploadTexture(diffuseImage, gpuassets);
                                diffuses.Add(mesh, new ShaderResourceView(D3DViewport.Device, diffuse));
                            }
                            instances.Add(mesh, new List<Matrix4x4>());
                        }
                        instances[mesh].Add(transform);
                        seci++;
                    }
                }
            }

            foreach (var (mesh, transforms) in instances)
            {
                ShaderResourceView diffuse = diffuses.ContainsKey(mesh) ? diffuses[mesh] : null;
                ModelRenderer.DrawMesh(
                                target,
                                depth,
                                mesh,
                                camera,
                                transforms.ToArray(),
                                diffuse);
                if (diffuse != null) diffuse.Dispose();
            }
        }
        //public static void DrawTerrain(RenderTargetView target, DepthStencilView depth, Camera camera, Scenario scenario, AssetCache assets, GpuCache gpuassets)
        //{
        //    TerrainVisual terrain = AssetDatabase.ScenarioTerrainVisual(ScenarioName, Assets);
        //    GpuDatabase.GetOrUploadTerrainVisual(TerrainVisual, GpuAssets)

        //        TerrainRenderer.DrawVisualMesh(
        //        target, depth,
        //        GpuTerrainVisual,
        //        Camera,
        //        Atlas);
        //}
    }
}
