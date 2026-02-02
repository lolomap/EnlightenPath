using DG.Tweening;
using Events;
using Spawnables.Data;
using UnityEngine;
using Zenject;
namespace Spawnables
{
    public class Keyhole : MonoBehaviour, ISpawnObject
    {
        public MeshRenderer Mesh;
        public float OpenDuration;
        public KeyholeSO Data;
        public bool IsUsed;
        
        [Inject] private EventBus _eventBus;
        [Inject] private MapManager _mapManager;

        private void OnEnable()
        {
            _eventBus.ItemDropped.EventRaised += OnItemDropped;
        }

        private void OnDisable()
        {
            _eventBus.ItemDropped.EventRaised -= OnItemDropped;
        }

        private void Awake()
        {
            Mesh.material = new(Mesh.material);
        }

        public void OnDestroy()
        {
            if (IsUsed)
                _eventBus.ItemLost.RaiseEvent(Data);
        }

        private void Start()
        {
            Mesh.material.color = Data.Color;
        }
        
        private void OnItemDropped(SlotItem item)
        {
            if (_mapManager.ConnectingSourceGridPos != _mapManager.WorldToGrid(transform.position, true)) return;
            Key key = item.GetComponent<Key>();
            if (key == null) return;
            if (key.Data.Color != Data.Color) return;

            key.IsUsed = true;
            IsUsed = true;
            Destroy(key.gameObject);
            _eventBus.Unlocked.RaiseEvent(Data.Color);
            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, OpenDuration))
                .AppendCallback(() =>
                {
                    Destroy(gameObject);
                })
                .Play();
        }

        public void OnSpawn(SpawnObjectSO data) => Data = data as KeyholeSO;
        public SpawnObjectSO IData => Data;
    }
}
