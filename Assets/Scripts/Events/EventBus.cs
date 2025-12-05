using Events.EventPayloads;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "EventBus", menuName = "Events/EventBus", order = 0)]
    public class EventBus : ScriptableObject
    {
        public readonly GenericEventChannel FogIsReady = new();
        public readonly GenericEventChannel FogObstaclesDirty = new();
        public readonly GenericEventChannel<LightChangedPayload> LightChanged = new();
        public readonly GenericEventChannel<int> RequestTiles = new();
        public readonly GenericEventChannel<int> DestroyTiles = new();
        public readonly GenericEventChannel<bool> ToggleMovementUI = new();
        public readonly GenericEventChannel<Vector2Int> MovedToDark = new();
        public readonly GenericEventChannel<Vector3> PreviewMoved = new();
        public readonly GenericEventChannel<Vector3> PreviewRotated = new();
        public readonly GenericEventChannel<Vector3> SubmitPlacing = new();
    }
}