using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Map
{
    public enum ScenarioPlayers
    {
        Players2,
        Players4,
        Players6,
    }
    public enum ScenarioType
    {
        Campaign,
        Final,
        Development,
    }

    public class MissionEntry
    {
        public string MapName { get; set; }
        public ScenarioPlayers Players { get; set; }
        public ScenarioType Type { get; set; }
        public string File { get; set; }
        public string LoadingImageName { get; set; }
    }
}
