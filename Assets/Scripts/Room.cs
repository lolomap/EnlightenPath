using AYellowpaper.SerializedCollections;
using Events;
using Items.Data;
using UnityEngine;
using Zenject;

public class Room : MonoBehaviour
{
    public SerializedDictionary<SpawnLocation, Transform> SpawnPivots;
    
    [Inject] private EventBus _eventBus;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && _eventBus != null)
            _eventBus.FogObstaclesDirty.RaiseEvent();
    }
}