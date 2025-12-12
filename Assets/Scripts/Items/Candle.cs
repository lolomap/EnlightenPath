using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Items.Data;
using UnityEngine;
using Zenject;

namespace Items
{
    [RequireComponent(typeof(LightSource))]
    public class Candle : MonoBehaviour, ISpawnObject, IHandSlot
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

        private void Start()
        {
            Toggle(Data.IsFlaming);
        }

        public void OnSpawn(SpawnObjectSO data) => Data = data as CandleSO;

        private void Toggle(bool isFlaming)
        {
            Data.IsFlaming = isFlaming;
            _lightSource.enabled = isFlaming;
            foreach (GameObject flame in Flame)
            {
                flame.SetActive(isFlaming);
            }
        }

        private void OnPicked(ISlot item)
        {
            if (!ReferenceEquals(item, this)) return;
            
            List<ISlot> candles = _inventory.GetItems<Candle>();
            if (Data.IsFlaming)
            {
                foreach (ISlot candle in candles)
                {
                    ((Candle)candle).Toggle(true);
                }
            }
            else
            {
                if (candles.Any(candle => ((Candle)candle).Data.IsFlaming))
                    Toggle(true);
            }
        }
    }
}
