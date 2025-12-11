using Items.Data;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Pickable))]
    public class Key : MonoBehaviour, ISpawnObject, IHandSlot
    {
        private KeySO _data;
    }
}
