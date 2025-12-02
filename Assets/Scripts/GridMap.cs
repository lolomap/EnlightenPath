using System;
using System.Collections.Generic;
using Data;
using Utilities;

public class GridMap
{
	public int Width { get; }
	public int Height { get; }
	
	private readonly RoomSO[,] _grid;

	public GridMap(int width, int height)
	{
		Width = width;
		Height = height;

		_grid = new RoomSO[width, height];
	}
	
	public RoomSO Replace(int x, int y, RoomSO room)
	{
		x = Math.Abs(x % Width);
		y = Math.Abs(y % Height);

		RoomSO replaced = _grid[x, y];

		room.GridPos = new(x, y);
		_grid[x, y] = room;

		return replaced;
	}

	public RoomSO Get(int x, int y)
	{
		x = Math.Abs(x % Width);
		y = Math.Abs(y % Height);
		
		return _grid[x, y];
	}

	public bool IsValidPlace(int x, int y)
	{
		bool result = false;
		
		x = Math.Abs(x % Width);
		y = Math.Abs(y % Height);
		
		if (_grid[x, y] != null)
			return false;

		RoomSO downNeighbor = Get(x, y + 1);
		RoomSO leftNeighbor = Get(x - 1, y);
		RoomSO upNeighbor = Get(x, y - 1);
		RoomSO rightNeighbor = Get(x+1, y);

		if (downNeighbor != null && downNeighbor.Connections.Contains(Direction.Up)) result = true;
		if (leftNeighbor != null && leftNeighbor.Connections.Contains(Direction.Right)) result = true;
		if (upNeighbor != null && upNeighbor.Connections.Contains(Direction.Down)) result = true;
		if (rightNeighbor != null && rightNeighbor.Connections.Contains(Direction.Left)) result = true;
		
		return result;
	}
}