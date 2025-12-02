using Data;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		private GrandCandle _candle;

		[Inject] private DungeonConfig _config;
		
		private void Awake()
		{
			_candle = new(_config.GrandCandle);
		}
	}
}