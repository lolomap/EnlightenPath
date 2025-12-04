using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data;
using UnityEngine;

[Serializable]
public class GrandCandleConfig
{
	public SerializedDictionary<RoomSO, int> Rooms;
}

public class GrandCandle
{
	private GrandCandleConfig _config;
	private readonly List<RoomSO> _rooms;

	public int Fuel => _rooms.Count;

	public GrandCandle(GrandCandleConfig config)
	{
		_config = config;
		_rooms = new();
		
		foreach ((RoomSO room, int count) in config.Rooms)
		{
			for (int i = 0; i < count; i++)
			{
				_rooms.Add(UnityEngine.Object.Instantiate(room));
			}
		}
	}

	public List<RoomSO> Pop(int count, bool remove)
	{
		if (count > _rooms.Count) count = _rooms.Count;
		if (count <= 0) return new();
		
		List<RoomSO> result = _rooms.GetRange(_rooms.Count - count, count);
		
		if (remove)
			_rooms.RemoveRange(_rooms.Count - count, count);
				
		return result;
	}
}