using System;
using Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PreviewManager : MonoBehaviour
{
    public Camera TargetCamera;
    public Color PreviewTint;
    public Color PreviewBlockTint;
    public float PreviewHeight;
    public Vector2 CellSizeSnapping;
    public LayerMask PlaneLayer;
    
    private GameObject _previewObject;
    private bool _isBlocked;

    [Inject] private EventBus _eventBus;

    private void Update()
    {
        if (_previewObject != null)
            UpdatePosition();
    }

    public void Preview(GameObject prefab)
    {
        if (_previewObject != null)
            DropPreview();
        
        SetupPreview(prefab);
    }

    public void DropPreview()
    {
        Destroy(_previewObject);
        _previewObject = null;
    }

    public void SetBlocked(bool isBlocked)
    {
        _isBlocked = isBlocked;
        SetTint(isBlocked ? PreviewBlockTint : PreviewTint);
    }

    private void SetupPreview(GameObject prefab)
    {
        _previewObject = Instantiate(prefab, transform);
        
        SetTint(PreviewTint);
        DisableCollision();
        UpdatePosition();
    }

    private void SetTint(Color color)
    {
        if (_previewObject == null) return;
        
        MeshRenderer[] meshes = _previewObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer previewMesh in meshes)
        {
            previewMesh.material = new(previewMesh.material)
            {
                color = color
            };
        }
    }
    
    private void DisableCollision()
    {
        if (_previewObject == null) return;
        
        Collider[] colliders = _previewObject.GetComponentsInChildren<Collider>();
        foreach (Collider previewCollider in colliders)
        {
            previewCollider.enabled = false;
        }
    }

    private void UpdatePosition()
    {
        Ray ray = TargetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, PlaneLayer)) return;
        Vector3 newPos = hit.point;
        
        newPos.y = PreviewHeight;
        if (CellSizeSnapping.x > 0)
            newPos.x = Mathf.Round(newPos.x / CellSizeSnapping.x) * CellSizeSnapping.x;
        if (CellSizeSnapping.y > 0)
            newPos.z = Mathf.Round(newPos.z / CellSizeSnapping.y) * CellSizeSnapping.y;

        if (newPos == _previewObject.transform.position) return;
        
        _previewObject.transform.position = newPos;
        _eventBus.PreviewMoved.RaiseEvent(_previewObject.transform.position);
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

        _eventBus.SubmitPlacing.RaiseEvent(_previewObject.transform.position);
        DropPreview();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_previewObject == null) return;
        
        _previewObject.transform.Rotate(0, 90, 0);
        _eventBus.PreviewRotated.RaiseEvent(_previewObject.transform.position);
    }
}