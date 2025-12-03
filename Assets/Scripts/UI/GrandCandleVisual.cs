using System.Collections.Generic;
using Data;
using EditorAttributes;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		public TilesSelector TilesTarget;
		
		private GrandCandle _candle;

		[Inject] private DungeonConfig _config;
		
		private void Awake()
		{
			_candle = new(_config.GrandCandle);
		}

		[Button]
		public void Get(int count, bool destroy = false)
		{
			List<RoomSO> rooms = _candle.Pop(count, true);
			
			if (!destroy && TilesTarget != null)
				TilesTarget.AddTiles(rooms);
		}
	}
}