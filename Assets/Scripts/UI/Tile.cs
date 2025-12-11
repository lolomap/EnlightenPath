using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image Preview;
        public Color HighlightColor;
        public float ShakeDuration;
        public float ShakeStrength;
        public float HoverScale;
        public float HoverScaleTime;
        public RoomSO Content { get; private set; }

        private bool _isHighlighted;
        private Color _baseColor;
        private Tween _highlightTween;
        
        [Inject] private TilesSelector _tilesSelector;

        private void Awake()
        {
            _baseColor = Preview.color;
            Preview.rectTransform.DOShakeAnchorPos(ShakeDuration, ShakeStrength).SetLoops(-1);
        }

        private void OnDestroy()
        {
            Preview.rectTransform.DOKill();
            _highlightTween.Kill();
        }

        public void Init(RoomSO room)
        {
            Content = room;
            Preview.sprite = room.Preview;
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _isHighlighted = isHighlighted;

            if (_isHighlighted)
            {
                _highlightTween = DOTween.Sequence()
                    .Append(Preview.DOColor(HighlightColor, ShakeDuration))
                    .Append(Preview.DOColor(_baseColor, ShakeDuration))
                    .SetLoops(-1)
                    .Play();
            }
            else
            {
                _highlightTween.Kill(true);
                Preview.color = _baseColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _tilesSelector.Select(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Preview.rectTransform.DOKill(true);
            Preview.rectTransform.DOScale(Vector3.one * HoverScale, HoverScaleTime);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            Preview.rectTransform.DOKill(true);
            Preview.rectTransform.DOScale(Vector3.one, HoverScaleTime);
            Preview.rectTransform.DOShakeAnchorPos(ShakeDuration, ShakeStrength).SetLoops(-1);
        }
    }
}
