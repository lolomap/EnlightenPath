using Events.EventPayloads;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "EventBus", menuName = "Events/EventBus", order = 0)]
    public class EventBus : ScriptableObject
    {
        public readonly GenericEventChannel FogObstaclesDirty = new();
        public readonly GenericEventChannel<LightChangedPayload> LightChanged = new();
        public readonly GenericEventChannel<bool> TogglePlayerMovement = new();
        public readonly GenericEventChannel<Vector3> PreviewMoved = new();
        public readonly GenericEventChannel<Vector3> PreviewRotated = new();
        public readonly GenericEventChannel<Vector3> SubmitPlacing = new();
    }
}