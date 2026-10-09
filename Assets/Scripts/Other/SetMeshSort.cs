using UnityEngine;

public class SetMeshSorting : MonoBehaviour
{
    public string sortingLayerName = "Ground";

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerName = sortingLayerName;
        }
    }
}