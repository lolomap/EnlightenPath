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
    public LayerMask PlaneLayer;

    private Camera _mainCamera;
    private Transform _pivot;
    private Vector3 _mapSize;
    
    [Inject] private MapManager _mapManager;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera missing!");
            return;
        }
        
        _pivot = _mainCamera.transform.parent;
        _mapSize = new(_mapManager.Width, _mapManager.Height);
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
                offset.y = 0;
                break;
            case Orientation.Corner:
                break;
        }

        if (mainCameraProjection.x > _mapManager.MapCenter.x) offset.x *= -1;
        if (mainCameraProjection.z > _mapManager.MapCenter.z) offset.y *= -1;

        Vector3 localOffset = _pivot.InverseTransformVector(offset);
        localOffset.z = transform.localPosition.z;

        transform.localPosition = localOffset;
    }
}
