using System.Collections.Generic;
using System.Linq;
using Events;
using Spawnables.Data;
using UnityEngine;
using Zenject;
namespace Spawnables
{
    [RequireComponent(typeof(LightSource))]
    public class Candle : HandSlotItem, ISpawnObject
    {
        public List<GameObject> Flame;
        public CandleSO Data;

        private LightSource _lightSource;

        [Inject] private EventBus _eventBus;
        [Inject] private Inventory _inventory;

        private void OnEnable()
        {
            _eventBus.ItemPicked.EventRaised += OnPicked;
        }

        private void OnDisable()
        {
            _eventBus.ItemPicked.EventRaised -= OnPicked;
        }

        private void Awake()
        {
            _lightSource = GetComponent<LightSource>();
        }
        public void OnDestroy() => _eventBus.ItemLost.RaiseEvent(Data);

        private void Start()
        {
            Toggle(Data.IsFlaming);
            _lightSource.Tick();
        }

        public void OnSpawn(SpawnObjectSO data) => Data = data as CandleSO;

        public void Toggle(bool isFlaming)
        {
            Data.IsFlaming = isFlaming;
            _lightSource.enabled = isFlaming;
            foreach (GameObject flame in Flame)
            {
                flame.SetActive(isFlaming);
            }
        }

        private void OnPicked(SlotItem item)
        {
            if (!ReferenceEquals(item, this)) return;
            
            List<Candle> candles = _inventory.GetItems<Candle>();
            if (Data.IsFlaming)
            {
                foreach (Candle candle in candles)
                {
                    candle.Toggle(true);
                }
            }
            else
            {
                if (candles.Any(candle => candle.Data.IsFlaming))
                    Toggle(true);
            }
        }
    }
}
