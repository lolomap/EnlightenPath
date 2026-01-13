using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EditorAttributes;
using Events;
using FischlWorks_FogWar;
using UnityEngine;
using Utilities;
using Zenject;

public struct LightChangedContext
{
    public Vector2Int LastPos;
    public Vector2Int PresentPos;
    public int Intensity;
    public bool LastFlaming;
    public bool PresentFlaming;
}

public class LightManager : MonoBehaviour
{
    private bool _isReady;
    private readonly List<LightSource> _lightSources = new();
    private readonly Dictionary<Vector2Int, int> _lightedRooms = new();

    public HashSet<Vector2Int> WaitingForLightRooms { get; } = new();

    [Inject] private EventBus _eventBus;
    [Inject] private DungeonConfig _config;
    [Inject] private MapManager _mapManager;
    [Inject] private csFogWar _fogWar;
    [Inject] private GrandCandle _grandCandle;

    private void OnEnable()
    {
        _eventBus.MapIsReady.EventRaised += Init;
        _eventBus.LightSourceInstantiated.EventRaised += RegisterLightSource;
    }

    private void OnDisable()
    {
        _eventBus.MapIsReady.EventRaised -= Init;
        _eventBus.LightSourceInstantiated.EventRaised -= RegisterLightSource;
    }
    
    [Button]
    public void UpdateAllSources()
    {
        List<LightChangedContext> commonChanges = _lightSources
            .Where(lightSource => lightSource.enabled)
            .Select(lightSource => lightSource.UpdateLightPos()).ToList();
        
        List<LightChangedContext> extinguishedChanges = _lightSources
            .Where(lightSource => !lightSource.enabled)
            .Select(lightSource => lightSource.UpdateLightPos())
            .Where(context => context.LastFlaming).ToList();

        foreach (LightChangedContext context in commonChanges)
            UpdateDark(context);

        foreach (LightChangedContext context in extinguishedChanges)
            UpdateDark(context);
        
        foreach (LightChangedContext context in commonChanges)
            UpdateLight(context);

        ApplyLightEffect();
    }

    public void RegisterLightSource(LightSource source)
    {
        _lightSources.Add(source);
        if (_isReady)
            source.UpdateLightPos();
    }

    public void UnregisterLightSource(LightSource source)
    {
        _lightSources.Remove(source);
    }

    private void UpdateDark(LightChangedContext context)
    {
        LightCast(context.LastPos, context.Intensity, (gridPos) =>
        {
            if (_lightedRooms.ContainsKey(gridPos))
            {
                _lightedRooms[gridPos]--;
                if (_lightedRooms[gridPos] < 0) // Darkness cannot overlap
                    _lightedRooms[gridPos] = 0;
            }
        });
    }

    private void UpdateLight(LightChangedContext context)
    {
        LightCast(context.PresentPos, context.Intensity, (gridPos) =>
        {
            if (_lightedRooms.TryAdd(gridPos, 1))
            {
                if (gridPos != context.PresentPos && _mapManager.GetRoomInPos(gridPos) == null)
                    WaitingForLightRooms.Add(gridPos);
            }
            else _lightedRooms[gridPos]++;
        });
    }

    public void ForceApplyLightEffect() => ApplyLightEffect();

    private void Init()
    {
        UpdateAllSources();
        _isReady = true;
    }

    private void ApplyLightEffect()
    {
        List<Vector2Int> toRemove = new();
        foreach ((Vector2Int gridPos, int count) in _lightedRooms)
        {
            if (count > 0) continue;
			
            toRemove.Add(gridPos);
            _mapManager.EraseRoom(gridPos, false);
        }
        foreach (Vector2Int gridPos in toRemove)
        {
            _lightedRooms.Remove(gridPos);
        }
		
        _fogWar.MarkObstaclesDirty();

        if (WaitingForLightRooms.Count > 0)
            _grandCandle.Pick(WaitingForLightRooms.Count);
        else
            _eventBus.ToggleMovementUI.RaiseEvent(true);
    }
    
    /// <summary>
    /// Call logic for every tile that can be lighted from specified position with specified intensity 
    /// </summary>
    /// <param name="lightPos">Source grid position</param>
    /// <param name="intensity">Number of tiles lighted in front of source in one direction</param>
    /// <param name="callback">Action to call for lighted grid position</param>
    /// <param name="stopDirectionCondition">Additional condition of light blocking: (last iteration room, current room) => stops iteration</param>
    public void LightCast(Vector2Int lightPos, int intensity, Action<Vector2Int> callback, Func<RoomSO, RoomSO, bool> stopDirectionCondition = null)
    {
        stopDirectionCondition ??= (_, _) => false;

        callback(_mapManager.GridLoop(lightPos));

        int maxHIntensity = Math.Min(intensity + 1, _config.Width); // Light ray cannot overlap self
        int maxVIntensity = Math.Min(intensity + 1, _config.Height); // Light ray cannot overlap self
        bool horizontalMax = false, verticalMax = false;
        bool downBlocked = false, leftBlocked = false, upBlocked = false, rightBlocked = false;
        for (int i = 1; !horizontalMax || !verticalMax; i++)
        {
            horizontalMax = i >= maxHIntensity;
            verticalMax = i >= maxVIntensity;
            
            if (!horizontalMax)
            {
                leftBlocked = leftBlocked || LightCastBlockStep(lightPos, Direction.Left, i, stopDirectionCondition);
                rightBlocked = rightBlocked || LightCastBlockStep(lightPos, Direction.Right, i, stopDirectionCondition);
            }
            if (!verticalMax)
            {
                downBlocked = downBlocked || LightCastBlockStep(lightPos, Direction.Down, i, stopDirectionCondition);
                upBlocked = upBlocked || LightCastBlockStep(lightPos, Direction.Up, i, stopDirectionCondition);
            }
			
            if (!verticalMax && !downBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.down * i));
            if (!horizontalMax && !leftBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.left * i));
            if (!verticalMax && !upBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.up * i));
            if (!horizontalMax && !rightBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.right * i));
        }
    }

    /// <summary>
    /// Calculate one step of light casting
    /// </summary>
    /// <param name="lightPos">Source grid position</param>
    /// <param name="direction">Direction or light ray</param>
    /// <param name="i">Iteration</param>
    /// <param name="stopCondition">Additional condition of light blocking: (last iteration room, current room) => stops iteration</param>
    /// <returns>Does current iteration block ray and is not lighted </returns>
    private bool LightCastBlockStep(Vector2Int lightPos, Direction direction, int i, Func<RoomSO, RoomSO, bool> stopCondition)
    {
        RoomSO lastRoom = _mapManager.GetRoomInPos(lightPos + Connections.ToOffset(direction) * (i - 1));
        RoomSO targetRoom = _mapManager.GetRoomInPos(lightPos + Connections.ToOffset(direction) * i);
        if (stopCondition(lastRoom, targetRoom))
            return true;

        List<Direction> targetConnections = targetRoom != null
            ? targetRoom.Connections
            : new() { Direction.Down, Direction.Left, Direction.Right, Direction.Up };
        
        return lastRoom != null && !Connections.HasConnected(direction, targetConnections, lastRoom.Connections);
    }
}