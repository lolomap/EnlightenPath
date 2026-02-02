using System.Collections.Generic;
using Data;
using DI;
using EditorAttributes;
using Events;
using FischlWorks_FogWar;
using Spawnables.Data;
using UnityEngine;
using Utilities;
using Zenject;
using Void = EditorAttributes.Void;

public class MapManager : MonoBehaviour
{
	public Vector3 MapCenter;
	public Vector2 CellSize;
	public Vector2Int StartRoomPos;
	public RoomSO StartRoom;
	public Room NoRoomPrefab;
	
	private GridMap _grid;
	private readonly Dictionary<Vector2Int, Room> _roomObjects = new();
	public readonly List<IMonsterTrigger> Monsters = new();

	public float Width => _grid.Width * CellSize.x;
	public float Height => _grid.Height * CellSize.y;
	public Vector3 StartPos { get; private set; }
	public Vector2Int ConnectingSourceGridPos { get; set; }
	
	[Inject] private EventBus _eventBus;
	[Inject] private DungeonConfig _config;
	[Inject] private csFogWar _fogWar;

	private void Awake()
	{
		_grid = new(_config.Width, _config.Height);
		StartPos = GridToWorld(StartRoomPos);
	}

	private void OnEnable()
	{
		_fogWar.FogIsReady += Init;
	}
	
	private void OnDisable()
	{
		_fogWar.FogIsReady -= Init;
	}

	public RoomSO GetRoomInPos(Vector2Int position) => _grid.Get(position.x, position.y);
	public Room GetRoomObjectInPos(Vector2Int position) => _roomObjects.GetValueOrDefault(position);
	public Transform GetFreeSpawnPivot(Vector2Int position)
	{
		Room room = _roomObjects[position];
		foreach ((SpawnLocation _, Transform pivot) in room.SpawnPivots)
		{
			if (pivot.childCount > 0) continue;
			return pivot;
		}

		return null;
	}

	public Vector2Int WorldToGrid(Vector3 position, bool loop = false)
	{
		Vector2Int result = new();
		
		position -= MapCenter;
		position.x += _grid.Width * CellSize.x / 2f;
		position.z += _grid.Height * CellSize.y / 2f;

		if (position.x < 0) position.x -= CellSize.x;
		if (position.z < 0) position.z -= CellSize.y;
		
		result.x = (int) (position.x / CellSize.x);
		result.y = (int) (position.z / CellSize.y);

		return loop ? _grid.LoopPosition(result) : result;
	}

	public Vector3 GridToWorld(Vector2Int position, bool loop = false)
	{
		if (loop) position = _grid.LoopPosition(position);
		
		Vector3 result = new(position.x * CellSize.x, 0f, position.y * CellSize.y);

		result.x -= _grid.Width * CellSize.x / 2f;
		result.z -= _grid.Height * CellSize.y / 2f;
		result += MapCenter + new Vector3(CellSize.x / 2f, 0f, CellSize.y / 2f);
		
		return result;
	}

	public Vector2Int GridLoop(Vector2Int position) => _grid.LoopPosition(position);
	public Vector3 WorldLoop(Vector3 position) => GridToWorld(WorldToGrid(position, true));
	public bool IsValidPlace(Vector2Int targetPos, List<Direction> targetConnections, Vector2Int originPos, bool allowOccupied = false)
		=> _grid.IsValidPlace(targetPos, targetConnections, originPos, allowOccupied);
	
	private void Init()
	{
		for (int x = 0; x < _grid.Width; x++)
		{
			for (int y = 0; y < _grid.Height; y++)
			{
				EraseRoom(new(x, y), false);
			}
		}

		PlaceRoom(StartRoom, StartRoomPos, false);
		
		_eventBus.MapIsReady.RaiseEvent();
	}
	
	public void PlaceRoom(RoomSO roomConfig, Vector2Int gridPos, bool updateFog = true)
	{
		gridPos = _grid.LoopPosition(gridPos);
		
		Vector3 centeredPos = GridToWorld(gridPos, true);
		RoomSO roomData = Instantiate(roomConfig);

		if (_roomObjects.ContainsKey(gridPos))
		{
			Destroy(_roomObjects[gridPos].gameObject);
			_roomObjects.Remove(gridPos);
		}

		Room roomObj = DIGlobal.Instantiate(
			roomData.Prefab.gameObject, centeredPos, Quaternion.Euler(0, 90 * (int)roomConfig.Direction, 0), transform
			).GetComponent<Room>();
		
		_roomObjects[gridPos] = roomObj;
		
		roomData.GridPos = gridPos;
		
		_grid.Replace(gridPos.x, gridPos.y, roomData);
		
		if (roomData.IsPit)
			roomObj.BreakGround();
		else
			SpawnRoomObjects(gridPos, roomConfig);
		
		if (updateFog)
			_fogWar.MarkObstaclesDirty();
	}

	public void EraseRoom(Vector2Int gridPos, bool updateFog = true)
	{
		Vector3 centeredPos = GridToWorld(gridPos, true);

		Room roomObj;
		if (_roomObjects.ContainsKey(gridPos))
		{
			roomObj = _roomObjects[gridPos];
            		
            Destroy(roomObj.gameObject);
            _roomObjects.Remove(gridPos);
            _grid.Replace(gridPos.x, gridPos.y, null);
		}
		
		if (NoRoomPrefab != null)
		{
			roomObj = DIGlobal.Instantiate(NoRoomPrefab.gameObject, centeredPos, Quaternion.identity, transform).GetComponent<Room>();
			_roomObjects[gridPos] = roomObj;

			if (updateFog)
				_fogWar.MarkObstaclesDirty();
		}
	}
	
	private void RotateRoom(Vector2Int gridPos, Direction direction)
	{
		RoomSO room = _grid.Get(gridPos.x, gridPos.y);
		RotateRoom(room, direction);
		
		if (_roomObjects.TryGetValue(gridPos, out Room roomObj))
			roomObj.transform.rotation = Quaternion.Euler(0, 90 * (int)direction, 0);
	}

	public static void RotateRoom(RoomSO room, Direction direction)
	{
		if (room == null || room.Direction == direction) return;
		
		for (int i = 0; i < room.Connections.Count; i++)
		{
			room.Connections[i] = (Direction)((int)(room.Connections[i] + ((int)direction - (int)room.Direction)) % (int)Direction.Count);
			if (room.Connections[i] < 0) room.Connections[i] += (int)Direction.Count;
		}
		room.Direction = direction;
	}

	private void SpawnRoomObjects(Vector2Int gridPos, RoomSO room)
	{
		foreach (SpawnObjectSO spawnable in room.SpawnedInside)
		{
			//TODO: chance

			Room roomObject = _roomObjects[gridPos];
			
			Transform pivot = null;
			foreach (SpawnLocation location in spawnable.PossibleLocations)
			{
				if (!roomObject.SpawnPivots.TryGetValue(location, out pivot)) continue;
				if (pivot.childCount > 0) pivot = null;
				else break;
			}
			if (pivot == null) return;
			
			GameObject obj = DIGlobal.Instantiate(spawnable.Prefab, pivot);
			
			if (obj.HasComponent(out LightSource lightSource))
				_eventBus.LightSourceInstantiated.RaiseEvent(lightSource);
			if (obj.HasComponent(out SlotItem slotItem))
				roomObject.Placed.Add(slotItem);

			SpawnObjectSO objData = Instantiate(spawnable);
			objData.name = spawnable.name;
			if (obj.HasComponent(out ISpawnObject spawnObject))
			{
				spawnObject.OnSpawn(objData);
				roomObject.RoomItemsUI.AddItem(obj, objData);
			}
		}
	}
	
	#region INSPECTOR_TEST
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
	
	public void TestRotateRoom(int x, int y, Direction direction)
	{
		RotateRoom(new Vector2Int(x, y), direction);
		foreach (Direction connection in _grid.Get(x, y).Connections)
		{
			Debug.Log($"New connections in ({x}, {y}): {connection}");
		}
	}
	
	[FoldoutGroup("Test Placing", nameof(_testPlacePos), nameof(_testPlaceRoom))]
	[SerializeField]
#pragma warning disable CS0414 // Field is assigned but its value is never used
	private Void _testPlacing;
#pragma warning restore CS0414 // Field is assigned but its value is never used

	[SerializeField, HideProperty] private Vector2Int _testPlacePos;
	[SerializeField, HideProperty] private RoomSO _testPlaceRoom;
	[SerializeField, HideProperty] private bool _testPlaceUpdateFog;
	
	[Button]
	public void TestPlaceRoom()
	{
		PlaceRoom(_testPlaceRoom, _testPlacePos, _testPlaceUpdateFog);
	}
#endif
	#endregion
}