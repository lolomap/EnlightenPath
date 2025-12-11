using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using Events;
using UnityEngine;
using Utilities;
using Zenject;

public struct LightChangedContext
{
    public Vector2Int Last;
    public Vector2Int Present;
    public int Intensity;
}

public class LightManager : MonoBehaviour
{
    private bool _isReady;
    private readonly List<LightSource> _lightSources = new();
    private readonly Dictionary<Vector2Int, int> _lightedRooms = new();

    public HashSet<Vector2Int> WaitingForLightRooms { get; } = new();

    [Inject] private EventBus _eventBus;
    [Inject] private MapManager _mapManager;
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
        List<LightChangedContext> changes = _lightSources.Select(lightSource => lightSource.UpdateLightPos()).ToList();

        foreach (LightChangedContext context in changes)
            UpdateDark(context);
        
        foreach (LightChangedContext context in changes)
            UpdateLight(context);

        ApplyLightEffect();
    }

    public void RegisterLightSource(LightSource source)
    {
        _lightSources.Add(source);
        if (_isReady)
            source.UpdateLightPos();
    }

    private void UpdateDark(LightChangedContext context)
    {
        LightCast(context.Last, context.Intensity, (gridPos) =>
        {
            if (_lightedRooms.ContainsKey(gridPos)) _lightedRooms[gridPos]--;
        });
    }

    private void UpdateLight(LightChangedContext context)
    {
        LightCast(context.Present, context.Intensity, (gridPos) =>
        {
            if (_lightedRooms.TryAdd(gridPos, 1))
            {
                if (gridPos != context.Present)
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
		
        _eventBus.FogObstaclesDirty.RaiseEvent();

        if (WaitingForLightRooms.Count > 0)
            _grandCandle.Pick(WaitingForLightRooms.Count);
        else
            _eventBus.ToggleMovementUI.RaiseEvent(true);
    }
    
    private void LightCast(Vector2Int lightPos, int intensity, Action<Vector2Int> callback)
    {
        callback(_mapManager.GridLoop(lightPos));

        bool downBlocked = false, leftBlocked = false, upBlocked = false, rightBlocked = false;
        for (int i = 1; i <= intensity; i++)
        {
            if (!_mapManager.GetRoomInPos(lightPos + Vector2Int.down * (i - 1)).Connections.Contains(Direction.Down))
                downBlocked = true;
            if (!_mapManager.GetRoomInPos(lightPos + Vector2Int.left * (i - 1)).Connections.Contains(Direction.Left))
                leftBlocked = true;
            if (!_mapManager.GetRoomInPos(lightPos + Vector2Int.right * (i - 1)).Connections.Contains(Direction.Right))
                rightBlocked = true;
            if (!_mapManager.GetRoomInPos(lightPos + Vector2Int.up * (i - 1)).Connections.Contains(Direction.Up))
                upBlocked = true;
			
            if (!downBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.down * i));
            if (!leftBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.left * i));
            if (!upBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.up * i));
            if (!rightBlocked)
                callback(_mapManager.GridLoop(lightPos + Vector2Int.right * i));
        }
    }
}