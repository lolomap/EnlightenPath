using Data;
using UI;

namespace DI
{
	public class DungeonInstaller  : BaseInstaller
	{
		public DungeonConfig DungeonConfig;
		public MapManager MapManager;
		public LightManager LightManager;
		public TilesSelector TilesSelector;
		public PreviewManager PreviewManager;
		public GrandCandle GrandCandle;
		public Inventory Inventory;

		public override void InstallBindings()
		{
			base.InstallBindings();
			
			Container.Bind<DungeonConfig>().FromScriptableObject(DungeonConfig).AsSingle();
			Container.Bind<MapManager>().FromInstance(MapManager).AsSingle();
			Container.Bind<LightManager>().FromInstance(LightManager).AsSingle();
			Container.Bind<TilesSelector>().FromInstance(TilesSelector).AsSingle();
			Container.Bind<PreviewManager>().FromInstance(PreviewManager).AsSingle();
			Container.Bind<GrandCandle>().FromInstance(GrandCandle).AsSingle();
			Container.Bind<Inventory>().FromInstance(Inventory).AsSingle();
		}
	}
}