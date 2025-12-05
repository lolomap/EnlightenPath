using Events;
using UnityEngine;
using Zenject;

public class LightSource : MonoBehaviour
{
    public int Intensity;
    private Vector3 _lastPosition;
    
    [Inject] private EventBus _eventBus;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    public void UpdateLight()
    {
        _eventBus.LightChanged.RaiseEvent(new() {Last = _lastPosition, Present = transform.position, Intensity = Intensity});
        _lastPosition = transform.position;
    }
}