using System.Collections.Generic;
using UnityEngine;
namespace Utilities
{
    public static class GOManipulator
    {
        public static bool HasComponent<T>(this GameObject go, out T component)
        {
            component = go.GetComponent<T>();
            return component != null;
        }
        
        public static void SetTint(this GameObject target, Color color)
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
        
        public static void ToggleCollision(this GameObject target, bool enabled)
        {
            if (target == null) return;
            
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            foreach (Collider previewCollider in colliders)
            {
                previewCollider.enabled = enabled;
            }
        }

        public static List<GameObject> FilterByLayer(this GameObject target, LayerMask layer)
        {
            List<GameObject> result = new();

            if ((layer.value & 1 << target.layer) != 0)
                result.Add(target);
            foreach (Transform child in target.transform)
            {
                result.AddRange(FilterByLayer(child.gameObject, layer));
            }
            
            return result;
        }
    }
}
