using System;
using UnityEngine;
using Zenject;

public class LightSource : MonoBehaviour
{
    public int Intensity;
    
    private Vector3 _lastPosition;
    private bool _lastFlaming;

    [Inject] private MapManager _mapManager;
    [Inject] private LightManager _lightManager;

    private void Awake()
    {
        Tick();
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