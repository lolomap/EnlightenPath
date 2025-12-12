using Items.Data;
using UnityEngine;

namespace Items
{
    public class Key : MonoBehaviour, ISpawnObject, IHandSlot
    {
        public MeshRenderer Mesh;
        public KeySO Data;

        private void Awake()
        {
            Mesh.material = new(Mesh.material);
        }

        private void Start()
        {
            Mesh.material.color = Data.Color;
        }

        public void OnSpawn(SpawnObjectSO data) => Data = data as KeySO;
    }
}
