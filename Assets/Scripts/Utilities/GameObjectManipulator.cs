using UnityEngine;
namespace Utilities
{
    public static class GameObjectManipulator
    {
        public static void SetTint(GameObject target, Color color)
        {
            if (target == null) return;
            
            MeshRenderer[] meshes = target.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer previewMesh in meshes)
            {
                previewMesh.material = new(previewMesh.material)
                {
                    color = color
                };
            }
        }
        
        public static void ToggleCollision(GameObject target, bool enabled)
        {
            if (target == null) return;
            
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            foreach (Collider previewCollider in colliders)
            {
                previewCollider.enabled = enabled;
            }
        }
    }
}
