using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.Data
{
	public enum SpawnLocation
	{
		UpLeftCorner,
		UpRightCorner,
		DownLeftCorner,
		DownRightCorner,
		
		DownWall,
		LeftWall,
		UpWall,
		RightWall,
		
		Count
	}
	
	[CreateAssetMenu(fileName = "Item", menuName = "Game/Item")]
	public class SpawnObjectSO : ScriptableObject
	{
		public GameObject Prefab;
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

	public interface ISpawnObject
	{
		public void OnSpawn() {}
	}
}