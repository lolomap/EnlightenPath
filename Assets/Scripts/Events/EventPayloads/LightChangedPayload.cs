using UnityEngine;

namespace Events.EventPayloads
{
    public struct LightChangedPayload
    {
        public Vector3 Last;
        public Vector3 Present;
        public int Intensity;
    }
}
