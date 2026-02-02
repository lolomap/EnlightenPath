using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spawnables.Data;
using UnityEngine;
using Zenject;
namespace Spawnables
{
    [RequireComponent(typeof(LightSource))]
    public class WaxEater : MonoBehaviour, IMonsterTrigger
    {
        public WaxEaterSO Data;
        
        private Vector2Int _gridPos;
        private Vector2Int _lastTriggeredPos = new(-1, -1);
        private LightSource _lightSource;

        [Inject] private MapManager _mapManager;
        [Inject] private LightManager _lightManager;
        [Inject] private Inventory _inventory;
        [Inject] private GrandCandle _grandCandle;

        private void Awake()
        {
            _lightSource = GetComponent<LightSource>();
            
            _gridPos = _mapManager.WorldToGrid(transform.position);
            
            if (Math.Abs(_mapManager.ConnectingSourceGridPos.x - _gridPos.x) <= 1 ||
                Math.Abs(_mapManager.ConnectingSourceGridPos.y - _gridPos.y) <= 1)
                _lastTriggeredPos = _mapManager.ConnectingSourceGridPos;

            //_mapManager.GetRoomInPos(_gridPos).Connections.Clear();
        }

        public void OnDestroy()
        {
            _mapManager.Monsters.Remove(this);
            transform.DOKill();
        }

        public void UpdateTrigger()
        {
            Vector2Int targetPos = _mapManager.ConnectingSourceGridPos;
            if (targetPos.x != _gridPos.x && targetPos.y != _gridPos.y && _lastTriggeredPos is {x: -1, y: -1}) return;

            bool triggered = false;
            List<Candle> toExtinguish = new();
            _lightManager.LightCast(_gridPos, 999,
                visiblePos =>
                {
                    List<Candle> candles = _mapManager.GetRoomObjectInPos(visiblePos).Placed.Where(item => item is Candle).Cast<Candle>().ToList();
                    toExtinguish.AddRange(candles);

                    if (visiblePos != targetPos) return;

                    if (_lastTriggeredPos != targetPos)
                        triggered = true;
                },
                (_, visibleRoom) => visibleRoom == null);
            if (!triggered)
            {
                if (_lastTriggeredPos is not { x: -1, y: -1 })
                {
                    _lastTriggeredPos = new(-1, -1);
                    triggered = true;
                }
            }
            else _lastTriggeredPos = targetPos;

            if (!triggered) return;

            transform.DOJump(transform.position, 1.5f, 1, 1f);
            
            if (_lastTriggeredPos == targetPos)
            {
                _grandCandle.Pick(Data.TilesToEat, true);
                foreach (Candle candle in _inventory.GetItems<Candle>())
                {
                    candle.Toggle(false);
                }
            }
            
            foreach (Candle candle in toExtinguish)
            {
                candle.Toggle(false);
            }

            if (targetPos == _gridPos)
            {
                _lightSource.enabled = false;
            }
        }

        public void OnSpawn(SpawnObjectSO data)
        {
            Data = (WaxEaterSO)data;
            _mapManager.Monsters.Add(this);
        }
        public SpawnObjectSO IData => Data;
    }
}
