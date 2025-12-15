using System.Collections.Generic;
using Spawnables.Data;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

[CreateAssetMenu(fileName = "Room", menuName = "Game/Room", order = 0)]
public class RoomSO : ScriptableObject
{
    public Room Prefab;
    public Sprite Preview;
    public List<Direction> Connections = new() { Direction.Down };
    public Direction Direction;
    public List<SpawnObjectSO> SpawnedInside;
    
    public Vector2Int GridPos { get; set; }
    
    private void OnValidate()
    {
        if (Connections.Count < 1)
        {
            Debug.LogWarning("Room must have at least one connection. Added automatically");
            Connections.Add(Direction.Down);
        }

        if (Prefab == null)
            Debug.LogWarning("No prefab to spawn room!");
    }
}