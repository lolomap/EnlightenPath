using Spawnables.Data;
using UnityEngine;
namespace Spawnables
{
    public class Key : HandSlotItem, ISpawnObject 
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
