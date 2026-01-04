using Spawnables.Data;
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
        public readonly GenericEventChannel<SlotItem> ItemPicked = new();
        public readonly GenericEventChannel<SlotItem> ItemDropped = new();
        //TODO: visualise lost items in ui
        public readonly GenericEventChannel<SpawnObjectSO> ItemLost = new();

        public readonly GenericEventChannel<Color> Unlocked = new();
        public readonly GenericEventChannel Win = new();
        public readonly GenericEventChannel GameOver = new();
    }
}