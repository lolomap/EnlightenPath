using AYellowpaper.SerializedCollections;
using FischlWorks_FogWar;
using Items.Data;
using UnityEngine;
using Zenject;

public class Room : MonoBehaviour
{
    public SerializedDictionary<SpawnLocation, Transform> SpawnPivots;

    [Inject] private csFogWar _fogWar;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && _fogWar != null)
            _fogWar.MarkObstaclesDirty();
    }
}