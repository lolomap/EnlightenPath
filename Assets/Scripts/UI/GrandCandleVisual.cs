using System.Collections.Generic;
using Data;
using DG.Tweening;
using EditorAttributes;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		public TilesSelector TilesTarget;
		public float MeltDuration;
		
		private GrandCandle _candle;
		private float _maxHeightScale;
		private float _maxHeightElements;

		[Inject] private DungeonConfig _config;
		
		private void Awake()
		{
			_candle = new(_config.GrandCandle);
			_maxHeightScale = transform.localScale.y;
			_maxHeightElements = _candle.Fuel;
		}

		private void UpdateHeight()
		{
			transform.DOScaleY(_candle.Fuel * _maxHeightScale / _maxHeightElements, MeltDuration);
		}

		[Button]
		public void Get(int count, bool destroy = false)
		{
			List<RoomSO> rooms = _candle.Pop(count, true);
			UpdateHeight();
			
			if (!destroy && TilesTarget != null)
				TilesTarget.AddTiles(rooms);
		}
	}
}