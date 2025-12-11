using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right,
        
        Count
    }

    public static class Connections
    {
        public static Vector2Int ToOffset(Direction direction)
        {
            return direction switch
            {
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.Up => Vector2Int.up,
                Direction.Right => Vector2Int.right,
                Direction.Count => Vector2Int.zero,
                _ => Vector2Int.zero
            };
        }
        
        public static bool HasConnected(Direction originToTargetDirection, List<Direction> targetConnections, List<Direction> originConnections)
        {
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
    }
}
