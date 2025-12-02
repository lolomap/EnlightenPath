using Data;
using Zenject;

namespace DI
{
	public class DungeonInstaller  : MonoInstaller
	{
		public DungeonConfig DungeonConfig;

		public override void InstallBindings()
		{
			Container.Bind<DungeonConfig>().FromScriptableObject(DungeonConfig).AsSingle();
		}
	}
}