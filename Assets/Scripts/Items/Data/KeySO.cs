using UnityEngine;

namespace Items.Data
{
    [CreateAssetMenu(fileName = "Key", menuName = "Game/Item/Key", order = 0)]
    public class KeySO : SpawnObjectSO
    {
        public Color Color;
    }
}
