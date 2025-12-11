using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "EventBus", menuName = "Events/EventBus", order = 0)]
    public class EventBus : ScriptableObject
    {
        public readonly GenericEventChannel FogIsReady = new();
        public readonly GenericEventChannel FogObstaclesDirty = new();
        
        public readonly GenericEventChannel<LightSource> LightSourceInstantiated = new();
        
        public readonly GenericEventChannel<bool> ToggleMovementUI = new();
        public readonly GenericEventChannel<Vector2Int> ForceMove = new();

        public readonly GenericEventChannel RemoveSelectedTile = new();

        public readonly GenericEventChannel MapIsReady = new();
        public readonly GenericEventChannel<ISlot> ItemPicked = new();
        public readonly GenericEventChannel<ISlot> ItemDropped = new();
    }
}