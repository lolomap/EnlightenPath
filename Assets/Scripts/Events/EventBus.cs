using System;
using System.Reflection;
using Spawnables.Data;
using UnityEngine;

namespace Events
{
    //TODO: Move events with one seneder to their class and use DI
    [CreateAssetMenu(fileName = "EventBus", menuName = "Events/EventBus", order = 0)]
    public class EventBus : ScriptableObject
    {
        public readonly GenericEventChannel<LightSource> LightSourceInstantiated = new(nameof(LightSourceInstantiated));
        public readonly GenericEventChannel<(LightSource, bool)> LightSourceToggled = new(nameof(LightSourceToggled));
        
        public readonly GenericEventChannel<bool> ToggleMovementUI = new(nameof(ToggleMovementUI));
        public readonly GenericEventChannel<Vector2Int> ForceMove = new(nameof(ForceMove));

        public readonly GenericEventChannel RemoveSelectedTile = new(nameof(RemoveSelectedTile));

        public readonly GenericEventChannel MapIsReady = new(nameof(MapIsReady));
        public readonly GenericEventChannel<SlotItem> ItemPicked = new(nameof(ItemPicked));
        public readonly GenericEventChannel<SlotItem> ItemDropped = new(nameof(ItemDropped));
        //TODO: visualise lost items in ui
        public readonly GenericEventChannel<SpawnObjectSO> ItemLost = new(nameof(ItemLost));

        public readonly GenericEventChannel<Color> Unlocked = new(nameof(Unlocked));
        public readonly GenericEventChannel Win = new(nameof(Win));
        public readonly GenericEventChannel GameOver = new(nameof(GameOver));
        
        public void SubscribeAll(EventChannel.EventRaisedRawHandler callback)
        {
            foreach (FieldInfo gameEvent in GetType().GetFields())
            {
                EventChannel eventObj = (EventChannel)gameEvent.GetValue(this);
                eventObj.EventRaisedRaw += callback;
            }
        }
        
        public void UnsubscribeAll(EventChannel.EventRaisedRawHandler callback)
        {
            foreach (FieldInfo gameEvent in GetType().GetFields())
            {
                EventChannel eventObj = (EventChannel)gameEvent.GetValue(this);
                eventObj.EventRaisedRaw -= callback;
            }
        }
    }
}