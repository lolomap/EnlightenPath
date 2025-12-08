using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		public float MaxElements;
		public float MeltDuration;
		
		private float _maxHeightScale;
		
		private void Awake()
		{
			_maxHeightScale = transform.localScale.y;
		}

		public void UpdateHeight(int fuel)
		{
			transform.DOScaleY(fuel * _maxHeightScale / MaxElements, MeltDuration);
		}
	}
}