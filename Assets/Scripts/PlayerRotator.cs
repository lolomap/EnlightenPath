using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotator : MonoBehaviour
{
    public Camera TargetCamera;
    public LayerMask PlaneLayer;
    
    private void Update()
    {
        Ray ray = TargetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, PlaneLayer)) return;
        Vector3 cursorWorldPos = hit.point;
        
        transform.LookAt(cursorWorldPos);
        transform.eulerAngles = new(0f, transform.eulerAngles.y, 0f);
    }
}