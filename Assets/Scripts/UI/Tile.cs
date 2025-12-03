using System;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace UI
{
    public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image Preview;
        public float ShakeDuration;
        public float ShakeStrength;
        public float HoverScale;
        [FormerlySerializedAs("HoverTime")]
        public float HoverScaleTime;
        public RoomSO Content { get; private set; }

        private void Awake()
        {
            Preview.rectTransform.DOShakeAnchorPos(ShakeDuration, ShakeStrength).SetLoops(-1);
        }

        public void Init(RoomSO room)
        {
            Content = room;
            Preview.sprite = room.Preview;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
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
