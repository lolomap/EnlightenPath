using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
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
		(x, y) = LoopPosition(x, y);

		RoomSO replaced = _grid[x, y];

		room.GridPos = new(x, y);
		_grid[x, y] = room;

		return replaced;
	}

	public RoomSO Get(int x, int y)
	{
		(x, y) = LoopPosition(x, y);
		
		return _grid[x, y];
	}

	public bool IsValidPlace(int x, int y, List<Direction> connections)
	{
		bool result = false;

		(x, y) = LoopPosition(x, y);
		
		if (_grid[x, y] != null)
			return false;

		RoomSO downNeighbor = connections.Contains(Direction.Down) ? Get(x, y - 1) : null;
		RoomSO leftNeighbor = connections.Contains(Direction.Left) ? Get(x - 1, y) : null;
		RoomSO upNeighbor = connections.Contains(Direction.Up) ? Get(x, y + 1) : null;
		RoomSO rightNeighbor = connections.Contains(Direction.Right) ? Get(x + 1, y) : null;

		if (downNeighbor != null && downNeighbor.Connections.Contains(Direction.Up)) result = true;
		if (leftNeighbor != null && leftNeighbor.Connections.Contains(Direction.Right)) result = true;
		if (upNeighbor != null && upNeighbor.Connections.Contains(Direction.Down)) result = true;
		if (rightNeighbor != null && rightNeighbor.Connections.Contains(Direction.Left)) result = true;
		
		return result;
	}

	public Vector2Int LoopPosition(Vector2Int pos)
	{
		(int x, int y) = LoopPosition(pos.x, pos.y);
		return new(x, y);
	}
	private (int, int) LoopPosition(int x, int y)
	{
		x %= Width;
		y %= Height;
		if (x < 0)
			x = Width + x;
		if (y < 0)
			y = Height + y;

		return (x, y);
	}
}