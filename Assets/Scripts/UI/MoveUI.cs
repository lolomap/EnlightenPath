using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;
namespace UI
{
    public class MoveUI : MonoBehaviour
    {
        public Button MoveDown;
        public Button MoveLeft;
        public Button MoveUp;
        public Button MoveRight;
        
        private List<Direction> _connections = new();

        [Inject] private EventBus _eventBus;
        [Inject] private MapManager _mapManager;

        private void OnEnable()
        {
            _eventBus.ToggleMovementUI.EventRaised += Toggle;
        }

        private void OnDisable()
        {
            _eventBus.ToggleMovementUI.EventRaised -= Toggle;
        }

        private void Toggle(bool isEnabled)
        {
            //TODO: FIX CONNECTIONS
            if (isEnabled)
                _connections = _mapManager.GetRoomInPos(_mapManager.WorldToGrid(transform.position)).Connections;
            
            MoveDown.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Down));
            MoveLeft.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Left));
            MoveUp.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Up));
            MoveRight.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Right));
        }
    }
}
