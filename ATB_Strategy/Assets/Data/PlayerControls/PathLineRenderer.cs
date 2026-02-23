using UnityEngine;
using System.Collections.Generic;

public class PathLineRenderer : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.35f, 0);

    public void Init()
    {
        _lineRenderer.enabled = false;
    }

    public void SetPathLine(List<Vector3> path)
    {
        if (path == null) return;

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, path[i] + _offset);
        }
    }

    public void UnsetPathLine()
    {
        _lineRenderer.enabled = false;
    }
}
