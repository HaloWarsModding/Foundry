using Chef.HW1.Map;
using Chef.HW1.Script;
using Chef.HW1.Unit;
using Chef.HW1.Workspace;
using DirectXTexNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Image = DirectXTexNet.Image;

namespace Chef.HW1
{
    public class Asset<T>
    {
        public Asset(T value, string file)
        {
            Value = value;
            File = file;
        }

        public T Value { get; set; }
        public string File { get; set; }
        public bool Edited { get; set; }
    }

    public class AssetCache
    {
        public Dictionary<string, Asset<Image>> Textures { get; set; } = new Dictionary<string, Asset<Image>>();
        public Dictionary<string, Asset<Model>> Models { get; set; } = new Dictionary<string, Asset<Model>>();
        public Dictionary<string, Asset<Visual>> Visuals { get; set; } = new Dictionary<string, Asset<Visual>>();
        public Dictionary<string, Asset<ProtoObject>> ProtoObjects { get; set; } = new Dictionary<string, Asset<ProtoObject>>();
        public Dictionary<string, Asset<Triggerscript>> Triggerscripts { get; set; } = new Dictionary<string, Asset<Triggerscript>>();
        public Dictionary<string, Asset<MissionEntry>> MissionEntries { get; set; } = new Dictionary<string, Asset<MissionEntry>>();
        public Dictionary<string, Asset<Scenario>> Scenarios { get; set; } = new Dictionary<string, Asset<Scenario>>();
        public Dictionary<string, Asset<TerrainVisual>> TerrainVisuals { get; set; } = new Dictionary<string, Asset<TerrainVisual>>();
        public Dictionary<string, Asset<ScratchImage>> TerrainTextures { get; set; } = new Dictionary<string, Asset<ScratchImage>>();
    }

    public class AssetDatabase
    {
        public static void Index(string dir, AssetCache cache)
        {
            foreach (string f in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                string name;

                name = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f);
                name = Path.GetRelativePath(dir, name);
                name = name.ToLower();
                switch (Path.GetExtension(f))
                {
                    // models
                    case ".ugx":
                        if (cache.Models.ContainsKey(name)) continue;
                        cache.Models.Add(name, new Asset<Model>(null, f));
                        break;

                    // visuals
                    case ".vis":
                        if (cache.Visuals.ContainsKey(name)) continue;
                        cache.Visuals.Add(name, new Asset<Visual>(null, f));
                        break;

                    // textures
                    case ".ddx":
                    case ".dds":
                        if (cache.Textures.ContainsKey(name)) continue;
                        cache.Textures.Add(name, new Asset<DirectXTexNet.Image>(null, f));
                        break;
                }

                name = Path.GetFileNameWithoutExtension(f);
                name = name.ToLower();
                switch(Path.GetExtension(f))
                {
                    // terrain visuals
                    case ".xtd":
                        if (cache.TerrainVisuals.ContainsKey(name)) continue;
                        cache.TerrainVisuals.Add(name, new Asset<TerrainVisual>(null, f));
                        break;

                    // terrain sims
                    case ".xsd":
                        //if (cache.TerrainSims.ContainsKey(name)) continue;
                        //cache.TerrainSims.Add(name, new Asset<TerrainSim>(null, f));
                        break;

                    // scenarios
                    case ".scn":
                        if (cache.Scenarios.ContainsKey(name)) continue;
                        Scenario scn;
                        using (Stream s = File.OpenRead(f))
                        {
                            scn = ScenarioIO.ReadXml(s);
                        }
                        //TODO: the sc2/sc3 loading here is kind of hacky... Should refactor the IO function to take a Scenario as input instead of giving a new one as output.
                        using (Stream s = File.OpenRead(Path.ChangeExtension(f, ".sc2")))
                        {
                            Scenario sc2 = ScenarioIO.ReadXml(s);
                            if (sc2 != null)
                                foreach (var o in sc2.Objects) scn.Objects.Add(o);
                        }
                        using (Stream s = File.OpenRead(Path.ChangeExtension(f, ".sc3")))
                        {
                            Scenario sc3 = ScenarioIO.ReadXml(s);
                            if (sc3 != null)
                                foreach (var o in sc3.Objects) scn.Objects.Add(o);
                        }
                        cache.Scenarios.Add(name, new Asset<Scenario>(scn, f));
                        break;

                    // scripts
                    case ".triggerscript":
                        if (cache.Scenarios.ContainsKey(name)) continue;
                        Triggerscript ts = new Triggerscript();
                        using (Stream s = File.OpenRead(f))
                        {
                           TriggerscriptIO.ReadXml(s, ts);
                        }
                        cache.Triggerscripts.Add(name, new Asset<Triggerscript>(ts, f));
                        TriggerscriptHelpers.Validate(ts);
                        break;
                }

                name = Path.GetFileNameWithoutExtension(f);
                name = name.ToLower();
                switch(Path.GetFileName(f))
                {
                    case "objects.xml":
                    case "objects_update.xml":
                        using (Stream s = File.OpenRead(f))
                        {
                            ProtoObject[] objects = ProtoObjectIO.ReadXml(s);
                            foreach (var obj in objects)
                            {
                                if (cache.ProtoObjects.ContainsKey(obj.Name.ToLower())) continue;
                                cache.ProtoObjects.Add(obj.Name.ToLower(), new Asset<ProtoObject>(obj, f));
                            }
                        }
                        break;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getter/loaders
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static Image GetOrLoadTexture(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Textures.ContainsKey(name))
            {
                return null;
            }

            var asset = cache.Textures[name];
            if (asset.Value ==  null)
            {
                asset.Value = TexHelper.Instance.LoadFromDDSFile(asset.File, DDS_FLAGS.NONE).GetImage(0);
            }

            return asset.Value;
        }
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static Model GetOrLoadModel(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Models.ContainsKey(name))
            {
                return null;
            }

            var asset = cache.Models[name];
            if (asset.Value == null) //not currently loaded.
            {
                using (Stream s = File.OpenRead(asset.File))
                {
                    var ugx = ModelIO.ReadUgx(s);
                    asset.Value = ugx;
                }
            }

            return asset.Value;
        }
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static Visual GetOrLoadVisual(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Visuals.ContainsKey(name))
            {
                return null;
            }

            var asset = cache.Visuals[name];
            if (asset.Value == null) //not currently loaded.
            {
                using (Stream s = File.OpenRead(asset.File))
                {
                    var vis = VisualIO.ReadXml(s);
                    asset.Value = vis;
                }
            }

            return asset.Value;
        }
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static ProtoObject GetOrLoadProtoObject(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.ProtoObjects.ContainsKey(name))
            {
                return null; //name was never indexed.
            }
            //xml is always immediately loaded, so just return what we have.
            return cache.ProtoObjects[name].Value;
        }
        /// <summary>
        /// </summary>
        /// <returns>An array of the scn, sc2, and sc3 scenario files, or null if name is not available.</returns>
        public static Scenario GetOrLoadScenario(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Scenarios.ContainsKey(name))
            {
                return null; //name was never indexed.
            }
            //xml is always immediately loaded when indexing, so just return what we have.
            return cache.Scenarios[name].Value;
        }
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static TerrainVisual GetOrLoadTerrainVisual(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.TerrainVisuals.ContainsKey(name))
            {
                return null; //name was never indexed.
            }

            var asset = cache.TerrainVisuals[name];
            if (asset.Value == null)
            {
                TerrainVisual vis = new TerrainVisual();
                using (Stream s = File.OpenRead(asset.File))
                {
                    TerrainIO.ReadXtd(s, vis);
                }
                using (Stream s = File.OpenRead(Path.ChangeExtension(asset.File, ".xtt")))
                {
                    TerrainIO.ReadXtt(s, vis);
                }
                using (Stream s = File.OpenRead(Path.ChangeExtension(asset.File, ".xsd")))
                {
                    TerrainIO.ReadXsd(s, vis);
                }
                asset.Value = vis;
            }

            return asset.Value;
        }
        
        // Triggerscripts
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static Triggerscript GetOrLoadTriggerscript(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Triggerscripts.ContainsKey(name))
            {
                return null;
            }
            //xml is always immediately loaded when indexing, so just return what we have.
            return cache.Triggerscripts[name].Value;
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static void SaveTriggerscript(string name, AssetCache cache, bool force = false)
        {
            name = name.ToLower();
            if (!cache.Triggerscripts.ContainsKey(name))
            {
                return;
            }
            var script = cache.Triggerscripts[name];

            if (script.Edited || force)
            {
                using (FileStream file = new FileStream(script.File, FileMode.OpenOrCreate))
                {
                    TriggerscriptIO.WriteXml(file, script.Value);
                }
            }
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static void TriggerscriptMarkEdited(string name, AssetCache cache, bool edited)
        {
            name = name.ToLower();
            if (!cache.Triggerscripts.ContainsKey(name))
            {
                return;
            }
            var script = cache.Triggerscripts[name];

            script.Edited = edited;
        }
        /// <summary>
        /// </summary>
        /// <returns>If the script has been edited.</returns>
        public static bool TriggerscriptIsEdited(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.Triggerscripts.ContainsKey(name))
            {
                return false;
            }
            var script = cache.Triggerscripts[name];

            return script.Edited;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // References and dependencies.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// </summary>
        /// <returns>A collection of models used by the visual.</returns>
        public static IEnumerable<string> ModelTextures(string name, int section, AssetCache cache)
        {
            Model model = GetOrLoadModel(name, cache);
            if (model == null) yield break;

            if (section >= model.Sections.Length) yield break;

            string diffuse = model.Sections[section].Material.GetMap(ModelMaterialMap.Diffuse);
            yield return "art" + diffuse;
        }
        /// <summary>
        /// </summary>
        /// <returns>A collection of models used by the visual.</returns>
        public static IEnumerable<string> VisualModelNames(string name, AssetCache cache)
        {
            name = name.ToLower();
            Visual vis = GetOrLoadVisual(name, cache);
            if (vis == null) yield break;

            foreach (var m in vis.Models)
            {
                string modelPath = "art\\" + m.Component.File;
                string modelExt = Path.GetExtension(modelPath);
                string modelName = modelPath.Substring(0, modelPath.Length - modelExt.Length);
                yield return modelName;
            }
        }
        /// <summary>
        /// </summary>
        /// <returns>The visual used by the proto object, or null if object or visual could not be found.</returns>
        public static string ObjectVisualName(string name, AssetCache cache)
        {
            name = name.ToLower();
            if (!cache.ProtoObjects.ContainsKey(name))
            {
                return null;
            }
            string visPath = "art\\" + cache.ProtoObjects[name].Value.Visual;
            string visExt = Path.GetExtension(visPath);
            string visName = visPath.Substring(0, visPath.Length - visExt.Length);
            return visName;
        }
        /// <summary>
        /// </summary>
        /// <returns>null if name is not available.</returns>
        public static TerrainVisual ScenarioTerrainVisual(string name, AssetCache cache)
        {
            if (!cache.Scenarios.ContainsKey(name))
            {
                return null;
            }
            //Scenario scn = GetOrLoadScenario(name, cache);
            return GetOrLoadTerrainVisual(name, cache);
        }
        public static IEnumerable<string> TerrainTextures(string name, AssetCache cache)
        {
            TerrainVisual terrain = GetOrLoadTerrainVisual(name, cache);
            if (terrain == null) yield break;
            foreach (var texture in terrain.Textures)
            {
                if (texture != "")
                    yield return "art\\terrain\\" + texture + "_df";
            }
        }
    }
}
