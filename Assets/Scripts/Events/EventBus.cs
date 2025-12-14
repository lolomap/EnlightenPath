using UnityEngine;

namespace Events
{
    //TODO: Move events with one seneder to their class and use DI
    [CreateAssetMenu(fileName = "EventBus", menuName = "Events/EventBus", order = 0)]
    public class EventBus : ScriptableObject
    {
        public readonly GenericEventChannel<LightSource> LightSourceInstantiated = new();
        
        public readonly GenericEventChannel<bool> ToggleMovementUI = new();
        public readonly GenericEventChannel<Vector2Int> ForceMove = new();

        public readonly GenericEventChannel RemoveSelectedTile = new();

        public readonly GenericEventChannel MapIsReady = new();
        public readonly GenericEventChannel<ISlot> ItemPicked = new();
        public readonly GenericEventChannel<ISlot> ItemDropped = new();

        public readonly GenericEventChannel<Color> Unlocked = new();
        public readonly GenericEventChannel Win = new();
        public readonly GenericEventChannel Lose = new();
    }
}