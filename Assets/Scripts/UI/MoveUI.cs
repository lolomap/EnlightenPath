using System.Collections.Generic;
using System.Linq;
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
            if (isEnabled)
            {
                Vector2Int gridPos = _mapManager.WorldToGrid(transform.position);
                _connections = _mapManager.GetRoomInPos(gridPos).Connections;
                _connections = _connections.Where(connection =>
                {
                    List<Direction> neighbourConnections = _mapManager.GetRoomInPos(gridPos + Connections.ToOffset(connection))?
                        .Connections ?? new() { Direction.Down, Direction.Left, Direction.Up, Direction.Right };
                    return Connections.HasConnected(connection, neighbourConnections, _connections);
                }).ToList();
            }
            
            MoveDown.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Down));
            MoveLeft.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Left));
            MoveUp.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Up));
            MoveRight.gameObject.SetActive(isEnabled && _connections.Contains(Direction.Right));
        }
    }
}
