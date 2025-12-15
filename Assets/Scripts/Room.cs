using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Events;
using FischlWorks_FogWar;
using Spawnables.Data;
using UnityEngine;
using Utilities;
using Zenject;

public class Room : MonoBehaviour
{
    public LayerMask GroundLayer;
    public SerializedDictionary<SpawnLocation, Transform> SpawnPivots;

    public readonly List<SlotItem> Placed = new();
    
    private Vector2Int _gridPos;

    [Inject] private EventBus _eventBus;
    [Inject] private MapManager _mapManager;
    [Inject] private csFogWar _fogWar;

    private void OnEnable()
    {
        if (_eventBus == null) return;
        _eventBus.ItemPicked.EventRaised += OnPicked;
        _eventBus.ItemDropped.EventRaised += OnDropped;
    }

    private void OnDisable()
    {
        if (_eventBus == null) return;
        _eventBus.ItemPicked.EventRaised -= OnPicked;
        _eventBus.ItemDropped.EventRaised -= OnDropped;
    }

    private void Awake()
    {
        if (_mapManager != null)
            _gridPos = _mapManager.WorldToGrid(transform.position);
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && _fogWar != null)
            _fogWar.MarkObstaclesDirty();
    }

    public void BreakGround()
    {
        foreach (GameObject groundObj in GameObjectManipulator.FilterByLayer(gameObject, GroundLayer))
        {
            groundObj.SetActive(false);
        }
    }

    private void OnPicked(SlotItem item)
    {
        Vector2Int pickedPos = _mapManager.WorldToGrid(item.transform.position);
        if (pickedPos != _gridPos) return;

        Placed.Remove(item);
    }

    private void OnDropped(SlotItem item)
    {
        Vector2Int pickedPos = _mapManager.WorldToGrid(item.transform.position);
        if (pickedPos != _gridPos) return;
        
        Placed.Add(item);
    }
}