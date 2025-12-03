using System.Collections.Generic;
using Data;
using UnityEngine;
namespace UI
{
    public class TilesSelector : MonoBehaviour
    {
        public Tile TilePrefab;
        
        public void AddTiles(List<RoomSO> rooms)
        {
            foreach (RoomSO room in rooms)
            {
                Tile tile = Instantiate(TilePrefab, transform);
                tile.Init(room);
            }
        }
    }
}
