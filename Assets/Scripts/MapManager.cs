using System.Collections.Generic;
using Data;
using EditorAttributes;
using Events;
using UnityEngine;
using Utilities;
using Zenject;

public class MapManager : MonoBehaviour
{
	public Vector3 MapCenter;
	public Vector2 CellSize;
	public RoomSO StartRoom;
	
	private GridMap _grid;
	private readonly Dictionary<Vector2Int, GameObject> _roomObjects = new();

	[Inject] private EventBus _eventBus;
	[Inject] private DungeonConfig _config;

	private void Awake()
	{
		_grid = new(_config.Width, _config.Height);
		
		Init();
	}

	public RoomSO GetRoomInPos(Vector2Int position) => _grid.Get(position.x, position.y);

	public Vector2Int WorldToGrid(Vector3 position)
	{
		Vector2Int result = new();
		
		position -= MapCenter;
		position.x += _grid.Width * CellSize.x / 2f;
		position.z += _grid.Height * CellSize.y / 2f;
		
		result.x = (int) (position.x / CellSize.x);
		result.y = (int) (position.z / CellSize.y);

		return result;
	}

	public Vector3 GridToWorld(Vector2Int position)
	{
		Vector3 result = new(position.x * CellSize.x, 0f, position.y * CellSize.y);

		result.x -= _grid.Width * CellSize.x / 2f;
		result.z -= _grid.Height * CellSize.y / 2f;
		result += MapCenter + new Vector3(CellSize.x / 2f, 0f, CellSize.y / 2f);
		
		return result;
	}

	private void Init()
	{
		PlaceRoom(StartRoom, WorldToGrid(MapCenter));
	}
	
	private void PlaceRoom(RoomSO roomConfig, Vector2Int gridPos)
	{
		Vector3 centeredPos = GridToWorld(gridPos);
		RoomSO roomData = Instantiate(roomConfig);

		GameObject roomObj = Instantiate(roomData.Prefab, centeredPos, Quaternion.identity, transform);
		_roomObjects[gridPos] = roomObj;
		roomData.GridPos = gridPos;
		_grid.Replace(gridPos.x, gridPos.y, roomData);
		_eventBus.FogObstaclesDirty.RaiseEvent();
	}

	private void RotateRoom(Vector2Int gridPos, Direction direction)
	{
		RoomSO room = _grid.Get(gridPos.x, gridPos.y);
		if (room == null || room.Direction == direction) return;
		
		_roomObjects[gridPos].transform.rotation = Quaternion.Euler(0, 90 * (int)direction, 0);
		for (int i = 0; i < room.Connections.Count; i++)
		{
			room.Connections[i] = (Direction)((int)(room.Connections[i] + ((int)direction - (int)room.Direction)) % (int)Direction.Count);
			if (room.Connections[i] < 0) room.Connections[i] += (int)Direction.Count;
		}
		room.Direction = direction;
	}
	
#if UNITY_EDITOR
	[FoldoutGroup("Test Position Convertion", nameof(_testPos), nameof(_testGridPos))]
	[SerializeField]
#pragma warning disable CS0414 // Field is assigned but its value is never used
	private Void _testPosConvertion;
#pragma warning restore CS0414 // Field is assigned but its value is never used
	
	[SerializeField, HideProperty] private Vector3 _testPos;
	[SerializeField, HideProperty] private Vector2Int _testGridPos;
	[Button]
	public void TestWorldToGrid()
	{
		_testGridPos = WorldToGrid(_testPos);
	}
	[Button]
	public void TestGridToWorld()
	{
		_testPos = GridToWorld(_testGridPos);
	}
	[Button]
	public void TestRotateRoom(int x, int y, Direction direction)
	{
		RotateRoom(new(x, y), direction);
		foreach (Direction connection in _grid.Get(x, y).Connections)
		{
			Debug.Log($"New connections in ({x}, {y}): {connection}");
		}
	}
#endif
}