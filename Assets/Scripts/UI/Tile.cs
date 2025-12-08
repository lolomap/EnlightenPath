using System;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
namespace UI
{
    public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image Preview;
        public float ShakeDuration;
        public float ShakeStrength;
        public float HoverScale;
        public float HoverScaleTime;
        public RoomSO Content { get; private set; }

        private bool _isHighlighted;
        
        [Inject] private TilesSelector _tilesSelector;

        private void Awake()
        {
            Preview.rectTransform.DOShakeAnchorPos(ShakeDuration, ShakeStrength).SetLoops(-1);
        }

        private void OnDestroy()
        {
            Preview.rectTransform.DOKill();
        }

        public void Init(RoomSO room)
        {
            Content = room;
            Preview.sprite = room.Preview;
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _isHighlighted = isHighlighted;
            
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
