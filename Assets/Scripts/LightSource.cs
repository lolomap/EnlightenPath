using System;
using Events;
using FischlWorks_FogWar;
using UnityEngine;
using Zenject;
using Logger = Utilities.Logger;

public class LightSource : MonoBehaviour
{
    public int Intensity;
    
    private Vector3 _lastPosition;
    private bool _lastFlaming;
    private int _fogRevealer;

    [Inject] private EventBus _eventBus;
    [Inject] private MapManager _mapManager;
    [Inject] private LightManager _lightManager;
    [Inject] private csFogWar _fog;

    private void Awake()
    {
        Tick();
    }

    private void OnEnable()
    {
        _fogRevealer = _fog.AddFogRevealer(new(transform, (Intensity + 1) * 10, true));
        _eventBus.LightSourceToggled.RaiseEvent((this, true));
    }

    private void OnDisable()
    {
        _fog.RemoveFogRevealer(_fogRevealer);
        _eventBus.LightSourceToggled.RaiseEvent((this, false));
    }

    public void Tick()
    {
        _lastPosition = transform.position;
        _lastFlaming = enabled;
    }

    private void OnDestroy()
    {
        _lightManager.UnregisterLightSource(this);
    }

    public LightChangedContext UpdateLightPos()
    {
        LightChangedContext result = new()
        {
            LastPos = _mapManager.WorldToGrid(_lastPosition),
            PresentPos = _mapManager.WorldToGrid(transform.position),
            Intensity = Intensity,
            LastFlaming = _lastFlaming,
            PresentFlaming = enabled
        };
        
        _lastPosition = transform.position;
        _lastFlaming = enabled;
        
        return result;
    }
}