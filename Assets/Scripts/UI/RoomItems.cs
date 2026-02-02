using System;
using System.Collections.Generic;
using Spawnables.Data;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using Zenject;
namespace UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RotationConstraint))]
    public class RoomItems : MonoBehaviour
    {
        public ItemPreview Prefab;
        
        private Canvas _canvas;
        private RotationConstraint _rotationConstraint;
        private Transform _items;

        [Inject] private WorldUIPivot _worldUIPivot;
        
        private void Awake()
        {
            _items = GetComponentInChildren<HorizontalLayoutGroup>().transform;
            _items.gameObject.SetActive(false);
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main;
            _rotationConstraint = GetComponent<RotationConstraint>();
            _rotationConstraint.SetSources(new(){ new() {sourceTransform = _worldUIPivot.transform, weight = 1f}});
        }

        public void AddItem(GameObject itemObj, SpawnObjectSO itemData)
        {
            if (itemData.Preview == null) return;
            
            if (!_items.gameObject.activeSelf)
                _items.gameObject.SetActive(true);
            
            ItemPreview preview = Instantiate(Prefab.gameObject, Vector3.zero, Quaternion.identity, _items).GetComponent<ItemPreview>();
            preview.transform.localRotation = Quaternion.identity;
            preview.transform.localPosition = new(preview.transform.localPosition.x, preview.transform.localPosition.y, 0f);
            preview.Preview.sprite = itemData.Preview;
            preview.Target = itemObj;
        }

        public void RemoveItem(GameObject itemObj)
        {
            foreach (Transform item in _items)
            {
                ItemPreview itemPreview = item.GetComponent<ItemPreview>();
                if (ReferenceEquals(itemPreview.Target, itemObj))
                {
                    RemoveItem(itemPreview);
                    return;
                }
            }
        }
        
        private void RemoveItem(Component item)
        {
            if (_items.childCount - 1 == 0)
                _items.gameObject.SetActive(false);

            Destroy(item.gameObject);
        }
    }
}
