using Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace UI
{
    public class TileTrash : MonoBehaviour
    {
        private Image _tileSprite;
        // QUEUE
        
        [Inject] private EventBus _eventBus;

        private void Show(RoomSO lostTile)
        {
            
        }
    }
}
