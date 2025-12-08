using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Data
{
	[CreateAssetMenu(fileName = "Room", menuName = "Game/Room", order = 0)]
	public class RoomSO : ScriptableObject
	{
		public Room Prefab;
		public Sprite Preview;
		public List<Direction> Connections = new() { Direction.Down };
		public Direction Direction;
		public List<SpawnObjectSO> SpawnedInside;
		public Vector2Int GridPos;

		private void OnValidate()
		{
			if (Connections.Count < 1)
			{
				Debug.LogWarning("Room must have at least one connection. Added automatically");
				Connections.Add(Direction.Down);
			}
		}
	}
}