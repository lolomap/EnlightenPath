using Data;

namespace DI
{
	public class DungeonInstaller  : BaseInstaller
	{
		public DungeonConfig DungeonConfig;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.Bind<DungeonConfig>().FromScriptableObject(DungeonConfig).AsSingle();
		}
	}
}