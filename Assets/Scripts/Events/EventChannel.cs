using System;
using UnityEngine;
namespace Events
{
    public class GenericEventChannel<T>
    {
        public delegate void EventRaisedHandler(T payload);

        public event EventRaisedHandler EventRaised;
        
        public void RaiseEvent(T payload)
        {
            EventRaised?.Invoke(payload);
        }
    }
    
    public class GenericEventChannel
    {
        public event Action EventRaised;
        
        public void RaiseEvent()
        {
            EventRaised?.Invoke();
        }
    }
}
