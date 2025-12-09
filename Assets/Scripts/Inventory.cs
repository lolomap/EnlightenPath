using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using Zenject;

public interface ISlot {}
public interface IHandSlot : ISlot {}
public interface IBeltSlot : ISlot {}

public class Inventory : MonoBehaviour
{
    public Transform LeftHandPivot;
    public Transform RightHandPivot;
    public Transform BeltPivot;
    
    private IHandSlot _leftHand;
    private IHandSlot _rightHand;
    private IBeltSlot _belt;
    
    private IEnumerable<ISlot> AllSlots => new ISlot[]
    {
        _leftHand,
        _rightHand,
        _belt
    };

    [Inject] private EventBus _eventBus;

    private void OnEnable()
    {
        _eventBus.ItemPicked.EventRaised += OnPicked;
    }

    public IEnumerable<LightSource> GetLightSources()
    {
        return AllSlots
            .Where(slot => slot != null)
            .Select(slot => ((MonoBehaviour)slot).GetComponent<LightSource>())
            .Where(lightSource => lightSource != null);
    }
    
    private void OnPicked(ISlot payload)
    {
        MonoBehaviour picked = (MonoBehaviour)payload;
        
        switch (payload)
        {
            case IHandSlot slot:
            {
                if (_rightHand == null)
                {
                    _rightHand = slot;
                    picked.transform.SetParent(RightHandPivot);
                    picked.transform.localPosition = Vector3.zero;
                    picked.transform.localRotation = Quaternion.identity;
                }
                else if (_leftHand == null)
                {
                    _leftHand = slot;
                    picked.transform.SetParent(LeftHandPivot);
                    picked.transform.localPosition = Vector3.zero;
                    picked.transform.localRotation = Quaternion.identity;
                }
                
                break;
            }
            case IBeltSlot:
                break;
        }
    }
}