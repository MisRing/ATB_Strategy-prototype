using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PathLineRenderer : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineCutStep = 0.5f;
    [SerializeField] private float _raycastHeight = 0.25f;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.35f, 0);

    public void Init()
    {
        _lineRenderer.enabled = false;
    }

    public void SetPathLine(List<Vector3> path)
    {
        if (path == null) return;

        path = SetPathDetails(path);
        path = SmoothPath(path);

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, path[i] + _offset);
        }
    }

    private List<Vector3> SetPathDetails(List<Vector3> path)
    {
        List<Vector3> newPath = new List<Vector3>();

        newPath.Add(SetPointHeight(path[0]));

        for(int i = 0; i < path.Count - 1; i++)
        {
            if (Vector3.Distance(path[i], path[i + 1]) <= _lineCutStep)
            {
                newPath.Add(SetPointHeight(path[i + 1]));
                continue;
            }

            int newDotsCount = Mathf.FloorToInt(Vector3.Distance(path[i], path[i + 1]) / _lineCutStep);

            while(Vector3.Distance(newPath[newPath.Count - 1], path[i + 1]) >= _lineCutStep)
            {
                Vector3 p1 = newPath[newPath.Count - 1];
                Vector3 p2 = path[i + 1];
                float t = _lineCutStep / Vector3.Distance(p1, p2);
                Vector3 newDot = p1 + (p2 - p1) * t;
                newPath.Add(SetPointHeight(newDot));
            }

            newPath.Add(SetPointHeight(path[i + 1]));
        }

        return newPath;
    }

    private List<Vector3> SmoothPath(List<Vector3> path)
    {
        List<Vector3> smoothed = new List<Vector3>();
        smoothed.Add(path[0]);

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 prev = path[i - 1];
            Vector3 current = path[i];
            Vector3 next = path[i + 1];

            Vector3 smooth = (prev + current + next) / 3f;
            smoothed.Add(smooth);
        }

        smoothed.Add(path[path.Count - 1]);
        return smoothed;
    }

    private Vector3 SetPointHeight(Vector3 point)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(point, out hit, _raycastHeight, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return point;
    }

    public void UnsetPathLine()
    {
        _lineRenderer.enabled = false;
    }
}
