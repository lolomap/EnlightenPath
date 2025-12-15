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
                if (gridPos != context.PresentPos)
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
    
    public void LightCast(Vector2Int lightPos, int intensity, Action<Vector2Int> callback, Func<RoomSO, bool> stopDirectionCondition = null)
    {
        stopDirectionCondition ??= _ => false;

        callback(_mapManager.GridLoop(lightPos));

        bool downBlocked = false, leftBlocked = false, upBlocked = false, rightBlocked = false;
        int maxIntensity = Math.Min(intensity + 1, Math.Min(_config.Width, _config.Height)); // Light ray cannot overlap self
        for (int i = 1; i < maxIntensity; i++)
        {
            RoomSO downRoom = _mapManager.GetRoomInPos(lightPos + Vector2Int.down * (i - 1));
            if (stopDirectionCondition(downRoom) || downRoom != null && !downRoom.Connections.Contains(Direction.Down))
                downBlocked = true;
            RoomSO leftRoom = _mapManager.GetRoomInPos(lightPos + Vector2Int.left * (i - 1));
            if (stopDirectionCondition(leftRoom) || leftRoom != null && !leftRoom.Connections.Contains(Direction.Left))
                leftBlocked = true;
            RoomSO upRoom = _mapManager.GetRoomInPos(lightPos + Vector2Int.right * (i - 1));
            if (stopDirectionCondition(upRoom) || upRoom != null && !upRoom.Connections.Contains(Direction.Right))
                rightBlocked = true;
            RoomSO rightRoom = _mapManager.GetRoomInPos(lightPos + Vector2Int.up * (i - 1));
            if (stopDirectionCondition(rightRoom) || rightRoom != null && !rightRoom.Connections.Contains(Direction.Up))
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