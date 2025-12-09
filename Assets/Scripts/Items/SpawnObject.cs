using UnityEngine;

namespace Items
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
	}

	public interface ISpawnObject
	{
		public void OnSpawn() {}
	}
}