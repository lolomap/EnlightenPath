using System.Collections.Generic;
using Data;
using UnityEngine;
namespace UI
{
    public class TilesSelector : MonoBehaviour
    {
        public Tile TilePrefab;
        public PreviewManager Preview;

        private readonly List<Tile> _tiles = new();

        public Tile Selected { get; private set; }

        public void AddTiles(List<RoomSO> rooms)
        {
            foreach (RoomSO room in rooms)
            {
                Tile tile = Instantiate(TilePrefab, transform);
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
            Preview.Preview(selected.Content.Prefab);
        }

        public void Remove(Tile removed)
        {
            _tiles.Remove(removed);
            Destroy(removed.gameObject);
        }
    }
}
