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
	private readonly List<RoomSO> _rooms = new();
	private GrandCandleVisual _ui;
	
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
		List<RoomSO> removed = Pop(count, true);
		_ui.UpdateHeight(_rooms.Count);
			
		if (!destroy && _tilesSelector != null)
			_tilesSelector.AddTiles(removed);

		return removed;
	}
	
	private List<RoomSO> Pop(int count, bool remove)
	{
		if (count > _rooms.Count) count = _rooms.Count;
		if (count <= 0) return new();
		
		List<RoomSO> result = _rooms.GetRange(_rooms.Count - count, count);
		
		if (remove)
			_rooms.RemoveRange(_rooms.Count - count, count);
				
		return result;
	}
}