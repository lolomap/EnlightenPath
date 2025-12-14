using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data;
using UI;
using UnityEngine;
using Zenject;

[Serializable]
public class GrandCandleConfig
{
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
	private GrandCandleVisual _ui;

	public event Action Underflow;
	
	[Inject] private DungeonConfig _config;
	[Inject] private TilesSelector _tilesSelector;

	private void Awake()
	{
		foreach ((RoomSO room, int count) in _config.GrandCandle.Rooms)
		{
			for (int i = 0; i < count; i++)
			{
				_rooms.Add(Instantiate(room));
			}
		}
		
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
			
		if (!destroy && _tilesSelector != null)
			_tilesSelector.AddTiles(removed);

		return removed;
	}

	public void Push(RoomSO room, Origin origin)
	{
		int position = origin switch
		{
			Origin.Top => _rooms.Count,
			Origin.Bottom => 0,
			Origin.Random => 0, //TODO
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
}