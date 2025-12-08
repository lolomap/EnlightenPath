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

        private readonly List<Tile> _tiles = new();

        public Tile Selected { get; private set; }

        [Inject] private EventBus _eventBus;
        [Inject] private PreviewManager _previewManager;

        public void AddTiles(List<RoomSO> rooms)
        {
            foreach (RoomSO room in rooms)
            {
                Tile tile = DIGlobal.Instantiate(TilePrefab.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Tile>();
                tile.Init(room);
                _tiles.Add(tile);
            }
        }

        public void Select(Tile selected)
        {
            Selected = selected;
            
            foreach (Tile tile in _tiles)
            {
                tile.SetHighlighted(false);
            }
            
            if (selected == null) return;

            selected.SetHighlighted(true);
            _previewManager.Preview(selected.Content.Prefab.gameObject, Quaternion.Euler(0, 90 * (int)Selected.Content.Direction, 0));
        }

        public void Remove(Tile removed)
        {
            _tiles.Remove(removed);
            Destroy(removed.gameObject);

            if (_tiles.Count == 0)
                _eventBus.ToggleMovementUI.RaiseEvent(true);
        }
    }
}
