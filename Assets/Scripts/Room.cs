using Events;
using UnityEngine;
using Zenject;

public class Room : MonoBehaviour
{
    [Inject] private EventBus _eventBus;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && _eventBus != null)
            _eventBus.FogObstaclesDirty.RaiseEvent();
    }
}