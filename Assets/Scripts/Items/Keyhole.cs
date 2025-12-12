using System;
using DG.Tweening;
using Events;
using Items.Data;
using UnityEngine;
using Zenject;
namespace Items
{
    public class Keyhole : MonoBehaviour, ISpawnObject
    {
        public MeshRenderer Mesh;
        public float OpenDuration;
        public KeyholeSO Data;

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

        private void Start()
        {
            Mesh.material.color = Data.Color;
        }
        
        private void OnItemDropped(ISlot item)
        {
            if (_mapManager.ConnectingSourceGridPos != _mapManager.WorldToGrid(transform.position, true)) return;
            Key key = ((MonoBehaviour)item).GetComponent<Key>();
            if (key == null) return;
            if (key.Data.Color != Data.Color) return;
            
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
    }
}
