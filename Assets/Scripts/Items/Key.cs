using UnityEngine;

namespace Items
{
    public class KeySO : SpawnObjectSO
    {
        public Color Color;
    }
    
    public class Key : MonoBehaviour, ISpawnObject, IHandSlot
    {
        private KeySO _data;
    }
}
