using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
namespace UI
{
    [RequireComponent(typeof(Image))]
    public class ItemPreview : MonoBehaviour, IPointerClickHandler
    {
        public Image Preview;
        public GameObject Target;

        private void Awake()
        {
            Preview = GetComponent<Image>();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Target.HasComponent(out Pickable pickable))
            {
                pickable.Pick();
            }
        }
    }
}
