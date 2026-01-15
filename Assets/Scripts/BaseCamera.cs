using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

[RequireComponent(typeof(Camera))]
public class BaseCamera : MonoBehaviour
{
    public Camera HorizontalCamera;
    public Camera VerticalCamera;
    public Camera CornerCamera;
    public Camera MainCamera;
    public LayerMask PlaneLayer;

    private Camera _camera;
    private UniversalAdditionalCameraData _cameraData;
    private List<Camera> _stack;
    private Vector2Int _lastOffset;

    [Inject] private MapManager _mapManager;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _cameraData = _camera.GetUniversalAdditionalCameraData();
    }

    private void LateUpdate()
    {
        Ray ray = _camera.ScreenPointToRay(new(Screen.width / 2, Screen.height / 2));
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, PlaneLayer)) return;
        Vector3 mainCameraProjection = hit.point;
        
        Vector2Int offset = Vector2Int.zero;
        if (mainCameraProjection.x > _mapManager.MapCenter.x) offset.x = 1; else offset.x = -1;
        if (mainCameraProjection.z > _mapManager.MapCenter.z) offset.y = 1; else offset.y = -1;
        
        if (_lastOffset == offset) return;

        _cameraData.cameraStack.Clear();

        _stack = (offset.x, offset.y) switch
        {
            (-1, -1) => new() { MainCamera, HorizontalCamera, VerticalCamera, CornerCamera },
            (-1, 1) => new() { VerticalCamera, MainCamera, HorizontalCamera, CornerCamera },
            (1, -1) => new() { HorizontalCamera, MainCamera, VerticalCamera, CornerCamera },
            (1, 1) => new() { HorizontalCamera, VerticalCamera, MainCamera, CornerCamera },
            _ => _stack
        };

        _cameraData.cameraStack.AddRange(_stack);
        _lastOffset = offset;
    }
}