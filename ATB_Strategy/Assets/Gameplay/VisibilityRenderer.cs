using System.Collections.Generic;
using UnityEngine;

public class VisibilityRenderer : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField] private List<GameObject> _otherObjects = new List<GameObject>();
    
    private void OnEnable()
    {
        FogOfWarUtility.OnVisibilityChanged += UpdateVisibility;
    }
    
    private void OnDisable()
    {
        FogOfWarUtility.OnVisibilityChanged -= UpdateVisibility;
    }

    private void UpdateVisibility()
    {
        GridTile currentTile = GridParameters.LevelGrid.GetTileByWorldPos(transform.position);
        bool enable = currentTile == null ? false : currentTile.Visibility == TileVisibility.Visible;
        
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = enable;
        }
        
        foreach (SkinnedMeshRenderer skinnedMesh in _skinnedMeshRenderers)
        {
            skinnedMesh.enabled = enable;
        }

        foreach (GameObject obj in _otherObjects)
        {
            obj.SetActive(enable);
        }
    }
}
