using System;
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
        public List<Direction> Connections = new();

        [Inject] private EventBus _eventBus;

        private void Awake()
        {
            Toggle(true);
        }

        private void OnEnable()
        {
            _eventBus.ToggleMovementUI.EventRaised += Toggle;
        }

        private void OnDisable()
        {
            _eventBus.ToggleMovementUI.EventRaised -= Toggle;
        }

        public void Toggle(bool isEnabled)
        {
            MoveDown.gameObject.SetActive(isEnabled && Connections.Contains(Direction.Down));
            MoveLeft.gameObject.SetActive(isEnabled && Connections.Contains(Direction.Left));
            MoveUp.gameObject.SetActive(isEnabled && Connections.Contains(Direction.Up));
            MoveRight.gameObject.SetActive(isEnabled && Connections.Contains(Direction.Right));
        }
    }
}
