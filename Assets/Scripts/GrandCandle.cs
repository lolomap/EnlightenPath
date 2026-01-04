using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Data;
using Events;
using Spawnables.Data;
using UI;
using UnityEngine;
using Utilities;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class GrandCandleConfig
{
	public SerializedDictionary<RoomSO, int> StartRooms;
	public SerializedDictionary<RoomSO, int> Rooms;
}

[RequireComponent(typeof(GrandCandleVisual))]
public class GrandCandle : MonoBehaviour
{
	public enum Origin
	{
		Top,
		Bottom,
		Random
	}
	
	private readonly List<RoomSO> _rooms = new();
	private readonly Dictionary<string, int> _items = new();
	private GrandCandleVisual _ui;

	public event Action Underflow;
	
	[Inject] private DungeonConfig _config;
	[Inject] private TilesSelector _tilesSelector;
	[Inject] private EventBus _eventBus;

	private void OnEnable()
	{
		_eventBus.ItemLost.EventRaised += OnLost;
	}

	private void OnDisable()
	{
		_eventBus.ItemLost.EventRaised -= OnLost;
	}

	private void Awake()
	{
		List<RoomSO> temp = new();
		
		foreach ((RoomSO room, int count) in _config.GrandCandle.Rooms)
		{
			for (int i = 0; i < count; i++)
			{
				temp.Add(Instantiate(room));
				RegisterItems(room);
			}
		}
		temp.Shuffle();
		_rooms.AddRange(temp);
		
		temp.Clear();
		
		foreach ((RoomSO room, int count) in _config.GrandCandle.StartRooms)
		{
			for (int i = 0; i < count; i++)
			{
				temp.Add(Instantiate(room));
				RegisterItems(room);
			}
		}
		temp.Shuffle();
		_rooms.AddRange(temp);
		
		_ui = GetComponent<GrandCandleVisual>();
		_ui.MaxElements = _rooms.Count;
	}

	public List<RoomSO> Pick(int count, bool destroy = false)
	{
		if (count > _rooms.Count)
		{
			count = _rooms.Count;
			Underflow?.Invoke();
		}
		
		List<RoomSO> removed = Pop(count);
		_ui.UpdateHeight(_rooms.Count);

		if (destroy)
		{
			foreach (SpawnObjectSO item in removed.SelectMany(room => room.SpawnedInside))
			{
				_eventBus.ItemLost.RaiseEvent(item);
			}
		}
		else if (_tilesSelector != null)
			_tilesSelector.AddTiles(removed);

		return removed;
	}

	public void Push(RoomSO room, Origin origin)
	{
		int position = origin switch
		{
			Origin.Top => _rooms.Count,
			Origin.Bottom => 0,
			Origin.Random => Random.Range(0, _rooms.Count + 1), //TODO: Deterministic Random
			_ => 0
		};
		_rooms.Insert(position, room);
	}

	//TODO: Origin argument
	private List<RoomSO> Pop(int count)
	{
		List<RoomSO> result = Get(count);
		_rooms.RemoveRange(_rooms.Count - count, count);
		return result;
	}
	
	private List<RoomSO> Get(int count)
	{
		return count <= 0 ? new() : _rooms.GetRange(_rooms.Count - count, count);
	}

	private void RegisterItems(RoomSO room)
	{
		foreach (SpawnObjectSO item in room.SpawnedInside)
		{
			if (!_items.TryAdd(item.name, 1)) _items[item.name]++;
		}
	}

	private void OnLost(SpawnObjectSO item)
	{
		if (_items.ContainsKey(item.name))
		{
			_items[item.name]--;
			if (_items[item.name] <= 0 && _config.RequiredItems.Any(x => x.name == item.name))
				_eventBus.GameOver.RaiseEvent();
		}
	}
}