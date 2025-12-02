using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data;

[Serializable]
public class GrandCandleConfig
{
	public SerializedDictionary<RoomSO, int> Rooms;
}

public class GrandCandle
{
	private GrandCandleConfig _config;
	private readonly List<RoomSO> _rooms;

	public GrandCandle(GrandCandleConfig config)
	{
		_config = config;
		_rooms = new();
		
		foreach ((RoomSO room, int count) in config.Rooms)
		{
			for (int i = 0; i < count; i++)
			{
				_rooms.Add(room);
			}
		}
	}

	public List<RoomSO> Pop(int count)
	{
		return _rooms.GetRange(_rooms.Count - count, count);
	}
}