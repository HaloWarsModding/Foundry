using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Chef.HW1.Unit
{
    public static class VisualIO
    {
        public static Visual ReadXml(Stream stream)
        {
            Visual visual = new Visual();

            XDocument doc = XDocument.Load(stream);
            ReadModels(doc.Element("visual"), visual);
            
            return visual;
        }

        private static void ReadModels(XElement root, Visual visual)
        {
            List<VisualModel> models = new List<VisualModel>();
            foreach(XElement e in root.Elements("model"))
            {
                VisualModel vm = new VisualModel();
                vm.Name = e.Attribute("name").Value;
                vm.Component = new VisualComponent();

                var asset = e.Element("component").Element("asset");
                if (asset == null) continue;

                if (asset.Element("file") != null )
                {
                    vm.Component.File = e.Element("component").Element("asset").Element("file").Value + ".ugx";
                }
                if (asset.Element("damagefile") != null)
                {
                    vm.Component.DamageFile = e.Element("component").Element("asset").Element("damagefile").Value + ".dmg";
                }

                models.Add(vm);
            }
            visual.Models = models.ToArray();
        }
    }
}
