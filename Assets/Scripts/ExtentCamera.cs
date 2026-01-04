using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class ExtentCamera : MonoBehaviour
{
    public enum Orientation
    {
        Vertical,
        Horizontal,
        Corner
    }

    public Camera BaseCamera;
    public Orientation CameraOrientation;
    public LayerMask PlaneLayer;

    private Camera _camera;
    private Camera _mainCamera;
    private UniversalAdditionalCameraData _mainCameraData;
    private Transform _pivot;
    private Vector3 _mapSize;
    private Vector3 _lastOffset;
    
    [Inject] private MapManager _mapManager;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _mainCamera = Camera.main;
        _mainCameraData = _mainCamera.GetUniversalAdditionalCameraData();
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera missing!");
            return;
        }
        
        _pivot = _mainCamera.transform.parent;
        _mapSize = new(_mapManager.Width, 0f, _mapManager.Height);
    }

    private void LateUpdate()
    {
        Ray ray = _mainCamera.ScreenPointToRay(new(Screen.width / 2, Screen.height / 2));
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, PlaneLayer)) return;
        Vector3 mainCameraProjection = hit.point;
        
        Vector3 offset = _mapSize;

        switch (CameraOrientation)
        {
            case Orientation.Vertical:
                offset.x = 0;
                break;
            case Orientation.Horizontal:
                offset.z = 0;
                break;
            case Orientation.Corner:
                break;
        }

        if (mainCameraProjection.x > _mapManager.MapCenter.x) offset.x *= -1;
        if (mainCameraProjection.z > _mapManager.MapCenter.z) offset.z *= -1;

        Vector3 localOffset = _pivot.InverseTransformVector(offset);
        localOffset.z = transform.localPosition.z;

        transform.localPosition = localOffset;

        if (offset == _lastOffset) return;
        _lastOffset = offset;
        
        _mainCameraData.cameraStack.Remove(_camera);
        int baseIndex = _mainCameraData.cameraStack.FindIndex(x => x == BaseCamera);
        int targetIndex = baseIndex;
        if (offset.x < 0) targetIndex--; else targetIndex++;
        if (offset.z < 0) targetIndex--; else targetIndex++;
        targetIndex = Math.Clamp(targetIndex, 0, _mainCameraData.cameraStack.Count);
        _mainCameraData.cameraStack.Insert(targetIndex, _camera);
    }
}
