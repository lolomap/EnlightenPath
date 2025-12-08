using System;
using System.Collections.Generic;
using Data;
using DI;
using EditorAttributes;
using Events;
using Events.EventPayloads;
using UI;
using UnityEngine;
using Utilities;
using Zenject;
using Void = EditorAttributes.Void;

public class MapManager : MonoBehaviour
{
	public Vector3 MapCenter;
	public Vector2 CellSize;
	public RoomSO StartRoom;
	public Room NoRoomPrefab;
	
	private GridMap _grid;
	private Room _cachedRoomObject;
	private Vector2Int _connectingSourceGridPos;
	private readonly Dictionary<Vector2Int, Room> _roomObjects = new();
	private readonly Dictionary<Vector2Int, int> _lightedRooms = new();
	private readonly List<Vector2Int> _waitingForLightRooms = new();

	public float Width => _grid.Width * CellSize.x;
	public float Height => _grid.Height * CellSize.y;
	
	[Inject] private EventBus _eventBus;
	[Inject] private DungeonConfig _config;
	[Inject] private TilesSelector _tilesSelector;
	[Inject] private PreviewManager _previewManager;

	private void Awake()
	{
		_grid = new(_config.Width, _config.Height);
		Init();
	}

	private void OnEnable()
	{
		_eventBus.PreviewMoved.EventRaised += OnPreviewMove;
		_eventBus.PreviewRotated.EventRaised += OnPreviewRotated;
		_eventBus.SubmitPlacing.EventRaised += OnSubmitPlacing;
		_eventBus.LightChanged.EventRaised += OnLightChanged;
	}
	
	private void OnDisable()
	{
		_eventBus.PreviewMoved.EventRaised -= OnPreviewMove;
		_eventBus.PreviewRotated.EventRaised -= OnPreviewRotated;
		_eventBus.SubmitPlacing.EventRaised -= OnSubmitPlacing;
		_eventBus.LightChanged.EventRaised -= OnLightChanged;
	}

	public void SetConnectingSource(Vector2Int sourceGridPos) => _connectingSourceGridPos = sourceGridPos;

	public RoomSO GetRoomInPos(Vector2Int position) => _grid.Get(position.x, position.y);

	public Vector2Int WorldToGrid(Vector3 position, bool loop = false)
	{
		Vector2Int result = new();
		
		position -= MapCenter;
		position.x += _grid.Width * CellSize.x / 2f;
		position.z += _grid.Height * CellSize.y / 2f;

		if (position.x < 0) position.x -= CellSize.x;
		if (position.z < 0) position.x -= CellSize.y;
		
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

	private void Init()
	{
		for (int x = 0; x < _grid.Width; x++)
		{
			for (int y = 0; y < _grid.Height; y++)
			{
				EraseRoom(new(x, y), false);
			}
		}

		PlaceRoom(StartRoom, WorldToGrid(MapCenter), false);
	}
	
	private void PlaceRoom(RoomSO roomConfig, Vector2Int gridPos, bool updateFog = true)
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
		
		if (updateFog)
			_eventBus.FogObstaclesDirty.RaiseEvent();
	}

	private void EraseRoom(Vector2Int gridPos, bool updateFog = true)
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
				_eventBus.FogObstaclesDirty.RaiseEvent();
		}
	}
	
	private void RotateRoom(Vector2Int gridPos, Direction direction)
	{
		RoomSO room = _grid.Get(gridPos.x, gridPos.y);
		RotateRoom(room, direction);
		
		if (_roomObjects.TryGetValue(gridPos, out Room roomObj))
			roomObj.transform.rotation = Quaternion.Euler(0, 90 * (int)direction, 0);
	}

	private static void RotateRoom(RoomSO room, Direction direction)
	{
		if (room == null || room.Direction == direction) return;
		
		for (int i = 0; i < room.Connections.Count; i++)
		{
			room.Connections[i] = (Direction)((int)(room.Connections[i] + ((int)direction - (int)room.Direction)) % (int)Direction.Count);
			if (room.Connections[i] < 0) room.Connections[i] += (int)Direction.Count;
		}
		room.Direction = direction;
	}

	private void LightCast(Vector2Int lightPos, int intensity, Action<Vector2Int> callback)
	{
		callback(_grid.LoopPosition(lightPos));

		bool downBlocked = false, leftBlocked = false, upBlocked = false, rightBlocked = false;
		for (int i = 1; i <= intensity; i++)
		{
			if (!GetRoomInPos(lightPos + Vector2Int.down * (i - 1)).Connections.Contains(Direction.Down))
				downBlocked = true;
			if (!GetRoomInPos(lightPos + Vector2Int.left * (i - 1)).Connections.Contains(Direction.Left))
				leftBlocked = true;
			if (!GetRoomInPos(lightPos + Vector2Int.right * (i - 1)).Connections.Contains(Direction.Right))
				rightBlocked = true;
			if (!GetRoomInPos(lightPos + Vector2Int.up * (i - 1)).Connections.Contains(Direction.Up))
				upBlocked = true;
			
			if (!downBlocked)
				callback(_grid.LoopPosition(lightPos + Vector2Int.down * i));
			if (!leftBlocked)
				callback(_grid.LoopPosition(lightPos + Vector2Int.left * i));
			if (!upBlocked)
				callback(_grid.LoopPosition(lightPos + Vector2Int.up * i));
			if (!rightBlocked)
				callback(_grid.LoopPosition(lightPos + Vector2Int.right * i));
		}
	}
	
	private void OnPreviewMove(Vector3 pos)
	{
		Vector2Int gridPos = WorldToGrid(pos, true);

		bool isValid = _waitingForLightRooms.Contains(gridPos) &&
			_grid.IsValidPlace(gridPos, _tilesSelector.Selected.Content.Connections, _connectingSourceGridPos);
		_previewManager.SetBlocked(!isValid);

		if (_cachedRoomObject != null)
			GameObjectManipulator.ToggleCollision(_cachedRoomObject.gameObject, true);
		if (_roomObjects.TryGetValue(gridPos, out _cachedRoomObject))
			GameObjectManipulator.ToggleCollision(_cachedRoomObject.gameObject, false);
		_eventBus.FogObstaclesDirty.RaiseEvent();
	}

	private void OnPreviewRotated(Vector3 pos)
	{
		Direction newDirection = (Direction)((int)(_tilesSelector.Selected.Content.Direction + 1) % (int)Direction.Count);
		RotateRoom(_tilesSelector.Selected.Content, newDirection);
		
		OnPreviewMove(pos);
	}

	private void OnSubmitPlacing(Vector3 pos)
	{
		PlaceRoom(_tilesSelector.Selected.Content, WorldToGrid(pos));
		_tilesSelector.Remove(_tilesSelector.Selected);
	}

	private void OnLightChanged(LightChangedPayload context)
	{
		Vector2Int lastGridPos = WorldToGrid(context.Last);
		Vector2Int presentGridPos = WorldToGrid(context.Present);

		_waitingForLightRooms.Clear();

		LightCast(lastGridPos, context.Intensity, (gridPos) =>
		{
			if (_lightedRooms.ContainsKey(gridPos)) _lightedRooms[gridPos]--;
		});
		
		LightCast(presentGridPos, context.Intensity, (gridPos) =>
		{
			if (_lightedRooms.TryAdd(gridPos, 1))
			{
				if (gridPos != presentGridPos)
					_waitingForLightRooms.Add(gridPos);
			}
			else _lightedRooms[gridPos]++;
		});

		List<Vector2Int> toRemove = new();
		foreach ((Vector2Int gridPos, int count) in _lightedRooms)
		{
			if (count > 0) continue;
			
			toRemove.Add(gridPos);
			EraseRoom(gridPos, false);
		}
		foreach (Vector2Int gridPos in toRemove)
		{
			_lightedRooms.Remove(gridPos);
		}
		
		_eventBus.FogObstaclesDirty.RaiseEvent();
		
		if (_waitingForLightRooms.Count > 0)
			_eventBus.RequestTiles.RaiseEvent(_waitingForLightRooms.Count);
		else _eventBus.ToggleMovementUI.RaiseEvent(true);
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