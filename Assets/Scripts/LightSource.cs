using UnityEngine;
using Zenject;

public class LightSource : MonoBehaviour
{
    public int Intensity;
    private Vector3 _lastPosition;

    [Inject] private MapManager _mapManager;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    public LightChangedContext UpdateLightPos()
    {
        LightChangedContext result = new()
        {
            Last = _mapManager.WorldToGrid(_lastPosition),
            Present = _mapManager.WorldToGrid(transform.position),
            Intensity = Intensity
        };
        
        _lastPosition = transform.position;
        
        return result;
    }
}