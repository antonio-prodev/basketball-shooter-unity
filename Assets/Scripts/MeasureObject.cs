using UnityEngine;

public class MeasureObject : MonoBehaviour
{
    [ContextMenu("Log Exact Size")]
    void LogSize()
    {
        // Get the MeshRenderer to calculate the visual bounds
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            Debug.Log($"{gameObject.name} Dimensions: " +
                      $"Width (X): {size.x:F2}m, " +
                      $"Height (Y): {size.y:F2}m, " +
                      $"Length (Z): {size.z:F2}m");
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on this object.");
        }
    }
}