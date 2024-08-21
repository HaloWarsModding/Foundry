using Chef.HW1.Map;
using Chef.HW1.Unit;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Chef.Win.Render
{
    public class GpuCache
    {
        public Dictionary<TerrainVisual, TerrainVisualMesh> TerrainVisualMeshes { get; set; } = new Dictionary<TerrainVisual, TerrainVisualMesh>();
        public Dictionary<ModelSection, ModelSectionMesh> ModelSectionMeshes { get; set; } = new Dictionary<ModelSection, ModelSectionMesh>();
        public Dictionary<DirectXTexNet.Image, Texture2D> Textures { get; set; } = new Dictionary<DirectXTexNet.Image, Texture2D>();
        public Dictionary<TerrainVisual, Texture2D> TerrainTextures { get; set; } = new Dictionary<TerrainVisual, Texture2D>();
    }

    public class GpuDatabase
    {
        public static TerrainVisualMesh GetOrUploadTerrainVisual(TerrainVisual visual, GpuCache cache, bool reload = false)
        {
            //not loaded yet.
            if (!cache.TerrainVisualMeshes.ContainsKey(visual))
            {
                var mesh = TerrainRenderer.UploadVisualMesh(visual);
                cache.TerrainVisualMeshes.Add(visual, mesh);
            }
            //loaded, but we want to reload it.
            if(cache.TerrainVisualMeshes.ContainsKey(visual) && reload)
            {
                cache.TerrainVisualMeshes[visual].Dispose();
                cache.TerrainVisualMeshes.Remove(visual);
                var mesh = TerrainRenderer.UploadVisualMesh(visual);
                cache.TerrainVisualMeshes.Add(visual, mesh);
            }
            //return asset
            return cache.TerrainVisualMeshes[visual];
        }
        public static ModelSectionMesh GetOrUploadModel(ModelSection section, GpuCache cache, bool reload = false)
        {
            if (!cache.ModelSectionMeshes.ContainsKey(section))
            {
                var mesh = ModelRenderer.UploadSection(section);
                cache.ModelSectionMeshes.Add(section, mesh);
            }

            return cache.ModelSectionMeshes[section];
        }
        public static Texture2D GetOrUploadTexture(DirectXTexNet.Image image, GpuCache cache, bool reload = false)
        {
            if (!cache.Textures.ContainsKey(image))
            {
                var mesh = CommonRenderer.UploadTexture(image);
                cache.Textures.Add(image, mesh);
            }
            return cache.Textures[image];
        }
    }
}
