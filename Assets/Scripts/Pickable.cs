using Events;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(SlotItem))]
public class Pickable : MonoBehaviour, IPointerClickHandler
{
    private bool _isPicked;
    
    [Inject] private EventBus _eventBus;
    [Inject] private MapManager _mapManager;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Pick();
    }

    public void Pick()
    {
        if (_isPicked) { Drop(); return; }
        if (_mapManager.ConnectingSourceGridPos != _mapManager.WorldToGrid(transform.position, true)) return;

        _isPicked = true;
        _eventBus.ItemPicked.RaiseEvent(GetComponent<SlotItem>());
    }

    private void Drop()
    {
        Transform pivot = _mapManager.GetFreeSpawnPivot(_mapManager.WorldToGrid(transform.position, true));
        if (pivot == null) return;
        transform.SetParent(pivot, false);
        
        _isPicked = false;
        _eventBus.ItemDropped.RaiseEvent(GetComponent<SlotItem>());
    }
}