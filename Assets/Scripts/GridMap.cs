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
		_grid[x, y] = room;
		if (room != null)
			room.GridPos = new(x, y);

		return replaced;
	}

	public RoomSO Get(int x, int y)
	{
		(x, y) = LoopPosition(x, y);
		
		return _grid[x, y];
	}

	public bool IsValidPlace(Vector2Int targetPos, List<Direction> targetConnections, Vector2Int originPos, bool allowOccupied = false)
	{
		(targetPos.x, targetPos.y) = LoopPosition(targetPos.x, targetPos.y);
		(originPos.x, originPos.y) = LoopPosition(originPos.x, originPos.y);

		if (targetPos.x != originPos.x && targetPos.y != originPos.y)
			return false;
		
		Direction originToTargetDirection = Direction.Down;
		if (targetPos.y > originPos.y)
			originToTargetDirection = Direction.Up;
		if (targetPos.x < originPos.x)
			originToTargetDirection = Direction.Left;
		else if (targetPos.x > originPos.x)
			originToTargetDirection = Direction.Right;
		
		RoomSO target = Get(targetPos.x, targetPos.y);
		RoomSO origin = Get(originPos.x, originPos.y);
		
		if (origin == null || target != null && !allowOccupied)
			return false;

		List<Direction> originConnections = origin.Connections;
		
		return
			originToTargetDirection == Direction.Up &&
				targetConnections.Contains(Direction.Down) && originConnections.Contains(Direction.Up) ||
			originToTargetDirection == Direction.Right &&
				targetConnections.Contains(Direction.Left) && originConnections.Contains(Direction.Right) ||
			originToTargetDirection == Direction.Down &&
				targetConnections.Contains(Direction.Up) && originConnections.Contains(Direction.Down) ||
			originToTargetDirection == Direction.Left &&
				targetConnections.Contains(Direction.Right) && originConnections.Contains(Direction.Left);
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