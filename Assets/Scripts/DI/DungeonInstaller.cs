using Data;
using FischlWorks_FogWar;
using UI;
using Zenject;

namespace DI
{
	public class DungeonInstaller  : MonoInstaller
	{
		//TODO: make systems normal class, not MB
		public DungeonConfig DungeonConfig;
		public MapManager MapManager;
		public csFogWar FogWar;
		public LightManager LightManager;
		public TilesSelector TilesSelector;
		public PreviewManager PreviewManager;
		public GrandCandle GrandCandle;
		public Inventory Inventory;
		public TurnSequenceManager TurnSequenceManager;

		public override void InstallBindings()
		{
			Container.Bind<DungeonConfig>().FromScriptableObject(DungeonConfig).AsSingle();
			Container.BindInterfacesAndSelfTo<DungeonState>().AsSingle();
			
			Container.Bind<MapManager>().FromInstance(MapManager).AsSingle();
			Container.Bind<csFogWar>().FromInstance(FogWar).AsSingle();
			Container.Bind<LightManager>().FromInstance(LightManager).AsSingle();
			Container.Bind<TilesSelector>().FromInstance(TilesSelector).AsSingle();
			Container.Bind<PreviewManager>().FromInstance(PreviewManager).AsSingle();
			Container.Bind<GrandCandle>().FromInstance(GrandCandle).AsSingle();
			Container.Bind<Inventory>().FromInstance(Inventory).AsSingle();

			Container.Bind<TurnSequenceManager>().FromScriptableObject(TurnSequenceManager).AsSingle();
		}
	}
}