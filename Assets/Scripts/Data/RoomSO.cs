using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Data
{
	[CreateAssetMenu(fileName = "Room", menuName = "Game/Room", order = 0)]
	public class RoomSO : ScriptableObject
	{
		public GameObject Prefab;
		public Sprite Preview;
		public List<Direction> Connections;
		public Direction Direction;
		public List<SpawnObjectSO> SpawnedInside;
		public Vector2Int GridPos;
	}
}