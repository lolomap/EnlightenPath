using System;
using System.Collections.Generic;
using Data;
using DI;
using Events;
using UnityEngine;
using Zenject;
namespace UI
{
    public class TilesSelector : MonoBehaviour
    {
        public Tile TilePrefab;

        private Tile _selected;
        private readonly List<Tile> _tiles = new();

        [Inject] private EventBus _eventBus;
        [Inject] private PreviewManager _previewManager;

        private void OnEnable()
        {
            _eventBus.RemoveSelectedTile.EventRaised += OnRemoveSelected;
        }

        private void OnDisable()
        {
            _eventBus.RemoveSelectedTile.EventRaised -= OnRemoveSelected;
        }

        public void AddTiles(List<RoomSO> rooms)
        {
            _eventBus.ToggleMovementUI.RaiseEvent(false);
         
            foreach (RoomSO room in rooms)
            {
                Tile tile = DIGlobal.Instantiate(TilePrefab.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Tile>();
                tile.Init(room);
                _tiles.Add(tile);
            }
        }

        public void Select(Tile selected)
        {
            _selected = selected;
            foreach (Tile tile in _tiles)
            {
                tile.SetHighlighted(false);
            }
            
            if (_selected == null) return;

            selected.SetHighlighted(true);
            _previewManager.PreviewRoom(selected.Content, Quaternion.Euler(0, 90 * (int)_selected.Content.Direction, 0));
        }

        private void Remove(Tile removed)
        {
            _tiles.Remove(removed);
            Destroy(removed.gameObject);

            if (_tiles.Count == 0)
                _eventBus.ToggleMovementUI.RaiseEvent(true);
        }

        private void OnRemoveSelected()
        {
            Remove(_selected);
        }
    }
}
