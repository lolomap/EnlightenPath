using System;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using EditorAttributes;
using Events;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		public float MeltDuration;
		
		private GrandCandle _candle;
		private float _maxHeightScale;
		private float _maxHeightElements;

		[Inject] private DungeonConfig _config;
		[Inject] private EventBus _eventBus;
		[Inject] private TilesSelector _tilesSelector;
		
		private void Awake()
		{
			_candle = new(_config.GrandCandle);
			_maxHeightScale = transform.localScale.y;
			_maxHeightElements = _candle.Fuel;
		}

		private void OnEnable()
		{
			_eventBus.RequestTiles.EventRaised += OnRequestTiles;
			_eventBus.RequestTiles.EventRaised += OnDestroyTiles;
		}

		private void OnDisable()
		{
			_eventBus.RequestTiles.EventRaised -= OnRequestTiles;
			_eventBus.RequestTiles.EventRaised -= OnDestroyTiles;
		}

		private void UpdateHeight()
		{
			transform.DOScaleY(_candle.Fuel * _maxHeightScale / _maxHeightElements, MeltDuration);
		}

		private void OnRequestTiles(int count) => Get(count);
		private void OnDestroyTiles(int count) => Get(count, true);

		[Button]
		private void Get(int count, bool destroy = false)
		{
			List<RoomSO> rooms = _candle.Pop(count, true);
			UpdateHeight();
			
			if (!destroy && _tilesSelector != null)
				_tilesSelector.AddTiles(rooms);
		}
	}
}