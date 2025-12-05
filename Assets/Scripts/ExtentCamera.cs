using Data;
using UnityEngine;
using Zenject;

public class ExtentCamera : MonoBehaviour
{
    public enum Orientation
    {
        
        Vertical,
        Horizontal,
        Corner
    }

    public Orientation CameraOrientation;
    public MapManager Map;

    private Camera _mainCamera;
    private Transform _pivot;
    private Vector3 _mapSize;
    
    private Mesh _debugMesh;

    [Inject] private DungeonConfig _config;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera missing!");
            return;
        }
        
        _pivot = _mainCamera.transform.parent;
        _mapSize = new(_config.Width * Map.CellSize.x, _config.Height * Map.CellSize.y);
    }

    private void LateUpdate()
    {
        Vector3 offset = _mapSize;

        switch (CameraOrientation)
        {
            case Orientation.Vertical:
                offset.x = 0;
                break;
            case Orientation.Horizontal:
                offset.y = 0;
                break;
            case Orientation.Corner:
                break;
        }

        if (_mainCamera.transform.position.x > Map.MapCenter.x) offset.x *= -1;
        if (_mainCamera.transform.position.y > Map.MapCenter.y) offset.y *= -1;

        Vector3 localOffset = _pivot.InverseTransformVector(offset);
        localOffset.z = transform.localPosition.z;

        transform.localPosition = localOffset;
    }
}
