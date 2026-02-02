using System.Collections.Generic;
using UnityEngine;
namespace Spawnables.Data
{
	public enum SpawnLocation
	{
		UpLeftCorner,
		UpRightCorner,
		DownLeftCorner,
		DownRightCorner,
		Center,
		
		DownWall,
		LeftWall,
		UpWall,
		RightWall,
		
		Count
	}
	
	public interface ISpawnObject
	{
		public SpawnObjectSO IData { get; }
		public void OnSpawn(SpawnObjectSO data);
		public void OnDestroy();
	}

	public interface IMonsterTrigger : ISpawnObject
	{
		public void UpdateTrigger();
	}
	
	[CreateAssetMenu(fileName = "Spawnable", menuName = "Game/Spawnable")]
	public class SpawnObjectSO : ScriptableObject
	{
		public GameObject Prefab;
		public Sprite Preview;
		public float SpawnChance;
		public List<SpawnLocation> PossibleLocations = new() { SpawnLocation.UpRightCorner };

		private void OnValidate()
		{
			if (Prefab == null)
				Debug.LogWarning("No prefab to spawn object!");

			if (PossibleLocations.Count < 1)
			{
				Debug.LogWarning("Object must have at least one location to spawn! Added automatically");
				PossibleLocations.Add(SpawnLocation.UpRightCorner);
			}
		}
	}
}