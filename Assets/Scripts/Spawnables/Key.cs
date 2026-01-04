using Events;
using JetBrains.Annotations;
using Spawnables.Data;
using UnityEngine;
using Zenject;
namespace Spawnables
{
    public class Key : HandSlotItem, ISpawnObject 
    {
        public MeshRenderer Mesh;
        public KeySO Data;
        public bool IsUsed;
        
        [Inject] private EventBus _eventBus;

        private void Awake()
        {
            Mesh.material = new(Mesh.material);
        }
        
        public void OnDestroy()
        {
            if (!IsUsed) _eventBus.ItemLost.RaiseEvent(Data);
        }

        private void Start()
        {
            Mesh.material.color = Data.Color;
        }

        public void OnSpawn(SpawnObjectSO data) => Data = data as KeySO;
    }
}
