using Events;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(ISlot))]
public class Pickable : MonoBehaviour, IPointerClickHandler
{
    [Inject] private EventBus _eventBus;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _eventBus.ItemPicked.RaiseEvent(GetComponent<ISlot>());
    }
}