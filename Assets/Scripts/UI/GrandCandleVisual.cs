using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class GrandCandleVisual : MonoBehaviour
	{
		public Transform ScaleTarget;
		public Transform MoveTarget;
		public float MaxElements;
		public float MeltDuration;
		
		private float _maxHeightScale;
		private float _maxHeightPos;
		
		private void Awake()
		{
			_maxHeightScale = ScaleTarget.localScale.y;
			_maxHeightPos = MoveTarget.localPosition.y;
		}

		public void UpdateHeight(int fuel)
		{
			ScaleTarget.DOScaleY(fuel * _maxHeightScale / MaxElements, MeltDuration);
			MoveTarget.DOLocalMoveY(fuel * _maxHeightPos / MaxElements, MeltDuration);
		}
	}
}