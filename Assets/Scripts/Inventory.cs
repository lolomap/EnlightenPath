using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using Zenject;

public class SlotItem : MonoBehaviour {}
public class HandSlotItem : SlotItem {}
public class BeltSlotItem : SlotItem {}

public class Inventory : MonoBehaviour
{
    public Transform LeftHandPivot;
    public Transform RightHandPivot;
    public Transform BeltPivot;
    
    private HandSlotItem _leftHand;
    private HandSlotItem _rightHand;
    private BeltSlotItem _belt;
    
    private SlotItem[] AllSlots => new SlotItem[]
    {
        _leftHand,
        _rightHand,
        _belt
    };

    [Inject] private EventBus _eventBus;

    private void OnEnable()
    {
        _eventBus.ItemPicked.EventRaised += OnPicked;
        _eventBus.ItemDropped.EventRaised += OnDropped;
    }

    private void OnDisable()
    {
        _eventBus.ItemPicked.EventRaised -= OnPicked;
        _eventBus.ItemDropped.EventRaised -= OnDropped;
    }

    public List<T> GetItems<T>() where T : SlotItem
    {
        return AllSlots.Where(slot => slot is T).Cast<T>().ToList();
    }

    public IEnumerable<LightSource> GetLightSources()
    {
        return AllSlots
            .Where(slot => slot != null)
            .Select(slot => (slot).GetComponent<LightSource>())
            .Where(lightSource => lightSource != null);
    }
    
    private void OnPicked(SlotItem item)
    {
        switch (item)
        {
            case HandSlotItem slot:
            {
                if (_rightHand == null)
                {
                    _rightHand = slot;
                    item.transform.SetParent(RightHandPivot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                }
                else if (_leftHand == null)
                {
                    _leftHand = slot;
                    item.transform.SetParent(LeftHandPivot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                }
                
                break;
            }
            case BeltSlotItem:
                break;
        }
    }

    private void OnDropped(SlotItem item)
    {
        switch (item)
        {
            case HandSlotItem:
            {
                if (_rightHand == item)
                    _rightHand = null;
                else if (_leftHand == item)
                    _leftHand = null;
                
                break;
            }
            case BeltSlotItem:
            {
                _belt = null;
                break;
            }
        }
    }
}