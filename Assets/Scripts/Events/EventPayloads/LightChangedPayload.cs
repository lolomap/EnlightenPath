using System.Collections.Generic;
using UnityEngine;

namespace Events.EventPayloads
{
    public struct LightChangedPayload
    {
        public List<Vector2Int> Darked;
        public List<Vector2Int> Lighted;
    }
}
