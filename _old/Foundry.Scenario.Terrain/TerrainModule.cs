namespace Foundry.Scenario.Terrain
{
    public class TerrainModule : BaseModule
    {
        public override Type PageType => throw new NotImplementedException();
        
        public ScenarioModule Owner { get; private set; }

        public TerrainModule(ScenarioModule owner)
        {
            Owner = owner;
        }
    }
}