using DI;
using Events;
using FischlWorks_FogWar;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Zenject;

public class PreviewManager : MonoBehaviour
{
    public Camera TargetCamera;
    public Color PreviewTint;
    public Color PreviewBlockTint;
    public float PreviewHeight;
    public LayerMask PlaneLayer;
    public int RenderLayer;
    
    private GameObject _previewObject;
    private int _previewFogRevealer;
    private bool _isBlocked;
    private bool _isRoom;
    private RoomSO _previewRoom;
    private Room _cachedRoomObject;

    [Inject] private EventBus _eventBus;
    [Inject] private MapManager _mapManager;
    [Inject] private csFogWar _fogWar;
    [Inject] private LightManager _lightManager;

    private void Update()
    {
        bool moved = false;
        if (_previewObject != null)
            moved = UpdatePosition();
        if (moved && _isRoom)
            ProcessRoom();
    }

    public void PreviewRoom(RoomSO room, Quaternion rotation)
    {
        _isRoom = true;
        _previewRoom = room;
        Preview(room.Prefab.gameObject, rotation);
    }

    private void Preview(GameObject prefab, Quaternion rotation)
    {
        if (_previewObject != null)
            DropPreview();
        
        SetupPreview(prefab, rotation);
    }

    private void DropPreview()
    {
        Destroy(_previewObject);
        _isRoom = false;
        _previewObject = null;
        _previewRoom = null;
    }

    private void SetBlocked(bool isBlocked)
    {
        _isBlocked = isBlocked;
        _previewObject.SetTint(isBlocked ? PreviewBlockTint : PreviewTint);
    }

    private void SetupPreview(GameObject prefab, Quaternion rotation)
    {
        _previewObject = DIGlobal.Instantiate(prefab, Vector3.zero, rotation, transform);
        
        _previewObject.layer = RenderLayer;
        _previewObject.SetTint(PreviewTint);
        _previewObject.ToggleCollision(false);
        UpdatePosition();
    }

    private bool UpdatePosition()
    {
        Ray ray = TargetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, PlaneLayer)) return false;
        Vector3 newPos = hit.point;
        
        newPos.y = PreviewHeight;
        if (_mapManager.CellSize.x > 0)
            newPos.x = Mathf.Round((newPos.x + _mapManager.CellSize.x * 0.5f) / _mapManager.CellSize.x)
                * _mapManager.CellSize.x
                - _mapManager.CellSize.x * 0.5f;
        if (_mapManager.CellSize.y > 0)
            newPos.z = Mathf.Round((newPos.z + _mapManager.CellSize.y * 0.5f) / _mapManager.CellSize.y)
                * _mapManager.CellSize.y
                - _mapManager.CellSize.y * 0.5f;

        if (newPos == _previewObject.transform.position) return false;
        
        _previewObject.transform.position = newPos;
        
        return true;
    }

    private void ProcessRoom()
    {
        Vector2Int gridPos = _mapManager.WorldToGrid(_previewObject.transform.position);
        Vector2Int gridPosLooped = _mapManager.GridLoop(gridPos);

        bool isValid = _lightManager.WaitingForLightRooms.Contains(gridPosLooped) &&
            _mapManager.IsValidPlace(gridPos, _previewRoom.Connections, _mapManager.ConnectingSourceGridPos);
        SetBlocked(!isValid);

        if (_cachedRoomObject != null)
            _cachedRoomObject.gameObject.ToggleCollision(true);
        _cachedRoomObject = _mapManager.GetRoomObjectInPos(gridPosLooped);
        if (_cachedRoomObject != null)
            _cachedRoomObject.gameObject.ToggleCollision(false);
        _fogWar.MarkObstaclesDirty();
    }
    
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (_previewObject == null) return;
        DropPreview();
    }
    
    public void OnClick(InputAction.CallbackContext context)
    {
        if (_previewObject == null) return;
        if (_isBlocked) return;

        if (_isRoom)
        {
            Vector2Int gridPos = _mapManager.WorldToGrid(_previewObject.transform.position, true);
		
            _mapManager.PlaceRoom(_previewRoom, gridPos);
		
            _eventBus.RemoveSelectedTile.RaiseEvent();
            _lightManager.WaitingForLightRooms.Remove(gridPos);
        }
        
        DropPreview();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_previewObject == null) return;
        
        _previewObject.transform.Rotate(0, 90, 0);
        
        if (_isRoom)
        {
            Direction newDirection = (Direction)((int)(_previewRoom.Direction + 1) % (int)Direction.Count);
            MapManager.RotateRoom(_previewRoom, newDirection);
            ProcessRoom();
        }
    }
}