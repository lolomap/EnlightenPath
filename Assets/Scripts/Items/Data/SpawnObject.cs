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
		public SpawnLocation Location = SpawnLocation.UpRightCorner;

		private void OnValidate()
		{
			if (Prefab == null)
				Debug.LogWarning("No prefab to spawn object!");
		}
	}

	public interface ISpawnObject
	{
		public void OnSpawn() {}
	}
}